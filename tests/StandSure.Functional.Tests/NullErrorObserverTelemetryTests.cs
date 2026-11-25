using System;

using AwesomeAssertions;

using Xunit;

namespace StandSure.Functional.Tests;

/// <summary>
/// Issue #41: a null <c>TapError</c> or <c>MapError</c> delegate on a failure keeps the original error and records the
/// missing delegate in telemetry only, as <c>Match</c> does for a null active branch. A success records nothing.
/// </summary>
[Collection(TelemetryCollection.Name)]
public sealed class NullErrorObserverTelemetryTests : IDisposable
{
    private const string NullDelegateCode = "result.null_delegate";

    private static readonly Error NotFound = Error.NotFound("tel.observer.nf", "missing");

    private readonly FailureEventScope _scope = new();

    public void Dispose() => this._scope.Dispose();

    private int NullDelegateCount() => this._scope.Count(NullDelegateCode);

    [Fact]
    public void TapError_NullOnFailure_RecordsOneNullDelegateFailure()
    {
        _ = Result.Failure<int>(NotFound).TapError(null!);

        this.NullDelegateCount().Should().Be(1);
    }

    [Fact]
    public void MapError_NullOnFailure_RecordsOneNullDelegateFailure()
    {
        _ = Result.Failure<int>(NotFound).MapError(null!);

        this.NullDelegateCount().Should().Be(1);
    }

    [Fact]
    public void TapError_NullOnSuccess_RecordsNothing()
    {
        _ = Result.Success(1).TapError(null!);

        this.NullDelegateCount().Should().Be(0);
    }

    [Fact]
    public void MapError_NullOnSuccess_RecordsNothing()
    {
        _ = Result.Success(1).MapError(null!);

        this.NullDelegateCount().Should().Be(0);
    }
}
