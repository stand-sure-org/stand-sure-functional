# Harness report: `20261119T162804Z-baseline.jsonl`

Records: 420

## Data quality

Read this first. Records with a transport error, a truncated model answer or a runner error (a failure of the harness itself after the model answered) are counted here but are EXCLUDED from the compile-rate and probe-rate denominators below. `Compile failed` and `Probe crashed` count eligible (non-excluded) records only; `extractFailed` is one cause of a compile failure.

| Family | Records | Transport errors | Truncated | Runner errors | Extract failed | Concatenated | Compile failed | Probe crashed |
|---|---|---|---|---|---|---|---|---|
| anthropic | 180 | 0 | 2 (max_tokens: 2) | 0 | 0 | 8 | 22 | 0 |
| gemini | 120 | 0 | 0 | 0 | 0 | 0 | 6 | 0 |
| ollama | 120 | 1 | 0 | 0 | 0 | 0 | 41 | 0 |
| overall | 420 | 1 | 2 (max_tokens: 2) | 0 | 0 | 8 | 69 | 0 |

Malformed lines skipped: 0

## 1. Compile rate

Denominator n = records that are not transport errors, not truncated and not runner errors (those are excluded; see Data quality). Compile rate = compiled / n.

| Family | n (eligible) | Compiled | Compile rate |
|---|---|---|---|
| anthropic | 178 | 156 | 87.6% |
| gemini | 120 | 114 | 95.0% |
| ollama | 119 | 78 | 65.5% |
| overall | 417 | 348 | 83.5% |

## 2. Probe outcome rates

Denominator n = eligible records that compiled and have a probe result (transport-error, truncated and runner-error records excluded). `crashed` means no verdict and stays in n. `compiled, not probed` counts eligible records that compiled but have no probe result; they are not in n.

| Family | n (probed) | threw | returned | timeout | crashed | compiled, not probed |
|---|---|---|---|---|---|---|
| anthropic | 156 | 0 (0.0%) | 156 (100.0%) | 0 (0.0%) | 0 (0.0%) | 0 |
| gemini | 114 | 0 (0.0%) | 114 (100.0%) | 0 (0.0%) | 0 (0.0%) | 0 |
| ollama | 78 | 20 (25.6%) | 58 (74.4%) | 0 (0.0%) | 0 (0.0%) | 0 |
| overall | 348 | 20 (5.7%) | 328 (94.3%) | 0 (0.0%) | 0 (0.0%) | 0 |

## 3. Entry return type frequency

Records with extracted usage facts (n); `(none)` = no entry method found.

### anthropic (n = 178)

| Return type | Records | Share |
|---|---|---|
| `CreateUserResult` | 15 | 8.4% |
| `PlaceOrderResult` | 15 | 8.4% |
| `ImportResult` | 14 | 7.9% |
| `Result<Invoice>` | 14 | 7.9% |
| `UserSettings` | 14 | 7.9% |
| `UsageResult` | 13 | 7.3% |
| `ChargeResult` | 12 | 6.7% |
| `Task<QuoteResult>` | 12 | 6.7% |
| `RegistrationResult` | 10 | 5.6% |
| `IResult` | 8 | 4.5% |
| `TimeoutResult` | 8 | 4.5% |
| `Product?` | 7 | 3.9% |
| `GetProductResult` | 5 | 2.8% |
| `TimeoutReadResult` | 4 | 2.2% |
| `SignupResult` | 3 | 1.7% |
| `TimeoutSetting` | 3 | 1.7% |
| `ChargeOutcome` | 2 | 1.1% |
| `ProductLookupResult` | 2 | 1.1% |
| `Results<Ok<OrderSummary>, BadRequest<ProblemDetails>, NotFound<ProblemDetails>>` | 2 | 1.1% |
| `Results<Ok<OrderSummary>, BadRequest<string>, NotFound>` | 2 | 1.1% |
| `Task<ShippingQuoteResult>` | 2 | 1.1% |
| `UsageOutcome` | 2 | 1.1% |
| `CardChargeResult` | 1 | 0.6% |
| `ContactImportResult` | 1 | 0.6% |
| `ProductLookup` | 1 | 0.6% |
| `RegisterResult` | 1 | 0.6% |
| `Results<Ok<OrderSummary>, NotFound, ProblemHttpResult>` | 1 | 0.6% |
| `Results<Ok<OrderSummary>, NotFound, ValidationProblem>` | 1 | 0.6% |
| `Results<Ok<OrderSummary>, NotFound<ProblemDetails>, BadRequest<ProblemDetails>>` | 1 | 0.6% |
| `SettingsLookup` | 1 | 0.6% |
| `Task<ShippingQuoteOutcome>` | 1 | 0.6% |

### gemini (n = 120)

