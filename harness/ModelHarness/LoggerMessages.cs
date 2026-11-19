namespace ModelHarness;

using System.Diagnostics.CodeAnalysis;

using Microsoft.Extensions.Logging;

[ExcludeFromCodeCoverage]
internal static partial class LoggerMessages
{
    [LoggerMessage(LogLevel.Information,
        EventId = 1,
        EventName = nameof(RequestStarted),
        Message = "Calling {Family} model {ModelId}")]
    public static partial void RequestStarted(this ILogger logger, string family, string modelId);

    [LoggerMessage(LogLevel.Information,
        EventId = 2,
        EventName = nameof(RequestCompleted),
        Message = "{Family} model {ModelId} answered in {ElapsedMs} ms")]
    public static partial void RequestCompleted(this ILogger logger, string family, string modelId, long elapsedMs);

    [LoggerMessage(LogLevel.Warning,
        EventId = 3,
        EventName = nameof(RequestFailed),
        Message = "{Family} model {ModelId} failed: {Error}")]
    public static partial void RequestFailed(this ILogger logger, string family, string modelId, string error);

    [LoggerMessage(LogLevel.Warning,
        EventId = 4,
        EventName = nameof(MissingApiKey),
        Message = "{Family} model {ModelId} skipped: environment variable {Variable} is not set")]
    public static partial void MissingApiKey(this ILogger logger, string family, string modelId, string variable);
}
