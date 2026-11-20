using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

using AwesomeAssertions;

using StandSure.Functional.Json;

using Xunit;

namespace StandSure.Functional.Tests;

[Collection(TelemetryCollection.Name)]
public sealed class JsonTests
{
    private static readonly JsonSerializerOptions Options = new JsonSerializerOptions { TypeInfoResolver = new DefaultJsonTypeInfoResolver() }.AddFunctional();

    private static string Ser<T>(T value) => JsonSerializer.Serialize(value, Options);

    private static T De<T>(string json) => JsonSerializer.Deserialize<T>(json, Options)!;

    [Fact]
    public void Result_Success_RoundTrips()
    {
        var json = Ser(Result.Success(42));

        json.Should().Be("""{"value":42}""");
        var back = De<Result<int>>(json);
        back.IsSuccess.Should().BeTrue();
        back.TryGetValue(out int v).Should().BeTrue();
        v.Should().Be(42);
    }

    [Fact]
    public void Result_NullSuccess_RoundTripsAsSuccess()
    {
        var json = Ser(Result.Success<string?>(null));

        json.Should().Be("""{"value":null}""");
        var back = De<Result<string?>>(json);
        back.IsSuccess.Should().BeTrue();
        back.TryGetValue(out string? v).Should().BeTrue();
        v.Should().BeNull();
    }

    [Fact]
    public void Result_Failure_RoundTrips_WithInnerAndChildren()
    {
        var child = Error.Validation("c.1", "child one");
        var inner = Error.NotFound("db.row", "no row", new Dictionary<string, string> { ["id"] = "7" });
        var parent = Error.Validation("v.all", "bad", [child]).Wrap("outer", "wrapped");
        var withInner = Error.Conflict("top", "top", new Dictionary<string, string> { ["k"] = "v" }).Wrap("w", "w");
        var error = inner.Wrap("outer.2", "outer two");

        var json = Ser(Result.Failure<int>(error));

        json.Should().StartWith(
            """{"error":{"kind":"NotFound","code":"outer.2","message":"outer two","metadata":{},"errors":[],"inner":{"kind":"NotFound","code":"db.row""");

        var back = De<Result<int>>(json);
        back.TryGetValue(out Error? e).Should().BeTrue();
        e!.Kind.Should().Be(ErrorKind.NotFound);
        e.Code.Should().Be("outer.2");
        e.Inner!.Code.Should().Be("db.row");
        e.Inner.Metadata["id"].Should().Be("7");

        var back2 = De<Result<int>>(Ser(Result.Failure<int>(parent)));
        back2.TryGetValue(out Error? p).Should().BeTrue();
        p!.Inner!.Errors.Should().ContainSingle().Which.Code.Should().Be("c.1");
        p.Errors.Should().BeEmpty();
        withInner.Metadata.Should().BeEmpty();
    }

    [Fact]
    public void Deserialized_Error_CollectionsAreReadOnlySnapshots()
    {
        const string json =
            """{"error":{"kind":"Validation","code":"p","message":"m","metadata":{"k":"v"},"errors":[{"kind":"Validation","code":"c","message":"c","metadata":{"ck":"cv"}}]}}""";

        var back = De<Result<int>>(json);

        back.TryGetValue(out Error? e).Should().BeTrue();
        e!.Metadata.Should().NotBeAssignableTo<Dictionary<string, string>>();
        ((e.Metadata as IDictionary<string, string>)?.IsReadOnly ?? true).Should().BeTrue();
        e.Errors.Should().NotBeAssignableTo<List<Error>>();
        e.Errors.Should().ContainSingle().Which.Metadata.Should().NotBeAssignableTo<Dictionary<string, string>>();
    }

    [Fact]
    public void Deserialized_Error_HasNonNullCollections()
    {
        var back = De<Result<int>>("""{"error":{"kind":"Unexpected","code":"x","message":"m","metadata":null,"errors":null}}""");

        back.TryGetValue(out Error? e).Should().BeTrue();
        e!.Metadata.Should().NotBeNull().And.BeEmpty();
        e.Errors.Should().NotBeNull().And.BeEmpty();
        e.Inner.Should().BeNull();
    }

    [Fact]
    public void Result_EmptyObject_IsUninitializedFailure()
    {
        var back = De<Result<int>>("{}");

        back.TryGetValue(out Error? e).Should().BeTrue();
        e.Should().BeSameAs(Error.Uninitialized);
    }

    [Fact]
    public void Result_BothNull_IsUninitializedFailure()
    {
        var back = De<Result<string?>>("""{"value":null,"error":null}""");

        back.TryGetValue(out Error? e).Should().BeTrue();
        e.Should().BeSameAs(Error.Uninitialized);
    }

    [Fact]
    public void Result_BothPresent_ErrorWins()
    {
        var back = De<Result<int>>("""{"value":1,"error":{"kind":"Forbidden","code":"no","message":"m"}}""");

        back.TryGetValue(out Error? e).Should().BeTrue();
        e!.Code.Should().Be("no");
    }