| Return type | Records | Share |
|---|---|---|
| `CreateUserResult` | 10 | 8.3% |
| `ImportResult` | 10 | 8.3% |
| `PlaceOrderResult` | 10 | 8.3% |
| `UserSettings` | 10 | 8.3% |
| `ChargeResult` | 9 | 7.5% |
| `Result<Invoice>` | 9 | 7.5% |
| `TimeoutResult` | 8 | 6.7% |
| `IResult` | 7 | 5.8% |
| `Product?` | 6 | 5.0% |
| `RegistrationResult` | 6 | 5.0% |
| `Task<QuoteResult>` | 6 | 5.0% |
| `UsageResult` | 6 | 5.0% |
| `GetProductResult` | 4 | 3.3% |
| `GetUsageResult` | 4 | 3.3% |
| `RegisterResult` | 4 | 3.3% |
| `Results<Ok<OrderSummary>, BadRequest<string>, NotFound>` | 3 | 2.5% |
| `Task<GetQuoteResult>` | 3 | 2.5% |
| `GetTimeoutResult` | 2 | 1.7% |
| `InvoiceResult` | 1 | 0.8% |
| `PaymentResult` | 1 | 0.8% |
| `Task<ShippingQuoteResult>` | 1 | 0.8% |

### ollama (n = 119)

| Return type | Records | Share |
|---|---|---|
| `Result` | 10 | 8.4% |
| `TimeSpan` | 10 | 8.4% |
| `UserSettings` | 10 | 8.4% |
| `object` | 9 | 7.6% |
| `IResult` | 6 | 5.0% |
| `IEnumerable<UsagePeriod>` | 5 | 4.2% |
| `Invoice` | 5 | 4.2% |
| `PaymentResult` | 5 | 4.2% |
| `Product?` | 5 | 4.2% |
| `Result<Invoice>` | 5 | 4.2% |
| `Result<Product>` | 5 | 4.2% |
| `Task<Result<ShippingQuote>>` | 5 | 4.2% |
| `Task<ShippingQuote>` | 5 | 4.2% |
| `User` | 5 | 4.2% |
| `Result<Order>` | 4 | 3.4% |
| `(IReadOnlyList<Contact> Imported, IReadOnlyList<FailedRow> Failed)` | 2 | 1.7% |
| `(IReadOnlyList<Contact> Imported, IReadOnlyList<ImportFailure> Failures)` | 2 | 1.7% |
| `ImportResult` | 2 | 1.7% |
| `Order` | 2 | 1.7% |
| `Result<UsagePeriod[]>` | 2 | 1.7% |
| `Task<IResult>` | 2 | 1.7% |
| `Task<OrderResult>` | 2 | 1.7% |
| `Task<Results<Ok<OrderSummary>, BadRequest<string>, NotFound>>` | 2 | 1.7% |
| `(IReadOnlyList<Contact> Imported, IReadOnlyList<ImportFailure> Failed)` | 1 | 0.8% |
| `(IReadOnlyList<Contact> Successful, IReadOnlyList<(ContactRow Row, string Error)> Failed)` | 1 | 0.8% |
| `(IReadOnlyList<Contact> Successful, IReadOnlyList<FailedRow> Failed)` | 1 | 0.8% |
| `(bool Success, Account? Account, List<string> Errors)` | 1 | 0.8% |
| `OrderPlacementResult` | 1 | 0.8% |
| `Result<PlaceOrderFailure, Order>` | 1 | 0.8% |
| `Result<UsagePeriod[], (MeterReading Reading, string Error)>` | 1 | 0.8% |
| `Result<UsagePeriod[], Error>` | 1 | 0.8% |
| `Result<UsagePeriod[], UsageError>` | 1 | 0.8% |

### overall (n = 417)

