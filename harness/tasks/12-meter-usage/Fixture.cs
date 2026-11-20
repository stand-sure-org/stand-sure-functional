using System;
using System.Collections.Generic;

public sealed record MeterReading(DateTimeOffset TakenAt, decimal CumulativeKwh);

public sealed record UsagePeriod(DateTimeOffset From, DateTimeOffset To, decimal Kwh, decimal RunningTotalKwh);

public interface IMeterReadings
{
    /// <summary>Returns the meter's readings in the order they were recorded (possibly empty).</summary>
    IReadOnlyList<MeterReading> GetReadings(string meterId);
}
