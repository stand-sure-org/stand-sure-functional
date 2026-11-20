namespace ModelHarness.Run;

using System;

/// <summary>The real process environment.</summary>
public sealed class ProcessEnvironment : IEnvironmentReader
{
    public string? Get(string name) => Environment.GetEnvironmentVariable(name);
}
