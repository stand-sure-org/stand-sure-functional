namespace ModelHarness.Tests;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

using AwesomeAssertions;

using Microsoft.Extensions.Logging.Abstractions;

using ModelHarness.Models;
using ModelHarness.Probe;
using ModelHarness.Run;
using ModelHarness.Tasks;

public sealed class HarnessRunnerTests
{
    private const string GoodCode = "```csharp\npublic class Solution { public int Run() => 1; }\n```";

    private static HarnessTask T(string id) => new(id, $"prompt for {id}", string.Empty, string.Empty, "Solution", "Run");

    private static ModelConfig Model(string family, string id) => new(family, id, true);

    private static RunPlan Plan(
        string condition = "baseline",
        int runs = 1,
        IReadOnlyList<ModelConfig>? models = null,
        IReadOnlyList<HarnessTask>? tasks = null) =>
        new(Guid.Parse("11111111-2222-3333-4444-555555555555"), condition, "SYSTEM", runs,
            models ?? [Model("anthropic", "m1")], tasks ?? [T("t1")]);

    private static ModelResponse Ok(string text) => new("m", text, 10, 20, TimeSpan.FromMilliseconds(5), null);

    private static async Task<(RunSummary Summary, List<JsonElement> Lines)> RunAsync(
        RunPlan plan,
        Dictionary<string, IModelClient> clients,
        IProbeRunner? probe = null,
        CancellationToken? ct = null)
    {
        var runner = new HarnessRunner(clients, probe ?? new FakeProbe(), NullLogger.Instance);
        var writer = new StringWriter();
        var summary = await runner.RunAsync(plan, writer, ct ?? TestContext.Current.CancellationToken);
        var lines = writer.ToString()
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(l => JsonDocument.Parse(l).RootElement.Clone())
            .ToList();
        return (summary, lines);
    }

    private static Dictionary<string, IModelClient> Clients(params IModelClient[] clients) =>
        clients.ToDictionary(c => c.Family);

    [Fact]
    public async Task WritesOneRecordPerModelTaskRun()
    {
        var plan = Plan(runs: 2, models: [Model("anthropic", "a"), Model("gemini", "g")], tasks: [T("t1"), T("t2")]);
        var clients = Clients(new FakeClient("anthropic", _ => Ok(GoodCode)), new FakeClient("gemini", _ => Ok(GoodCode)));

        var (summary, lines) = await RunAsync(plan, clients);

        lines.Should().HaveCount(8);
        summary.Written.Should().Be(8);
        summary.Cancelled.Should().BeFalse();
        lines.Select(l => (l.GetProperty("model").GetString(), l.GetProperty("task").GetString(), l.GetProperty("run").GetInt32()))
            .Distinct().Should().HaveCount(8);
    }

    [Fact]
    public async Task Record_HasFlatLabelsAndCamelCaseFields()
    {
        var clients = Clients(new FakeClient("anthropic", _ => Ok(GoodCode) with { StopReason = "end_turn" }));

        var (_, lines) = await RunAsync(Plan(), clients);

        var r = lines.Single();
        r.GetProperty("runId").GetString().Should().Be("11111111-2222-3333-4444-555555555555");
        r.GetProperty("condition").GetString().Should().Be("baseline");
        r.GetProperty("family").GetString().Should().Be("anthropic");
        r.GetProperty("model").GetString().Should().Be("m1");
        r.GetProperty("task").GetString().Should().Be("t1");
        r.GetProperty("run").GetInt32().Should().Be(1);
        r.GetProperty("rawOutput").GetString().Should().Be(GoodCode);
        r.GetProperty("extractStatus").GetString().Should().Be("single");
        r.GetProperty("compileSuccess").GetBoolean().Should().BeTrue();
        r.GetProperty("inputTokens").GetInt32().Should().Be(10);
        r.GetProperty("outputTokens").GetInt32().Should().Be(20);
        r.GetProperty("latencyMs").GetInt64().Should().Be(5);
        r.GetProperty("stopReason").GetString().Should().Be("end_turn");
        r.GetProperty("truncated").GetBoolean().Should().BeFalse();
        r.GetProperty("usage").GetProperty("declaredTypes").EnumerateArray().Select(e => e.GetString()).Should().Contain("Solution");
        r.GetProperty("probe").GetProperty("outcome").GetString().Should().Be("returned");
        DateTimeOffset.Parse(r.GetProperty("ts").GetString()!).Offset.Should().Be(TimeSpan.Zero);
        r.TryGetProperty("transportError", out _).Should().BeFalse();
    }

