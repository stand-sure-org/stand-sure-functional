# Harness report: `20261119T184248Z-with-library.reextracted.jsonl`

Records: 420 | Compared with: `20261119T162804Z-baseline.reextracted.jsonl`

## Data quality

Read this first. Records with a transport error, a truncated model answer or a runner error (a failure of the harness itself after the model answered) are counted here but are EXCLUDED from the compile-rate and probe-rate denominators below. `Compile failed` and `Probe crashed` count eligible (non-excluded) records only; `extractFailed` is one cause of a compile failure.

| Family | Records | Transport errors | Truncated | Runner errors | Extract failed | Concatenated | Compile failed | Probe crashed |
|---|---|---|---|---|---|---|---|---|
| anthropic | 180 | 0 | 0 | 0 | 0 | 2 | 16 | 0 |
| gemini | 120 | 0 | 0 | 0 | 0 | 0 | 4 | 0 |
| ollama | 120 | 3 | 0 | 0 | 0 | 0 | 59 | 0 |
| overall | 420 | 3 | 0 | 0 | 0 | 2 | 79 | 0 |

Malformed lines skipped: 0
Malformed lines skipped in `20261119T162804Z-baseline.reextracted.jsonl`: 0

## 1. Compile rate

Denominator n = records that are not transport errors, not truncated and not runner errors (those are excluded; see Data quality). Compile rate = compiled / n.

| Family | n (eligible) | Compiled | Compile rate |
|---|---|---|---|
| anthropic | 180 | 164 | 91.1% |
| gemini | 120 | 116 | 96.7% |
| ollama | 117 | 58 | 49.6% |
| overall | 417 | 338 | 81.1% |

## 2. Probe outcome rates

Denominator n = eligible records that compiled and have a probe result (transport-error, truncated and runner-error records excluded). `crashed` means no verdict and stays in n. `compiled, not probed` counts eligible records that compiled but have no probe result; they are not in n.

| Family | n (probed) | threw | returned | timeout | crashed | compiled, not probed |
|---|---|---|---|---|---|---|
| anthropic | 164 | 0 (0.0%) | 164 (100.0%) | 0 (0.0%) | 0 (0.0%) | 0 |
| gemini | 116 | 0 (0.0%) | 116 (100.0%) | 0 (0.0%) | 0 (0.0%) | 0 |
| ollama | 58 | 0 (0.0%) | 58 (100.0%) | 0 (0.0%) | 0 (0.0%) | 0 |
| overall | 338 | 0 (0.0%) | 338 (100.0%) | 0 (0.0%) | 0 (0.0%) | 0 |

## 3. Entry return type frequency

Records with extracted usage facts (n); `(none)` = no entry method found.

### anthropic (n = 180)

| Return type | Records | Share |
|---|---|---|
| `ImportReport` | 15 | 8.3% |
| `Result<Account>` | 15 | 8.3% |
| `Result<IReadOnlyList<UsagePeriod>>` | 15 | 8.3% |
| `Result<Invoice>` | 15 | 8.3% |
| `Result<Order>` | 15 | 8.3% |
| `Result<PaymentReceipt>` | 15 | 8.3% |
| `Result<TimeSpan>` | 15 | 8.3% |
| `Result<User>` | 15 | 8.3% |
| `Task<Result<ShippingQuote>>` | 15 | 8.3% |
| `UserSettings` | 15 | 8.3% |
| `Result<Product>` | 14 | 7.8% |
| `IResult` | 8 | 4.4% |
| `Results<Ok<OrderSummary>, BadRequest<string>, NotFound<string>, ProblemHttpResult>` | 4 | 2.2% |
| `Results<Ok<OrderSummary>, BadRequest<string>, NotFound<string>>` | 2 | 1.1% |
| `Option<Product>` | 1 | 0.6% |
| `Results<Ok<OrderSummary>, BadRequest<ProblemDetails>, NotFound<ProblemDetails>, ProblemHttpResult>` | 1 | 0.6% |

### gemini (n = 120)

