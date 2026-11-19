namespace ModelHarness.Run;

using System.Collections.Generic;

using ModelHarness.Analysis;
using ModelHarness.Probe;

/// <summary>One JSONL line. <c>condition</c>, <c>family</c>, <c>model</c> and <c>task</c> are flat so they work as Loki labels.</summary>
/// <param name="Ts">ISO-8601 UTC completion time.</param>
/// <param name="RunId">Shared by every record of one CLI invocation.</param>
/// <param name="PromptHash">SHA-256 hex of system prompt, <c>\n---\n</c>, user prompt.</param>
/// <param name="ExtractStatus"><c>single</c>, <c>concatenated</c> or <c>extractFailed</c>; null when no model output was obtained.</param>
/// <param name="Usage">Null on transport error or when nothing was extracted.</param>
/// <param name="Probe">Null unless the solution compiled.</param>
/// <param name="StopReason">Provider stop/finish reason.</param>
/// <param name="Truncated">True when the model output was cut off (max tokens, safety, ...).</param>
/// <param name="TransportError">Set when no usable model answer was obtained.</param>
/// <param name="Notes">Runner-side failures after a model answer (for example the probe threw).</param>
public sealed record RunRecord(
    string Ts,
    string RunId,
    string Condition,
    string Family,
    string Model,
    string Task,
    int Run,
    string PromptHash,
    string RawOutput,
    string? ExtractStatus,
    bool CompileSuccess,
    IReadOnlyList<string> Diagnostics,
    UsageFacts? Usage,
    ProbeResult? Probe,
    int InputTokens,
    int OutputTokens,
    long LatencyMs,
    string? StopReason,
    bool Truncated,
    string? TransportError,
    string? Notes);