    [Fact]
    public async Task TransportError_RecordedNotThrown()
    {
        var clients = Clients(new FakeClient("anthropic", _ => new ModelResponse("m", string.Empty, 0, 0, TimeSpan.Zero, "HTTP 500 from anthropic")));
        var probe = new FakeProbe();

        var (_, lines) = await RunAsync(Plan(), clients, probe);

        var r = lines.Single();
        r.GetProperty("transportError").GetString().Should().Be("HTTP 500 from anthropic");
        r.GetProperty("compileSuccess").GetBoolean().Should().BeFalse();
        probe.Calls.Should().Be(0);
    }

    [Fact]
    public async Task ThrowingClient_RecordedNotThrown()
    {
        var clients = Clients(new FakeClient("anthropic", _ => throw new InvalidOperationException("boom")));

        var (summary, lines) = await RunAsync(Plan(runs: 2), clients);

        summary.Written.Should().Be(2);
        lines.Should().OnlyContain(l => l.GetProperty("transportError").GetString()!.Contains("boom"));
    }

    [Fact]
    public async Task ThrowingProbe_KeepsRecordWithNotes()
    {
        var clients = Clients(new FakeClient("anthropic", _ => Ok(GoodCode)));

        var (_, lines) = await RunAsync(Plan(), clients, new FakeProbe { Throw = true });

        var r = lines.Single();
        r.GetProperty("compileSuccess").GetBoolean().Should().BeTrue();
        r.GetProperty("notes").GetString().Should().Contain("probe");
        r.TryGetProperty("transportError", out _).Should().BeFalse();
    }

    [Fact]
    public async Task MissingClientForFamily_RecordedAsTransportError()
    {
        var plan = Plan(models: [Model("ollama", "local")]);

        var (_, lines) = await RunAsync(plan, Clients(new FakeClient("anthropic", _ => Ok(GoodCode))));

        lines.Single().GetProperty("transportError").GetString().Should().Contain("ollama");
    }

    [Fact]
    public async Task ExtractFailed_SkipsCompileAndProbe()
    {
        var clients = Clients(new FakeClient("anthropic", _ => Ok("I cannot help with that.")));
        var probe = new FakeProbe();

        var (_, lines) = await RunAsync(Plan(), clients, probe);

        var r = lines.Single();
        r.GetProperty("extractStatus").GetString().Should().Be("extractFailed");
        r.GetProperty("compileSuccess").GetBoolean().Should().BeFalse();
        probe.Calls.Should().Be(0);
    }

    [Fact]
    public async Task CompileFailure_RecordsDiagnosticsAndSkipsProbe()
    {
        var clients = Clients(new FakeClient("anthropic", _ => Ok("```csharp\npublic class Solution { public int Run() => \"x\"; }\n```")));
        var probe = new FakeProbe();

        var (_, lines) = await RunAsync(Plan(), clients, probe);

        var r = lines.Single();
        r.GetProperty("compileSuccess").GetBoolean().Should().BeFalse();
        r.GetProperty("diagnostics").GetArrayLength().Should().BeGreaterThan(0);
        r.GetProperty("usage").ValueKind.Should().Be(JsonValueKind.Object);
        probe.Calls.Should().Be(0);
    }

