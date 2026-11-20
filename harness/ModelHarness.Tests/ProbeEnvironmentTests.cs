namespace ModelHarness.Tests;

using System;

using AwesomeAssertions;

using ModelHarness.Probe;

/// <summary>
/// Sets a process-wide environment variable to prove the probe child does not inherit it. Runs in its own
/// non-parallel collection, so no other test can observe (or race on) the temporary value.
/// </summary>
[Collection(ProcessEnvironmentCollection.Name)]
public sealed class ProbeEnvironmentTests
{
    [Fact]
    public async Task ProbeEnvironment_ContainsNoApiKeys()
    {
        ProbeExecutorTests.RequireSandbox();

        var assembly = ProbeExecutorTests.Build("""
                                                using System;
                                                using System.Collections;
                                                using System.Linq;

                                                public sealed class Solution(IGreeter greeter)
                                                {
                                                    private static readonly string[] Allowed = ["PATH", "HOME", "TMPDIR", "DOTNET_ROOT", "DOTNET_EnableDiagnostics", "DOTNET_GCHeapHardLimit"];

                                                    public string? Run(int id)
                                                    {
                                                        var key = Environment.GetEnvironmentVariable("ANTHROPIC_API_KEY");
                                                        if (key is not null)
                                                        {
                                                            throw new UnauthorizedAccessException("ANTHROPIC_API_KEY leaked");
                                                        }

                                                        var extra = Environment.GetEnvironmentVariables().Keys.Cast<string>().Where(k => !Allowed.Contains(k)).ToList();
                                                        if (extra.Count > 0)
                                                        {
                                                            throw new InvalidOperationException(string.Join(",", extra));
                                                        }

                                                        return key ?? greeter.Greet(id);
                                                    }
                                                }
                                                """);

        var previous = Environment.GetEnvironmentVariable("ANTHROPIC_API_KEY");
        Environment.SetEnvironmentVariable("ANTHROPIC_API_KEY", previous ?? "sk-ant-probe-test-not-a-real-key");

        try
        {
            var result = await ProbeExecutor.RunAsync(assembly,
                ProbeExecutorTests.Synthetic,
                ProbeExecutorTests.Timeout,
                TestContext.Current.CancellationToken);

            result.Should().BeEquivalentTo(new { Outcome = ProbeOutcome.Returned, ExceptionType = (string?)null });
        }
        finally
        {
            Environment.SetEnvironmentVariable("ANTHROPIC_API_KEY", previous);
        }
    }
}
