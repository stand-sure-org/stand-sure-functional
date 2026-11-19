namespace ModelHarness.Report;

using System;
using System.Collections.Generic;
using System.Text.Json;

using ModelHarness.Run;

/// <summary>The records of one JSONL results file, plus how many non-blank lines could not be used.</summary>
/// <param name="Records">Successfully parsed records, in file order.</param>
/// <param name="MalformedLines">Non-blank lines that were not valid <see cref="RunRecord"/> JSON (or lacked family or task).</param>
public sealed record RunLog(IReadOnlyList<RunRecord> Records, int MalformedLines)
{
    /// <summary>Parses JSONL lines. Never throws on bad content: blank lines are ignored, malformed lines are counted.</summary>
    public static RunLog Parse(IEnumerable<string> lines)
    {
        ArgumentNullException.ThrowIfNull(lines);
        var records = new List<RunRecord>();
        var malformed = 0;
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            var record = TryParse(line);
            if (record is null)
            {
                malformed++;
                continue;
            }

            records.Add(record);
        }

        return new RunLog(records, malformed);
    }

    private static RunRecord? TryParse(string line)
    {
        try
        {
            var record = JsonSerializer.Deserialize(line, RunJson.Default.RunRecord);
            return record is { Family: { Length: > 0 }, Task: { Length: > 0 } } ? record : null;
        }
        catch (JsonException)
        {
            return null;
        }
    }
}
