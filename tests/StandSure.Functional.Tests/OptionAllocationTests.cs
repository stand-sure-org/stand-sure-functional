using System;
using System.Runtime.CompilerServices;
using AwesomeAssertions;
using Xunit;

namespace StandSure.Functional.Tests;

public sealed class OptionAllocationTests
{
    private const int Iterations = 1_000;

    [Fact]
    public void CreateAndMatch_ValueType_AllocatesNothing()
    {
        // Warm up: JIT (and tiering) of everything on the measured path happens here, not below.
        for (var i = 0; i < 3; i++)
        {
            _ = CreateAndMatch(Iterations);
        }

        var before = GC.GetAllocatedBytesForCurrentThread();
        var sum = CreateAndMatch(Iterations);
        var allocated = GC.GetAllocatedBytesForCurrentThread() - before;

        allocated.Should().Be(0);
        sum.Should().BeGreaterThan(0);
    }

    [Fact]
    public void Combinators_SomePath_ValueType_AllocatesNothing()
    {
        for (var i = 0; i < 3; i++)
        {
            _ = Combine(Iterations);
        }

        var before = GC.GetAllocatedBytesForCurrentThread();
        var sum = Combine(Iterations);
        var allocated = GC.GetAllocatedBytesForCurrentThread() - before;

        allocated.Should().Be(0);
        sum.Should().BeGreaterThan(0);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static long Combine(int iterations)
    {
        long sum = 0;
        for (var i = 1; i <= iterations; i++)
        {
            var o = Option.Some(i)
                .Map(static v => v + 1)
                .Bind(static v => Option.Some(v * 2))
                .Filter(static v => v > 0);
            sum += o.Match(static () => 0, static v => v) + o.GetValueOrDefault(0);
            sum += o.ToResult(Error.Uninitialized).GetValueOrDefault(0);
            sum += o.ToResult(static () => Error.Uninitialized).GetValueOrDefault(0);
        }

        return sum;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static long CreateAndMatch(int iterations)
    {
        long sum = 0;
        for (var i = 0; i < iterations; i++)
        {
            Option<int> o = (i & 1) == 0 ? i : global::StandSure.Functional.None.Instance;
            sum += o switch
            {
                None => 1,
                int value => value,
            };
        }

        return sum;
    }
}
