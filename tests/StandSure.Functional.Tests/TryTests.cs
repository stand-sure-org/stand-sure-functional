using System;
using System.Threading;
using System.Threading.Tasks;

using AwesomeAssertions;

using Xunit;

namespace StandSure.Functional.Tests;

public sealed class TryTests
{
    [Fact]
    public void Run_Success_ReturnsValue()
    {
        Result<int> r = Try.Run(() => 42);

        r.TryGetValue(out int v).Should().BeTrue();
        v.Should().Be(42);
    }

    [Fact]
    public void Run_Action_ReturnsUnit()
    {
        bool ran = false;

        Result<Unit> r = Try.Run(() => { ran = true; });

        r.IsSuccess.Should().BeTrue();
        ran.Should().BeTrue();
    }

    [Fact]
    public void Run_Throws_ReturnsUnexpectedWithException()
    {
        InvalidOperationException ex = new("boom");

        Result<int> r = Try.Run((Func<int>)(() => throw ex));

        r.TryGetValue(out Error? e).Should().BeTrue();
        e!.Kind.Should().Be(ErrorKind.Unexpected);
        e.Exception.Should().BeSameAs(ex);
    }

    [Fact]
    public void Run_OperationCanceled_Propagates()
    {
        Action act = () => Try.Run((Func<int>)(() => throw new OperationCanceledException()));

        act.Should().Throw<OperationCanceledException>();
    }

    [Fact]
    public void Run_OutOfMemory_Propagates()
    {
        Action act = () => Try.Run((Func<int>)(() => throw new OutOfMemoryException()));

        act.Should().Throw<OutOfMemoryException>();
    }

    [Fact]
    public void Run_AggregateNotAllCancellation_BecomesFailure()
    {
        AggregateException ae = new(new InvalidOperationException("x"), new OperationCanceledException());

        Result<int> r = Try.Run((Func<int>)(() => throw ae));

        r.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Run_NullFunc_ReturnsFailure()
    {
        Result<int> r = Try.Run((Func<int>)null!);

        r.TryGetValue(out Error? e).Should().BeTrue();
        e!.Code.Should().Be("result.null_delegate");
    }

    [Fact]
    public void Run_NullAction_ReturnsFailure()
    {
        Result<Unit> r = Try.Run((Action)null!);

        r.TryGetValue(out Error? e).Should().BeTrue();
        e!.Code.Should().Be("result.null_delegate");
    }

    [Fact]
    public async Task RunAsync_Success_ReturnsValue()
    {
        Result<int> r = await Try.RunAsync(_ => Task.FromResult(7), TestContext.Current.CancellationToken);

        r.TryGetValue(out int v).Should().BeTrue();
        v.Should().Be(7);
    }

    [Fact]
    public async Task RunAsync_NonGeneric_ReturnsUnit()
    {
        Result<Unit> r = await Try.RunAsync(_ => Task.CompletedTask, TestContext.Current.CancellationToken);

        r.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task RunAsync_ThrowsInTask_BecomesFailure()
    {
        Result<int> r = await Try.RunAsync<int>(async _ =>
            {
                await Task.Yield();
                throw new InvalidOperationException("late");
            },
            TestContext.Current.CancellationToken);

        r.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task RunAsync_TaskCanceled_Propagates()
    {
        using CancellationTokenSource cts = new();
        await cts.CancelAsync();

        Func<Task> act = async () => await Try.RunAsync(ct => Task.FromCanceled<int>(ct), cts.Token);

        await act.Should().ThrowAsync<TaskCanceledException>();
    }

    [Fact]
    public async Task RunAsync_SyncThrowBeforeTask_BecomesFailure()
    {
        Result<int> r = await Try.RunAsync<int>(_ => throw new InvalidOperationException("sync"), TestContext.Current.CancellationToken);

        r.TryGetValue(out Error? e).Should().BeTrue();
        e!.Exception.Should().BeOfType<InvalidOperationException>();
    }

    [Fact]
    public async Task RunAsync_SyncOceBeforeTask_Propagates()
    {
        Func<Task> act = async () => await Try.RunAsync<int>(_ => throw new OperationCanceledException(), TestContext.Current.CancellationToken);

        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task RunAsync_AggregateWrappingOce_Propagates()
    {
        Func<Task> act = async () => await Try.RunAsync<int>(_ =>
            {
                Task.FromCanceled(new CancellationToken(true)).Wait();
                return Task.FromResult(1);
            },
            TestContext.Current.CancellationToken);

        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task RunAsync_PassesToken()
    {
        using CancellationTokenSource cts = new();
        CancellationToken seen = default;

        await Try.RunAsync(ct =>
            {
                seen = ct;
                return Task.CompletedTask;
            },
            cts.Token);

        seen.Should().Be(cts.Token);
    }

    [Fact]
    public async Task RunAsync_NullDelegate_ReturnsFailure()
    {
        Result<int> r = await Try.RunAsync<int>(null!, TestContext.Current.CancellationToken);
        Result<Unit> u = await Try.RunAsync((Func<CancellationToken, Task>)null!, TestContext.Current.CancellationToken);

        r.TryGetValue(out Error? re).Should().BeTrue();
        re!.Code.Should().Be("result.null_delegate");
        u.TryGetValue(out Error? ue).Should().BeTrue();
        ue!.Code.Should().Be("result.null_delegate");
    }

    [Fact]
    public async Task RunAsync_DelegateReturnsNullTask_ReturnsNullTaskFailure()
    {
        Result<int> r = await Try.RunAsync<int>(_ => null!, TestContext.Current.CancellationToken);
        Result<Unit> u = await Try.RunAsync(_ => (Task)null!, TestContext.Current.CancellationToken);

        r.TryGetValue(out Error? re).Should().BeTrue();
        re!.Code.Should().Be("result.null_task");
        re.Exception.Should().BeNull();
        u.TryGetValue(out Error? ue).Should().BeTrue();
        ue!.Code.Should().Be("result.null_task");
    }
}
