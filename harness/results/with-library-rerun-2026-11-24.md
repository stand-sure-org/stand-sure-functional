# With-library re-run, 2026-11-24 (regressed tasks only)

**Why:** issues #6-#14 came from the 2026-11-19 with-library run. The API summary changed since; this run
checks which regressions still hold.

**What changed in the prompt:** `docs/llm-api-summary.md` line 3 now asks for a `using` for every namespace
used (`System`, `System.Collections.Generic`, `System.Linq`, `System.Threading`, `System.Threading.Tasks`),
not only `using StandSure.Functional;`. Same 7 tasks, all three runs:

| run | answers with `using System;` | failed on a missing BCL type or LINQ method |
|---|---|---|
| baseline | 216/245 | 12 |
| with-lib 11-19 | 178/245 | 26 |
| with-lib today | 202/245 | 20 |

Better, not fixed. After this run, the first example in the summary also gained `using
System.Collections.Generic;` (it uses `List<Error>`; models copy examples over prose), so the current
promptHash no longer matches `d9b36074…`. The next run measures that.

**Run:** `run --condition with-library --runs 5 --tasks 01,03,04,07,08,10,11` (7 tasks x 7 models x 5 = 245
records, promptHash `d9b36074…`), raw records `raw/20261124T202040Z-with-library.jsonl` (local, gitignored).
Compared with `raw/20261119T162804Z-baseline.reextracted.jsonl` and
`raw/20261119T184248Z-with-library.reextracted.jsonl`, same tasks. Counts are compiled / records.
Regression rule as in DESIGN §6: more than 10 points worse than baseline, n >= 3 on both sides.

| family | task | issue | baseline | with-lib 11-19 | with-lib today | regresses today |
|---|---|---|---|---|---|---|
| anthropic | 01-create-user |  | 14/15 (93%) | 15/15 (100%) | 15/15 (100%) | no |
| anthropic | 03-config-timeout |  | 12/15 (80%) | 14/15 (93%) | 15/15 (100%) | no |
| anthropic | 04-charge-card | #6 | 14/15 (93%) | 11/15 (73%) | 15/15 (100%) | no |
| anthropic | 07-order-endpoint |  | 5/15 (33%) | 8/15 (53%) | 14/15 (93%) | no |
| anthropic | 08-user-settings | #7 | 15/15 (100%) | 13/15 (87%) | 15/15 (100%) | no |
| anthropic | 10-import-contacts |  | 15/15 (100%) | 14/15 (93%) | 14/15 (93%) | no |
| anthropic | 11-invoice-layers |  | 12/15 (80%) | 14/15 (93%) | 10/15 (67%) | yes (harness, see below) |
| gemini | 01-create-user |  | 8/10 (80%) | 10/10 (100%) | 10/10 (100%) | no |
| gemini | 03-config-timeout |  | 10/10 (100%) | 10/10 (100%) | 10/10 (100%) | no |
| gemini | 04-charge-card |  | 10/10 (100%) | 10/10 (100%) | 10/10 (100%) | no |
| gemini | 07-order-endpoint |  | 10/10 (100%) | 10/10 (100%) | 10/10 (100%) | no |
| gemini | 08-user-settings |  | 10/10 (100%) | 10/10 (100%) | 10/10 (100%) | no |
| gemini | 10-import-contacts | #8 | 10/10 (100%) | 6/10 (60%) | 7/10 (70%) | yes |
| gemini | 11-invoice-layers |  | 6/10 (60%) | 10/10 (100%) | 10/10 (100%) | no |
| ollama | 01-create-user | #9 | 9/10 (90%) | 2/10 (20%) | 4/10 (40%) | yes |
| ollama | 03-config-timeout | #10 | 10/10 (100%) | 5/10 (50%) | 5/10 (50%) | yes |
| ollama | 04-charge-card | #11 | 6/10 (60%) | 4/10 (40%) | 2/10 (20%) | yes |
| ollama | 07-order-endpoint | #12 | 3/10 (30%) | 0/10 (0%) | 2/10 (20%) | no, on the threshold (1/10 would regress); #12 kept open |
| ollama | 08-user-settings |  | 10/10 (100%) | 10/10 (100%) | 10/10 (100%) | no |
| ollama | 10-import-contacts | #13 | 9/10 (90%) | 0/10 (0%) | 3/10 (30%) | yes |
| ollama | 11-invoice-layers | #14 | 7/10 (70%) | 4/10 (40%) | 4/10 (40%) | yes |
| **all** | | | 205/245 (84%) | 180/245 (73%) | 195/245 (80%) | |

## What the remaining failures are

