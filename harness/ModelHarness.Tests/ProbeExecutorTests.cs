namespace ModelHarness.Tests;

using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

using AwesomeAssertions;

using ModelHarness.Compile;
using ModelHarness.Probe;
using ModelHarness.Tasks;

public sealed class ProbeExecutorTests
{
    private const string Fixture = """
        public interface IGreeter
        {
            string? Greet(int id);
        }
        """;

    private const string Probe = """
        public sealed class ProbeGreeter : IGreeter
        {
            public string? Greet(int id) => null;
        }

        public static class HarnessProbe
        {
            public static object?[] ConstructorArguments() => [new ProbeGreeter()];

            public static object?[] Arguments() => [7];
        }
        """;

    private static readonly HarnessTask Synthetic = new("00-synthetic", "prompt", Fixture, Probe, "Solution", "Run");

    private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(10);

    [Fact]
    public async Task ReturningSolution_Returned()
    {
        RequireSandbox();
        var assembly = Build("""
            namespace Anywhere;

            public sealed class Solution(IGreeter greeter)
            {
                public string Run(int id) => greeter.Greet(id) ?? "missing";
            }
            """);

        var result = await ProbeExecutor.RunAsync(assembly, Synthetic, Timeout, TestContext.Current.CancellationToken);

        result.Should().BeEquivalentTo(new { Outcome = ProbeOutcome.Returned, ExceptionType = (string?)null, Reason = (string?)null });
    }

    [Fact]
    public async Task ThrowingSolution_Threw_WithType()
    {
        RequireSandbox();
        var assembly = Build("""
            using System.Collections.Generic;

            public sealed class Solution(IGreeter greeter)
            {
                public string Run(int id) => greeter.Greet(id) ?? throw new KeyNotFoundException("no greeting");
            }
            """);

        var result = await ProbeExecutor.RunAsync(assembly, Synthetic, Timeout, TestContext.Current.CancellationToken);

        result.Outcome.Should().Be(ProbeOutcome.Threw);
        result.ExceptionType.Should().Be("System.Collections.Generic.KeyNotFoundException");
    }

    [Fact]
    public async Task InfiniteLoop_Timeout_ProcessKilled()
    {
        RequireSandbox();
        var assembly = Build("""
            public sealed class Solution(IGreeter greeter)
            {
                public int Run(int id)
                {
                    var spins = 0;
                    while (greeter is not null)
                    {
                        spins++;
                    }

                    return spins;
                }
            }
            """);
        var timeout = TimeSpan.FromSeconds(2);
        var pid = 0;

        var result = await ProbeExecutor.RunAsync(assembly, Synthetic, timeout, p => pid = p, TestContext.Current.CancellationToken);

        result.Outcome.Should().Be(ProbeOutcome.Timeout);
        result.DurationMs.Should().BeGreaterThanOrEqualTo(1900).And.BeLessThan(2000 + 5000);
        pid.Should().NotBe(0);
        ProcessIsGone(pid).Should().BeTrue("the probe's process tree is killed on timeout");
    }

    [Fact]
    public async Task EnvironmentExit_Crashed()
    {
        RequireSandbox();
        var assembly = Build("""
            using System;

            public sealed class Solution(IGreeter greeter)
            {
                public string? Run(int id)
                {
                    Environment.Exit(0);
                    return greeter.Greet(id);
                }
            }
            """);

        var result = await ProbeExecutor.RunAsync(assembly, Synthetic, Timeout, TestContext.Current.CancellationToken);

        result.Outcome.Should().Be(ProbeOutcome.Crashed);
        result.Reason.Should().StartWith("no-result");
    }

    [Theory]
    [InlineData("async Task", "")]
    [InlineData("async Task<int>", "return 1;")]
    [InlineData("async ValueTask", "")]
    [InlineData("async ValueTask<int>", "return 1;")]
    public async Task AsyncSolution_Awaited(string signature, string tail)
    {
        // The exception is raised only after the first real suspension, so "threw" proves the result was awaited.
        RequireSandbox();
        var assembly = Build($$"""
            using System;
            using System.Threading.Tasks;

            public sealed class Solution(IGreeter greeter)
            {
                public {{signature}} Run(int id)
                {
                    await Task.Delay(50);
                    if (greeter.Greet(id) is null)
                    {
                        throw new TimeoutException("late");
                    }

                    {{tail}}
                }
            }
            """);

        var result = await ProbeExecutor.RunAsync(assembly, Synthetic, Timeout, TestContext.Current.CancellationToken);

        result.Outcome.Should().Be(ProbeOutcome.Threw);
        result.ExceptionType.Should().Be("System.TimeoutException");
    }

