# StandSure.Functional

`Result<T>`, `Option<T>`, `Error`, `Unit` and `Try` for C# 15 / .NET 11. No runtime dependencies beyond the BCL.

## Why

Expected failures (not found, invalid input, a dependency that is down) are ordinary outcomes, not exceptions.
This library makes them values you return, chain and match, so the compiler shows you every place a failure can go.

- `Result<T>` and `Option<T>` are non-boxing C# 15 unions (`readonly struct`), so `switch` on them is exhaustive without a discard.
- Failure comes first in every two-branch call: `Match(onFailure, onSuccess)`, `Match(onNone, onSome)`.
- No library method throws for an expected failure: bad arguments and null delegates become failures. There is no throwing
  `.Value` accessor, and `default(Result<T>)` is a failure, not a crash. Two documented exceptions: JSON (de)serialization of
  malformed input throws `JsonException` (the System.Text.Json contract), and `AddFunctional` validates its argument
  (it throws for `null`, or for options that are already in use).
- `Error` carries a kind, a dotted code, a message, metadata, an inner error chain and child errors. It serializes to plain JSON.
- Failures are recorded once, at creation, through `ActivitySource` and `Meter` named `StandSure.Functional`:
  event `functional.failure` and counter `functional.failures` (tags `error.kind`, `error.code`) for a root failure;
  event `functional.failure.wrapped` for `Wrap`, and `functional.failure.grouped` (tag `error.errors.count`) for a parent
  that holds child errors (`Error.Validation(code, message, errors)`, `Combine`, `Traverse`). Wraps and groups are not
  counted again, so `Combine` of N failures counts N. Prefer the factory overloads `Ensure(predicate, v => error)`,
  `EnsureAsync(..., v => error)` and `ToResult(() => error)`: an `Error` passed directly is built, and recorded, even
  when the check passes.
  Messages and metadata are emitted only when the `AppContext` switch `StandSure.Functional.EmitErrorDetails` is on.
- Every method that runs your delegate (`Map`, `Bind`, `Match`, `Ensure`, `Tap`, `Traverse`, `Try.Run`, the `*Async`
  forms, and the `Option` equivalents) starts a span on the `StandSure.Functional` source, named after the method. The
  spans carry no status and no tags. Subscribe with `AddSource("StandSure.Functional")`; with no subscriber they cost
  one listener check. Failure events raised inside your delegate attach to that span.
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
