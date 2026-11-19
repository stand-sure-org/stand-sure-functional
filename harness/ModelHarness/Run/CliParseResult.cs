namespace ModelHarness.Run;

/// <summary>Either parsed options or a usage error.</summary>
public sealed record CliParseResult(CliOptions? Options, string? Error);
