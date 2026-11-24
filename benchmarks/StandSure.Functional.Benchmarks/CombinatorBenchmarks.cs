using System;
using System.Diagnostics;

using BenchmarkDotNet.Attributes;

namespace StandSure.Functional.Benchmarks;

/// <summary>
/// Cost of the combinators that run caller code, which are the methods that start an Activity span.
/// <see cref="Listening"/> false is the production default (no listener: StartActivity returns null);
/// true attaches a listener that records every span, the worst case.
/// </summary>
[MemoryDiagnoser]
public class CombinatorBenchmarks
{
    private static readonly Error Failure = Error.Unexpected("boom", "Boom.");
    private static readonly Func<int, int> Double = static x => x * 2;
    private static readonly Func<int, Result<int>> Half = static x => Result.Success(x / 2);
    private static readonly Func<int, bool> Positive = static x => x > 0;
    private static readonly Func<Error, int> OnFailure = static _ => -1;
    private static readonly Func<int, int> OnSuccess = static x => x + 1;

    private ActivityListener? _listener;
    private Result<int> _ok;
    private Result<int> _failed;

    [Params(false, true)]
    public bool Listening { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        this._ok = Result.Success(Environment.TickCount | 1);
        this._failed = Result.Failure<int>(Failure);

        if (!this.Listening)
        {
            return;
        }

        this._listener = new ActivityListener
        {
            ShouldListenTo = static source => source.Name == "StandSure.Functional",
            Sample = static (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded,
        };
        ActivitySource.AddActivityListener(this._listener);
    }

    [GlobalCleanup]
    public void Cleanup() => this._listener?.Dispose();

    [Benchmark]
    public Result<int> MapOk() => this._ok.Map(Double);

    [Benchmark]
    public Result<int> MapFailed() => this._failed.Map(Double);

    [Benchmark]
    public Result<int> BindOk() => this._ok.Bind(Half);

    [Benchmark]
    public int MatchOk() => this._ok.Match(OnFailure, OnSuccess);

    [Benchmark]
    public Result<int> EnsureOk() => this._ok.Ensure(Positive, Failure);

    [Benchmark]
    public int ChainOk() => this._ok.Map(Double).Bind(Half).Ensure(Positive, Failure).Match(OnFailure, OnSuccess);
}
