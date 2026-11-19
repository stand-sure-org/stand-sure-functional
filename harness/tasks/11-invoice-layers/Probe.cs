using System;
using System.Collections.Generic;

public sealed class ProbeOrderSource : IOrderSource
{
    public CustomerOrder? Get(Guid orderId) =>
        new(orderId, "XX", new List<OrderLine> { new("SKU-1", 2, 9.99m), new("SKU-2", 1, 25.00m) });
}

public sealed class ProbeTaxRateTable : ITaxRateTable
{
    public decimal? FindRate(string region) => null;
}

/// <summary>Injected failure: not found at the bottom layer (the order's tax region is unknown).</summary>
public static class HarnessProbe
{
    public static object?[] ConstructorArguments() => [new ProbeOrderSource(), new ProbeTaxRateTable()];

    public static object?[] Arguments() => [Guid.Parse("a3b4c5d6-e7f8-4901-a2b3-c4d5e6f70819")];
}
