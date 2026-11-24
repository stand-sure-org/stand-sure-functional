namespace ModelHarness.Tests;

using System;
using System.IO;
using System.Linq;
using System.Text.Json;

using AwesomeAssertions;

using Microsoft.Extensions.Logging.Abstractions;

using ModelHarness.Analysis;
using ModelHarness.Probe;
using ModelHarness.Report;
using ModelHarness.Run;

public sealed class ReportBuilderTests
{
    private static readonly JsonSerializerOptions Json = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    private static RunRecord Rec(
        string family = "anthropic",
        string task = "01-a",
        bool compiled = true,
        ProbeOutcome? probe = ProbeOutcome.Returned,
        string[]? methods = null,
        string[]? declared = null,
        string[]? idioms = null,
        string? entry = "Result<int>",
        string? extract = "single",
        bool truncated = false,
        string? stop = "end_turn",
        string? transport = null,
        string? notes = null,
        bool withProbe = true)
    {
        var usage = extract != "extractFailed" && transport is null && !truncated
            ? new UsageFacts(declared ?? [], [], methods ?? [], entry, 0, 0, 0, 0, idioms ?? [])
            : null;

        return new RunRecord(
            "2026-11-19T10:00:00Z",
            "r",
            "baseline",
            family,
            family + "-m",
            task,
            1,
            "h",
            "raw",
            extract,
            compiled,
            [],
            transport is null ? usage : null,
            compiled && withProbe && probe is { } p ? new ProbeResult(p, p == ProbeOutcome.Threw ? "System.Exception" : null, null, 5) : null,
            1,
            1,
            1,
            stop,
            truncated,
            transport,
            notes);
    }

    private static RunLog Log(params RunRecord[] records) => new(records, 0);

    private static string Build(RunLog log, RunLog? compare = null) => ReportBuilder.Build(log, compare, "in.jsonl", compare is null ? null : "cmp.jsonl");

    private static string Section(string report, string heading)
    {
        var start = report.IndexOf(heading, StringComparison.Ordinal);
        start.Should().BeGreaterThanOrEqualTo(0, $"heading '{heading}' must exist");
        var next = report.IndexOf("\n## ", start + heading.Length, StringComparison.Ordinal);
        return next < 0 ? report[start..] : report[start..next];
    }

    [Fact]
    public void ComputesCompileRatePerFamily()
    {
        var log = Log(
            Rec("anthropic", "01-a"),
            Rec("anthropic", "01-a"),
            Rec("anthropic", "02-b"),
            Rec("anthropic", "02-b", compiled: false, extract: "extractFailed"),
            Rec("anthropic", "02-b", compiled: false, truncated: true, stop: "max_tokens"),
            Rec("anthropic", "02-b", compiled: false, transport: "HTTP 500", stop: null),
            Rec("gemini", "01-a"),
            Rec("gemini", "01-a", compiled: false, extract: "extractFailed"));

        var section = Section(Build(log), "## 1. Compile rate");

        section.Should().Contain("| anthropic | 4 | 3 | 75.0% |");
        section.Should().Contain("| gemini | 2 | 1 | 50.0% |");
        section.Should().Contain("| overall | 6 | 4 | 66.7% |");
        section.Should().Contain("excluded");
    }

    [Fact]
    public void ProbeRates_UseCompiledProbedRecordsOnly()
    {
        var log = Log(
            Rec(probe: ProbeOutcome.Returned),
            Rec(probe: ProbeOutcome.Threw),
            Rec(probe: ProbeOutcome.Timeout),
            Rec(probe: ProbeOutcome.Crashed),
            Rec(compiled: false, extract: "extractFailed"));

        var section = Section(Build(log), "## 2. Probe outcome rates");

        section.Should().Contain("| anthropic | 4 | 1 (25.0%) | 1 (25.0%) | 1 (25.0%) | 1 (25.0%) |");
    }

