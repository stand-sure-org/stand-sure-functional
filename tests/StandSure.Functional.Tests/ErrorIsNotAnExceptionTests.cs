using System.Linq;

using AwesomeAssertions;

using Microsoft.CodeAnalysis;

using Xunit;

namespace StandSure.Functional.Tests;

/// <summary>
/// Pins the API summary's trap line: <see cref="Error"/> is not an exception, so matching it against an exception type
/// does not compile (CS8121, the harness's most common devstral failure), and the caught exception is
/// <see cref="Error.Exception"/>.
/// </summary>
public sealed class ErrorIsNotAnExceptionTests
{
    private const string Prefix = """
                                  using System.IO;
                                  using StandSure.Functional;
                                  public static class Probe
                                  {
                                      public static object Run(Error e) =>
                                  """;

    [Theory]
    [InlineData("e is IOException x ? x.Message : e.Message")]
    [InlineData("e switch { IOException x => x.Message, _ => e.Message }")]
    public void MatchingAnErrorAgainstAnExceptionType_DoesNotCompile(string expression)
    {
        var errors = CompileProbe.Errors($"{Prefix} {expression};\n}}");

        errors.Select(d => d.Id).Should().Contain("CS8121");
    }

    // A plain type test compiles (Error is sealed, so the compiler only warns CS0184) and is always false.
    [Fact]
    public void TypeTestingAnErrorAgainstAnExceptionType_IsAlwaysFalse()
    {
        var diagnostics = CompileProbe.Create($"{Prefix} e is IOException;\n}}").GetDiagnostics(TestContext.Current.CancellationToken);

        diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error).Should().BeEmpty();
        diagnostics.Select(d => d.Id).Should().Contain("CS0184");
    }

    [Fact]
    public void MatchingErrorExceptionAgainstAnExceptionType_Compiles()
    {
        var errors = CompileProbe.Errors(
            $"{Prefix} e.Exception is IOException x ? Error.Unavailable(\"disk.unavailable\", x.Message) : e;\n}}");

        errors.Should().BeEmpty();
    }
}
