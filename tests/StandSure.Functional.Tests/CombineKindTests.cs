using System.Linq;

using AwesomeAssertions;

using Xunit;

namespace StandSure.Functional.Tests;

/// <summary>
/// The kind of the parent error from <c>Combine</c> and <c>Traverse</c>. Children that agree keep their kind; a mix takes
/// <see cref="ErrorKind.Unexpected"/>, then <see cref="ErrorKind.Unavailable"/>, if present, and otherwise
/// <see cref="ErrorKind.Validation"/>. A batch of outages must not reach the caller's kind switch as a 400.
/// </summary>
public sealed class CombineKindTests
{
    private static Error Of(ErrorKind kind, int i) => kind switch
    {
        ErrorKind.Validation => Error.Validation($"kind.validation.{i}", "v"),
        ErrorKind.NotFound => Error.NotFound($"kind.notfound.{i}", "n"),
        ErrorKind.Conflict => Error.Conflict($"kind.conflict.{i}", "c"),
        ErrorKind.Unauthorized => Error.Unauthorized($"kind.unauthorized.{i}", "u"),
        ErrorKind.Forbidden => Error.Forbidden($"kind.forbidden.{i}", "f"),
        ErrorKind.Unavailable => Error.Unavailable($"kind.unavailable.{i}", "a"),
        _ => Error.Unexpected($"kind.unexpected.{i}", "x"),
    };

    public static TheoryData<ErrorKind[], ErrorKind> Cases => new()
    {
        { [ErrorKind.Unavailable, ErrorKind.Unavailable], ErrorKind.Unavailable },
        { [ErrorKind.NotFound, ErrorKind.NotFound], ErrorKind.NotFound },
        { [ErrorKind.Forbidden], ErrorKind.Forbidden },
        { [ErrorKind.Validation, ErrorKind.Validation], ErrorKind.Validation },
        { [ErrorKind.Validation, ErrorKind.Unavailable], ErrorKind.Unavailable },
        { [ErrorKind.NotFound, ErrorKind.Unavailable, ErrorKind.Unexpected], ErrorKind.Unexpected },
        { [ErrorKind.Unexpected, ErrorKind.Conflict], ErrorKind.Unexpected },
        { [ErrorKind.NotFound, ErrorKind.Conflict], ErrorKind.Validation },
        { [ErrorKind.Unauthorized, ErrorKind.Forbidden, ErrorKind.Validation], ErrorKind.Validation },
    };

    [Theory]
    [MemberData(nameof(Cases))]
    public void Combine_ParentKind_FollowsTheChildren(ErrorKind[] kinds, ErrorKind expected)
    {
        var results = kinds.Select((k, i) => Result.Failure<int>(Of(k, i))).Append(Result.Success(1));

        var combined = results.Combine();

        combined.TryGetValue(out Error? error).Should().BeTrue();
        error!.Kind.Should().Be(expected);
        error.Code.Should().Be("result.combine_failed");
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void Traverse_ParentKind_FollowsTheChildren(ErrorKind[] kinds, ErrorKind expected)
    {
        var traversed = Enumerable.Range(0, kinds.Length).Traverse(i => Result.Failure<int>(Of(kinds[i], i)));

        traversed.TryGetValue(out Error? error).Should().BeTrue();
        error!.Kind.Should().Be(expected);
    }

    [Fact]
    public void Combine_DefaultElementAmongValidationFailures_ReportsUnexpected()
    {
        var combined = new[] { Result.Failure<int>(Of(ErrorKind.Validation, 0)), default }.Combine();

        combined.TryGetValue(out Error? error).Should().BeTrue();
        error!.Kind.Should().Be(ErrorKind.Unexpected, "a default result is an Unexpected failure");
    }
}
