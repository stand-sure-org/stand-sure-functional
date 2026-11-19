---
entry-method: GetQuoteAsync
---
# Get a shipping quote asynchronously, honouring cancellation

A checkout asks a remote rates service for a shipping quote. The call goes over the network and can be slow, and the checkout may cancel it (for example when the customer navigates away).

## What to write

A public class named `Solution` with:

- a public constructor `Solution(IShippingRatesClient rates)`;
- a public instance method `GetQuoteAsync(string postcode, decimal weightKg, CancellationToken cancellationToken)`. Choose its return type yourself.

## Behaviour

1. Check the input before calling the service: the postcode must not be empty or whitespace (trim it and convert it to upper case), and the weight must be greater than 0 and at most 30 kg. Invalid input is an expected failure.
2. Get the rate with `IShippingRatesClient.GetRateAsync`. No carrier serving the postcode (the client returns null) is an expected failure.
3. The price is `BaseFee + PerKg × weightKg`, rounded to 2 decimal places (midpoint away from zero). Give the caller a `ShippingQuote` with the carrier and the price.

Honour `cancellationToken`.

## Provided types

The types below already exist in the global namespace of the project your code is compiled into. Use them as they are: do not redeclare, copy or modify them.

{{FIXTURE}}

## Your answer

- Reply with one ```csharp code block containing everything you write: the `using` directives, `Solution`, and any supporting types you add.
- Implicit usings are off, so include every `using` directive you need. Nullable reference types are enabled.
- Do not write a `Main` method, top-level statements or tests.
