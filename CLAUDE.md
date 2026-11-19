# StandSure.Functional

BCL-only C# library (MIT, .NET 11 / C# 15): `Result<T>`, `Option<T>`, `Error`, `Unit`, `Try`.
Design docs and plan live in `.specify/features/functional-core/` (start with `DECISION.md`, `DESIGN.md`).

## Commands

```bash
dotnet build -c Release     # must produce 0 warnings
dotnet test                 # xUnit v3 on Microsoft.Testing.Platform (set in global.json)
dotnet test --filter-method "*Unit*"   # focused run
```

## Layout

```
src/StandSure.Functional/                  the library (PublicAPI.Shipped/Unshipped.txt tracked here)
tests/StandSure.Functional.Tests/          xUnit v3 + AwesomeAssertions
tests/StandSure.Functional.ApprovalTests/  public API snapshot
benchmarks/StandSure.Functional.Benchmarks/
harness/ModelHarness/  harness/tasks/ (NN-slug: task.md, Fixture.cs, Probe.cs)  harness/prompts/  harness/results/
docs/llm-api-summary.md                    one page, the with-library prompt
.specify/features/functional-core/
```

Build config: `Directory.Build.props` (all projects), `src/Directory.Build.props` (docs file + PublicApiAnalyzers, src only),
`Directory.Packages.props` (central package versions), `BannedSymbols.txt`, `nuget.config` (nuget.org only).

## Global Constraints

- `TargetFramework` `net11.0` only; default LangVersion (C# 15); SDK pinned via `global.json` to `11.0.100`, `rollForward: latestFeature`.
- Core package `StandSure.Functional`: zero NuGet runtime dependencies (analyzers are `PrivateAssets=all`).
- Root namespace and package id come from one MSBuild property, `FunctionalRootName` = `StandSure.Functional`, in `Directory.Build.props`.
- `Nullable` enabled, `TreatWarningsAsErrors` true in all configurations. `GenerateDocumentationFile` and PublicApiAnalyzers apply to `src/**` only.
- Ban `System.Console` via `BannedSymbols.txt`. Use ILogger with LoggerMessage source generator where logging is needed (harness only).
- C# style: braces always, early returns and guard clauses, one top-level type per file, file-scoped namespaces.
- The library never throws from its own APIs. `Try` never catches `OperationCanceledException` (including subclasses) or `OutOfMemoryException`.
- Failure/absence comes first in every two-branch signature: `Match(onFailure, onSuccess)`, `Match(onNone, onSome)`.
- No throwing `.Value` or `.Error` accessors on `Result<T>`.
- Telemetry source and meter name: `StandSure.Functional`. Event names `functional.failure` and `functional.failure.wrapped`; counter `functional.failures`; tags `error.kind` and `error.code`. Messages and metadata are emitted only when the `AppContext` switch `StandSure.Functional.EmitErrorDetails` is on.
- Tests: xUnit v3 + AwesomeAssertions (never FluentAssertions). TDD: red, green, refactor.
- Package versions: latest stable at implementation time. Stop and flag any package last updated more than 2 years ago.
- Secrets via `fnox` (repo has `fnox.toml`). Never commit keys.
- Commits use `--no-gpg-sign` and go on a feature branch, never `main`. In this repo the commit hook commits as `stand-sure-ai[bot]` and pushes.
- Never stage `.agents/`, `.claude/`, `.entire/`, `fnox.toml` unless the owner asks (owner-managed config). Stage explicit paths only.
