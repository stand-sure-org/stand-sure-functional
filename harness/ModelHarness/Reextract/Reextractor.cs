namespace ModelHarness.Reextract;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

using ModelHarness.Analysis;
using ModelHarness.Compile;
using ModelHarness.Run;
using ModelHarness.Tasks;

/// <summary>
/// Recomputes <see cref="RunRecord.Usage"/> for existing JSONL records from their <see cref="RunRecord.RawOutput"/>:
/// extract, compile, usage facts. No model calls and no probe.
/// </summary>
/// <remarks>
/// Only <c>usage</c> changes. <c>extractStatus</c>, <c>compileSuccess</c>, <c>diagnostics</c> and <c>probe</c> stay as
/// recorded, so they keep describing the run that produced the probe result (a fresh compile against today's library can
/// differ; see <see cref="ReextractSummary.CompileDrift"/>). Lines are written in input order; malformed lines, records
/// without model output (transport errors) and unknown task ids are copied byte for byte. Blank lines are dropped.
/// </remarks>
public static class Reextractor
{
    public static ReextractSummary Run(IEnumerable<string> lines, IReadOnlyList<HarnessTask> tasks, TextWriter output)
    {
        ArgumentNullException.ThrowIfNull(lines);
        ArgumentNullException.ThrowIfNull(tasks);
        ArgumentNullException.ThrowIfNull(output);

        var byId = tasks.ToDictionary(t => t.Id, StringComparer.Ordinal);
        int total = 0, rewritten = 0, passed = 0, malformed = 0, drift = 0;
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            total++;
            var record = TryParse(line);
            if (record is null)
            {
                malformed++;
                passed++;
                output.WriteLine(line);
                continue;
            }

            if (record.TransportError is not null
                || string.IsNullOrEmpty(record.RawOutput)
                || !byId.TryGetValue(record.Task, out var task))
            {
                passed++;
                output.WriteLine(line);
                continue;
            }

            var (usage, compiled) = Recompute(record, task);
            if (compiled is { } success && success != record.CompileSuccess)
            {
                drift++;
            }

            rewritten++;
            output.WriteLine(JsonSerializer.Serialize(record with { Usage = usage }, RunJson.Default.RunRecord));
        }

        return new ReextractSummary(total, rewritten, passed, malformed, drift);
    }

    // Mirrors HarnessRunner: no extracted code means no usage facts.
    private static (UsageFacts? Usage, bool? CompileSuccess) Recompute(RunRecord record, HarnessTask task)
    {
        var extract = CodeExtractor.Extract(record.RawOutput);
        if (extract.Code is null)
        {
            return (null, null);
        }

        var withLibrary = string.Equals(record.Condition, RunPlan.WithLibrary, StringComparison.Ordinal);
        var compiled = SolutionCompiler.Compile(task, extract.Code, withLibrary);
        return (UsageExtractor.Extract(compiled, task), compiled.Success);
    }

    private static RunRecord? TryParse(string line)
    {
        try
        {
            return JsonSerializer.Deserialize(line, RunJson.Default.RunRecord) is { Task: { Length: > 0 } } record ? record : null;
        }
        catch (JsonException)
        {
            return null;
        }
    }
}
