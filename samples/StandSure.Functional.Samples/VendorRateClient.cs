namespace StandSure.Functional.Samples;

/// <summary>
/// Stands in for third-party code the app did not write: no tracing, and it throws instead of returning failures.
/// </summary>
internal sealed class VendorRateClient
{
    public decimal GetRate(string currency) =>
        currency == "USD" ? 1m : throw new InvalidOperationException($"Vendor has no rate for '{currency}'.");

    public async Task<decimal> GetRateAsync(string currency, CancellationToken cancellationToken)
    {
        await Task.Delay(TimeSpan.FromMilliseconds(20), cancellationToken).ConfigureAwait(false);
        return this.GetRate(currency);
    }
}
