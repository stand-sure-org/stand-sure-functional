using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using AwesomeAssertions;

using Xunit;

namespace StandSure.Functional.Tests;

/// <summary>
/// DESIGN v1.3 "Spans": every public method that runs caller code starts one span, named after the method, with no
/// status and no tags. Iterators and methods that run no caller code start none.
/// </summary>
[Collection(TelemetryCollection.Name)]
public sealed class SpanTests
{
    private static readonly Error Err = Error.Validation("span.test", "span test");

    private static Task<Result<int>> Pending => Task.FromResult(Result.Success(1));

    // Case key → (expected span name, call). Keys are strings so xUnit can enumerate the theory at discovery time.
    private static readonly Dictionary<string, (string Span, Func<Task> Call)> Cases = new()
    {
        ["Result.Match"] = ("Match", Sync(() => Result.Success(1).Match(_ => 0, v => v))),
        ["Result.Map"] = ("Map", Sync(() => Result.Success(1).Map(v => v + 1))),
        ["Result.Bind"] = ("Bind", Sync(() => Result.Success(1).Bind(Result.Success))),
        ["Result.MapError"] = ("MapError", Sync(() => Result.Failure<int>(Err).MapError(e => e))),
        ["Result.Ensure(Error)"] = ("Ensure", Sync(() => Result.Success(1).Ensure(_ => true, Err))),
        ["Result.Ensure(Func<T, Error>)"] = ("Ensure", Sync(() => Result.Success(1).Ensure(_ => true, _ => Err))),
        ["Result.Ensure(expression)"] = ("Ensure", Sync(() => Result.Success(1).Ensure(_ => true))),
        ["Result.Tap"] = ("Tap", Sync(() => Result.Success(1).Tap(_ => { }))),
        ["Result.TapError"] = ("TapError", Sync(() => Result.Failure<int>(Err).TapError(_ => { }))),
        ["Result.Match(Action)"] = ("Match", Sync(() => Result.Success(1).Match(_ => { }, _ => { }))),
        ["Result<Unit>.Bind(Func<Result<T>>)"] = ("Bind", Sync(() => Result.Success().Bind(() => Result.Success(1)))),
        ["Option.Match"] = ("Match", Sync(() => Option.Some(1).Match(() => 0, v => v))),
        ["Option.Match(Action)"] = ("Match", Sync(() => Option.Some(1).Match(() => { }, _ => { }))),
        ["Option.Map"] = ("Map", Sync(() => Option.Some(1).Map(v => v + 1))),
        ["Option.Bind"] = ("Bind", Sync(() => Option.Some(1).Bind(Option.Some))),
        ["Option.Filter"] = ("Filter", Sync(() => Option.Some(1).Filter(_ => true))),
        ["Option.ToResult(Func<Error>)"] = ("ToResult", Sync(() => Option.None<int>().ToResult(() => Err))),
        ["Traverse"] = ("Traverse", Sync(() => new[] { 1, 2 }.Traverse(Result.Success))),
        ["Try.Run<T>"] = ("Run", Sync(() => Try.Run(() => 1))),
        ["Try.Run(Action)"] = ("Run", Sync(() => Try.Run(() => { }))),
        ["Try.RunAsync<T>"] = ("RunAsync", () => Try.RunAsync(_ => Task.FromResult(1))),
        ["Try.RunAsync(Task)"] = ("RunAsync", () => Try.RunAsync(_ => Task.CompletedTask)),
        ["MapAsync(sync)"] = ("MapAsync", () => Pending.MapAsync(v => v + 1)),
        ["MapAsync(async)"] = ("MapAsync", () => Pending.MapAsync(v => Task.FromResult(v + 1))),
        ["BindAsync(sync)"] = ("BindAsync", () => Pending.BindAsync(Result.Success)),
        ["BindAsync(async)"] = ("BindAsync", () => Pending.BindAsync(v => Task.FromResult(Result.Success(v)))),
        ["MatchAsync(sync)"] = ("MatchAsync", () => Pending.MatchAsync(_ => 0, (int v) => v)),
        ["MatchAsync(async)"] = ("MatchAsync",
            () => Pending.MatchAsync(_ => Task.FromResult(0), (int v) => Task.FromResult(v))),
        ["TapAsync(sync)"] = ("TapAsync", () => Pending.TapAsync(_ => { })),
        ["TapAsync(async)"] = ("TapAsync", () => Pending.TapAsync(_ => Task.CompletedTask)),
        ["TapErrorAsync(sync)"] = ("TapErrorAsync", () => Task.FromResult(Result.Failure<int>(Err)).TapErrorAsync(_ => { })),
        ["TapErrorAsync(async)"] = ("TapErrorAsync",
            () => Task.FromResult(Result.Failure<int>(Err)).TapErrorAsync(_ => Task.CompletedTask)),
        ["MapErrorAsync(sync)"] = ("MapErrorAsync", () => Task.FromResult(Result.Failure<int>(Err)).MapErrorAsync(e => e)),
        ["MapErrorAsync(async)"] = ("MapErrorAsync",
            () => Task.FromResult(Result.Failure<int>(Err)).MapErrorAsync(e => Task.FromResult(e))),
        ["EnsureAsync(sync, Error)"] = ("EnsureAsync", () => Pending.EnsureAsync(_ => true, Err)),
        ["EnsureAsync(async, Error)"] = ("EnsureAsync", () => Pending.EnsureAsync(_ => Task.FromResult(true), Err)),
        ["EnsureAsync(sync, Func<T, Error>)"] = ("EnsureAsync", () => Pending.EnsureAsync(_ => true, _ => Err)),
        ["EnsureAsync(async, Func<T, Error>)"] = ("EnsureAsync",
            () => Pending.EnsureAsync(_ => Task.FromResult(true), _ => Err)),
    };

