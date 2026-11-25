using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using AwesomeAssertions;

using Xunit;

namespace StandSure.Functional.Tests;

/// <summary>
/// <c>TapAsync</c>, <c>TapErrorAsync</c> and <c>MapErrorAsync</c> on <see cref="Task{TResult}"/> of
/// <see cref="Result{T}"/>, each in the ordinary-lambda and task-returning-lambda forms. Same rules as their sync
/// counterparts and the other async combinators; delegates that observe or reshape a failure keep it when they are
/// missing (#41). Proposed by the Eventually design (issue draft "Add TapAsync and TapErrorAsync").
/// </summary>
public sealed class ResultTaskSideEffectTests
{
    private static readonly Error Boom = Error.NotFound("side.effect", "missing");

    private static Task<Result<int>> Ok(int value) => Task.FromResult(Result.Success(value));

    private static Task<Result<int>> Failed() => Task.FromResult(Result.Failure<int>(Boom));

    // ---- TapAsync ----

    [Fact]
    public async Task TapAsync_Sync_OnSuccess_RunsOnceWithTheValue_AndReturnsTheResultUnchanged()
    {
        var seen = new List<int>();

        var result = await Ok(7).TapAsync(v => seen.Add(v));

        seen.Should().Equal(7);
        result.GetValueOrDefault(0).Should().Be(7);
    }

    [Fact]
    public async Task TapAsync_Async_OnSuccess_RunsOnceWithTheValue_AndReturnsTheResultUnchanged()
    {
        var seen = new List<int>();

        var result = await Ok(7).TapAsync(async v =>
        {
            await Task.Yield();
            seen.Add(v);
        });

        seen.Should().Equal(7);
        result.GetValueOrDefault(0).Should().Be(7);
    }

    [Fact]
    public async Task TapAsync_OnFailure_DoesNotRun_BothForms()
    {
        var ran = 0;

        var a = await Failed().TapAsync(_ => ran++);
        var b = await Failed().TapAsync(_ =>
        {
            ran++;
            return Task.CompletedTask;
        });

        ran.Should().Be(0);
        ErrorOf(a).Should().Be(Boom);
        ErrorOf(b).Should().Be(Boom);
    }

    [Fact]
    public async Task TapAsync_NullDelegate_OnSuccess_IsANullDelegateFailure_BothForms()
    {
        ErrorOf(await Ok(1).TapAsync((Action<int>)null!)).Code.Should().Be("result.null_delegate");
        ErrorOf(await Ok(1).TapAsync((Func<int, Task?>)null!)).Code.Should().Be("result.null_delegate");
    }

    [Fact]
    public async Task TapAsync_LambdaReturnsNullTask_IsANullTaskFailure()
    {
        var result = await Ok(1).TapAsync(_ => (Task?)null);

        ErrorOf(result).Code.Should().Be("result.null_task");
    }

    // ---- TapErrorAsync ----

    [Fact]
    public async Task TapErrorAsync_OnFailure_RunsOnceWithTheError_AndReturnsTheResultUnchanged_BothForms()
    {
        var seen = new List<Error>();

        var a = await Failed().TapErrorAsync(e => seen.Add(e));
        var b = await Failed().TapErrorAsync(async e =>
        {
            await Task.Yield();
            seen.Add(e);
        });

        seen.Should().Equal(Boom, Boom);
        ErrorOf(a).Should().Be(Boom);
        ErrorOf(b).Should().Be(Boom);
    }

    [Fact]
    public async Task TapErrorAsync_OnSuccess_DoesNotRun_BothForms()
    {
        var ran = 0;

        var a = await Ok(3).TapErrorAsync(_ => ran++);
        var b = await Ok(3).TapErrorAsync(_ =>
        {
            ran++;
            return Task.CompletedTask;
        });

        ran.Should().Be(0);
        a.GetValueOrDefault(0).Should().Be(3);
        b.GetValueOrDefault(0).Should().Be(3);
    }

    [Fact]
    public async Task TapErrorAsync_OnDefault_PassesUninitialized()
    {
        Error? seen = null;

        _ = await Task.FromResult(default(Result<int>)).TapErrorAsync(e => seen = e);

        seen.Should().Be(Error.Uninitialized);
    }

    [Fact]
    public async Task TapErrorAsync_NullDelegate_OnFailure_KeepsTheOriginalError_BothForms()
    {
        ErrorOf(await Failed().TapErrorAsync((Action<Error>)null!)).Should().Be(Boom);
        ErrorOf(await Failed().TapErrorAsync((Func<Error, Task?>)null!)).Should().Be(Boom);
    }

    [Fact]
    public async Task TapErrorAsync_LambdaReturnsNullTask_KeepsTheOriginalError()
    {
        var result = await Failed().TapErrorAsync(_ => (Task?)null);

        ErrorOf(result).Should().Be(Boom);
    }

