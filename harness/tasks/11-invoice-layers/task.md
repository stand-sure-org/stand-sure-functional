---
entry-method: CreateInvoice
---
# Add context to a failure as it passes up three layers

Creating an invoice goes through three layers of code. A failure deep down (an unknown tax region) means nothing on its own by the time it reaches the top, so each layer it passes through adds what that layer was doing.

## What to write

A public class named `Solution` with:

- a public constructor `Solution(IOrderSource orders, ITaxRateTable taxRates)`;
- a public instance method `CreateInvoice(Guid orderId)`, the top layer. Choose its return type yourself.
- two more methods (any accessibility you like) for the lower layers:
  - `PriceOrder(CustomerOrder order)`, the middle layer;
  - `LookUpTaxRate(string region)`, the bottom layer.

## Behaviour

- `LookUpTaxRate` gets the region's rate from `ITaxRateTable.FindRate`. An unknown region is an expected failure.
- `PriceOrder` computes the net amount (sum of `Quantity × UnitPrice` over the lines), looks up the tax rate for the order's region, and computes tax = net × rate, rounded to 2 decimal places (midpoint away from zero).
- `CreateInvoice` loads the order with `IOrderSource.Get` (a missing order is an expected failure), prices it, and gives the caller an `Invoice` with net, tax and gross (net + tax).

When `LookUpTaxRate` fails, the failure that comes out of `CreateInvoice` must still carry the original cause (which region is unknown), plus the context added by `PriceOrder` (which order it was pricing) and by `CreateInvoice` (that creating the invoice failed). The caller logs all of it and shows a short message to the user.

## Provided types

The types below already exist in the global namespace of the project your code is compiled into. Use them as they are: do not redeclare, copy or modify them.

{{FIXTURE}}

## Your answer

- Reply with one ```csharp code block containing everything you write: the `using` directives, `Solution`, and any supporting types you add.
- Implicit usings are off, so include every `using` directive you need. Nullable reference types are enabled.
- Do not write a `Main` method, top-level statements or tests.
