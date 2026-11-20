using System;
using System.Globalization;

using BenchmarkDotNet.Attributes;

namespace StandSure.Functional.Benchmarks;

[MemoryDiagnoser]
public class ResultBenchmarks
{
    private static readonly Error Failure = Error.Unexpected("boom", "Boom.");

    // Mutable, non-constant inputs so the JIT cannot fold the measured work away.
    private int _number = 42;
    private string _text = "value";
    private Error _error = Failure;
    private Result<int> _okInt;
    private Result<int> _failedInt;
    private Result<string> _okString;
    private Result<string> _failedString;

    [GlobalSetup]
    public void Setup()
    {
        this._number = Environment.TickCount | 1;
        this._text = "value" + this._number.ToString(CultureInfo.InvariantCulture);
        this._error = Failure;
        this._okInt = Result.Success(this._number);
        this._failedInt = Result.Failure<int>(this._error);
        this._okString = Result.Success(this._text);
        this._failedString = Result.Failure<string>(this._error);
    }

    [Benchmark]
    public Result<int> CreateInt() => Result.Success(this._number);

    [Benchmark]
    public Result<int> CreateIntFailure() => Result.Failure<int>(this._error);

    [Benchmark]
    public int SwitchIntOk() => Describe(this._okInt);

    [Benchmark]
    public int SwitchIntFailure() => Describe(this._failedInt);

    [Benchmark]
    public int DeconstructIntOk()
    {
        var (value, error) = this._okInt;
        return error is null ? value : -1;
    }

    [Benchmark]
    public int DeconstructIntFailure()
    {
        var (value, error) = this._failedInt;
        return error is null ? value : -1;
    }

    [Benchmark]
    public Result<string> CreateString() => Result.Success(this._text);

    [Benchmark]
    public int SwitchStringOk() => Describe(this._okString);

    [Benchmark]
    public int SwitchStringFailure() => Describe(this._failedString);

    [Benchmark]
    public int DeconstructStringOk()
    {
        var (value, error) = this._okString;
        return error is null ? value!.Length : -1;
    }

    [Benchmark]
    public int DeconstructStringFailure()
    {
        var (value, error) = this._failedString;
        return error is null ? value!.Length : -1;
    }

    private static int Describe(Result<int> result) => result switch
    {
        Error e => e.Code.Length,
        int value => value,
        null => 0,
    };

    private static int Describe(Result<string> result) => result switch
    {
        Error e => e.Code.Length,
        string value => value.Length,
        null => 0,
    };
}
