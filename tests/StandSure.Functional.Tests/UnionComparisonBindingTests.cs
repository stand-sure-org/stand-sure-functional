using System.Linq;

using AwesomeAssertions;

using Microsoft.CodeAnalysis;

using Xunit;

namespace StandSure.Functional.Tests;

/// <summary>
/// The implicit case conversions meet the new <c>==</c> operators: comparing a union with a raw value converts the value
/// first, and comparing with <see langword="null"/> does not compile, because both unions are non-nullable structs.
/// </summary>
public sealed class UnionComparisonBindingTests
{
    [Theory]
    [InlineData("Option<int> o = 5; return o == 5;")]
    [InlineData("Option<string> o = \"a\"; return o == None.Instance;")]
    [InlineData("Result<int> r = 5; return r == 5;")]
    [InlineData("Result<int> r = 5; return r == Error.NotFound(\"c\", \"m\");")]
    public void ComparingWithACaseValue_Compiles(string body)
    {
        Diagnostics(body).Should().BeEmpty();
    }

    [Theory]
    [InlineData("Option<string> o = \"a\"; return o == null;")]
    [InlineData("Option<int?> o = 5; return o == null;")]
    [InlineData("Result<string> r = \"a\"; return r == null;")]
    [InlineData("Result<string?> r = \"a\"; return r == null;")]
    public void ComparingWithNull_IsACompileError(string body)
    {
        Diagnostics(body).Should().ContainSingle(d => d.Id == "CS0037");
    }

    [Fact]
    public void RawValueComparison_ConvertsFirst_ButEqualsObjectDoesNot()
    {
        (Result.Success(5) == 5).Should().BeTrue();
        (Option.Some(5) == 5).Should().BeTrue();

        // Equals(object) sees a boxed int, not a Result<int>: the usual split for a type with implicit conversions.
        Result.Success(5).Equals((object)5).Should().BeFalse();
        Option.Some(5).Equals((object)5).Should().BeFalse();
    }

    private static Diagnostic[] Diagnostics(string body)
    {
        var source = "using StandSure.Functional; public static class Probe { public static bool Run() { " + body + " } }";

        return [.. CompileProbe.Create(source).GetDiagnostics(TestContext.Current.CancellationToken)
            .Where(d => d.Severity >= DiagnosticSeverity.Warning)];
    }
}