    // Methods that run no caller code, or are iterators (DESIGN v1.3, "Excluded: iterators").
    private static readonly Dictionary<string, Func<Task>> NoSpanCases = new()
    {
        ["Scan (iterator)"] = Sync(() => new[] { 1, 2 }.Scan(0, (a, v) => a + v).ToList()),
        ["Pairwise (iterator)"] = Sync(() => new[] { 1, 2 }.Pairwise().ToList()),
        ["Choose (iterator)"] = Sync(() => new[] { Option.Some(1) }.Choose().ToList()),
        ["Combine"] = Sync(() => new[] { Result.Success(1) }.Combine()),
        ["Partition"] = Sync(() => new[] { Result.Success(1) }.Partition()),
        ["GetValueOrDefault"] = Sync(() => Result.Success(1).GetValueOrDefault(0)),
        ["Option.ToResult(Error)"] = Sync(() => Option.Some(1).ToResult(Err)),
    };

    public static TheoryData<string> CaseKeys => [.. Cases.Keys];

    public static TheoryData<string> NoSpanCaseKeys => [.. NoSpanCases.Keys];

    [Theory]
    [MemberData(nameof(CaseKeys))]
    public async Task Method_RunningCallerCode_StartsOneSpanNamedAfterIt(string key)
    {
        var (expected, call) = Cases[key];
        using var capture = new SpanCapture();

        await call();

        var span = capture.TopLevelSpans.Should().ContainSingle().Subject;
        span.OperationName.Should().Be(expected);
        span.Status.Should().Be(ActivityStatusCode.Unset);
        span.TagObjects.Should().BeEmpty();
    }

    [Theory]
    [MemberData(nameof(NoSpanCaseKeys))]
    public async Task Method_NotRunningCallerCodeOrIterator_StartsNoSpan(string key)
    {
        using var capture = new SpanCapture();

        await NoSpanCases[key]();

        capture.Spans.Should().BeEmpty();
    }

    // Extensions that only adapt void to Unit and forward to the public method of the same name start no span of
    // their own: the target's span covers the call (DESIGN v1.3 "Spans", pass-through rule).
    [Theory]
    [InlineData("Result.Match(Action)")]
    [InlineData("Option.Match(Action)")]
    [InlineData("Result<Unit>.Bind(Func<Result<T>>)")]
    public async Task PassThroughExtension_StartsNoNestedSpan(string key)
    {
        using var capture = new SpanCapture();

        await Cases[key].Call();

        capture.Spans.Should().ContainSingle();
    }

    [Fact]
    public void Span_OnFailedResult_HasNoStatusAndNoTags()
    {
        using var capture = new SpanCapture();

        _ = Result.Success(1).Bind(_ => Result.Failure<int>(Err));

        var span = capture.TopLevelSpans.Should().ContainSingle().Subject;
        span.Status.Should().Be(ActivityStatusCode.Unset);
        span.TagObjects.Should().BeEmpty();
    }

    [Fact]
    public void Span_WhenDelegateThrowsInsideTry_HasNoStatus_AndCarriesTheFailureEvent()
    {
        using var capture = new SpanCapture();

        _ = Try.Run((Func<int>)(() => throw new InvalidOperationException("boom")));

        // DESIGN v1.3 "Known consequence": while our span is open it is Activity.Current, so the failure event that
        // Error.FromException records lands on the Run span rather than on the caller's span.
        var span = capture.TopLevelSpans.Should().ContainSingle().Subject;
        span.Status.Should().Be(ActivityStatusCode.Unset);
        span.Events.Should().ContainSingle(e => e.Name == "functional.failure");
        capture.Parent.Events.Should().BeEmpty();
    }

    [Fact]
    public async Task AsyncSpan_IsChildOfCallersActivity_AndParentsSpansStartedInsideTheDelegate()
    {
        using var capture = new SpanCapture();

        _ = await Pending.MapAsync(v => Task.FromResult(Result.Success(v).Map(x => x + 1)));

        var outer = capture.TopLevelSpans.Should().ContainSingle().Subject;
        outer.OperationName.Should().Be("MapAsync");
        capture.Spans.Should().ContainSingle(a => a.OperationName == "Map" && a.ParentSpanId == outer.SpanId);
    }

    private static Func<Task> Sync(Action call) => () =>
    {
        call();
        return Task.CompletedTask;
    };
}
