namespace ModelHarness.Report;

/// <summary>Either parsed report options or a usage error.</summary>
public sealed record ReportParseResult(ReportOptions? Options, string? Error);
