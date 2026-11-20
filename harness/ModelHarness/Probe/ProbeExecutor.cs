namespace ModelHarness.Probe;

using System;
using System.Buffers;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using ModelHarness.Compilation;
using ModelHarness.Tasks;

/// <summary>Runs a compiled solution once, out of process, under a macOS sandbox, and records how it ended.</summary>
/// <remarks>
/// <para>Process: the assembly is written to <c>solution.dll</c> in a fresh temp directory, which is also the working directory
/// and <c>HOME</c>. ProbeRunner (<c>dotnet ProbeRunner.dll &lt;assembly&gt; &lt;entryType&gt; &lt;entryMethod&gt;</c>) is started under
/// <c>/usr/bin/sandbox-exec</c> with <see cref="SandboxProfile"/>. Its environment is cleared and holds only <c>PATH</c>,
/// <c>HOME</c>, <c>TMPDIR</c> (both the probe dir), <c>DOTNET_ROOT</c>, <c>DOTNET_EnableDiagnostics=0</c> and <c>DOTNET_GCHeapHardLimit</c> (1 GiB), so API keys in the harness's environment never reach
/// model code. Stdin is closed immediately.</para>
/// <para>Location: ProbeRunner runs from its own build output. ModelHarness references the project for build order only and
/// bakes ProbeRunner's <c>TargetPath</c> into <c>[AssemblyMetadata("ProbeRunnerPath")]</c> at build time.</para>
/// <para>stdout protocol: ProbeRunner writes one JSON line (<c>{"outcome":"returned|threw|crashed","exceptionType":...,"reason":...}</c>)
/// and silences <c>Console.Out</c>/<c>Console.Error</c> before model code runs. The executor keeps only the last
/// <see cref="StdoutTailBytes"/> bytes of stdout and takes the last line that parses as a result with a known outcome; stderr is
/// drained and discarded. Model-controlled strings are truncated to <see cref="MaxFieldLength"/> characters.</para>
/// <para>Termination: on timeout the whole process tree is killed and the result is <see cref="ProbeOutcome.Timeout"/>; on caller
/// cancellation the tree is killed and <see cref="OperationCanceledException"/> propagates. Every wait after a kill is bounded by
/// <see cref="KillGrace"/>. The temp directory is deleted afterwards (best effort). A timeout that is not positive or exceeds
/// <see cref="MaxTimeout"/> (including <see cref="System.Threading.Timeout.InfiniteTimeSpan"/>) is refused before anything starts:
/// the result is <see cref="ProbeOutcome.Crashed"/> with reason <c>invalid-timeout: ...</c>, never an exception.</para>
/// </remarks>
public static class ProbeExecutor
{
    /// <summary>The default per-probe timeout.</summary>
    public static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(10);

    /// <summary>The largest accepted per-probe timeout; anything above it (or not positive) is refused as <c>invalid-timeout</c>.</summary>
    public static readonly TimeSpan MaxTimeout = TimeSpan.FromMinutes(5);

