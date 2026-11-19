namespace ModelHarness.Compile;

/// <summary>The outcome of <see cref="CodeExtractor.Extract"/>.</summary>
/// <param name="Code">The extracted C#, or <see langword="null"/> when <paramref name="Status"/> is <see cref="ExtractStatus.ExtractFailed"/>.</param>
/// <param name="Status">How the code was obtained.</param>
public sealed record ExtractResult(string? Code, ExtractStatus Status);
