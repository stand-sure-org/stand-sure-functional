---
entry-method: GetProduct
---
# Get a product by id

A product page loads one product by its id through `IProductRepository`. Ids come from links that may be stale, so there being no product with the requested id is a normal, expected outcome that every caller has to handle, not an exceptional one.

## What to write

A public class named `Solution` with:

- a public constructor `Solution(IProductRepository products)`;
- a public instance method `GetProduct(int id)`. Choose its return type yourself.

## Behaviour

- Product ids are positive. A zero or negative id can never identify a product, so treat it as not found without querying the repository.
- Otherwise look the product up with `IProductRepository.FindById` and give the caller the `Product`, or let it know that there is no such product.

## Provided types

The types below already exist in the global namespace of the project your code is compiled into. Use them as they are: do not redeclare, copy or modify them.

{{FIXTURE}}

## Your answer

- Reply with one ```csharp code block containing everything you write: the `using` directives, `Solution`, and any supporting types you add.
- Implicit usings are off, so include every `using` directive you need. Nullable reference types are enabled.
- Do not write a `Main` method, top-level statements or tests.
