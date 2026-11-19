using System;
using System.Threading.Tasks;
using AwesomeAssertions;
using Xunit;

namespace StandSure.Functional.Tests;

public sealed class ResultAsyncTests
{
    private static readonly Error Boom = Error.NotFound("thing", "missing");

    private static Task<Result<int>> Ok(int value) => Task.FromResult(Result.Success(value));

    private static Task<Result<int>> Bad() => Task.FromResult(Result.Failure<int>(Boom));

    // ---- MapAsync ----

    [Fact]
    public async Task MapAsync_SyncLambda_Compiles()
    {
        var r = await Ok(1).MapAsync(x => x + 1);

        r.GetValueOrDefault(-1).Should().Be(2);
    }

    [Fact]
    public async Task MapAsync_AsyncLambda_Compiles()
    {
        var r = await Ok(1).MapAsync(async x =>
        {
            await Task.Yield();
            return x + 1;
        });

        r.GetValueOrDefault(-1).Should().Be(2);
    }

    // Overload binding evidence: the static result type proves which overload each lambda shape picked.
    [Fact]
    public void MapAsync_Binding_SyncLambdaYieldsTaskOfResultOfInt()
    {
        Task<Result<int>> sync = Ok(1).MapAsync(x => x + 1);
        Task<Result<int>> asyncLambda = Ok(1).MapAsync(async x => x + 1);
        Task<Result<int>> returnsTask = Ok(1).MapAsync(x => Task.FromResult(x + 1));
        Task<Result<string>> toString = Ok(1).MapAsync(x => x.ToString());

        sync.Should().NotBeNull();
        asyncLambda.Should().NotBeNull();
        returnsTask.Should().NotBeNull();
        toString.Should().NotBeNull();
    }

