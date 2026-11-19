using System.Threading;
using System.Threading.Tasks;

public sealed record ShippingRate(string Carrier, decimal BaseFee, decimal PerKg);

public sealed record ShippingQuote(string Carrier, decimal Price);

public interface IShippingRatesClient
{
    /// <summary>Calls the remote rates service. Returns null when no carrier serves the postcode.</summary>
    Task<ShippingRate?> GetRateAsync(string postcode, CancellationToken cancellationToken);
}
