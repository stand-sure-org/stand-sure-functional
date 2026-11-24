using System;
using System.Collections.Generic;

using AwesomeAssertions;

using Xunit;

namespace StandSure.Functional.Tests;

/// <summary>Review finding 10: <see cref="Result{T}"/> implements <see cref="IEquatable{T}"/> with value semantics.</summary>
public sealed class ResultEquatableTests
{
    private static readonly Error NotFound = Error.NotFound("eq.result", "m");

    [Fact]
    public void Result_ImplementsIEquatable()
    {
        typeof(IEquatable<Result<int>>).IsAssignableFrom(typeof(Result<int>)).Should().BeTrue();
    }

    [Fact]
    public void Successes_WithEqualValues_AreEqual_WithEqualHashCodes()
    {
        var a = Result.Success("same");
        var b = Result.Success(new string(['s', 'a', 'm', 'e']));

        a.Equals(b).Should().BeTrue();
        (a == b).Should().BeTrue();
        (a != b).Should().BeFalse();
        a.GetHashCode().Should().Be(b.GetHashCode());
    }

    [Fact]
    public void Successes_WithDifferentValues_AreNotEqual()
    {
        (Result.Success(1) == Result.Success(2)).Should().BeFalse();
    }

    [Fact]
    public void Failures_WithEqualErrors_AreEqual_WithEqualHashCodes()
    {
        var a = Result.Failure<int>(Error.NotFound("eq.result", "m"));
        var b = Result.Failure<int>(Error.NotFound("eq.result", "m"));

        (a == b).Should().BeTrue();
        a.GetHashCode().Should().Be(b.GetHashCode());
    }

    [Fact]
    public void SuccessAndFailure_AreNotEqual()
    {
        (Result.Success(0) == Result.Failure<int>(NotFound)).Should().BeFalse();
    }

    [Fact]
    public void Default_EqualsAnExplicitUninitializedFailure()
    {
        var explicitFailure = Result.Failure<int>(Error.Uninitialized);

        (default(Result<int>) == explicitFailure).Should().BeTrue();
        default(Result<int>).GetHashCode().Should().Be(explicitFailure.GetHashCode());
    }

    [Fact]
    public void NullSuccessValues_AreEqual()
    {
        (Result.Success<string?>(null) == Result.Success<string?>(null)).Should().BeTrue();
        (Result.Success<string?>(null) == Result.Success<string?>("x")).Should().BeFalse();
    }

    [Fact]
    public void EqualsObject_MatchesTypedEquals()
    {
        object boxed = Result.Success(1);

        Result.Success(1).Equals(boxed).Should().BeTrue();
        Result.Success(1).Equals((object?)null).Should().BeFalse();
        Result.Success(1).Equals((object)1).Should().BeFalse("a raw value is not a result");
    }

    [Fact]
    public void WorksAsADictionaryKey()
    {
        var seen = new HashSet<Result<int>> { Result.Success(1), Result.Failure<int>(NotFound) };

        seen.Contains(Result.Success(1)).Should().BeTrue();
        seen.Contains(Result.Failure<int>(Error.NotFound("eq.result", "m"))).Should().BeTrue();
        seen.Contains(Result.Success(2)).Should().BeFalse();
    }
}
