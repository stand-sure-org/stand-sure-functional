# StandSure.Functional: API summary

Namespace `StandSure.Functional`: write `using StandSure.Functional;` (implicit usings may be off). No library method throws.
Expected failures are values: `Result<T>` holds an `Error` or a `T`; `Option<T>` holds `None` or a `T`.

- `Result<T>`: `IsSuccess`, `IsFailure`, `Match`, `Map`, `Bind`, `MapError`, `Ensure`, `Tap`, `GetValueOrDefault(fallback)`, `Deconstruct`, `TryGetValue`.
  Create with `Result.Success(value)` or `Result.Failure<T>(error)`, or just return a `T` or an `Error` (also from an `async` method returning `Task<Result<T>>`). `Result.Success()` gives `Result<Unit>`, the "void" result.
- `Error`: `Kind` (`ErrorKind.Validation`, `NotFound`, `Conflict`, `Unauthorized`, `Forbidden`, `Unavailable`, `Unexpected`), `Code` (dotted: `book.title.required`), `Message`, `Metadata`, `Inner`, `Errors`.
  One factory per kind: `Error.Validation(code, message)`, `Error.NotFound(code, message)`, ...; `Error.Validation(code, message, errors)` groups several; `Error.FromException(ex)`.
  `error.Wrap(code, message)` adds context (same `Kind`, old error becomes `Inner`); `error.Is(code)` searches the `Inner` chain.
- `Option<T>`: `Option.Some(x)`, `Option<T>.None`, or a `T` directly. `IsSome`, `IsNone`, `Match`, `Map`, `Bind`, `Filter`, `GetValueOrDefault(fallback)`, `ToResult(errorIfNone)`.
- `Try.Run(() => ...)` and `Try.RunAsync(ct => ..., ct)` turn exceptions into failures of kind `Unexpected`.
- On `Task<Result<T>>` only: `MapAsync`, `BindAsync`, `EnsureAsync`, `MatchAsync`.
- Sequences: `results.Combine()`, `items.Traverse(f)`, `results.Partition()`, `options.Choose()`, `items.Pairwise()`, `items.Scan(seed, f)`.

**Failure comes first:** `Match(onFailure, onSuccess)` and `Match(onNone, onSome)`. Always pass them as named arguments.

## Return a value or an error

```csharp
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
Option<string> raw = env.Read("RETRIES") is { } text ? text : Option<string>.None;
int retries = raw.Bind(ParseInt).GetValueOrDefault(3);
static Option<int> ParseInt(string s) => int.TryParse(s, out var n) ? n : Option<int>.None;

Option<Theme> remembered = memory.Find(key) is { } hit ? hit : Option<Theme>.None;
Theme theme = remembered.Match(
    onNone: () => disk.Find(key) ?? Theme.Default,
    onSome: t => t);
```

## Exceptions and async with a CancellationToken

```csharp
Result<PrintJob> job = Try.Run(() => printer.Print(document))
    .MapError(e => e.Wrap("print.failed", "Printing the document failed."));

public Task<Result<Forecast>> ForecastAsync(string city, CancellationToken ct) =>
    Try.RunAsync(token => weather.GetTemperatureAsync(city, token), ct)   // Task<Result<double>>
        .EnsureAsync(t => t > -100, Error.Unavailable("forecast.no_data", "No temperature returned."))
        .MapAsync(t => new Forecast(city, t));
```

## Chain steps; add context as a failure moves up; Go-style Deconstruct

```csharp
Result<Ticket> ticket = Parse(request)                   // Result<Request>; stops at the first failure
    .Ensure(r => r.Seats > 0, Error.Validation("request.no_seats", "Ask for at least one seat."))
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
string label = result.Match(onFailure: e => e.Message, onSuccess: b => b.Title);
int pages = await LoadAsync(id).MatchAsync(onFailure: e => 0, onSuccess: b => b.Pages);
```

## Sequences: split outcomes, sliding window, running totals

```csharp
var (loaded, broken) = files.Select(LoadPhoto).Partition(); // IReadOnlyList<Photo>, IReadOnlyList<Error>
IEnumerable<double> changes = temperatures.Pairwise().Select(p => p.Current - p.Previous);
IEnumerable<int> totals = steps.Scan(0, (sum, x) => sum + x); // [1, 2, 3] -> 1, 3, 6
```

## Traps

- Positional `Match` arguments are failure first. Name them: `onFailure:` / `onSuccess:`, `onNone:` / `onSome:`.
- `MatchAsync`: both lambdas sync, or both async. Mixing them does not compile.
- `result.Map(async x => ...)` gives `Result<Task<U>>`: use `MapAsync` on a `Task<Result<T>>`. If the lambda returns a `Result`, use `BindAsync` (`MapAsync` would nest `Result<Result<U>>`).
- The `...Async` methods extend `Task<Result<T>>` only. From a `Result<T>`, use `Task.FromResult(result)`.
- `switch` needs type patterns, `Error e => ..., T x => ...`, and is exhaustive without `_`. A `Result<Error>` can't be observed by `switch`: use `IsSuccess` or `Deconstruct`.
- `default(Result<T>)` is a failure holding `Error.Uninitialized`. `default(Option<T>)` is None.
- `Try.Run` / `Try.RunAsync` never catch `OperationCanceledException`: cancellation still throws.
- There is no throwing `.Value` / `.Error`. Read values with `Match`, `Deconstruct`, `TryGetValue` or `GetValueOrDefault`.
