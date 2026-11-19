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
        _number = Environment.TickCount | 1;
        _text = "value" + _number.ToString(CultureInfo.InvariantCulture);
        _error = Failure;
        _okInt = Result.Ok(_number);
        _failedInt = Result.Fail<int>(_error);
        _okString = Result.Ok(_text);
        _failedString = Result.Fail<string>(_error);
    }

    [Benchmark]
    public Result<int> CreateInt() => Result.Ok(_number);

    [Benchmark]
    public Result<int> CreateIntFailure() => Result.Fail<int>(_error);

    [Benchmark]
    public int SwitchIntOk() => Describe(_okInt);

    [Benchmark]
    public int SwitchIntFailure() => Describe(_failedInt);

    [Benchmark]
    public int DeconstructIntOk()
    {
        var (value, error) = _okInt;
        return error is null ? value : -1;
    }

    [Benchmark]
    public int DeconstructIntFailure()
    {
        var (value, error) = _failedInt;
        return error is null ? value : -1;
    }

    [Benchmark]
    public Result<string> CreateString() => Result.Ok(_text);

    [Benchmark]
    public int SwitchStringOk() => Describe(_okString);

    [Benchmark]
    public int SwitchStringFailure() => Describe(_failedString);

    [Benchmark]
    public int DeconstructStringOk()
    {
        var (value, error) = _okString;
        return error is null ? value!.Length : -1;
    }

    [Benchmark]
    public int DeconstructStringFailure()
    {
        var (value, error) = _failedString;
        return error is null ? value!.Length : -1;
    }

    private static int Describe(Result<int> result) => result switch
    {
        Error e => e.Code.Length,
        int value => value,
    };

    private static int Describe(Result<string> result) => result switch
    {
        Error e => e.Code.Length,
        string value => value.Length,
    };
}
