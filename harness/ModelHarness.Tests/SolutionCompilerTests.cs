namespace ModelHarness.Tests;

using System;
using System.IO;

using AwesomeAssertions;

using Microsoft.CodeAnalysis;

using ModelHarness.Compile;
using ModelHarness.Tasks;

public sealed class SolutionCompilerTests
{
    private static readonly string TasksRoot = Path.Combine(RepoLocator.FindRoot(AppContext.BaseDirectory), "harness", "tasks");

    private static HarnessTask Task(string prefix) =>
        TaskCatalog.Load(TasksRoot).Single(t => t.Id.StartsWith(prefix, StringComparison.Ordinal));

    [Fact]
    public void CompilesValidSolution()
    {
        const string code = """
            using System;
            public sealed class Solution(IUserStore users)
            {
                public User? CreateUser(string email, string name) => users.EmailExists(email) ? null : users.Add(email, name);
            }
            """;

        var result = SolutionCompiler.Compile(Task("01-"), code, withLibrary: false);

        result.Success.Should().BeTrue(string.Join("\n", result.Diagnostics));
        result.Assembly.Should().NotBeNullOrEmpty();
        result.Diagnostics.Should().BeEmpty();
        result.StrippedTypes.Should().BeEmpty();
    }

    [Fact]
    public void ReportsDiagnosticsForInvalid()
    {
        var result = SolutionCompiler.Compile(Task("01-"), "public sealed class Solution { int X() => missing; }", withLibrary: false);

        result.Success.Should().BeFalse();
        result.Assembly.Should().BeNull();
        result.Diagnostics.Should().ContainSingle().Which.Should().Contain("CS0103");
        result.Compilation.Should().NotBeNull();
    }

    [Fact]
    public void WithLibrary_ResolvesResultType()
    {
        const string code = """
            using StandSure.Functional;
            public sealed class Solution(IUserStore users)
            {
                public Result<User> CreateUser(string email, string name) => users.Add(email, name);
            }
            """;

        SolutionCompiler.Compile(Task("01-"), code, withLibrary: true).Success.Should().BeTrue();
        var without = SolutionCompiler.Compile(Task("01-"), code, withLibrary: false);
        without.Success.Should().BeFalse();
        without.Diagnostics.Should().Contain(d => d.Contains("CS0246", StringComparison.Ordinal) || d.Contains("CS0234", StringComparison.Ordinal));
    }

    [Fact]
    public void FixtureTypesAreCompiledIn()
    {
        const string code = "public sealed class Solution(IUserStore users) { public bool Has(string e) => users.EmailExists(e); }";

        var result = SolutionCompiler.Compile(Task("01-"), code, withLibrary: false);

        result.Success.Should().BeTrue(string.Join("\n", result.Diagnostics));
        result.Compilation.GetTypeByMetadataName("HarnessProbe").Should().NotBeNull();
    }

    [Fact]
    public void RedeclaredFixtureType_IsStripped_AndRecorded()
    {
        const string code = """
            using System;
            public sealed record User(Guid Id, string Email, string DisplayName);
            public interface IUserStore
            {
                bool EmailExists(string email);
                User Add(string email, string displayName);
            }
            public sealed class Solution(IUserStore users)
            {
                public User Make() => users.Add("a@b.c", "A");
            }
            """;

        var result = SolutionCompiler.Compile(Task("01-"), code, withLibrary: false);

        result.Success.Should().BeTrue(string.Join("\n", result.Diagnostics));
        result.StrippedTypes.Should().BeEquivalentTo("User", "IUserStore");
    }

    [Fact]
    public void RedeclaredFixtureType_InsideNamespace_IsStripped()
    {
        const string code = """
            namespace My.App;
            public sealed record User(System.Guid Id, string Email, string DisplayName);
            public sealed class Solution(IUserStore users) { public bool Has(string e) => users.EmailExists(e); }
            """;

        var result = SolutionCompiler.Compile(Task("01-"), code, withLibrary: false);

        result.StrippedTypes.Should().BeEquivalentTo("User");
    }

    [Fact]
    public void Solution_IsNeverStripped()
    {
        var result = SolutionCompiler.Compile(Task("01-"), "public sealed class Solution { }", withLibrary: false);

        result.StrippedTypes.Should().NotContain("Solution");
        result.Compilation.GetTypeByMetadataName("Solution").Should().NotBeNull();
    }

    [Fact]
    public void ImplicitUsingsAreOff()
    {
        var result = SolutionCompiler.Compile(Task("01-"), "public sealed class Solution { List<int> X() => new(); }", withLibrary: false);

        result.Success.Should().BeFalse();
        result.Diagnostics.Should().Contain(d => d.Contains("CS0246", StringComparison.Ordinal));
    }

    [Fact]
    public void WarningsDoNotFailTheBuild_ButStayVisibleOnCompilation()
    {
        const string code = "public sealed class Solution { public string? Maybe() => null; public int Len() => Maybe().Length; }";

        var result = SolutionCompiler.Compile(Task("01-"), code, withLibrary: false);

        result.Success.Should().BeTrue(string.Join("\n", result.Diagnostics));
        result.Compilation.GetDiagnostics(TestContext.Current.CancellationToken)
            .Should().Contain(d => d.Id == "CS8602" && d.Severity == DiagnosticSeverity.Warning);
    }

    [Fact]
    public void Garbage_ReturnsFailureWithoutThrowing()
    {
        var result = SolutionCompiler.Compile(Task("01-"), "}}} not code {{{ \0", withLibrary: true);

        result.Success.Should().BeFalse();
        result.Diagnostics.Should().NotBeEmpty();
    }

    [Fact]
    public void ConcatenatedFences_WithRepeatedUsings_Compile()
    {
        var text = "```csharp\nusing System;\npublic sealed class Solution(IUserStore u) { public Guid G() => Guid.NewGuid(); }\n```\n```csharp\nusing System;\npublic sealed class Helper { }\n```";
        var extracted = CodeExtractor.Extract(text);

        var result = SolutionCompiler.Compile(Task("01-"), extracted.Code!, withLibrary: false);

        extracted.Status.Should().Be(ExtractStatus.Concatenated);
        result.Success.Should().BeTrue(string.Join("\n", result.Diagnostics));
    }
}