    [Fact]
    public async Task MapAsync_Failure_SkipsLambda()
    {
        var called = false;

        var r = await Bad().MapAsync(x => { called = true; return x; });
        var r2 = await Bad().MapAsync(async x => { called = true; await Task.Yield(); return x; });

        called.Should().BeFalse();
        r.IsFailure.Should().BeTrue();
        r2.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task MapAsync_DefaultResult_StaysUninitialized()
    {
        var r = await Task.FromResult(default(Result<int>)).MapAsync(x => x + 1);

        r.TryGetValue(out Error? e).Should().BeTrue();
        e.Should().BeSameAs(Error.Uninitialized);
    }

    [Fact]
    public async Task MapAsync_NullDelegates_ReturnNullDelegateFailure()
    {
        var r = await Ok(1).MapAsync((Func<int, int>)null!);
        var r2 = await Ok(1).MapAsync((Func<int, Task<int>>)null!);

        r.TryGetValue(out Error? e).Should().BeTrue();
        e!.Code.Should().Be("result.null_delegate");
        r2.TryGetValue(out Error? e2).Should().BeTrue();
        e2!.Code.Should().Be("result.null_delegate");
    }

    [Fact]
    public async Task MapAsync_AsyncLambdaReturningNullTask_ReturnsNullTaskFailure()
    {
        var r = await Ok(1).MapAsync(_ => (Task<int>)null!);

        r.TryGetValue(out Error? e).Should().BeTrue();
        e!.Code.Should().Be("result.null_task");
    }

    [Fact]
    public async Task MapAsync_NullSourceTask_ReturnsFailure()
    {
        var r = await ((Task<Result<int>>)null!).MapAsync(x => x);

        r.TryGetValue(out Error? e).Should().BeTrue();
        e!.Code.Should().Be("result.null_task");
    }

    [Fact]
    public async Task MapAsync_SourceTaskThrows_Propagates()
    {
        var source = Task.FromException<Result<int>>(new InvalidOperationException("src"));

        var act = async () => await source.MapAsync(x => x);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task MapAsync_AsyncLambdaThrows_Propagates()
    {
        var act = async () => await Ok(1).MapAsync(async x =>
        {
            await Task.Yield();
            throw new InvalidOperationException("nope");
#pragma warning disable CS0162
            return x;
#pragma warning restore CS0162
        });

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    // ---- BindAsync ----

    [Fact]
    public async Task BindAsync_SyncLambda_Success()
    {
        var r = await Ok(1).BindAsync(x => Result.Success(x + 1));

        r.GetValueOrDefault(-1).Should().Be(2);
    }

    [Fact]
    public async Task BindAsync_AsyncLambda_Success()
    {
        var r = await Ok(1).BindAsync(async x =>
        {
            await Task.Yield();
            return Result.Success(x + 1);
        });

        r.GetValueOrDefault(-1).Should().Be(2);
    }

    [Fact]
    public async Task BindAsync_Failure_SkipsLambda()
    {
        var called = false;

        var r = await Bad().BindAsync(x => { called = true; return Result.Success(x); });
        var r2 = await Bad().BindAsync(x => { called = true; return Task.FromResult(Result.Success(x)); });

        called.Should().BeFalse();
        r.IsFailure.Should().BeTrue();
        r2.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task BindAsync_ChainOfThree_StopsAtFirstFailure()
    {
        var thirdCalled = false;

        var r = await Ok(1)
            .BindAsync(x => Task.FromResult(Result.Success(x + 1)))
            .BindAsync(_ => Task.FromResult(Result.Failure<int>(Boom)))
            .BindAsync(x => { thirdCalled = true; return Task.FromResult(Result.Success(x)); });

        thirdCalled.Should().BeFalse();
        r.TryGetValue(out Error? e).Should().BeTrue();
        e.Should().BeSameAs(Boom);
    }

    [Fact]
    public async Task BindAsync_ChainOfThree_AllSucceed()
    {
        var r = await Ok(1)
            .BindAsync(x => Task.FromResult(Result.Success(x + 1)))
            .BindAsync(x => Result.Success(x * 10))
            .BindAsync(async x => { await Task.Yield(); return Result.Success(x + 5); });

        r.GetValueOrDefault(-1).Should().Be(25);
    }

    [Fact]
    public async Task BindAsync_NullDelegates_ReturnNullDelegateFailure()
    {
        var r = await Ok(1).BindAsync((Func<int, Result<int>>)null!);
        var r2 = await Ok(1).BindAsync((Func<int, Task<Result<int>>>)null!);

        r.TryGetValue(out Error? e).Should().BeTrue();
        e!.Code.Should().Be("result.null_delegate");
        r2.TryGetValue(out Error? e2).Should().BeTrue();
        e2!.Code.Should().Be("result.null_delegate");
    }

    [Fact]
    public async Task BindAsync_DefaultResult_StaysUninitialized()
    {
        var r = await Task.FromResult(default(Result<int>)).BindAsync(x => Task.FromResult(Result.Success(x)));

        r.TryGetValue(out Error? e).Should().BeTrue();
        e.Should().BeSameAs(Error.Uninitialized);
    }

    // ---- MatchAsync ----

    [Fact]
    public async Task MatchAsync_SyncLambdas_Success()
    {
        var s = await Ok(2).MatchAsync(onFailure: _ => "bad", onSuccess: v => $"ok{v}");

        s.Should().Be("ok2");
    }

    [Fact]
    public async Task MatchAsync_SyncLambdas_Failure_FailureFirst()
    {
        var s = await Bad().MatchAsync(onFailure: e => e.Code, onSuccess: v => "ok");

        s.Should().Be("thing");
    }

    [Fact]
    public async Task MatchAsync_AsyncLambdas_BothBranches()
    {
        var ok = await Ok(2).MatchAsync(
            onFailure: async e => { await Task.Yield(); return -1; },
            onSuccess: async v => { await Task.Yield(); return v * 3; });
        var bad = await Bad().MatchAsync(
            onFailure: async e => { await Task.Yield(); return -1; },
            onSuccess: async v => { await Task.Yield(); return v * 3; });

        ok.Should().Be(6);
        bad.Should().Be(-1);
    }

    [Fact]
    public async Task MatchAsync_DefaultResult_RunsOnFailureWithUninitialized()
    {
        var e = await Task.FromResult(default(Result<int>)).MatchAsync(err => err, _ => Boom);

        e.Should().BeSameAs(Error.Uninitialized);
    }

    [Fact]
    public async Task MatchAsync_NullDelegate_ReturnsDefault()
    {
        var s = await Bad().MatchAsync(null!, v => "x");
        var s2 = await Bad().MatchAsync((Func<Error, Task<string>>)null!, v => Task.FromResult("x"));

        s.Should().BeNull();
        s2.Should().BeNull();
    }

    [Fact]
    public async Task MatchAsync_NullSourceTask_RunsOnFailure()
    {
        var code = await ((Task<Result<int>>)null!).MatchAsync(e => e.Code, _ => "ok");

        code.Should().Be("result.null_task");
    }

    // ---- EnsureAsync ----

    [Fact]
    public async Task EnsureAsync_SyncPredicate_TrueAndFalse()
    {
        var pass = await Ok(5).EnsureAsync(v => v > 0, Boom);
        var fail = await Ok(-5).EnsureAsync(v => v > 0, Boom);

        pass.IsSuccess.Should().BeTrue();
        fail.TryGetValue(out Error? e).Should().BeTrue();
        e.Should().BeSameAs(Boom);
    }

    [Fact]
    public async Task EnsureAsync_AsyncPredicate_TrueAndFalse()
    {
        var pass = await Ok(5).EnsureAsync(async v => { await Task.Yield(); return v > 0; }, Boom);
        var fail = await Ok(-5).EnsureAsync(async v => { await Task.Yield(); return v > 0; }, Boom);

        pass.IsSuccess.Should().BeTrue();
        fail.TryGetValue(out Error? e).Should().BeTrue();
        e.Should().BeSameAs(Boom);
    }

    [Fact]
    public async Task EnsureAsync_Failure_SkipsPredicate()
    {
        var called = false;

        var r = await Bad().EnsureAsync(v => { called = true; return true; }, Error.Conflict("c", "m"));
        var r2 = await Bad().EnsureAsync(v => { called = true; return Task.FromResult(true); }, Error.Conflict("c", "m"));

        called.Should().BeFalse();
        r.TryGetValue(out Error? e).Should().BeTrue();
        e.Should().BeSameAs(Boom);
        r2.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task EnsureAsync_DefaultResult_StaysUninitialized()
    {
        var r = await Task.FromResult(default(Result<int>)).EnsureAsync(v => true, Boom);

        r.TryGetValue(out Error? e).Should().BeTrue();
        e.Should().BeSameAs(Error.Uninitialized);
    }

    [Fact]
    public async Task EnsureAsync_Factory_SyncAndAsyncPredicates_TrueAndFalse()
    {
        var calls = 0;
        Error Make(int v) { calls++; return Error.Validation("v.bad", $"{v}"); }

        var syncPass = await Ok(5).EnsureAsync(v => v > 0, Make);
        var syncFail = await Ok(-5).EnsureAsync(v => v > 0, Make);
        var asyncPass = await Ok(5).EnsureAsync(async v => { await Task.Yield(); return v > 0; }, Make);
        var asyncFail = await Ok(-6).EnsureAsync(async v => { await Task.Yield(); return v > 0; }, Make);

        syncPass.IsSuccess.Should().BeTrue();
        asyncPass.IsSuccess.Should().BeTrue();
        syncFail.TryGetValue(out Error? e1).Should().BeTrue();
        e1!.Message.Should().Be("-5");
        asyncFail.TryGetValue(out Error? e2).Should().BeTrue();
        e2!.Message.Should().Be("-6");
        calls.Should().Be(2);
    }

    [Fact]
    public async Task EnsureAsync_Factory_Failure_SkipsPredicateAndFactory()
    {
        var called = false;

        var r = await Bad().EnsureAsync(v => { called = true; return true; }, v => { called = true; return Boom; });
        var r2 = await Bad().EnsureAsync(v => { called = true; return Task.FromResult(true); }, v => { called = true; return Boom; });

        called.Should().BeFalse();
        r.TryGetValue(out Error? e).Should().BeTrue();
        e.Should().BeSameAs(Boom);
        r2.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task EnsureAsync_Factory_NullDelegates_ReturnNullDelegateFailure()
    {
        var nullFactorySync = await Ok(-1).EnsureAsync(v => v > 0, (Func<int, Error>)null!);
        var nullFactoryAsync = await Ok(-1).EnsureAsync(v => Task.FromResult(v > 0), (Func<int, Error>)null!);
        var nullFactoryPass = await Ok(1).EnsureAsync(v => Task.FromResult(v > 0), (Func<int, Error>)null!);
        var nullPredicate = await Ok(1).EnsureAsync((Func<int, Task<bool>>)null!, v => Boom);
        var nullTask = await Ok(1).EnsureAsync(v => (Task<bool>)null!, v => Boom);

        nullFactorySync.Match(onFailure: e => e.Code, onSuccess: _ => "").Should().Be("result.null_delegate");
        nullFactoryAsync.Match(onFailure: e => e.Code, onSuccess: _ => "").Should().Be("result.null_delegate");
        nullFactoryPass.IsSuccess.Should().BeTrue();
        nullPredicate.Match(onFailure: e => e.Code, onSuccess: _ => "").Should().Be("result.null_delegate");
        nullTask.Match(onFailure: e => e.Code, onSuccess: _ => "").Should().Be("result.null_task");
    }

    [Fact]
    public async Task EnsureAsync_NullPredicates_ReturnNullDelegateFailure()
    {
        var r = await Ok(1).EnsureAsync((Func<int, bool>)null!, Boom);
        var r2 = await Ok(1).EnsureAsync((Func<int, Task<bool>>)null!, Boom);

        r.TryGetValue(out Error? e).Should().BeTrue();
        e!.Code.Should().Be("result.null_delegate");
        r2.TryGetValue(out Error? e2).Should().BeTrue();
        e2!.Code.Should().Be("result.null_delegate");
    }
}
