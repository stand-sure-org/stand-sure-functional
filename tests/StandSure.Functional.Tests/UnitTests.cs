using AwesomeAssertions;

using Xunit;

namespace StandSure.Functional.Tests;

public sealed class UnitTests
{
    [Fact]
    public void Value_EqualsDefault()
    {
        Unit.Value.Should().Be(default(Unit));
    }

    [Fact]
    public void ToString_IsEmptyTuple()
    {
        Unit.Value.ToString().Should().Be("()");
    }

    [Fact]
    public void CompareTo_IsAlwaysZero()
    {
        Unit.Value.CompareTo(default).Should().Be(0);
    }

    [Fact]
    public void GetHashCode_IsZero()
    {
        Unit.Value.GetHashCode().Should().Be(0);
    }

    [Fact]
    public void Operators_AreConsistent()
    {
        (Unit.Value == default).Should().BeTrue();
        (Unit.Value != default).Should().BeFalse();
    }
}
