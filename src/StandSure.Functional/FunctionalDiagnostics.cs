using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace StandSure.Functional;

/// <summary>BCL-only telemetry for failures: one <see cref="ActivitySource"/> and one <see cref="Meter"/>.</summary>
internal static class FunctionalDiagnostics
{
    public const string Name = "StandSure.Functional";

    public const string EmitErrorDetailsSwitch = "StandSure.Functional.EmitErrorDetails";

    public static readonly ActivitySource Source = new(Name);

    public static readonly Meter Meter = new(Name);

    public static readonly Counter<long> Failures = Meter.CreateCounter<long>(
        "functional.failures",
        unit: "{failure}",
        description: "Root failures created by the library (one per Error construction; wraps are not counted).");

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
