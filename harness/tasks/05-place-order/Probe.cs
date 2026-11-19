using System;

public sealed class ProbeCustomerDirectory : ICustomerDirectory
{
    public Customer? Find(string customerNumber) =>
        new(Guid.Parse("6f1c2a4e-0b7d-4c55-9a43-1d2e3f405162"), "Grace Hopper", 1000m);
}

public sealed class ProbeCatalog : ICatalog
{
    public decimal? GetUnitPrice(string sku) => null;
}

public sealed class ProbeOrderBook : IOrderBook
{
    public Order Place(Guid customerId, string sku, int quantity, decimal total) =>
        new(Guid.NewGuid(), customerId, sku, quantity, total);
}

/// <summary>Injected failure: not found in the second step (the SKU is unknown).</summary>
public static class HarnessProbe
{
    public static object?[] ConstructorArguments() =>
        [new ProbeCustomerDirectory(), new ProbeCatalog(), new ProbeOrderBook()];

    public static object?[] Arguments() => ["C-1001", "SKU-UNKNOWN", 2];
}