    [Fact]
    public void Result_UnknownProperty_Ignored()
    {
        var back = De<Result<int>>("""{"extra":{"a":[1,{"b":2}]},"value":5,"more":[null]}""");

        back.TryGetValue(out int v).Should().BeTrue();
        v.Should().Be(5);
    }

    [Fact]
    public void Result_NotAnObject_ThrowsJsonException()
    {
        var act = () => De<Result<int>>("[1]");

        act.Should().Throw<JsonException>();
    }

    [Fact]
    public void Option_Some_RoundTrips()
    {
        var json = Ser(Option.Some("hi"));

        json.Should().Be("""{"value":"hi"}""");
        var back = De<Option<string>>(json);
        back.TryGetValue(out string? v).Should().BeTrue();
        v.Should().Be("hi");
    }

    [Fact]
    public void Option_EmptyObject_IsNone()
    {
        De<Option<string>>("{}").IsNone.Should().BeTrue();
        De<Option<string>>("""{"value":null}""").IsNone.Should().BeTrue();
        De<Option<string>>("""{"x":[1],"value":null}""").IsNone.Should().BeTrue();
        Ser(Option.None<string>()).Should().Be("{}");
        Ser(default(Option<int>)).Should().Be("{}");
    }

    [Fact]
    public void Default_Result_SerializesAsUninitializedError()
    {
        var json = Ser(default(Result<int>));

        json.Should().StartWith(""""{"error":{"kind":"Unexpected","code":"result.uninitialized"""");
        De<Result<int>>(json).TryGetValue(out Error? e).Should().BeTrue();
        e!.Code.Should().Be("result.uninitialized");
    }

    [Fact]
    public void Exception_NotSerialized()
    {
        var error = Error.FromException(new InvalidOperationException("boom"));

        var json = Ser(Result.Failure<int>(error));

        using var doc = JsonDocument.Parse(json);
        doc.RootElement.GetProperty("error").TryGetProperty("exception", out _).Should().BeFalse();
        De<Result<int>>(json).TryGetValue(out Error? e).Should().BeTrue();
        e!.Exception.Should().BeNull();
    }

    [Fact]
    public void ErrorKind_SerializedAsString()
    {
        Ser(ErrorKind.NotFound).Should().Be("\"NotFound\"");
        Ser(Error.Forbidden("a", "b")).Should().Contain("\"kind\":\"Forbidden\"");
    }

    [Fact]
    public void ErrorWithoutAddFunctional_StillRoundTripsInsideResult()
    {
        var plain = new JsonSerializerOptions { TypeInfoResolver = new DefaultJsonTypeInfoResolver() };

        var json = JsonSerializer.Serialize(Result.Failure<int>(Error.NotFound("a", "b")), plain);
        var back = JsonSerializer.Deserialize<Result<int>>(json, plain);

        back.TryGetValue(out Error? e).Should().BeTrue();
        e!.Code.Should().Be("a");
    }

    [Theory]
    [InlineData("""{"error":{}}""")]
    [InlineData("""{"error":{"code":"x"}}""")]
    [InlineData("""{"error":{"code":"x","message":"m"}}""")]
    [InlineData("""{"error":{"kind":"NotFound","code":null,"message":"m"}}""")]
    [InlineData("""{"error":{"kind":"NotFound","code":"x","message":"m","errors":[null]}}""")]
    [InlineData("""{"error":{"kind":"NotFound","code":"x","message":"m","metadata":{"a":null}}}""")]
    public void InvalidError_ThrowsJsonException(string json)
    {
        var act = () => De<Result<int>>(json);

        act.Should().Throw<JsonException>();
    }

    [Fact]
    public void Deserializing_DoesNotRecordNewFailures()
    {
        const string Code = "json.no.record.unique";
        var count = 0;
        using var listener = new System.Diagnostics.Metrics.MeterListener();

        listener.InstrumentPublished = (i, l) =>
        {
            if (i.Meter.Name == "StandSure.Functional" && i.Name == "functional.failures")
            {
                l.EnableMeasurementEvents(i);
            }
        };

        listener.SetMeasurementEventCallback<long>((_, _, tags, _) =>
        {
            foreach (var tag in tags)
            {
                if (tag.Key == "error.code" && Equals(tag.Value, Code))
                {
                    System.Threading.Interlocked.Increment(ref count);
                }
            }
        });

        listener.Start();

        _ = Error.NotFound(Code, "control");
        System.Threading.Volatile.Read(ref count).Should().Be(1, "the positive control must be observed");

        _ = De<Result<int>>("""{"error":{"kind":"NotFound","code":"json.no.record.unique","message":"m"}}""");

        System.Threading.Volatile.Read(ref count).Should().Be(1);
    }
}
