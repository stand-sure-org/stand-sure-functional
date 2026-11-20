using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Linq;

using AwesomeAssertions;

using Xunit;

namespace StandSure.Functional.Tests;

[Collection(TelemetryCollection.Name)]
public sealed class TelemetryTests : IDisposable
{
    private const string DetailsSwitch = "StandSure.Functional.EmitErrorDetails";
    private const string TestSourceName = "StandSure.Functional.Tests.Source";

    private readonly ActivitySource _testSource = new(TestSourceName);
    private readonly ActivityListener _activityListener;
    private readonly MeterListener _meterListener = new();
    private readonly List<(long Value, Dictionary<string, object?> Tags)> _measurements = [];
    private readonly object _gate = new();
    private readonly List<Activity> _librarySpans = [];

    public TelemetryTests()
    {
        this._activityListener = new ActivityListener
        {
            ShouldListenTo = s => s.Name == "StandSure.Functional" || s.Name == TestSourceName,
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData,
            ActivityStopped = a =>
            {
                if (a.Source.Name != "StandSure.Functional")
                {
                    return;
                }

                lock (this._gate)
                {
                    this._librarySpans.Add(a);
                }
            },
        };

        ActivitySource.AddActivityListener(this._activityListener);

        this._meterListener.InstrumentPublished = (instrument, listener) =>
        {
            if (instrument.Meter.Name == "StandSure.Functional" && instrument.Name == "functional.failures")
            {
                listener.EnableMeasurementEvents(instrument);
            }
        };

        this._meterListener.SetMeasurementEventCallback<long>((_, value, tags, _) =>
        {
            var copy = new Dictionary<string, object?>();

            foreach (var tag in tags)
            {
                copy[tag.Key] = tag.Value;
            }

            lock (this._gate)
            {
                this._measurements.Add((value, copy));
            }
        });

        this._meterListener.Start();
    }

    public void Dispose()
    {
        AppContext.SetSwitch(DetailsSwitch, false);
        this._meterListener.Dispose();
        this._activityListener.Dispose();
        this._testSource.Dispose();
    }

    [Fact]
    public void Factory_AddsFailureEvent_WithKindAndCode()
    {
        using var activity = this._testSource.StartActivity("op")!;

        _ = Error.NotFound("tel.event.1", "missing");

        var evt = activity.Events.Single(e => e.Name == "functional.failure");
        Tag(evt, "error.kind").Should().Be("NotFound");
        Tag(evt, "error.code").Should().Be("tel.event.1");
    }

    [Fact]
    public void Factory_IncrementsCounterOnce()
    {
        _ = Error.Conflict("tel.counter.1", "clash");

        var hits = this.For("tel.counter.1");
        hits.Should().ContainSingle();
        hits[0].Value.Should().Be(1);
        hits[0].Tags["error.kind"].Should().Be("Conflict");
    }

    [Fact]
    public void Wrap_AddsWrappedEvent_DoesNotIncrementCounter()
    {
        var inner = Error.NotFound("tel.wrap.inner", "inner");
        using var activity = this._testSource.StartActivity("op")!;

        _ = inner.Wrap("tel.wrap.outer", "outer");

        var evt = activity.Events.Single(e => e.Name == "functional.failure.wrapped");
        Tag(evt, "error.code").Should().Be("tel.wrap.outer");
        Tag(evt, "error.inner.code").Should().Be("tel.wrap.inner");
        activity.Events.Should().NotContain(e => e.Name == "functional.failure");
        this.For("tel.wrap.outer").Should().BeEmpty();
        this.For("tel.wrap.inner").Should().ContainSingle();
    }

    [Fact]
    public void ValidationGroup_AddsGroupedEvent_DoesNotIncrementCounter()
    {
        var a = Error.Validation("tel.group.child.a", "a");
        var b = Error.Validation("tel.group.child.b", "b");
        using var activity = this._testSource.StartActivity("op")!;

        _ = Error.Validation("tel.group.parent", "two problems", [a, b]);

        var evt = activity.Events.Single(e => e.Name == "functional.failure.grouped");
        Tag(evt, "error.kind").Should().Be("Validation");
        Tag(evt, "error.code").Should().Be("tel.group.parent");
        Tag(evt, "error.errors.count").Should().Be(2);
        activity.Events.Should().NotContain(e => e.Name == "functional.failure");
        this.For("tel.group.parent").Should().BeEmpty();
    }

    [Fact]
    public void ValidationGroup_WithNoChildren_IsARootFailure()
    {
        using var activity = this._testSource.StartActivity("op")!;

        _ = Error.Validation("tel.group.empty", "nothing inside", []);

        activity.Events.Should().ContainSingle(e => e.Name == "functional.failure");
        this.For("tel.group.empty").Should().ContainSingle();
    }

