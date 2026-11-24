namespace StandSure.Functional.Samples;

/// <summary>Scenario 3: an async chain whose first step fails, so every later step only passes the failure along.</summary>
internal static class AsyncUpstreamFailureScenario
{
    public static async Task<string> RunAsync(CancellationToken cancellationToken)
    {
        using var activity = SampleTelemetry.Source.StartActivity();

        var result = LoadAsync("o-404", cancellationToken)
            .EnsureAsync(o => o.Amount > 0, o => Error.Validation("order.amount_not_positive", o.Id))
            .MapAsync(o => o.Amount * 1.1m)
            .BindAsync(total => Task.FromResult(Result.Success(decimal.Round(total, 2))));

        var awaited = await result.ConfigureAwait(false);
        SampleTelemetry.RecordOutcome(activity, awaited);

        return awaited.Match(onFailure: e => $"failed: {e.Code}", onSuccess: total => $"total: {total}");
    }

    private static async Task<Result<Order>> LoadAsync(string id, CancellationToken cancellationToken)
    {
        using var activity = SampleTelemetry.Source.StartActivity();

        await Task.Delay(TimeSpan.FromMilliseconds(15), cancellationToken).ConfigureAwait(false);
        return Result.Failure<Order>(Error.NotFound("order.not_found", $"No order '{id}'."));
    }
}
