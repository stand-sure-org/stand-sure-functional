# Functional Core + Model-Usage Harness Implementation Plan

**Version:** 1.1 (Post-review)
**Date:** 2026-11-19
**Changes:** Union probe results folded in (exhaustiveness verified; `Result<Error>` ambiguity; `None` name clash); Review Focus task refs fixed; harness fixture/isolation gaps closed; `Try` AggregateException handling; CLAUDE.md added to Task 1.

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Build the BCL-only `StandSure.Functional` core (`Unit`, `Error`, `Result<T>`, `Option<T>`, `Try`, telemetry, JSON) test-first, plus a harness that measures which API shapes models naturally write, and use the harness's baseline to fix the core's combinator names.

**Architecture:** Three parts. **Part A** builds the name-independent core foundations. **Part B** builds the harness and runs the baseline; it can run in parallel with Part A. **Part C** waits for the owner to approve the baseline report, then adds the combinators, collection helpers and API summary, and runs the with-library condition. Core unions are hand-written `[Union] readonly struct`s with typed `TryGetValue` (non-boxing).

**Tech Stack:** .NET 11 / C# 15, xUnit v3, AwesomeAssertions, Microsoft.CodeAnalysis.PublicApiAnalyzers, BannedApiAnalyzers, Roslyn (`Microsoft.CodeAnalysis.CSharp`, harness only), BenchmarkDotNet, System.Text.Json source generation, `System.Diagnostics` `ActivitySource`/`Meter`.

**Spec:** `.specify/features/functional-core/DESIGN.md` (v1.2) and `DECISION.md`. Read both before starting any task.

## Global Constraints

