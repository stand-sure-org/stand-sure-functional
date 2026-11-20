using System;

using BenchmarkDotNet.Attributes;

namespace StandSure.Functional.Benchmarks;

[MemoryDiagnoser]
public class OptionBenchmarks
{
    // Mutable, non-constant inputs so the JIT cannot fold the measured work away.
    private int _number = 42;
    private Option<int> _someInt;
    private Option<int> _noneInt;

    [GlobalSetup]
    public void Setup()
    {
        this._number = Environment.TickCount | 1;
        this._someInt = Option.Some(this._number);
        this._noneInt = Option.None<int>();
    }

    [Benchmark]
    public Option<int> CreateSome() => Option.Some(this._number);

    [Benchmark]
    public Option<int> CreateNone() => Option.None<int>();

    [Benchmark]
    public int SwitchSome() => Describe(this._someInt);

    [Benchmark]
    public int SwitchNone() => Describe(this._noneInt);

    // Option<T> has no Deconstruct; TryGetValue is its destructuring equivalent.
    [Benchmark]
    public int TryGetValueSome() => this._someInt.TryGetValue(out int value) ? value : -1;

    [Benchmark]
    public int TryGetValueNone() => this._noneInt.TryGetValue(out int value) ? value : -1;

    private static int Describe(Option<int> option) => option switch
    {
        None => 0,
        int value => value,
    };
}
