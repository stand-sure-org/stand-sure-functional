# Agentic adoption check, 2026-11-24

Question: when a coding agent works in a repository that already references `StandSure.Functional`, does it use the
library or write its own result types?

The model-usage harness answers a narrower question: it puts `docs/llm-api-summary.md` in the system prompt and asks
for one code block. Real agents work in a repository and have to find the library themselves. This check gave fresh
tool-using agents (Claude, one per run, no shared context, told to stay inside their repository) a ticket in a small
consumer repository, `Acme.Checkout`, whose project file references `StandSure.Functional` 0.1.0.

## Setup

- Tickets, worded as ordinary work items with no mention of exceptions, results or the library:
  - T1, sign-up: validate four fields, report every problem at once, then create the account (harness task 06).
  - T2, shipping quote: validate input, call an async rates client, handle "no carrier", honour cancellation (task 09).
- Conditions, two runs per ticket per condition (12 runs):
  - **A, package only**: the `PackageReference` and nothing else. No source file names the namespace (global using).
  - **B, CLAUDE.md + summary**: A, plus a three-bullet `CLAUDE.md` (`docs/agents/consumer-CLAUDE.md`) and
    `docs/llm-api-summary.md` copied to `docs/standsure-functional.md`.
  - **C, one example**: A, plus one existing method (`CatalogService.GetProduct`) that returns `Result<Product>`.
- No .NET SDK was available, so nothing was compiled. Usage was read from the diffs.

## Results

| Condition | Used `Result<T>` | Hand-rolled result type | Notes |
|---|---|---|---|
| A, package only | 0 of 4 | 4 of 4 | Every run said it could not see the package API and would not guess. One tried to read the GitHub repository. |
| B, CLAUDE.md + summary | 4 of 4 | 0 of 4 | T1: grouped child errors. T2: `Try.RunAsync`, `Unavailable` for transport failures, `Wrap` for context. |
| C, one example | 3 of 4 | 1 of 4 (wrapped library `Error`s in its own type) | Agents used only what the example showed: no grouped errors (one run joined messages into one string), no `Try`. |

## Reading

- Discovery, not API shape, is the bottleneck. Agents behaved sensibly: without visible docs they refused to guess.
- An example in the codebase gets the type adopted but caps usage at what the example shows.
- The three-bullet `CLAUDE.md` plus the one-page summary was enough for full, idiomatic use in every run.
- Small n, one model family, uncompiled. Treat as direction, not measurement. A compiled rerun inside the harness
  (a "repository" condition next to baseline and with-library) would make it a measurement.
