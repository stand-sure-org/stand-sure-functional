# StandSure.Functional: API summary

Implicit usings may be off: write `using StandSure.Functional;` and a `using` for every other namespace you use (`System`, `System.Collections.Generic`, `System.Linq`, `System.Threading`, `System.Threading.Tasks`). The NuGet package also adds `StandSure.Functional` as a global using, so the explicit one is harmless. No library method throws for an expected failure (malformed JSON still throws `JsonException`).
Expected failures are values: `Result<T>` holds an `Error` or a `T`; `Option<T>` holds `None` or a `T`.

- `Result<T>`: `IsSuccess`, `IsFailure`, `Match`, `Map`, `Bind`, `MapError`, `Ensure(predicate, v => error)`, `Tap`, `TapError`, `GetValueOrDefault(fallback)`, `Deconstruct`, `TryGetValue`.
  Create with `Result.Success(value)` or `Result.Failure<T>(error)`, or just return a `T` or an `Error` (also from an `async` method returning `Task<Result<T>>`). `Result.Success()` gives `Result<Unit>`, the "void" result.
- `Error`: `Kind` (`ErrorKind.Validation`, `NotFound`, `Conflict`, `Unauthorized`, `Forbidden`, `Unavailable`, `Unexpected`), `Code` (dotted and constant: `book.title.required`; ids and values go in `Metadata`, never in the code), `Message`, `Metadata`, `Inner`, `Errors`, `Exception` (set by `Try`/`FromException`).
  One factory per kind: `Error.Validation(code, message)`, `Error.NotFound(code, message)`, ...; `Error.Validation(code, message, errors)` groups several; `Error.FromException(ex)`. `error.Wrap(code, message)` adds context (same `Kind`, old error becomes `Inner`); `error.Is(code)` searches the `Inner` chain.
  Building an `Error` records a failure: give `Ensure`, `EnsureAsync` and `ToResult` a factory (`v => Error.X(...)`), which runs only on failure.
- `Option<T>`: `Option.Some(x)`, `Option<T>.None`, or a `T` directly. `IsSome`, `IsNone`, `Match`, `Map`, `Bind`, `Filter`, `GetValueOrDefault(fallback)`, `ToResult(ifNone)` (`ifNone`: `() => Error.NotFound(...)`).
- `Try.Run(() => ...)` and `Try.RunAsync(ct => ..., ct)` turn exceptions into failures of kind `Unexpected`.
- On `Task<Result<T>>` only (from a `Result<T>`, use `Task.FromResult(result)`): `MapAsync`, `BindAsync`, `EnsureAsync`, `MatchAsync`.
- Sequences: `results.Combine()`, `items.Traverse(f)`, `results.Partition()`, `options.Choose()`, `items.Pairwise()`, `items.Scan(seed, f)`. A `Combine`/`Traverse` failure holds every child in `Errors`; its kind is the children's when they agree, else `Unexpected`, then `Unavailable`, then `Validation`.

**Failure comes first:** `Match(onFailure, onSuccess)` and `Match(onNone, onSome)`. Always pass them as named arguments.

## Return a value or an error

```csharp
using System.Collections.Generic;
using StandSure.Functional;

public Result<Book> AddBook(string title, string author)
{
    if (title.Length == 0)
    {
        return Error.Validation("book.title.required", "Title is required.");
    }
    return shelf.Add(new Book(title, author)); // a Book converts to a success
}

public Result<Pet> Find(int id) =>
    kennel.Lookup(id) is { } pet ? pet : Error.NotFound("pet.not_found", $"No pet {id}.");

// Report every problem at once (children go in Error.Errors). For results of one type, use checks.Combine().
var errors = new List<Error>();
if (input.Title.Length == 0) { errors.Add(Error.Validation("title.required", "Title is required.")); }
if (input.Year < 1888) { errors.Add(Error.Validation("year.too_early", "Year must be 1888 or later.")); }
return errors.Count > 0 ? Error.Validation("movie.invalid", "The movie has errors.", errors) : new Movie(input.Title, input.Year);
```

## Optional values: fallback; memory, then disk, then default

```csharp
Option<string> raw = env.Read("RETRIES"); // string? converts; null becomes None
Result<int> retries = raw.Match<Result<int>>( // missing: default 3; unparseable: a failure, not a silent default
    onNone: () => 3,
    onSome: s => int.TryParse(s, out var n) ? n : Error.Validation("retries.invalid", $"'{s}' is not a number."));

Option<Theme> remembered = memory.Find(key); // Theme? converts; null becomes None
Theme theme = remembered.Match(onNone: () => disk.Find(key) ?? Theme.Default, onSome: t => t);
```

