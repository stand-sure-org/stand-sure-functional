namespace ModelHarness.Tests;

using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
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

    internal static readonly HarnessTask Synthetic = new("00-synthetic", "prompt", Fixture, Probe, "Solution", "Run");

    internal static readonly TimeSpan Timeout = TimeSpan.FromSeconds(10);

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

    public static TheoryData<string> PlantableOutsideRoots() =>
        [
            DarwinUserCacheDirectory(),
            "/Users/Shared",
        ];

    [Theory]
    [MemberData(nameof(PlantableOutsideRoots))]
    public async Task FileReadOutsideAllowlist_IsDenied(string root)
    {
        // A fixture planted outside every allowlisted root: the probe must not be able to read it back.
        RequireSandbox();
        Assert.SkipUnless(Directory.Exists(root), root + " does not exist on this machine.");
        var planted = Path.Combine(root, "probe-fixture-" + Guid.NewGuid().ToString("N") + ".txt");
        try
        {
            File.WriteAllText(planted, "planted-not-a-secret");
        }
        catch (UnauthorizedAccessException)
        {
            Assert.Skip(root + " is not writable on this machine.");
        }

        try
        {
            var assembly = Build($$"""
                using System.IO;

                public sealed class Solution(IGreeter greeter)
                {
                    public string? Run(int id) => File.ReadAllText(@"{{planted}}") + greeter.Greet(id);
                }
                """);

            var result = await ProbeExecutor.RunAsync(assembly, Synthetic, Timeout, TestContext.Current.CancellationToken);

            result.Outcome.Should().Be(ProbeOutcome.Threw);
            result.ExceptionType.Should().Be("System.UnauthorizedAccessException");
        }
        finally
        {
            File.Delete(planted);
        }
    }

    [Fact]
    public async Task FileReadThroughDataVolumeFirmlink_IsDenied()
    {
        // /System is allowlisted; /System/Volumes/Data is the firmlinked data volume and must not become a way around the list.
        RequireSandbox();
        Assert.SkipUnless(Directory.Exists("/System/Volumes/Data/Users/Shared"), "No firmlinked data volume on this machine.");
        var name = "probe-fixture-" + Guid.NewGuid().ToString("N") + ".txt";
        var planted = Path.Combine("/Users/Shared", name);
        try
        {
            File.WriteAllText(planted, "planted-not-a-secret");
        }
        catch (UnauthorizedAccessException)
        {
            Assert.Skip("/Users/Shared is not writable on this machine.");
        }

        try
        {
            var assembly = Build($$"""
                using System.IO;

                public sealed class Solution(IGreeter greeter)
                {
                    public string? Run(int id) => File.ReadAllText(@"/System/Volumes/Data/Users/Shared/{{name}}") + greeter.Greet(id);
                }
                """);

            var result = await ProbeExecutor.RunAsync(assembly, Synthetic, Timeout, TestContext.Current.CancellationToken);

            result.Outcome.Should().Be(ProbeOutcome.Threw);
            result.ExceptionType.Should().Be("System.UnauthorizedAccessException");
        }
        finally
        {
            File.Delete(planted);
        }
    }

    [Fact]
    public async Task RunnerReachedThroughSymlink_StillReturns()
    {
        // The repo may be reached through a symlink (e.g. ~/repos -> /Volumes/Repos). Seatbelt matches real paths only, so
        // every allowlisted root must be resolved before it becomes a -D parameter; the link lives under the denied temp dir.
        RequireSandbox();
        var runner = ProbeExecutor.ProbeRunnerPath();
        runner.Should().NotBeNull();
        var link = Path.Combine(Path.GetTempPath(), "probe-runner-link-" + Guid.NewGuid().ToString("N"));
        Directory.CreateSymbolicLink(link, Path.GetDirectoryName(runner)!);
        try
        {
            var assembly = Build("public sealed class Solution(IGreeter greeter) { public string? Run(int id) => greeter.Greet(id); }");

            var result = await ProbeExecutor.RunAsync(
                assembly,
                Synthetic,
                Timeout,
                null,
                Path.Combine(link, Path.GetFileName(runner)!),
                TestContext.Current.CancellationToken);

            result.Should().BeEquivalentTo(new { Outcome = ProbeOutcome.Returned, ExceptionType = (string?)null, Reason = (string?)null });
        }
        finally
        {
            Directory.Delete(link);
        }
    }

    [Theory]
    [MemberData(nameof(PlantableOutsideRoots))]
    public async Task DirectoryListingOutsideAllowlist_IsDenied(string root)
    {
        RequireSandbox();
        Assert.SkipUnless(Directory.Exists(root), root + " does not exist on this machine.");
        var planted = Path.Combine(root, "probe-fixture-" + Guid.NewGuid().ToString("N"));
        try
        {
            Directory.CreateDirectory(planted);
            File.WriteAllText(Path.Combine(planted, "entry.txt"), "planted");
        }
        catch (UnauthorizedAccessException)
        {
            Assert.Skip(root + " is not writable on this machine.");
        }

        try
        {
            var assembly = Build($$"""
                using System.IO;

                public sealed class Solution(IGreeter greeter)
                {
                    public string? Run(int id) => Directory.GetFileSystemEntries(@"{{planted}}").Length + greeter.Greet(id);
                }
                """);

            var result = await ProbeExecutor.RunAsync(assembly, Synthetic, Timeout, TestContext.Current.CancellationToken);

            result.Outcome.Should().Be(ProbeOutcome.Threw);
            result.ExceptionType.Should().Be("System.UnauthorizedAccessException");
        }
        finally
        {
            Directory.Delete(planted, recursive: true);
        }
    }

    public static TheoryData<string> HostConfigurationDirectories() =>
        [
            "/opt/homebrew/etc",
            "/opt/homebrew/var",
            "/usr/local/etc",
            "/Library/Application Support",
            "/Library/Keychains",
            "/private/etc/ssh",
        ];

    [Theory]
    [MemberData(nameof(HostConfigurationDirectories))]
    public async Task HostConfigurationListing_IsDenied(string directory)
    {
        // Only the entry count would come back, and only on failure of this test; contents are never read.
        RequireSandbox();
        Assert.SkipUnless(Directory.Exists(directory), directory + " does not exist on this machine.");
        var assembly = Build($$"""
            using System.IO;

            public sealed class Solution(IGreeter greeter)
            {
                public string? Run(int id) => Directory.GetFileSystemEntries(@"{{directory}}").Length + greeter.Greet(id);
            }
            """);

        var result = await ProbeExecutor.RunAsync(assembly, Synthetic, Timeout, TestContext.Current.CancellationToken);

        result.Outcome.Should().Be(ProbeOutcome.Threw);
        result.ExceptionType.Should().Be("System.UnauthorizedAccessException");
    }

    [Fact]
    public async Task LocalhostTcp_IsDenied()
    {
        RequireSandbox();
        using var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var port = ((IPEndPoint)listener.LocalEndpoint).Port;
        var assembly = Build($$"""
            using System.Net;
            using System.Net.Sockets;

            public sealed class Solution(IGreeter greeter)
            {
                public string? Run(int id)
                {
                    using var client = new TcpClient();
                    client.Connect(IPAddress.Loopback, {{port}});
                    return greeter.Greet(id);
                }
            }
            """);

        var result = await ProbeExecutor.RunAsync(assembly, Synthetic, Timeout, TestContext.Current.CancellationToken);

        result.Outcome.Should().Be(ProbeOutcome.Threw);
        result.ExceptionType.Should().Be("System.Net.Sockets.SocketException");
    }

    [Fact]
    public async Task UnixDomainSocketConnect_IsDenied()
    {
        RequireSandbox();
        var directory = Directory.CreateTempSubdirectory("uds-");
        var path = Path.Combine(directory.FullName, "s.sock");
        try
        {
            using var listener = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.Unspecified);
            listener.Bind(new UnixDomainSocketEndPoint(path));
            listener.Listen();
            var assembly = Build($$"""
                using System.Net.Sockets;

                public sealed class Solution(IGreeter greeter)
                {
                    public string? Run(int id)
                    {
                        using var client = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.Unspecified);
                        client.Connect(new UnixDomainSocketEndPoint(@"{{path}}"));
                        return greeter.Greet(id);
                    }
                }
                """);

            var result = await ProbeExecutor.RunAsync(assembly, Synthetic, Timeout, TestContext.Current.CancellationToken);

            result.Outcome.Should().Be(ProbeOutcome.Threw);
            result.ExceptionType.Should().Be("System.Net.Sockets.SocketException");
        }
        finally
        {
            directory.Delete(recursive: true);
        }
    }

    public static TheoryData<TimeSpan> InvalidTimeouts() =>
        [
            TimeSpan.Zero,
            TimeSpan.FromMilliseconds(-5),
            System.Threading.Timeout.InfiniteTimeSpan,
            TimeSpan.MaxValue,
            ProbeExecutor.MaxTimeout + TimeSpan.FromMilliseconds(1),
        ];

    [Theory]
    [MemberData(nameof(InvalidTimeouts))]
    public async Task InvalidTimeout_CrashedWithReason_NeverStarts(TimeSpan timeout)
    {
        var assembly = Build("public sealed class Solution(IGreeter greeter) { public string? Run(int id) => greeter.Greet(id); }");
        var started = false;

        var result = await ProbeExecutor.RunAsync(assembly, Synthetic, timeout, _ => started = true, TestContext.Current.CancellationToken);

        result.Outcome.Should().Be(ProbeOutcome.Crashed);
        result.Reason.Should().StartWith("invalid-timeout");
        started.Should().BeFalse();
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
                        Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(new byte[] { 1, 2, 3 })),
                        System.Text.RegularExpressions.Regex.IsMatch("a1", @"^\p{L}\d$"),
                        System.Text.Encoding.Latin1.GetByteCount("é"),
                        "e\u0301".Normalize(System.Text.NormalizationForm.FormC),
                        string.Compare("ä", "b", new CultureInfo("sv-SE"), CompareOptions.None),
                        DateTime.Parse("19.11.2026", new CultureInfo("de-DE")),
                        new DateTime(2026, 11, 19).ToString("D", new CultureInfo("ja-JP")),
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
    public async Task LocalTimeZone_MatchesTheHost()
    {
        // The read allowlist must keep /etc/localtime and the zoneinfo database, or Local silently becomes UTC.
        RequireSandbox();
        var hostZone = TimeZoneInfo.Local.Id;
        var assembly = Build($$"""
            using System;

            public sealed class Solution(IGreeter greeter)
            {
                public string? Run(int id) =>
                    TimeZoneInfo.Local.Id == "{{hostZone}}" ? greeter.Greet(id) : throw new InvalidOperationException(TimeZoneInfo.Local.Id);
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

    internal static void RequireSandbox() =>
        Assert.SkipUnless(OperatingSystem.IsMacOS(), "The probe runs only under macOS sandbox-exec.");

    internal static byte[] Build(string solution, bool withLibrary = false)
    {
        var compiled = SolutionCompiler.Compile(Synthetic, solution, withLibrary);
        compiled.Diagnostics.Should().BeEmpty();
        return compiled.Assembly!;
    }

    /// <summary>The per-user cache directory (<c>confstr(_CS_DARWIN_USER_CACHE_DIR)</c>), a sibling of the user temp dir.</summary>
    private static string DarwinUserCacheDirectory()
    {
        if (!OperatingSystem.IsMacOS())
        {
            return "/nonexistent-cache";
        }

        using var getconf = Process.Start(new ProcessStartInfo("/usr/bin/getconf", "DARWIN_USER_CACHE_DIR") { RedirectStandardOutput = true })!;
        var path = getconf.StandardOutput.ReadToEnd().Trim();
        getconf.WaitForExit();
        return path.TrimEnd('/');
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
