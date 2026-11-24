using System;
using System.Collections.Generic;
using System.Linq;

using AwesomeAssertions;

using Xunit;

namespace StandSure.Functional.Tests;

public sealed class CollectionHelperTests
{
    private static readonly Error E1 = Error.NotFound("t.one", "one");
    private static readonly Error E2 = Error.NotFound("t.two", "two");

    private static Result<int> Ok(int v) => Result.Success(v);

    private static Result<int> Bad(Error e) => Result.Failure<int>(e);

    [Fact]
    public void Combine_AllOk()
    {
        var r = new[] { Ok(1), Ok(2), Ok(3) }.Combine();

        r.TryGetValue(out IReadOnlyList<int>? values).Should().BeTrue();
        values.Should().Equal(1, 2, 3);
    }

    [Fact]
    public void Combine_CollectsEveryFailure()
    {
        var r = new[] { Ok(1), Bad(E1), Ok(3), Bad(E2) }.Combine();

        r.TryGetValue(out Error? error).Should().BeTrue();
        error!.Kind.Should().Be(ErrorKind.NotFound, "both children are NotFound");
        error.Code.Should().Be("result.combine_failed");
        error.Message.Should().Be("2 of 4 failed");
        error.Errors.Should().Equal(E1, E2);
    }

    [Fact]
    public void Combine_Empty_IsOkEmpty()
    {
        var r = Array.Empty<Result<int>>().Combine();

        r.TryGetValue(out IReadOnlyList<int>? values).Should().BeTrue();
        values.Should().BeEmpty();
    }

    [Fact]
    public void Combine_DefaultElement_CountsAsUninitializedFailure()
    {
        var r = new[] { Ok(1), default }.Combine();

        r.TryGetValue(out Error? error).Should().BeTrue();
        error!.Message.Should().Be("1 of 2 failed");
        error.Errors.Should().ContainSingle().Which.Should().BeSameAs(Error.Uninitialized);
    }

    [Fact]
    public void Combine_NullSource_IsNullSourceFailure()
    {
        IEnumerable<Result<int>> source = null!;

        source.Combine().TryGetValue(out Error? error).Should().BeTrue();
        error!.Code.Should().Be("result.null_source");
    }

    [Fact]
    public void Traverse_AllOk()
    {
        var r = new[] { 1, 2, 3 }.Traverse(x => Ok(x * 2));

        r.TryGetValue(out IReadOnlyList<int>? values).Should().BeTrue();
        values.Should().Equal(2, 4, 6);
    }

    [Fact]
    public void Traverse_CollectsEveryFailure()
    {
        var calls = 0;

        var r = new[] { 1, 2, 3, 4 }.Traverse(x =>
        {
            calls++;
            return x % 2 == 0 ? Bad(x == 2 ? E1 : E2) : Ok(x);
        });

        calls.Should().Be(4);
        r.TryGetValue(out Error? error).Should().BeTrue();
        error!.Code.Should().Be("result.combine_failed");
        error.Message.Should().Be("2 of 4 failed");
        error.Errors.Should().Equal(E1, E2);
    }

    [Fact]
    public void Traverse_Empty_IsOkEmpty()
    {
        var r = Array.Empty<int>().Traverse(x => Ok(x));

        r.TryGetValue(out IReadOnlyList<int>? values).Should().BeTrue();
        values.Should().BeEmpty();
    }

    [Fact]
    public void Traverse_NullMapper_IsNullDelegateFailure()
    {
        var r = new[] { 1 }.Traverse<int, int>(null!);

        r.TryGetValue(out Error? error).Should().BeTrue();
        error!.Code.Should().Be("result.null_delegate");
    }

    [Fact]
    public void Traverse_NullSource_IsNullSourceFailure()
    {
        IEnumerable<int> source = null!;

        source.Traverse(x => Ok(x)).TryGetValue(out Error? error).Should().BeTrue();
        error!.Code.Should().Be("result.null_source");
    }

