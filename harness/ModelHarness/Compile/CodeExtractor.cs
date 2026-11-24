namespace ModelHarness.Compile;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

/// <summary>Pulls C# source out of messy model markdown. Never throws.</summary>
/// <remarks>
/// Fences labelled <c>csharp</c>, <c>cs</c>, <c>c#</c> (case-insensitive, only the first word of the info string counts)
/// or unlabelled are taken; fences with any other label are skipped. An unterminated final fence runs to the end of the text.
/// A non-partial type redeclared by a later fence is removed from earlier fences (models often answer, then post a
/// revised version or a corrected type); an earlier fence left with no declarations is dropped. Partial types across
/// fences are a planned split and are kept.
/// The remaining fences are concatenated in order. Because C# requires <c>using</c> directives before members, top-level
/// <c>using</c> directives of a concatenation are hoisted to the top, de-duplicated by text; everything else (duplicate
/// types included) is left to the compiler. With no fence at all, the raw text counts only when it parses with no syntax errors.
/// </remarks>
public static class CodeExtractor
{
    /// <summary>Extracts C# from <paramref name="modelText"/>.</summary>
    public static ExtractResult Extract(string modelText)
    {
        if (string.IsNullOrWhiteSpace(modelText))
        {
            return new ExtractResult(null, ExtractStatus.ExtractFailed);
        }

        var fences = ReadFences(modelText);

        if (fences.Count == 0)
        {
            return fences.SawFence || !ParsesCleanly(modelText)
                ? new ExtractResult(null, ExtractStatus.ExtractFailed)
                : new ExtractResult(modelText, ExtractStatus.Single);
        }

        if (fences.Count == 1)
        {
            return new ExtractResult(fences[0], ExtractStatus.Single);
        }

        var (kept, revised) = DropSuperseded(fences);

        if (kept.Count == 1)
        {
            return new ExtractResult(kept[0], revised ? ExtractStatus.Revised : ExtractStatus.Single);
        }

        return new ExtractResult(Concatenate(kept), revised ? ExtractStatus.Revised : ExtractStatus.Concatenated);
    }

    // A type that a later fence redeclares is removed from every earlier fence; an earlier fence left with no
    // declarations is dropped. The rest of each earlier fence (other types, usings) stays.
    private static (List<string> Fences, bool Revised) DropSuperseded(List<string> fences)
    {
        var kept = new List<string>();
        var revised = false;

        foreach (var fence in fences)
        {
            var redeclared = DeclaredTypes(CSharpSyntaxTree.ParseText(fence).GetCompilationUnitRoot()).Select(t => t.Key).ToHashSet(StringComparer.Ordinal);

            for (var i = kept.Count - 1; i >= 0; i--)
            {
                var root = CSharpSyntaxTree.ParseText(kept[i]).GetCompilationUnitRoot();
                var superseded = DeclaredTypes(root).Where(t => redeclared.Contains(t.Key)).Select(t => t.Node).ToList();

                if (superseded.Count == 0)
                {
                    continue;
                }

                revised = true;
                var remaining = root.RemoveNodes(superseded, SyntaxRemoveOptions.KeepNoTrivia)!;

                if (remaining.DescendantNodes().OfType<MemberDeclarationSyntax>().Any(m => m is not BaseNamespaceDeclarationSyntax))
                {
                    kept[i] = remaining.ToFullString();
                }
                else
                {
                    kept.RemoveAt(i);
                }
            }

            kept.Add(fence);
        }

        return (kept, revised);
    }

