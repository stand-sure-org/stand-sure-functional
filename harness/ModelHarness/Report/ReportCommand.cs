namespace ModelHarness.Report;

using System;
using System.IO;
using System.Text;

using Microsoft.Extensions.Logging;

/// <summary>Runs <c>report</c>: reads the JSONL file(s), writes the Markdown report. Returns a process exit code.</summary>
public static class ReportCommand
{
    public const int ExitOk = 0;
    public const int ExitUsage = 2;

    public static int Run(ReportOptions options, ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(logger);

        if (!File.Exists(options.InputPath))
        {
            logger.CliError($"Input file not found: {options.InputPath}");
            return ExitUsage;
        }

        if (options.ComparePath is not null && !File.Exists(options.ComparePath))
        {
            logger.CliError($"Compare file not found: {options.ComparePath}");
            return ExitUsage;
        }

        var input = RunLog.Parse(File.ReadLines(options.InputPath));
        var compare = options.ComparePath is null ? null : RunLog.Parse(File.ReadLines(options.ComparePath));
        var report = ReportBuilder.Build(
            input,
            compare,
            Path.GetFileName(options.InputPath),
            options.ComparePath is null ? null : Path.GetFileName(options.ComparePath));

        var directory = Path.GetDirectoryName(Path.GetFullPath(options.OutputPath));
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        File.WriteAllText(options.OutputPath, report, new UTF8Encoding(false));
        logger.ReportWritten(input.Records.Count, input.MalformedLines, options.OutputPath);
        return ExitOk;
    }
}