    [Fact]
    public void Traverse_ThrowingMapper_Propagates()
    {
        var act = () => new[] { 1 }.Traverse<int, int>(_ => throw new InvalidOperationException("boom"));

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Partition_PreservesOrder()
    {
        var (successes, failures) = new[] { Ok(1), Bad(E1), Ok(2), Bad(E2), Ok(3) }.Partition();

        successes.Should().Equal(1, 2, 3);
        failures.Should().Equal(E1, E2);
    }

    [Fact]
    public void Partition_Empty_IsEmptyBoth()
    {
        var (successes, failures) = Array.Empty<Result<int>>().Partition();

        successes.Should().BeEmpty();
        failures.Should().BeEmpty();
    }

    [Fact]
    public void Partition_NullSource_IsEmptyBoth()
    {
        IEnumerable<Result<int>> source = null!;

        var (successes, failures) = source.Partition();

        successes.Should().BeEmpty();
        failures.Should().BeEmpty();
    }

    [Fact]
    public void Choose_SkipsNone()
    {
        var source = new[] { Option.Some(1), Option<int>.None, Option.Some(3), default };

        source.Choose().Should().Equal(1, 3);
    }

    [Fact]
    public void Choose_IsLazy()
    {
        var counting = new CountingSource<Option<int>>([Option.Some(1), Option.Some(2)]);

        var chosen = counting.Choose();

        counting.EnumeratorCount.Should().Be(0);
        chosen.First().Should().Be(1);
        counting.MoveNextCount.Should().Be(1);
    }

    [Fact]
    public void Choose_NullSource_IsEmpty()
    {
        IEnumerable<Option<int>> source = null!;

        source.Choose().Should().BeEmpty();
    }

    [Fact]
    public void Pairwise_Empty_IsEmpty()
    {
        Array.Empty<int>().Pairwise().Should().BeEmpty();
    }

    [Fact]
    public void Pairwise_Single_IsEmpty()
    {
        new[] { 1 }.Pairwise().Should().BeEmpty();
    }

    [Fact]
    public void Pairwise_Normal_YieldsAdjacentPairs()
    {
        new[] { 1, 2, 3, 4 }.Pairwise().Should().Equal((1, 2), (2, 3), (3, 4));
    }

    [Fact]
    public void Pairwise_IsLazyAndSingleEnumeration()
    {
        var counting = new CountingSource<int>([1, 2, 3]);

        var pairs = counting.Pairwise();

        counting.EnumeratorCount.Should().Be(0);
        pairs.ToList().Should().HaveCount(2);
        counting.EnumeratorCount.Should().Be(1);
        counting.MoveNextCount.Should().Be(3);
    }

    [Fact]
    public void Pairwise_NullSource_IsEmpty()
    {
        IEnumerable<int> source = null!;

        source.Pairwise().Should().BeEmpty();
    }

    [Fact]
    public void Scan_Empty_IsEmpty()
    {
        Array.Empty<int>().Scan(0, (a, x) => a + x).Should().BeEmpty();
    }

    [Fact]
    public void Scan_Single_YieldsOneRunningValue()
    {
        new[] { 5 }.Scan(10, (a, x) => a + x).Should().Equal(15);
    }

    [Fact]
    public void Scan_Normal_YieldsRunningTotalsWithoutSeed()
    {
        new[] { 1, 2, 3, 4 }.Scan(0, (a, x) => a + x).Should().Equal(1, 3, 6, 10);
    }

    [Fact]
    public void Scan_CanChangeAccumulatorType()
    {
        new[] { 1, 2 }.Scan("", (a, x) => a + x).Should().Equal("1", "12");
    }

    [Fact]
    public void Scan_IsLazy()
    {
        var counting = new CountingSource<int>([1, 2, 3]);

        var running = counting.Scan(0, (a, x) => a + x);

        counting.EnumeratorCount.Should().Be(0);
        running.First().Should().Be(1);
        counting.MoveNextCount.Should().Be(1);
    }

    [Fact]
    public void Scan_NullSourceOrAccumulator_IsEmpty()
    {
        IEnumerable<int> source = null!;

        source.Scan(0, (a, x) => a + x).Should().BeEmpty();
        new[] { 1 }.Scan(0, null!).Should().BeEmpty();
    }
}
