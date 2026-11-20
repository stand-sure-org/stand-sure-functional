using System;

public sealed class ProbeConfigSource : IConfigSource
{
    public string? GetValue(string key) => "thirty";
}

/// <summary>Injected failure: invalid input (the configured value is not a number).</summary>
public static class HarnessProbe
{
    public static object?[] ConstructorArguments() => [new ProbeConfigSource()];

    public static object?[] Arguments() => ["Http:TimeoutSeconds", TimeSpan.FromSeconds(30)];
}