| Return type | Records | Share |
|---|---|---|
| `IResult` | 10 | 8.3% |
| `Result<Account>` | 10 | 8.3% |
| `Result<IReadOnlyList<UsagePeriod>>` | 10 | 8.3% |
| `Result<Invoice>` | 10 | 8.3% |
| `Result<Order>` | 10 | 8.3% |
| `Result<PaymentReceipt>` | 10 | 8.3% |
| `Result<TimeSpan>` | 10 | 8.3% |
| `Result<User>` | 10 | 8.3% |
| `Task<Result<ShippingQuote>>` | 10 | 8.3% |
| `UserSettings` | 10 | 8.3% |
| `ImportResult` | 6 | 5.0% |
| `Option<Product>` | 5 | 4.2% |
| `Result<Product>` | 5 | 4.2% |
| `(IReadOnlyList<Contact> Imported, IReadOnlyList<FailedRow> Failed)` | 4 | 3.3% |

### ollama (n = 117)

| Return type | Records | Share |
|---|---|---|
| `Result<Account>` | 10 | 8.5% |
| `Result<IReadOnlyList<UsagePeriod>>` | 10 | 8.5% |
| `Result<Invoice>` | 10 | 8.5% |
| `Result<Order>` | 10 | 8.5% |
| `Result<PaymentReceipt>` | 10 | 8.5% |
| `Result<Product>` | 10 | 8.5% |
| `Result<TimeSpan>` | 10 | 8.5% |
| `Task<Result<ShippingQuote>>` | 10 | 8.5% |
| `Result<User>` | 7 | 6.0% |
| `UserSettings` | 6 | 5.1% |
| `IResult` | 5 | 4.3% |
| `Results` | 5 | 4.3% |
| `Option<UserSettings>` | 3 | 2.6% |
| `Result<(IReadOnlyList<Contact> Successful, IReadOnlyList<(ContactRow Row, Error Error)> Failed)>` | 3 | 2.6% |
| `Result<(IReadOnlyList<Contact> imported, IReadOnlyList<(int lineNumber, Error error)> failed)>` | 3 | 2.6% |
| `Result<(IReadOnlyList<Contact> Successes, IReadOnlyList<(ContactRow Row, Error Error)> Failures)>` | 2 | 1.7% |
| `(IReadOnlyList<Contact> imported, IReadOnlyList<(int lineNumber, Error error)> failed)` | 1 | 0.9% |
| `Result<(IReadOnlyList<Contact> imported, IReadOnlyList<(ContactRow row, Error error)> failed)>` | 1 | 0.9% |
| `Result<UserSettings>` | 1 | 0.9% |

### overall (n = 417)

| Return type | Records | Share |
|---|---|---|
| `Result<Account>` | 35 | 8.4% |
| `Result<IReadOnlyList<UsagePeriod>>` | 35 | 8.4% |
| `Result<Invoice>` | 35 | 8.4% |
| `Result<Order>` | 35 | 8.4% |
| `Result<PaymentReceipt>` | 35 | 8.4% |
| `Result<TimeSpan>` | 35 | 8.4% |
| `Task<Result<ShippingQuote>>` | 35 | 8.4% |
| `Result<User>` | 32 | 7.7% |
| `UserSettings` | 31 | 7.4% |
| `Result<Product>` | 29 | 7.0% |
| `IResult` | 23 | 5.5% |
| `ImportReport` | 15 | 3.6% |
| `ImportResult` | 6 | 1.4% |
| `Option<Product>` | 6 | 1.4% |
| `Results` | 5 | 1.2% |
| `(IReadOnlyList<Contact> Imported, IReadOnlyList<FailedRow> Failed)` | 4 | 1.0% |
| `Results<Ok<OrderSummary>, BadRequest<string>, NotFound<string>, ProblemHttpResult>` | 4 | 1.0% |
| `Option<UserSettings>` | 3 | 0.7% |
| `Result<(IReadOnlyList<Contact> Successful, IReadOnlyList<(ContactRow Row, Error Error)> Failed)>` | 3 | 0.7% |
| `Result<(IReadOnlyList<Contact> imported, IReadOnlyList<(int lineNumber, Error error)> failed)>` | 3 | 0.7% |
| `Result<(IReadOnlyList<Contact> Successes, IReadOnlyList<(ContactRow Row, Error Error)> Failures)>` | 2 | 0.5% |
| `Results<Ok<OrderSummary>, BadRequest<string>, NotFound<string>>` | 2 | 0.5% |
| `(IReadOnlyList<Contact> imported, IReadOnlyList<(int lineNumber, Error error)> failed)` | 1 | 0.2% |
| `Result<(IReadOnlyList<Contact> imported, IReadOnlyList<(ContactRow row, Error error)> failed)>` | 1 | 0.2% |
| `Result<UserSettings>` | 1 | 0.2% |
| `Results<Ok<OrderSummary>, BadRequest<ProblemDetails>, NotFound<ProblemDetails>, ProblemHttpResult>` | 1 | 0.2% |

