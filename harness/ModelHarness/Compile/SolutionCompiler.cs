namespace ModelHarness.Compile;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using ModelHarness.Compilation;
using ModelHarness.Tasks;

/// <summary>Compiles model code together with a task's fixture and probe into an in-memory assembly. Never throws for bad model input.</summary>
/// <remarks>
/// Options: dynamically linked library, nullable enabled, default language version, Release optimisation, no implicit or global
/// usings (the task prompts say "Implicit usings are off", so the model must write its own). References are
/// <see cref="ReferenceAssemblies.All"/> plus the built <c>StandSure.Functional.dll</c> when <c>withLibrary</c> is set.
/// Top-level types (also inside namespaces) the model declares with a name the fixture or probe also declares are removed
/// from the model's tree, except <c>Solution</c>.
/// </remarks>
public static class SolutionCompiler
{
    /// <summary>The syntax-tree path of the model's code; the fixture and probe trees use other paths (<c>fixture.cs</c>, <c>probe.cs</c>).</summary>
    public const string ModelFileName = "model.cs";

    private const string EntryType = "Solution";

    private static readonly Lazy<IReadOnlyList<MetadataReference>> Framework =
        new(() => [.. ReferenceAssemblies.All().Select(p => (MetadataReference)MetadataReference.CreateFromFile(p))]);

    private static readonly Lazy<MetadataReference> Library =
        new(() => MetadataReference.CreateFromFile(LibraryPath));

    /// <summary>Path of the <c>StandSure.Functional.dll</c> the harness is built against (project reference).</summary>
    public static string LibraryPath => typeof(StandSure.Functional.Unit).Assembly.Location;

    /// <summary>Compiles <paramref name="code"/> with the task's fixture and probe.</summary>
    public static CompileResult Compile(HarnessTask task, string code, bool withLibrary)
    {
        ArgumentNullException.ThrowIfNull(task);

        var parseOptions = CSharpParseOptions.Default;
        var fixtureTree = CSharpSyntaxTree.ParseText(task.FixtureSource, parseOptions, "fixture.cs");
        var probeTree = CSharpSyntaxTree.ParseText(task.ProbeSource, parseOptions, "probe.cs");
        var reserved = new HashSet<string>(StringComparer.Ordinal);
        foreach (var name in TopLevelTypeNames(fixtureTree.GetRoot()).Concat(TopLevelTypeNames(probeTree.GetRoot())))
        {
            reserved.Add(name);
        }

        reserved.Remove(EntryType);

        var modelRoot = CSharpSyntaxTree.ParseText(code ?? string.Empty, parseOptions, ModelFileName).GetRoot();
        var doomed = TopLevelTypes(modelRoot).Where(t => reserved.Contains(NameOf(t))).ToList();
        var stripped = doomed.Select(NameOf).Distinct(StringComparer.Ordinal).ToList();
        if (doomed.Count > 0)
        {
            modelRoot = modelRoot.RemoveNodes(doomed, SyntaxRemoveOptions.KeepNoTrivia) ?? modelRoot;
        }

        var modelTree = CSharpSyntaxTree.Create((CSharpSyntaxNode)modelRoot, parseOptions, ModelFileName);
        var references = withLibrary ? [.. Framework.Value, Library.Value] : Framework.Value;
        var compilation = CSharpCompilation.Create(
            "ModelSolution_" + Guid.NewGuid().ToString("N"),
            [modelTree, fixtureTree, probeTree],
            references,
            new CSharpCompilationOptions(
                OutputKind.DynamicallyLinkedLibrary,
                optimizationLevel: OptimizationLevel.Release,
                nullableContextOptions: NullableContextOptions.Enable));

        using var stream = new MemoryStream();
        var emit = compilation.Emit(stream);
        var errors = emit.Diagnostics
            .Where(d => d.Severity == DiagnosticSeverity.Error)
            .Select(Format)
            .ToList();
        var success = emit.Success && errors.Count == 0;
        return new CompileResult(success, errors, success ? stream.ToArray() : null, compilation, stripped);
    }

    private static string Format(Diagnostic diagnostic)
    {
        var span = diagnostic.Location.GetLineSpan();
        var where = diagnostic.Location.IsInSource
            ? $" [{span.Path}({span.StartLinePosition.Line + 1},{span.StartLinePosition.Character + 1})]"
            : string.Empty;
        return $"{diagnostic.Id}: {diagnostic.GetMessage()}{where}";
    }

    private static IEnumerable<string> TopLevelTypeNames(SyntaxNode root) => TopLevelTypes(root).Select(NameOf);

    private static string NameOf(MemberDeclarationSyntax member) => member switch
    {
        BaseTypeDeclarationSyntax type => type.Identifier.ValueText,
        DelegateDeclarationSyntax @delegate => @delegate.Identifier.ValueText,
        _ => string.Empty,
    };

    private static IEnumerable<MemberDeclarationSyntax> TopLevelTypes(SyntaxNode root)
    {
        if (root is not CompilationUnitSyntax unit)
        {
            yield break;
        }

        foreach (var member in Members(unit.Members))
        {
            yield return member;
        }
    }

    private static IEnumerable<MemberDeclarationSyntax> Members(SyntaxList<MemberDeclarationSyntax> members)
    {
        foreach (var member in members)
        {
            if (member is BaseNamespaceDeclarationSyntax ns)
            {
                foreach (var inner in Members(ns.Members))
                {
                    yield return inner;
                }
            }
            else if (member is BaseTypeDeclarationSyntax or DelegateDeclarationSyntax)
            {
                yield return member;
            }
        }
    }
}
