---
entry-method: Import
---
# Import a list of contacts, keeping successes and failures apart

A CSV upload has already been parsed into rows. Each row is imported on its own: a bad row must not stop the rest of the import.

## What to write

A public class named `Solution` with:

- a public constructor `Solution(IContactStore contacts)`;
- a public instance method `Import(IReadOnlyList<ContactRow> rows)`. Choose its return type yourself.

## Behaviour

For each row, in order:

1. Check it: `Name` must not be empty or whitespace, and `Email` must contain exactly one `@` with at least one character on each side. An invalid row is an expected failure for that row.
2. Save it with `IContactStore.Save` (trimmed name and email). The store throws `DuplicateContactException` when the email already exists; that is an expected failure for that row.

The caller shows the person who uploaded the file what was imported and what was not, so it needs every saved `Contact` and, separately, every failed row with its line number and the reason it failed.

## Provided types

The types below already exist in the global namespace of the project your code is compiled into. Use them as they are: do not redeclare, copy or modify them.

{{FIXTURE}}

## Your answer

- Reply with one ```csharp code block containing everything you write: the `using` directives, `Solution`, and any supporting types you add.
- Implicit usings are off, so include every `using` directive you need. Nullable reference types are enabled.
- Do not write a `Main` method, top-level statements or tests.
