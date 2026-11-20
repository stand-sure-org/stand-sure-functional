---
entry-method: PlaceOrder
---
# Place an order in three dependent steps

Placing an order takes three steps. Each step needs what the previous one produced, and any of them can fail in an expected way.

## What to write

A public class named `Solution` with:

- a public constructor `Solution(ICustomerDirectory customers, ICatalog catalog, IOrderBook orders)`;
- a public instance method `PlaceOrder(string customerNumber, string sku, int quantity)`. Choose its return type yourself.

## Behaviour

1. Find the customer with `ICustomerDirectory.Find`. An unknown customer number is an expected failure.
2. Price the order: get the unit price with `ICatalog.GetUnitPrice` (an unknown SKU is an expected failure); the total is unit price × quantity. A total above the customer's `CreditLimit` is an expected failure.
3. Place the order with `IOrderBook.Place` and give the caller the placed `Order`.

Stop at the first failure: later steps must not run. The caller needs to know which step failed and why.

## Provided types

The types below already exist in the global namespace of the project your code is compiled into. Use them as they are: do not redeclare, copy or modify them.

{{FIXTURE}}

## Your answer

- Reply with one ```csharp code block containing everything you write: the `using` directives, `Solution`, and any supporting types you add.
- Implicit usings are off, so include every `using` directive you need. Nullable reference types are enabled.
- Do not write a `Main` method, top-level statements or tests.
