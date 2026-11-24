using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using AwesomeAssertions;

using Xunit;

namespace StandSure.Functional.Tests;

/// <summary>
/// The sync-delegate <c>*Async</c> overloads run the combinator's logic through an internal core with no span of its own
/// (trace review 2026-11-20, option 3). So a null delegate names the <c>*Async</c> method the caller wrote, and the call
/// produces one span, not <c>Map</c> nested inside <c>MapAsync</c>.
/// </summary>
[Collection(TelemetryCollection.Name)]
public sealed class SyncDelegateAsyncTests : IDisposable
{
    private const string DetailsSwitch = "StandSure.Functional.EmitErrorDetails";

    private static readonly Error Err = Error.Validation("sync.async", "m");

    private static Task<Result<int>> Pending => Task.FromResult(Result.Success(1));

    private static readonly Dictionary<string, (string Span, Func<Task> Call)> Overloads = new()
    {
        ["MapAsync"] = ("MapAsync", () => Pending.MapAsync(v => v + 1)),
        ["BindAsync"] = ("BindAsync", () => Pending.BindAsync(Result.Success)),
        ["MatchAsync"] = ("MatchAsync", () => Pending.MatchAsync(_ => 0, (int v) => v)),
        ["EnsureAsync(Error)"] = ("EnsureAsync", () => Pending.EnsureAsync(_ => true, Err)),
        ["EnsureAsync(Func<T, Error>)"] = ("EnsureAsync", () => Pending.EnsureAsync(_ => false, _ => Err)),
    };

    public SyncDelegateAsyncTests() => AppContext.SetSwitch(DetailsSwitch, true);

    public static TheoryData<string> OverloadKeys => [.. Overloads.Keys];

    public void Dispose() => AppContext.SetSwitch(DetailsSwitch, false);

    [Theory]
    [MemberData(nameof(OverloadKeys))]
    public async Task SyncDelegateOverload_StartsOneSpan_WithNoNestedCombinatorSpan(string key)
    {
        var (expected, call) = Overloads[key];
        using var capture = new SpanCapture();

        await call();

        capture.Spans.Should().ContainSingle().Which.OperationName.Should().Be(expected);
    }

    [Fact]
    public async Task MapAsync_NullDelegate_NamesMapAsync()
    {
        var result = await Pending.MapAsync((Func<int, int>)null!);

        MessageOf(result).Should().Be("The delegate 'map' passed to MapAsync was null.");
    }

    [Fact]
    public async Task BindAsync_NullDelegate_NamesBindAsync()
    {
        var result = await Pending.BindAsync((Func<int, Result<int>>)null!);

        MessageOf(result).Should().Be("The delegate 'bind' passed to BindAsync was null.");
    }

    [Fact]
    public async Task EnsureAsync_NullPredicate_NamesEnsureAsync()
    {
        var result = await Pending.EnsureAsync((Func<int, bool>)null!, Err);

        MessageOf(result).Should().Be("The delegate 'predicate' passed to EnsureAsync was null.");
    }

    [Fact]
    public async Task EnsureAsync_NullErrorFactory_NamesEnsureAsync()
    {
        var result = await Pending.EnsureAsync(_ => false, (Func<int, Error>)null!);

        MessageOf(result).Should().Be("The delegate 'error' passed to EnsureAsync was null.");
    }

    [Fact]
    public async Task MatchAsync_NullActiveBranch_NamesMatchAsync()
    {
        using var scope = new FailureEventScope();

        _ = await Pending.MatchAsync<int, int>(_ => -1, (Func<int, int>)null!);

        scope.Messages("result.null_delegate").Should().Equal("The delegate 'onSuccess' passed to MatchAsync was null.");
    }

    private static string MessageOf<T>(Result<T> result)
    {
        result.TryGetValue(out Error? error).Should().BeTrue();
        error!.Code.Should().Be("result.null_delegate");
        return error.Message;
    }
}