    /// <summary>
    /// The Seatbelt profile. Parameters (passed with <c>-D</c>, never spliced into the text; all real paths with every symlink
    /// resolved, because Seatbelt matches real paths only): <c>PROBE_DIR</c>, <c>RUNNER_DIR</c>, <c>DOTNET_ROOT</c> and
    /// <c>DOTNET</c> (the muxer).
    /// </summary>
    /// <remarks>
    /// <para>Deny by default: no network (TCP, loopback and Unix-domain sockets alike), no process fork, no exec other than the
    /// dotnet muxer, no signals to other processes, no Mach/XPC service lookups (so no LaunchServices, keychain or DNS daemon).</para>
    /// <para>Reads are an allowlist; everything else (home, repositories, the user temp and cache dirs, <c>/opt/homebrew</c>,
    /// <c>/usr/local</c>, <c>/Library</c>, <c>/Users/Shared</c>, <c>/private/etc</c>, other users' files) is denied. Contents and
    /// listings (<c>file-read*</c>) are allowed only under:</para>
    /// <list type="bullet">
    /// <item><c>PROBE_DIR</c>, <c>RUNNER_DIR</c>, <c>DOTNET_ROOT</c>: the solution, ProbeRunner with its deps, the shared frameworks.</item>
    /// <item><c>/usr/share/icu</c>: ICU data (cultures, casing, collation, normalisation).</item>
    /// <item><c>/usr/share/zoneinfo</c>, <c>/private/var/db/timezone</c> and the literal <c>/private/etc/localtime</c>: time zones,
    /// so <c>TimeZoneInfo.Local</c> matches the host instead of silently becoming UTC.</item>
    /// <item>The literal <c>/</c> (the root directory's own entry and listing, not its subtree): the host opens it while
    /// resolving paths and aborts without it.</item>
    /// </list>
    /// <para>Metadata (<c>file-read-metadata</c>: stat and readlink, not contents or listings) is allowed only on the ancestors of
    /// the three roots (the muxer and runtime stat every component on the way to them) and on the <c>/etc</c> and <c>/var</c>
    /// symlinks (followed to reach <c>localtime</c> and zoneinfo). Each rule above was removed in turn and a regression test
    /// failed; nothing else (not <c>/System</c>, <c>/usr/lib</c> or <c>/dev/urandom</c>: libraries come from the dyld shared
    /// cache) is needed.</para>
    /// <para>Writes only inside the probe directory (and <c>/dev/null</c>). <c>sysctl-read</c> is the only other allowance.</para>
    /// </remarks>
    internal const string SandboxProfile = """
        (version 1)
        (deny default)
        (deny network*)
        (allow process-exec (literal (param "DOTNET")))
        (allow file-read-metadata
            (literal "/etc")
            (literal "/var")
            (path-ancestors (param "DOTNET_ROOT"))
            (path-ancestors (param "RUNNER_DIR"))
            (path-ancestors (param "PROBE_DIR")))
        (allow file-read*
            (literal "/")
            (subpath "/usr/share/icu")
            (subpath "/usr/share/zoneinfo")
            (subpath "/private/var/db/timezone")
            (literal "/private/etc/localtime")
            (subpath (param "DOTNET_ROOT"))
            (subpath (param "RUNNER_DIR"))
            (subpath (param "PROBE_DIR")))
        (allow file-write* (subpath (param "PROBE_DIR")))
        (allow file-write-data (literal "/dev/null"))
        (allow sysctl-read)
        """;

    private const string SandboxExec = "/usr/bin/sandbox-exec";
    private const string AssemblyFileName = "solution.dll";
    private const string ProbeRunnerPathKey = "ProbeRunnerPath";
    private const string FallbackPath = "/usr/bin:/bin";
    private const int StdoutTailBytes = 64 * 1024;
    private const int MaxFieldLength = 256;
    private const string GcHeapHardLimit = "0x40000000";

    private static readonly TimeSpan KillGrace = TimeSpan.FromSeconds(3);

    /// <summary>Probes <paramref name="assembly"/> (a compiled solution with the task's fixture and probe) once.</summary>
    /// <param name="assembly">The image from <c>CompileResult.Assembly</c>.</param>
    /// <param name="task">Supplies <see cref="HarnessTask.EntryType"/> and <see cref="HarnessTask.EntryMethod"/>.</param>
    /// <param name="timeout">Wall-clock limit for the child process (see <see cref="DefaultTimeout"/>); must be in (0, <see cref="MaxTimeout"/>].</param>
    /// <param name="ct">Cancelling kills the child and throws <see cref="OperationCanceledException"/>.</param>
    public static Task<ProbeResult> RunAsync(byte[] assembly, HarnessTask task, TimeSpan timeout, CancellationToken ct) =>
        RunAsync(assembly, task, timeout, null, ct);

    /// <summary>As the public overload; <paramref name="processStarted"/> receives the child's process id (for tests).</summary>
    internal static Task<ProbeResult> RunAsync(
        byte[] assembly,
        HarnessTask task,
        TimeSpan timeout,
        Action<int>? processStarted,
        CancellationToken ct) =>
        RunAsync(assembly, task, timeout, processStarted, ProbeRunnerPath(), ct);

