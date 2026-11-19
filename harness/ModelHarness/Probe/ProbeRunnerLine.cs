namespace ModelHarness.Probe;

/// <summary>Wire shape of the JSON line ProbeRunner writes to stdout: <c>{"outcome":"...","exceptionType":"...","reason":"..."}</c>.</summary>
internal sealed record ProbeRunnerLine(string? Outcome, string? ExceptionType, string? Reason);
