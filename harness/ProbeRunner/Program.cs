namespace ProbeRunner;

using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

/// <summary>
/// <c>ProbeRunner &lt;assemblyPath&gt; &lt;entryType&gt; &lt;entryMethod&gt;</c>: invokes a compiled solution once and
/// writes exactly one JSON line (<see cref="ProbeOutput"/>) to stdout.
/// </summary>
/// <remarks>
/// stdout is the IPC contract with <c>ModelHarness.Probe.ProbeExecutor</c>. The raw stdout stream is captured before any
/// model code runs; <c>Console.Out</c> and <c>Console.Error</c> are then pointed at <see cref="TextWriter.Null"/> so ordinary
/// model output never reaches the pipe. The process ends with <see cref="Environment.Exit(int)"/> so foreground threads
/// left by model code cannot keep it alive after the result line is written.
/// </remarks>
internal static class Program
{
    public static async Task<int> Main(string[] args)
    {
        // RS0030 bans System.Console for logging. This is not logging: stdout is the IPC channel to the parent (ruling R2).
#pragma warning disable RS0030
        var stdout = Console.OpenStandardOutput();
        Console.SetOut(TextWriter.Null);
        Console.SetError(TextWriter.Null);
#pragma warning restore RS0030

        ProbeOutput output;
        if (args.Length != 3)
        {
            output = ProbeOutput.Fail("usage: ProbeRunner <assemblyPath> <entryType> <entryMethod>");
        }
        else
        {
            output = await ProbeInvoker.RunAsync(Path.GetFullPath(args[0]), args[1], args[2]).ConfigureAwait(false);
        }

        var line = JsonSerializer.SerializeToUtf8Bytes(output, ProbeJson.Default.ProbeOutput);
        stdout.Write("\n"u8);
        stdout.Write(line);
        stdout.Write("\n"u8);
        stdout.Flush();
        Environment.Exit(0);
        return 0;
    }
}