| Return type | Records | Share |
|---|---|---|
| `UserSettings` | 34 | 8.2% |
| `Result<Invoice>` | 28 | 6.7% |
| `ImportResult` | 26 | 6.2% |
| `CreateUserResult` | 25 | 6.0% |
| `PlaceOrderResult` | 25 | 6.0% |
| `ChargeResult` | 21 | 5.0% |
| `IResult` | 21 | 5.0% |
| `UsageResult` | 19 | 4.6% |
| `Product?` | 18 | 4.3% |
| `Task<QuoteResult>` | 18 | 4.3% |
| `RegistrationResult` | 16 | 3.8% |
| `TimeoutResult` | 16 | 3.8% |
| `Result` | 10 | 2.4% |
| `TimeSpan` | 10 | 2.4% |
| `GetProductResult` | 9 | 2.2% |
| `object` | 9 | 2.2% |
| `PaymentResult` | 6 | 1.4% |
| `IEnumerable<UsagePeriod>` | 5 | 1.2% |
| `Invoice` | 5 | 1.2% |
| `RegisterResult` | 5 | 1.2% |
| `Result<Product>` | 5 | 1.2% |
| `Results<Ok<OrderSummary>, BadRequest<string>, NotFound>` | 5 | 1.2% |
| `Task<Result<ShippingQuote>>` | 5 | 1.2% |
| `Task<ShippingQuote>` | 5 | 1.2% |
| `User` | 5 | 1.2% |
| `GetUsageResult` | 4 | 1.0% |
| `Result<Order>` | 4 | 1.0% |
| `TimeoutReadResult` | 4 | 1.0% |
| `SignupResult` | 3 | 0.7% |
| `Task<GetQuoteResult>` | 3 | 0.7% |
| `Task<ShippingQuoteResult>` | 3 | 0.7% |
| `TimeoutSetting` | 3 | 0.7% |
| `(IReadOnlyList<Contact> Imported, IReadOnlyList<FailedRow> Failed)` | 2 | 0.5% |
| `(IReadOnlyList<Contact> Imported, IReadOnlyList<ImportFailure> Failures)` | 2 | 0.5% |
| `ChargeOutcome` | 2 | 0.5% |
| `GetTimeoutResult` | 2 | 0.5% |
| `Order` | 2 | 0.5% |
| `ProductLookupResult` | 2 | 0.5% |
| `Result<UsagePeriod[]>` | 2 | 0.5% |
| `Results<Ok<OrderSummary>, BadRequest<ProblemDetails>, NotFound<ProblemDetails>>` | 2 | 0.5% |
| `Task<IResult>` | 2 | 0.5% |
| `Task<OrderResult>` | 2 | 0.5% |
| `Task<Results<Ok<OrderSummary>, BadRequest<string>, NotFound>>` | 2 | 0.5% |
| `UsageOutcome` | 2 | 0.5% |
| `(IReadOnlyList<Contact> Imported, IReadOnlyList<ImportFailure> Failed)` | 1 | 0.2% |
| `(IReadOnlyList<Contact> Successful, IReadOnlyList<(ContactRow Row, string Error)> Failed)` | 1 | 0.2% |
| `(IReadOnlyList<Contact> Successful, IReadOnlyList<FailedRow> Failed)` | 1 | 0.2% |
| `(bool Success, Account? Account, List<string> Errors)` | 1 | 0.2% |
| `CardChargeResult` | 1 | 0.2% |
| `ContactImportResult` | 1 | 0.2% |
| `InvoiceResult` | 1 | 0.2% |
| `OrderPlacementResult` | 1 | 0.2% |
| `ProductLookup` | 1 | 0.2% |
| `Result<PlaceOrderFailure, Order>` | 1 | 0.2% |
| `Result<UsagePeriod[], (MeterReading Reading, string Error)>` | 1 | 0.2% |
| `Result<UsagePeriod[], Error>` | 1 | 0.2% |
| `Result<UsagePeriod[], UsageError>` | 1 | 0.2% |
| `Results<Ok<OrderSummary>, NotFound, ProblemHttpResult>` | 1 | 0.2% |
| `Results<Ok<OrderSummary>, NotFound, ValidationProblem>` | 1 | 0.2% |
| `Results<Ok<OrderSummary>, NotFound<ProblemDetails>, BadRequest<ProblemDetails>>` | 1 | 0.2% |
| `SettingsLookup` | 1 | 0.2% |
| `Task<ShippingQuoteOutcome>` | 1 | 0.2% |

## 4. Idiom frequency

Records with extracted usage facts (n); a record can carry several idioms, so shares do not sum to 100%.

### anthropic (n = 178)

| Idiom | Records | Share |
|---|---|---|
| `result-like-type` | 149 | 83.7% |
| `exception` | 83 | 46.6% |
| `nullable-return` | 7 | 3.9% |
| `bool-try-out` | 6 | 3.4% |

### gemini (n = 120)

| Idiom | Records | Share |
|---|---|---|
| `result-like-type` | 104 | 86.7% |
| `exception` | 42 | 35.0% |
| `nullable-return` | 6 | 5.0% |
| `bool-try-out` | 5 | 4.2% |

### ollama (n = 119)

| Idiom | Records | Share |
|---|---|---|
| `exception` | 96 | 80.7% |
| `result-like-type` | 55 | 46.2% |
| `nullable-return` | 5 | 4.2% |
| `tuple-return` | 3 | 2.5% |
| `bool-try-out` | 2 | 1.7% |

### overall (n = 417)

| Idiom | Records | Share |
|---|---|---|
| `result-like-type` | 308 | 73.9% |
| `exception` | 221 | 53.0% |
| `nullable-return` | 18 | 4.3% |
| `bool-try-out` | 13 | 3.1% |
| `tuple-return` | 3 | 0.7% |

## 5. Top 30 invoked method names on outcome-like types

Records invoking the method (n = records with usage facts). Only `Type.Method` entries whose type is outcome-like (name contains Result, Option, Outcome or Either, or a library type); ranked by count, ties alphabetical.

### anthropic (n = 178)

