namespace ModelHarness.Reextract;

using System;
using System.IO;
using System.Text;

using Microsoft.Extensions.Logging;

using ModelHarness.Tasks;

/// <summary>Runs <c>reextract</c>: reads one JSONL file, writes a new one beside it. Returns a process exit code.</summary>
public static class ReextractCommand
{
    public const int ExitOk = 0;
    public const int ExitUsage = 2;

    /// <param name="options">Input and output paths. The output must not exist yet, and must differ from the input.</param>
    /// <param name="tasksRoot">The <c>harness/tasks</c> directory.</param>
    /// <param name="logger">Receives the summary or the usage error.</param>
    public static int Run(ReextractOptions options, string tasksRoot, ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(tasksRoot);
        ArgumentNullException.ThrowIfNull(logger);

        if (!File.Exists(options.InputPath))
        {
            logger.CliError($"Input file not found: {options.InputPath}");
            return ExitUsage;
        }

        if (string.Equals(Path.GetFullPath(options.InputPath), Path.GetFullPath(options.OutputPath), StringComparison.Ordinal)
            || File.Exists(options.OutputPath))
        {
            logger.CliError($"Output must be a new file (it exists or is the input): {options.OutputPath}");
            return ExitUsage;
        }

        var tasks = TaskCatalog.Load(tasksRoot);
        var directory = Path.GetDirectoryName(Path.GetFullPath(options.OutputPath));
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        ReextractSummary summary;
        using (var stream = new FileStream(options.OutputPath, FileMode.CreateNew, FileAccess.Write, FileShare.Read))
        using (var writer = new StreamWriter(stream, new UTF8Encoding(false)) { NewLine = "\n" })
        {
            summary = Reextractor.Run(File.ReadLines(options.InputPath), tasks, writer);
        }

        logger.ReextractWritten(
            summary.Total, summary.Rewritten, summary.PassedThrough, summary.Malformed, summary.CompileDrift, options.OutputPath);
        return ExitOk;
    }
}
