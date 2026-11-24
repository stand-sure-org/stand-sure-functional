using System;
using System.Linq;
using System.Threading.Tasks;

using AwesomeAssertions;

using Xunit;

namespace StandSure.Functional.Tests;

/// <summary>
/// Delegate parameters are non-nullable, so the compiler warns a caller who passes <see langword="null"/>. Callers with
/// nullable checks off can still pass one; the guard turns it into a <c>result.null_delegate</c> failure that names the
/// parameter and the method.
/// </summary>
public sealed class NullDelegateTests
{
    [Theory]
    [InlineData("Result.Success(1).Map<int>(null)")]
    [InlineData("Result.Success(1).Bind<int>(null)")]
    [InlineData("Result.Success(1).Match<int>(null, v => v)")]
    [InlineData("Result.Success(1).Tap(null)")]
    [InlineData("Result.Success(1).Ensure(null, Error.Validation(\"c\", \"m\"))")]
    [InlineData("Option.Some(1).Filter(null)")]
    [InlineData("Option.Some(1).ToResult((System.Func<Error>?)null)")]
    [InlineData("new[] { 1 }.Traverse<int, int>(null)")]
    [InlineData("Try.Run((System.Func<int>?)null)")]
    [InlineData("Task.FromResult(Result.Success(1)).MapAsync((System.Func<int, Task<int>?>?)null)")]
    public void PassingNull_IsACompilerWarning(string call)
    {
        var source = $$"""
                       using System.Threading.Tasks;
                       using StandSure.Functional;
                       public static class Probe
                       {
                           public static object Run() => {{call}};
                       }
                       """;

        var diagnostics = CompileProbe.Create(source).GetDiagnostics(TestContext.Current.CancellationToken);

        diagnostics.Where(d => d.Severity == Microsoft.CodeAnalysis.DiagnosticSeverity.Error).Should().BeEmpty();
        // CS8625: a null literal for a non-nullable parameter; CS8604: a possibly-null argument (the nullable casts, used so
        // the cast itself raises nothing and only the parameter's annotation can produce the warning).
        diagnostics.Should().Contain(d => d.Id == "CS8625" || d.Id == "CS8604", "a null delegate must be flagged at the call site");
    }

    [Fact]
    public void Map_NullDelegate_NamesParameterAndMethod()
    {
        var result = Result.Success(1).Map<int>(null!);

        MessageOf(result).Should().Be("The delegate 'map' passed to Map was null.");
    }

    [Fact]
    public void Traverse_NullDelegate_NamesParameterAndMethod()
    {
        MessageOf(new[] { 1 }.Traverse<int, int>(null!)).Should().Be("The delegate 'map' passed to Traverse was null.");
    }

    [Fact]
    public async Task MapAsync_NullDelegate_NamesParameterAndMethod()
    {
        var result = await Task.FromResult(Result.Success(1)).MapAsync((Func<int, Task<int>?>)null!);

        MessageOf(result).Should().Be("The delegate 'map' passed to MapAsync was null.");
    }

    [Fact]
    public void TryRun_NullDelegate_NamesParameterAndMethod()
    {
        MessageOf(Try.Run((Func<int>)null!)).Should().Be("The delegate 'fn' passed to Run was null.");
    }

    private static string MessageOf<T>(Result<T> result)
    {
        result.TryGetValue(out Error? error).Should().BeTrue();
        error!.Code.Should().Be("result.null_delegate");
        return error.Message;
    }
}
