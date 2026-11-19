---
entry-method: GetOrder
---
# Turn an order lookup into an HTTP response in a minimal API endpoint

An ASP.NET Core minimal API exposes `GET /orders/{id}`. The lookup logic and the HTTP mapping are kept apart: one method decides what happened without knowing about HTTP, and the endpoint handler turns that into an HTTP response.

## What to write

A public class named `Solution` with:

- a public constructor `Solution(IOrderQueries orders)`;
- a public instance method `FindOrder(int id)` holding the lookup logic. It must not use any HTTP or ASP.NET Core types. Choose its return type yourself.
- a public instance method `GetOrder(int id)`, the endpoint handler. It calls `FindOrder` and turns what it returns into the HTTP response. Choose its return type yourself.

`GetOrder` is mapped directly by the application, e.g. `app.MapGet("/orders/{id:int}", solution.GetOrder);`. Do not write the application setup or the route registration. ASP.NET Core (the `Microsoft.AspNetCore.App` framework) is referenced.

## Behaviour

- Order ids are positive. A zero or negative id is an expected failure: respond 400 Bad Request with a body explaining the problem, without querying the store.
- No order with that id (`IOrderQueries.FindById` returns null) is an expected failure: respond 404 Not Found.
- Otherwise respond 200 OK with the `OrderSummary` as the JSON body.

## Provided types

The types below already exist in the global namespace of the project your code is compiled into. Use them as they are: do not redeclare, copy or modify them.

{{FIXTURE}}

## Your answer

- Reply with one ```csharp code block containing everything you write: the `using` directives, `Solution`, and any supporting types you add.
- Implicit usings are off, so include every `using` directive you need. Nullable reference types are enabled.
- Do not write a `Main` method, top-level statements or tests.