    [Fact]
    public void RanksMethodNames()
    {
        var common = new[] { "Result.Map", "Add", "Option.Some", "List.Add" };

        var log = Log(
            Rec(methods: [.. common, "Result.Bind"]),
            Rec(methods: ["Result.Map", "Result.Bind", "Option.Some", "Result.Ok"]),
            Rec(methods: ["Result.Map", "IResult.Foo"]),
            Rec("gemini", methods: ["Outcome.Run"]));

        var section = Section(Build(log), "## 5. Top 30 invoked");
        var overall = section[section.IndexOf("### overall", StringComparison.Ordinal)..];

        var order = new[] { "`Result.Map` | 3", "`Option.Some` | 2", "`Result.Bind` | 2", "`IResult.Foo` | 1", "`Outcome.Run` | 1", "`Result.Ok` | 1" };
        var positions = order.Select(o => overall.IndexOf(o, StringComparison.Ordinal)).ToList();
        positions.Should().OnlyContain(p => p >= 0);
        positions.Should().BeInAscendingOrder();
        overall.Should().NotContain("`Add`");
        overall.Should().NotContain("List.Add");
    }

    [Fact]
    public void RanksMethodNames_CapsAtThirty()
    {
        var methods = Enumerable.Range(0, 35).Select(i => $"Result.M{i:00}").ToArray();

        var section = Section(Build(Log(Rec(methods: methods))), "## 5. Top 30 invoked");

        section.Should().Contain("Result.M29");
        section.Should().NotContain("Result.M30");
    }

    [Fact]
    public void CompareFlagsRegression()
    {
        var current = Log(
            Rec("anthropic", "01-a", compiled: true),
            Rec("anthropic", "01-a", compiled: true),
            Rec("anthropic", "01-a", compiled: false, extract: "extractFailed"),
            Rec("anthropic", "01-a", compiled: false, extract: "extractFailed"),
            Rec("anthropic", "02-b", probe: ProbeOutcome.Threw),
            Rec("anthropic", "02-b", probe: ProbeOutcome.Threw),
            Rec("anthropic", "02-b", probe: ProbeOutcome.Returned),
            Rec("anthropic", "03-c", compiled: false, extract: "extractFailed"),
            Rec("anthropic", "03-c", compiled: false, extract: "extractFailed"));

        var baseline = Log(
            Rec("anthropic", "01-a"),
            Rec("anthropic", "01-a"),
            Rec("anthropic", "01-a"),
            Rec("anthropic", "01-a"),
            Rec("anthropic", "02-b", probe: ProbeOutcome.Returned),
            Rec("anthropic", "02-b", probe: ProbeOutcome.Returned),
            Rec("anthropic", "02-b", probe: ProbeOutcome.Returned),
            Rec("anthropic", "03-c"),
            Rec("anthropic", "03-c"));

        var section = Section(Build(current, baseline), "## 7. Per-task notes");

        section.Should().Contain("anthropic / 01-a");
        section.Should().Contain("compile rate regressed: 100.0% (4/4) -> 50.0% (2/4)");
        section.Should().Contain("anthropic / 02-b");
        section.Should().Contain("threw rate regressed: 0.0% (0/3) -> 66.7% (2/3)");
        section.Should().NotContain("03-c", "n < 3 on both sides is not enough evidence");
        section.Should().Contain("more than 10 percentage points");
    }

    [Fact]
    public void CompareExactlyTenPoints_IsNotARegression()
    {
        // 10/10 -> 9/10 is exactly 10 pp worse: not strictly more than 10.
        var current = Log([.. Enumerable.Range(0, 9).Select(_ => Rec()), Rec(compiled: false, extract: "extractFailed")]);
        var baseline = Log([.. Enumerable.Range(0, 10).Select(_ => Rec())]);

        var section = Section(Build(current, baseline), "## 7. Per-task notes");

        section.Should().Contain("No regressions");
    }

    [Fact]
    public void NoCompare_StatesIt()
    {
        Section(Build(Log(Rec())), "## 7. Per-task notes").Should().Contain("No --compare file");
    }

