using System.Collections.Generic;
using System.Linq;

using AwesomeAssertions;

using Xunit;

namespace StandSure.Functional.Tests;

/// <summary>
/// Side-effect forms that adapt <c>void</c> to <see cref="Unit"/>: <c>Match</c> with two actions, <c>Bind</c> with no
/// parameter after a <c>Result&lt;Unit&gt;</c> step, and <c>TapError</c>. The compile cases are the shapes models wrote
/// in the harness (issue #8), which failed with CS0411 before these overloads existed.
/// </summary>
public sealed class UnitAdapterTests
{
    private static readonly Error Err = Error.Validation("adapter.test", "adapter test");

    [Theory]
    [InlineData("""
                var failed = new List<Error>(); var imported = new List<int>();
                Result.Success(1).Match(onFailure: e => failed.Add(e), onSuccess: c => imported.Add(c));
                """)]
    [InlineData("""
                var failed = new List<Error>(); var imported = new List<int>();
                Result.Success(1).Match(failed.Add, imported.Add);
                """)]
    [InlineData("""
                var seen = new List<int>();
                Option.Some(1).Match(onNone: () => seen.Clear(), onSome: v => seen.Add(v));
                """)]
    [InlineData("""
                Result<int> saved = Result.Success().Bind(() => Try.Run(() => 1));
                """)]
    [InlineData("""
                var log = new List<string>();
                Result<int> r = Result.Success(1).Tap(v => log.Add("ok")).TapError(e => log.Add(e.Code));
                """)]
    public void SideEffectShapes_Compile(string body)
    {
        var source = $$"""
                       using System.Collections.Generic;
                       using StandSure.Functional;
                       public static class Probe
                       {
                           public static void Run()
                           {
                               {{body}}
                           }
                       }
                       """;

        CompileProbe.Errors(source).Should().BeEmpty();
    }

    [Fact]
    public void Match_ValueReturningLambdas_StillBindToTheFuncOverload()
    {
        var set = new HashSet<int>();

        // HashSet.Add returns bool, so the Func overload applies and wins; the result is the bool, not Unit.
        bool added = Result.Success(1).Match(onFailure: _ => false, onSuccess: set.Add);

        added.Should().BeTrue();
    }

    [Fact]
    public void Match_Actions_OnSuccess_RunsOnlyTheSuccessAction()
    {
        var calls = new List<string>();

        Unit unit = Result.Success(7).Match(onFailure: e => calls.Add("failure"), onSuccess: v => calls.Add($"success {v}"));

        unit.Should().Be(Unit.Value);
        calls.Should().Equal("success 7");
    }

    [Fact]
    public void Match_Actions_OnFailure_RunsOnlyTheFailureAction()
    {
        var calls = new List<Error>();

        _ = Result.Failure<int>(Err).Match(onFailure: calls.Add, onSuccess: _ => calls.Add(Error.Uninitialized));

        calls.Should().Equal(Err);
    }

    [Fact]
    public void Match_Actions_OnDefault_PassesUninitialized()
    {
        Error? seen = null;

        _ = default(Result<int>).Match(onFailure: e => seen = e, onSuccess: _ => { });

        seen.Should().Be(Error.Uninitialized);
    }

    [Fact]
    public void Match_Actions_NullActiveBranch_ReturnsUnitWithoutThrowing()
    {
        Unit unit = Result.Success(1).Match(onFailure: _ => { }, onSuccess: (System.Action<int>)null!);

        unit.Should().Be(Unit.Value);
    }

    [Fact]
    public void Match_Actions_NullInactiveBranch_HasNoEffect()
    {
        var ran = false;

        _ = Result.Success(1).Match(onFailure: (System.Action<Error>)null!, onSuccess: _ => ran = true);

        ran.Should().BeTrue();
    }

    [Fact]
    public void OptionMatch_Actions_RunTheActiveBranch()
    {
        var calls = new List<string>();

        _ = Option.Some(3).Match(onNone: () => calls.Add("none"), onSome: v => calls.Add($"some {v}"));
        _ = Option.None<int>().Match(onNone: () => calls.Add("none"), onSome: v => calls.Add($"some {v}"));

        calls.Should().Equal("some 3", "none");
    }

    [Fact]
    public void BindUnit_OnSuccess_RunsTheNextStep()
    {
        var result = Result.Success().Bind(() => Result.Success(42));

        result.GetValueOrDefault(0).Should().Be(42);
    }

    [Fact]
    public void BindUnit_OnFailure_SkipsTheNextStepAndKeepsTheError()
    {
        var ran = false;

        var result = Result.Failure<Unit>(Err).Bind(() =>
        {
            ran = true;
            return Result.Success(1);
        });

        ran.Should().BeFalse();
        result.TryGetValue(out Error? error).Should().BeTrue();
        error.Should().Be(Err);
    }

    [Fact]
    public void BindUnit_NullNextStep_OnSuccess_NamesTheParameterAndBind()
    {
        var result = Result.Success().Bind((System.Func<Result<int>>)null!);

        result.TryGetValue(out Error? error).Should().BeTrue();
        error!.Code.Should().Be("result.null_delegate");
        error.Message.Should().Be("The delegate 'bind' passed to Bind was null.");
    }

    [Fact]
    public void BindUnit_NullNextStep_OnFailure_KeepsTheOriginalError()
    {
        var result = Result.Failure<Unit>(Err).Bind((System.Func<Result<int>>)null!);

        result.TryGetValue(out Error? error).Should().BeTrue();
        error.Should().Be(Err);
    }

    [Fact]
    public void TapError_OnFailure_RunsTheActionAndReturnsTheSameResult()
    {
        Error? seen = null;
        var failure = Result.Failure<int>(Err);

        var result = failure.TapError(e => seen = e);

        seen.Should().Be(Err);
        result.Should().Be(failure);
    }

    [Fact]
    public void TapError_OnSuccess_DoesNotRunTheAction()
    {
        var ran = false;

        var result = Result.Success(5).TapError(_ => ran = true);

        ran.Should().BeFalse();
        result.GetValueOrDefault(0).Should().Be(5);
    }

    [Fact]
    public void TapError_OnDefault_PassesUninitialized()
    {
        Error? seen = null;

        _ = default(Result<int>).TapError(e => seen = e);

        seen.Should().Be(Error.Uninitialized);
    }

    [Fact]
    public void TapError_NullAction_OnFailure_IsANullDelegateFailure()
    {
        var result = Result.Failure<int>(Err).TapError(null!);

        result.TryGetValue(out Error? error).Should().BeTrue();
        error!.Code.Should().Be("result.null_delegate");
        error.Message.Should().Be("The delegate 'action' passed to TapError was null.");
    }

    [Fact]
    public void TapError_NullAction_OnSuccess_HasNoEffect()
    {
        Result.Success(1).TapError(null!).GetValueOrDefault(0).Should().Be(1);
    }

    [Fact]
    public void Match_Actions_ComposeAsAnExpression()
    {
        var sink = new List<int>();

        var units = new[] { Result.Success(1), Result.Failure<int>(Err), Result.Success(3) }
            .Select(r => r.Match(onFailure: _ => sink.Add(0), onSuccess: sink.Add))
            .ToList();

        units.Should().HaveCount(3);
        sink.Should().Equal(1, 0, 3);
    }
}
