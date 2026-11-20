# Spans: what the sample traces show (2026-11-20)

Source: `samples/StandSure.Functional.Samples`, exported over OTLP to the local collector and read back from its
`debug` exporter. `[app]` spans belong to the sample (`StandSure.Functional.Samples`), `[lib]` spans to the library.

## Scenario 1: untraced vendor code throws, wrapped in `Try`

```
[app] RunAsync (31.3 ms) status=Error
  [lib] Run (3.0 ms)        events: functional.failure[exception.unhandled]
  [lib] RunAsync (22.2 ms)  events: functional.failure[exception.unhandled]
  [lib] Bind (0.0 ms)
```

Reads well. The vendor code has no tracing, yet each failing call has its own span, with its duration and the
failure event on it. The caller's span gets `Error` status because the caller set it from the `Result`.

## Scenario 2: sync validation chain inside `Traverse` (3 orders, 2 fail)

```
[app] Run (1.9 ms) status=Error
  [lib] Traverse (1.5 ms)  events: functional.failure.grouped[result.combine_failed n=2]
    [lib] Ensure  [lib] Ensure  [lib] Map                                   (o-1, passes)
    [lib] Ensure  events: functional.failure[order.amount_not_positive]
    [lib] Ensure  [lib] Map                                                 (o-2, passing the failure along)
    [lib] Ensure  [lib] Ensure  events: functional.failure[order.email_missing]
    [lib] Map                                                               (o-3, passing the failure along)
```

The failure events land on the exact `Ensure` that built the error, which is useful. But 9 child spans for 3 orders
is chatty, and 3 of them are `Ensure`/`Map` calls that ran no caller code: the input was already a failure.

## Scenario 3: async chain whose first step fails

```
[app] RunAsync (17.8 ms) status=Error
  [app] LoadAsync (17.0 ms)    events: functional.failure[order.not_found]
  [lib] EnsureAsync (16.4 ms)
    [lib] Ensure (0.0 ms)
  [lib] MapAsync (15.9 ms)
    [lib] Map (0.0 ms)
  [lib] BindAsync (15.3 ms)
  [lib] Match (0.1 ms)
```

Two problems:
- The `*Async` spans are siblings that overlap `LoadAsync`, and each lasts about 16 ms although none of them ran
  caller code. The time is spent awaiting the step before. Starting the span on the first line measures waiting.
- The sync-delegate `*Async` overloads call the sync combinator, so each one adds a nested span (`Ensure` inside
  `EnsureAsync`, `Map` inside `MapAsync`), again with no caller code run.

## Not in any scenario: library spans without a parent become root traces

The `Match` calls in `Program.cs` run outside any span. Each one produced a separate one-span trace (`Match`, 0.1 ms).
Any app that calls `Map`/`Match` outside a traced request would fill its backend with one-span traces.

## Options for the owner

1. **No root spans.** Start a library span only when `Activity.Current` is not null, so the library never creates a
   trace on its own. One helper keeps the first-line pattern:
   `using var activity = FunctionalDiagnostics.StartActivity();`.
2. **No span when no caller code runs.** Skip the span when the input is already a failure (or None), and in the
   `*Async` forms start it after the upstream await. This breaks the "first line" rule, so it would need a guard
   clause first, or an extracted method holding the span.
3. **No double spans.** The sync-delegate `*Async` overloads call an internal, unspanned implementation.

Recommendation: 1 and 3 regardless. For 2, whether the passthrough spans and the measured waiting are worth the
exception to the first-line rule is the owner's call.
