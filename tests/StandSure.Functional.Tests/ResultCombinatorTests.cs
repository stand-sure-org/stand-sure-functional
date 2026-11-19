using System;
using AwesomeAssertions;
using Xunit;

namespace StandSure.Functional.Tests;

public sealed class ResultCombinatorTests
{
    private static readonly Error Boom = Error.NotFound("thing", "missing");

    // ---- Match ----

    [Fact]
    public void Match_Success_RunsOnSuccess()
    {
        Result.Success(2).Match(onFailure: _ => -1, onSuccess: v => v * 10).Should().Be(20);
    }

    [Fact]
    public void Match_Failure_RunsOnFailure()
    {
        Result.Failure<int>(Boom).Match(onFailure: e => e.Code.Length, onSuccess: v => v).Should().Be(5);
    }

    [Fact]
    public void Match_FailureFirstOrder()
    {
        Error? seen = null;

        Result.Failure<int>(Boom).Match(e => { seen = e; return 0; }, v => v);

        seen.Should().BeSameAs(Boom);
    }

    [Fact]
    public void Match_Default_RunsOnFailureWithUninitialized()
    {
        default(Result<int>).Match(e => e, _ => Boom).Should().BeSameAs(Error.Uninitialized);
    }

    [Fact]
    public void Match_NullOnFailure_OnFailure_ReturnsDefault()
    {
        Result.Failure<string>(Boom).Match(null!, v => v).Should().BeNull();
    }

    [Fact]
    public void Match_NullOnSuccess_OnSuccess_ReturnsDefault()
    {
        Result.Success(3).Match(_ => 7, null!).Should().Be(0);
    }

    [Fact]
    public void Match_NullDelegateForUnusedBranch_IsIgnored()
    {
        Result.Success(3).Match(null!, v => v + 1).Should().Be(4);
    }

    // ---- Map ----

    [Fact]
    public void Map_Success_Transforms()
    {
        Result.Success(2).Map(v => v.ToString()).Match(_ => "x", v => v).Should().Be("2");
    }

    [Fact]
    public void Map_Failure_PassesErrorAndSkipsLambda()
    {
        var called = false;

        var r = Result.Failure<int>(Boom).Map(v => { called = true; return v; });

        called.Should().BeFalse();
        r.TryGetValue(out Error? e).Should().BeTrue();
        e.Should().BeSameAs(Boom);
    }

    [Fact]
    public void Default_Map_StaysUninitialized()
    {
        var r = default(Result<int>).Map(v => v + 1);

        r.TryGetValue(out Error? e).Should().BeTrue();
        e.Should().BeSameAs(Error.Uninitialized);
    }

    [Fact]
    public void Map_NullDelegate_OnSuccess_ReturnsNullDelegateFailure()
    {
        var r = Result.Success(1).Map<int>(null!);

        r.TryGetValue(out Error? e).Should().BeTrue();
        e!.Code.Should().Be("result.null_delegate");
        e.Kind.Should().Be(ErrorKind.Unexpected);
    }

    [Fact]
    public void Map_NullDelegate_OnFailure_KeepsOriginalError()
    {
        var r = Result.Failure<int>(Boom).Map<int>(null!);

        r.TryGetValue(out Error? e).Should().BeTrue();
        e.Should().BeSameAs(Boom);
    }

    [Fact]
    public void Map_ThrowingLambda_IsNotCaught()
    {
        var act = () => Result.Success(1).Map<int>(_ => throw new InvalidOperationException("nope"));

        act.Should().Throw<InvalidOperationException>();
    }

    // ---- Bind ----

    [Fact]
    public void Bind_Success_ReturnsLambdaResult()
    {
        Result.Success(2).Bind(v => Result.Success(v + 1)).Match(_ => -1, v => v).Should().Be(3);
    }

    [Fact]
    public void Bind_Success_LambdaFailure_Flows()
    {
        var r = Result.Success(2).Bind(_ => Result.Failure<int>(Boom));

        r.TryGetValue(out Error? e).Should().BeTrue();
        e.Should().BeSameAs(Boom);
    }

