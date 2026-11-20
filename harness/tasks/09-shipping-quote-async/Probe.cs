using System.Threading;
using System.Threading.Tasks;

public sealed class ProbeShippingRatesClient : IShippingRatesClient
{
    public async Task<ShippingRate?> GetRateAsync(string postcode, CancellationToken cancellationToken)
    {
        await Task.Yield();
        return null;
    }
}

/// <summary>Injected failure: not found (no carrier serves the postcode). The token is never cancelled.</summary>
public static class HarnessProbe
{
    public static object?[] ConstructorArguments() => [new ProbeShippingRatesClient()];

    public static object?[] Arguments() => ["ZZ9 9ZZ", 2.5m, CancellationToken.None];
}
