using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Threading.Tasks;
using AwesomeAssertions;
using Xunit;

namespace StandSure.Functional.Tests;

[Collection(TelemetryCollection.Name)]
public sealed class ResultMatchTelemetryTests : IDisposable
{
    private const string NullDelegateCode = "result.null_delegate";

    private readonly MeterListener _listener = new();
    private readonly List<string> _codes = [];
    private readonly object _gate = new();

    public ResultMatchTelemetryTests()
    {
        _listener.InstrumentPublished = (instrument, l) =>
        {
            if (instrument.Meter.Name == "StandSure.Functional" && instrument.Name == "functional.failures")
            {
                l.EnableMeasurementEvents(instrument);
            }
        };
        _listener.SetMeasurementEventCallback<long>((_, _, tags, _) =>
        {
            foreach (var tag in tags)
            {
                if (tag.Key == "error.code" && tag.Value is string code)
                {
                    lock (_gate)
                    {
                        _codes.Add(code);
                    }
                }
            }
        });
        _listener.Start();
    }

    public void Dispose() => _listener.Dispose();

    private int NullDelegateCount()
    {
        lock (_gate)
        {
            return _codes.FindAll(c => c == NullDelegateCode).Count;
        }
    }

    [Fact]
    public void Match_NullOnSuccess_RecordsNullDelegateFailure()
    {
        var r = Result.Success(1).Match<int>(_ => 0, null!);

        r.Should().Be(0);
        NullDelegateCount().Should().Be(1);
    }

    [Fact]
    public void Match_NullOnFailure_RecordsNullDelegateFailure()
    {
        var r = Result.Failure<int>(Error.NotFound("tel.match.nf", "x")).Match<int>(null!, v => v);

        r.Should().Be(0);
        NullDelegateCount().Should().Be(1);
    }

    [Fact]
    public void Match_NullUnusedBranch_RecordsNothing()
    {
        Result.Success(1).Match<int>(null!, v => v).Should().Be(1);

        NullDelegateCount().Should().Be(0);
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
        NullDelegateCount().Should().Be(2);
    }
}
