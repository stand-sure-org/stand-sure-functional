namespace ModelHarness.Reextract;

using System;

/// <summary>Options of <c>reextract --input results.jsonl --output new.jsonl</c>.</summary>
public sealed record ReextractOptions(string InputPath, string OutputPath)
{
    public const string Command = "reextract";

    public const string Usage =
        "usage: reextract --input <jsonl> --output <new jsonl> (re-runs extraction, compile and usage facts from rawOutput; no model calls, no probe)";

    public static ReextractParseResult Parse(string[] args)
    {
        ArgumentNullException.ThrowIfNull(args);

        if (args.Length == 0 || !string.Equals(args[0], Command, StringComparison.Ordinal))
        {
            return Fail("expected the 'reextract' command");
        }

        string? input = null;
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

        return new ReextractParseResult(new ReextractOptions(input, output), null);
    }

    private static ReextractParseResult Fail(string message) => new(null, $"{message}\n{Usage}");
}