    [Fact]
    public async Task EmptyTruncatedResponse_IsExtractFailedNotTransportError()
    {
        var clients = Clients(new FakeClient("anthropic", _ => new ModelResponse("m", string.Empty, 5, 4096, TimeSpan.FromMilliseconds(3), null, "max_tokens", true)));
        var probe = new FakeProbe();

        var (_, lines) = await RunAsync(Plan(), clients, probe);

        var r = lines.Single();
        r.GetProperty("extractStatus").GetString().Should().Be("extractFailed");
        r.GetProperty("truncated").GetBoolean().Should().BeTrue();
        r.GetProperty("stopReason").GetString().Should().Be("max_tokens");
        r.GetProperty("outputTokens").GetInt32().Should().Be(4096);
        r.TryGetProperty("transportError", out _).Should().BeFalse();
        probe.Calls.Should().Be(0);
    }

    [Fact]
    public async Task Truncation_IsRecorded()
    {
        var clients = Clients(new FakeClient("anthropic", _ => Ok(GoodCode) with { StopReason = "max_tokens", Truncated = true }));

        var (_, lines) = await RunAsync(Plan(), clients);

        lines.Single().GetProperty("truncated").GetBoolean().Should().BeTrue();
        lines.Single().GetProperty("stopReason").GetString().Should().Be("max_tokens");
    }

    [Fact]
    public async Task PromptHash_StablePerTaskCondition()
    {
        var clients = Clients(new FakeClient("anthropic", _ => Ok(GoodCode)));
        var plan = Plan(runs: 3, tasks: [T("t1"), T("t2")]);

        var (_, first) = await RunAsync(plan, clients);
        var (_, second) = await RunAsync(plan, clients);
        var (_, other) = await RunAsync(plan with { SystemPrompt = "OTHER SYSTEM" }, clients);

        string Hash(JsonElement l) => l.GetProperty("promptHash").GetString()!;
        first.Where(l => l.GetProperty("task").GetString() == "t1").Select(Hash).Distinct().Should().HaveCount(1);
        first.Select(Hash).Distinct().Should().HaveCount(2);
        first.OrderBy(l => l.GetProperty("task").GetString()).Select(Hash).Distinct()
            .Should().Equal(second.OrderBy(l => l.GetProperty("task").GetString()).Select(Hash).Distinct());
        other.Select(Hash).Intersect(first.Select(Hash)).Should().BeEmpty();
        HarnessRunner.PromptHash("S", "U").Should().MatchRegex("^[0-9a-f]{64}$");
    }

    [Fact]
    public async Task Condition_DecidesWithLibrary()
    {
        const string code = "```csharp\nusing StandSure.Functional;\npublic class Solution { public Unit Run() => default; }\n```";
        var clients = Clients(new FakeClient("anthropic", _ => Ok(code)));

        var (_, baseline) = await RunAsync(Plan("baseline"), clients);
        var (_, withLib) = await RunAsync(Plan("with-library"), clients);

        baseline.Single().GetProperty("compileSuccess").GetBoolean().Should().BeFalse();
        withLib.Single().GetProperty("compileSuccess").GetBoolean().Should().BeTrue();
    }

