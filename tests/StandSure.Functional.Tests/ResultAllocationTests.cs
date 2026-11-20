using System;
using System.Runtime.CompilerServices;
using AwesomeAssertions;
using Xunit;

namespace StandSure.Functional.Tests;

public sealed class ResultAllocationTests
{
    private const int Iterations = 1_000;

    private static readonly Error Failure = Error.Unexpected("boom", "Boom.");

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
            Result<int> r = (i & 1) == 0 ? i : Failure;
            sum += r switch
            {
                Error e => e.Code.Length,
                int value => value,
            };
        }

        return sum;
    }

    [Fact]
    public void Combinators_OnSuccessPath_AllocateNothing()
    {
        for (var i = 0; i < 3; i++)
        {
            _ = Combinators(Iterations);
        }

        var before = GC.GetAllocatedBytesForCurrentThread();
        var sum = Combinators(Iterations);
        var allocated = GC.GetAllocatedBytesForCurrentThread() - before;

        allocated.Should().Be(0);
        sum.Should().BeGreaterThan(0);
    }

    // Static lambdas are cached by the compiler, so any allocation measured here belongs to the combinators.
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static long Combinators(int iterations)
    {
        long sum = 0;
        for (var i = 0; i < iterations; i++)
        {
            Result<int> r = (i & 1) == 0 ? i + 1 : Failure;
            sum += r
                .Map(static v => v + 1)
                .Bind(static v => Result.Success(v * 2))
                .Ensure(static v => v > 0, Failure)
                .Ensure(static v => v > 0)
                .Ensure(static v => v > 0, static v => Failure)
                .MapError(static e => e)
                .Tap(static _ => { })
                .GetValueOrDefault(7);
            sum += r.Match(static e => 1, static v => v);
        }

        return sum;
    }
}
