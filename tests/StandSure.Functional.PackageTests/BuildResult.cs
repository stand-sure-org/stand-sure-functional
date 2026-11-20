namespace StandSure.Functional.PackageTests;

/// <summary>The exit code and combined stdout/stderr of a consumer build.</summary>
public sealed record BuildResult(int ExitCode, string Output);
