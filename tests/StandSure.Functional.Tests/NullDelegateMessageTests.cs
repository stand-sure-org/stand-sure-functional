using System;
using System.Threading.Tasks;

using AwesomeAssertions;

using Xunit;

namespace StandSure.Functional.Tests;

/// <summary>
/// The <c>Match</c> overloads return <c>default</c> for a null active branch, so the <c>result.null_delegate</c> message is
/// read from the failure event. <c>Match</c> reaches the error through the <c>NullBranch</c> helper, which must forward
/// the public method's name; without the forward the message would name the helper.
/// </summary>
[Collection(TelemetryCollection.Name)]
public sealed class NullDelegateMessageTests : IDisposable
{
    private const string DetailsSwitch = "StandSure.Functional.EmitErrorDetails";
    private const string NullDelegateCode = "result.null_delegate";

    private readonly FailureEventScope _scope;

    public NullDelegateMessageTests()
    {
        AppContext.SetSwitch(DetailsSwitch, true);
        this._scope = new FailureEventScope();
    }

    public void Dispose()
    {
        this._scope.Dispose();
        AppContext.SetSwitch(DetailsSwitch, false);
    }

    [Fact]
    public void ResultMatch_NullActiveBranch_NamesMatch()
    {
        _ = Result.Success(1).Match<int>(_ => -1, null!);

        this._scope.Messages(NullDelegateCode).Should().Equal("The delegate 'onSuccess' passed to Match was null.");
    }

    [Fact]
    public void OptionMatch_NullActiveBranch_NamesMatch()
    {
        _ = Option<int>.None.Match<int>(null!, v => v);

        this._scope.Messages(NullDelegateCode).Should().Equal("The delegate 'onNone' passed to Match was null.");
    }

    [Fact]
    public async Task MatchAsync_NullActiveBranch_NamesMatchAsync()
    {
        _ = await Task.FromResult(Result.Failure<int>(Error.NotFound("m.missing", "m")))
            .MatchAsync((Func<Error, Task<int>>)null!, v => Task.FromResult(v));

        this._scope.Messages(NullDelegateCode).Should().Equal("The delegate 'onFailure' passed to MatchAsync was null.");
    }
}