| Rank | Method | Records | Share |
|---|---|---|---|
| 1 | `Result.Success` | 8 | 4.5% |
| 2 | `Results.Ok` | 8 | 4.5% |
| 3 | `Result.Fail` | 7 | 3.9% |
| 4 | `Result.Failure` | 7 | 3.9% |
| 5 | `Results.NotFound` | 7 | 3.9% |
| 6 | `TypedResults.NotFound` | 7 | 3.9% |
| 7 | `TypedResults.Ok` | 7 | 3.9% |
| 8 | `QuoteResult.Success` | 6 | 3.4% |
| 9 | `Result.Ok` | 6 | 3.4% |
| 10 | `QuoteResult.Failure` | 5 | 2.8% |
| 11 | `Result.Map` | 5 | 2.8% |
| 12 | `Results.Problem` | 5 | 2.8% |
| 13 | `TypedResults.BadRequest` | 5 | 2.8% |
| 14 | `UsageResult.Failure` | 5 | 2.8% |
| 15 | `UsageResult.Success` | 5 | 2.8% |
| 16 | `Result.WithContext` | 4 | 2.2% |
| 17 | `CreateUserResult.Failure` | 3 | 1.7% |
| 18 | `CreateUserResult.Success` | 3 | 1.7% |
| 19 | `RegistrationResult.Failure` | 3 | 1.7% |
| 20 | `RegistrationResult.Success` | 3 | 1.7% |
| 21 | `Result.MapError` | 3 | 1.7% |
| 22 | `SignupResult.Failure` | 3 | 1.7% |
| 23 | `SignupResult.Success` | 3 | 1.7% |
| 24 | `OrderLookupResult.GetType` | 2 | 1.1% |
| 25 | `Result.Bind` | 2 | 1.1% |
| 26 | `Results.BadRequest` | 2 | 1.1% |
| 27 | `ShippingQuoteResult.Failure` | 2 | 1.1% |
| 28 | `ShippingQuoteResult.Success` | 2 | 1.1% |
| 29 | `TimeoutReadResult.Failure` | 2 | 1.1% |
| 30 | `TimeoutReadResult.Invalid` | 2 | 1.1% |

### gemini (n = 120)

| Rank | Method | Records | Share |
|---|---|---|---|
| 1 | `TypedResults.BadRequest` | 8 | 6.7% |
| 2 | `TypedResults.NotFound` | 8 | 6.7% |
| 3 | `TypedResults.Ok` | 8 | 6.7% |
| 4 | `Result.Fail` | 3 | 2.5% |
| 5 | `Result.Ok` | 3 | 2.5% |
| 6 | `CreateUserResult.Fail` | 2 | 1.7% |
| 7 | `CreateUserResult.Ok` | 2 | 1.7% |
| 8 | `QuoteResult.Failure` | 2 | 1.7% |
| 9 | `QuoteResult.Success` | 2 | 1.7% |
| 10 | `RegistrationResult.GetEnumerator` | 2 | 1.7% |
| 11 | `Result.Success` | 2 | 1.7% |
| 12 | `Results.BadRequest` | 2 | 1.7% |
| 13 | `Results.NotFound` | 2 | 1.7% |
| 14 | `Results.Ok` | 2 | 1.7% |
| 15 | `InvoiceResult.Failure` | 1 | 0.8% |
| 16 | `InvoiceResult.Success` | 1 | 0.8% |
| 17 | `QuoteResult.Fail` | 1 | 0.8% |
| 18 | `QuoteResult.Ok` | 1 | 0.8% |
| 19 | `RegistrationResult.Fail` | 1 | 0.8% |
| 20 | `RegistrationResult.Failure` | 1 | 0.8% |
| 21 | `RegistrationResult.Ok` | 1 | 0.8% |
| 22 | `RegistrationResult.Success` | 1 | 0.8% |
| 23 | `Result.Failure` | 1 | 0.8% |
| 24 | `Results.StatusCode` | 1 | 0.8% |
| 25 | `ShippingQuoteResult.Failure` | 1 | 0.8% |
| 26 | `ShippingQuoteResult.Success` | 1 | 0.8% |

### ollama (n = 119)

| Rank | Method | Records | Share |
|---|---|---|---|
| 1 | `Result.Success` | 25 | 21.0% |
| 2 | `Result.Failure` | 23 | 19.3% |
| 3 | `Result.NotFound` | 10 | 8.4% |
| 4 | `Results.Ok` | 8 | 6.7% |
| 5 | `Results.BadRequest` | 7 | 5.9% |
| 6 | `Result.Ok` | 5 | 4.2% |
| 7 | `Result.Found` | 3 | 2.5% |
| 8 | `Results.Problem` | 3 | 2.5% |
| 9 | `Results.NotFound` | 2 | 1.7% |
| 10 | `Results.StatusCode` | 1 | 0.8% |
| 11 | `TypedResults.BadRequest` | 1 | 0.8% |
| 12 | `TypedResults.NotFound` | 1 | 0.8% |
| 13 | `TypedResults.Ok` | 1 | 0.8% |

### overall (n = 417)

