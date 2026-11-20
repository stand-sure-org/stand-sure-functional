using System;
using System.Diagnostics;
using System.Linq;

namespace StandSure.Functional.Tests;

/// <summary>
/// Starts a sampled activity and counts the <c>functional.failure</c> events recorded on it. Activity.Current flows with
/// the test's own execution context, so failures recorded by tests running in parallel never land here (unlike the
/// process-wide <c>functional.failures</c> counter).
/// </summary>
internal sealed class FailureEventScope : IDisposable
{
    private readonly ActivitySource _source;
    private readonly ActivityListener _listener;
    private readonly Activity _activity;

    public FailureEventScope()
    {
        var name = "StandSure.Functional.Tests.Scope." + Guid.NewGuid().ToString("N");
        this._source = new ActivitySource(name);

        this._listener = new ActivityListener
        {
            ShouldListenTo = s => s.Name == name,
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData,
        };

        ActivitySource.AddActivityListener(this._listener);
        this._activity = this._source.StartActivity("test")!;
    }

    public int Count(string code) =>
        this._activity.Events.Count(e => e.Name == "functional.failure"
                                         && e.Tags.Any(t => t.Key == "error.code" && Equals(t.Value, code)));

    public void Dispose()
    {
        this._activity.Dispose();
        this._listener.Dispose();
        this._source.Dispose();
    }
}
