# Design: Functional Core + Model-Usage Harness

**Version:** 1.2 (Owner answers to builder check)
**Date:** 2026-11-19
**Changes:** v1.1 settled harness execution, baseline authority, wire format, telemetry
firing points, run mode. v1.2: no budget cap (5 runs per cell), Gemini via
`GEMINI_API_KEY`, local models pulled fresh, baseline authority confirmed by owner.
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
- JSON **[builder's call, settled after builder check]**:
  - `Error` is serialized via a source-generated `JsonSerializerContext` in core.
    Field names line up with ProblemDetails so the ASP.NET package maps 1:1:
    `code`→`type`, `message`→`title`, `kind`→`status` (via the documented mapping),
    `metadata`/`errors`/`inner` → extensions.
  - `Result<T>` / `Option<T>` use a `JsonConverterFactory` in core. Wire shape is plain
    JSON readable by non-.NET services: `{"value": …}` or `{"error": {…}}`;
    Option: `{"value": …}` or `{}`. The converter delegates `T` to the caller's options/
    context, so AOT callers register `Result<TheirType>` in their own context. The later
    generators emit those registrations.

### `Result<T>` (`[Union] readonly struct`, cases `Error` | `T`)

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
- Implicit from `T` (null → None); `Option.Some(x)`, `Option<T>.None`.
- `default` is None.
- `Match(onNone, onSome)`, `Map`, `Bind`, `Filter`, `GetValueOrDefault`,
  `ToResult(Error ifNone)`, `TryGetValue(out T)`.

### `Unit`

`readonly struct`, `Unit.Value`, equality/comparison, `ToString() => "()"`.
README credits language-ext (Paul Louth, MIT) as inspiration.

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
- No telemetry on success paths. Cost when no listener ≈ one null check / `Enabled` check.

### Performance and consumer hints

- `readonly struct`, `[MethodImpl(AggressiveInlining)]` on trivial members,
  `[StackTraceHidden]`, `[DebuggerStepThrough]`, `[DebuggerDisplay]`.
- `[OverloadResolutionPriority]` where an overload pair is unavoidable, to steer
  both the compiler and models toward the preferred one.
- `[CallerArgumentExpression]` on `Ensure` to auto-fill error messages.
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
