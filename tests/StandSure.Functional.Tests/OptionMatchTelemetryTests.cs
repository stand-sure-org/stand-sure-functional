using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using AwesomeAssertions;
using Xunit;

namespace StandSure.Functional.Tests;

[Collection(TelemetryCollection.Name)]
public sealed class OptionMatchTelemetryTests : IDisposable
{
    private const string NullDelegateCode = "result.null_delegate";

    private readonly MeterListener _listener = new();
    private readonly List<string> _codes = [];
    private readonly object _gate = new();

    public OptionMatchTelemetryTests()
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
