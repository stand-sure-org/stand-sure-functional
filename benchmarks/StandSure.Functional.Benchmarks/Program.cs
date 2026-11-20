using BenchmarkDotNet.Running;

namespace StandSure.Functional.Benchmarks;

// BenchmarkDotNet 0.15.8 has no net11.0 toolchain (its default out-of-process run throws NotImplementedException:
// GetRuntimeVersion not implemented for NotRecognized), so run in-process:
//   dotnet run -c Release --project benchmarks/StandSure.Functional.Benchmarks -- --filter '*' --job short --inProcess
internal static class Program
{
    private static void Main(string[] args) => BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
}
