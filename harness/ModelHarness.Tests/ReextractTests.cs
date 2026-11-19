namespace ModelHarness.Tests;

using System;
using System.IO;
using System.Text.Json;

using AwesomeAssertions;

using Microsoft.Extensions.Logging.Abstractions;

using ModelHarness.Analysis;
using ModelHarness.Probe;
using ModelHarness.Reextract;
using ModelHarness.Run;
using ModelHarness.Tasks;

public sealed class ReextractTests : IDisposable
{
    private const string TupleWithUsing = """
        ```csharp
        using StandSure.Functional;
        public sealed class Solution(IUserStore users)
        {
            public (User? User, string? Error) CreateUser(string email, string name) => (users.Add(email, name), null);
        }
        ```
        """;

    private const string LibraryResult = """
        ```csharp
        using StandSure.Functional;
        public sealed class Solution(IUserStore users)
        {
            public Result<User> CreateUser(string email, string name) => users.Add(email, name);
        }
        ```
        """;

    private static readonly string TasksRoot = Path.Combine(RepoLocator.FindRoot(AppContext.BaseDirectory), "harness", "tasks");

    private static readonly UsageFacts Stale = new([], [], [], null, 0, 0, 0, 0, ["tuple-return", "library-result"]);

    private readonly DirectoryInfo _dir = Directory.CreateTempSubdirectory("reextract-");

    public void Dispose() => _dir.Delete(recursive: true);

    [Fact]
    public void Reextract_RewritesOnlyUsage_AndPassesOtherLinesThroughInOrder()
    {
        var probe = new ProbeResult(ProbeOutcome.Returned, null, null, 12);
        string[] lines =
        [
            Line(Record("01-create-user", TupleWithUsing) with { Diagnostics = ["kept as recorded"], Probe = probe }),
            Line(Record("01-create-user", LibraryResult) with { Usage = null }),
            "{not json",
            Line(Record("01-create-user", string.Empty) with { TransportError = "timeout", Usage = null }),
            Line(Record("99-no-such-task", TupleWithUsing)),
        ];

        using var output = new StringWriter { NewLine = "\n" };
        var summary = Reextractor.Run(lines, TaskCatalog.Load(TasksRoot), output);

        var written = output.ToString().Split('\n', StringSplitOptions.RemoveEmptyEntries);
        written.Should().HaveCount(5);
        written[2].Should().Be(lines[2]);
        written[3].Should().Be(lines[3]);
        written[4].Should().Be(lines[4]);

        var tuple = Parse(written[0]);
        tuple.Usage!.Idioms.Should().Contain("tuple-return").And.NotContain("library-result");
        tuple.Diagnostics.Should().Equal("kept as recorded");
        tuple.CompileSuccess.Should().BeTrue();
        tuple.Probe.Should().Be(probe);
        tuple.RawOutput.Should().Be(TupleWithUsing);

        Parse(written[1]).Usage!.Idioms.Should().Contain("library-result");

        summary.Should().Be(new ReextractSummary(Total: 5, Rewritten: 2, PassedThrough: 3, Malformed: 1, CompileDrift: 0));
    }

    [Fact]
    public void Command_WritesNewFile_AndRefusesToOverwrite()
    {
        var input = Path.Combine(_dir.FullName, "in.jsonl");
        var output = Path.Combine(_dir.FullName, "out.jsonl");
        File.WriteAllText(input, Line(Record("01-create-user", TupleWithUsing)) + "\n");
        var options = new ReextractOptions(input, output);

        ReextractCommand.Run(options, TasksRoot, NullLogger.Instance).Should().Be(ReextractCommand.ExitOk);
        Parse(File.ReadAllLines(output)[0]).Usage!.Idioms.Should().NotContain("library-result");

        ReextractCommand.Run(options, TasksRoot, NullLogger.Instance).Should().Be(ReextractCommand.ExitUsage);
        ReextractCommand.Run(new ReextractOptions(input, input), TasksRoot, NullLogger.Instance).Should().Be(ReextractCommand.ExitUsage);
    }

    [Theory]
    [InlineData(new[] { "reextract", "--input", "a.jsonl", "--output", "b.jsonl" }, true)]
    [InlineData(new[] { "reextract", "--input", "a.jsonl" }, false)]
    [InlineData(new[] { "reextract", "--input", "a.jsonl", "--output" }, false)]
    [InlineData(new[] { "reextract", "--bogus", "x", "--input", "a.jsonl", "--output", "b.jsonl" }, false)]
    public void Options_Parse(string[] args, bool ok)
    {
        var parsed = ReextractOptions.Parse(args);

        (parsed.Options is not null).Should().Be(ok);
        if (ok)
        {
            parsed.Options.Should().Be(new ReextractOptions("a.jsonl", "b.jsonl"));
        }
        else
        {
            parsed.Error.Should().Contain(ReextractOptions.Usage);
        }
    }

    private static RunRecord Record(string task, string raw) => new(
        Ts: "2026-11-19T00:00:00.000Z",
        RunId: "run",
        Condition: RunPlan.WithLibrary,
        Family: "anthropic",
        Model: "m",
        Task: task,
        Run: 1,
        PromptHash: "hash",
        RawOutput: raw,
        ExtractStatus: "single",
        CompileSuccess: true,
        Diagnostics: [],
        Usage: Stale,
        Probe: null,
        InputTokens: 1,
        OutputTokens: 2,
        LatencyMs: 3,
        StopReason: "end_turn",
        Truncated: false,
        TransportError: null,
        Notes: null);

    private static string Line(RunRecord record) => JsonSerializer.Serialize(record, RunJson.Default.RunRecord);

    private static RunRecord Parse(string line) => JsonSerializer.Deserialize(line, RunJson.Default.RunRecord)!;
}
