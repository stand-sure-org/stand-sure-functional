using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace StandSure.Functional.Tests;

/// <summary>
/// Listens to the library's <c>StandSure.Functional</c> source and records every span it stops, under a parent activity
/// owned by the test. Use only inside <see cref="TelemetryCollection"/>: the listener is process-wide.
/// </summary>
internal sealed class SpanCapture : IDisposable
{
    private const string LibrarySource = "StandSure.Functional";

    private readonly ActivitySource _testSource;
    private readonly ActivityListener _listener;
    private readonly ConcurrentQueue<Activity> _stopped = new();

    public SpanCapture()
    {
        var testSourceName = "StandSure.Functional.Tests.Spans." + Guid.NewGuid().ToString("N");
        this._testSource = new ActivitySource(testSourceName);

        this._listener = new ActivityListener
        {
            ShouldListenTo = s => s.Name == LibrarySource || s.Name == testSourceName,
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData,
            ActivityStopped = a =>
            {
                if (a.Source.Name == LibrarySource)
                {
                    this._stopped.Enqueue(a);
                }
            },
        };

        ActivitySource.AddActivityListener(this._listener);
        this.Parent = this._testSource.StartActivity("test")!;
    }

    /// <summary>The test's own activity; library spans started directly by the test are its children.</summary>
    public Activity Parent { get; }

    /// <summary>Library spans stopped so far, in stop order.</summary>
    public IReadOnlyList<Activity> Spans => [.. this._stopped];

    /// <summary>Library spans that are direct children of <see cref="Parent"/>.</summary>
    public IReadOnlyList<Activity> TopLevelSpans => [.. this._stopped.Where(a => a.ParentSpanId == this.Parent.SpanId)];

    public void Dispose()
    {
        this.Parent.Dispose();
        this._listener.Dispose();
        this._testSource.Dispose();
    }
}
