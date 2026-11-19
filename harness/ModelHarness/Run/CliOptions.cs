namespace ModelHarness.Run;

using System;
using System.Collections.Generic;
using System.Globalization;

/// <summary>Options of <c>run --condition baseline|with-library [--runs 5] [--models a,b] [--tasks x,y] [--output dir] [--ollama-timeout-minutes 10]</c>.</summary>
public sealed record CliOptions(
    string Condition,
    int Runs,
    IReadOnlyList<string>? Models,
    IReadOnlyList<string>? Tasks,
    string? OutputDirectory,
    TimeSpan OllamaTimeout)
{
    public const int DefaultRuns = 5;

    public const string Usage =
        "usage: run --condition baseline|with-library [--runs N] [--models id,...] [--tasks id,...] [--output dir] [--ollama-timeout-minutes N]";

    /// <summary>Hand-rolled parser: tiny surface, no dependencies.</summary>
    public static CliParseResult Parse(string[] args)
    {
        ArgumentNullException.ThrowIfNull(args);
        if (args.Length == 0 || !string.Equals(args[0], "run", StringComparison.Ordinal))
        {
            return Fail("expected the 'run' command");
        }

        string? condition = null;
        var runs = DefaultRuns;
        IReadOnlyList<string>? models = null;
        IReadOnlyList<string>? tasks = null;
        string? output = null;
        var ollamaTimeout = ClientFactory.DefaultOllamaTimeout;

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
                case "--condition":
                    condition = value;
                    break;
                case "--runs":
                    if (!int.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out runs) || runs < 1)
                    {
                        return Fail("--runs must be a positive integer");
                    }

                    break;
                case "--models":
                    models = Split(value);
                    break;
                case "--tasks":
                    tasks = Split(value);
                    break;
                case "--output":
                    output = value;
                    break;
                case "--ollama-timeout-minutes":
                    if (!int.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out var minutes) || minutes < 1)
                    {
                        return Fail("--ollama-timeout-minutes must be a positive integer");
                    }

                    ollamaTimeout = TimeSpan.FromMinutes(minutes);
                    break;
                default:
                    return Fail($"unknown option {name}");
            }
        }

        if (condition is not (RunPlan.Baseline or RunPlan.WithLibrary))
        {
            return Fail("--condition must be baseline or with-library");
        }

        return new CliParseResult(new CliOptions(condition, runs, models, tasks, output, ollamaTimeout), null);
    }

    private static string[] Split(string value) =>
        value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    private static CliParseResult Fail(string message) => new(null, $"{message}\n{Usage}");
}
