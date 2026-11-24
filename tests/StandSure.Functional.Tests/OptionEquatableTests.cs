using System;
using System.Collections.Generic;

using AwesomeAssertions;

using Xunit;

namespace StandSure.Functional.Tests;

/// <summary>Review finding 10: <see cref="Option{T}"/> implements <see cref="IEquatable{T}"/> with value semantics.</summary>
public sealed class OptionEquatableTests
{
    [Fact]
    public void Option_ImplementsIEquatable()
    {
        typeof(IEquatable<Option<int>>).IsAssignableFrom(typeof(Option<int>)).Should().BeTrue();
    }

    [Fact]
    public void Somes_WithEqualValues_AreEqual_WithEqualHashCodes()
    {
        var a = Option.Some("same");
        var b = Option.Some(new string(['s', 'a', 'm', 'e']));

        (a == b).Should().BeTrue();
        (a != b).Should().BeFalse();
        a.GetHashCode().Should().Be(b.GetHashCode());
    }

    [Fact]
    public void Somes_WithDifferentValues_AreNotEqual()
    {
        (Option.Some(1) == Option.Some(2)).Should().BeFalse();
    }

    [Fact]
    public void AllNones_AreEqual()
    {
        (Option<int>.None == default(Option<int>)).Should().BeTrue();
        (Option.None<string>() == new Option<string>((string?)null)).Should().BeTrue();
        Option<int>.None.GetHashCode().Should().Be(default(Option<int>).GetHashCode());
    }

    [Fact]
    public void SomeZero_IsNotNone()
    {
        (Option.Some(0) == Option<int>.None).Should().BeFalse();
    }

    [Fact]
    public void EqualsObject_MatchesTypedEquals()
    {
        object boxed = Option.Some(1);

        Option.Some(1).Equals(boxed).Should().BeTrue();
        Option.Some(1).Equals((object?)null).Should().BeFalse();
    }

    [Fact]
    public void WorksAsADictionaryKey()
    {
        var seen = new HashSet<Option<int>> { Option.Some(1), Option<int>.None };

        seen.Contains(Option.Some(1)).Should().BeTrue();
        seen.Contains(default).Should().BeTrue();
        seen.Contains(Option.Some(2)).Should().BeFalse();
    }
}
