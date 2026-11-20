namespace ModelHarness;

using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using ModelHarness.Models;
using ModelHarness.Reextract;
using ModelHarness.Report;
using ModelHarness.Run;
using ModelHarness.Tasks;

internal static class Program
{
    private const int ExitOk = 0;
    private const int ExitUsage = 2;
    private const int ExitCancelled = 130;

    private static async Task<int> Main(string[] args)
    {
        using var loggerFactory = LoggerFactory.Create(b => b.AddSimpleConsole(o => o.SingleLine = true));
        var logger = loggerFactory.CreateLogger("ModelHarness");

        if (args.Length > 0 && string.Equals(args[0], ReportOptions.Command, StringComparison.Ordinal))
        {
            var report = ReportOptions.Parse(args);

            if (report.Options is not { } reportOptions)
            {
                logger.CliError(report.Error ?? ReportOptions.Usage);
                return ExitUsage;
            }

            return ReportCommand.Run(reportOptions, logger);
        }

        if (args.Length > 0 && string.Equals(args[0], ReextractOptions.Command, StringComparison.Ordinal))
        {
            var reextract = ReextractOptions.Parse(args);

            if (reextract.Options is not { } reextractOptions)
            {
                logger.CliError(reextract.Error ?? ReextractOptions.Usage);
                return ExitUsage;
            }

            var root = FindRoot();

            if (root is null)
            {
                logger.CliError($"Could not find {RepoLocator.SolutionFileName} above the current or base directory.");
                return ExitUsage;
            }

            return ReextractCommand.Run(reextractOptions, Path.Combine(root, "harness", "tasks"), logger);
        }

        var parsed = CliOptions.Parse(args);

        if (parsed.Options is not { } options)
        {
            logger.CliError(parsed.Error ?? CliOptions.Usage);
            return ExitUsage;
        }

        using var cts = new CancellationTokenSource();

        using var sigint = PosixSignalRegistration.Create(PosixSignal.SIGINT,
            context =>
            {
                context.Cancel = true;
                cts.Cancel();
            });

        using var sigterm = PosixSignalRegistration.Create(PosixSignal.SIGTERM,
            context =>
            {
                context.Cancel = true;
                cts.Cancel();
            });

        try
        {
            return await RunAsync(options, logger, cts.Token).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            return ExitCancelled;
        }
    }

    private static async Task<int> RunAsync(CliOptions options, ILogger logger, CancellationToken ct)
    {
        var root = FindRoot();

        if (root is null)
        {
            logger.CliError($"Could not find {RepoLocator.SolutionFileName} above the current or base directory.");
            return ExitUsage;
        }

        var promptsDir = Path.Combine(root, "harness", "prompts");
        var withLibrary = string.Equals(options.Condition, RunPlan.WithLibrary, StringComparison.Ordinal);
        string systemPrompt;

        if (withLibrary)
        {
            var summaryPath = Path.Combine(root, "docs", "llm-api-summary.md");

            if (!File.Exists(summaryPath))
            {
                logger.CliError($"with-library needs {summaryPath}, which does not exist yet. No models were called.");
                return ExitUsage;
            }

            systemPrompt = SystemPrompts.ComposeWithLibrary(
                await File.ReadAllTextAsync(Path.Combine(promptsDir, "system-with-library.md"), ct).ConfigureAwait(false),
                await File.ReadAllTextAsync(summaryPath, ct).ConfigureAwait(false));
        }
        else
        {
            systemPrompt = (await File.ReadAllTextAsync(Path.Combine(promptsDir, "system-baseline.md"), ct).ConfigureAwait(false))
                .Replace("\r\n", "\n", StringComparison.Ordinal).TrimEnd();
        }

        var allModels = ModelConfig.LoadEnabledFromFile(Path.Combine(root, "harness", "models.json"));
        var allTasks = TaskCatalog.Load(Path.Combine(root, "harness", "tasks"));
        var models = Select(allModels, options.Models, m => m.Id, "model", logger, out var modelsOk);
        var tasks = Select(allTasks, options.Tasks, t => t.Id, "task", logger, out var tasksOk);

        if (!modelsOk || !tasksOk)
        {
            return ExitUsage;
        }

        var outputDir = options.OutputDirectory ?? Path.Combine(root, "harness", "results", "raw");
        Directory.CreateDirectory(outputDir);
        var startedAt = DateTimeOffset.UtcNow;
        var path = Path.Combine(outputDir, HarnessRunner.OutputFileName(startedAt, options.Condition));
        var plan = new RunPlan(Guid.NewGuid(), options.Condition, systemPrompt, options.Runs, models, tasks);

        var clients = ClientFactory.Create(
            new ProcessEnvironment(),
            logger,
            timeout => new HttpClient { Timeout = timeout },
            options.OllamaTimeout);

        var runner = new HarnessRunner(clients, new ProbeExecutorRunner(), logger);

        logger.RunStarted(plan.RunId.ToString(), plan.Condition, models.Count, tasks.Count, plan.Runs, path);
        await using var stream = new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.Read);
        await using var writer = new StreamWriter(stream, new UTF8Encoding(false)) { NewLine = "\n" };
        var summary = await runner.RunAsync(plan, writer, ct).ConfigureAwait(false);
        logger.RunFinished(summary.Written, summary.Cancelled);
        return summary.Cancelled ? ExitCancelled : ExitOk;
    }

    private static System.Collections.Generic.IReadOnlyList<T> Select<T>(
        System.Collections.Generic.IReadOnlyList<T> all,
        System.Collections.Generic.IReadOnlyList<string>? wanted,
        Func<T, string> id,
        string kind,
        ILogger logger,
        out bool ok)
    {
        ok = true;

        if (wanted is null)
        {
            return all;
        }

        var unknown = wanted.Where(w => !all.Any(a => string.Equals(id(a), w, StringComparison.Ordinal))).ToList();

        if (unknown.Count > 0)
        {
            logger.CliError($"Unknown {kind} id(s): {string.Join(", ", unknown)}. Known: {string.Join(", ", all.Select(id))}");
            ok = false;
            return [];
        }

        return all.Where(a => wanted.Contains(id(a), StringComparer.Ordinal)).ToList();
    }

    private static string? FindRoot()
    {
        foreach (var start in new[] { Directory.GetCurrentDirectory(), AppContext.BaseDirectory })
        {
            try
            {
                return RepoLocator.FindRoot(start);
            }
            catch (DirectoryNotFoundException)
            {
                // Try the next starting point.
            }
        }

        return null;
    }
}