    [Fact]
    public void Bind_Failure_PassesErrorThrough()
    {
        var r = Result.Failure<int>(Boom).Bind(v => Result.Success(v));

        r.TryGetValue(out Error? e).Should().BeTrue();
        e.Should().BeSameAs(Boom);
    }

    [Fact]
    public void Bind_ShortCircuits_SecondNotCalled()
    {
        var secondCalled = false;

        var r = Result.Success(1)
            .Bind(_ => Result.Failure<int>(Boom))
            .Bind(v => { secondCalled = true; return Result.Success(v); });

        secondCalled.Should().BeFalse();
        r.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Default_Bind_StaysUninitialized()
    {
        var called = false;

        var r = default(Result<int>).Bind(v => { called = true; return Result.Success(v); });

        called.Should().BeFalse();
        r.TryGetValue(out Error? e).Should().BeTrue();
        e.Should().BeSameAs(Error.Uninitialized);
    }

    [Fact]
    public void Bind_NullDelegate_OnSuccess_ReturnsNullDelegateFailure()
    {
        var r = Result.Success(1).Bind<int>(null!);

        r.TryGetValue(out Error? e).Should().BeTrue();
        e!.Code.Should().Be("result.null_delegate");
    }

    [Fact]
    public void Bind_DefaultResultReturnedByLambda_IsUninitializedFailure()
    {
        var r = Result.Success(1).Bind(_ => default(Result<int>));

        r.TryGetValue(out Error? e).Should().BeTrue();
        e.Should().BeSameAs(Error.Uninitialized);
    }

    // ---- MapError ----

    [Fact]
    public void MapError_Failure_TransformsError()
    {
        var other = Error.Conflict("c", "conflict");

        var r = Result.Failure<int>(Boom).MapError(_ => other);

        r.TryGetValue(out Error? e).Should().BeTrue();
        e.Should().BeSameAs(other);
    }

    [Fact]
    public void MapError_Success_SkipsLambda()
    {
        var called = false;

        var r = Result.Success(5).MapError(e => { called = true; return e; });

        called.Should().BeFalse();
        r.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Default_MapError_ReceivesUninitialized()
    {
        Error? seen = null;

        var r = default(Result<int>).MapError(e => { seen = e; return Boom; });

        seen.Should().BeSameAs(Error.Uninitialized);
        r.TryGetValue(out Error? e2).Should().BeTrue();
        e2.Should().BeSameAs(Boom);
    }

    [Fact]
    public void MapError_LambdaReturnsNull_BecomesUninitialized()
    {
        var r = Result.Failure<int>(Boom).MapError(_ => null!);

        r.TryGetValue(out Error? e).Should().BeTrue();
        e.Should().BeSameAs(Error.Uninitialized);
    }

    [Fact]
    public void MapError_NullDelegate_OnFailure_ReturnsNullDelegateFailure()
    {
        var r = Result.Failure<int>(Boom).MapError(null!);

        r.TryGetValue(out Error? e).Should().BeTrue();
        e!.Code.Should().Be("result.null_delegate");
    }

    [Fact]
    public void MapError_NullDelegate_OnSuccess_StaysSuccess()
    {
        Result.Success(1).MapError(null!).IsSuccess.Should().BeTrue();
    }

    // ---- Ensure ----

    [Fact]
    public void Ensure_PredicateTrue_StaysSuccess()
    {
        Result.Success(5).Ensure(v => v > 0, Boom).IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Ensure_PredicateFalse_FailsWithGivenError()
    {
        var r = Result.Success(-5).Ensure(v => v > 0, Boom);

        r.TryGetValue(out Error? e).Should().BeTrue();
        e.Should().BeSameAs(Boom);
    }

    [Fact]
    public void Ensure_Failure_SkipsPredicate()
    {
        var called = false;

        var r = Result.Failure<int>(Boom).Ensure(v => { called = true; return true; }, Error.Conflict("c", "m"));

        called.Should().BeFalse();
        r.TryGetValue(out Error? e).Should().BeTrue();
        e.Should().BeSameAs(Boom);
    }

    [Fact]
    public void Ensure_CallerExpression_InMessage()
    {
        var r = Result.Success(-5).Ensure(v => v > 0);

        r.TryGetValue(out Error? e).Should().BeTrue();
        e!.Code.Should().Be("result.ensure_failed");
        e.Kind.Should().Be(ErrorKind.Validation);
        e.Message.Should().Be("v => v > 0");
    }

    [Fact]
    public void Ensure_ExplicitNullExpression_UsesFallbackMessage()
    {
        var r = Result.Success(-5).Ensure(v => v > 0, (string?)null);

        r.TryGetValue(out Error? e).Should().BeTrue();
        e!.Code.Should().Be("result.ensure_failed");
        e.Message.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void Ensure_NullErrorLiteral_BindsToErrorOverload_AndFallsBackToUninitialized()
    {
        var r = Result.Success(-5).Ensure(v => v > 0, null!);

        r.TryGetValue(out Error? e).Should().BeTrue();
        e.Should().BeSameAs(Error.Uninitialized);
    }

    [Fact]
    public void Default_Ensure_StaysUninitialized()
    {
        var r = default(Result<int>).Ensure(v => true, Boom);

        r.TryGetValue(out Error? e).Should().BeTrue();
        e.Should().BeSameAs(Error.Uninitialized);
    }

    [Fact]
    public void Ensure_NullPredicate_OnSuccess_ReturnsNullDelegateFailure()
    {
        var r = Result.Success(1).Ensure(null!, Boom);

        r.TryGetValue(out Error? e).Should().BeTrue();
        e!.Code.Should().Be("result.null_delegate");
    }

    [Fact]
    public void Ensure_Expression_NullPredicate_OnSuccess_ReturnsNullDelegateFailure()
    {
        var r = Result.Success(1).Ensure(null!);

        r.TryGetValue(out Error? e).Should().BeTrue();
        e!.Code.Should().Be("result.null_delegate");
    }

    // ---- Tap ----

    [Fact]
    public void Tap_Success_RunsActionAndReturnsSame()
    {
        var seen = 0;

        var r = Result.Success(7).Tap(v => seen = v);

        seen.Should().Be(7);
        r.TryGetValue(out int value).Should().BeTrue();
        value.Should().Be(7);
    }

    [Fact]
    public void Tap_Failure_SkipsAction()
    {
        var called = false;

        var r = Result.Failure<int>(Boom).Tap(_ => called = true);

        called.Should().BeFalse();
        r.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Default_Tap_StaysUninitialized()
    {
        var called = false;

        var r = default(Result<int>).Tap(_ => called = true);

        called.Should().BeFalse();
        r.TryGetValue(out Error? e).Should().BeTrue();
        e.Should().BeSameAs(Error.Uninitialized);
    }

    [Fact]
    public void Tap_NullDelegate_OnSuccess_ReturnsNullDelegateFailure()
    {
        var r = Result.Success(1).Tap(null!);

        r.TryGetValue(out Error? e).Should().BeTrue();
        e!.Code.Should().Be("result.null_delegate");
    }

    [Fact]
    public void Tap_ThrowingAction_IsNotCaught()
    {
        var act = () => Result.Success(1).Tap(_ => throw new InvalidOperationException("nope"));

        act.Should().Throw<InvalidOperationException>();
    }

    // ---- GetValueOrDefault ----

    [Fact]
    public void GetValueOrDefault_Success_ReturnsValue()
    {
        Result.Success(4).GetValueOrDefault(9).Should().Be(4);
    }

    [Fact]
    public void GetValueOrDefault_Failure_ReturnsFallback()
    {
        Result.Failure<int>(Boom).GetValueOrDefault(9).Should().Be(9);
    }

    [Fact]
    public void Default_GetValueOrDefault_ReturnsFallback()
    {
        default(Result<int>).GetValueOrDefault(9).Should().Be(9);
    }
}
