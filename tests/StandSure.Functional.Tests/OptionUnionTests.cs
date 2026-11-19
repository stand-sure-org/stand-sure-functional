using AwesomeAssertions;
using Xunit;

namespace StandSure.Functional.Tests;

public sealed class OptionUnionTests
{
    private static string Describe(Option<int> o) => o switch
    {
        None => "none",
        int i => i.ToString(System.Globalization.CultureInfo.InvariantCulture),
    };

    [Fact]
    public void ImplicitFromValue_IsSome()
    {
        Option<int> o = 42;

        o.IsSome.Should().BeTrue();
        o.IsNone.Should().BeFalse();
        o.TryGetValue(out int value).Should().BeTrue();
        value.Should().Be(42);
        o.TryGetValue(out global::StandSure.Functional.None? none).Should().BeFalse();
        none.Should().BeNull();
        o.Value.Should().Be(42);
        Describe(o).Should().Be("42");
    }

    [Fact]
    public void ImplicitFromNull_IsNone()
    {
        Option<string?> o = (string?)null;

        o.IsNone.Should().BeTrue();
        o.IsSome.Should().BeFalse();
        o.TryGetValue(out string? value).Should().BeFalse();
        value.Should().BeNull();
        o.Value.Should().BeSameAs(global::StandSure.Functional.None.Instance);
    }

    [Fact]
    public void ImplicitFromNone_IsNone()
    {
        Option<int> o = global::StandSure.Functional.None.Instance;

        o.IsNone.Should().BeTrue();
        Describe(o).Should().Be("none");
        Option<int>.None.IsNone.Should().BeTrue();
    }

    [Fact]
    public void Default_IsNone()
    {
        var o = default(Option<int>);
        var array = new Option<int>[3];

        o.IsNone.Should().BeTrue();
        o.IsSome.Should().BeFalse();
        o.HasValue.Should().BeTrue();
        o.TryGetValue(out int value).Should().BeFalse();
        value.Should().Be(0);
        o.TryGetValue(out global::StandSure.Functional.None? none).Should().BeTrue();
        none.Should().BeSameAs(global::StandSure.Functional.None.Instance);
        o.Value.Should().BeSameAs(global::StandSure.Functional.None.Instance);
        Describe(o).Should().Be("none");
        array[1].IsNone.Should().BeTrue();
        Describe(array[1]).Should().Be("none");
    }

    [Fact]
    public void Switch_IsExhaustiveWithoutDiscard()
    {
        Describe(Option.Some(7)).Should().Be("7");
        Describe(Option.None<int>()).Should().Be("none");
    }

    [Fact]
    public void OptionOfNullableValueType_NullIsNone()
    {
        Option<int?> o = (int?)null;
        Option<int?> some = (int?)5;

        o.IsNone.Should().BeTrue();
        some.IsSome.Should().BeTrue();
        some.TryGetValue(out int? v).Should().BeTrue();
        v.Should().Be(5);
    }

    [Fact]
    public void Factories_CreateExpectedCases()
    {
        Option.Some("x").IsSome.Should().BeTrue();
        Option.Some<string>(null!).IsNone.Should().BeTrue();
        Option.None<string>().IsNone.Should().BeTrue();
    }

    [Fact]
    public void None_IsSingleton()
    {
        global::StandSure.Functional.None.Instance.Should().NotBeNull();
        global::StandSure.Functional.None.Instance.Should().BeSameAs(global::StandSure.Functional.None.Instance);
    }
}
