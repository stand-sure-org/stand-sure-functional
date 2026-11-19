public sealed class ProbeOrderQueries : IOrderQueries
{
    public OrderSummary? FindById(int id) => null;
}

/// <summary>Injected failure: not found (no order with the id).</summary>
public static class HarnessProbe
{
    public static object?[] ConstructorArguments() => [new ProbeOrderQueries()];

    public static object?[] Arguments() => [17];
}
