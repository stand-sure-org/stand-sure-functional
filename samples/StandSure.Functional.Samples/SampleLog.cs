namespace StandSure.Functional.Samples;

using Microsoft.Extensions.Logging;

internal static partial class SampleLog
{
    [LoggerMessage(LogLevel.Information, EventId = 1, EventName = nameof(ScenarioFinished),
        Message = "{Scenario}: {Outcome} (trace {TraceId})")]
    public static partial void ScenarioFinished(this ILogger logger, string scenario, string outcome, string traceId);

    [LoggerMessage(LogLevel.Information, EventId = 2, EventName = nameof(Exporting),
        Message = "Exporting traces and metrics over OTLP to {Endpoint}")]
    public static partial void Exporting(this ILogger logger, string endpoint);
}
