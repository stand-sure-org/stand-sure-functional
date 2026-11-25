# Design: Functional Core + Model-Usage Harness

**Version:** 1.7 (Diagnostic IDs; `Unit.Default` alias)
**Date:** 2026-11-25
**Changes:** v1.1 settled harness execution, baseline authority, wire format, telemetry
firing points, run mode. v1.2: no budget cap (5 runs per cell), Gemini via
`GEMINI_API_KEY`, local models pulled fresh, baseline authority confirmed by owner.
v1.3: the `StandSure.Functional` ActivitySource now starts a span in every method that
runs caller code (§Telemetry, "Spans"); offline sample apps to observe the traces.
v1.4: `Error` gets value equality over its contents and a `ToString` that honours the
`EmitErrorDetails` switch (§`Error`, "Equality and ToString"), from the 2026-11-24 review.
v1.5: delegate parameters are non-nullable; the never-throw guards stay (§"Null delegates").
v1.6: side-effect forms (`Match` with actions, parameterless `Bind` on `Result<Unit>`,
`TapError`) and the pass-through span rule (§`Unit`, "Side-effect forms"; issue #8).
v1.7: diagnostic ID ranges, and `Unit.Default` as a deprecated alias (§`Unit`, "Diagnostic IDs").
See `DECISION.md` for the why and the scope table.

Items marked **[builder's call]** were proposed by Claude during design and not
explicitly confirmed by the owner; change them freely if evidence says otherwise.

---

## 1. Intent

Give models a small, familiar, non-throwing vocabulary for the
validate → call dependency → adapt → return shape, measured against what models
actually write, with telemetry that feeds intervention research for free.

### Goals

- A model with no prior exposure, given a one-page API summary, writes correct code
  against the library on the first try for common tasks.
- Zero heap allocation for `Result<T>`/`Option<T>` creation and pattern matching
  when `T` is a value type and telemetry has no listener.
- Every failure is observable through BCL telemetry without the caller doing anything.
- Results survive serialization across service boundaries (the Stacked Services
  model: API → handler → validation → client → next service).

### Non-goals

- `Either<TLeft, TRight>`. C# 15 `union` replaces it; users declare their own unions.
- Category-theory surface (Kleisli, Functor/Monad interfaces, HKT emulation).
- Re-implementing what LINQ already has (`Chunk`, `Zip`, `DistinctBy`, `CountBy`,
  `AggregateBy`, `Index`, ...).
- netstandard / net10 support.
- Analyzers, generators, OTel and ASP.NET packages (later features; core must not
  preclude them).

### Anti-goals (would make this a failure even if it works)

- The library throws, or makes throwing easier than returning a failure.
- Becoming language-ext: a huge surface where models pick the wrong overload.
- Boxing on the hot path, or a `default` value that silently looks like success.
- Magic agents can't see (compiler-only generated code, reflection-driven behaviour).
- PII leaking into telemetry by default.

## 2. Constraints

- net11.0, C# 15 (default LangVersion). SDK 11.0.100 is installed and verified.
- Verified behaviour on SDK 11.0.100 (throwaway probes, 2026-11-19):
  - `public union Outcome<T>(T, Error);` compiles at default LangVersion; generic unions work.
  - Non-exhaustive switch over a union = **CS8509 warning**.
  - `default` of a compiler-generated union has `Value == null`.
  - Compiler-generated unions store `object Value` → **value-type cases box** (24 B for `int`).
  - A hand-written `[Union] readonly struct` implementing `IUnion` with `object? Value`,
    `bool HasValue`, and typed `bool TryGetValue(out TCase)` overloads gets the same
    `switch` syntax and implicit conversion from case constructors with **0 allocations**
    over 1,000 matches. Core types use this shape.
  - `System.Diagnostics.ActivitySourceFactory` (`Create(ActivitySourceOptions)`) exists
    in net11.0 ref assemblies, not net10. Config side lives in
    `Microsoft.Extensions.Diagnostics.Tracing` (`ITracingBuilder`, `AddListener`,
    `EnableTracing`, `ActivitySourceScopes.Global/Local`).
- Owner's C# conventions: braces always, early returns/guard clauses, one top-level
  type per file, never `Console.*` (ILogger + LoggerMessage source generator),
  secrets via `fnox`, commits `--no-gpg-sign`, never commit to `main`.
- Core package has **zero NuGet runtime dependencies** (BCL only).
- Warnings as errors; PublicApiAnalyzers (`PublicAPI.Shipped/Unshipped.txt`);
  BannedApiAnalyzers (ban `Console`).
- Root namespace / package id defined once (MSBuild property) so the eventual rename
  is a one-line change plus a mechanical namespace sweep.

## 3. Approaches considered (ordering)

| | Approach | Trade-off |
|---|---|---|
| **A (chosen)** | Harness + baseline first, then core API from evidence, then re-run with API summary | Few days before library code; names grounded in what models do; harness becomes a regression check and early intervention data |
| B | Core first, harness after | Fastest to code; renames later ripple into analyzers/generators/docs |
| C | Parallel | Core names lock before evidence arrives; B with coordination overhead |

Within the core, approaches for the union types:

| | Approach | Trade-off |
|---|---|---|
| **Hand-written `[Union]` structs (chosen)** | Full control, non-boxing, `TryGetValue` access | More code to write and test |
| `union` keyword | Least code | Boxes value types; `default` semantics not controllable |
| Classic struct + `Match` only (no union integration) | Works everywhere | Loses native `switch` exhaustiveness, the main guardrail |

## 4. Part 1 — Model-usage harness

### Purpose

Measure which names and shapes models reach for *naturally*, so the core API matches
them, then measure how well models use the API we ship.

### Shape

- Location: `harness/ModelHarness/` (console app, net11.0). Not packaged.
- Calls models over **direct HTTP** (owner prefers direct API usage over SDKs):
  Anthropic Messages API, Gemini API, Ollama's local HTTP API. Keys via `fnox`
  (initialized in this repo); Gemini may use the existing global `GEMINI_API_KEY`.
  **[builder's call]** if an official SDK is clearly simpler and current.
- Raw API, not agent CLIs, for v1: reproducible and isolates the model from harness
  tooling. `claude -p` / `agy` (works in this repo) as an "in-harness" condition is a
  later addition **[builder's call]**.
- Models: Claude Opus 5.5, Sonnet 5.5, Haiku 5.5 (`claude-opus-5-5`, `claude-sonnet-5-5`,
  `claude-haiku-5-5`); current Gemini Pro and Flash (builder looks up current IDs);
  local: pull two current coding-focused models that fit this machine's memory
  (owner: "we can pull any model"); skip the installed non-coder models. Model list is
  config, not code.
- **No budget cap** (owner).
- Two conditions:
  - **Baseline**: task + "Expected failures must not throw; return a value describing
    the failure. Write idiomatic modern C# 15 / .NET 11." No library mentioned.
  - **With-library**: same task + the one-page API summary (`docs/llm-api-summary.md`,
    produced in Part 2).
- 5 runs per (model, task, condition), default temperature.
- Each output is compiled against a small fixture project (task interfaces + stubs;
  in the with-library condition, also the core library).

### Measurement: static + one behavioural probe **[builder's call]**

- Static: compile result + Roslyn extract (below). This is the main signal for *names
  and shapes*, which is what the baseline exists to find.
- Behavioural: each task has one failure-injecting stub (returns not-found / throws /
  returns invalid input). The compiled output is invoked through the task's entry point
  in a **separate process with a timeout**, no network, temp working directory.
  Recorded: did it throw for the injected expected failure (yes/no), did it return
  without hanging. No per-task correctness suites in v1. Baseline outputs have no
  shared API, so only "threw / didn't throw" is compared across conditions.

### Run mode

Local tool, run by hand; produces reports for review. **Not a CI gate** in v1.
Comparisons use aggregate rates per model family across the 5 runs; a family/task
cell that regresses gets a note in the report, not a failure.

### Task set (12)

Each task gives the model a caller-defined interface for the dependency.

1. Validate an email + name, then create a user via `IUserStore`.
2. Get an entity by id; not found is an expected outcome.
3. Read an optional config value with a fallback.
4. Call a third-party client that throws; convert to a non-throwing outcome.
5. Chain three dependent steps; stop at the first failure.
6. Validate several fields and report *all* problems at once.
7. Map an outcome to an HTTP response in a minimal API endpoint.
8. Optional lookup in cache, fall back to store, then to a default.
9. Async call honouring a `CancellationToken`.
10. Process a list; collect successes and failures separately.
11. Add context to a failure as it passes up three layers.
12. A collection transform needing a sliding window / pairwise / scan.

### What is recorded (JSONL, one line per run)

model, task, condition, run, prompt hash, raw output, compile success + diagnostics,
and a Roslyn syntax-walk extract: type names declared/used, method names invoked,
`throw` count, `try/catch` count, ignored return values of outcome types,
switch-exhaustiveness diagnostics. Logs go to `harness/results/` (gitignored raw,
committed summary) and are Loki-ingestible (owner already ships logs to Loki).

### Outputs

- `harness/results/baseline-report.md`: frequency tables of names/shapes per model
  family; compile rates; common failure-handling idioms (tuple returns, `bool TryX(out)`,
  `Result` look-alikes, exceptions).
- Later `with-library-report.md`, same format, compared against baseline.

## 5. Part 2 — Core library

Working names: package `StandSure.Functional`, namespace `StandSure.Functional`.
Ship `global using StandSure.Functional;` via a `buildTransitive` props file so models
never type the namespace. **[builder's call]** on exact mechanism.

The API below is the **starting proposal**.

**What the baseline can change:** member *names*, member *set*, and the `Try` entry
point name. It does **not** reopen the type shapes (`Result<T>`, `Option<T>`, `Error`,
`Unit`, non-boxing unions, failure-first): the owner fixed those (confirmed 2026-11-19). If models mostly use
exceptions or `bool TryX(out)`, that informs the API summary and later analyzers,
not the core shape. Renames: Claude proposes them in the report with frequencies per
family; the owner approves. Members not listed are out unless the report justifies them.

**Parallel start:** `Unit`, `Error`, the union plumbing, `Try` internals, JSON and
telemetry do not depend on naming and can be built (TDD) while the baseline runs.

### Null delegates (v1.5, 2026-11-24 review)

- Every delegate parameter is non-nullable (`Func<T, TOut> map`, not `Func<T, TOut>? map`), so a
  caller with nullable enabled gets CS8625 or CS8604 at the call site, and an error under
  warnings-as-errors. The nullable annotations had switched that help off.
- The runtime guard stays for callers with nullable off: a null delegate becomes a
  `result.null_delegate` `Unexpected` failure whose message names the parameter and the method
  ("The delegate 'map' passed to Map was null."; the sync-delegate `*Async` overloads name
  themselves, see §Spans "One span per call"). `Option` combinators give None and `Match` gives
  `default`, as before; building the error records the usual `functional.failure` event, on the
  method's span.
- Guards are written `ResultErrors.IsMissing(x)`, not `x is null`. Nullable flow analysis (and
  Rider's "Expression is always false") treats `x is null` on a non-nullable parameter as dead
  code; the helper keeps the guard visible and says why it exists. Tried and rejected: JetBrains
  `[CanBeNull]` (Rider prefers the NRT annotation and still warns) and an `.editorconfig` rule that
  turns the inspection off (it would also hide genuinely redundant checks in `src/`).
- Values that may legitimately be null (`IEnumerable<T>? source`, the task a delegate returns,
  `Task<TOut>?`) keep their `?`.

### `Error` (sealed record, reference type)

- `ErrorKind Kind`: `Validation, NotFound, Conflict, Unauthorized, Forbidden,
  Unavailable, Unexpected`. Each maps to an HTTP status (mapping lives in the ASP.NET
  sibling package; core only documents it).
- `string Code`: open, dotted convention e.g. `user.email.invalid`.
- `string Message`.
- `IReadOnlyDictionary<string, string> Metadata` (string values keep JSON/AOT simple)
  **[builder's call]**.
- `Error? Inner`: wrapping chain (Go `%w`).
- `IReadOnlyList<Error> Errors`: children for multi-error validation (empty otherwise).
- Factories: `Error.Validation(code, message)`, `Error.NotFound(...)`, … one per kind;
  `Error.FromException(Exception)` → `Unexpected`, exception type/message in metadata;
  exception object kept in-process only, excluded from serialization **[builder's call]**.
- `Wrap(string code, string message)` returns a new Error with `this` as `Inner`.
- `Is(string code)` walks the `Inner` chain (Go `errors.Is`).
- `Error.Uninitialized`: what a `default` Result holds.
- Equality and ToString (v1.4, 2026-11-24 review). The compiler-generated record members
  compared `Metadata`, `Errors` and `Exception` by reference (FrozenDictionary and arrays
  don't override `Equals`), so two errors with the same metadata were unequal while two
  without any were equal only because they share the static empties. And `ToString()`
  printed `Message` and the `Exception` whatever the switch said.
  - `Equals` compares `Kind`, `Code` and `Message` (ordinal); `Metadata` as a set of
    key/value pairs (order ignored); `Inner` recursively; `Errors` in order, element by
    element; `Exception` by reference (two different exception objects are two different
    failures). `GetHashCode` uses `Kind`, `Code`, `Message`, the counts of `Metadata` and
    `Errors`, and `Inner`'s hash, so it agrees with `Equals` without hashing contents.
  - `Message` counts towards equality **[owner agreed]**: "same code, different text" being
    equal would surprise more than it helps.
  - `ToString()` (via `PrintMembers`) prints `Kind`, `Code`, the `Metadata` **keys**, `Inner`
    and the number of `Errors`. `Message` and metadata values are printed only when the
    `EmitErrorDetails` switch is on, the same rule as telemetry, because `{error}` in a log
    line leaves the process just as an exported span does. The `Exception` is never printed;
    its type is, when present. The properties themselves are unchanged: `error.Message` is
    always there for code that wants it.
- JSON **[builder's call, settled after builder check]**:
  - `Error` is serialized via a source-generated `JsonSerializerContext` in core.
    Field names line up with ProblemDetails so the ASP.NET package maps 1:1:
    `code`→`type`, `message`→`title`, `kind`→`status` (via the documented mapping),
    `metadata`/`errors`/`inner` → extensions.
  - `Result<T>` / `Option<T>` use a `JsonConverterFactory` in core. Wire shape is plain
    JSON readable by non-.NET services: `{"value": …}` or `{"error": {…}}`;
    Option: `{"value": …}` or `{}`. The converter delegates `T` to the caller's options/
    context, so AOT callers register `Result<TheirType>` in their own context. The later
    generators emit those registrations. A `null` value is read through that metadata too, after
    `error` has had its chance to win (2026-11-24 review #9): `Result<int>` from `{"value":null}`
    throws `JsonException` like plain STJ, `Result<int?>` is `Success(null)`. Reference types get
    `Success(null)` either way, since generic arguments carry no runtime nullability. Option keeps
    `{"value":null}` as None.

### `Result<T>` (`[Union] readonly struct`, cases `Error` | `T`)

- Equality (2026-11-24 review): `IEquatable<Result<T>>`, `==`/`!=`. Successes compare values with
  `EqualityComparer<T>.Default`, failures compare `Error`s; `default` equals a failure holding `Error.Uninitialized`.
- Construction: implicit from `T` and from `Error`; `Result.Success(value)`,
  `Result.Failure<T>(error)`, `Result.Success()` → `Result<Unit>`. No non-generic `Result`
  struct; `Result<Unit>` is the "void" form **[builder's call]**.
- `default(Result<T>)` is a failure holding `Error.Uninitialized`.
- `bool IsSuccess`, `bool IsFailure`.
- Union members: `object? Value`, `HasValue`, `TryGetValue(out Error)`,
  `TryGetValue(out T)` with `[NotNullWhen(true)]`.
- `Deconstruct(out T? value, out Error? error)`: Go style
  `var (user, err) = result; if (err is not null) { return err; }`.
- `Match<TOut>(Func<Error, TOut> onFailure, Func<T, TOut> onSuccess)`. Failure first.
- `Map`, `Bind`, `MapError`, `Ensure(Func<T,bool> predicate, Error error)`,
  `Tap(Action<T>)`, `GetValueOrDefault(T fallback)`.
- **No** throwing `.Value`/`.Error` accessors.
- Async: extension methods on `Task<Result<T>>`: `MapAsync`, `BindAsync`, `MatchAsync`,
  `EnsureAsync`; one overload per sync/async lambda combination actually needed by the
  harness tasks, no more.

### `Option<T>` (`[Union] readonly struct`, cases `None` | `T`)

- `None` is a singleton marker type so `switch { None => …, T x => … }` works.
- Equality (2026-11-24 review): `IEquatable<Option<T>>`, `==`/`!=`. Every None equals every None;
  Somes compare values with `EqualityComparer<T>.Default`.
- Comparing either union with a raw case value converts it first (`Some(5) == 5` and
  `Success(5) == 5` are true), while `Equals((object)5)` is false: it sees a boxed `int`.
  `== null` does not compile (CS0037); both unions are non-nullable structs.
- Implicit from `T` (null → None); `Option.Some(x)`, `Option<T>.None`.
- `default` is None.
- `Match(onNone, onSome)`, `Map`, `Bind`, `Filter`, `GetValueOrDefault`,
  `ToResult(Error ifNone)`, `TryGetValue(out T)`.

### `Unit`

`readonly struct`, `Unit.Value`, equality/comparison, `ToString() => "()"`.
README credits language-ext (Paul Louth, MIT) as inspiration.

`Unit.Default` is a permanent, deprecated alias of `Unit.Value` (owner, 2026-11-25). In the
harness, every devstral answer guessed `Default`, which is also the name TryHelpers and language-ext use.
It is `[Obsolete(DiagnosticId = "SSF0001")]` at warning level and `[EditorBrowsable(Never)]`. Guessed code
compiles, and the warning names `Unit.Value`; without the alias, CS0117 gives no hint. It is never removed,
since removing it would bring back the hintless error. `docs/llm-api-summary.md` names only `Unit.Value`.

#### Diagnostic IDs (v1.7)

`[Obsolete]`/`[Experimental]` diagnostic IDs and future analyzer rules share one prefix, `SSF`, because
consumers suppress both through the same `NoWarn` and `.editorconfig` entries. Each ID has a page
`docs/diagnostics/SSFnnnn.md`, linked through `UrlFormat`. Error-level misuse (`Try.Run(Func<Task>)`) stays on the
compiler's own CS0619 with no ID, because it should never be suppressed.

| Range | Use |
|---|---|
| `SSF0001`–`SSF0999` | Attribute diagnostics in the core package |
| `SSF1000`+ | Rules from the analyzer package (DECISION sequencing step 3) |

| ID | Member | Severity |
|---|---|---|
| `SSF0001` | `Unit.Default`: use `Unit.Value` | Warning |

#### Side-effect forms (v1.6, issue #8)

`void` has no place in a chain of results, so the core combinators take `Func` only.
Compatibility with `Action` and parameterless steps lives in extension classes
(`ResultUnitExtensions`, `OptionUnitExtensions`) that adapt to `Unit` and forward to the
ordinary combinator. This follows the owner's earlier TryHelpers pattern (`Action`
overloads forwarding through `AsFunc()` to the `Func<_, Unit>` form), written from scratch.

| Member | Forwards to |
|---|---|
| `Result<T>.Match(Action<Error> onFailure, Action<T> onSuccess) -> Unit` | `Match<Unit>` |
| `Option<T>.Match(Action onNone, Action<T> onSome) -> Unit` | `Match<Unit>` |
| `Result<Unit>.Bind(Func<Result<TOut>> bind)` | `Bind(Func<Unit, Result<TOut>>)` |

- **Why:** in the with-library harness runs, all `CS0411` failures with `Match`/`Bind` were side-effect
  code: `Match` as a statement with `List.Add` lambdas (11, Gemini), and `Bind(() => ...)`
  after a `Result<Unit>` step (14, devstral). Declaring the result type would not help:
  C# method type inference ignores the assignment target.
- **Returns `Unit`, not `void`,** so the call works as a statement and still composes as
  an expression.
- **Overload resolution.** The instance `Match<TOut>` is tried first; extensions are
  considered only when it is not applicable, which is exactly the void-lambda case. A
  lambda that returns a value (for example `HashSet.Add`) keeps binding to `Match<TOut>`.
- **Adapters** (`UnitAdapters`, internal) are delegates bound to the caller's delegate (a
  static method closed over its first argument), not capturing lambdas. A `null` delegate
  stays `null`, so the target's null-delegate rules apply and name the caller's parameter.
  Measured (BenchmarkDotNet, .NET 11, no listener): `Match` with actions 0 B, against 48 B
  for a capturing-lambda adapter (one-off comparison run, not kept). `DOTNET_JitDisasm`
  on the Tier1 benchmark shows why: `Match` (try/finally included) is inlined into the
  caller and the two adapter delegates live in the stack frame; `[AggressiveInlining]`
  plays no part. A capturing lambda still heap-allocates its closure object. This
  depends on the JIT inlining at each call site, so 0 B is observed, not guaranteed, and
  no allocation test pins it.
- `TapError(Action<Error>)` is an ordinary `Result<T>` method mirroring `Tap`, with its own
  span. A `null` action turns a failure into a `result.null_delegate` failure, as `MapError` does.

### `Try`

- `Try.Run<T>(Func<T>)` → `Result<T>`; `Try.Run(Action)` → `Result<Unit>`.
- `Try.RunAsync<T>(Func<CancellationToken, Task<T>>, CancellationToken)` and the
  `Func<CancellationToken, Task>` form.
- Catches `Exception` → `Error.FromException`. Never catches `OperationCanceledException`;
  also lets `OutOfMemoryException` propagate **[builder's call]**.
- Naming (`Try.Run` vs `TryHelpers.Try` vs `Result.Try`) is a prime harness question.

### Collection helpers over results/options

High value for workflows; included in v1 unless the report says otherwise:
`Combine` (many `Result<T>` → `Result<IReadOnlyList<T>>`, accumulating *all* errors
into one parent), `Traverse`, `Partition` (successes, failures),
`Choose` (Options → values).

The parent's kind (owner decision, 2026-11-24 review): the children's kind when they all agree; for a mix,
`Unexpected` if any child is, else `Unavailable` if any child is, else `Validation`. A batch of outages must not
reach a caller's kind switch as a 400.

### functools / itertools / toolz candidates

Inclusion decided by the baseline report (task 12 and incidental usage), not here:
`Pipe`, `Compose`, partial application, `Memoize`, `Scan`/accumulate, `Pairwise`,
`Window`, `Interleave`, `Cycle`, `Product`, `Combinations`, `Permutations`,
consecutive `GroupBy`, `Juxt`, `Merge` (dictionaries).
Exclude anything LINQ already provides.

### Telemetry (core)

- One static `ActivitySource` and one static `Meter`, both named `StandSure.Functional`
  (constant, single place).
- **Fires on `Error` construction only** (factories, `FromException`, `Wrap`), never on
  conversion of an existing `Error` into a `Result<T>`, so passing a failure up three
  layers counts once. If `Activity.Current is not null`, add event `functional.failure`
  with tags `error.kind`, `error.code`. On `Wrap`: `functional.failure.wrapped` with
  inner and outer codes (not counted as a new failure).
- Counter `functional.failures` tagged `error.kind`, `error.code`, incremented once per
  root failure (not on `Wrap`).
- Correlation (agent, session, run): **not** core tags. The span's trace context plus
  W3C baggage set by the framework/harness carry it; listeners read baggage. Keeps the
  core telemetry contract small and stable.
- **Messages and metadata are not emitted by default** (PII); opt-in switch
  **[builder's call]** on mechanism (an `AppContext` switch keeps core dependency-free).
- With no listener on `StandSure.Functional`, success paths emit nothing: the cost is
  one listener check. With a listener, success paths emit spans (below).

#### Spans (v1.3)

Owner confirmed 2026-11-20: the sync combinators are in scope.

Before v1.3 the `ActivitySource` existed but never started an activity, so subscribing
to it with `AddSource("StandSure.Functional")` gave nothing.

**Rule.** Every public method that runs code the library did not write (a caller's
delegate) starts a span on its first line, using the default name (the method name):

```csharp
using var activity = FunctionalDiagnostics.Source.StartActivity();
```

- **One source**, `StandSure.Functional`. There are no per-area sources and no
  enable-all switch: subscribing is the opt-in.
- **Span boundaries.** The span starts on the first line and includes awaiting an
  upstream `Task<Result<T>>`. Finer spans come only from extracting a method that has
  its own span.
- **No status and no tags.** Our spans never set `Error` status, and never mark
  exceptions. Our code did not fail. A failed result is handled upstream by whoever
  called us, and a failing delegate owns its own instrumentation. Marking our span would
  also make every span above it in the call tree read as failed.
- **Error recording is unchanged.** `FunctionalDiagnostics` and the `functional.failure*`
  events and counter are not modified.
- **No span on equality.** `Equals`, `GetHashCode`, `==` and `!=` on `Result<T>`, `Option<T>`
  and `Error` call `T`'s equality, but they run on every `HashSet`/`Dictionary` probe, so
  they start no span (review on PR #30, 2026-11-24).
- **One span per call (owner, 2026-11-24; trace review option 3).** The sync-delegate
  `MapAsync`, `BindAsync`, `MatchAsync` and both `EnsureAsync` overloads run the
  combinator's logic through an internal core (`MapCore`, `BindCore`, `MatchCore`,
  `EnsureCore`, `EnsureLazyCore`) that starts no span, so they no longer nest `Map`
  inside `MapAsync`. The core takes the public method's name, so a null delegate's
  message names the method the caller wrote.
- **Pass-through extensions start no span (owner, 2026-11-25).** An extension that only
  adapts arguments and forwards to the public method of the same name (the side-effect
  forms in §`Unit`) starts no span; the target's span covers the call and has the same
  name, because `StartActivity()` names the span with `[CallerMemberName]`.

**Invariant.** With nothing subscribed to `StandSure.Functional`, behaviour is the same
as v1.2, and all allocation guarantees hold.

**Known consequence (to observe, not yet settled).** While our span is open it is
`Activity.Current`. A `functional.failure` event raised inside the caller's delegate
(including `Try` converting an exception) therefore attaches to our span rather than to
the caller's span. The sample apps exist to show whether this reads well in a real trace.

**Members in scope.**

| Area | Members |
|---|---|
| `Try` | `Run` (both), `RunAsync` (both) |
| Async (`ResultTaskExtensions`) | `BindAsync` ×2, `MapAsync` ×2, `MatchAsync` ×2, `EnsureAsync` ×4 |
| Sync `Result<T>` | `Match`, `Map`, `Bind`, `MapError`, `Ensure` ×3, `Tap`, `TapError` |
| Sync `Option<T>` | `Match`, `Map`, `Bind`, `Filter`, `ToResult` (lazy) |
| Collections | `Traverse` |

**Excluded: iterators.** `Choose`, `Scan` and `Pairwise` use `yield`. In an iterator the
first-line `using` runs at the first `MoveNext`, stays open until the enumerator is
disposed, and stays `Activity.Current` while the caller's `foreach` body runs. That
would change the caller's own tracing. Iterators get no span; if one is wanted later, it
goes on an extracted method around the single delegate call.

Methods that only return a task they were given, without awaiting, get no span.

### Performance and consumer hints

- `readonly struct`, `[MethodImpl(AggressiveInlining)]` on trivial members,
  `[StackTraceHidden]`, `[DebuggerStepThrough]`, `[DebuggerDisplay]`.
- `[OverloadResolutionPriority]` where an overload pair is unavoidable, to steer
  both the compiler and models toward the preferred one.
- `[CallerArgumentExpression]` on `Ensure` to auto-fill error messages. Kept, but hidden from IntelliSense
  (`[EditorBrowsable(Never)]`, 2026-11-24 review): the message is source code and can reach users.
- Nullable annotations everywhere (`[NotNullWhen]`, `[MaybeNull]`).
- Benchmarks (BenchmarkDotNet, `benchmarks/` project) asserting 0 allocations for
  value-type create/match/map with no listener.

### Repo layout

```
src/StandSure.Functional/
tests/StandSure.Functional.Tests/     xUnit + AwesomeAssertions
tests/StandSure.Functional.ApprovalTests/  public API snapshot
benchmarks/StandSure.Functional.Benchmarks/
harness/ModelHarness/  harness/fixtures/  harness/results/
docs/llm-api-summary.md               one page, the with-library prompt
.specify/features/functional-core/
```

### Testing

- TDD, red-green-refactor, every public member.
- Union behaviour tests: exhaustive switch compiles without discard; `default` is failure/None.
- Allocation tests via `GC.GetAllocatedBytesForCurrentThread` for the hot paths
  (benchmarks confirm).
- Telemetry tests with an in-test `ActivityListener` / `MeterListener`.
- Span tests (v1.3), in the existing `TelemetryCollection` (`DisableParallelization =
  true`), which runs alone, so a listener's spans never allocate inside a concurrently
  running allocation test:
  - For each member in scope: a span named after the method, a child of the test's
    activity, with no status and no tags.
  - The iterators produce no span.
  - The allocation tests still report 0 B with no listener.
  - Existing tests that assert failure events on the test's own activity (for example
    `Try` converting an exception) will now see those events on our span. Each one is
    updated to look on the span and to say so, rather than being silenced.
- Sample apps (`samples/`, v1.3): small console apps that fail in realistic ways
  (third-party call throws, validation chain fails, async chain with an upstream
  failure). They export over OTLP to the local `otelcol-contrib` collector
  (`localhost:4317`, or `OTEL_EXPORTER_OTLP_ENDPOINT`), never to a remote service, and
  are not run by `dotnet test`. Output through `ILogger` (the `Console` ban applies).
  Their output is how the "known consequence" above gets judged; see
  `reviews/spans-traces-2026-11-20.md`.
- JSON round-trip tests for `Error` and `Result<T>`.
- `Try` tests: exceptions become `Unexpected`; `OperationCanceledException` propagates.

## 6. Success criteria

- Baseline report exists and every core API name is either justified by it or marked
  as a deliberate deviation with reason.
- With-library run: compile success and "no throw for expected failure" rates beat
  baseline for every model family; any task where a family does worse gets an issue.
- Core: all tests green, 0-allocation benchmarks pass, public API snapshot approved.

## 7. Left to the builder

Exact harness prompt wording; fixture design; Roslyn extraction details; report
formatting; benchmark setup; CI workflow (owner's Oui DELIVER reusable workflow
pattern via the `oui-deliver-setup` skill); pre-commit hooks (`pre-commit-setup` skill);
everything marked **[builder's call]**.

## 8. Later features (must not be precluded)

- Analyzers: ignored `Result`, unhandled failure, CS8509 → error, misuse of `ValueTask`.
- Generator CLI: partial classes for handlers/clients, `ValueTask` + DI telemetry,
  per-method typed error unions (`union FindUserError(NotFound, Banned)`), which will
  need a `Result<T, TError>` or equivalent; core `Error` design must leave room.
- Sibling packages: OTel registration (`AddSource`, `EnableTracing` rules),
  ASP.NET `Result` ↔ ProblemDetails mapping, client-side ProblemDetails → `Result`.
