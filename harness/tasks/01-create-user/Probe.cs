using System;

public sealed class ProbeUserStore : IUserStore
{
    public bool EmailExists(string email) => false;

    public User Add(string email, string displayName) => new(Guid.NewGuid(), email, displayName);
}

/// <summary>Injected failure: invalid input (the email has no '@').</summary>
public static class HarnessProbe
{
    public static object?[] ConstructorArguments() => [new ProbeUserStore()];

    public static object?[] Arguments() => ["ada.example.com", "Ada Lovelace"];
}
