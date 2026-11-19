namespace ModelHarness.Reextract;

/// <summary>What <see cref="Reextractor.Run"/> did.</summary>
/// <param name="Total">Non-blank input lines.</param>
/// <param name="Rewritten">Records whose usage facts were recomputed.</param>
/// <param name="PassedThrough">Lines copied unchanged: malformed JSON, transport errors, empty output, unknown task ids.</param>
/// <param name="Malformed">Of <paramref name="PassedThrough"/>, lines that were not valid record JSON.</param>
/// <param name="CompileDrift">Rewritten records whose fresh compile result differs from the stored <c>compileSuccess</c>
/// (the stored value is kept; a non-zero count means the library or compiler changed since the run).</param>
public sealed record ReextractSummary(int Total, int Rewritten, int PassedThrough, int Malformed, int CompileDrift);
