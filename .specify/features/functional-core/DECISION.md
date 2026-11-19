# Decision: Functional Core + Model-Usage Harness

**Version:** 1.0 (Initial, from brainstorm)
**Date:** 2026-11-19
**Changes:** First write-up of the 2026-11-19 design conversation.

## Use case

A framework for rapidly building minimum viable prototypes with little to no human
involvement. The code that consumes this library is written mostly by models
(Anthropic Claude, Google Gemini/Antigravity, local Ollama/omlx models), supervised
by another agent. This library is the bottom layer of that framework.

Nearly every method in the framework has the same shape:

1. validate
2. if invalid, return a failure that may or may not carry helpful information
3. if valid, call the dependency (through an interface the *caller* defines)
4. return the result, adapting as needed

The library exists to make that shape the path of least resistance for a model.

## Business value (owner's layered agenda)

1. Lower risk for angel/alpha investors (and use that as a route to partnership).
2. Produce a very high volume of open-source, high-quality MVPs to draw attention to #1.
3. Capture agent interventions (via hooks/harnesses/telemetry) for longer-term research:
   is there an algebra of interventions; can counterfactuals inform agent planning
   without polluting context.

First audience is the owner's own framework ("us"). Public OSS adoption is a side
effect, not the target.

## Scope decisions

| Decision | Outcome |
|---|---|
| Ownership | New MIT repo `stand-sure-org/stand-sure-functional`, written from scratch. No code copied from the earlier TryHelpers repos. |
| Target | **net11.0 only** (C# 15 unions, `ActivitySourceFactory`). No netstandard, no net10. |
| Exceptions | Library never throws. `Try`/`TryAsync` bridges throwing code into `Result`. `OperationCanceledException` is never caught. |
| Ordering | Failure/absence first everywhere (`Match(onFailure, onSuccess)`, `Match(onNone, onSome)`). |
| Naming | Use names models already know: `Result`, `Option` (not Maybe/Some-as-type), `Map`, `Bind`, `Match`, `Ensure`, `Try`. |
| Overloads | Few. One obvious way per operation. |
| Error model | Closed set of error *kinds* + open string code (default). Per-method typed error unions come later, via generators. |
| Go idioms | `var (value, err) = result;` deconstruction alongside chaining; error wrapping with context (like `%w` / `errors.Is`). |
| Async | `Task<Result<T>>` at hand-written boundaries; `ValueTask<Result<T>>` only in generated code and library internals. |
| Telemetry | Always on, BCL only: static `ActivitySource`/`Meter` in core; DI-scoped `ActivitySourceFactory`/`IMeterFactory` in generated code; OTel registration in a sibling package. |
| Codegen | Generators write real, editable source (partial classes) into the repo. Compiler-only output is rejected (agents can't see or edit it). |
| AOT | Nice to have, not required. Avoid reflection in core anyway. |
| Testing | TDD; xUnit + AwesomeAssertions (not FluentAssertions). |
| Name | Working name `StandSure.Functional`. A witty, agent-friendly final name comes later; rename must be cheap. |
| Evidence first | Run a model-usage harness *before* freezing the core API. |

## Sequencing (approach A, chosen)

1. Model-usage harness + baseline run (no library) → **this feature**
2. Core library, API informed by the baseline → **this feature**
3. Analyzers + code fixes → separate design pass
4. Generator CLI → separate design pass
5. Sibling packages (OTel registration, ASP.NET/ProblemDetails mapping) → separate design pass
