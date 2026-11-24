using System.Linq;

using AwesomeAssertions;

using Microsoft.CodeAnalysis;

using Xunit;

namespace StandSure.Functional.Tests;

/// <summary>
/// Review finding 8: <c>Error.Validation(code, message, null)</c> was ambiguous (CS0121) between the metadata and the
/// child-errors overloads. Both give the same plain validation error, so the null binds to the metadata overload.
/// </summary>
public sealed class ValidationNullOverloadTests
{
    [Theory]
    [InlineData("Error.Validation(\"c\", \"m\", null)", "System.Collections.Generic.IReadOnlyDictionary<string, string>?")]
    [InlineData("Error.Validation(\"c\", \"m\", new[] { Error.NotFound(\"a\", \"a\") })", "System.Collections.Generic.IEnumerable<StandSure.Functional.Error>?")]
    [InlineData("Error.Validation(\"c\", \"m\", new System.Collections.Generic.Dictionary<string, string>())", "System.Collections.Generic.IReadOnlyDictionary<string, string>?")]
    public void ThirdArgument_BindsToTheExpectedOverload(string call, string parameterType)
    {
        var source = $$"""
                       using StandSure.Functional;
                       public static class Probe
                       {
                           public static Error Run() => {{call}};
                       }
                       """;

        var compilation = CompileProbe.Create(source);
        compilation.GetDiagnostics(TestContext.Current.CancellationToken).Where(d => d.Severity == DiagnosticSeverity.Error)
            .Should().BeEmpty();

        var tree = compilation.SyntaxTrees.Single();
        var invocation = tree.GetRoot(TestContext.Current.CancellationToken).DescendantNodes()
            .OfType<Microsoft.CodeAnalysis.CSharp.Syntax.InvocationExpressionSyntax>()
            .First(i => i.Expression is Microsoft.CodeAnalysis.CSharp.Syntax.MemberAccessExpressionSyntax { Name.Identifier.ValueText: "Validation" });
        var method = (IMethodSymbol)compilation.GetSemanticModel(tree).GetSymbolInfo(invocation, TestContext.Current.CancellationToken).Symbol!;

        method.Parameters[2].Type.ToDisplayString().Should().Be(parameterType);
    }

    [Fact]
    public void NullThirdArgument_GivesAPlainValidationError()
    {
        var error = Error.Validation("v.null", "m", null);

        error.Kind.Should().Be(ErrorKind.Validation);
        error.Metadata.Should().BeEmpty();
        error.Errors.Should().BeEmpty();
    }
}
