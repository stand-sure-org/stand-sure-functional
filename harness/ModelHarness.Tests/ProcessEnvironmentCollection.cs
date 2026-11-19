namespace ModelHarness.Tests;

/// <summary>Tests that mutate process-wide state (environment variables). xUnit runs a non-parallel collection on its
/// own, after the parallel ones, so nothing else is running while it does.</summary>
[CollectionDefinition(Name, DisableParallelization = true)]
public sealed class ProcessEnvironmentCollection
{
    public const string Name = "ProcessEnvironment";
}
