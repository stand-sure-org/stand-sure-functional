using System;
using System.Collections.Generic;
using AwesomeAssertions;
using Xunit;

namespace StandSure.Functional.Tests;

public sealed class ErrorTests
{
    public static TheoryData<ErrorKind, Func<string, string, Error>> Factories => new()
    {
        { ErrorKind.Validation, (c, m) => Error.Validation(c, m) },
        { ErrorKind.NotFound, (c, m) => Error.NotFound(c, m) },
        { ErrorKind.Conflict, (c, m) => Error.Conflict(c, m) },
        { ErrorKind.Unauthorized, (c, m) => Error.Unauthorized(c, m) },
        { ErrorKind.Forbidden, (c, m) => Error.Forbidden(c, m) },
        { ErrorKind.Unavailable, (c, m) => Error.Unavailable(c, m) },
        { ErrorKind.Unexpected, (c, m) => Error.Unexpected(c, m) },
    };

    [Theory]
    [MemberData(nameof(Factories))]
    public void Factory_SetsKindCodeMessage(ErrorKind kind, Func<string, string, Error> factory)
    {
        var error = factory("some.code", "some message");

        error.Kind.Should().Be(kind);
        error.Code.Should().Be("some.code");
        error.Message.Should().Be("some message");
    }

    [Fact]
    public void Factory_KeepsSuppliedMetadata()
    {
        var metadata = new Dictionary<string, string> { ["k"] = "v" };

        Error.NotFound("c", "m", metadata).Metadata.Should().Contain("k", "v");
    }

    [Fact]
    public void Metadata_DefaultsToEmpty()
    {
        Error.Validation("c", "m").Metadata.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void Errors_DefaultsToEmpty()
    {
        Error.Validation("c", "m").Errors.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void Wrap_KeepsKind_SetsInner()
    {
        var inner = Error.NotFound("inner", "inner message");

        var wrapped = inner.Wrap("outer", "outer message");

        wrapped.Kind.Should().Be(ErrorKind.NotFound);
        wrapped.Code.Should().Be("outer");
        wrapped.Message.Should().Be("outer message");
        wrapped.Inner.Should().BeSameAs(inner);
    }

    [Fact]
    public void Is_FindsCodeThreeLevelsDeep()
    {
        var error = Error.Conflict("a", "m").Wrap("b", "m").Wrap("c", "m");

        error.Is("a").Should().BeTrue();
        error.Is("b").Should().BeTrue();
        error.Is("c").Should().BeTrue();
    }

    [Fact]
    public void Is_FalseForUnrelatedCode()
    {
        Error.Conflict("a", "m").Wrap("b", "m").Is("zzz").Should().BeFalse();
    }

    [Fact]
    public void FromException_MapsTypeAndMessage()
    {
        var original = new InvalidOperationException("boom");

        var error = Error.FromException(original);

        error.Kind.Should().Be(ErrorKind.Unexpected);
        error.Code.Should().Be("exception.unhandled");
        error.Metadata.Should().Contain("exception.type", typeof(InvalidOperationException).FullName!);
        error.Metadata.Should().Contain("exception.message", "boom");
        error.Exception.Should().BeSameAs(original);
    }

    [Fact]
    public void Uninitialized_HasExpectedCode()
    {
        Error.Uninitialized.Kind.Should().Be(ErrorKind.Unexpected);
        Error.Uninitialized.Code.Should().Be("result.uninitialized");
        Error.Uninitialized.Should().BeSameAs(Error.Uninitialized);
    }

    [Fact]
    public void ValidationParent_HoldsChildren()
    {
        var a = Error.Validation("a", "m");
        var b = Error.Validation("b", "m");

        var parent = Error.Validation("parent", "many", [a, b]);

        parent.Kind.Should().Be(ErrorKind.Validation);
        parent.Errors.Should().Equal(a, b);
    }
}
