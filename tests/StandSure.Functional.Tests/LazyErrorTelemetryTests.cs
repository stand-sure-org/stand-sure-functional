using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Threading.Tasks;
using AwesomeAssertions;
using Xunit;

namespace StandSure.Functional.Tests;

/// <summary>The factory overloads build (and so record) an error only on the failing path.</summary>
[Collection(TelemetryCollection.Name)]
public sealed class LazyErrorTelemetryTests : IDisposable
{
    private const string TestSourceName = "StandSure.Functional.Tests.LazySource";

    private readonly ActivitySource _testSource = new(TestSourceName);
    private readonly ActivityListener _activityListener;
    private readonly MeterListener _meterListener = new();
    private readonly List<string> _codes = [];
    private readonly object _gate = new();

    public LazyErrorTelemetryTests()
    {
        _activityListener = new ActivityListener
        {
            ShouldListenTo = s => s.Name == TestSourceName,
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData,
        };
        ActivitySource.AddActivityListener(_activityListener);
        _meterListener.InstrumentPublished = (instrument, listener) =>
        {
            if (instrument.Meter.Name == "StandSure.Functional" && instrument.Name == "functional.failures")
            {
                listener.EnableMeasurementEvents(instrument);
            }
        };
        _meterListener.SetMeasurementEventCallback<long>((_, _, tags, _) =>
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
        _meterListener.Start();
    }

    public void Dispose()
    {
        _meterListener.Dispose();
        _activityListener.Dispose();
        _testSource.Dispose();
    }

    [Fact]
    public void Ensure_Factory_RecordsOnlyOnFailure()
    {
        using var activity = _testSource.StartActivity("op")!;

        var pass = Result.Success(5).Ensure(v => v > 0, v => Error.Validation("lazy.ensure.pass", $"{v}"));
        var fail = Result.Success(-5).Ensure(v => v > 0, v => Error.Validation("lazy.ensure.fail", $"{v}"));

        pass.IsSuccess.Should().BeTrue();
        fail.IsFailure.Should().BeTrue();
        Count("lazy.ensure.pass").Should().Be(0);
        Count("lazy.ensure.fail").Should().Be(1);
        activity.Events.Should().ContainSingle().Which.Tags.Should().Contain(new KeyValuePair<string, object?>("error.code", "lazy.ensure.fail"));
    }

    [Fact]
    public void ToResult_Factory_RecordsOnlyOnNone()
    {
        using var activity = _testSource.StartActivity("op")!;

        var some = Option.Some(1).ToResult(() => Error.NotFound("lazy.toresult.some", "m"));
        var none = Option<int>.None.ToResult(() => Error.NotFound("lazy.toresult.none", "m"));

        some.IsSuccess.Should().BeTrue();
        none.IsFailure.Should().BeTrue();
        Count("lazy.toresult.some").Should().Be(0);
        Count("lazy.toresult.none").Should().Be(1);
        activity.Events.Should().ContainSingle();
    }

    [Fact]
    public async Task EnsureAsync_Factory_RecordsOnlyOnFailure()
    {
        using var activity = _testSource.StartActivity("op")!;

        var syncPass = await Task.FromResult(Result.Success(5)).EnsureAsync(v => v > 0, _ => Error.Validation("lazy.async.sync.pass", "m"));
        var syncFail = await Task.FromResult(Result.Success(-5)).EnsureAsync(v => v > 0, _ => Error.Validation("lazy.async.sync.fail", "m"));
        var asyncPass = await Task.FromResult(Result.Success(5)).EnsureAsync(
            async v => { await Task.Yield(); return v > 0; },
            _ => Error.Validation("lazy.async.task.pass", "m"));
        var asyncFail = await Task.FromResult(Result.Success(-5)).EnsureAsync(
            async v => { await Task.Yield(); return v > 0; },
            _ => Error.Validation("lazy.async.task.fail", "m"));

        syncPass.IsSuccess.Should().BeTrue();
        asyncPass.IsSuccess.Should().BeTrue();
        syncFail.IsFailure.Should().BeTrue();
        asyncFail.IsFailure.Should().BeTrue();
        Count("lazy.async.sync.pass").Should().Be(0);
        Count("lazy.async.task.pass").Should().Be(0);
        Count("lazy.async.sync.fail").Should().Be(1);
        Count("lazy.async.task.fail").Should().Be(1);
        activity.Events.Should().HaveCount(2);
    }

    [Fact]
    public void EagerEnsure_RecordsEvenOnSuccess()
    {
        // The documented cost of the eager overload: the Error is built, and recorded, before Ensure runs.
        var pass = Result.Success(5).Ensure(v => v > 0, Error.Validation("lazy.eager.pass", "m"));

        pass.IsSuccess.Should().BeTrue();
        Count("lazy.eager.pass").Should().Be(1);
    }

    private int Count(string code)
    {
        lock (_gate)
        {
            return _codes.Count(c => c == code);
        }
    }
}