    [Fact]
    public async Task ModelCalls_CappedAtFourPerFamily_AndFamiliesOverlap()
    {
        // Deterministic: the first calls park on a gate that opens only once 8 are in flight (4 per family, both
        // families at once). If the cap were missing, more than 4 of one family would park; if families did not overlap,
        // the gate would never open and the test would fail at the timeout instead of hanging.
        var inFlight = new ConcurrentDictionary<string, int>();
        var peak = new ConcurrentDictionary<string, int>();
        var open = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var total = 0;
        var peakTotal = 0;

        async Task<ModelResponse> Gated(string family)
        {
            var now = inFlight.AddOrUpdate(family, 1, (_, v) => v + 1);
            peak.AddOrUpdate(family, now, (_, v) => Math.Max(v, now));
            var all = Interlocked.Increment(ref total);
            InterlockedMax(ref peakTotal, all);
            if (all >= 8)
            {
                open.TrySetResult();
            }

            await open.Task.WaitAsync(TimeSpan.FromSeconds(30), TestContext.Current.CancellationToken);
            Interlocked.Decrement(ref total);
            inFlight.AddOrUpdate(family, 0, (_, v) => v - 1);
            return Ok(GoodCode);
        }

        var plan = Plan(runs: 12, models: [Model("anthropic", "a"), Model("gemini", "g")]);
        var clients = Clients(
            new FakeClient("anthropic", _ => throw new InvalidOperationException("sync path unused")) { RespondAsync = _ => Gated("anthropic") },
            new FakeClient("gemini", _ => throw new InvalidOperationException("sync path unused")) { RespondAsync = _ => Gated("gemini") });

        var (summary, lines) = await RunAsync(plan, clients);

        summary.Written.Should().Be(24);
        lines.Should().AllSatisfy(l => l.TryGetProperty("transportError", out _).Should().BeFalse());
        open.Task.IsCompletedSuccessfully.Should().BeTrue();
        peak["anthropic"].Should().Be(4);
        peak["gemini"].Should().Be(4);
        peakTotal.Should().Be(8);
    }

    [Fact]
    public async Task Probes_RunOneAtATime()
    {
        var clients = Clients(new FakeClient("anthropic", _ => Ok(GoodCode)) { Async = true });
        var probe = new FakeProbe { Delay = TimeSpan.FromMilliseconds(30) };

        var (summary, _) = await RunAsync(Plan(runs: 8), clients, probe);

        summary.Written.Should().Be(8);
        probe.Calls.Should().Be(8);
        probe.Peak.Should().Be(1);
    }

    [Fact]
    public async Task Cancellation_StopsRunAndFlagsSummary()
    {
        using var cts = new CancellationTokenSource();
        var started = 0;
        var clients = Clients(new FakeClient("anthropic", _ =>
        {
            if (Interlocked.Increment(ref started) >= 2)
            {
                cts.Cancel();
            }

            return Ok(GoodCode);
        })
        { Async = true });

        var (summary, lines) = await RunAsync(Plan(runs: 50), clients, ct: cts.Token);

        summary.Cancelled.Should().BeTrue();
        lines.Count.Should().BeLessThan(50);
    }

    private static void InterlockedMax(ref int target, int value)
    {
        int current;
        do
        {
            current = Volatile.Read(ref target);
            if (value <= current)
            {
                return;
            }
        }
        while (Interlocked.CompareExchange(ref target, value, current) != current);
    }

    private sealed class FakeClient(string family, Func<ModelRequest, ModelResponse> respond) : IModelClient
    {
        public string Family => family;

        public bool Async { get; init; }

        public Func<ModelRequest, Task<ModelResponse>>? RespondAsync { get; init; }

        public async Task<ModelResponse> CompleteAsync(ModelRequest request, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            if (RespondAsync is not null)
            {
                return await RespondAsync(request);
            }

            if (Async)
            {
                await Task.Yield();
            }

            return respond(request);
        }
    }

    private sealed class FakeProbe : IProbeRunner
    {
        private int active;
        private int calls;
        private int peak;

        public bool Throw { get; init; }

        public TimeSpan Delay { get; init; }

        public int Calls => Volatile.Read(ref calls);

        public int Peak => Volatile.Read(ref peak);

        public async Task<ProbeResult> RunAsync(byte[] assembly, HarnessTask task, CancellationToken ct)
        {
            Interlocked.Increment(ref calls);
            var now = Interlocked.Increment(ref active);
            InterlockedMax(ref peak, now);
            try
            {
                if (Throw)
                {
                    throw new InvalidOperationException("probe exploded");
                }

                if (Delay > TimeSpan.Zero)
                {
                    await Task.Delay(Delay, ct);
                }

                return new ProbeResult(ProbeOutcome.Returned, null, null, 1);
            }
            finally
            {
                Interlocked.Decrement(ref active);
            }
        }
    }
}
