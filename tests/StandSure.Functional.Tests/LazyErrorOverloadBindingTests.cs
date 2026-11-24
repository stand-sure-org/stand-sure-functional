using System.Linq;

using AwesomeAssertions;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Xunit;

namespace StandSure.Functional.Tests;

using Xunit.OpenCategories.V3;

/// <summary>Pins which overload each call shape binds to now that Ensure, EnsureAsync and ToResult have factory forms.</summary>
[Category(nameof(LazyErrorOverloadBindingTests))]
public sealed class LazyErrorOverloadBindingTests
{
    [Theory]
    [InlineData("Result.Success(1).Ensure(x => x > 0, x => Error.Validation(\"c\", $\"{x}\"))",
        "(System.Func<int, bool>, System.Func<int, StandSure.Functional.Error>)")]
    [InlineData("Result.Success(1).Ensure(IsPositive, MakeError)", "(System.Func<int, bool>, System.Func<int, StandSure.Functional.Error>)")]
    [InlineData("Result.Success(1).Ensure(x => x > 0, Error.Validation(\"c\", \"m\"))", "(System.Func<int, bool>, StandSure.Functional.Error)")]
    [InlineData("Result.Success(1).Ensure(x => x > 0, null!)", "(System.Func<int, bool>, StandSure.Functional.Error)")]
    [InlineData("Result.Success(1).Ensure(x => x > 0)", "(System.Func<int, bool>, string?)")]
    [InlineData("Option.Some(1).ToResult(() => Error.NotFound(\"c\", \"m\"))", "(System.Func<StandSure.Functional.Error>)")]
    [InlineData("Option.Some(1).ToResult(Error.NotFound(\"c\", \"m\"))", "(StandSure.Functional.Error)")]
    [InlineData("Option.Some(1).ToResult(null!)", "(StandSure.Functional.Error)")]
    [InlineData("Task.FromResult(Result.Success(1)).EnsureAsync(x => x > 0, x => Error.Validation(\"c\", \"m\"))",
        "(System.Threading.Tasks.Task<StandSure.Functional.Result<int>>, System.Func<int, bool>, System.Func<int, StandSure.Functional.Error>)")]
    [InlineData("Task.FromResult(Result.Success(1)).EnsureAsync(async x => { await Task.Yield(); return x > 0; }, x => Error.Validation(\"c\", \"m\"))",
        "(System.Threading.Tasks.Task<StandSure.Functional.Result<int>>, System.Func<int, System.Threading.Tasks.Task<bool>?>, System.Func<int, StandSure.Functional.Error>)")]
    [InlineData("Task.FromResult(Result.Success(1)).EnsureAsync(IsPositive, MakeError)",
        "(System.Threading.Tasks.Task<StandSure.Functional.Result<int>>, System.Func<int, bool>, System.Func<int, StandSure.Functional.Error>)")]
    [InlineData("Task.FromResult(Result.Success(1)).EnsureAsync(x => x > 0, null!)",
        "(System.Threading.Tasks.Task<StandSure.Functional.Result<int>>, System.Func<int, bool>, StandSure.Functional.Error)")]
    public void Call_BindsToExpectedOverload(string call, string parameters)
    {
        var source = $$"""
                       using System.Threading.Tasks;
                       using StandSure.Functional;
                       public static class Probe
                       {
                           private static bool IsPositive(int x) => x > 0;
                           private static Error MakeError(int x) => Error.Validation("c", "m");
                           public static object Run() => {{call}};
                       }
                       """;

        var compilation = CompileProbe.Create(source);
        compilation.GetDiagnostics(TestContext.Current.CancellationToken).Where(d => d.Severity == DiagnosticSeverity.Error).Should().BeEmpty();

        var tree = compilation.SyntaxTrees.Single();

        var invocation = tree.GetRoot(TestContext.Current.CancellationToken).DescendantNodes().OfType<InvocationExpressionSyntax>()
            .First(i => i.Expression is MemberAccessExpressionSyntax { Name.Identifier.ValueText: "Ensure" or "EnsureAsync" or "ToResult" });

        var method = (IMethodSymbol)compilation.GetSemanticModel(tree).GetSymbolInfo(invocation, TestContext.Current.CancellationToken).Symbol!;
        var reduced = method.ReducedFrom is null ? method : method.ReducedFrom.Construct([.. method.TypeArguments]);

        var shown = "(" + string.Join(", ", reduced.Parameters.Select(p => p.Type.ToDisplayString())) + ")";
        shown.Should().Be(parameters);
    }
}
