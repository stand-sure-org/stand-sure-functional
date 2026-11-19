---
entry-method: ChargeCard
---
# Charge a card through a third-party gateway

A checkout charges the customer's card through `IPaymentGateway`, a vendor SDK you cannot change. The SDK reports problems by throwing exceptions. The rest of the checkout must not deal with those exceptions; it needs a value back instead.

## What to write

A public class named `Solution` with:

- a public constructor `Solution(IPaymentGateway gateway)`;
- a public instance method `ChargeCard(string cardToken, decimal amount, string currency)`. Choose its return type yourself.

## Behaviour

- Call `IPaymentGateway.Charge` with the arguments as given. If the charge goes through, give the caller a `PaymentReceipt` with the gateway's transaction id, the amount and the currency.
- The gateway's documented exceptions are expected failures, and the caller treats them differently:
  - `PaymentDeclinedException`: the card was declined. The checkout shows the decline reason to the customer.
  - `GatewayUnavailableException` or `TimeoutException`: the gateway could not be reached. The checkout offers to try again later.
- Any other exception is a bug, not an expected failure.

## Provided types

The types below already exist in the global namespace of the project your code is compiled into. Use them as they are: do not redeclare, copy or modify them.

{{FIXTURE}}

## Your answer

- Reply with one ```csharp code block containing everything you write: the `using` directives, `Solution`, and any supporting types you add.
- Implicit usings are off, so include every `using` directive you need. Nullable reference types are enabled.
- Do not write a `Main` method, top-level statements or tests.