## 4. Idiom frequency

Records with extracted usage facts (n); a record can carry several idioms, so shares do not sum to 100%.

### anthropic (n = 180)

| Idiom | Records | Share |
|---|---|---|
| `library-result` | 170 | 94.4% |
| `exception` | 39 | 21.7% |
| `result-like-type` | 1 | 0.6% |

### gemini (n = 120)

| Idiom | Records | Share |
|---|---|---|
| `library-result` | 120 | 100.0% |
| `exception` | 24 | 20.0% |
| `result-like-type` | 6 | 5.0% |

### ollama (n = 117)

| Idiom | Records | Share |
|---|---|---|
| `library-result` | 111 | 94.9% |
| `exception` | 16 | 13.7% |

### overall (n = 417)

| Idiom | Records | Share |
|---|---|---|
| `library-result` | 401 | 96.2% |
| `exception` | 79 | 18.9% |
| `result-like-type` | 7 | 1.7% |

## 5. Top 30 invoked method names on outcome-like types

Records invoking the method (n = records with usage facts). Only `Type.Method` entries whose type is outcome-like (name contains Result, Option, Outcome or Either, or a library type); ranked by count, ties alphabetical.

### anthropic (n = 180)

| Rank | Method | Records | Share |
|---|---|---|---|
| 1 | `Result.Bind` | 52 | 28.9% |
| 2 | `Result.MapError` | 44 | 24.4% |
| 3 | `Result.Success` | 37 | 20.6% |
| 4 | `Result.Map` | 24 | 13.3% |
| 5 | `Result.Failure` | 19 | 10.6% |
| 6 | `Option.Match` | 17 | 9.4% |
| 7 | `Result.Match` | 15 | 8.3% |
| 8 | `Results.NotFound` | 8 | 4.4% |
| 9 | `Results.Ok` | 8 | 4.4% |
| 10 | `Option.Filter` | 7 | 3.9% |
| 11 | `Results.BadRequest` | 7 | 3.9% |
| 12 | `Results.Problem` | 7 | 3.9% |
| 13 | `TypedResults.BadRequest` | 7 | 3.9% |
| 14 | `TypedResults.NotFound` | 7 | 3.9% |
| 15 | `TypedResults.Ok` | 7 | 3.9% |
| 16 | `ResultTaskExtensions.BindAsync` | 5 | 2.8% |
| 17 | `TypedResults.Problem` | 5 | 2.8% |
| 18 | `Result.Ensure` | 3 | 1.7% |
| 19 | `Option.Some` | 2 | 1.1% |
| 20 | `Option.ToResult` | 1 | 0.6% |
| 21 | `ResultTaskExtensions.MapAsync` | 1 | 0.6% |
| 22 | `Results.Conflict` | 1 | 0.6% |

### gemini (n = 120)

| Rank | Method | Records | Share |
|---|---|---|---|
| 1 | `Result.MapError` | 20 | 16.7% |
| 2 | `Option.Match` | 14 | 11.7% |
| 3 | `Result.Match` | 14 | 11.7% |
| 4 | `Result.Map` | 12 | 10.0% |
| 5 | `Results.BadRequest` | 10 | 8.3% |
| 6 | `Results.NotFound` | 10 | 8.3% |
| 7 | `Results.Ok` | 10 | 8.3% |
| 8 | `Results.Problem` | 10 | 8.3% |
| 9 | `Result.Success` | 9 | 7.5% |
| 10 | `Result.Failure` | 8 | 6.7% |
| 11 | `ResultTaskExtensions.BindAsync` | 5 | 4.2% |
| 12 | `Result.Bind` | 3 | 2.5% |
| 13 | `ResultTaskExtensions.MapAsync` | 3 | 2.5% |
| 14 | `Option.Filter` | 2 | 1.7% |
| 15 | `ResultEnumerableExtensions.Traverse` | 2 | 1.7% |

