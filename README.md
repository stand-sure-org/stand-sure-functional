# StandSure.Functional

`Result<T>`, `Option<T>`, `Error`, `Unit` and `Try` for C# 15 / .NET 11. No runtime dependencies beyond the BCL.

## Why

Expected failures (not found, invalid input, a dependency that is down) are ordinary outcomes, not exceptions.
This library makes them values you return, chain and match, so the compiler shows you every place a failure can go.

- `Result<T>` and `Option<T>` are non-boxing C# 15 unions (`readonly struct`), so `switch` on them is exhaustive without a discard.
- Failure comes first in every two-branch call: `Match(onFailure, onSuccess)`, `Match(onNone, onSome)`.
- No library method throws. There is no throwing `.Value` accessor. `default(Result<T>)` is a failure, not a crash.
- `Error` carries a kind, a dotted code, a message, metadata, an inner error chain and child errors. It serializes to plain JSON.
- Failures are recorded once, at creation, through `ActivitySource` and `Meter` named `StandSure.Functional`.
  Messages and metadata are emitted only when the `AppContext` switch `StandSure.Functional.EmitErrorDetails` is on.
- The API names come from measuring what language models write unprompted (see `harness/results/baseline-report.md`).

## Install

```bash
dotnet add package StandSure.Functional
```

The package adds `global using StandSure.Functional;` to your project, so you don't need a using directive.
To opt out, add `<Using Remove="StandSure.Functional" />` to an `ItemGroup` in your project file.

## Quickstart

```csharp
public sealed record User(int Id, string Email);

public static class Users
{
    public static Result<User> Register(int id, string email)
    {
        if (!email.Contains('@'))
        {
            return Error.Validation("user.email.invalid", "Email must contain '@'.");
        }

        return new User(id, email); // a User converts to a success
    }
}

string message = Users.Register(1, "ada@example.com").Match(
    onFailure: error => $"{error.Code}: {error.Message}",
    onSuccess: user => $"Welcome, user {user.Id}.");
```

Always name the `Match` arguments: failure comes first, and most other libraries put success first.

## Documentation

- [`docs/llm-api-summary.md`](docs/llm-api-summary.md): the whole API on one page, with an example per common task and
  the known traps. It is also the page given to language models in the with-library harness condition.
- XML documentation ships with the package; every public member is documented.

## Building

```bash
dotnet build -c Release   # 0 warnings; warnings are errors
dotnet test               # all suites, including the package consumer test
```

The library packs itself on every build into `artifacts/packages/` (gitignored).
`tests/StandSure.Functional.PackageTests` restores that package into a throwaway project with no using directives,
with its own package cache, and checks that it compiles.

## Credits

Inspired by [language-ext](https://github.com/louthy/language-ext) by Paul Louth (MIT), whose `Unit`, `Option` and
`Either` shaped how many C# developers think about this. No code was copied; this is an independent, much smaller design.

## License

MIT.

## Built by Claude Code

🤖 This library was built entirely by Claude Code: the code, tests, model-usage harness and docs.
The owner set the direction and approved the design decisions.

![Built by Claude Code](docs/badges/claude-code-badge.svg)
