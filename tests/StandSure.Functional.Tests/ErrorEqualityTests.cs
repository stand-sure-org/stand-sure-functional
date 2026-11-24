using System;
using System.Collections.Generic;

using AwesomeAssertions;

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
}