### ollama (n = 117)

| Rank | Method | Records | Share |
|---|---|---|---|
| 1 | `Result.Success` | 31 | 26.5% |
| 2 | `Result.MapError` | 25 | 21.4% |
| 3 | `Result.Map` | 22 | 18.8% |
| 4 | `Result.Bind` | 20 | 17.1% |
| 5 | `Result.Match` | 16 | 13.7% |
| 6 | `Result.Failure` | 15 | 12.8% |
| 7 | `Option.Match` | 5 | 4.3% |
| 8 | `Results.BadRequest` | 5 | 4.3% |
| 9 | `Results.NotFound` | 5 | 4.3% |
| 10 | `Results.Ok` | 5 | 4.3% |
| 11 | `Results.Problem` | 5 | 4.3% |
| 12 | `ResultEnumerableExtensions.Partition` | 4 | 3.4% |
| 13 | `Result.Ensure` | 3 | 2.6% |
| 14 | `Option.GetValueOrDefault` | 1 | 0.9% |

### overall (n = 417)

| Rank | Method | Records | Share |
|---|---|---|---|
| 1 | `Result.MapError` | 89 | 21.3% |
| 2 | `Result.Success` | 77 | 18.5% |
| 3 | `Result.Bind` | 75 | 18.0% |
| 4 | `Result.Map` | 58 | 13.9% |
| 5 | `Result.Match` | 45 | 10.8% |
| 6 | `Result.Failure` | 42 | 10.1% |
| 7 | `Option.Match` | 36 | 8.6% |
| 8 | `Results.NotFound` | 23 | 5.5% |
| 9 | `Results.Ok` | 23 | 5.5% |
| 10 | `Results.BadRequest` | 22 | 5.3% |
| 11 | `Results.Problem` | 22 | 5.3% |
| 12 | `ResultTaskExtensions.BindAsync` | 10 | 2.4% |
| 13 | `Option.Filter` | 9 | 2.2% |
| 14 | `TypedResults.BadRequest` | 7 | 1.7% |
| 15 | `TypedResults.NotFound` | 7 | 1.7% |
| 16 | `TypedResults.Ok` | 7 | 1.7% |
| 17 | `Result.Ensure` | 6 | 1.4% |
| 18 | `TypedResults.Problem` | 5 | 1.2% |
| 19 | `ResultEnumerableExtensions.Partition` | 4 | 1.0% |
| 20 | `ResultTaskExtensions.MapAsync` | 4 | 1.0% |
| 21 | `Option.Some` | 2 | 0.5% |
| 22 | `ResultEnumerableExtensions.Traverse` | 2 | 0.5% |
| 23 | `Option.GetValueOrDefault` | 1 | 0.2% |
| 24 | `Option.ToResult` | 1 | 0.2% |
| 25 | `Results.Conflict` | 1 | 0.2% |

## 6. Self-declared type names

Records with extracted usage facts (n); top 30 per scope, ties alphabetical.

### anthropic (n = 180)

| Type | Records | Share |
|---|---|---|
| `Solution` | 180 | 100.0% |
| `ImportReport` | 15 | 8.3% |
| `PricedOrder` | 14 | 7.8% |
| `RowFailure` | 11 | 6.1% |
| `Found` | 4 | 2.2% |
| `ImportFailure` | 3 | 1.7% |
| `ValidRow` | 3 | 1.7% |
| `InvalidId` | 2 | 1.1% |
| `OrderLookup` | 2 | 1.1% |
| `Draft` | 1 | 0.6% |
| `FailedRow` | 1 | 0.6% |
| `FindOrderOutcome` | 1 | 0.6% |
| `Invalid` | 1 | 0.6% |
| `InvalidOrderId` | 1 | 0.6% |
| `Lookup` | 1 | 0.6% |
| `Missing` | 1 | 0.6% |
| `NotFound` | 1 | 0.6% |
| `NotFoundOrder` | 1 | 0.6% |
| `OrderNotFound` | 1 | 0.6% |
| `Pricing` | 1 | 0.6% |
| `Quote` | 1 | 0.6% |
| `SpanPredicateExtensions` | 1 | 0.6% |
| `StringExtensions` | 1 | 0.6% |
| `ValidContact` | 1 | 0.6% |