    // The Eventually case: roll back when the chain failed, without leaving the chain.
    [Fact]
    public async Task TapErrorAsync_RunsAnAsyncRollbackOnlyOnFailure()
    {
        var rolledBack = 0;

        Task RollbackAsync()
        {
            rolledBack++;
            return Task.CompletedTask;
        }

        _ = await Ok(1).BindAsync(_ => Task.FromResult(Result.Failure<Unit>(Boom))).TapErrorAsync(_ => RollbackAsync());
        _ = await Ok(1).BindAsync(_ => Task.FromResult(Result.Success())).TapErrorAsync(_ => RollbackAsync());

        rolledBack.Should().Be(1);
    }

    // ---- MapErrorAsync ----

    [Fact]
    public async Task MapErrorAsync_OnFailure_TransformsTheError_BothForms()
    {
        var a = await Failed().MapErrorAsync(e => e.Wrap("outer.sync", "Sync wrap."));
        var b = await Failed().MapErrorAsync(e => Task.FromResult(e.Wrap("outer.async", "Async wrap.")));

        ErrorOf(a).Code.Should().Be("outer.sync");
        ErrorOf(a).Inner.Should().Be(Boom);
        ErrorOf(b).Code.Should().Be("outer.async");
        ErrorOf(b).Inner.Should().Be(Boom);
    }

    [Fact]
    public async Task MapErrorAsync_OnSuccess_DoesNotRun_BothForms()
    {
        var ran = 0;

        var a = await Ok(4).MapErrorAsync(e =>
        {
            ran++;
            return e;
        });
        var b = await Ok(4).MapErrorAsync(e =>
        {
            ran++;
            return Task.FromResult(e);
        });

        ran.Should().Be(0);
        a.GetValueOrDefault(0).Should().Be(4);
        b.GetValueOrDefault(0).Should().Be(4);
    }

    [Fact]
    public async Task MapErrorAsync_NullDelegate_OnFailure_KeepsTheOriginalError_BothForms()
    {
        ErrorOf(await Failed().MapErrorAsync((Func<Error, Error>)null!)).Should().Be(Boom);
        ErrorOf(await Failed().MapErrorAsync((Func<Error, Task<Error>?>)null!)).Should().Be(Boom);
    }

    [Fact]
    public async Task MapErrorAsync_LambdaReturnsNullTask_KeepsTheOriginalError()
    {
        var result = await Failed().MapErrorAsync(_ => (Task<Error>?)null);

        ErrorOf(result).Should().Be(Boom);
    }

    [Fact]
    public async Task MapErrorAsync_LambdaReturnsNullError_BecomesUninitialized()
    {
        var result = await Failed().MapErrorAsync(_ => Task.FromResult<Error>(null!));

        ErrorOf(result).Should().Be(Error.Uninitialized);
    }

    // ---- Shared rules ----

    [Fact]
    public async Task NullSourceTask_IsANullTaskFailure_ForEveryMethod()
    {
        Task<Result<int>> source = null!;

        ErrorOf(await source.TapAsync(_ => { })).Code.Should().Be("result.null_task");
        ErrorOf(await source.TapAsync(_ => Task.CompletedTask)).Code.Should().Be("result.null_task");
        ErrorOf(await source.TapErrorAsync(_ => { })).Code.Should().Be("result.null_task");
        ErrorOf(await source.TapErrorAsync(_ => Task.CompletedTask)).Code.Should().Be("result.null_task");
        ErrorOf(await source.MapErrorAsync(e => e)).Code.Should().Be("result.null_task");
        ErrorOf(await source.MapErrorAsync(e => Task.FromResult(e))).Code.Should().Be("result.null_task");
    }

    [Fact]
    public async Task ExceptionInTheLambda_FaultsTheReturnedTask()
    {
        var act = () => Ok(1).TapAsync(_ => throw new InvalidOperationException("tap"));

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("tap");
    }


    // The rollback path itself: a throwing TapErrorAsync lambda faults the returned task too.
    [Fact]
    public async Task ExceptionInATapErrorAsyncLambda_FaultsTheReturnedTask()
    {
        var act = () => Failed().TapErrorAsync(_ => Task.FromException(new InvalidOperationException("rollback")));

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("rollback");
    }

    // The task-returning forms read the error themselves (not through the sync cores), so check default here too.
    [Fact]
    public async Task TaskForms_OnDefault_PassUninitialized()
    {
        Error? seen = null;

        _ = await Task.FromResult(default(Result<int>)).TapErrorAsync(e =>
        {
            seen = e;
            return Task.CompletedTask;
        });
        var mapped = await Task.FromResult(default(Result<int>)).MapErrorAsync(e => Task.FromResult(e));

        seen.Should().Be(Error.Uninitialized);
        ErrorOf(mapped).Should().Be(Error.Uninitialized);
    }

    private static Error ErrorOf<T>(Result<T> result)
    {
        result.TryGetValue(out Error? error).Should().BeTrue("the result should be a failure");
        return error!;
    }
}
