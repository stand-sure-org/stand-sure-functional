using System;
using System.Threading.Tasks;

using AwesomeAssertions;

using Xunit;

namespace StandSure.Functional.Tests;

[Collection(TelemetryCollection.Name)]
public sealed class ResultMatchTelemetryTests : IDisposable
{
    private const string NullDelegateCode = "result.null_delegate";

    private readonly FailureEventScope _scope = new();

    public void Dispose() => this._scope.Dispose();

    private int NullDelegateCount() => this._scope.Count(NullDelegateCode);

    [Fact]
    public void Match_NullOnSuccess_RecordsNullDelegateFailure()
    {
        var r = Result.Success(1).Match<int>(_ => 0, null!);

        r.Should().Be(0);
        this.NullDelegateCount().Should().Be(1);
    }

    [Fact]
    public void Match_NullOnFailure_RecordsNullDelegateFailure()
    {
        var r = Result.Failure<int>(Error.NotFound("tel.match.nf", "x")).Match<int>(null!, v => v);

        r.Should().Be(0);
        this.NullDelegateCount().Should().Be(1);
    }

    [Fact]
    public void Match_NullUnusedBranch_RecordsNothing()
    {
        Result.Success(1).Match<int>(null!, v => v).Should().Be(1);

        this.NullDelegateCount().Should().Be(0);
    }

    [Fact]
    public async Task MatchAsync_NullActiveDelegate_RecordsNullDelegateFailure()
    {
        var sync = await Task.FromResult(Result.Success(1)).MatchAsync((Func<Error, int>)null!, (Func<int, int>)null!);

        var asyncVariant = await Task.FromResult(Result.Success(1)).MatchAsync(
            (Func<Error, Task<int>>)null!,
            (Func<int, Task<int>>)null!);

        sync.Should().Be(0);
        asyncVariant.Should().Be(0);
        this.NullDelegateCount().Should().Be(2);
    }
}
