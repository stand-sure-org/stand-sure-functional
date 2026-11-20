namespace ModelHarness.Report;

using System;

/// <summary>Options of <c>report --input file.jsonl [--compare other.jsonl] --output report.md</c>.</summary>
public sealed record ReportOptions(string InputPath, string? ComparePath, string OutputPath)
{
    public const string Command = "report";

    public const string Usage = "usage: report --input <jsonl> [--compare <jsonl>] --output <markdown file>";

    public static ReportParseResult Parse(string[] args)
    {
        ArgumentNullException.ThrowIfNull(args);

        if (args.Length == 0 || !string.Equals(args[0], Command, StringComparison.Ordinal))
        {
            return Fail("expected the 'report' command");
        }

        string? input = null;
        string? compare = null;
        string? output = null;

        for (var i = 1; i < args.Length; i += 2)
        {
            var name = args[i];

            if (i + 1 >= args.Length)
            {
                return Fail($"option {name} needs a value");
            }

            var value = args[i + 1];

            switch (name)
            {
                case "--input":
                    input = value;
                    break;
                case "--compare":
                    compare = value;
                    break;
                case "--output":
                    output = value;
                    break;
                default:
                    return Fail($"unknown option {name}");
            }
        }

        if (string.IsNullOrWhiteSpace(input) || string.IsNullOrWhiteSpace(output))
        {
            return Fail("--input and --output are required");
        }

        return new ReportParseResult(new ReportOptions(input, compare, output), null);
    }

    private static ReportParseResult Fail(string message) => new(null, $"{message}\n{Usage}");
}
