using System;

public sealed class ProbeAccountStore : IAccountStore
{
    public Account Create(string username, string email, string password) => new(Guid.NewGuid(), username, email);
}

/// <summary>Injected failure: invalid input (username too short and password too short).</summary>
public static class HarnessProbe
{
    public static object?[] ConstructorArguments() => [new ProbeAccountStore()];

    public static object?[] Arguments() => [new SignupForm("x", "x@example.com", "short", 30)];
}