| Rank | Method | Records | Share |
|---|---|---|---|
| 1 | `Result.Success` | 35 | 8.4% |
| 2 | `Result.Failure` | 31 | 7.4% |
| 3 | `Results.Ok` | 18 | 4.3% |
| 4 | `TypedResults.NotFound` | 16 | 3.8% |
| 5 | `TypedResults.Ok` | 16 | 3.8% |
| 6 | `Result.Ok` | 14 | 3.4% |
| 7 | `TypedResults.BadRequest` | 14 | 3.4% |
| 8 | `Results.BadRequest` | 11 | 2.6% |
| 9 | `Results.NotFound` | 11 | 2.6% |
| 10 | `Result.Fail` | 10 | 2.4% |
| 11 | `Result.NotFound` | 10 | 2.4% |
| 12 | `QuoteResult.Success` | 8 | 1.9% |
| 13 | `Results.Problem` | 8 | 1.9% |
| 14 | `QuoteResult.Failure` | 7 | 1.7% |
| 15 | `Result.Map` | 5 | 1.2% |
| 16 | `UsageResult.Failure` | 5 | 1.2% |
| 17 | `UsageResult.Success` | 5 | 1.2% |
| 18 | `RegistrationResult.Failure` | 4 | 1.0% |
| 19 | `RegistrationResult.Success` | 4 | 1.0% |
| 20 | `Result.WithContext` | 4 | 1.0% |
| 21 | `CreateUserResult.Failure` | 3 | 0.7% |
| 22 | `CreateUserResult.Success` | 3 | 0.7% |
| 23 | `Result.Found` | 3 | 0.7% |
| 24 | `Result.MapError` | 3 | 0.7% |
| 25 | `ShippingQuoteResult.Failure` | 3 | 0.7% |
| 26 | `ShippingQuoteResult.Success` | 3 | 0.7% |
| 27 | `SignupResult.Failure` | 3 | 0.7% |
| 28 | `SignupResult.Success` | 3 | 0.7% |
| 29 | `CreateUserResult.Fail` | 2 | 0.5% |
| 30 | `CreateUserResult.Ok` | 2 | 0.5% |

## 6. Self-declared type names

Records with extracted usage facts (n); top 30 per scope, ties alphabetical.

### anthropic (n = 178)

| Type | Records | Share |
|---|---|---|
| `Solution` | 178 | 100.0% |
| `Success` | 29 | 16.3% |
| `Failure` | 19 | 10.7% |
| `Found` | 17 | 9.6% |
| `CreateUserResult` | 15 | 8.4% |
| `NotFound` | 15 | 8.4% |
| `PlaceOrderResult` | 15 | 8.4% |
| `ImportResult` | 14 | 7.9% |
| `Result` | 14 | 7.9% |
| `UsageResult` | 13 | 7.3% |
| `ValidationError` | 13 | 7.3% |
| `ChargeResult` | 12 | 6.7% |
| `QuoteResult` | 12 | 6.7% |
| `CreditLimitExceeded` | 11 | 6.2% |
| `Error` | 11 | 6.2% |
| `GatewayUnreachable` | 11 | 6.2% |
| `InvalidId` | 11 | 6.2% |
| `OrderLookupResult` | 11 | 6.2% |
| `UnknownSku` | 11 | 6.2% |
| `CustomerNotFound` | 10 | 5.6% |
| `RegistrationResult` | 10 | 5.6% |
| `CreateUserError` | 9 | 5.1% |
| `Declined` | 9 | 5.1% |
| `Invalid` | 9 | 5.1% |
| `Placed` | 9 | 5.1% |
| `UsageError` | 9 | 5.1% |
| `Created` | 8 | 4.5% |
| `TimeoutResult` | 8 | 4.5% |
| `UsageErrorKind` | 8 | 4.5% |
| `RowFailure` | 7 | 3.9% |

### gemini (n = 120)

| Type | Records | Share |
|---|---|---|
| `Solution` | 120 | 100.0% |
| `Success` | 76 | 63.3% |
| `Failure` | 27 | 22.5% |
| `NotFound` | 13 | 10.8% |
| `CreateUserResult` | 10 | 8.3% |
| `CreditLimitExceeded` | 10 | 8.3% |
| `Declined` | 10 | 8.3% |
| `ImportResult` | 10 | 8.3% |
| `InvalidId` | 10 | 8.3% |
| `PlaceOrderResult` | 10 | 8.3% |
| `Result` | 10 | 8.3% |
| `ChargeResult` | 9 | 7.5% |
| `Unavailable` | 9 | 7.5% |
| `ValidationError` | 9 | 7.5% |
| `TimeoutResult` | 8 | 6.7% |
| `InvalidDisplayName` | 7 | 5.8% |
| `InvalidEmail` | 7 | 5.8% |
| `Error` | 6 | 5.0% |
| `FindOrderResult` | 6 | 5.0% |
| `ImportFailure` | 6 | 5.0% |
| `InsufficientReadings` | 6 | 5.0% |
| `QuoteResult` | 6 | 5.0% |
| `RegistrationResult` | 6 | 5.0% |
| `UsageResult` | 6 | 5.0% |
| `BadReading` | 5 | 4.2% |
| `CreateUserError` | 5 | 4.2% |
| `CustomerNotFound` | 5 | 4.2% |
| `SkuNotFound` | 5 | 4.2% |
| `UnknownCustomer` | 5 | 4.2% |
| `UnknownSku` | 5 | 4.2% |

### ollama (n = 119)

