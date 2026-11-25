using System;

using AwesomeAssertions;

using Xunit;

namespace StandSure.Functional.Tests;

/// <summary>
/// The side-effect <c>Match</c> forms keep the documented null-delegate telemetry: a null active action records one
/// <c>result.null_delegate</c> failure, and a null inactive action records nothing.
/// </summary>
[Collection(TelemetryCollection.Name)]
public sealed class UnitAdapterTelemetryTests : IDisposable
{
    private const string NullDelegateCode = "result.null_delegate";

    private readonly FailureEventScope _scope = new();

    public void Dispose() => this._scope.Dispose();

    private int NullDelegateCount() => this._scope.Count(NullDelegateCode);

    [Fact]
    public void ResultMatch_NullActiveAction_RecordsNullDelegateFailure()
    {
        _ = Result.Success(1).Match(onFailure: _ => { }, onSuccess: (Action<int>)null!);

        this.NullDelegateCount().Should().Be(1);
    }

    [Fact]
    public void ResultMatch_NullInactiveAction_RecordsNothing()
    {
        _ = Result.Success(1).Match(onFailure: (Action<Error>)null!, onSuccess: _ => { });

        this.NullDelegateCount().Should().Be(0);
    }

    [Fact]
    public void OptionMatch_NullActiveAction_OnNone_RecordsNullDelegateFailure()
    {
        _ = Option.None<int>().Match(onNone: (Action)null!, onSome: _ => { });

        this.NullDelegateCount().Should().Be(1);
    }

    [Fact]
    public void OptionMatch_NullActiveAction_OnSome_RecordsNullDelegateFailure()
    {
        _ = Option.Some(1).Match(onNone: () => { }, onSome: (Action<int>)null!);

        this.NullDelegateCount().Should().Be(1);
    }
}
