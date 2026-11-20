using System;
using AwesomeAssertions;
using Xunit;

namespace StandSure.Functional.Tests;

[Collection(TelemetryCollection.Name)]
public sealed class OptionMatchTelemetryTests : IDisposable
{
    private const string NullDelegateCode = "result.null_delegate";

    private readonly FailureEventScope _scope = new();

    public void Dispose() => _scope.Dispose();

    private int NullDelegateCount() => _scope.Count(NullDelegateCode);

    [Fact]
    public void Match_NullActiveBranch_RecordsNullDelegateFailure()
    {
        Option.Some(1).Match<int>(() => 0, null!).Should().Be(0);
        Option<int>.None.Match<int>(null!, v => v).Should().Be(0);

        NullDelegateCount().Should().Be(2);
    }

    [Fact]
    public void Match_NullUnusedBranch_RecordsNothing()
    {
        Option.Some(1).Match<int>(null!, v => v).Should().Be(1);

        NullDelegateCount().Should().Be(0);
    }

    [Fact]
    public void MapBindFilter_NullDelegateOnSome_RecordsEach()
    {
        Option.Some(1).Map<int>(null!).IsNone.Should().BeTrue();
        Option.Some(1).Bind<int>(null!).IsNone.Should().BeTrue();
        Option.Some(1).Filter(null!).IsNone.Should().BeTrue();
        Option<int>.None.Map<int>(null!).IsNone.Should().BeTrue();

        NullDelegateCount().Should().Be(3);
    }
}
