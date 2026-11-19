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
