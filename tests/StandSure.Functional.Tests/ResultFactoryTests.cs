using AwesomeAssertions;
using Xunit;

namespace StandSure.Functional.Tests;

public sealed class ResultFactoryTests
{
    private static readonly Error Boom = Error.NotFound("thing", "missing");

    [Fact]
    public void Success_CreatesSuccessCarryingValue()
    {
        var r = Result.Success(42);

        r.IsSuccess.Should().BeTrue();
        r.TryGetValue(out int value).Should().BeTrue();
        value.Should().Be(42);
    }

    [Fact]
    public void Success_OfErrorTypedValue_IsStillSuccess()
    {
        var r = Result.Success<object>(Boom);

        r.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Failure_CreatesFailureCarryingError()
    {
        var r = Result.Failure<int>(Boom);

        r.IsFailure.Should().BeTrue();
        r.TryGetValue(out Error? error).Should().BeTrue();
        error.Should().BeSameAs(Boom);
    }

    [Fact]
    public void Failure_WithNullError_BecomesUninitialized()
    {
        var r = Result.Failure<int>(null!);

        r.IsFailure.Should().BeTrue();
        r.TryGetValue(out Error? error).Should().BeTrue();
        error.Should().BeSameAs(Error.Uninitialized);
    }

    [Fact]
    public void Success_WithoutValue_IsSuccessOfUnit()
    {
        var r = Result.Success();

        r.IsSuccess.Should().BeTrue();
        r.TryGetValue(out Unit unit).Should().BeTrue();
        unit.Should().Be(Unit.Value);
    }

    [Fact]
    public void OkAlias_ReturnsSameAsSuccess()
    {
        Result.Ok(7).Should().Be(Result.Success(7));
        Result.Ok().Should().Be(Result.Success());
    }

    [Fact]
    public void FailAlias_ReturnsSameAsFailure()
    {
        Result.Fail<int>(Boom).Should().Be(Result.Failure<int>(Boom));
    }
}
