namespace StandSure.Functional.Samples;

using System.Diagnostics;

/// <summary>The sample app's own spans: one root span per scenario, owned by the caller of the library.</summary>
internal static class SampleTelemetry
{
    public const string ServiceName = "standsure-functional-samples";

    public static readonly ActivitySource Source = new("StandSure.Functional.Samples");

    /// <summary>
    /// The caller's side of the contract: the library's spans never carry a status, so the code that received the
    /// failure decides what it means for its own span.
    /// </summary>
    public static void RecordOutcome<T>(Activity? activity, Result<T> result)
    {
        if (activity is null)
        {
            return;
        }

        if (result.TryGetValue(out Error? error))
        {
            activity.SetStatus(ActivityStatusCode.Error, error.Code);
            return;
        }

        activity.SetStatus(ActivityStatusCode.Ok);
    }
}
