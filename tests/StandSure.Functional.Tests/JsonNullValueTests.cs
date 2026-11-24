using System;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

using AwesomeAssertions;

using StandSure.Functional.Json;

using Xunit;

namespace StandSure.Functional.Tests;

/// <summary>A null <c>value</c> is read by the caller's options, never replaced with <c>default</c>.</summary>
[Collection(TelemetryCollection.Name)]
public sealed class JsonNullValueTests
{
    private static readonly JsonSerializerOptions Options =
        new JsonSerializerOptions { TypeInfoResolver = new DefaultJsonTypeInfoResolver() }.AddFunctional();

    [Fact]
    public void NullValue_ForInt_Throws()
    {
        var read = () => JsonSerializer.Deserialize<Result<int>>("""{"value":null}""", Options);

        read.Should().Throw<JsonException>();
    }

    [Fact]
    public void NullValue_ForDateTime_Throws()
    {
        var read = () => JsonSerializer.Deserialize<Result<DateTime>>("""{"value":null}""", Options);

        read.Should().Throw<JsonException>();
    }

    [Fact]
    public void NullValue_ForNullableInt_IsSuccessHoldingNull()
    {
        var back = JsonSerializer.Deserialize<Result<int?>>("""{"value":null}""", Options);

        back.TryGetValue(out int? v).Should().BeTrue();
        v.Should().BeNull();
    }

    [Fact]
    public void NullValue_UsesCallerConverter()
    {
        var options = new JsonSerializerOptions { TypeInfoResolver = new DefaultJsonTypeInfoResolver() }.AddFunctional();
        options.Converters.Add(new NullAsMinusOneConverter());

        var back = JsonSerializer.Deserialize<Result<int>>("""{"value":null}""", options);

        back.TryGetValue(out int v).Should().BeTrue();
        v.Should().Be(-1);
    }

    [Theory]
    [InlineData("""{"value":null,"error":{"kind":"Forbidden","code":"no","message":"m"}}""")]
    [InlineData("""{"error":{"kind":"Forbidden","code":"no","message":"m"},"value":null}""")]
    public void NullValue_ForInt_WithError_ErrorWins(string json)
    {
        var back = JsonSerializer.Deserialize<Result<int>>(json, Options);

        back.TryGetValue(out Error? e).Should().BeTrue();
        e!.Code.Should().Be("no");
    }

    [Theory]
    [InlineData("""{"value":null,"error":null}""")]
    [InlineData("""{"error":null,"value":null}""")]
    public void NullValue_ForInt_WithNullError_IsUninitialized(string json)
    {
        var back = JsonSerializer.Deserialize<Result<int>>(json, Options);

        back.TryGetValue(out Error? e).Should().BeTrue();
        e.Should().BeSameAs(Error.Uninitialized);
    }
}