## Exceptions and async with a CancellationToken

```csharp
Result<PrintJob> job = Try.Run(() => printer.Print(document))
    .MapError(e => e.Exception is TimeoutException ? Error.Unavailable("print.no_response", "The printer did not respond.")
        : e.Wrap("print.failed", "Printing the document failed."));

public Task<Result<Forecast>> ForecastAsync(string city, CancellationToken ct) =>
    Try.RunAsync(token => weather.FindAsync(city, token), ct)   // Task<Result<Forecast?>>
        .BindAsync(f => f is { } found ? Result.Success(found)
            : Result.Failure<Forecast>(Error.NotFound("forecast.not_found", $"No forecast for {city}.")));
```

## Chain steps; add context as a failure moves up; Go-style Deconstruct

```csharp
Result<Ticket> ticket = Parse(request)                   // Result<Request>; stops at the first failure
    .Ensure(r => r.Seats > 0, r => Error.Validation("request.no_seats", $"Asked for {r.Seats} seats."))
    .Bind(HoldSeats)                                     // Request -> Result<Hold>
    .Bind(Issue);                                        // Hold -> Result<Ticket>

Result<Recipe> Get(int id) => cookbook.Load(id)
    .MapError(e => e.Wrap("recipe.get_failed", $"Could not get recipe {id}."));
var (recipe, error) = kitchen.Get(id); // Go style
if (error is not null)
{
    return error.Is("recipe.not_found") ? "missing" : error.Message;
}
return recipe!.Title;
```

## Consume: HTTP response, switch

```csharp
app.MapGet("/flights/{id}", (int id, FlightService flights) => flights.Get(id).Match(
    onFailure: e => e.Kind switch
    {
        ErrorKind.NotFound => Results.NotFound(e.Message),
        ErrorKind.Validation => Results.BadRequest(e.Message),
        ErrorKind.Conflict => Results.Conflict(e.Message),
        _ => Results.Problem(e.Message),
    },
    onSuccess: flight => Results.Ok(flight)));

string text = result switch { Error e => e.Code, Book b => b.Title }; // exhaustive, no discard needed
int pages = await LoadAsync(id).MatchAsync(onFailure: e => 0, onSuccess: b => b.Pages);
```

## Sequences: split outcomes, sliding window, running totals

```csharp
var (loaded, broken) = files.Select(LoadPhoto).Partition(); // IReadOnlyList<Photo>, IReadOnlyList<Error>
IEnumerable<double> changes = temperatures.Pairwise().Select(p => p.Current - p.Previous);
IEnumerable<int> totals = steps.Scan(0, (sum, x) => sum + x); // [1, 2, 3] -> 1, 3, 6
```

## Traps

- Inside a lambda (`Bind`, `BindAsync`, `Match`), `c ? value : Error.X(...)` has no type: use `Result.Success(v)` / `Result.Failure<T>(e)`, or give the lambda a return type: `Bind(Result<U> (x) => ...)` or `Match<Result<U>>(...)`.
- A null reference converts to `Option<T>` as None, but a nullable value type (`int?`) does not convert: `n is { } v ? v : Option<int>.None`.
- `MatchAsync`: both lambdas sync, or both async. Mixing them does not compile.
- For side effects, `Match` also takes two `void` lambdas (`onFailure: e => failed.Add(e), onSuccess: v => saved.Add(v)`) and returns `Unit`. After a `Result<Unit>` step, `.Bind(() => next())` needs no parameter.
- `result.Map(async x => ...)` gives `Result<Task<U>>`: use `MapAsync` on a `Task<Result<T>>`. If the lambda returns a `Result`, use `BindAsync` (`MapAsync` would nest `Result<Result<U>>`).
- `switch` needs type patterns, `Error e => ..., T x => ...`, and is exhaustive without `_`. A `Result<Error>` always matches `Error e` in a `switch`: use `IsSuccess` or `Deconstruct`.
- `default(Result<T>)` is a failure holding `Error.Uninitialized`. `default(Option<T>)` is None.
- `Error` is not an exception: `e is IOException x` and `e switch { IOException x => ... }` do not compile, and `e is IOException` is always false. The exception `Try` caught is `e.Exception`: `e.Exception is IOException x ? ... : e`.
- `Try.Run` / `Try.RunAsync` never catch `OperationCanceledException`: cancellation still throws.
- `Try.Run(async () => ...)` does not compile (it would succeed before the work ran): use `Try.RunAsync(ct => ..., ct)`.
- `.Value` is the untyped union payload (`object?`): never use it. Read values with `Match`, `Deconstruct`, `TryGetValue` or `GetValueOrDefault`.