- `TargetFramework` `net11.0` only; default LangVersion (C# 15); SDK pinned via `global.json` to `11.0.100`, `rollForward: latestFeature`.
- Core package `StandSure.Functional`: **zero NuGet runtime dependencies** (analyzers are `PrivateAssets=all`).
- Root namespace and package id come from one MSBuild property, `FunctionalRootName` = `StandSure.Functional`, in `Directory.Build.props`.
- `Nullable` enabled, `TreatWarningsAsErrors` true in all configurations, `GenerateDocumentationFile` true.
- Ban `System.Console` via `BannedSymbols.txt`. Use ILogger with LoggerMessage source generator where logging is needed (harness only).
- C# style: braces always, early returns and guard clauses, one top-level type per file.
- The library never throws from its own APIs. `Try` never catches `OperationCanceledException` (including subclasses) or `OutOfMemoryException`.
- Failure/absence comes first in every two-branch signature: `Match(onFailure, onSuccess)`, `Match(onNone, onSome)`.
- No throwing `.Value` or `.Error` accessors on `Result<T>`.
- Telemetry source and meter name: `StandSure.Functional`. Event names `functional.failure` and `functional.failure.wrapped`; counter `functional.failures`; tags `error.kind` and `error.code`. Messages and metadata are emitted only when the `AppContext` switch `StandSure.Functional.EmitErrorDetails` is on.
- Tests: xUnit v3 + AwesomeAssertions (never FluentAssertions). TDD: red, green, refactor.
- Package versions: latest stable at implementation time. Stop and flag any package last updated more than 2 years ago.
- Secrets via `fnox` (repo has `fnox.toml`). Gemini may use the global `GEMINI_API_KEY`. Never commit keys.
- Commits use `--no-gpg-sign` and go on a feature branch, never `main`. In this repo the commit hook commits as `stand-sure-ai[bot]` **and pushes**.
- Do not stage or commit the owner's pending setup files (`.agents/`, `.claude/`, `.entire/`, `fnox.toml`) unless the owner says so. Stage explicit paths only.

## Review Focus

1. **`default` values everywhere.** A `default(Result<T>)` or `default(Option<T>)` (from array allocation, an unset field or `new T[n]`) behaves as failure `Error.Uninitialized` / None in `IsSuccess`, `TryGetValue`, `Deconstruct`, `switch`, `Match` and JSON, and never throws. → Tests in Tasks 3, 4, 7.
2. **`T` that overlaps `Error` or `object`.** For `Result<object>` and `Result<Error>`, success or failure is decided by the stored tag, never by the value's runtime type. `Result.Ok<object>(someError).IsSuccess` is true; implicit `Result<object> r = someError` is a failure (most specific conversion). Implicit `Error` → `Result<Error>` is a compile error (CS0457, verified) — acceptable; `Result.Ok(err)` is the way to make a success. → Task 3.
3. **Cancellation and sync throws in `RunAsync`.** `TaskCanceledException`, an OCE thrown synchronously before the task is returned, and an OCE wrapped in `AggregateException` from `.Wait()`-style code all propagate as OCE. A non-OCE thrown synchronously by the delegate becomes a failure, not an escaped exception. → Task 5.
4. **Malformed or partial JSON.** For `Result<T>`, both `{}` and `{"value":null,"error":null}` deserialize to failure `Error.Uninitialized`. When both `value` and `error` are present, `error` wins. Unknown properties are ignored. → Task 7.
5. **Messy model output in the harness.** Output with no code fence, with several fences, or with code that never terminates is recorded as `extract_failed`, `concatenated` or `timeout` respectively. It never crashes the run or hangs it past the per-probe timeout. → Tasks 11, 13.

---

## Part A — Core foundations (independent of naming)

### Task 1: Solution scaffold + `Unit`

**Files:**
- Create: `CLAUDE.md` (project guide for agents: commands, layout, Global Constraints of this plan, pointer to `.specify/features/functional-core/`), `global.json`, `Directory.Build.props`, `Directory.Packages.props`, `BannedSymbols.txt`, `.gitignore`, `.editorconfig`, `StandSure.Functional.slnx`
- Create: `src/StandSure.Functional/StandSure.Functional.csproj`, `PublicAPI.Shipped.txt` (empty), `PublicAPI.Unshipped.txt`, `Unit.cs`
- Create: `tests/StandSure.Functional.Tests/StandSure.Functional.Tests.csproj`, `UnitTests.cs`

**Interfaces:**
- Produces: `CLAUDE.md`; `public readonly struct Unit : IEquatable<Unit>, IComparable<Unit>` with `public static readonly Unit Value`, `==`/`!=`, `CompareTo` always returns 0, `GetHashCode` returns 0, `ToString()` returns `"()"`.

- [ ] **Step 1:** Create the scaffold files to satisfy Global Constraints. The test project references xunit.v3 and AwesomeAssertions through central package management.
- [ ] **Step 2: Write failing tests** in `UnitTests.cs`:
  - `Value_EqualsDefault`: `Unit.Value.Should().Be(default(Unit))`
  - `ToString_IsEmptyTuple`: `Unit.Value.ToString().Should().Be("()")`
  - `CompareTo_IsAlwaysZero`
  - `Operators_AreConsistent`: `(Unit.Value == default).Should().BeTrue()` and `!=` is false
- [ ] **Step 3:** Run `dotnet test`. Expected: build FAIL, `Unit` not found.
- [ ] **Step 4:** Implement `Unit` in `src/StandSure.Functional/Unit.cs`, with an XML doc crediting language-ext (Paul Louth, MIT) as inspiration, and add its members to `PublicAPI.Unshipped.txt`.
- [ ] **Step 5:** Run `dotnet build -c Release && dotnet test`. Expected: 0 warnings, all tests pass.
- [ ] **Step 6:** Commit: `feat: solution scaffold and Unit`.

### Task 2: `ErrorKind` + `Error`

**Files:**
- Create: `src/StandSure.Functional/ErrorKind.cs`, `src/StandSure.Functional/Error.cs`
- Test: `tests/StandSure.Functional.Tests/ErrorTests.cs`

**Interfaces:**
- Produces:
  - `public enum ErrorKind { Validation, NotFound, Conflict, Unauthorized, Forbidden, Unavailable, Unexpected }`
  - `public sealed record Error` with properties `ErrorKind Kind`, `string Code`, `string Message`, `IReadOnlyDictionary<string,string> Metadata` (never null), `Error? Inner`, `IReadOnlyList<Error> Errors` (never null), and `[JsonIgnore] Exception? Exception` (kept in-process only).
  - One factory per kind: `public static Error Validation(string code, string message, IReadOnlyDictionary<string,string>? metadata = null)`, and the same for `NotFound`, `Conflict`, `Unauthorized`, `Forbidden`, `Unavailable` and `Unexpected`.
  - `public static Error Validation(string code, string message, IEnumerable<Error> errors)`: a parent that holds several validation errors.
  - `public static Error FromException(Exception exception)`: kind `Unexpected`, code `exception.unhandled`, metadata keys `exception.type` and `exception.message`.
  - `public Error Wrap(string code, string message)`: returns a new error of the same `Kind` with `Inner = this`.
  - `public bool Is(string code)`: true if this error or any error in its `Inner` chain has that code.
  - `public static Error Uninitialized { get; }`: kind `Unexpected`, code `result.uninitialized`.

- [ ] **Step 1: Write failing tests:**
  - `Factory_SetsKindCodeMessage` (one test per kind, `[Theory]`)
  - `Metadata_DefaultsToEmpty`
  - `Errors_DefaultsToEmpty`
  - `Wrap_KeepsKind_SetsInner`
  - `Is_FindsCodeThreeLevelsDeep`
  - `Is_FalseForUnrelatedCode`
  - `FromException_MapsTypeAndMessage`: `ex.Exception.Should().BeSameAs(original)`
  - `Uninitialized_HasExpectedCode`
  - `ValidationParent_HoldsChildren`
- [ ] **Step 2:** Run `dotnet test --filter ErrorTests`. Expected: FAIL.
- [ ] **Step 3:** Implement `ErrorKind` and `Error`, and update `PublicAPI.Unshipped.txt`.
- [ ] **Step 4:** Run the tests. Expected: PASS.
- [ ] **Step 5:** Commit: `feat: Error and ErrorKind`.

### Task 3: `Result<T>` union core

**Files:**
- Create: `src/StandSure.Functional/Result.T.cs` (type `Result<T>`), `src/StandSure.Functional/Result.cs` (static factory class `Result`)
- Test: `tests/StandSure.Functional.Tests/ResultUnionTests.cs`, `tests/StandSure.Functional.Tests/ResultAllocationTests.cs`

**Interfaces:**
- Consumes: `Error`, `Error.Uninitialized`, `Unit` (Tasks 1–2).
- Produces:
  - `[Union] public readonly struct Result<T> : IUnion`, built from `T value` and `Error error` constructors. Implicit conversions from `T` and from `Error`. Fields: one `T`, one `Error?`, and a `byte` tag (0 = default, 1 = ok, 2 = fail).
  - `bool IsSuccess`, `bool IsFailure`. Tag 0 counts as failure.
  - `object? Value`: returns the `Error.Uninitialized` instance when the tag is 0.
  - `bool HasValue`: always true. (Verified: with `HasValue => true` and tag-0 routed to the `Error` case, `switch { Error e => …, int i => … }` is exhaustive under warnings-as-errors and `default` takes the `Error` arm.)
  - `bool TryGetValue([NotNullWhen(true)] out Error? error)` and `bool TryGetValue([MaybeNullWhen(false)] out T value)`.
  - `void Deconstruct(out T? value, out Error? error)`.
  - Static class `Result` with `Ok<T>(T value)`, `Fail<T>(Error error)`, `Ok()` → `Result<Unit>`.

- [ ] **Step 1: Write failing tests:**
  - `ImplicitFromValue_IsSuccess`
  - `ImplicitFromError_IsFailure`
  - `Default_IsFailureUninitialized`: `default(Result<int>).TryGetValue(out Error? e)` returns true and `e.Should().BeSameAs(Error.Uninitialized)`; `Deconstruct` gives `(0, Error.Uninitialized)`
  - `DefaultArrayElement_IsFailure`: `new Result<int>[3][1].IsFailure.Should().BeTrue()`
  - `Switch_IsExhaustiveWithoutDiscard`: a test-local `static string Describe(Result<int> r) => r switch { Error e => e.Code, int i => i.ToString() };`. It compiles under warnings-as-errors and returns the expected strings.
  - `Deconstruct_GoStyle_EarlyReturn`
  - `ResultOfObject_TagWinsOverRuntimeType`: `Result.Ok<object>(Error.NotFound("x","y")).IsSuccess.Should().BeTrue()`, and `switch { Error e => …, object o => … }` takes the `object` arm
  - `ResultOfObject_ImplicitFromError_IsFailure`: `Result<object> r = Error.NotFound("x","y"); r.IsFailure.Should().BeTrue()`
  - `ResultOfError_OkIsSuccess`: `Result.Ok(Error.NotFound("x","y")).IsSuccess.Should().BeTrue()` (inside generic `Ok<T>` the `T` constructor binds; verified). Implicit `Error` → `Result<Error>` is CS0457; document it in the XML doc, no test.
  - `MissingCase_IsCs8509`: compile `r switch { int i => … }` with Roslyn in-test and assert diagnostic `CS8509` (verified on SDK 11.0.100)
  - `NullReferenceValue_IsSuccess`: `Result.Ok<string?>(null).IsSuccess.Should().BeTrue()`
  - `OkUnit_IsSuccess`
  - `ResultAllocationTests.CreateAndMatch_ValueType_AllocatesNothing`: warm up, then 1,000 create-and-`switch` iterations; `GC.GetAllocatedBytesForCurrentThread()` delta `.Should().Be(0)`
- [ ] **Step 2:** Run the tests. Expected: FAIL.
- [ ] **Step 3:** Implement. Add `[MethodImpl(MethodImplOptions.AggressiveInlining)]` on trivial members, plus `[DebuggerDisplay]` and `[StackTraceHidden]`. A constructor never throws on null input.
- [ ] **Step 4:** Run the tests. Expected: PASS, including the 0-allocation test in Release (`dotnet test -c Release`).
- [ ] **Step 5:** Commit: `feat: Result<T> non-boxing union`.

### Task 4: `Option<T>` union core

**Files:**
- Create: `src/StandSure.Functional/None.cs`, `Option.T.cs` (type `Option<T>`), `Option.cs` (static factories)
- Test: `tests/StandSure.Functional.Tests/OptionUnionTests.cs`

**Interfaces:**
- Produces:
  - `public sealed class None` with a private constructor and `public static readonly None Instance`.
  - `[Union] public readonly struct Option<T> : IUnion` with cases `None | T`. Implicit from `T`; a null `T` becomes None. Implicit from `None`.
  - `bool IsSome`, `bool IsNone`, `object? Value` (`None.Instance` when absent), `bool HasValue` (always true).
  - `bool TryGetValue(out None? none)` and `bool TryGetValue([MaybeNullWhen(false)] out T value)`.
  - `public static Option<T> None { get; }` on `Option<T>`. Inside the struct this property shadows the case type, so the struct refers to the type as `global::StandSure.Functional.None` (verified: unqualified `None` binds to the property).
  - Static class `Option` with `Some<T>(T value)` and `None<T>()`.

- [ ] **Step 1: Write failing tests:**
  - `ImplicitFromValue_IsSome`
  - `ImplicitFromNull_IsNone`
  - `Default_IsNone`
  - `Switch_IsExhaustiveWithoutDiscard`: `o switch { None => "none", int i => ... }`
  - `OptionOfNullableValueType_NullIsNone`: `Option<int?> o = (int?)null; o.IsNone.Should().BeTrue()`
  - `CreateAndMatch_ValueType_AllocatesNothing`
- [ ] **Step 2:** Run the tests. Expected: FAIL.
- [ ] **Step 3:** Implement, using the same attributes as Task 3.
- [ ] **Step 4:** Run the tests. Expected: PASS.
- [ ] **Step 5:** Commit: `feat: Option<T> non-boxing union`.

### Task 5: `Try`

The public names here are provisional. Part C may rename them through the owner-approved report. Keep them in one file so a rename is a single edit.

**Files:**
- Create: `src/StandSure.Functional/Try.cs`
- Test: `tests/StandSure.Functional.Tests/TryTests.cs`

**Interfaces:**
- Consumes: `Result<T>`, `Error.FromException`, `Unit`.
- Produces: `public static class Try` with:
  - `Result<T> Run<T>(Func<T> fn)`
  - `Result<Unit> Run(Action action)`
  - `Task<Result<T>> RunAsync<T>(Func<CancellationToken, Task<T>> fn, CancellationToken cancellationToken = default)`
  - `Task<Result<Unit>> RunAsync(Func<CancellationToken, Task> fn, CancellationToken cancellationToken = default)`

- [ ] **Step 1: Write failing tests:**
  - `Run_Success_ReturnsValue`
  - `Run_Throws_ReturnsUnexpectedWithException`
  - `Run_OperationCanceled_Propagates`: `act.Should().Throw<OperationCanceledException>()`
  - `Run_OutOfMemory_Propagates`
  - `RunAsync_TaskCanceled_Propagates`: `ThrowAsync<TaskCanceledException>()`
  - `RunAsync_SyncThrowBeforeTask_BecomesFailure`: the delegate throws `InvalidOperationException` before returning a task
  - `RunAsync_SyncOceBeforeTask_Propagates`
  - `RunAsync_AggregateWrappingOce_Propagates`: the delegate blocks on a cancelled task with `.Wait()`; the result is an OCE, not a failure
  - `RunAsync_PassesToken`
- [ ] **Step 2:** Run the tests. Expected: FAIL.
- [ ] **Step 3:** Implement with two catch clauses in this order: (1) `catch (AggregateException ae) when (AllCancellation(ae))` rethrows the first flattened inner OCE via `ExceptionDispatchInfo.Throw`; (2) `catch (Exception ex) when (ex is not OperationCanceledException and not OutOfMemoryException)` returns `Error.FromException(ex)`. Everything else propagates untouched.
- [ ] **Step 4:** Run the tests. Expected: PASS.
- [ ] **Step 5:** Commit: `feat: Try bridge for throwing code`.

### Task 6: Telemetry

**Files:**
- Create: `src/StandSure.Functional/FunctionalDiagnostics.cs` (internal static)
- Modify: `src/StandSure.Functional/Error.cs`, so the factories, `FromException` and `Wrap` call diagnostics
- Test: `tests/StandSure.Functional.Tests/TelemetryTests.cs`. This needs `[Collection]` with parallelization disabled, because the listeners are process-global.

**Interfaces:**
- Produces:
  - `internal static class FunctionalDiagnostics` with `public const string Name = "StandSure.Functional"`, `ActivitySource Source`, `Meter Meter`, `Counter<long> Failures` (named `functional.failures`), `void RecordFailure(Error e)` and `void RecordWrap(Error outer)`.
  - `Error.Uninitialized` is a cached singleton that is never recorded.
  - Detail switch name: `StandSure.Functional.EmitErrorDetails` (`AppContext.TryGetSwitch`).

- [ ] **Step 1: Write failing tests** with an `ActivityListener` and a `MeterListener`:
  - `Factory_AddsFailureEvent_WithKindAndCode`
  - `Factory_IncrementsCounterOnce`
  - `Wrap_AddsWrappedEvent_DoesNotIncrementCounter`
  - `ConvertingErrorToResult_ThreeTimes_CountsOnce`: one `Error` converted to `Result<int>`, `Result<string>` and `Result<Unit>`; the counter delta is 1
  - `NoCurrentActivity_DoesNotThrow_StillCounts`
  - `Message_NotEmitted_ByDefault`
  - `Message_Emitted_WhenSwitchOn`
  - `Uninitialized_IsNotRecorded`
- [ ] **Step 2:** Run the tests. Expected: FAIL.
- [ ] **Step 3:** Implement, guarding with `Activity.Current is { } a` and `Failures.Enabled`.
- [ ] **Step 4:** Run all tests. Expected: PASS, and the Task 3 and 4 allocation tests still pass with no listener attached.
- [ ] **Step 5:** Commit: `feat: BCL failure telemetry`.

### Task 7: JSON

**Files:**
- Create: `src/StandSure.Functional/Json/FunctionalJsonContext.cs` (source-generated, `Error` and `ErrorKind` as a string enum), `ResultJsonConverterFactory.cs`, `OptionJsonConverterFactory.cs`
- Modify: `Result.T.cs` and `Option.T.cs` to add `[JsonConverter(typeof(...Factory))]`
- Test: `tests/StandSure.Functional.Tests/JsonTests.cs`

**Interfaces:**
- Produces:
  - Wire format, camelCase. `Result`: `{"value": T}` or `{"error": {"kind":"NotFound","code":…,"message":…,"metadata":{},"errors":[],"inner":null}}`. `Option`: `{"value": T}` or `{}`.
  - `T` is serialized through the caller's `JsonSerializerOptions` (`options.GetTypeInfo(typeof(T))`), so AOT callers register `Result<TheirType>` in their own context.
  - `public static JsonSerializerOptions AddFunctional(this JsonSerializerOptions options)`: adds the `FunctionalJsonContext` resolver to the chain.

- [ ] **Step 1: Write failing tests:**
  - `Result_Success_RoundTrips`
  - `Result_Failure_RoundTrips_WithInnerAndChildren`
  - `Result_EmptyObject_IsUninitializedFailure`
  - `Result_BothNull_IsUninitializedFailure`
  - `Result_BothPresent_ErrorWins`
  - `Result_UnknownProperty_Ignored`
  - `Option_Some_RoundTrips`
  - `Option_EmptyObject_IsNone`
  - `Default_Result_SerializesAsUninitializedError`
  - `Exception_NotSerialized`
  - `ErrorKind_SerializedAsString`
- [ ] **Step 2:** Run the tests. Expected: FAIL.
- [ ] **Step 3:** Implement the converters with `Utf8JsonReader`/`Utf8JsonWriter`, with no reflection beyond `GetTypeInfo`.
- [ ] **Step 4:** Run the tests. Expected: PASS.
- [ ] **Step 5:** Commit: `feat: JSON wire format for Error, Result, Option`.

### Task 8: Public API approval + benchmarks

**Files:**
- Create: `tests/StandSure.Functional.ApprovalTests/` (project, `PublicApiTests.cs`, `*.verified.txt`), `benchmarks/StandSure.Functional.Benchmarks/` (project, `ResultBenchmarks.cs`, `OptionBenchmarks.cs`)

- [ ] **Step 1:** Write `PublicApi_HasNotChanged` using `PublicApiGenerator` + `Verify.Xunit` (check both are current; see Global Constraints). Run it once, review the received file, and accept it.
- [ ] **Step 2:** Add benchmarks with `[MemoryDiagnoser]` covering create, switch and `Deconstruct` for `Result<int>`, `Option<int>` and `Result<string>`. Run `dotnet run -c Release --project benchmarks/... -- --filter '*'`. Expected: `Allocated` is `-` (0 B) for the value-type cases.
- [ ] **Step 3:** Commit: `test: public API snapshot and allocation benchmarks`.

---

## Part B — Model-usage harness (may run in parallel with Part A)

### Task 9: Harness scaffold + model clients

**Files:**
- Create: `harness/ModelHarness/ModelHarness.csproj` (console app, not packable), `harness/ModelHarness/Models/IModelClient.cs`, `ModelRequest.cs`, `ModelResponse.cs`, `AnthropicClient.cs`, `GeminiClient.cs`, `OllamaClient.cs`, `ModelConfig.cs`, `LoggerMessages.cs`
- Create: `harness/models.json`
- Test: `harness/ModelHarness.Tests/` (project), `ModelClientTests.cs`

**Interfaces:**
- Produces:
  - `public interface IModelClient { string Family { get; } Task<ModelResponse> CompleteAsync(ModelRequest request, CancellationToken ct); }`
  - `public sealed record ModelRequest(string ModelId, string System, string User)`
  - `public sealed record ModelResponse(string ModelId, string Text, int InputTokens, int OutputTokens, TimeSpan Latency, string? Error)`. A transport failure is reported in `Error` and never thrown, except OCE.
  - `models.json` entries: `{ "family": "anthropic|gemini|ollama", "id": "...", "enabled": true }`. Ship with `claude-opus-5-5`, `claude-sonnet-5-5` and `claude-haiku-5-5`, the current Gemini Pro and Flash IDs (look them up), and two Ollama coder models (Task 16 fills these in).
  - Keys come from the environment: `ANTHROPIC_API_KEY`, `GEMINI_API_KEY`. Run with `fnox exec --`. Ollama uses `http://localhost:11434`.

- [ ] **Step 1: Write failing tests** with a fake `HttpMessageHandler`:
  - `Anthropic_BuildsMessagesRequest_ParsesText`
  - `Gemini_BuildsGenerateContentRequest_ParsesText`
  - `Ollama_BuildsChatRequest_ParsesText`
  - `Http500_ReturnsErrorNotThrow`
  - `MissingKey_ReturnsErrorNotThrow`
  - `Config_LoadsEnabledModelsOnly`
- [ ] **Step 2:** Run `dotnet test harness/ModelHarness.Tests`. Expected: FAIL.
- [ ] **Step 3:** Implement with `HttpClient` and System.Text.Json source generation, making direct HTTP calls.
- [ ] **Step 4:** Run the tests. Expected: PASS.
- [ ] **Step 5:** Commit: `feat(harness): model clients`.

### Task 10: Task set + fixtures

**Files:**
- Create: `harness/tasks/NN-slug/` for the 12 tasks in DESIGN.md §4, each containing `task.md` (the user-prompt body), `Fixture.cs` (the caller-defined dependency interfaces and DTOs) and `Probe.cs` (a failure-injecting stub plus the probe's invocation arguments)
- Create: `harness/prompts/system-baseline.md`, `harness/prompts/system-with-library.md` (the latter includes `docs/llm-api-summary.md` at run time)
- Create: `harness/ModelHarness/Tasks/TaskCatalog.cs`
- Test: `harness/ModelHarness.Tests/TaskCatalogTests.cs`

**Interfaces:**
- Produces:
  - `public sealed record HarnessTask(string Id, string Prompt, string FixtureSource, string ProbeSource, string EntryType, string EntryMethod)`
  - `TaskCatalog.Load(string root) : IReadOnlyList<HarnessTask>`
  - Convention: each `task.md` names the class `Solution` and the method name and parameters, and leaves the **return type unspecified**, because the return type is what the baseline observes. The baseline system prompt is exactly the DESIGN.md §4 baseline text.

- [ ] **Step 1: Write failing tests:**
  - `Load_FindsTwelveTasks`
  - `EveryTask_HasFixtureAndProbe`
  - `EveryFixture_CompilesStandalone`: compiled with Roslyn against the net11 reference assemblies
  - `NoTaskPrompt_MentionsResultOrOption`: the baseline must not leak our vocabulary. Match whole words, case-sensitive, regex `\b(Result|Option|Either|Match|Bind)\b`, so "optional" in task 3 does not trip it
  - `EveryPrompt_SaysFixturesAreProvided`: each `task.md` states the fixture types already exist and must not be redeclared
- [ ] **Step 2:** Run the tests. Expected: FAIL.
- [ ] **Step 3:** Write the 12 tasks and `TaskCatalog`.
- [ ] **Step 4:** Run the tests. Expected: PASS.
- [ ] **Step 5:** Commit: `feat(harness): task set and fixtures`.

### Task 11: Code extraction + compilation

**Files:**
- Create: `harness/ModelHarness/Compile/CodeExtractor.cs`, `SolutionCompiler.cs`
- Test: `harness/ModelHarness.Tests/CodeExtractorTests.cs`, `SolutionCompilerTests.cs`

**Interfaces:**
- Produces:
  - `CodeExtractor.Extract(string modelText) : ExtractResult`, where `ExtractResult(string? Code, ExtractStatus Status)` and `ExtractStatus { Single, Concatenated, ExtractFailed }`. It takes ```` ```csharp ````/```` ```cs ````/unlabelled fences; several fences are concatenated in order; with no fence at all it tries the raw text only if it parses as C# with no syntax errors, and otherwise returns `ExtractFailed`.
  - `SolutionCompiler.Compile(HarnessTask task, string code, bool withLibrary) : CompileResult`, where `CompileResult(bool Success, IReadOnlyList<string> Diagnostics, byte[]? Assembly, CSharpCompilation Compilation, IReadOnlyList<string> StrippedTypes)`. The compilation is model code **plus `FixtureSource` and `ProbeSource`**. Before compiling, any top-level type the model declares whose name matches a fixture type is removed from the model's syntax tree and its name recorded in `StrippedTypes` (redeclaration is common and would otherwise measure nothing about API shape). It references the net11 reference pack, plus `StandSure.Functional.dll` when `withLibrary` is true.

- [ ] **Step 1: Write failing tests:**
  - `NoFence_InvalidCSharp_ExtractFailed`
  - `NoFence_ValidCSharp_Single`
  - `TwoFences_Concatenated`
  - `CompilesValidSolution`
  - `ReportsDiagnosticsForInvalid`
  - `WithLibrary_ResolvesResultType`
  - `FixtureTypesAreCompiledIn`: model code using `IUserStore` without declaring it compiles
  - `RedeclaredFixtureType_IsStripped_AndRecorded`
- [ ] **Step 2:** Run the tests. Expected: FAIL.
- [ ] **Step 3:** Implement.
- [ ] **Step 4:** Run the tests. Expected: PASS.
- [ ] **Step 5:** Commit: `feat(harness): extraction and compilation`.

### Task 12: Syntax and semantic extraction

**Files:**
- Create: `harness/ModelHarness/Analysis/UsageExtractor.cs`, `UsageFacts.cs`
- Test: `harness/ModelHarness.Tests/UsageExtractorTests.cs`

**Interfaces:**
- Consumes: `CompileResult.Compilation`.
- Produces: `UsageFacts(IReadOnlyList<string> DeclaredTypes, IReadOnlyList<string> UsedTypes, IReadOnlyList<string> InvokedMethods, string? EntryReturnType, int ThrowCount, int TryCatchCount, int IgnoredOutcomeCount, int NonExhaustiveSwitchCount, IReadOnlyList<string> Idioms)`. `Idioms` contains the tags `tuple-return`, `bool-try-out`, `nullable-return`, `exception`, `result-like-type` and `library-result`. An "outcome type" is any type named `*Result*`/`*Option*`/`*Outcome*`/`*Either*`, or a `ValueTuple` that includes a bool or an error-ish member. `NonExhaustiveSwitchCount` counts CS8509 diagnostics.

- [ ] **Step 1: Write failing tests**, one per fact, each using small inline sources:
  - `CountsThrows`
  - `DetectsTupleReturn`
  - `DetectsBoolTryOut`
  - `DetectsSelfDeclaredResultType`
  - `CountsIgnoredOutcomeInvocation`
  - `RecordsEntryReturnType`
  - `CountsCs8509`
- [ ] **Step 2:** Run the tests. Expected: FAIL.
- [ ] **Step 3:** Implement with `CSharpSyntaxWalker` plus `SemanticModel`.
- [ ] **Step 4:** Run the tests. Expected: PASS.
- [ ] **Step 5:** Commit: `feat(harness): usage extraction`.

### Task 13: Behavioural probe (out of process)

**Files:**
- Create: `harness/ProbeRunner/ProbeRunner.csproj` (console app), `Program.cs`
- Create: `harness/ModelHarness/Probe/ProbeExecutor.cs`
- Test: `harness/ModelHarness.Tests/ProbeExecutorTests.cs`

**Interfaces:**
- Produces:
  - `ProbeRunner <assemblyPath> <entryType> <entryMethod>`. It loads the assembly in a collectible `AssemblyLoadContext`, builds the probe's stub arguments from the `Probe` class in the compiled assembly, invokes the entry method, and awaits it if it is Task-like. It writes one JSON line to stdout: `{"outcome":"returned|threw|timeout|crashed","exceptionType":...}`.
  - `ProbeExecutor.RunAsync(byte[] assembly, HarnessTask task, TimeSpan timeout, CancellationToken ct) : Task<ProbeResult>` writes the assembly to a fresh temp directory and starts ProbeRunner with that directory as the working directory and the `DOTNET_` environment only. It starts the child with `ProcessStartInfo.Environment.Clear()` and then adds only `PATH`, `HOME` (set to the temp dir) and `DOTNET_ROOT`, so `fnox`-injected keys never reach model code. Network is denied by launching under macOS `sandbox-exec` with a profile that has `(deny network*)` and allows file writes only inside the temp dir; on non-macOS the probe refuses to run and records `crashed` with reason `no-sandbox`. It kills the process tree on timeout. The default timeout is 10 s.

- [ ] **Step 1: Write failing tests:**
  - `ReturningSolution_Returned`
  - `ThrowingSolution_Threw_WithType`
  - `InfiniteLoop_Timeout_ProcessKilled`
  - `EnvironmentExit_Crashed`
  - `AsyncSolution_Awaited`
  - `ProbeEnvironment_ContainsNoApiKeys`: parent sets `ANTHROPIC_API_KEY`; a solution that returns `Environment.GetEnvironmentVariable("ANTHROPIC_API_KEY")` reports null
  - `NetworkAccess_IsDenied`: a solution that opens a TCP connection to `1.1.1.1:443` gets an exception (recorded as `threw`)
- [ ] **Step 2:** Run the tests. Expected: FAIL.
- [ ] **Step 3:** Implement.
- [ ] **Step 4:** Run the tests. Expected: PASS.
- [ ] **Step 5:** Commit: `feat(harness): out-of-process behavioural probe`.

### Task 14: Runner + JSONL results

**Files:**
- Create: `harness/ModelHarness/Run/HarnessRunner.cs`, `RunRecord.cs`, `Program.cs` (CLI)
- Modify: `.gitignore`, adding `harness/results/raw/`
- Test: `harness/ModelHarness.Tests/HarnessRunnerTests.cs`

**Interfaces:**
- Consumes: `IModelClient`, `TaskCatalog`, `CodeExtractor`, `SolutionCompiler`, `UsageExtractor`, `ProbeExecutor`.
- Produces:
  - CLI: `dotnet run --project harness/ModelHarness -- run --condition baseline|with-library [--runs 5] [--models <id,...>] [--tasks <id,...>]`.
  - One `RunRecord` per line in `harness/results/raw/<utc-timestamp>-<condition>.jsonl`, with fields: `ts`, `runId`, `condition`, `family`, `model`, `task`, `run`, `promptHash`, `rawOutput`, `extractStatus`, `compileSuccess`, `diagnostics`, `usage` (UsageFacts), `probe`, `inputTokens`, `outputTokens`, `latencyMs`, `transportError`. The flat top-level `family`, `model`, `task` and `condition` fields serve as Loki labels.
  - Model calls run concurrently, at most 4 per family. Probes run one at a time.

- [ ] **Step 1: Write failing tests** with a fake `IModelClient`:
  - `WritesOneRecordPerModelTaskRun`: 2 models × 2 tasks × 2 runs gives 8 lines
  - `TransportError_RecordedNotThrown`
  - `PromptHash_StablePerTaskCondition`
- [ ] **Step 2:** Run the tests. Expected: FAIL.
- [ ] **Step 3:** Implement.
- [ ] **Step 4:** Run the tests. Expected: PASS.
- [ ] **Step 5:** Commit: `feat(harness): runner and JSONL output`.

### Task 15: Report generator

**Files:**
- Create: `harness/ModelHarness/Report/ReportBuilder.cs`
- Test: `harness/ModelHarness.Tests/ReportBuilderTests.cs`

**Interfaces:**
- Produces: CLI `report --input <jsonl> [--compare <jsonl>] --output harness/results/<condition>-report.md`. The report has these sections, per family and overall:
  1. Compile rate
  2. Probe outcome rates (threw / returned / timeout)
  3. Entry return type frequency
  4. Idiom frequency
  5. Top 30 invoked method names on outcome-like types
  6. Self-declared type names
  7. Per-task notes for cells where a family regresses versus `--compare`

- [ ] **Step 1: Write failing tests** with fixed JSONL fixtures:
  - `ComputesCompileRatePerFamily`
  - `RanksMethodNames`
  - `CompareFlagsRegression`
- [ ] **Step 2:** Run the tests. Expected: FAIL.
- [ ] **Step 3:** Implement.
- [ ] **Step 4:** Run the tests. Expected: PASS.
- [ ] **Step 5:** Commit: `feat(harness): report generator`.

### Task 16: Pull local models + baseline run

- [ ] **Step 1:** Pick two current coding-focused Ollama models that fit 64 GB of RAM with room to spare. Look up what's current on the Ollama library and prefer the newest Qwen-Coder-class and one other family. `ollama pull` them, and set them in `harness/models.json`.
- [ ] **Step 2:** Run a smoke test: `fnox exec -- dotnet run --project harness/ModelHarness -- run --condition baseline --runs 1 --tasks 01`. Expected: one line per enabled model and no transport errors.
- [ ] **Step 3:** Run the full baseline with `--runs 5`.
- [ ] **Step 4:** Build `harness/results/baseline-report.md`.
- [ ] **Step 5:** Add a "Proposed core names" section to the report. For each Part C member (Tasks 18–20), give the proposed name, the evidence (frequencies per family) and a keep/rename call. Also include the `Try` entry-point naming.
- [ ] **Step 6:** Commit the report (not the raw output): `docs(harness): baseline report`.

---

## Part C — Naming-dependent core (after owner approval)

### Task 17: Owner gate

- [ ] **Step 1:** Present the "Proposed core names" section to the owner and record the decisions in `.specify/features/functional-core/reviews/baseline-naming-YYYY-MM-DD.md`.
- [ ] **Step 2:** Apply any approved `Try` rename (Task 5 file), update `PublicAPI.Unshipped.txt` and the approval snapshot, and run `dotnet test`. Expected: PASS.
- [ ] **Step 3:** Commit: `refactor: apply approved baseline naming`.

Tasks 18–20 use the **approved** names. The names below are the defaults if the report keeps them.

### Task 18: `Result<T>` combinators + async

**Files:**
- Create: `src/StandSure.Functional/Result.T.Combinators.cs` (partial `Result<T>`), `src/StandSure.Functional/ResultTaskExtensions.cs`
- Test: `tests/StandSure.Functional.Tests/ResultCombinatorTests.cs`, `ResultAsyncTests.cs`

**Interfaces:**
- Produces, on `Result<T>`:
  - `TOut Match<TOut>(Func<Error,TOut> onFailure, Func<T,TOut> onSuccess)`
  - `Result<TOut> Map<TOut>(Func<T,TOut> map)`
  - `Result<TOut> Bind<TOut>(Func<T,Result<TOut>> bind)`
  - `Result<T> MapError(Func<Error,Error> map)`
  - `Result<T> Ensure(Func<T,bool> predicate, Error error)`
  - `Result<T> Ensure(Func<T,bool> predicate, [CallerArgumentExpression(nameof(predicate))] string? expression = null)`: on failure it gives `Error.Validation("result.ensure_failed", expression)`. Give it `[OverloadResolutionPriority(-1)]` so the explicit-error overload is preferred.
  - `Result<T> Tap(Action<T> action)`
  - `T GetValueOrDefault(T fallback)`
- Produces, on `Task<Result<T>>`: `MapAsync`, `BindAsync`, `MatchAsync` and `EnsureAsync`, each accepting a sync lambda and a `Task`-returning lambda.

- [ ] **Step 1: Write failing tests.** For each member, one success-path and one failure-path test, plus:
  - `Default_Map_StaysUninitialized`
  - `Bind_ShortCircuits_SecondNotCalled`
  - `Ensure_CallerExpression_InMessage`
  - `Match_FailureFirstOrder`: the first lambda receives the `Error`
  - `MapAsync_SyncLambda_Compiles`
  - `BindAsync_ChainOfThree_StopsAtFirstFailure`
- [ ] **Step 2:** Run the tests. Expected: FAIL.
- [ ] **Step 3:** Implement. Lambdas that throw are not caught; only `Try` catches.
- [ ] **Step 4:** Run the tests and the allocation tests. Expected: PASS.
- [ ] **Step 5:** Commit: `feat: Result combinators`.

### Task 19: `Option<T>` combinators

**Files:**
- Create: `src/StandSure.Functional/Option.T.Combinators.cs`
- Test: `tests/StandSure.Functional.Tests/OptionCombinatorTests.cs`

**Interfaces:**
- Produces:
  - `TOut Match<TOut>(Func<TOut> onNone, Func<T,TOut> onSome)`
  - `Option<TOut> Map<TOut>(Func<T,TOut>)`
  - `Option<TOut> Bind<TOut>(Func<T,Option<TOut>>)`
  - `Option<T> Filter(Func<T,bool>)`
  - `T GetValueOrDefault(T fallback)`
  - `Result<T> ToResult(Error ifNone)`

- [ ] **Step 1: Write failing tests**, one success-path and one None-path test per member, plus `ToResult_None_IsGivenError`.
- [ ] **Step 2:** Run the tests. Expected: FAIL.
- [ ] **Step 3:** Implement.
- [ ] **Step 4:** Run the tests. Expected: PASS.
- [ ] **Step 5:** Commit: `feat: Option combinators`.

### Task 20: Collection helpers + approved itertools set

**Files:**
- Create: `src/StandSure.Functional/ResultEnumerableExtensions.cs`, `OptionEnumerableExtensions.cs`, plus one file per approved itertools/functools member
- Test: matching `*Tests.cs`

**Interfaces:**
- Produces:
  - `Result<IReadOnlyList<T>> Combine<T>(this IEnumerable<Result<T>>)`: on any failure, returns `Error.Validation("result.combine_failed", "<n> of <m> failed", errors)` holding **all** the failures.
  - `Result<IReadOnlyList<TOut>> Traverse<T,TOut>(this IEnumerable<T>, Func<T,Result<TOut>>)`
  - `(IReadOnlyList<T> Successes, IReadOnlyList<Error> Failures) Partition<T>(this IEnumerable<Result<T>>)`
  - `IEnumerable<T> Choose<T>(this IEnumerable<Option<T>>)`
  - Only the itertools, functools and toolz members the owner approved in Task 17, each lazy where LINQ is lazy.

- [ ] **Step 1: Write failing tests:**
  - `Combine_AllOk`
  - `Combine_CollectsEveryFailure`
  - `Combine_Empty_IsOkEmpty`
  - `Traverse_CollectsEveryFailure`
  - `Partition_PreservesOrder`
  - `Choose_SkipsNone`
  - one test per approved itertools member, covering the empty, single-element and normal cases
- [ ] **Step 2:** Run the tests. Expected: FAIL.
- [ ] **Step 3:** Implement.
- [ ] **Step 4:** Run the tests. Expected: PASS.
- [ ] **Step 5:** Commit: `feat: collection helpers`.

### Task 21: Consumer experience + with-library run

**Files:**
- Create: `src/StandSure.Functional/buildTransitive/StandSure.Functional.props` (adds `<Using Include="StandSure.Functional" />`; pack it under `buildTransitive/`), `docs/llm-api-summary.md` (one page, at most about 120 lines), `README.md` (with credits: language-ext; the "Built by Claude Code" section per the owner's convention)
- Test: `tests/StandSure.Functional.PackageTests/` (a project that consumes the packed `.nupkg` from a local feed)

- [ ] **Step 1: Write failing test** `GlobalUsing_ResultAvailableWithoutUsingDirective`: a file with no `using` compiles against the packed package.
- [ ] **Step 2:** Run `dotnet pack`, then run the test. Expected: FAIL before the props file exists, PASS after.
- [ ] **Step 3:** Write `llm-api-summary.md` from the final public API, giving one example per harness task shape (DESIGN.md §4, tasks 1–12).
- [ ] **Step 4:** Run the with-library condition with `--runs 5`, then `report --input <with-library> --compare <baseline>`. Commit `harness/results/with-library-report.md`.
- [ ] **Step 5:** Check DESIGN.md §6 success criteria. Open a GitHub issue for each regressed family/task cell.
- [ ] **Step 6:** Commit: `feat: global usings, API summary, with-library report`.
