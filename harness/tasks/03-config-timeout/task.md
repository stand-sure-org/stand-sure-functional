---
entry-method: GetTimeout
---
# Read an optional timeout setting with a fallback

A service reads its HTTP timeout from configuration through `IConfigSource`. The setting is optional: most deployments leave it unset and get a fallback value. When it is set, it is a whole number of seconds.

## What to write

A public class named `Solution` with:

- a public constructor `Solution(IConfigSource config)`;
- a public instance method `GetTimeout(string key, TimeSpan fallback)`. Choose its return type yourself.

## Behaviour

- If the key is not set, or its value is empty or whitespace, the timeout is `fallback`.
- If the value is set, it must parse as an integer number of seconds (invariant culture, surrounding whitespace allowed) between 1 and 3600 inclusive; the timeout is that many seconds.
- A value that is set but is not a valid number of seconds is an expected failure, typically a typo in a deployment. Do not silently use the fallback for it: the caller needs to know which key had which bad value so it can report it.

## Provided types

The types below already exist in the global namespace of the project your code is compiled into. Use them as they are: do not redeclare, copy or modify them.

{{FIXTURE}}

## Your answer

- Reply with one ```csharp code block containing everything you write: the `using` directives, `Solution`, and any supporting types you add.
- Implicit usings are off, so include every `using` directive you need. Nullable reference types are enabled.
- Do not write a `Main` method, top-level statements or tests.
