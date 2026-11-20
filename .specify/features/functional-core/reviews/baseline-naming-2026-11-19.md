# Baseline naming decisions — 2026-11-19

**Gate:** PLAN Task 17. **Evidence:** `harness/results/baseline-report.md` § Proposed core names
(420 records: 7 models × 12 tasks × 5 runs; 417 eligible). **Decided by:** owner.

| # | Decision | Evidence | Status |
|---|---|---|---|
| 1 | Primary factories `Result.Success<T>(T)` / `Result.Failure<T>(Error)` (rename from `Ok`/`Fail`). Keep `Ok`/`Fail` as one-line forwarding aliases marked `[EditorBrowsable(EditorBrowsableState.Never)]`, omitted from `docs/llm-api-summary.md`. `Result.Ok()` (Unit) becomes `Result.Success()` with a hidden `Ok()` alias. | Static factory names on model-declared types: `Success` 65 vs `Ok` 22; `Failure` 60 vs `Fail` 17 (compiled-only 43/10, 39/11). gemini-flash leaned `Ok` (9 vs 6). | Approved. Task 21 counts which spelling with-library models use; drop aliases before 1.0 if unused. |
| 2 | Keep failure-first `Match(onFailure, onSuccess)` / `Match(onNone, onSome)` — **deliberate deviation**. | Every model-declared `Match` was success-first (15/15); first `switch` arm success in 38/42. | Approved. Mitigations: named arguments in every API-summary example; later analyzer for positional `Match`; Task 21 measures wrong-order rate. |
| 3 | Itertools/functools: include `Pairwise` and `Scan` only (lazy, LINQ-style). Exclude all other DESIGN candidates. | Task 12: index loops 30/35, running totals 34/35; zero usage of any other candidate. | Approved (low confidence). |
| 4 | Keep `Try.Run` / `Try.RunAsync` provisionally. | 0/417 reusable try/catch helpers; task 04 inline try/catch 35/35. | Approved; revisit after Task 21. |
| 5 | All other Part C names keep their defaults (`IsSuccess`, `IsFailure`, `Map`, `Bind`, `MapError`, `Ensure`, `Tap`, `GetValueOrDefault`, async set, Option members, `Combine`, `Traverse`, `Partition` with `(Successes, Failures)`, `Choose`). | High for `IsSuccess` (116); otherwise low counts, no competing names. | Approved by default. |

Owner note: the models reproduced the success-first order the owner wrote in early versions —
likely corpus frequency (ErrorOr, CSharpFunctionalExtensions, OneOf, Promise.then) rather than design intent.
