namespace ModelHarness.Tests;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

using AwesomeAssertions;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using ModelHarness.Tasks;

/// <summary>Guards <c>docs/llm-api-summary.md</c>, the only text the with-library condition gives models about the library.</summary>
public sealed partial class ApiSummaryTests
{
    private const int MaxLines = 130;

    private static readonly string RepoRoot = RepoLocator.FindRoot(AppContext.BaseDirectory);
    private static readonly string SummaryPath = Path.Combine(RepoRoot, "docs", "llm-api-summary.md");

    // Words from task-id slugs and fixture type names that are generic programming vocabulary, not task domain.
    private static readonly HashSet<string> GenericWords = new(StringComparer.OrdinalIgnoreCase)
    {
        "create", "get", "place", "register", "import", "async", "layers", "endpoint", "I", "Store", "Source", "Exception",
        "Client", "Repository", "Directory", "Queries", "Cache", "Table", "Row", "Line", "Book", "Form", "Summary", "Period",
        "Unavailable", "Declined", "Duplicate",
    };

    [Fact]
    public void Summary_Exists()
    {
        File.Exists(SummaryPath).Should().BeTrue();
    }

    [Fact]
    public void Summary_IsOnePage()
    {
        File.ReadAllLines(SummaryPath).Length.Should().BeLessThanOrEqualTo(MaxLines);
    }

    [Fact]
    public void Summary_NeverUsesHiddenOkFailAliases()
    {
        HiddenAlias().Matches(File.ReadAllText(SummaryPath)).Should().BeEmpty();
    }

    [Fact]
    public void Summary_CodeFencesAreLabelledCsharp()
    {
        var opening = true;

        foreach (var line in File.ReadAllLines(SummaryPath))
        {
            if (!line.StartsWith("```", StringComparison.Ordinal))
            {
                continue;
            }

            if (opening)
            {
                line.Should().Be("```csharp");
            }

            opening = !opening;
        }

        opening.Should().BeTrue("every code fence should be closed");
    }

    [Fact]
    public void Summary_EveryMatchUsesNamedArguments()
    {
        var matches = new List<InvocationExpressionSyntax>();

        foreach (var block in CsharpBlocks(File.ReadAllText(SummaryPath)))
        {
            var root = CSharpSyntaxTree.ParseText(block,
                    new CSharpParseOptions(kind: SourceCodeKind.Script),
                    cancellationToken: TestContext.Current.CancellationToken)
                .GetRoot(TestContext.Current.CancellationToken);

            foreach (var invocation in root.DescendantNodes())
            {
                if (invocation is InvocationExpressionSyntax { Expression: MemberAccessExpressionSyntax member } call
                    && member.Name.Identifier.ValueText is "Match" or "MatchAsync")
                {
                    matches.Add(call);
                }
            }
        }

        matches.Should().HaveCountGreaterThanOrEqualTo(3, "the summary should show Match with named arguments");

        foreach (var call in matches)
        {
            call.ArgumentList.Arguments.Should()
                .AllSatisfy(a => a.NameColon.Should().NotBeNull("failure-first Match needs named arguments: {0}", call.ToString()));
        }
    }

    [Fact]
    public void Summary_NamesNoTaskDomain()
    {
        // The summary is generic: an example in a task's own domain would hand the with-library condition a worked answer.
        var summary = File.ReadAllText(SummaryPath);
        var leaked = new SortedSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var word in TaskDomainWords())
        {
            if (!GenericWords.Contains(word) && Regex.IsMatch(summary, $@"\b{Regex.Escape(word)}\b", RegexOptions.IgnoreCase))
            {
                leaked.Add(word);
            }
        }

        leaked.Should().BeEmpty("the summary must not use task domain words, found: {0}", string.Join(", ", leaked));
    }

    private static IEnumerable<string> TaskDomainWords()
    {
        foreach (var task in TaskCatalog.Load(Path.Combine(RepoRoot, "harness", "tasks")))
        {
            foreach (var word in task.Id.Split('-')[1..])
            {
                yield return word;
            }

            foreach (var source in new[] { task.FixtureSource, task.ProbeSource })
            {
                var root = CSharpSyntaxTree.ParseText(source, cancellationToken: TestContext.Current.CancellationToken)
                    .GetRoot(TestContext.Current.CancellationToken);

                foreach (var type in root.DescendantNodes().OfType<BaseTypeDeclarationSyntax>())
                {
                    var name = type.Identifier.ValueText;

                    if (name == "HarnessProbe" || name.StartsWith("Probe", StringComparison.Ordinal))
                    {
                        continue;
                    }

                    yield return name;

                    foreach (Match part in PascalWord().Matches(name))
                    {
                        yield return part.Value;
                    }
                }
            }
        }
    }

    private static IEnumerable<string> CsharpBlocks(string markdown)
    {
        foreach (Match block in CsharpFence().Matches(markdown.Replace("\r\n", "\n", StringComparison.Ordinal)))
        {
            yield return block.Groups["code"].Value;
        }
    }

    [GeneratedRegex("[A-Z][a-z]+")]
    private static partial Regex PascalWord();

    [GeneratedRegex(@"\bResult\.(Ok|Fail)\s*[<(]")]
    private static partial Regex HiddenAlias();

    [GeneratedRegex(@"^```csharp\n(?<code>.*?)^```", RegexOptions.Multiline | RegexOptions.Singleline)]
    private static partial Regex CsharpFence();
}
