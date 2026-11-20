---
entry-method: GetUsage
---
# Turn cumulative meter readings into usage per period

An electricity meter reports a cumulative total in kWh each time it is read. The billing screen shows how much was used between each pair of consecutive readings, plus the running total since the first reading.

## What to write

A public class named `Solution` with:

- a public constructor `Solution(IMeterReadings readings)`;
- a public instance method `GetUsage(string meterId)`. Choose its return type yourself.

## Behaviour

Get the meter's readings with `IMeterReadings.GetReadings`. For each consecutive pair of readings, produce one `UsagePeriod`:

- `From` and `To` are the two readings' `TakenAt`;
- `Kwh` is the later `CumulativeKwh` minus the earlier one;
- `RunningTotalKwh` is the sum of `Kwh` over this period and every period before it.

The readings come from field devices and can be bad. These are expected failures:

- fewer than two readings (there is no period to report);
- a reading whose `TakenAt` is not later than the previous reading's;
- a reading whose `CumulativeKwh` is lower than the previous reading's (a meter fault).

For a bad reading, the caller needs to know which reading it was and what is wrong with it.

## Provided types

The types below already exist in the global namespace of the project your code is compiled into. Use them as they are: do not redeclare, copy or modify them.

{{FIXTURE}}

## Your answer

- Reply with one ```csharp code block containing everything you write: the `using` directives, `Solution`, and any supporting types you add.
- Implicit usings are off, so include every `using` directive you need. Nullable reference types are enabled.
- Do not write a `Main` method, top-level statements or tests.