    /// <summary>As above, with the ProbeRunner path supplied (for tests that reach it through a symlink).</summary>
    internal static async Task<ProbeResult> RunAsync(
        byte[] assembly,
        HarnessTask task,
        TimeSpan timeout,
        Action<int>? processStarted,
        string? runner,
        CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(assembly);
        ArgumentNullException.ThrowIfNull(task);
        ct.ThrowIfCancellationRequested();

        // TimeSpan has no NaN or +infinity; InfiniteTimeSpan is -1 ms. So "positive and at most MaxTimeout" is the whole bound.
        if (timeout <= TimeSpan.Zero || timeout > MaxTimeout)
        {
            return Crashed($"invalid-timeout: {timeout} (must be > 0 and <= {MaxTimeout})");
        }

        if (!OperatingSystem.IsMacOS() || !File.Exists(SandboxExec))
        {
            return Crashed("no-sandbox");
        }

        if (runner is null || !File.Exists(runner))
        {
            return Crashed("probe-runner-missing");
        }

        var dotnetRoot = RealPath(ReferenceAssemblies.DotnetRoot());
        var dotnet = Path.Combine(dotnetRoot, "dotnet");
        if (!File.Exists(dotnet))
        {
            return Crashed("dotnet-missing");
        }

        var directory = RealPath(Directory.CreateTempSubdirectory("probe-").FullName);
        try
        {
            var assemblyPath = Path.Combine(directory, AssemblyFileName);
            await File.WriteAllBytesAsync(assemblyPath, assembly, ct).ConfigureAwait(false);
            var startInfo = CreateStartInfo(directory, RealPath(runner), dotnetRoot, dotnet, assemblyPath, task);
            return await RunProcessAsync(startInfo, timeout, processStarted, ct).ConfigureAwait(false);
        }
        finally
        {
            TryDelete(directory);
        }
    }

    private static ProcessStartInfo CreateStartInfo(
        string directory,
        string runner,
        string dotnetRoot,
        string dotnet,
        string assemblyPath,
        HarnessTask task)
    {
        var startInfo = new ProcessStartInfo(SandboxExec)
        {
            WorkingDirectory = directory,
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
        };

        foreach (var (name, value) in new[]
        {
            ("PROBE_DIR", directory),
            ("RUNNER_DIR", Path.GetDirectoryName(runner)!),
            ("DOTNET_ROOT", dotnetRoot),
            ("DOTNET", dotnet),
        })
        {
            startInfo.ArgumentList.Add("-D");
            startInfo.ArgumentList.Add(name + "=" + value);
        }

        startInfo.ArgumentList.Add("-p");
        startInfo.ArgumentList.Add(SandboxProfile);
        startInfo.ArgumentList.Add(dotnet);
        startInfo.ArgumentList.Add(runner);
        startInfo.ArgumentList.Add(assemblyPath);
        startInfo.ArgumentList.Add(task.EntryType);
        startInfo.ArgumentList.Add(task.EntryMethod);

        // Nothing from the harness's environment (fnox-injected API keys included) is inherited.
        startInfo.Environment.Clear();
        startInfo.Environment["PATH"] = dotnetRoot + ":" + FallbackPath;
        startInfo.Environment["HOME"] = directory;

        // Path.GetTempPath() falls back to /tmp, which the profile makes unwritable; keep temp files inside the probe dir
        // so ordinary code that uses temp files is not turned into "threw" by the sandbox.
        startInfo.Environment["TMPDIR"] = directory + "/";
        startInfo.Environment["DOTNET_ROOT"] = dotnetRoot;

        // No diagnostic IPC socket, debugger or profiler attach surface in the child.
        startInfo.Environment["DOTNET_EnableDiagnostics"] = "0";

        // Cap the managed heap so a runaway allocation fails with OutOfMemoryException instead of swapping the host.
        startInfo.Environment["DOTNET_GCHeapHardLimit"] = GcHeapHardLimit;
        return startInfo;
    }

    private static async Task<ProbeResult> RunProcessAsync(
        ProcessStartInfo startInfo,
        TimeSpan timeout,
        Action<int>? processStarted,
        CancellationToken ct)
    {
        using var process = new Process { StartInfo = startInfo };
        var stopwatch = Stopwatch.StartNew();
        try
        {
            process.Start();
        }
        catch (Exception ex) when (ex is Win32Exception or InvalidOperationException)
        {
            return Crashed("start-failed: " + ex.GetType().FullName);
        }

        processStarted?.Invoke(process.Id);
        process.StandardInput.Close();
        var stdout = ReadTailAsync(process.StandardOutput.BaseStream, StdoutTailBytes);
        var stderr = ReadTailAsync(process.StandardError.BaseStream, 0);

        using var timeoutSource = new CancellationTokenSource(timeout);
        using var linked = CancellationTokenSource.CreateLinkedTokenSource(ct, timeoutSource.Token);
        try
        {
            await process.WaitForExitAsync(linked.Token).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            var elapsed = stopwatch.ElapsedMilliseconds;
            Kill(process);
            await WithinGrace(process.WaitForExitAsync(CancellationToken.None)).ConfigureAwait(false);
            ct.ThrowIfCancellationRequested();
            return new ProbeResult(ProbeOutcome.Timeout, null, null, elapsed);
        }

        var duration = stopwatch.ElapsedMilliseconds;
        if (!await WithinGrace(Task.WhenAll(stdout, stderr)).ConfigureAwait(false))
        {
            // Only possible if something outside the killed tree still holds the pipe.
            Kill(process);
            return new ProbeResult(ProbeOutcome.Crashed, null, "stdout-held-open", duration);
        }

        return Parse(await stdout.ConfigureAwait(false), process.ExitCode, duration);
    }