| Type | Records | Share |
|---|---|---|
| `Solution` | 119 | 100.0% |
| `Result` | 40 | 33.6% |
| `Error` | 8 | 6.7% |
| `FailedRow` | 5 | 4.2% |
| `GatewayUnavailable` | 5 | 4.2% |
| `NotFound` | 5 | 4.2% |
| `PaymentDeclined` | 5 | 4.2% |
| `PaymentResult` | 5 | 4.2% |
| `PricedOrder` | 5 | 4.2% |
| `Timeout` | 5 | 4.2% |
| `Account` | 4 | 3.4% |
| `Errors` | 4 | 3.4% |
| `Failure` | 4 | 3.4% |
| `FailureResult` | 4 | 3.4% |
| `FindOrderResult` | 4 | 3.4% |
| `SuccessResult` | 4 | 3.4% |
| `CreditLimitExceededException` | 3 | 2.5% |
| `Found` | 3 | 2.5% |
| `ImportFailure` | 3 | 2.5% |
| `InvalidRowException` | 3 | 2.5% |
| `NotFoundResult` | 3 | 2.5% |
| `OkResult` | 3 | 2.5% |
| `Success` | 3 | 2.5% |
| `CustomerNotFoundError` | 2 | 1.7% |
| `CustomerNotFoundException` | 2 | 1.7% |
| `ExpectedFailure` | 2 | 1.7% |
| `ImportResult` | 2 | 1.7% |
| `OrderPlacementException` | 2 | 1.7% |
| `OrderResult` | 2 | 1.7% |
| `CreditLimitExceeded` | 1 | 0.8% |

### overall (n = 417)

| Type | Records | Share |
|---|---|---|
| `Solution` | 417 | 100.0% |
| `Success` | 108 | 25.9% |
| `Result` | 64 | 15.3% |
| `Failure` | 50 | 12.0% |
| `NotFound` | 33 | 7.9% |
| `ImportResult` | 26 | 6.2% |
| `CreateUserResult` | 25 | 6.0% |
| `Error` | 25 | 6.0% |
| `PlaceOrderResult` | 25 | 6.0% |
| `CreditLimitExceeded` | 22 | 5.3% |
| `ValidationError` | 22 | 5.3% |
| `ChargeResult` | 21 | 5.0% |
| `Found` | 21 | 5.0% |
| `InvalidId` | 21 | 5.0% |
| `Declined` | 19 | 4.6% |
| `UsageResult` | 19 | 4.6% |
| `QuoteResult` | 18 | 4.3% |
| `CustomerNotFound` | 16 | 3.8% |
| `OrderLookupResult` | 16 | 3.8% |
| `RegistrationResult` | 16 | 3.8% |
| `TimeoutResult` | 16 | 3.8% |
| `UnknownSku` | 16 | 3.8% |
| `ImportFailure` | 15 | 3.6% |
| `CreateUserError` | 14 | 3.4% |
| `GatewayUnreachable` | 11 | 2.6% |
| `FailedRow` | 10 | 2.4% |
| `FindOrderResult` | 10 | 2.4% |
| `PricedOrder` | 10 | 2.4% |
| `UsageError` | 10 | 2.4% |
| `EmailAlreadyRegistered` | 9 | 2.2% |

## 7. Per-task notes: regressions versus --compare

No --compare file was supplied, so no regression check was made.

## Caveats

- Task 08 is a low-signal cell for the throw/no-throw probe: it has no failure path, so `threw` there says little about expected-failure handling. In this input: 35 probed records, 0 threw.
- Task 09's probe never exercises cancellation, so cancellation handling is invisible to the probe. In this input: 30 probed records.
- Outcome-type detection is name-based and has known false positives, for example ASP.NET `IResult` / `Results.Ok` in task 07. In this input: 18 of 35 task 07 records invoked `IResult.*` or `Results.*` methods; those entries appear in section 5 although they are not functional outcome types.
- Result-line forgery by model code is a documented limit of the probe: solution code can print a fake result line, so a `returned` or `threw` verdict is not tamper-proof.

## Proposed core names

For the Task 17 gate. The owner approves or changes each call below; nothing here changes type shapes (DESIGN §5).

### Method

- **Records.** 417 eligible records (420 minus 1 transport error and 2 truncated), whether or not they compiled. Per family: anthropic 178, gemini 120, ollama 119. Compiled-only figures (anthropic 156, gemini 114, ollama 78) are given where they change the picture. The unit is **records containing the name**, not occurrences.
- **Signals.**
  1. Members declared on model-declared outcome-like types (type name contains Result/Outcome/Option/Maybe/Either/Lookup). These were found by parsing `rawOutput` code, and split into kinds: static factory, instance method, `bool` property and nested case record.
  2. Member calls (`.Name(`) in the raw code. ASP.NET `Results.*` / `TypedResults.*` were excluded.
  3. `usage.EntryReturnType` shapes.
  4. `usage.Idioms`, plus pattern greps for switch arms, `Deconstruct`, `??`, index loops and try/catch.
- **Limits.**
  - Baseline models had no library, so these are names models *invent*, which only approximates what they will *guess* against our API.
  - Most combinator evidence comes from a single task (11) and mostly from anthropic models. Many members have n = 0.
  - Ollama compiled 65.5%. Its declarations are counted, but compiled-only figures drop it sharply.
  - One model can dominate a family. For example, devstral alone accounts for 25 of the 65 records with a static `Success(` factory.
  - Regex extraction is approximate. Spot checks of the `Match`, `WithContext` and `Deconstruct` hits matched the code, but treat single-digit counts as anecdotes.

### Member names

Counts are in the order anthropic / gemini / ollama.

