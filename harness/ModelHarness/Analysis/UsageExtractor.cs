namespace ModelHarness.Analysis;

using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using ModelHarness.Compile;
using ModelHarness.Tasks;

/// <summary>Turns one compiled model answer into <see cref="UsageFacts"/>.</summary>
/// <remarks>
/// Only the model's own syntax tree is analysed: the trees whose path is <see cref="SolutionCompiler.ModelFileName"/>.
/// The fixture and probe trees are never walked and their diagnostics never counted. Works on failed compilations too; where
/// symbols are missing the walker falls back to syntax. Never throws on odd input (returns <see cref="UsageFacts.Empty"/>).
/// </remarks>
public static class UsageExtractor
{
    /// <summary>Extracts facts from a compiled answer.</summary>
    public static UsageFacts Extract(CompileResult result, HarnessTask task)
    {
        ArgumentNullException.ThrowIfNull(result);
        return Extract(result.Compilation, task);
    }

    /// <summary>Extracts facts from a compilation, analysing only its <see cref="SolutionCompiler.ModelFileName"/> trees.</summary>
    public static UsageFacts Extract(CSharpCompilation compilation, HarnessTask task)
    {
        ArgumentNullException.ThrowIfNull(compilation);
        ArgumentNullException.ThrowIfNull(task);

        try
        {
            return ExtractCore(compilation, task);
        }
        catch (Exception ex) when (ex is not OutOfMemoryException and not OperationCanceledException)
        {
            return UsageFacts.Empty;
        }
    }

    private static UsageFacts ExtractCore(CSharpCompilation compilation, HarnessTask task)
    {
        var trees = compilation.SyntaxTrees
            .Where(t => string.Equals(t.FilePath, SolutionCompiler.ModelFileName, StringComparison.Ordinal))
            .ToList();

        var declared = new SortedSet<string>(StringComparer.Ordinal);
        var used = new SortedSet<string>(StringComparer.Ordinal);
        var invoked = new SortedSet<string>(StringComparer.Ordinal);
        int throws = 0, catches = 0, ignored = 0;
        bool tryOut = false, tuple = false, resultLike = false, library = false;

        foreach (var tree in trees)
        {
            var walker = new UsageWalker(compilation.GetSemanticModel(tree));
            walker.Visit(tree.GetRoot());
            declared.UnionWith(walker.DeclaredTypes);
            used.UnionWith(walker.UsedTypes);
            invoked.UnionWith(walker.InvokedMethods);
            throws += walker.ThrowCount;
            catches += walker.CatchCount;
            ignored += walker.IgnoredOutcomeCount;
            tryOut |= walker.BoolTryOut;
            tuple |= walker.TupleReturn;
            resultLike |= walker.ResultLikeType;
            library |= walker.LibraryResult;
        }

        var nonExhaustive = compilation.GetDiagnostics()
            .Count(d => d.Id == "CS8509" && d.Location.SourceTree is { } t && trees.Contains(t));

        var entry = FindEntry(compilation, task, trees);
        var entryType = entry?.ReturnType;
        var entryDisplay = entryType?.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
        var nullable = entryType is not null && IsNullable(OutcomeTypes.Unwrap(entryType));

        var idioms = new List<string>();
        AddIf(idioms, tuple, "tuple-return");
        AddIf(idioms, tryOut, "bool-try-out");
        AddIf(idioms, nullable, "nullable-return");
        AddIf(idioms, throws + catches > 0, "exception");
        AddIf(idioms, resultLike, "result-like-type");
        AddIf(idioms, library, "library-result");

        return new UsageFacts([.. declared], [.. used], [.. invoked], entryDisplay, throws, catches, ignored, nonExhaustive, idioms);
    }

    private static IMethodSymbol? FindEntry(CSharpCompilation compilation, HarnessTask task, List<SyntaxTree> trees)
    {
        var candidates = compilation.GetSymbolsWithName(task.EntryType, SymbolFilter.Type)
            .OfType<INamedTypeSymbol>()
            .Where(t => t.Locations.Any(l => l.SourceTree is { } s && trees.Contains(s)));

        return candidates
                   .SelectMany(t => t.GetMembers(task.EntryMethod).OfType<IMethodSymbol>())
                   .FirstOrDefault(m => m.DeclaredAccessibility == Accessibility.Public)
               ?? candidates.SelectMany(t => t.GetMembers(task.EntryMethod).OfType<IMethodSymbol>()).FirstOrDefault();
    }

    private static bool IsNullable(ITypeSymbol type) =>
        type.TypeKind != TypeKind.Error
        && (type.NullableAnnotation == NullableAnnotation.Annotated
            || type.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T);

    private static void AddIf(List<string> list, bool condition, string tag)
    {
        if (condition)
        {
            list.Add(tag);
        }
    }
}
