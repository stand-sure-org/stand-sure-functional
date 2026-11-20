namespace ModelHarness.Run;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using ModelHarness.Analysis;
using ModelHarness.Compile;
using ModelHarness.Models;
using ModelHarness.Probe;
using ModelHarness.Tasks;

/// <summary>
/// Runs every (model, task, run) of a <see cref="RunPlan"/>: prompt, model call, extract, compile, usage facts, probe.
/// </summary>
/// <remarks>
/// Model calls are capped per family (default 4) with one semaphore per family. Probes are serialised by a single
/// semaphore. Compile and usage extraction run on the worker that made the model call. Each finished record is written and
/// flushed immediately, so files are crash-safe and records may land out of order. A failure of one record is recorded,
/// never propagated; only cancellation stops the run. On cancellation, records whose model call, compile or probe is still in
/// flight are dropped (no partial or "cancelled" records); records already completed stay written and flushed.
/// </remarks>
public sealed class HarnessRunner(
    IReadOnlyDictionary<string, IModelClient> clients,
    IProbeRunner probe,
    ILogger logger,
    TimeProvider? time = null,
    int maxConcurrentPerFamily = HarnessRunner.DefaultMaxConcurrentPerFamily)
{
    public const int DefaultMaxConcurrentPerFamily = 4;

    private const string PromptSeparator = "\n---\n";

    private readonly TimeProvider clock = time ?? TimeProvider.System;

    /// <summary>The file name for a run started at <paramref name="startedAt"/>, e.g. <c>20261119T110509Z-baseline.jsonl</c>.</summary>
    public static string OutputFileName(DateTimeOffset startedAt, string condition) =>
        $"{startedAt.UtcDateTime.ToString("yyyyMMdd'T'HHmmss'Z'", CultureInfo.InvariantCulture)}-{condition}.jsonl";

    /// <summary>SHA-256 hex (lower case) of <c>system + "\n---\n" + user</c>.</summary>
    public static string PromptHash(string system, string user) =>
        Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(system + PromptSeparator + user)));

    public async Task<RunSummary> RunAsync(RunPlan plan, TextWriter output, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(plan);
        ArgumentNullException.ThrowIfNull(output);

        var familyGates = new Dictionary<string, SemaphoreSlim>(StringComparer.Ordinal);

        foreach (var model in plan.Models)
        {
            familyGates.TryAdd(model.Family, new SemaphoreSlim(maxConcurrentPerFamily, maxConcurrentPerFamily));
        }

        using var probeGate = new SemaphoreSlim(1, 1);
        using var writeGate = new SemaphoreSlim(1, 1);
        var written = 0;
        var workers = new List<Task>();

        foreach (var model in plan.Models)
        {
            foreach (var task in plan.Tasks)
            {
                for (var run = 1; run <= plan.Runs; run++)
                {
                    var (m, t, r) = (model, task, run);

                    workers.Add(Task.Run(async () =>
                        {
                            var record = await this.ExecuteAsync(plan, m, t, r, familyGates[m.Family], probeGate, ct).ConfigureAwait(false);
                            await WriteAsync(output, writeGate, record).ConfigureAwait(false);
                            Interlocked.Increment(ref written);
                            logger.RecordWritten(record.Family, record.Model, record.Task, record.Run, record.CompileSuccess);
                        },
                        CancellationToken.None));
                }
            }
        }

        var cancelled = false;

        try
        {
            await Task.WhenAll(workers).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            cancelled = true;
            await DrainAsync(workers).ConfigureAwait(false);
        }
        finally
        {
            foreach (var gate in familyGates.Values)
            {
                gate.Dispose();
            }
        }

        await output.FlushAsync(CancellationToken.None).ConfigureAwait(false);
        return new RunSummary(Volatile.Read(ref written), cancelled);
    }

    private static async Task DrainAsync(List<Task> workers)
    {
        foreach (var worker in workers)
        {
            try
            {
                await worker.ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                // Expected while shutting down.
            }
        }
    }

    private static async Task WriteAsync(TextWriter output, SemaphoreSlim gate, RunRecord record)
    {
        var line = JsonSerializer.Serialize(record, RunJson.Default.RunRecord);
        await gate.WaitAsync(CancellationToken.None).ConfigureAwait(false);

        try
        {
            await output.WriteAsync((line + "\n").AsMemory(), CancellationToken.None).ConfigureAwait(false);
            await output.FlushAsync(CancellationToken.None).ConfigureAwait(false);
        }
        finally
        {
            gate.Release();
        }
    }

    private async Task<RunRecord> ExecuteAsync(
        RunPlan plan,
        ModelConfig model,
        HarnessTask task,
        int run,
        SemaphoreSlim familyGate,
        SemaphoreSlim probeGate,
        CancellationToken ct)
    {
        var hash = PromptHash(plan.SystemPrompt, task.Prompt);
        ModelResponse? response = null;
        string? extractStatus = null;
        var compileSuccess = false;
        IReadOnlyList<string> diagnostics = [];
        UsageFacts? usage = null;
        ProbeResult? probeResult = null;
        string? transportError = null;
        string? notes = null;

        try
        {
            response = await this.CallModelAsync(plan, model, task, familyGate, ct).ConfigureAwait(false);
            transportError = response.Error;

            if (transportError is null)
            {
                var extract = CodeExtractor.Extract(response.Text);
                extractStatus = StatusName(extract.Status);

                if (extract.Code is not null)
                {
                    var compiled = SolutionCompiler.Compile(task, extract.Code, plan.IsWithLibrary);
                    compileSuccess = compiled.Success;
                    diagnostics = compiled.Diagnostics;
                    usage = UsageExtractor.Extract(compiled, task);

                    if (compiled.Success && compiled.Assembly is not null)
                    {
                        (probeResult, notes) = await this.ProbeAsync(compiled.Assembly, task, probeGate, ct).ConfigureAwait(false);
                    }
                }
            }
        }
        catch (Exception ex) when (!(ex is OperationCanceledException && ct.IsCancellationRequested))
        {
            var message = $"{ex.GetType().Name}: {ex.Message}";
            logger.RecordFailed(model.Family, model.Id, task.Id, run, ex.GetType().Name);

            if (response is null)
            {
                transportError = message;
            }
            else
            {
                notes = $"runner error: {message}";
            }
        }

        return new RunRecord(
            Ts: this.clock.GetUtcNow().UtcDateTime.ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'", CultureInfo.InvariantCulture),
            RunId: plan.RunId.ToString(),
            Condition: plan.Condition,
            Family: model.Family,
            Model: model.Id,
            Task: task.Id,
            Run: run,
            PromptHash: hash,
            RawOutput: response?.Text ?? string.Empty,
            ExtractStatus: extractStatus,
            CompileSuccess: compileSuccess,
            Diagnostics: diagnostics,
            Usage: usage,
            Probe: probeResult,
            InputTokens: response?.InputTokens ?? 0,
            OutputTokens: response?.OutputTokens ?? 0,
            LatencyMs: (long)(response?.Latency.TotalMilliseconds ?? 0),
            StopReason: response?.StopReason,
            Truncated: response?.Truncated ?? false,
            TransportError: transportError,
            Notes: notes);
    }

    private async Task<ModelResponse> CallModelAsync(
        RunPlan plan,
        ModelConfig model,
        HarnessTask task,
        SemaphoreSlim familyGate,
        CancellationToken ct)
    {
        if (!clients.TryGetValue(model.Family, out var client))
        {
            return new ModelResponse(model.Id, string.Empty, 0, 0, TimeSpan.Zero, $"no client for family '{model.Family}'");
        }

        await familyGate.WaitAsync(ct).ConfigureAwait(false);

        try
        {
            return await client.CompleteAsync(new ModelRequest(model.Id, plan.SystemPrompt, task.Prompt), ct).ConfigureAwait(false);
        }
        finally
        {
            familyGate.Release();
        }
    }

    private async Task<(ProbeResult? Result, string? Notes)> ProbeAsync(
        byte[] assembly,
        HarnessTask task,
        SemaphoreSlim probeGate,
        CancellationToken ct)
    {
        await probeGate.WaitAsync(ct).ConfigureAwait(false);

        try
        {
            return (await probe.RunAsync(assembly, task, ct).ConfigureAwait(false), null);
        }
        catch (Exception ex) when (!(ex is OperationCanceledException && ct.IsCancellationRequested))
        {
            return (null, $"probe error: {ex.GetType().Name}: {ex.Message}");
        }
        finally
        {
            probeGate.Release();
        }
    }

    private static string StatusName(ExtractStatus status) =>
        JsonNamingPolicy.CamelCase.ConvertName(status.ToString());
}
