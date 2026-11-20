---
entry-method: Register
---
# Validate a sign-up form and report every problem

A sign-up form has several fields. When the form has problems, the person filling it in should see all of them at once, so they can fix the whole form in one go rather than one field per attempt.

## What to write

A public class named `Solution` with:

- a public constructor `Solution(IAccountStore accounts)`;
- a public instance method `Register(SignupForm form)`. Choose its return type yourself.

## Behaviour

Check every field, and collect every problem found rather than stopping at the first:

- `Username`: 3 to 20 characters; letters, digits and underscores only.
- `Email`: contains exactly one `@`, with at least one character on each side.
- `Password`: at least 12 characters, with at least one letter and at least one digit.
- `Age`: at least 13.

Invalid fields are expected failures. Each problem reported must say which field it is about and what is wrong with it. Only when every field is valid, create the account with `IAccountStore.Create` and give the caller the created `Account`.

## Provided types

The types below already exist in the global namespace of the project your code is compiled into. Use them as they are: do not redeclare, copy or modify them.

{{FIXTURE}}

## Your answer

- Reply with one ```csharp code block containing everything you write: the `using` directives, `Solution`, and any supporting types you add.
- Implicit usings are off, so include every `using` directive you need. Nullable reference types are enabled.
- Do not write a `Main` method, top-level statements or tests.