    // Non-partial types declared at the top level or directly in a namespace, keyed by namespace, name and arity.
    private static IEnumerable<(string Key, MemberDeclarationSyntax Node)> DeclaredTypes(CompilationUnitSyntax root)
    {
        foreach (var member in root.DescendantNodes().OfType<MemberDeclarationSyntax>())
        {
            if (member.Parent is not (CompilationUnitSyntax or BaseNamespaceDeclarationSyntax)
                || member.Modifiers.Any(SyntaxKind.PartialKeyword))
            {
                continue;
            }

            var name = member switch
            {
                BaseTypeDeclarationSyntax type => $"{type.Identifier.ValueText}`{(type as TypeDeclarationSyntax)?.Arity ?? 0}",
                DelegateDeclarationSyntax d => $"{d.Identifier.ValueText}`{d.Arity}",
                _ => null,
            };

            if (name is not null)
            {
                var ns = string.Join('.', member.Ancestors().OfType<BaseNamespaceDeclarationSyntax>().Reverse().Select(n => n.Name.ToString()));
                yield return ($"{ns}:{name}", member);
            }
        }
    }

    private static bool ParsesCleanly(string text)
    {
        var tree = CSharpSyntaxTree.ParseText(text);
        return !tree.GetDiagnostics().Any(d => d.Severity == DiagnosticSeverity.Error);
    }

    private static string Concatenate(List<string> fences)
    {
        var usings = new List<string>();
        var seen = new HashSet<string>(StringComparer.Ordinal);
        var bodies = new List<string>();

        foreach (var fence in fences)
        {
            var root = CSharpSyntaxTree.ParseText(fence).GetCompilationUnitRoot();

            foreach (var directive in root.Usings)
            {
                var text = directive.ToString().Trim();

                if (seen.Add(text))
                {
                    usings.Add(text);
                }
            }

            bodies.Add(root.Usings.Count == 0 ? fence : root.RemoveNodes(root.Usings, SyntaxRemoveOptions.KeepNoTrivia)!.ToFullString());
        }

        var builder = new StringBuilder();

        foreach (var directive in usings)
        {
            builder.Append(directive).Append('\n');
        }

        foreach (var body in bodies)
        {
            builder.Append('\n').Append(body.Trim('\n', '\r')).Append('\n');
        }

        return builder.ToString();
    }

    private static Fences ReadFences(string text)
    {
        var fences = new Fences();
        var lines = text.Split('\n');

        for (var i = 0; i < lines.Length; i++)
        {
            var (marker, length, info) = ParseOpening(lines[i].TrimEnd('\r'));

            if (length == 0)
            {
                continue;
            }

            fences.SawFence = true;
            var body = new List<string>();

            for (i++; i < lines.Length; i++)
            {
                var line = lines[i].TrimEnd('\r');

                if (IsClosing(line, marker, length))
                {
                    break;
                }

                body.Add(line);
            }

            var label = info.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? string.Empty;
            var content = string.Join('\n', body);

            if (IsCSharpLabel(label) && !string.IsNullOrWhiteSpace(content))
            {
                fences.Add(content);
            }
        }

        return fences;
    }

    private static bool IsCSharpLabel(string label) =>
        label.Length == 0
        || label.Equals("csharp", StringComparison.OrdinalIgnoreCase)
        || label.Equals("cs", StringComparison.OrdinalIgnoreCase)
        || label.Equals("c#", StringComparison.OrdinalIgnoreCase);

    private static (char Marker, int Length, string Info) ParseOpening(string line)
    {
        var trimmed = line.TrimStart();

        if (line.Length - trimmed.Length > 3 || trimmed.Length < 3 || (trimmed[0] != '`' && trimmed[0] != '~'))
        {
            return default;
        }

        var marker = trimmed[0];
        var length = 0;

        while (length < trimmed.Length && trimmed[length] == marker)
        {
            length++;
        }

        var info = trimmed[length..].Trim();
        return length < 3 || (marker == '`' && info.Contains('`', StringComparison.Ordinal)) ? default : (marker, length, info);
    }

    private static bool IsClosing(string line, char marker, int openLength)
    {
        var trimmed = line.Trim();
        return trimmed.Length >= openLength && trimmed.All(c => c == marker);
    }

    private sealed class Fences : List<string>
    {
        public bool SawFence { get; set; }
    }
}
