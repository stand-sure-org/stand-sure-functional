using Xunit;

namespace StandSure.Functional.Tests;

/// <summary>Serializes tests that attach process-global telemetry listeners.</summary>
[CollectionDefinition(Name, DisableParallelization = true)]
public sealed class TelemetryCollection
{
    public const string Name = "Telemetry";
}
