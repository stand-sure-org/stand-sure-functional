namespace ModelHarness.Tests;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

using AwesomeAssertions;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using ModelHarness.Compilation;
using ModelHarness.Tasks;

public sealed class TaskCatalogTests
{
    private static readonly string RepoRoot = RepoLocator.FindRoot(AppContext.BaseDirectory);
    private static readonly string TasksRoot = Path.Combine(RepoRoot, "harness", "tasks");
    private static readonly string PromptsRoot = Path.Combine(RepoRoot, "harness", "prompts");

    // Names the with-library condition imports from StandSure.Functional; a global-namespace
    // fixture type with one of these names would shadow the library type in the model's code.
    private static readonly string[] LibraryTypeNames = ["Result", "Option", "Error", "Unit", "None", "Try", "Some"];

    public static TheoryData<string> TaskIds()
    {
        var data = new TheoryData<string>();
        foreach (var task in TaskCatalog.Load(TasksRoot))
        {
            data.Add(task.Id);
        }

        return data;
    }

    [Fact]
    public void Load_FindsTwelveTasks()
    {
        var tasks = TaskCatalog.Load(TasksRoot);

        tasks.Should().HaveCount(12);
        tasks.Select(t => t.Id).Should().BeInAscendingOrder(StringComparer.Ordinal);
        tasks.Select(t => t.Id).Should().OnlyHaveUniqueItems();
        tasks.Select(t => t.Id).Should().AllSatisfy(id => id.Should().MatchRegex(@"^\d{2}-[a-z0-9-]+$"));
    }

    [Fact]
    public void EveryTask_HasFixtureAndProbe()
    {
        foreach (var task in TaskCatalog.Load(TasksRoot))
        {
            task.FixtureSource.Should().NotBeNullOrWhiteSpace(task.Id);
            task.ProbeSource.Should().NotBeNullOrWhiteSpace(task.Id);
            task.ProbeSource.Should().Contain("static class HarnessProbe", task.Id);
            task.ProbeSource.Should().Contain("ConstructorArguments()", task.Id);
            task.ProbeSource.Should().Contain("Arguments()", task.Id);
            task.EntryType.Should().Be("Solution", task.Id);
            task.EntryMethod.Should().MatchRegex(@"^[A-Z][A-Za-z0-9]+$", task.Id);
        }
    }

    [Theory]
    [MemberData(nameof(TaskIds))]
    public void EveryFixture_CompilesStandalone(string id)
    {
        var task = TaskCatalog.Load(TasksRoot).Single(t => t.Id == id);

        var diagnostics = Compile(id, task.FixtureSource);

        diagnostics.Where(d => d.Severity >= DiagnosticSeverity.Warning).Should().BeEmpty();
    }

    [Theory]
    [MemberData(nameof(TaskIds))]
    public void EveryProbe_BindsToTheSignatureTheTaskAsksFor(string id)
    {
        // Contract for Task 13: a Solution written to the task.md signature can be constructed with
        // HarnessProbe.ConstructorArguments() and its entry method invoked with HarnessProbe.Arguments().
        var task = TaskCatalog.Load(TasksRoot).Single(t => t.Id == id);
        var ctorParameters = Regex.Match(task.Prompt, @"public constructor `Solution\((?<p>[^)]*)\)`").Groups["p"].Value;
        var methodParameters = Regex.Match(task.Prompt, "`" + task.EntryMethod + @"\((?<p>[^)]*)\)`").Groups["p"].Value;
        ctorParameters.Should().NotBeEmpty(id);
        methodParameters.Should().NotBeEmpty(id);
        var stub = $$"""
            using System;
            using System.Collections.Generic;
            using System.Threading;

            namespace Anywhere;

            public sealed class Solution
            {
                public Solution({{ctorParameters}})
                {
                }

                public object? {{task.EntryMethod}}({{methodParameters}}) => null;
            }
            """;

        var assembly = Emit(id, task.FixtureSource, task.ProbeSource, stub);

        var probe = assembly.GetType("HarnessProbe")!;
        var ctorArguments = (object?[])probe.GetMethod("ConstructorArguments")!.Invoke(null, null)!;
        var arguments = (object?[])probe.GetMethod("Arguments")!.Invoke(null, null)!;
        var solutionType = assembly.GetTypes().Single(t => t.Name == task.EntryType);
        var solution = Activator.CreateInstance(solutionType, ctorArguments);
        var invoke = () => solutionType.GetMethod(task.EntryMethod)!.Invoke(solution, arguments);
        invoke.Should().NotThrow(id);
    }

    [Fact]
    public void ReferenceAssemblies_CompileAMinimalApiHandlerAgainstAFixture()
    {
        // Positive control for Task 11: the shared + ASP.NET Core reference set resolves
        // and a handler using Microsoft.AspNetCore.Http compiles beside a task fixture.
        var task = TaskCatalog.Load(TasksRoot).Single(t => t.Id.StartsWith("07-", StringComparison.Ordinal));
        const string handler = """
            using Microsoft.AspNetCore.Http;

            public sealed class Solution(IOrderQueries orders)
            {
                public IResult GetOrder(int id)
                {
                    var order = orders.FindById(id);
                    return order is null ? Results.NotFound() : Results.Ok(order);
                }
            }
            """;

        var diagnostics = Compile("handler", task.FixtureSource, task.ProbeSource, handler);

        diagnostics.Where(d => d.Severity >= DiagnosticSeverity.Warning).Should().BeEmpty();
    }

