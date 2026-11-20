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

    [LoggerMessage(LogLevel.Information,
        EventId = 5,
        EventName = nameof(RecordWritten),
        Message = "{Family} {ModelId} {TaskId} run {Run} written (compiled: {Compiled})")]
    public static partial void RecordWritten(this ILogger logger, string family, string modelId, string taskId, int run, bool compiled);

    [LoggerMessage(LogLevel.Warning,
        EventId = 6,
        EventName = nameof(RecordFailed),
        Message = "{Family} {ModelId} {TaskId} run {Run} hit an unexpected {ExceptionType}")]
    public static partial void RecordFailed(this ILogger logger, string family, string modelId, string taskId, int run, string exceptionType);

    [LoggerMessage(LogLevel.Information,
        EventId = 7,
        EventName = nameof(RunStarted),
        Message = "Run {RunId} ({Condition}): {Models} models x {Tasks} tasks x {Runs} runs -> {Path}")]
    public static partial void RunStarted(this ILogger logger, string runId, string condition, int models, int tasks, int runs, string path);

    [LoggerMessage(LogLevel.Information,
        EventId = 8,
        EventName = nameof(RunFinished),
        Message = "Run finished: {Written} records written (cancelled: {Cancelled})")]
    public static partial void RunFinished(this ILogger logger, int written, bool cancelled);

    [LoggerMessage(LogLevel.Error,
        EventId = 9,
        EventName = nameof(CliError),
        Message = "{Message}")]
    public static partial void CliError(this ILogger logger, string message);

    [LoggerMessage(LogLevel.Information,
        EventId = 10,
        EventName = nameof(ReportWritten),
        Message = "Report written from {Records} records ({Malformed} malformed lines skipped) -> {Path}")]
    public static partial void ReportWritten(this ILogger logger, int records, int malformed, string path);

    [LoggerMessage(LogLevel.Information,
        EventId = 11,
        EventName = nameof(ReextractWritten),
        Message = "Re-extracted {Total} lines: {Rewritten} rewritten, {PassedThrough} copied unchanged ({Malformed} malformed); {CompileDrift} records now compile differently (stored values kept) -> {Path}")]
    public static partial void ReextractWritten(
        this ILogger logger, int total, int rewritten, int passedThrough, int malformed, int compileDrift, string path);
}
