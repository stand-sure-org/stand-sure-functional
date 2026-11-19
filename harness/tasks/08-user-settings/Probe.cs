using System;

public sealed class ProbeSettingsCache : ISettingsCache
{
    public UserSettings? Get(Guid userId) => null;

    public void Set(Guid userId, UserSettings settings)
    {
    }
}

public sealed class ProbeSettingsStore : ISettingsStore
{
    public UserSettings? Load(Guid userId) => null;
}

/// <summary>Injected failure: not found (the user has no settings in the cache or the store).</summary>
public static class HarnessProbe
{
    public static object?[] ConstructorArguments() => [new ProbeSettingsCache(), new ProbeSettingsStore()];

    public static object?[] Arguments() => [Guid.Parse("0b6c7d8e-9f10-4a2b-8c3d-4e5f60718293")];
}
