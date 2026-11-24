# With-library re-run, 2026-11-24 (regressed tasks only)

**Why:** issues #6-#14 came from the 2026-11-19 with-library run. The API summary changed since; this run
checks which regressions still hold.

**What changed in the prompt:** `docs/llm-api-summary.md` line 3 now asks for a `using` for every namespace
used (`System`, `System.Collections.Generic`, `System.Linq`, `System.Threading`, `System.Threading.Tasks`),
not only `using StandSure.Functional;`. In the 11-19 run, with-library answers wrote `using System;` in
293/420 records against 379/420 in the baseline, and failed on a missing BCL type or LINQ method 31 times
against 14. Today: 202/245.

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
| ollama | 07-order-endpoint | #12 | 3/10 (30%) | 0/10 (0%) | 2/10 (20%) | no (10 points, not more) |
| ollama | 08-user-settings |  | 10/10 (100%) | 10/10 (100%) | 10/10 (100%) | no |
| ollama | 10-import-contacts | #13 | 9/10 (90%) | 0/10 (0%) | 3/10 (30%) | yes |
| ollama | 11-invoice-layers | #14 | 7/10 (70%) | 4/10 (40%) | 4/10 (40%) | yes |
| **all** | | | 205/245 (84%) | 180/245 (73%) | 195/245 (80%) | |

## What the remaining failures are

- **anthropic / 11-invoice-layers:** 4 of 5 failures are claude-sonnet-5-5 answering twice (a first version,
  then a revised one in a second code block). The extractor concatenates the blocks (`extractStatus:
  concatenated`), so `Solution` is defined two or three times (CS0101/CS0111/CS0229). Harness extraction,
  not the library. The fifth (opus) is CS0411 on `Bind`.
- **gemini / 10 (#8):** CS0411, `Match<TOut>` type inference. Real API friction.
- **ollama:** a mix. Library-related: CS0411 on `Bind`/`Match` inference, CS1061 reaching for a `.Error`
  property (deliberately absent). Not library-related: missing `Microsoft.AspNetCore.Http` using
  (`IResult`/`Results`), matching an `Error` against an exception type (CS8121), `string.Equals(a, b, 1)`
  (CS1503).
