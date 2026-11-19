namespace ModelHarness.Probe;

/// <summary>The outcome of <see cref="ProbeExecutor.RunAsync"/>.</summary>
/// <param name="Outcome"><c>returned</c>, <c>threw</c>, <c>timeout</c> or <c>crashed</c>. Expected-failure handling is measured by threw-or-not.</param>
/// <param name="ExceptionType">Full name of the exception the solution threw (unwrapped from <c>TargetInvocationException</c>); set only for <see cref="ProbeOutcome.Threw"/>.</param>
/// <param name="Reason">Why there is no verdict, for <see cref="ProbeOutcome.Crashed"/>: a runner failure such as <c>entry-type-not-found</c>,
/// <c>no-sandbox</c> (not macOS), <c>no-result (exit code N)</c> (e.g. the solution called <c>Environment.Exit</c>), or <c>probe-runner-missing</c>.</param>
/// <param name="DurationMs">Wall-clock milliseconds from process start to exit or kill (0 when nothing was started).</param>
public sealed record ProbeResult(ProbeOutcome Outcome, string? ExceptionType, string? Reason, long DurationMs);