### gemini (n = 120)

| Type | Records | Share |
|---|---|---|
| `Solution` | 120 | 100.0% |
| `FailedRow` | 6 | 5.0% |
| `ImportResult` | 6 | 5.0% |
| `ImportFailure` | 2 | 1.7% |
| `OrderPricing` | 2 | 1.7% |
| `RowFailure` | 2 | 1.7% |
| `PricedOrder` | 1 | 0.8% |

### ollama (n = 117)

| Type | Records | Share |
|---|---|---|
| `Solution` | 117 | 100.0% |

### overall (n = 417)

| Type | Records | Share |
|---|---|---|
| `Solution` | 417 | 100.0% |
| `ImportReport` | 15 | 3.6% |
| `PricedOrder` | 15 | 3.6% |
| `RowFailure` | 13 | 3.1% |
| `FailedRow` | 7 | 1.7% |
| `ImportResult` | 6 | 1.4% |
| `ImportFailure` | 5 | 1.2% |
| `Found` | 4 | 1.0% |
| `ValidRow` | 3 | 0.7% |
| `InvalidId` | 2 | 0.5% |
| `OrderLookup` | 2 | 0.5% |
| `OrderPricing` | 2 | 0.5% |
| `Draft` | 1 | 0.2% |
| `FindOrderOutcome` | 1 | 0.2% |
| `Invalid` | 1 | 0.2% |
| `InvalidOrderId` | 1 | 0.2% |
| `Lookup` | 1 | 0.2% |
| `Missing` | 1 | 0.2% |
| `NotFound` | 1 | 0.2% |
| `NotFoundOrder` | 1 | 0.2% |
| `OrderNotFound` | 1 | 0.2% |
| `Pricing` | 1 | 0.2% |
| `Quote` | 1 | 0.2% |
| `SpanPredicateExtensions` | 1 | 0.2% |
| `StringExtensions` | 1 | 0.2% |
| `ValidContact` | 1 | 0.2% |

## 7. Per-task notes: regressions versus --compare

Rule: a family/task cell regresses when its compile-failure rate or its `threw` rate is worse than in the compare file by more than 10 percentage points, with n >= 3 on both sides. Compile n is eligible records; threw n is probed records (see sections 1 and 2). Families are checked individually, not overall.

- **anthropic / 04-charge-card**: compile rate regressed: 93.3% (14/15) -> 73.3% (11/15)
- **anthropic / 08-user-settings**: compile rate regressed: 100.0% (15/15) -> 86.7% (13/15)
- **gemini / 10-import-contacts**: compile rate regressed: 100.0% (10/10) -> 60.0% (6/10)
- **ollama / 01-create-user**: compile rate regressed: 90.0% (9/10) -> 28.6% (2/7)
- **ollama / 03-config-timeout**: compile rate regressed: 100.0% (10/10) -> 50.0% (5/10)
- **ollama / 04-charge-card**: compile rate regressed: 60.0% (6/10) -> 40.0% (4/10)
- **ollama / 07-order-endpoint**: compile rate regressed: 30.0% (3/10) -> 0.0% (0/10)
- **ollama / 10-import-contacts**: compile rate regressed: 100.0% (9/9) -> 0.0% (0/10)
- **ollama / 11-invoice-layers**: compile rate regressed: 70.0% (7/10) -> 40.0% (4/10)

## Caveats

- Task 08 is a low-signal cell for the throw/no-throw probe: it has no failure path, so `threw` there says little about expected-failure handling. In this input: 33 probed records, 0 threw.
- Task 09's probe never exercises cancellation, so cancellation handling is invisible to the probe. In this input: 29 probed records.
- Outcome-type detection is name-based and has known false positives, for example ASP.NET `IResult` / `Results.Ok` in task 07. In this input: 23 of 35 task 07 records invoked `IResult.*` or `Results.*` methods; those entries appear in section 5 although they are not functional outcome types.
- Result-line forgery by model code is a documented limit of the probe: solution code can print a fake result line, so a `returned` or `threw` verdict is not tamper-proof.
