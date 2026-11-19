namespace ModelHarness.Report;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

using ModelHarness.Analysis;
using ModelHarness.Probe;
using ModelHarness.Run;

/// <summary>Renders a results JSONL file as a deterministic Markdown report (no timestamps, fixed ordering).</summary>
public static class ReportBuilder
{
    private const string Overall = "overall";
    private const int TopCount = 30;
    private const int RegressionPoints = 10;
    private const int MinRegressionN = 3;

    private static readonly HashSet<string> LibraryTypes = new(StringComparer.Ordinal) { "Result", "Option", "Error", "Unit", "Try" };

    private sealed record Scope(string Name, IReadOnlyList<RunRecord> Records);

    /// <summary>Builds the report. <paramref name="compare"/> enables section 7 regression notes.</summary>
    public static string Build(RunLog input, RunLog? compare, string inputName, string? compareName)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(inputName);

        var scopes = Scopes(input.Records);
        var sb = new StringBuilder();
        sb.Append("# Harness report: ").Append(Cell(inputName, code: true)).Append('\n').Append('\n');
        sb.Append("Records: ").Append(input.Records.Count.ToString(CultureInfo.InvariantCulture));
        if (compare is not null)
        {
            sb.Append(" | Compared with: ").Append(Cell(compareName ?? "(compare file)", code: true));
        }

        sb.Append('\n').Append('\n');

