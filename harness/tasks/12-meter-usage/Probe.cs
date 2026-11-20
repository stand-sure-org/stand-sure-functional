using System;
using System.Collections.Generic;

public sealed class ProbeMeterReadings : IMeterReadings
{
    public IReadOnlyList<MeterReading> GetReadings(string meterId) =>
        new List<MeterReading>
        {
            new(new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero), 1000.0m),
            new(new DateTimeOffset(2026, 2, 1, 0, 0, 0, TimeSpan.Zero), 1250.5m),
            new(new DateTimeOffset(2026, 3, 1, 0, 0, 0, TimeSpan.Zero), 1200.0m),
        };
}

/// <summary>Injected failure: invalid input (the third reading's cumulative total goes down).</summary>
public static class HarnessProbe
{
    public static object?[] ConstructorArguments() => [new ProbeMeterReadings()];

    public static object?[] Arguments() => ["MTR-0042"];
}
