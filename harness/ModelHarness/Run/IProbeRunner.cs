namespace ModelHarness.Run;

using System.Threading;
using System.Threading.Tasks;

using ModelHarness.Probe;
using ModelHarness.Tasks;

/// <summary>The runner's view of the behavioural probe; consumer-defined so tests can stub it.</summary>
public interface IProbeRunner
{
    Task<ProbeResult> RunAsync(byte[] assembly, HarnessTask task, CancellationToken ct);
}
