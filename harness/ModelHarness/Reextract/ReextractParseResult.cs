namespace ModelHarness.Reextract;

/// <summary>Either parsed re-extract options or a usage error.</summary>
public sealed record ReextractParseResult(ReextractOptions? Options, string? Error);