        AppendDataQuality(sb, scopes, input.MalformedLines, compare, compareName);
        AppendCompileRate(sb, scopes);
        AppendProbeRates(sb, scopes);
        AppendFrequency(sb, scopes, "## 3. Entry return type frequency", "Return type", "Records with extracted usage facts (n); `(none)` = no entry method found.",
            r => [r.Usage?.EntryReturnType ?? "(none)"], null);
        AppendFrequency(sb, scopes, "## 4. Idiom frequency", "Idiom", "Records with extracted usage facts (n); a record can carry several idioms, so shares do not sum to 100%.",
            r => r.Usage?.Idioms ?? [], null);
        AppendFrequency(sb, scopes, "## 5. Top 30 invoked method names on outcome-like types", "Method", "Records invoking the method (n = records with usage facts). Only `Type.Method` entries whose type is outcome-like (name contains Result, Option, Outcome or Either, or a library type); ranked by count, ties alphabetical.",
            r => (r.Usage?.InvokedMethods ?? []).Where(IsOutcomeMethod), TopCount, rankStyle: true);
        AppendFrequency(sb, scopes, "## 6. Self-declared type names", "Type", $"Records with extracted usage facts (n); top {TopCount} per scope, ties alphabetical.",
            r => r.Usage?.DeclaredTypes ?? [], TopCount);
        AppendRegressions(sb, input.Records, compare);
        AppendCaveats(sb, input.Records);
        return sb.ToString();
    }

    private static List<Scope> Scopes(IReadOnlyList<RunRecord> records)
    {
        var scopes = records
            .GroupBy(r => r.Family, StringComparer.Ordinal)
            .OrderBy(g => g.Key, StringComparer.Ordinal)
            .Select(g => new Scope(g.Key, g.ToList()))
            .ToList();
        scopes.Add(new Scope(Overall, records));
        return scopes;
    }

    private static bool IsTransport(RunRecord r) => r.TransportError is not null;

    private static bool IsRunnerError(RunRecord r) =>
        r.TransportError is null && r.Notes is not null && r.Notes.StartsWith("runner error", StringComparison.Ordinal);

    private static bool IsExcluded(RunRecord r) => IsTransport(r) || r.Truncated || IsRunnerError(r);

    private static IEnumerable<RunRecord> Eligible(IEnumerable<RunRecord> records) => records.Where(r => !IsExcluded(r));

    private static IEnumerable<RunRecord> Probed(IEnumerable<RunRecord> records) =>
        Eligible(records).Where(r => r.CompileSuccess && r.Probe is not null);

    private static IEnumerable<RunRecord> CompiledNotProbed(IEnumerable<RunRecord> records) =>
        Eligible(records).Where(r => r.CompileSuccess && r.Probe is null);

    private static IEnumerable<RunRecord> Analysable(IEnumerable<RunRecord> records) =>
        Eligible(records).Where(r => r.Usage is not null);

    private static bool IsOutcomeMethod(string method)
    {
        var dot = method.IndexOf('.', StringComparison.Ordinal);
        if (dot <= 0)
        {
            return false;
        }

        var type = method[..dot];
        return LibraryTypes.Contains(type) || OutcomeTypes.IsOutcomeName(type);
    }

    private static void AppendDataQuality(StringBuilder sb, List<Scope> scopes, int malformed, RunLog? compare, string? compareName)
    {
        sb.Append("## Data quality\n\n");
        sb.Append("Read this first. Records with a transport error, a truncated model answer or a runner error (a failure of the harness itself after the model answered) are counted here but are EXCLUDED from the compile-rate and probe-rate denominators below. ");
        sb.Append("`Compile failed` and `Probe crashed` count eligible (non-excluded) records only; `extractFailed` is one cause of a compile failure.\n\n");
        sb.Append("| Family | Records | Transport errors | Truncated | Runner errors | Extract failed | Concatenated | Compile failed | Probe crashed |\n");
        sb.Append("|---|---|---|---|---|---|---|---|---|\n");
        foreach (var s in scopes)
        {
            var truncated = s.Records.Where(r => r.Truncated).ToList();
            var byReason = truncated
                .GroupBy(r => r.StopReason ?? "(none)", StringComparer.Ordinal)
                .OrderBy(g => g.Key, StringComparer.Ordinal)
                .Select(g => $"{g.Key}: {g.Count().ToString(CultureInfo.InvariantCulture)}");
            var truncatedCell = truncated.Count == 0
                ? "0"
                : $"{truncated.Count.ToString(CultureInfo.InvariantCulture)} ({Cell(string.Join(", ", byReason))})";
            Row(
                sb,
                Cell(s.Name),
                N(s.Records.Count),
                N(s.Records.Count(IsTransport)),
                truncatedCell,
                N(s.Records.Count(IsRunnerError)),
                N(s.Records.Count(r => r.ExtractStatus == "extractFailed")),
                N(s.Records.Count(r => r.ExtractStatus == "concatenated")),
                N(Eligible(s.Records).Count(r => !r.CompileSuccess)),
                N(Eligible(s.Records).Count(r => r.Probe?.Outcome == ProbeOutcome.Crashed)));
        }

        sb.Append('\n').Append("Malformed lines skipped: ").Append(N(malformed)).Append('\n');
        if (compare is not null)
        {
            sb.Append("Malformed lines skipped in ").Append(Cell(compareName ?? "compare file", code: true)).Append(": ").Append(N(compare.MalformedLines)).Append('\n');
        }

        sb.Append('\n');
    }

    private static void AppendCompileRate(StringBuilder sb, List<Scope> scopes)
    {
        sb.Append("## 1. Compile rate\n\n");
        sb.Append("Denominator n = records that are not transport errors, not truncated and not runner errors (those are excluded; see Data quality). Compile rate = compiled / n.\n\n");
        sb.Append("| Family | n (eligible) | Compiled | Compile rate |\n|---|---|---|---|\n");
        foreach (var s in scopes)
        {
            var eligible = Eligible(s.Records).ToList();
            var compiled = eligible.Count(r => r.CompileSuccess);
            Row(sb, Cell(s.Name), N(eligible.Count), N(compiled), Pct(compiled, eligible.Count));
        }

        sb.Append('\n');
    }

    private static void AppendProbeRates(StringBuilder sb, List<Scope> scopes)
    {
        sb.Append("## 2. Probe outcome rates\n\n");
        sb.Append("Denominator n = eligible records that compiled and have a probe result (transport-error, truncated and runner-error records excluded). `crashed` means no verdict and stays in n. `compiled, not probed` counts eligible records that compiled but have no probe result; they are not in n.\n\n");
        sb.Append("| Family | n (probed) | threw | returned | timeout | crashed | compiled, not probed |\n|---|---|---|---|---|---|---|\n");
        foreach (var s in scopes)
        {
            var probed = Probed(s.Records).ToList();
            string Count(ProbeOutcome o)
            {
                var c = probed.Count(r => r.Probe!.Outcome == o);
                return $"{N(c)} ({Pct(c, probed.Count)})";
            }

            Row(sb, Cell(s.Name), N(probed.Count), Count(ProbeOutcome.Threw), Count(ProbeOutcome.Returned), Count(ProbeOutcome.Timeout), Count(ProbeOutcome.Crashed), N(CompiledNotProbed(s.Records).Count()));
        }

        sb.Append('\n');
    }

    private static void AppendFrequency(
        StringBuilder sb,
        List<Scope> scopes,
        string heading,
        string label,
        string note,
        Func<RunRecord, IEnumerable<string>> select,
        int? take,
        bool rankStyle = false)
    {
        sb.Append(heading).Append("\n\n").Append(note).Append("\n\n");
        foreach (var s in scopes)
        {
            var analysable = Analysable(s.Records).ToList();
            sb.Append("### ").Append(Cell(s.Name)).Append(" (n = ").Append(N(analysable.Count)).Append(")\n\n");
            var ranked = analysable
                .SelectMany(r => select(r).Distinct(StringComparer.Ordinal))
                .GroupBy(x => x, StringComparer.Ordinal)
                .Select(g => (Key: g.Key, Count: g.Count()))
                .OrderByDescending(x => x.Count)
                .ThenBy(x => x.Key, StringComparer.Ordinal)
                .Take(take ?? int.MaxValue)
                .ToList();
            if (ranked.Count == 0)
            {
                sb.Append("None.\n\n");
                continue;
            }

            sb.Append(rankStyle ? $"| Rank | {label} | Records | Share |\n|---|---|---|---|\n" : $"| {label} | Records | Share |\n|---|---|---|\n");
            var rank = 1;
            foreach (var (key, count) in ranked)
            {
                if (rankStyle)
                {
                    Row(sb, N(rank++), Cell(key, code: true), N(count), Pct(count, analysable.Count));
                }
                else
                {
                    Row(sb, Cell(key, code: true), N(count), Pct(count, analysable.Count));
                }
            }

            sb.Append('\n');
        }
    }

    private static void AppendRegressions(StringBuilder sb, IReadOnlyList<RunRecord> current, RunLog? compare)
    {
        sb.Append("## 7. Per-task notes: regressions versus --compare\n\n");
        if (compare is null)
        {
            sb.Append("No --compare file was supplied, so no regression check was made.\n\n");
            return;
        }

        sb.Append($"Rule: a family/task cell regresses when its compile-failure rate or its `threw` rate is worse than in the compare file by more than {RegressionPoints} percentage points, with n >= {MinRegressionN} on both sides. ");
        sb.Append("Compile n is eligible records; threw n is probed records (see sections 1 and 2). Families are checked individually, not overall.\n\n");

        var notes = new List<string>();
        var families = current.Select(r => r.Family).Distinct(StringComparer.Ordinal).OrderBy(f => f, StringComparer.Ordinal);
        foreach (var family in families)
        {
            var tasks = current.Where(r => r.Family == family).Select(r => r.Task).Distinct(StringComparer.Ordinal).OrderBy(t => t, StringComparer.Ordinal);
            foreach (var task in tasks)
            {
                var now = current.Where(r => r.Family == family && r.Task == task).ToList();
                var before = compare.Records.Where(r => r.Family == family && r.Task == task).ToList();
                var parts = new List<string>();

                var nowElig = Eligible(now).ToList();
                var beforeElig = Eligible(before).ToList();
                var nowFail = nowElig.Count(r => !r.CompileSuccess);
                var beforeFail = beforeElig.Count(r => !r.CompileSuccess);
                if (IsWorse(nowFail, nowElig.Count, beforeFail, beforeElig.Count))
                {
                    parts.Add($"compile rate regressed: {Pct(beforeElig.Count - beforeFail, beforeElig.Count)} ({N(beforeElig.Count - beforeFail)}/{N(beforeElig.Count)}) -> {Pct(nowElig.Count - nowFail, nowElig.Count)} ({N(nowElig.Count - nowFail)}/{N(nowElig.Count)})");
                }

                var nowProbed = Probed(now).ToList();
                var beforeProbed = Probed(before).ToList();
                var nowThrew = nowProbed.Count(r => r.Probe!.Outcome == ProbeOutcome.Threw);
                var beforeThrew = beforeProbed.Count(r => r.Probe!.Outcome == ProbeOutcome.Threw);
                if (IsWorse(nowThrew, nowProbed.Count, beforeThrew, beforeProbed.Count))
                {
                    parts.Add($"threw rate regressed: {Pct(beforeThrew, beforeProbed.Count)} ({N(beforeThrew)}/{N(beforeProbed.Count)}) -> {Pct(nowThrew, nowProbed.Count)} ({N(nowThrew)}/{N(nowProbed.Count)})");
                }

                if (parts.Count > 0)
                {
                    notes.Add($"- **{Cell(family)} / {Cell(task)}**: {string.Join("; ", parts)}");
                }
            }
        }

        if (notes.Count == 0)
        {
            sb.Append("No regressions found.\n\n");
            return;
        }

        foreach (var note in notes)
        {
            sb.Append(note).Append('\n');
        }

        sb.Append('\n');
    }

    /// <summary>True when bad/n rose by strictly more than the threshold; exact integer arithmetic, no float edge cases.</summary>
    private static bool IsWorse(int nowBad, int nowN, int beforeBad, int beforeN)
    {
        if (nowN < MinRegressionN || beforeN < MinRegressionN)
        {
            return false;
        }

        return 100L * (((long)nowBad * beforeN) - ((long)beforeBad * nowN)) > (long)RegressionPoints * nowN * beforeN;
    }

    private static void AppendCaveats(StringBuilder sb, IReadOnlyList<RunRecord> records)
    {
        var t07 = records.Where(r => TaskNumber(r, "07")).ToList();
        var aspnet = t07.Count(r => (r.Usage?.InvokedMethods ?? []).Any(m => m.StartsWith("IResult.", StringComparison.Ordinal) || m.StartsWith("Results.", StringComparison.Ordinal)));
        var t08 = Probed(records.Where(r => TaskNumber(r, "08"))).ToList();
        var t09 = Probed(records.Where(r => TaskNumber(r, "09"))).ToList();

        sb.Append("## Caveats\n\n");
        sb.Append($"- Task 08 is a low-signal cell for the throw/no-throw probe: it has no failure path, so `threw` there says little about expected-failure handling. In this input: {N(t08.Count)} probed records, {N(t08.Count(r => r.Probe!.Outcome == ProbeOutcome.Threw))} threw.\n");
        sb.Append($"- Task 09's probe never exercises cancellation, so cancellation handling is invisible to the probe. In this input: {N(t09.Count)} probed records.\n");
        sb.Append($"- Outcome-type detection is name-based and has known false positives, for example ASP.NET `IResult` / `Results.Ok` in task 07. In this input: {N(aspnet)} of {N(t07.Count)} task 07 records invoked `IResult.*` or `Results.*` methods; those entries appear in section 5 although they are not functional outcome types.\n");
        sb.Append("- Result-line forgery by model code is a documented limit of the probe: solution code can print a fake result line, so a `returned` or `threw` verdict is not tamper-proof.\n");
    }

    private static bool TaskNumber(RunRecord r, string number) =>
        r.Task.StartsWith(number + "-", StringComparison.Ordinal) || string.Equals(r.Task, number, StringComparison.Ordinal);

    private static string N(int value) => value.ToString(CultureInfo.InvariantCulture);

    private static string Pct(int numerator, int denominator) =>
        denominator == 0 ? "n/a" : (100.0 * numerator / denominator).ToString("0.0", CultureInfo.InvariantCulture) + "%";

    private static void Row(StringBuilder sb, params string[] cells) =>
        sb.Append("| ").AppendJoin(" | ", cells).Append(" |\n");

    /// <summary>Makes a value safe for a Markdown table cell: escapes pipes, flattens newlines, optionally wraps in a code span.</summary>
    private static string Cell(string value, bool code = false)
    {
        var flat = value.Replace("\r", " ", StringComparison.Ordinal).Replace("\n", " ", StringComparison.Ordinal)
            .Replace("|", "\\|", StringComparison.Ordinal);
        return code ? "`" + flat.Replace("`", "'", StringComparison.Ordinal) + "`" : flat;
    }
}
