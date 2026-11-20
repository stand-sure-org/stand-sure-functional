using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace StandSure.Functional.Tests;

/// <summary>Compiles a snippet against the library in memory (nullable on) to check overload binding and diagnostics.</summary>
internal static class CompileProbe
{
    public static CSharpCompilation Create(string source)
    {
        var tpa = (string)(AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") ?? string.Empty);

        var references = tpa
            .Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries)
            .Append(typeof(Result<>).Assembly.Location)
            .Distinct(StringComparer.Ordinal)
            .Select(path => MetadataReference.CreateFromFile(path));

        return CSharpCompilation.Create(
            "CompileProbe",
            [CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(LanguageVersion.Preview))],
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, nullableContextOptions: NullableContextOptions.Enable));
    }

    public static IReadOnlyList<Diagnostic> Errors(string source) =>
        Create(source).GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error).ToList();
}