| Member | Proposed name | Evidence (records) | Call | Confidence |
|---|---|---|---|---|
| Success factory `Result.Ok(v)` | `Result.Success(v)` | Static factory on an outcome type: `Success` 34/6/25 (65); `Ok` 8/9/5 (22, of which `Result.Ok` 14); no other spellings. Compiled only: `Success` 43 vs `Ok` 10. Per model: haiku 16 vs 5, sonnet 13 vs 3, devstral 25 vs 5, **gemini-flash 6 vs 9**; opus, gemini-pro and qwen declared almost none. Case records are also mostly named `Success` (29/76/3). | rename to `Success` | med |
| Failure factory `Result.Fail<T>(e)` | `Result.Failure<T>(e)` | Static: `Failure` 32/5/23 (60); `Fail` 8/9/0 (17). Compiled only: 39 vs 11. Same per-model skew as above. Case records: `Failure` 16/27/3. | rename to `Failure` | med |
| `IsSuccess` | `IsSuccess` | `bool IsSuccess` 55/35/26 (116, compiled 88); alternatives `Success` 0/5/2, `Succeeded` 3/0/0, `IsOk` 0/2/0. | keep | high |
| `IsFailure` | `IsFailure` | `bool IsFailure` 8/11/15 (34); `IsError` 0/1/0, `Failed` 4/4/0. Most types exposed only `IsSuccess` and negated it. | keep | med |
| `Match` (Result) | `Match` | Declared 11/4/0 (15, all on task 11 bar 3); invoked 2/0/0. No `Fold`/`Switch`/`Handle`. Every declaration put success first (see deviations). | keep | med (name) |
| `Map` | `Map` | 5/0/0 declared and invoked, task 11 only. No `Select`/`Then`/`Transform` on outcome types. | keep | low |
| `Bind` | `Bind` | 4/0/0 declared, task 11 only. No `Then`/`AndThen`/`FlatMap`/`SelectMany`. | keep | low |
| `MapError` | `MapError` | Declared on `Result`: `MapError` 4/0/0. `WithContext` on `Result` 4/0/0 (1 takes `Func<Error,Error>`, 3 take `string context`, which is MapError plus wrap). | keep; see shape note 5 | low |
| `Ensure` (explicit error) | `Ensure` | 0 records for `Ensure`/`Where`/`Filter`/`Validate`/`Require` on outcome types. Validation was done with plain `if`. | keep (no counter-evidence) | low |
| `Ensure` (caller-expression) | `Ensure` | 0 records, as above. | keep (no counter-evidence) | low |
| `Tap` | `Tap` | 0 records for `Tap`/`Do`/`Peek`/`OnSuccess`. | keep (no counter-evidence) | low |
| `GetValueOrDefault(fallback)` | `GetValueOrDefault` | 0 records for `ValueOr`/`OrElse`/`UnwrapOr`/`GetOrElse`. Fallbacks were written with `??` (80/30/64). The name matches BCL `Nullable<T>.GetValueOrDefault`. | keep | low |
| `MapAsync` / `BindAsync` / `MatchAsync` / `EnsureAsync` | as is | 0 async combinators in any record. Task 09 used plain `await` and returned bespoke or `Task<Result<…>>` types. | keep (no counter-evidence) | low |
| `Option.Some` / `Option<T>.None` | as is | 0 Option/Maybe types declared; 0 `Some(`/`None(`. Absence was a nullable return (entry `T?` in 7/6/5) or a bespoke type with `Found`/`NotFound` cases (case records `Found` 15/1/3, `NotFound` 13/13/5). | keep | low |
| `IsSome` / `IsNone` | as is | 0 records. | keep (no counter-evidence) | low |
| Option `Match` / `Map` / `Bind` / `Filter` / `GetValueOrDefault` / `ToResult` | as is | 0 records. One task 02 `Match` used `(onFound, onNotFound)`. | keep (no counter-evidence) | low |
| `Combine` | `Combine` | 0 helpers. Task 06 accumulated every error in a list in 32 of 34 records (12/10/10). That supports Combine's *all-errors* behaviour, not its name. Do not use `Sequence`: net11 already has an unrelated `Enumerable.Sequence`. | keep | low |
| `Traverse` | `Traverse` | 0 records. | keep (no counter-evidence) | low |
| `Partition` | `Partition`, elements `(Successes, Failures)` | 0 helpers. Task 10 list names were `Imported` 15/6/5 and `Failed` 14/5/5 (domain words), against `Failures` 1/7/2 and `Successes` 0/3/0. This is weak, element-name-only evidence. | keep | low |
| `Choose` | `Choose` | 0 records. | keep (no counter-evidence) | low |
| `Try` entry point | `Try.Run` / `Try.RunAsync` (provisional) | 0 of 417 records wrapped try/catch in a reusable helper. Both the targeted check and a cruder grep (any `Func`/`Action` parameter followed by `catch`) found 0. Task 04 was inline try/catch in 35 of 35. No `Result.Try`, `Try.Catch`, `Attempt`, `Result.From` or `Result.Of` anywhere. | undecided; keep `Try.Run` provisionally and let Task 21 decide | low |

### Itertools / functools candidates

