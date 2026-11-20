public sealed class ProbeProductRepository : IProductRepository
{
    public Product? FindById(int id) => null;
}

/// <summary>Injected failure: not found (the repository has no product with the id).</summary>
public static class HarnessProbe
{
    public static object?[] ConstructorArguments() => [new ProbeProductRepository()];

    public static object?[] Arguments() => [42];
}