    [Fact]
    public void ReferenceAssemblies_AreUniqueByFileName()
    {
        var paths = ReferenceAssemblies.All();

        paths.Should().NotBeEmpty();
        paths.Select(Path.GetFileName).Should().OnlyHaveUniqueItems();
        paths.Should().Contain(p => Path.GetFileName(p) == "System.Runtime.dll");
        paths.Should().Contain(p => Path.GetFileName(p) == "Microsoft.AspNetCore.Http.Results.dll");
    }

    [Fact]
    public void NoTaskPrompt_MentionsResultOrOption()
    {
        var vocabulary = new Regex(@"\b(Result|Option|Either|Match|Bind)\b");

        foreach (var task in TaskCatalog.Load(TasksRoot))
        {
            vocabulary.Matches(task.Prompt).Select(m => m.Value).Should().BeEmpty(task.Id);
        }
    }

    [Fact]
    public void NoTaskPrompt_UsesTypeNamePrimingWords()
    {
        // Plain-English words that are also common names for a Result look-alike (Outcome<T>,
        // OperationResult, Maybe<T>, Success/Failure...). Case-insensitive, over the full prompt
        // including fixture text. "failure"/"fail" stay allowed: the baseline system prompt uses them.
        // "optional" is allowed (task 03 says "optional config value", DESIGN wording).
        var priming = new Regex(
            @"\b(outcomes?|results?|either|maybe|options?|success\w*|succeed\w*|response objects?)\b",
            RegexOptions.IgnoreCase);

        foreach (var task in TaskCatalog.Load(TasksRoot))
        {
            priming.Matches(task.Prompt).Select(m => m.Value).Should().BeEmpty(task.Id);
        }
    }

    [Fact]
    public void EveryPrompt_SaysFixturesAreProvided()
    {
        foreach (var dir in Directory.GetDirectories(TasksRoot))
        {
            var markdown = File.ReadAllText(Path.Combine(dir, "task.md"));

            markdown.Should().Contain("already exist", dir);
            markdown.Should().Contain("do not redeclare", dir);
        }
    }

    [Fact]
    public void EveryPrompt_EmbedsFixtureAndNamesTheInvocationSurface()
    {
        foreach (var task in TaskCatalog.Load(TasksRoot))
        {
            task.Prompt.Should().Contain("```csharp\n" + task.FixtureSource.TrimEnd('\n') + "\n```", task.Id);
            task.Prompt.Should().NotContain(TaskCatalog.FixtureMarker, task.Id);
            task.Prompt.Should().NotContain("entry-method:", task.Id);
            task.Prompt.Should().NotStartWith("---", task.Id);
            task.Prompt.Should().NotContain("\r", task.Id);
            task.Prompt.Should().Contain("public class named `Solution`", task.Id);
            task.Prompt.Should().Contain("`" + task.EntryMethod + "(", task.Id);
            task.Prompt.Should().Contain("Choose its return type yourself", task.Id);
            task.Prompt.Should().NotContain("HarnessProbe", task.Id);
        }
    }

    [Fact]
    public void NoFixtureOrProbe_ReferencesOrShadowsTheLibrary()
    {
        foreach (var task in TaskCatalog.Load(TasksRoot))
        {
            foreach (var source in new[] { task.FixtureSource, task.ProbeSource })
            {
                source.Should().NotContain("StandSure", task.Id);

                var declared = CSharpSyntaxTree.ParseText(source, cancellationToken: TestContext.Current.CancellationToken).GetRoot(TestContext.Current.CancellationToken)
                    .DescendantNodes()
                    .OfType<Microsoft.CodeAnalysis.CSharp.Syntax.BaseTypeDeclarationSyntax>()
                    .Select(t => t.Identifier.ValueText);
                declared.Should().NotContain(LibraryTypeNames, task.Id);
            }
        }
    }

    [Fact]
    public void BaselineSystemPrompt_IsTheDesignText()
    {
        var text = File.ReadAllText(Path.Combine(PromptsRoot, "system-baseline.md")).TrimEnd();

        text.Should().Be(
            "Expected failures must not throw; return a value describing the failure. "
            + "Write idiomatic modern C# 15 / .NET 11.");
    }

    [Fact]
    public void WithLibrarySystemPrompt_IsBaselinePlusApiSummary()
    {
        var baseline = File.ReadAllText(Path.Combine(PromptsRoot, "system-baseline.md")).TrimEnd();
        var template = File.ReadAllText(Path.Combine(PromptsRoot, "system-with-library.md"));

        template.Should().StartWith(baseline);
        SystemPrompts.ComposeWithLibrary(template, "API PAGE").Should().Be(baseline + "\n\nAPI PAGE");
    }

    private static System.Reflection.Assembly Emit(string name, params string[] sources)
    {
        var compilation = CreateCompilation(name, sources);
        using var stream = new MemoryStream();
        var emitted = compilation.Emit(stream, cancellationToken: TestContext.Current.CancellationToken);
        emitted.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error).Should().BeEmpty(name);
        var context = new System.Runtime.Loader.AssemblyLoadContext(name, isCollectible: true);
        stream.Position = 0;
        return context.LoadFromStream(stream);
    }

    private static IReadOnlyList<Diagnostic> Compile(string name, params string[] sources) =>
        CreateCompilation(name, sources).GetDiagnostics(TestContext.Current.CancellationToken);

    private static CSharpCompilation CreateCompilation(string name, string[] sources)
    {
        var parseOptions = new CSharpParseOptions(LanguageVersion.Preview);
        return CSharpCompilation.Create(
            "Fixture_" + name.Replace('-', '_'),
            sources.Select(s => CSharpSyntaxTree.ParseText(s, parseOptions, cancellationToken: TestContext.Current.CancellationToken)),
            ReferenceAssemblies.All().Select(p => MetadataReference.CreateFromFile(p)),
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, nullableContextOptions: NullableContextOptions.Enable));
    }
}