    private static ProbeResult Parse(byte[] stdout, int exitCode, long duration)
    {
        var lines = Encoding.UTF8.GetString(stdout).Split('\n');
        foreach (var raw in lines.Reverse())
        {
            var line = raw.Trim();
            if (!line.StartsWith('{'))
            {
                continue;
            }

            ProbeRunnerLine? parsed;
            try
            {
                parsed = JsonSerializer.Deserialize(line, ProbeJson.Default.ProbeRunnerLine);
            }
            catch (JsonException)
            {
                continue;
            }

            ProbeOutcome? outcome = parsed?.Outcome switch
            {
                "returned" => ProbeOutcome.Returned,
                "threw" => ProbeOutcome.Threw,
                "crashed" => ProbeOutcome.Crashed,
                _ => null,
            };
            if (outcome is { } value)
            {
                return new ProbeResult(value, Truncate(parsed!.ExceptionType), Truncate(parsed.Reason), duration);
            }
        }

        return new ProbeResult(ProbeOutcome.Crashed, null, $"no-result (exit code {exitCode})", duration);
    }

    /// <summary>Reads <paramref name="stream"/> to EOF with a fixed buffer, keeping at most the last <paramref name="capacity"/> bytes.</summary>
    private static async Task<byte[]> ReadTailAsync(Stream stream, int capacity)
    {
        var buffer = ArrayPool<byte>.Shared.Rent(16 * 1024);
        var tail = new byte[capacity];
        var length = 0;
        try
        {
            int read;
            while ((read = await stream.ReadAsync(buffer).ConfigureAwait(false)) > 0)
            {
                if (capacity == 0)
                {
                    continue;
                }

                if (read >= capacity)
                {
                    Array.Copy(buffer, read - capacity, tail, 0, capacity);
                    length = capacity;
                    continue;
                }

                var overflow = Math.Max(0, length + read - capacity);
                if (overflow > 0)
                {
                    Array.Copy(tail, overflow, tail, 0, length - overflow);
                    length -= overflow;
                }

                Array.Copy(buffer, 0, tail, length, read);
                length += read;
            }
        }
        catch (Exception ex) when (ex is IOException or ObjectDisposedException)
        {
            // The pipe was torn down (process disposed or killed); keep what arrived.
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }

        return tail[..length];
    }

    private static async Task<bool> WithinGrace(Task task)
    {
        var finished = await Task.WhenAny(task, Task.Delay(KillGrace)).ConfigureAwait(false);
        return finished == task;
    }

    private static void Kill(Process process)
    {
        try
        {
            process.Kill(entireProcessTree: true);
        }
        catch (Exception ex) when (ex is InvalidOperationException or Win32Exception or NotSupportedException)
        {
            // Already exited, or exiting while we raced it.
        }
    }

    private static string? Truncate(string? value) =>
        value is { Length: > MaxFieldLength } ? value[..MaxFieldLength] : value;

    private static ProbeResult Crashed(string reason) => new(ProbeOutcome.Crashed, null, reason, 0);

    internal static string? ProbeRunnerPath() =>
        typeof(ProbeExecutor).Assembly
            .GetCustomAttributes<AssemblyMetadataAttribute>()
            .FirstOrDefault(a => a.Key == ProbeRunnerPathKey)?.Value;

    /// <summary>Resolves every symlinked component (e.g. <c>/var</c> → <c>/private/var</c>); Seatbelt matches real paths only.</summary>
    private static string RealPath(string path)
    {
        var current = Path.GetPathRoot(Path.GetFullPath(path)) ?? "/";
        foreach (var part in Path.GetFullPath(path).Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries))
        {
            var next = Path.Combine(current, part);
            var target = new FileInfo(next).ResolveLinkTarget(returnFinalTarget: true);
            current = target?.FullName ?? next;
        }

        return current;
    }

    private static void TryDelete(string directory)
    {
        try
        {
            Directory.Delete(directory, recursive: true);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            // Best effort: the OS cleans the temp area eventually.
        }
    }
}
