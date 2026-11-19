using System;
using System.Collections.Generic;

public sealed class ProbeContactStore : IContactStore
{
    public Contact Save(string name, string email) => new(Guid.NewGuid(), name, email);
}

/// <summary>Injected failure: invalid input (row 2 has an email without '@'); rows 1 and 3 are valid.</summary>
public static class HarnessProbe
{
    public static object?[] ConstructorArguments() => [new ProbeContactStore()];

    public static object?[] Arguments() =>
    [
        new List<ContactRow>
        {
            new(1, "Ada Lovelace", "ada@example.com"),
            new(2, "Alan Turing", "alan.example.com"),
            new(3, "Grace Hopper", "grace@example.com"),
        },
    ];
}