    [Fact]
    public void Combine_OfNFailures_IncrementsCounterByExactlyN()
    {
        string[] codes = ["tel.combine.1", "tel.combine.2", "tel.combine.3"];
        using var activity = this._testSource.StartActivity("op")!;

        var combined = codes
            .Select(c => Result.Failure<int>(Error.NotFound(c, c)))
            .Append(Result.Success(1))
            .Combine();

        var traversed = codes.Traverse(c => c == "tel.combine.1" ? Result.Success(1) : Result.Failure<int>(Error.Conflict(c + ".t", c)));

        combined.IsFailure.Should().BeTrue();
        traversed.IsFailure.Should().BeTrue();
        codes.Sum(c => this.For(c).Count).Should().Be(3);
        (this.For("tel.combine.2.t").Count + this.For("tel.combine.3.t").Count).Should().Be(2);
        this.For("result.combine_failed").Should().BeEmpty();

        // Combine runs no caller code, so its events stay on the caller's activity. Traverse runs the caller's map
        // inside its own span (DESIGN v1.3, "Known consequence"), so the errors built there land on that span.
        activity.Events.Count(e => e.Name == "functional.failure").Should().Be(3);
        activity.Events.Where(e => e.Name == "functional.failure.grouped")
            .Select(e => Tag(e, "error.errors.count"))
            .Should().Equal(3);

        Activity traverseSpan;
        lock (this._gate)
        {
            traverseSpan = this._librarySpans.Should()
                .ContainSingle(a => a.OperationName == "Traverse" && a.ParentSpanId == activity.SpanId).Subject;
        }

        traverseSpan.Events.Count(e => e.Name == "functional.failure").Should().Be(2);
        traverseSpan.Events.Where(e => e.Name == "functional.failure.grouped")
            .Select(e => Tag(e, "error.errors.count"))
            .Should().Equal(2);
    }

    [Fact]
    public void ConvertingErrorToResult_ThreeTimes_CountsOnce()
    {
        var error = Error.Unavailable("tel.convert.1", "down");

        Result<int> a = error;
        Result<string> b = error;
        Result<Unit> c = error;

        a.IsFailure.Should().BeTrue();
        b.IsFailure.Should().BeTrue();
        c.IsFailure.Should().BeTrue();
        this.For("tel.convert.1").Should().ContainSingle();
    }

    [Fact]
    public void NoCurrentActivity_DoesNotThrow_StillCounts()
    {
        Activity.Current = null;

        var act = () => Error.Forbidden("tel.noactivity.1", "no");

        act.Should().NotThrow();
        this.For("tel.noactivity.1").Should().ContainSingle();
    }

    [Fact]
    public void Message_NotEmitted_ByDefault()
    {
        AppContext.SetSwitch(DetailsSwitch, false);
        using var activity = this._testSource.StartActivity("op")!;

        _ = Error.Unexpected("tel.msg.off", "secret message", new Dictionary<string, string> { ["k"] = "v" });

        var evt = activity.Events.Single(e => e.Name == "functional.failure");
        evt.Tags.Select(t => t.Key).Should().NotContain(["error.message", "error.metadata.k"]);
    }

    [Fact]
    public void Message_Emitted_WhenSwitchOn()
    {
        AppContext.SetSwitch(DetailsSwitch, true);
        using var activity = this._testSource.StartActivity("op")!;

        _ = Error.Unexpected("tel.msg.on", "visible message", new Dictionary<string, string> { ["k"] = "v" });

        var evt = activity.Events.Single(e => e.Name == "functional.failure");
        Tag(evt, "error.message").Should().Be("visible message");
        Tag(evt, "error.metadata.k").Should().Be("v");
    }

    [Fact]
    public void Uninitialized_IsNotRecorded()
    {
        using var activity = this._testSource.StartActivity("op")!;

        _ = Error.Uninitialized;
        Result<int> r = default;
        _ = r.IsFailure;

        activity.Events.Should().BeEmpty();
        this.For("result.uninitialized").Should().BeEmpty();
    }

    private static object? Tag(ActivityEvent evt, string key) =>
        evt.Tags.Where(t => t.Key == key).Select(t => t.Value).FirstOrDefault();

    private List<(long Value, Dictionary<string, object?> Tags)> For(string code)
    {
        lock (this._gate)
        {
            return this._measurements.Where(m => Equals(m.Tags.GetValueOrDefault("error.code"), code)).ToList();
        }
    }
}
