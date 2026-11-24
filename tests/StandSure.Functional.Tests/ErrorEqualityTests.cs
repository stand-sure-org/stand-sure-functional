using System;
using System.Collections.Generic;
using System.Text.Json;

using AwesomeAssertions;

using StandSure.Functional.Json;

using Xunit;

namespace StandSure.Functional.Tests;

/// <summary>DESIGN v1.4: <see cref="Error"/> equality compares contents, not the references of its collections.</summary>
public sealed class ErrorEqualityTests
{
    private static Dictionary<string, string> Md(params (string Key, string Value)[] pairs)
    {
        var d = new Dictionary<string, string>();
        foreach (var (key, value) in pairs)
        {
            d[key] = value;
        }

        return d;
    }

    [Fact]
    public void SameMetadataContents_AreEqual_WithEqualHashCodes()
    {
        var a = Error.NotFound("eq.md", "m", Md(("id", "1"), ("tenant", "t")));
        var b = Error.NotFound("eq.md", "m", Md(("tenant", "t"), ("id", "1")));

        a.Should().Be(b);
        (a == b).Should().BeTrue();
        a.GetHashCode().Should().Be(b.GetHashCode());
    }

    [Fact]
    public void DifferentMetadataValue_AreNotEqual()
    {
        Error.NotFound("eq.md", "m", Md(("id", "1"))).Should().NotBe(Error.NotFound("eq.md", "m", Md(("id", "2"))));
    }

    [Fact]
    public void DifferentMetadataKeys_AreNotEqual()
    {
        Error.NotFound("eq.md", "m", Md(("id", "1"))).Should().NotBe(Error.NotFound("eq.md", "m", Md(("key", "1"))));
    }

    [Fact]
    public void MetadataVersusNone_AreNotEqual()
    {
        Error.NotFound("eq.md", "m", Md(("id", "1"))).Should().NotBe(Error.NotFound("eq.md", "m"));
    }

    [Fact]
    public void SameChildren_AreEqual_WithEqualHashCodes()
    {
        var a = Error.Validation("eq.parent", "p", [Error.NotFound("eq.a", "a"), Error.Conflict("eq.b", "b")]);
        var b = Error.Validation("eq.parent", "p", [Error.NotFound("eq.a", "a"), Error.Conflict("eq.b", "b")]);

        a.Should().Be(b);
        a.GetHashCode().Should().Be(b.GetHashCode());
    }

    [Fact]
    public void ChildrenInADifferentOrder_AreNotEqual()
    {
        var a = Error.Validation("eq.parent", "p", [Error.NotFound("eq.a", "a"), Error.Conflict("eq.b", "b")]);
        var b = Error.Validation("eq.parent", "p", [Error.Conflict("eq.b", "b"), Error.NotFound("eq.a", "a")]);

        a.Should().NotBe(b);
    }

    [Fact]
    public void SameInnerChain_AreEqual_DifferentInner_AreNot()
    {
        var a = Error.NotFound("eq.inner", "i", Md(("id", "1"))).Wrap("eq.outer", "o");
        var b = Error.NotFound("eq.inner", "i", Md(("id", "1"))).Wrap("eq.outer", "o");
        var c = Error.NotFound("eq.inner", "i", Md(("id", "2"))).Wrap("eq.outer", "o");

        a.Should().Be(b);
        a.GetHashCode().Should().Be(b.GetHashCode());
        a.Should().NotBe(c);
    }

    [Theory]
    [InlineData(ErrorKind.Conflict, "eq.base", "m")]
    [InlineData(ErrorKind.NotFound, "eq.other", "m")]
    [InlineData(ErrorKind.NotFound, "eq.base", "other message")]
    public void KindCodeAndMessage_AllCount(ErrorKind kind, string code, string message)
    {
        var other = kind == ErrorKind.Conflict ? Error.Conflict(code, message) : Error.NotFound(code, message);

        Error.NotFound("eq.base", "m").Should().NotBe(other);
    }

    [Fact]
    public void Exception_ComparesByReference()
    {
        var boom = new InvalidOperationException("boom");

        Error.FromException(boom).Should().Be(Error.FromException(boom));
        Error.FromException(boom).Should().NotBe(Error.FromException(new InvalidOperationException("boom")));
    }

    [Fact]
    public void Null_IsNotEqual()
    {
        var a = Error.NotFound("eq.null", "m");

        a.Equals(null).Should().BeFalse();
        (a == null).Should().BeFalse();
        (null == a).Should().BeFalse();
    }

    [Fact]
    public void NullCodeOrMessage_HashesAndComparesWithoutThrowing()
    {
        // Nullable annotations do not stop a null at runtime (nullable-off callers, an exception whose Message is null).
        var a = Error.NotFound(null!, null!);
        var b = Error.NotFound(null!, null!);

        var hashing = () => a.GetHashCode();

        hashing.Should().NotThrow();
        a.Should().Be(b);
        a.GetHashCode().Should().Be(b.GetHashCode());
        a.Should().NotBe(Error.NotFound("eq.null", "m"));
    }

    [Fact]
    public void ExceptionWithNullMessage_HashesWithoutThrowing()
    {
        var error = Error.FromException(new NullMessageException());

        var hashing = () => error.GetHashCode();

        hashing.Should().NotThrow();
    }

    [Fact]
    public void JsonRoundTrip_IsEqualToTheOriginal()
    {
        var original = Error
            .Validation("eq.json.parent", "p", [Error.NotFound("eq.json.a", "a", Md(("id", "1"))), Error.Conflict("eq.json.b", "b")])
            .Wrap("eq.json.outer", "o");

        var back = JsonSerializer.Deserialize<Error>(JsonSerializer.Serialize(original, JsonOptions), JsonOptions);

        back.Should().Be(original);
        back!.GetHashCode().Should().Be(original.GetHashCode());
    }

    [Fact]
    public void Uninitialized_EqualsADefaultResultsError_AndAnErrorBuiltWithTheSameContents()
    {
        default(Result<int>).TryGetValue(out Error? fromDefault).Should().BeTrue();

        fromDefault.Should().Be(Error.Uninitialized);

        // Equality is by value now, so a hand-built error with the same contents is the same failure.
        Error.Unexpected("result.uninitialized", "The result was never initialized (default value).")
            .Should().Be(Error.Uninitialized);
    }

    private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions().AddFunctional();
}
