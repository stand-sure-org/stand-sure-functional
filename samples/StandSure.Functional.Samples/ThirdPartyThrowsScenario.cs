namespace StandSure.Functional.Samples;

using System.Diagnostics;

/// <summary>Scenario 1: untraced vendor code throws; <see cref="Try"/> turns it into a failure.</summary>
internal static class ThirdPartyThrowsScenario
{
    public static async Task<Result<decimal>> RunAsync(VendorRateClient vendor, CancellationToken cancellationToken)
    {
        using var activity = SampleTelemetry.Source.StartActivity();

        var sync = Try.Run(() => vendor.GetRate("EUR"));
        var async = await Try.RunAsync(ct => vendor.GetRateAsync("GBP", ct), cancellationToken).ConfigureAwait(false);

        var result = sync.Bind(_ => async);
        SampleTelemetry.RecordOutcome(activity, result);
        return result;
    }
}
