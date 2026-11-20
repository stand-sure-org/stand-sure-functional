using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AwesomeAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace StandSure.Functional.Tests;

public sealed class OptionNullableConversionTests
{
    [Fact]
    public void NullableValues_ConvertToOption_WithoutNullableWarnings()
    {
        const string source = """
            using StandSure.Functional;
            public static class Probe
            {
                public static void Run()
                {
                    string? s = null;
                    Option<string> a = s;
                    var b = Option.Some(s);
                    var d = new Option<string>(s);
                    int? n = null;
                    Option<int?> c = n;
                }
            }
            """;

        var diagnostics = Compile(source);

        diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error).Should().BeEmpty();
        diagnostics.Select(d => d.Id).Where(id => id.StartsWith("CS86", StringComparison.Ordinal)).Should().BeEmpty();
    }

    [Fact]
    public void NullValues_BecomeNone()
    {
        string? s = null;
        Option<string> a = s!;

        a.IsNone.Should().BeTrue();
        Option.Some(s).IsNone.Should().BeTrue();
    }

    private static IReadOnlyList<Diagnostic> Compile(string source)
    {
        var tpa = (string)(AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") ?? string.Empty);
        var references = tpa
            .Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries)
            .Append(typeof(Result<>).Assembly.Location)
            .Distinct(StringComparer.Ordinal)
            .Select(path => MetadataReference.CreateFromFile(path));

        var compilation = CSharpCompilation.Create(
            "NullableProbe",
            [CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(LanguageVersion.Preview))],
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, nullableContextOptions: NullableContextOptions.Enable));

        return compilation.GetDiagnostics();
    }
}