- **anthropic / 11-invoice-layers:** 4 of 5 failures are claude-sonnet-5-5 answering twice (a first version,
  then a revised one in a second code block). The extractor concatenates the blocks (`extractStatus:
  concatenated`), so `Solution` is defined two or three times (CS0101/CS0111/CS0229). Harness extraction,
  not the library (#36). The fifth (opus) is CS0411 on `Bind`.
- **gemini / 10 (#8):** CS0411, `Match<TOut>` type inference. Real API friction.
- **ollama:** a mix. Library-related: CS0411 on `Bind`/`Match` inference, CS1061 reaching for a `.Error`
  property (deliberately absent). Not library-related: missing `Microsoft.AspNetCore.Http` using
  (`IResult`/`Results`), matching an `Error` against an exception type (CS8121), `string.Equals(a, b, 1)`
  (CS1503).

## Re-scored with the revised-block extractor (#36)

The extractor now removes a non-partial type from earlier fences when a later fence redeclares it (a fence left empty is dropped), status `revised`.
Re-extracting and recompiling every `concatenated` record of the three runs changes 9 records: 8 failures now
compile, none that compiled breaks. Rows of the table above that change:

| family | task | baseline | with-lib 11-19 | with-lib today | regresses today |
|---|---|---|---|---|---|
| anthropic | 07-order-endpoint | 5/15 → 7/15 (47%) | 8/15 | 14/15 → 15/15 (100%) | no |
| anthropic | 08-user-settings | 15/15 | 13/15 → 14/15 | 15/15 | no |
| anthropic | 11-invoice-layers | 12/15 | 14/15 | 10/15 → 14/15 (93%) | no (was yes) |

Records' stored `compileSuccess` is unchanged (re-extraction never rewrites results); the next run scores with
the new extractor.

## Run 2: example fixed, new extractor

Same 7 tasks × 7 models × 5 runs, after #34 (the first summary example writes its BCL `using`) and #37 (revised-block
extractor). promptHash `223602ef…`, raw records `raw/20261124T210716Z-with-library.jsonl` (local). All 245 records
extracted as `single`. Baseline and run 1 below are re-scored with the #37 extractor.

| family | task | issue | baseline | run 1 | run 2 | run 2 regresses |
|---|---|---|---|---|---|---|
| anthropic | 01-create-user |  | 14/15 (93%) | 15/15 (100%) | 15/15 (100%) | no |
| anthropic | 03-config-timeout |  | 12/15 (80%) | 15/15 (100%) | 15/15 (100%) | no |
| anthropic | 04-charge-card |  | 14/15 (93%) | 15/15 (100%) | 14/15 (93%) | no |
| anthropic | 07-order-endpoint |  | 7/15 (47%) | 15/15 (100%) | 13/15 (87%) | no |
| anthropic | 08-user-settings |  | 15/15 (100%) | 15/15 (100%) | 15/15 (100%) | no |
| anthropic | 10-import-contacts |  | 15/15 (100%) | 14/15 (93%) | 14/15 (93%) | no |
| anthropic | 11-invoice-layers |  | 12/15 (80%) | 14/15 (93%) | 15/15 (100%) | no |
| gemini | 01-create-user |  | 8/10 (80%) | 10/10 (100%) | 10/10 (100%) | no |
| gemini | 03-config-timeout |  | 10/10 (100%) | 10/10 (100%) | 10/10 (100%) | no |
| gemini | 04-charge-card |  | 10/10 (100%) | 10/10 (100%) | 10/10 (100%) | no |
| gemini | 07-order-endpoint |  | 10/10 (100%) | 10/10 (100%) | 10/10 (100%) | no |
| gemini | 08-user-settings |  | 10/10 (100%) | 10/10 (100%) | 10/10 (100%) | no |
| gemini | 10-import-contacts | #8 | 10/10 (100%) | 7/10 (70%) | 7/10 (70%) | yes |
| gemini | 11-invoice-layers |  | 6/10 (60%) | 10/10 (100%) | 10/10 (100%) | no |
| ollama | 01-create-user | #9 | 9/10 (90%) | 4/10 (40%) | 5/10 (50%) | yes |
| ollama | 03-config-timeout | #10 | 10/10 (100%) | 5/10 (50%) | 10/10 (100%) | no |
| ollama | 04-charge-card | #11 | 6/10 (60%) | 2/10 (20%) | 1/10 (10%) | yes |
| ollama | 07-order-endpoint | #12 | 3/10 (30%) | 2/10 (20%) | 0/10 (0%) | yes |
| ollama | 08-user-settings |  | 10/10 (100%) | 10/10 (100%) | 10/10 (100%) | no |
| ollama | 10-import-contacts | #13 | 9/10 (90%) | 3/10 (30%) | 0/10 (0%) | yes |
| ollama | 11-invoice-layers | #14 | 7/10 (70%) | 4/10 (40%) | 6/10 (60%) | no, on the threshold |
| **all** | | | 207/245 (84%) | 200/245 (82%) | 200/245 (82%) | |

**The `using` fix worked:** failures on a missing BCL type or LINQ method went 12 (baseline) / 20 (run 1) / **8**
(run 2); `using System;` in 215/245 (baseline 216/245). Anthropic and Gemini do not regress anywhere except
gemini/10 (#8); anthropic/10 is one compile below baseline (14/15 vs 15/15), inside the 10-point threshold.

**Ollama is noisy at n=10:** ollama/03 went 50% → 100% and ollama/10 30% → 0% between two runs on near-identical
prompts. What is left in its failures (run 2, tasks 01/04/07/10):
- `Error` treated as an exception: matched against `PaymentDeclinedException` (CS8121 x14), passed where an
  `Exception` is expected (CS1503 x11). Task 04.
- Missing `Microsoft.AspNetCore.Http` using: `IResult` / `Results` (CS0246/CS0103). Task 07.
- Guessed members: `error.Match(...)` (CS1061 x7), `Unit.Default` (CS0117 x5).
- Type inference on `Bind`/`Match` (CS0411 x5), the same friction as #8 / #13.

**What happens to the issues** (DESIGN §6 rule applied to the latest run, run 2):
- #10 (ollama/03): closed by this PR. It regressed in run 1 (5/10) and is 10/10 in run 2, so given the noise above it
  rests on one run; the next with-library run re-opens it if it drops again.
- #12 (ollama/07): stays open. It sat on the threshold in run 1 and is a clear regression in run 2 (0/10).
- #14 (ollama/11): stays open, on the threshold (6/10 vs 7/10).
- #8, #9, #11, #13: stay open, still regress.