    [Fact]
    public async Task ProbeEnvironment_ContainsNoApiKeys()
    {
        RequireSandbox();
        var assembly = Build("""
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
            var result = await ProbeExecutor.RunAsync(assembly, Synthetic, Timeout, TestContext.Current.CancellationToken);

            result.Should().BeEquivalentTo(new { Outcome = ProbeOutcome.Returned, ExceptionType = (string?)null });
        }
        finally
        {
            Environment.SetEnvironmentVariable("ANTHROPIC_API_KEY", previous);
        }
    }

    [Fact]
    public async Task NetworkAccess_IsDenied()
    {
        RequireSandbox();
        var assembly = Build("""
            using System.Net;
            using System.Net.Sockets;

            public sealed class Solution(IGreeter greeter)
            {
                public string? Run(int id)
                {
                    using var client = new TcpClient();
                    client.Connect(IPAddress.Parse("1.1.1.1"), 443);
                    return greeter.Greet(id);
                }
            }
            """);

        var result = await ProbeExecutor.RunAsync(assembly, Synthetic, Timeout, TestContext.Current.CancellationToken);

        result.Outcome.Should().Be(ProbeOutcome.Threw);
        result.ExceptionType.Should().Be("System.Net.Sockets.SocketException");
    }

    [Fact]
    public async Task FileWriteOutsideProbeDirectory_IsDenied()
    {
        RequireSandbox();
        var target = Path.Combine(Path.GetTempPath(), "probe-escape-" + Guid.NewGuid().ToString("N"));
        var assembly = Build($$"""
            using System.IO;

            public sealed class Solution(IGreeter greeter)
            {
                public string? Run(int id)
                {
                    File.WriteAllText("inside.txt", "fine");
                    File.WriteAllText(@"{{target}}", "escaped");
                    return greeter.Greet(id);
                }
            }
            """);

        var result = await ProbeExecutor.RunAsync(assembly, Synthetic, Timeout, TestContext.Current.CancellationToken);

        result.Outcome.Should().Be(ProbeOutcome.Threw);
        result.ExceptionType.Should().Be("System.UnauthorizedAccessException");
        File.Exists(target).Should().BeFalse();
    }

    public static TheoryData<string> PrivateDirectories() =>
        [
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            RepoLocator.FindRoot(AppContext.BaseDirectory),
            Path.GetTempPath(),
        ];

    [Theory]
    [MemberData(nameof(PrivateDirectories))]
    public async Task PrivateDirectoryRead_IsDenied(string home)
    {
        RequireSandbox();
        var assembly = Build($$"""
            using System.IO;

            public sealed class Solution(IGreeter greeter)
            {
                public string? Run(int id) => Directory.GetFileSystemEntries(@"{{home}}").Length + greeter.Greet(id);
            }
            """);

        var result = await ProbeExecutor.RunAsync(assembly, Synthetic, Timeout, TestContext.Current.CancellationToken);

        result.Outcome.Should().Be(ProbeOutcome.Threw);
        result.ExceptionType.Should().Be("System.UnauthorizedAccessException");
    }

    [Fact]
    public async Task StdoutFlood_StillReportsTheResultLine()
    {
        // 8 MB with no newline, plus a forged-looking line, through both Console and the raw stdout stream.
        RequireSandbox();
        var assembly = Build("""
            using System;

            public sealed class Solution(IGreeter greeter)
            {
                public string? Run(int id)
                {
                    Console.WriteLine("{\"outcome\":\"threw\",\"exceptionType\":\"Console.Forgery\"}");
                    using var raw = Console.OpenStandardOutput();
                    var chunk = new byte[64 * 1024];
                    Array.Fill(chunk, (byte)'x');
                    for (var i = 0; i < 128; i++)
                    {
                        raw.Write(chunk);
                    }

                    return greeter.Greet(id);
                }
            }
            """);

        var result = await ProbeExecutor.RunAsync(assembly, Synthetic, Timeout, TestContext.Current.CancellationToken);

        result.Should().BeEquivalentTo(new { Outcome = ProbeOutcome.Returned, ExceptionType = (string?)null });
    }

    [Fact]
    public async Task RunawayAllocation_HitsTheHeapCap()
    {
        RequireSandbox();
        var assembly = Build("""
            using System;
            using System.Collections.Generic;

            public sealed class Solution(IGreeter greeter)
            {
                public string? Run(int id)
                {
                    var hoard = new List<byte[]>();
                    while (greeter is not null)
                    {
                        var block = new byte[32 * 1024 * 1024];
                        Array.Fill(block, (byte)1);
                        hoard.Add(block);
                    }

                    return hoard.Count.ToString();
                }
            }
            """);

        var result = await ProbeExecutor.RunAsync(assembly, Synthetic, Timeout, TestContext.Current.CancellationToken);

        result.Outcome.Should().Be(ProbeOutcome.Threw);
        result.ExceptionType.Should().Be("System.OutOfMemoryException");
    }

    [Fact]
    public async Task ChildProcess_IsDenied()
    {
        RequireSandbox();
        var assembly = Build("""
            using System.Diagnostics;

            public sealed class Solution(IGreeter greeter)
            {
                public string? Run(int id)
                {
                    using var child = Process.Start("/usr/bin/true");
                    child?.WaitForExit();
                    return greeter.Greet(id);
                }
            }
            """);

        var result = await ProbeExecutor.RunAsync(assembly, Synthetic, Timeout, TestContext.Current.CancellationToken);

        result.Outcome.Should().Be(ProbeOutcome.Threw);
        result.ExceptionType.Should().Be("System.ComponentModel.Win32Exception");
    }

    [Fact]
    public async Task MissingSolution_CrashedNotThrew()
    {
        RequireSandbox();
        var assembly = Build("""
            public sealed class NotTheSolution(IGreeter greeter)
            {
                public string? Run(int id) => greeter.Greet(id);
            }
            """);

        var result = await ProbeExecutor.RunAsync(assembly, Synthetic, Timeout, TestContext.Current.CancellationToken);

        result.Should().BeEquivalentTo(new { Outcome = ProbeOutcome.Crashed, Reason = "entry-type-not-found" });
    }

    [Fact]
    public async Task WithLibrarySolution_ResolvesStandSureFunctional()
    {
        RequireSandbox();
        var assembly = Build(
            """
            using StandSure.Functional;

            public sealed class Solution(IGreeter greeter)
            {
                public Result<string> Run(int id) => greeter.Greet(id) is { } text ? Result.Ok(text) : Result.Ok("missing");
            }
            """,
            withLibrary: true);

        var result = await ProbeExecutor.RunAsync(assembly, Synthetic, Timeout, TestContext.Current.CancellationToken);

        result.Should().BeEquivalentTo(new { Outcome = ProbeOutcome.Returned, Reason = (string?)null });
    }

    [Fact]
    public async Task AspNetMinimalApiHandler_Returned()
    {
        RequireSandbox();
        var root = RepoLocator.FindRoot(AppContext.BaseDirectory);
        var task = TaskCatalog.Load(Path.Combine(root, "harness", "tasks")).Single(t => t.Id.StartsWith("07-", StringComparison.Ordinal));
        var compiled = SolutionCompiler.Compile(
            task,
            """
            using Microsoft.AspNetCore.Http;

            public sealed class Solution(IOrderQueries orders)
            {
                public IResult GetOrder(int id) => orders.FindById(id) is { } order ? Results.Ok(order) : Results.NotFound();
            }
            """,
            withLibrary: false);
        compiled.Diagnostics.Should().BeEmpty();

        var result = await ProbeExecutor.RunAsync(compiled.Assembly!, task, Timeout, TestContext.Current.CancellationToken);

        result.Should().BeEquivalentTo(new { Outcome = ProbeOutcome.Returned, Reason = (string?)null });
    }

    [Theory]
    [MemberData(nameof(TaskCatalogTests.TaskIds), MemberType = typeof(TaskCatalogTests))]
    public async Task EveryRealTask_StubSolution_ReturnsUnderTheSandbox(string id)
    {
        // Benign code must not be turned into "threw" by the sandbox: every real HarnessProbe stub, invoked against a
        // Solution written to the task's signature, completes.
        RequireSandbox();
        var root = RepoLocator.FindRoot(AppContext.BaseDirectory);
        var task = TaskCatalog.Load(Path.Combine(root, "harness", "tasks")).Single(t => t.Id == id);
        var ctorParameters = Regex.Match(task.Prompt, @"public constructor `Solution\((?<p>[^)]*)\)`").Groups["p"].Value;
        var methodParameters = Regex.Match(task.Prompt, "`" + task.EntryMethod + @"\((?<p>[^)]*)\)`").Groups["p"].Value;
        var stub = $$"""
            using System;
            using System.Collections.Generic;
            using System.Threading;

            namespace Anywhere;

            public sealed class Solution
            {
                public Solution({{ctorParameters}})
                {
                }

                public object? {{task.EntryMethod}}({{methodParameters}}) => null;
            }
            """;
        var compiled = SolutionCompiler.Compile(task, stub, withLibrary: false);
        compiled.Diagnostics.Should().BeEmpty(id);

        var result = await ProbeExecutor.RunAsync(compiled.Assembly!, task, Timeout, TestContext.Current.CancellationToken);

        result.Should().BeEquivalentTo(new { Outcome = ProbeOutcome.Returned, ExceptionType = (string?)null, Reason = (string?)null }, id);
    }

    [Fact]
    public async Task CommonBclCalls_ReturnUnderTheSandbox()
    {
        RequireSandbox();
        var assembly = Build("""
            using System;
            using System.Diagnostics;
            using System.Globalization;
            using System.IO;
            using System.Net.Http;
            using System.Text.Json;
            using System.Threading.Tasks;

            public sealed class Solution(IGreeter greeter)
            {
                public async Task<string> Run(int id)
                {
                    var stopwatch = Stopwatch.StartNew();
                    using var http = new HttpClient();
                    var parts = new object[]
                    {
                        DateTime.Now,
                        DateTimeOffset.UtcNow,
                        TimeZoneInfo.Local.Id,
                        TimeZoneInfo.FindSystemTimeZoneById("Europe/London").BaseUtcOffset,
                        Guid.NewGuid(),
                        Random.Shared.Next(),
                        CultureInfo.CurrentCulture.Name,
                        123.45m.ToString("C", CultureInfo.GetCultureInfo("de-DE")),
                        "straße".ToUpper(CultureInfo.GetCultureInfo("de-DE")),
                        JsonSerializer.Serialize(new { id }),
                        Path.GetTempFileName(),
                        Environment.ProcessorCount,
                        http.Timeout,
                    };
                    await Task.Delay(10);
                    await Task.Run(() => stopwatch.Stop());
                    return string.Join("|", parts) + greeter.Greet(id);
                }
            }
            """);

        var result = await ProbeExecutor.RunAsync(assembly, Synthetic, Timeout, TestContext.Current.CancellationToken);

        result.Should().BeEquivalentTo(new { Outcome = ProbeOutcome.Returned, ExceptionType = (string?)null });
    }

    [Fact]
    public async Task CallerCancellation_KillsChildAndPropagates()
    {
        RequireSandbox();
        var assembly = Build("""
            using System.Threading;

            public sealed class Solution(IGreeter greeter)
            {
                public string? Run(int id)
                {
                    Thread.Sleep(Timeout.Infinite);
                    return greeter.Greet(id);
                }
            }
            """);
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(TestContext.Current.CancellationToken);
        var pid = 0;

        var run = () => ProbeExecutor.RunAsync(assembly, Synthetic, TimeSpan.FromSeconds(30), p =>
        {
            pid = p;
            cts.CancelAfter(TimeSpan.FromMilliseconds(500));
        }, cts.Token);

        await run.Should().ThrowAsync<OperationCanceledException>();
        ProcessIsGone(pid).Should().BeTrue();
    }

    [Fact]
    public async Task NonMacOs_RefusesWithNoSandbox()
    {
        Assert.SkipWhen(OperatingSystem.IsMacOS(), "Only meaningful where sandbox-exec is unavailable.");
        var assembly = Build("public sealed class Solution(IGreeter greeter) { public string? Run(int id) => greeter.Greet(id); }");

        var result = await ProbeExecutor.RunAsync(assembly, Synthetic, Timeout, TestContext.Current.CancellationToken);

        result.Should().BeEquivalentTo(new { Outcome = ProbeOutcome.Crashed, Reason = "no-sandbox", DurationMs = 0L });
    }

    private static void RequireSandbox() =>
        Assert.SkipUnless(OperatingSystem.IsMacOS(), "The probe runs only under macOS sandbox-exec.");

    private static byte[] Build(string solution, bool withLibrary = false)
    {
        var compiled = SolutionCompiler.Compile(Synthetic, solution, withLibrary);
        compiled.Diagnostics.Should().BeEmpty();
        return compiled.Assembly!;
    }

    private static bool ProcessIsGone(int pid)
    {
        try
        {
            using var process = Process.GetProcessById(pid);
            return process.HasExited;
        }
        catch (ArgumentException)
        {
            return true;
        }
    }
}
