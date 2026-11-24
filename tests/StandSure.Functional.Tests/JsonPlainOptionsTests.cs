using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

using AwesomeAssertions;

using StandSure.Functional.Json;

using Xunit;

namespace StandSure.Functional.Tests;

/// <summary>
/// <c>AddFunctional()</c> on options the caller has not given a resolver. Inserting only the source-generated context
/// into an empty chain left the context as the sole resolver, so every caller type failed with "metadata not provided".
/// </summary>
public sealed class JsonPlainOptionsTests
{
    [Fact]
    public void AddFunctional_OnPlainOptions_SerializesResultOfCallerType()
    {
        var options = new JsonSerializerOptions().AddFunctional();

        var json = JsonSerializer.Serialize(Result.Success(new JsonSample(1, "a")), options);

        json.Should().Be("""{"value":{"Id":1,"Name":"a"}}""");
    }

    [Fact]
    public void AddFunctional_OnPlainOptions_SerializesCallerTypeOnItsOwn()
    {
        var options = new JsonSerializerOptions().AddFunctional();

        var json = JsonSerializer.Serialize(new JsonSample(1, "a"), options);

        json.Should().Be("""{"Id":1,"Name":"a"}""");
    }

    [Fact]
    public void AddFunctional_OnPlainOptions_StillWritesErrorInTheWireFormat()
    {
        var options = new JsonSerializerOptions().AddFunctional();

        var json = JsonSerializer.Serialize(Error.NotFound("plain.missing", "m"), options);

        json.Should().Contain("\"kind\":\"NotFound\"").And.Contain("\"code\":\"plain.missing\"");
    }

    [Fact]
    public void AddFunctional_WithCallerResolver_LeavesTheCallersChoiceAlone()
    {
        var callerResolver = new DefaultJsonTypeInfoResolver();
        var options = new JsonSerializerOptions { TypeInfoResolver = callerResolver };

        _ = options.AddFunctional();

        options.TypeInfoResolverChain.Should().HaveCount(2);
        options.TypeInfoResolverChain.Last().Should().BeSameAs(callerResolver);
    }

    [Fact]
    public void AddFunctional_WithCallersSourceGeneratedContext_WritesErrorInTheWireFormat()
    {
        var options = new JsonSerializerOptions { TypeInfoResolver = JsonSampleContext.Default }.AddFunctional();

        var json = JsonSerializer.Serialize(Error.NotFound("aot.missing", "m"), options);

        json.Should().Contain("\"kind\":\"NotFound\"").And.Contain("\"code\":\"aot.missing\"");
    }

    [Fact]
    public void AddFunctional_OnPlainOptions_RoundTripsNestedErrorInTheWireFormat()
    {
        var options = new JsonSerializerOptions().AddFunctional();
        var error = Error
            .Validation("outer.children", "children", [Error.NotFound("child.a", "a"), Error.Conflict("child.b", "b")])
            .Wrap("outer.wrapped", "outer");

        var json = JsonSerializer.Serialize(error, options);
        var back = JsonSerializer.Deserialize<Error>(json, options)!;

        json.Should().Contain("\"inner\":{").And.Contain("\"errors\":[").And.Contain("\"code\":\"child.b\"");
        back.Code.Should().Be("outer.wrapped");
        back.Inner!.Errors.Select(e => e.Code).Should().Equal("child.a", "child.b");
    }
}
