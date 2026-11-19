namespace ModelHarness.Run;

using System.Threading;
using System.Threading.Tasks;

using ModelHarness.Probe;
using ModelHarness.Tasks;

/// <summary>Adapts the static <see cref="ProbeExecutor"/> to <see cref="IProbeRunner"/>.</summary>
public sealed class ProbeExecutorRunner : IProbeRunner
{
    public Task<ProbeResult> RunAsync(byte[] assembly, HarnessTask task, CancellationToken ct) =>
        ProbeExecutor.RunAsync(assembly, task, ProbeExecutor.DefaultTimeout, ct);
}
