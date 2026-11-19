namespace ModelHarness.Run;

/// <summary>What <see cref="HarnessRunner.RunAsync"/> did.</summary>
/// <param name="Written">Records written.</param>
/// <param name="Cancelled">True when cancellation stopped the run early.</param>
public sealed record RunSummary(int Written, bool Cancelled);
