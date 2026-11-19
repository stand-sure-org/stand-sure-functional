---
entry-method: GetSettings
---
# Look up settings in a cache, then a store, then fall back to defaults

A user's display settings may be in a cache, may only be in the persistent store, or may not exist at all (most users never change their settings). The page always needs some settings to render with.

## What to write

A public class named `Solution` with:

- a public constructor `Solution(ISettingsCache cache, ISettingsStore store)`;
- a public instance method `GetSettings(Guid userId)`. Choose its return type yourself.

## Behaviour

1. Look in the cache (`ISettingsCache.Get`). On a hit, use those settings.
2. On a cache miss, load from the store (`ISettingsStore.Load`). If the store has settings, put them in the cache (`ISettingsCache.Set`) and use them.
3. If neither has settings for the user, use `UserSettings.Default`. Do not cache the default.

A user with no saved settings is an expected, ordinary outcome.

## Provided types

The types below already exist in the global namespace of the project your code is compiled into. Use them as they are: do not redeclare, copy or modify them.

{{FIXTURE}}

## Your answer

- Reply with one ```csharp code block containing everything you write: the `using` directives, `Solution`, and any supporting types you add.
- Implicit usings are off, so include every `using` directive you need. Nullable reference types are enabled.
- Do not write a `Main` method, top-level statements or tests.
