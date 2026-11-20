---
entry-method: CreateUser
---
# Create a user

A sign-up service receives an email address and a display name from a web form. It must check them and, if they are acceptable, register the new user through `IUserStore`.

## What to write

A public class named `Solution` with:

- a public constructor `Solution(IUserStore users)`;
- a public instance method `CreateUser(string email, string displayName)`. Choose its return type yourself.

## Behaviour

1. Trim both inputs, then check them:
   - the email is required, at most 254 characters, and contains exactly one `@` with at least one character before it and a `.` somewhere after it;
   - the display name is required and at most 100 characters.
2. If the email is already registered (`IUserStore.EmailExists`), the user cannot be created.
3. Otherwise add the user with the trimmed values (`IUserStore.Add`) and give the caller the created `User`.

Invalid input and an already-registered email are expected failures: the caller shows them to the person filling in the form, so it needs to know what went wrong.

## Provided types

The types below already exist in the global namespace of the project your code is compiled into. Use them as they are: do not redeclare, copy or modify them.

{{FIXTURE}}

## Your answer

- Reply with one ```csharp code block containing everything you write: the `using` directives, `Solution`, and any supporting types you add.
- Implicit usings are off, so include every `using` directive you need. Nullable reference types are enabled.
- Do not write a `Main` method, top-level statements or tests.