Task 12 (n = 35, 15/10/10) made 0 library-style sequence calls: no `Zip`, `Skip(1)`, `Aggregate`, `Pairwise`, `Scan` or `Window`. It used an index loop over `[i ± 1]` in 30 of 35 (15/8/7) and a running-total `+=` in 34 of 35 (15/9/10). That is **shape** evidence for Pairwise and Scan, not name evidence. Incidental usage across all 417 records was 0 for every candidate. LINQ checked against the net11.0 reference pack: `Chunk`, `Zip`, `Index`, `CountBy`, `AggregateBy`, `Aggregate`, `GroupBy` and `SelectMany` are present; `Pairwise`, `Scan`, `Window`, `Interleave`, `Cycle`, `Product`, `Combinations` and `Permutations` are not.

| Candidate | Evidence | Recommendation |
|---|---|---|
| `Pairwise` | Task 12 shape, 30/35 index loops; 0 calls | include (low). It is the only candidate the data points at. Alternatively, defer to Task 21. |
| `Scan` (accumulate) | Task 12 shape, 34/35 running totals; 0 calls | include (low), same caveat |
| `Window` | 0; `Chunk` covers the non-overlapping case | exclude |
| `Pipe`, `Compose`, partial application, `Juxt` | 0 | exclude |
| `Memoize` | 0 (no `Lazy<` either) | exclude |
| `Interleave`, `Cycle`, `Product`, `Combinations`, `Permutations` | 0; `Product` is `SelectMany` | exclude |
| Consecutive `GroupBy` | 0 | exclude |
| `Merge` (dictionaries) | 0 | exclude |

### Shape observations (not renames)

These inform the API summary and analyzers, not the type shapes.

1. **Outcome style.** Entry return types per family:
   - **Bespoke result type:** 125/85/10.
   - **Generic `Result<…>` the model declared itself:** 14/9/35.
   - **Plain value, letting exceptions carry failure:** 17/10/51.
   - **ASP.NET (task 07):** 15/10/10.
   - **Nullable:** 7/6/5.
   - **Tuple:** 0/0/8, all ollama, mostly task 10.
   
   `bool TryX(out …)` appeared in 4/2/2 records. Ollama leans on exceptions: the `exception` idiom is in 80.7% of its records, against 46.6% for anthropic and 35.0% for gemini.
2. **Case vocabulary.** Models prefer domain-named union cases (`NotFound`, `Declined`, `InvalidId`, `CreditLimitExceeded`) to a generic Error. Our `ErrorKind` + `Code` must carry that meaning. The API summary should show `Error.NotFound("product.not_found", …)` early.
3. **Consumption.**
   - Pattern matching beats combinators: `switch` expressions 21/16/10 and `is` patterns 24/17/5, against 15 `Match` declarations.
   - Guard-and-return on failure appeared in 13/15/10 records.
   - Exhaustive `switch` over `Result<T>` cases is therefore the main consumption path, so it should feature in the summary.
4. **Deconstruct.** Declared only by gemini (14 records). Every generic-Result form put value before error, `(value, error)` or `(isSuccess, value, error)`. That matches our `Deconstruct(out T? value, out Error? error)`. `var (a, b) = …` was used in 2/4/0 records.
5. **Error chaining.**
   - `WithContext(string)` declared on the error type in 8/2/0 records. That is the same job as our `Error.Wrap(code, message)`; the call stays `Wrap`, with low confidence.
   - The inner link was named `Cause` 11/2/0, `InnerError` 1/9/2, `Inner` 0/4/2. The split is too even to change `Inner`.
   - A `Result.WithContext(string)` shorthand (3 anthropic records) is a possible later convenience. It is not proposed for v1.
6. **Cancellation.** No task 09 record caught `OperationCanceledException` or broad `Exception`. This is consistent with our `Try` rule; nothing to change.

### Deliberate deviations (keep our choice despite the data)

1. **Failure-first `Match(onFailure, onSuccess)` / `Match(onNone, onSome)`.** The data is unanimous the other way:
   - every model-declared `Match` put success first (15 of 15: `onSuccess`/`onOk`/`onFound`/`onPlaced` first);
   - case and factory declaration order was success first in 214 of 214 records;
   - the first `switch` arm was success first in 38 of 42.
   
   We keep failure-first because the owner fixed it (CLAUDE.md, DESIGN §5). The consequence is that models will habitually write `Match(onSuccess, onFailure)`. When the lambda types differ this is a compile error; when both branches return the same type and ignore their input, it is a silent bug. Recommended mitigations:
   - use named arguments (`onFailure:` / `onSuccess:`) in every example in `docs/llm-api-summary.md`;
   - add a future analyzer that flags positional `Match` calls;
   - in Task 21, measure how often models get the order wrong.
2. **`GetValueOrDefault(fallback)`, kept over `ValueOr` / `UnwrapOr`.** There is no data either way. The name matches the BCL `Nullable<T>` that models already use, so it is kept.
3. **`Combine`, kept over `Sequence`.** There is no data. `Sequence` is the usual FP name, but net11 ships an unrelated `Enumerable.Sequence`, so `Combine` avoids the clash.
4. **`Try.Run`.** This is not a deviation: there is no data to deviate from. It is listed here so the owner knows the name is not evidence-backed until Task 21.
