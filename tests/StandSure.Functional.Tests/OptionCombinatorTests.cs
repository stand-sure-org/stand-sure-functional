using System;
using AwesomeAssertions;
using Xunit;

namespace StandSure.Functional.Tests;

public sealed class OptionCombinatorTests
{
    private static readonly Error Boom = Error.NotFound("opt.thing", "missing");

    private static Option<int> Some(int v) => Option.Some(v);

    private static Option<int> NoneInt => Option<int>.None;

    [Fact]
    public void Match_Some_RunsOnSome()
    {
        Some(2).Match(onNone: () => -1, onSome: v => v * 10).Should().Be(20);
    }

    [Fact]
    public void Match_None_RunsOnNone()
    {
        NoneInt.Match(onNone: () => -1, onSome: v => v).Should().Be(-1);
    }

    [Fact]
    public void Map_Some_Transforms()
    {
        Some(2).Map(v => v + 1).Should().Be(Some(3));
    }

    [Fact]
    public void Map_None_StaysNoneWithoutCalling()
    {
        var called = false;

        var r = NoneInt.Map(v => { called = true; return v; });

        r.IsNone.Should().BeTrue();
        called.Should().BeFalse();
    }

    [Fact]
    public void Map_ReturningNull_IsNone()
    {
        Option.Some("a").Map<string>(_ => null!).IsNone.Should().BeTrue();
    }

    [Fact]
    public void Bind_Some_ChainsResult()
    {
        Some(2).Bind(v => Option.Some(v * 3)).Should().Be(Some(6));
        Some(2).Bind(_ => Option<int>.None).IsNone.Should().BeTrue();
    }

    [Fact]
    public void Bind_None_StaysNoneWithoutCalling()
    {
        var called = false;

        var r = NoneInt.Bind(v => { called = true; return Option.Some(v); });

        r.IsNone.Should().BeTrue();
        called.Should().BeFalse();
    }

    [Fact]
    public void Filter_Some_KeepsWhenPredicateHolds()
    {
        Some(4).Filter(v => v > 3).Should().Be(Some(4));
    }

    [Fact]
    public void Filter_Some_DropsWhenPredicateFails()
    {
        Some(4).Filter(v => v > 5).IsNone.Should().BeTrue();
    }

    [Fact]
    public void Filter_None_StaysNoneWithoutCalling()
    {
        var called = false;

        var r = NoneInt.Filter(_ => { called = true; return true; });

        r.IsNone.Should().BeTrue();
        called.Should().BeFalse();
    }

    [Fact]
    public void GetValueOrDefault_Some_ReturnsValue()
    {
        Some(7).GetValueOrDefault(-1).Should().Be(7);
    }

    [Fact]
    public void GetValueOrDefault_None_ReturnsFallback()
    {
        NoneInt.GetValueOrDefault(-1).Should().Be(-1);
    }

    [Fact]
    public void ToResult_Some_IsSuccess()
    {
        var r = Some(5).ToResult(Boom);

        r.IsSuccess.Should().BeTrue();
        r.GetValueOrDefault(0).Should().Be(5);
    }

    [Fact]
    public void ToResult_None_IsGivenError()
    {
        var r = NoneInt.ToResult(Boom);

        r.IsFailure.Should().BeTrue();
        r.Match(e => e, _ => null!).Should().BeSameAs(Boom);
    }

    [Fact]
    public void ToResult_Factory_Some_DoesNotCallFactory()
    {
        var called = false;

        var r = Some(5).ToResult(() => { called = true; return Boom; });

        r.TryGetValue(out int v).Should().BeTrue();
        v.Should().Be(5);
        called.Should().BeFalse();
    }

    [Fact]
    public void ToResult_Factory_NoneAndDefault_UseFactoryError()
    {
        NoneInt.ToResult(() => Boom).Match(onFailure: e => e, onSuccess: _ => null!).Should().BeSameAs(Boom);
        default(Option<int>).ToResult(() => Boom).IsFailure.Should().BeTrue();
    }

    [Fact]
    public void ToResult_Factory_NullFactory_OnlyMattersForNone()
    {
        Some(1).ToResult((Func<Error>)null!).IsSuccess.Should().BeTrue();
        NoneInt.ToResult((Func<Error>)null!).Match(onFailure: e => e.Code, onSuccess: _ => "").Should().Be("result.null_delegate");
    }

    [Fact]
    public void ToResult_Factory_ReturningNull_IsUninitialized()
    {
        NoneInt.ToResult(() => null!).Match(onFailure: e => e, onSuccess: _ => null!).Should().BeSameAs(Error.Uninitialized);
    }

    [Fact]
    public void ToResult_NoneWithNullError_IsUninitialized()
    {
        NoneInt.ToResult(null!).Match(e => e, _ => null!).Should().BeSameAs(Error.Uninitialized);
    }

    [Fact]
    public void Default_BehavesAsNoneInEveryMember()
    {
        var d = default(Option<int>);

        d.Match(() => -1, v => v).Should().Be(-1);
        d.Map(v => v).IsNone.Should().BeTrue();
        d.Bind(v => Option.Some(v)).IsNone.Should().BeTrue();
        d.Filter(_ => true).IsNone.Should().BeTrue();
        d.GetValueOrDefault(9).Should().Be(9);
        d.ToResult(Boom).IsFailure.Should().BeTrue();
    }

    [Fact]
    public void NullDelegates_NeverThrow()
    {
        Some(1).Map<int>(null!).IsNone.Should().BeTrue();
        Some(1).Bind<int>(null!).IsNone.Should().BeTrue();
        Some(1).Filter(null!).IsNone.Should().BeTrue();
        Some(1).Match<int>(() => 5, null!).Should().Be(0);
        NoneInt.Match<int>(null!, v => v).Should().Be(0);
        NoneInt.Map<int>(null!).IsNone.Should().BeTrue();
        Some(1).Match<int>(null!, v => v).Should().Be(1);
    }

    [Fact]
    public void ThrowingDelegate_Propagates()
    {
        var act = () => Some(1).Map<int>(_ => throw new InvalidOperationException("x"));

        act.Should().Throw<InvalidOperationException>();
    }
}
