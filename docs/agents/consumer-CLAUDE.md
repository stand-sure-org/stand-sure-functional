# CLAUDE.md snippet for projects that use StandSure.Functional

Paste the block below into the consuming repository's `CLAUDE.md` (or `AGENTS.md`), and copy
`docs/llm-api-summary.md` from this repository (it also ships inside the package, under `docs/`) to
`docs/standsure-functional.md` in the consuming repository.

Why: coding agents do not read package internals. With only the `PackageReference`, 4 of 4 fresh agents wrote their
own result types and said they could not see the library's API. With this block and the summary file, 4 of 4 used
`Result<T>` and `Error` (see `harness/results/agentic-adoption-2026-11-24.md`).

```markdown
- Expected failures (invalid input, not found, a dependency with no answer) are values, not exceptions.
  Return `Result<T>` / `Option<T>` with `Error` from the `StandSure.Functional` package (already referenced, global using is automatic).
  Don't write your own Result, Either or error types.
- API and examples: `docs/standsure-functional.md`. Read it before writing a service method.
- `Match` takes failure first: always name the arguments (`onFailure:`, `onSuccess:`).
```

Keep the file path in the second bullet in step with where you copy the summary. The package's global using means no
source file in the consuming repository names the namespace, so without this block an agent has nothing to find.