    [Fact]
    public void DataQuality_CountsRevisedSeparatelyFromConcatenated()
    {
        var log = RunLog.Parse(
        [
            JsonSerializer.Serialize(Rec(extract: "revised"), Json),
            JsonSerializer.Serialize(Rec(extract: "revised"), Json),
            JsonSerializer.Serialize(Rec(extract: "concatenated"), Json),
        ]);

        Section(Build(log), "## Data quality").Should().Contain("| anthropic | 3 | 0 | 0 | 0 | 0 | 1 | 2 | 0 | 0 |\n");
    }

    [Fact]
    public void DataQuality_IsFirst_CountsAndMalformedLines()
    {
        var log = RunLog.Parse(
        [
            JsonSerializer.Serialize(Rec(), Json),
            "{ not json",
            "",
            "[1,2]",
            """{"ts":"x"}""",
            JsonSerializer.Serialize(Rec(truncated: true, compiled: false, stop: "max_tokens", extract: null), Json),
            JsonSerializer.Serialize(Rec(transport: "boom", compiled: false, stop: null, extract: null), Json),
            JsonSerializer.Serialize(Rec(extract: "concatenated", probe: ProbeOutcome.Crashed), Json),
        ]);

        log.MalformedLines.Should().Be(3);
        var report = Build(log);

        report.IndexOf("## Data quality", StringComparison.Ordinal).Should().BeLessThan(report.IndexOf("## 1.", StringComparison.Ordinal));
        var dq = Section(report, "## Data quality");
        dq.Should().Contain("| anthropic | 4 | 1 | 1 (max_tokens: 1) | 0 | 0 | 1 | 0 | 0 | 1 |\n");
        dq.Should().Contain("Malformed lines skipped: 3");
    }

    [Fact]
    public void Ordering_IsFamiliesAlphabeticalThenOverall_AndDeterministic()
    {
        var log = Log(Rec("gemini"), Rec("ollama"), Rec("anthropic"));

        var a = Build(log);
        var b = Build(Log(Rec("anthropic"), Rec("ollama"), Rec("gemini")));

        a.Should().Be(b);
        var s = Section(a, "## 1. Compile rate");
        var order = new[] { "| anthropic |", "| gemini |", "| ollama |", "| overall |" }.Select(x => s.IndexOf(x, StringComparison.Ordinal)).ToList();
        order.Should().BeInAscendingOrder().And.OnlyContain(p => p >= 0);
    }

    [Fact]
    public void FrequencySections_EntryTypesIdiomsDeclaredTypes()
    {
        var log = Log(
            Rec(entry: "Result<int>", idioms: ["library-result", "exception"], declared: ["Foo"]),
            Rec(entry: "Result<int>", idioms: ["exception"], declared: ["Foo", "Bar"]),
            Rec(entry: "(bool, string)", idioms: ["tuple-return"], declared: []));

        var report = Build(log);

        Section(report, "## 3. Entry return type").Should().Contain("| `Result<int>` | 2 | 66.7% |");
        Section(report, "## 4. Idiom frequency").Should().Contain("| `exception` | 2 | 66.7% |");
        Section(report, "## 6. Self-declared").Should().Contain("| `Foo` | 2 | 66.7% |");
    }

    [Fact]
    public void Cells_EscapePipes()
    {
        var log = Log(Rec(entry: "A|B"));

        Build(log).Should().Contain("`A\\|B`");
    }

    [Fact]
    public void Caveats_AreLast_AndNoProposedNames()
    {
        var log = Log(Rec(task: "07-x", methods: ["Results.Ok", "Result.Map"]), Rec(task: "08-y", probe: ProbeOutcome.Threw), Rec(task: "09-z"));

        var report = Build(log);

        var caveats = report.IndexOf("## Caveats", StringComparison.Ordinal);
        caveats.Should().BeGreaterThan(report.IndexOf("## 7.", StringComparison.Ordinal));
        report[caveats..].Should().Contain("Task 08").And.Contain("Task 09").And.Contain("IResult").And.Contain("forge");
        report.Should().NotContain("Proposed core names");
    }

    [Fact]
    public void EmptyInput_DoesNotCrash()
    {
        var report = Build(Log());

        report.Should().Contain("## Data quality").And.Contain("n/a");
    }

