using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace StandSure.Functional;

/// <summary>BCL-only telemetry for failures: one <see cref="ActivitySource"/> and one <see cref="Meter"/>.</summary>
/// <remarks>
/// <para>Three activity events, all tagged <c>error.kind</c> and <c>error.code</c>, added to <see cref="Activity.Current"/>:</para>
/// <list type="bullet">
/// <item><c>functional.failure</c>: a root failure was built. Also counted once in <c>functional.failures</c>.</item>
/// <item><c>functional.failure.wrapped</c>: an error was wrapped (<see cref="Error.Wrap"/>); extra tag
/// <c>error.inner.code</c>. Not counted: the inner error already was.</item>
/// <item><c>functional.failure.grouped</c>: a parent holding child errors was built (<c>Error.Validation(code, message,
/// errors)</c>, and the parent of <c>Combine</c> / <c>Traverse</c>); extra tag <c>error.errors.count</c> (an
/// <see cref="int"/>). Not counted: each child already was, so <c>Combine</c> of N failures counts N.</item>
/// </list>
/// <para>Messages and metadata are added only when the <c>AppContext</c> switch <c>StandSure.Functional.EmitErrorDetails</c> is on.</para>
/// </remarks>
internal static class FunctionalDiagnostics
{
    private const string Name = "StandSure.Functional";

    private const string EmitErrorDetailsSwitch = "StandSure.Functional.EmitErrorDetails";

    public static readonly ActivitySource Source = new(Name);

    private static readonly Meter Meter = new(Name);

    private static readonly Counter<long> Failures = Meter.CreateCounter<long>(
        "functional.failures",
        unit: "{failure}",
        description: "Root failures created by the library (one per Error construction; wraps and groups of child errors are not counted).");

    /// <summary>Records a newly constructed root failure: counts it once and adds an event to the current activity.</summary>
    /// <param name="error">The error just constructed.</param>
    public static void RecordFailure(Error error)
    {
        if (Failures.Enabled)
        {
            Failures.Add(
                1,
                new KeyValuePair<string, object?>("error.kind", error.Kind.ToString()),
                new KeyValuePair<string, object?>("error.code", error.Code));
        }

        if (Activity.Current is not { IsAllDataRequested: true } activity)
        {
            return;
        }

        var tags = new ActivityTagsCollection
        {
            ["error.kind"] = error.Kind.ToString(),
            ["error.code"] = error.Code,
        };

        AddDetails(tags, error);
        activity.AddEvent(new ActivityEvent("functional.failure", tags: tags));
    }

    /// <summary>Records that an error was wrapped; adds an event but does not count (the root was already counted).</summary>
    /// <param name="outer">The wrapping error just constructed; its <see cref="Error.Inner"/> is the wrapped error.</param>
    public static void RecordWrap(Error outer)
    {
        if (Activity.Current is not { IsAllDataRequested: true } activity)
        {
            return;
        }

        var tags = new ActivityTagsCollection
        {
            ["error.kind"] = outer.Kind.ToString(),
            ["error.code"] = outer.Code,
            ["error.inner.code"] = outer.Inner?.Code,
        };

        AddDetails(tags, outer);
        activity.AddEvent(new ActivityEvent("functional.failure.wrapped", tags: tags));
    }

    /// <summary>Records a parent error that groups child errors; adds an event but does not count (each child was).</summary>
    /// <param name="parent">The parent error just constructed; its <see cref="Error.Errors"/> is not empty.</param>
    public static void RecordGroup(Error parent)
    {
        if (Activity.Current is not { IsAllDataRequested: true } activity)
        {
            return;
        }

        var tags = new ActivityTagsCollection
        {
            ["error.kind"] = parent.Kind.ToString(),
            ["error.code"] = parent.Code,
            ["error.errors.count"] = parent.Errors.Count,
        };

        AddDetails(tags, parent);
        activity.AddEvent(new ActivityEvent("functional.failure.grouped", tags: tags));
    }

    // Read on every record (not cached) so the switch can be toggled at runtime and in tests; the lookup is cheap
    // and only reached when an activity is current.
    private static void AddDetails(ActivityTagsCollection tags, Error error)
    {
        if (!AppContext.TryGetSwitch(EmitErrorDetailsSwitch, out var enabled) || !enabled)
        {
            return;
        }

        tags["error.message"] = error.Message;

        foreach (var (key, value) in error.Metadata)
        {
            tags["error.metadata." + key] = value;
        }
    }
}
