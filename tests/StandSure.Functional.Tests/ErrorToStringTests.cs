using System;
using System.Collections.Generic;

using AwesomeAssertions;

using Xunit;

namespace StandSure.Functional.Tests;

/// <summary>
/// DESIGN v1.4: <c>Error.ToString()</c> follows the <c>EmitErrorDetails</c> switch. A logged <c>{error}</c> leaves the
/// process as an exported span does, so message and metadata values print only when the switch is on.
/// </summary>
[Collection(TelemetryCollection.Name)]
public sealed class ErrorToStringTests : IDisposable
{
    private const string DetailsSwitch = "StandSure.Functional.EmitErrorDetails";

    public void Dispose() => AppContext.SetSwitch(DetailsSwitch, false);

    [Fact]
    public void SwitchOff_PrintsKindCodeAndMetadataKeysOnly()
    {
        AppContext.SetSwitch(DetailsSwitch, false);
        var error = Error.NotFound("ts.missing", "secret message", new Dictionary<string, string> { ["id"] = "secret-id" });

        error.ToString().Should().Be("Error { Kind = NotFound, Code = ts.missing, Metadata = [id], Errors = 0 }");
    }

    [Fact]
    public void SwitchOn_AlsoPrintsMessageAndMetadataValues()
    {
        AppContext.SetSwitch(DetailsSwitch, true);
        var error = Error.NotFound("ts.missing", "visible message", new Dictionary<string, string> { ["id"] = "7" });

        error.ToString().Should()
            .Be("Error { Kind = NotFound, Code = ts.missing, Message = visible message, Metadata = [id=7], Errors = 0 }");
    }

    [Fact]
    public void Inner_IsPrintedUnderTheSameRule()
    {
        AppContext.SetSwitch(DetailsSwitch, false);
        var error = Error.NotFound("ts.inner", "secret inner").Wrap("ts.outer", "secret outer");

        var text = error.ToString();

        text.Should().Be(
            "Error { Kind = NotFound, Code = ts.outer, Metadata = [], Errors = 0, Inner = Error { Kind = NotFound, Code = ts.inner, Metadata = [], Errors = 0 } }");
    }

    [Fact]
    public void Children_PrintAsACount()
    {
        AppContext.SetSwitch(DetailsSwitch, false);
        var error = Error.Validation("ts.parent", "p", [Error.NotFound("ts.a", "a"), Error.NotFound("ts.b", "b")]);

        error.ToString().Should().Contain("Errors = 2").And.NotContain("ts.a");
    }

    [Fact]
    public void Exception_PrintsItsTypeOnly()
    {
        AppContext.SetSwitch(DetailsSwitch, false);
        var exception = new InvalidOperationException("connection string with password");

        var text = Error.FromException(exception).ToString();

        text.Should().Contain("Exception = System.InvalidOperationException");
        text.Should().NotContain("connection string with password");
    }

    [Fact]
    public void Exception_WithTheSwitchOn_ShowsTheErrorsMessageButNotTheExceptionItself()
    {
        AppContext.SetSwitch(DetailsSwitch, true);
        var exception = new InvalidOperationException("boom");

        var text = Error.FromException(exception).ToString();

        // FromException copies the exception message into Message, which the switch allows; the exception's own
        // ToString (type, message and stack) is never printed.
        text.Should().Contain("Message = boom").And.Contain("Exception = System.InvalidOperationException");
        text.Should().NotContain(exception.ToString());
    }
}