    [Fact]
    public void Cli_ParsesReportCommand()
    {
        var ok = ReportOptions.Parse(["report", "--input", "a.jsonl", "--compare", "b.jsonl", "--output", "o.md"]);

        ok.Options.Should().Be(new ReportOptions("a.jsonl", "b.jsonl", "o.md"));
        ReportOptions.Parse(["report", "--input", "a", "--output", "o"]).Options!.ComparePath.Should().BeNull();
    }

    [Theory]
    [InlineData("report --input a")]
    [InlineData("report --output o")]
    [InlineData("report --input a --output o --bogus 1")]
    [InlineData("report --input")]
    [InlineData("run --input a --output o")]
    public void Cli_RejectsBadReportInput(string line)
    {
        var result = ReportOptions.Parse(line.Split(' '));

        result.Options.Should().BeNull();
        result.Error.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void Parse_RoundTripsRunnerLines()
    {
        var line = JsonSerializer.Serialize(Rec(), Json);

        var log = RunLog.Parse([line]);

        log.Records.Should().ContainSingle().Which.Family.Should().Be("anthropic");
    }

    [Fact]
    public void Command_WritesReportFile_AndSurvivesMalformedLines()
    {
        var dir = Path.Combine(Path.GetTempPath(), "report-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);

        try
        {
            var input = Path.Combine(dir, "in.jsonl");
            File.WriteAllLines(input, [JsonSerializer.Serialize(Rec(), Json), "garbage"]);
            var output = Path.Combine(dir, "nested", "out.md");

            var code = ReportCommand.Run(new ReportOptions(input, null, output), NullLogger.Instance);

            code.Should().Be(0);
            File.ReadAllText(output).Should().Contain("Malformed lines skipped: 1");
            ReportCommand.Run(new ReportOptions(Path.Combine(dir, "missing.jsonl"), null, output), NullLogger.Instance).Should().Be(2);
        }
        finally
        {
            Directory.Delete(dir, true);
        }
    }

    [Fact]
    public void RunnerErrors_AreCountedAndExcludedFromCompileAndProbeDenominators()
    {
        var log = Log(Rec(), Rec(compiled: false, probe: null, notes: "runner error: IOException: disk"));

        var report = Build(log);

        Section(report, "## Data quality").Should().Contain("Runner errors").And.Contain("| anthropic | 2 | 0 | 0 | 1 | 0 | 0 | 0 | 0 | 0 |\n");
        Section(report, "## Data quality").Should().Contain("EXCLUDED").And.Contain("runner error");
        Section(report, "## 1. Compile rate").Should().Contain("| anthropic | 1 | 1 | 100.0% |");
    }

    [Fact]
    public void ProbeCrashed_CountsEligibleRecordsOnly()
    {
        var log = Log(Rec(probe: ProbeOutcome.Crashed), Rec(probe: ProbeOutcome.Crashed, truncated: true, stop: "max_tokens"));

        Section(Build(log), "## Data quality").Should().Contain("| anthropic | 2 | 0 | 1 (max_tokens: 1) | 0 | 0 | 0 | 0 | 0 | 1 |\n");
    }

    [Fact]
    public void CompiledButNotProbed_IsCountedInProbeSection()
    {
        var log = Log(Rec(), Rec(withProbe: false));

        var section = Section(Build(log), "## 2. Probe outcome rates");

        section.Should().Contain("compiled, not probed");
        section.Should().Contain("| anthropic | 1 | ");
        section.Should().Contain("| 1 |");
    }

    [Fact]
    public void NullArrayElements_DoNotCrashTheReport()
    {
        var line = JsonSerializer.Serialize(Rec(methods: ["Result.Map"], declared: ["Foo"], idioms: ["exception"]), Json)
            .Replace("\"Result.Map\"", "null", StringComparison.Ordinal)
            .Replace("\"Foo\"", "null", StringComparison.Ordinal)
            .Replace("\"exception\"", "null", StringComparison.Ordinal);

        line.Should().Contain("null");

        var log = RunLog.Parse([line]);
        var act = () => Build(log);

        act.Should().NotThrow();
        log.Records.Single().Usage!.InvokedMethods.Should().BeEmpty();
    }
}
