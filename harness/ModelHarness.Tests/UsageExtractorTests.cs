namespace ModelHarness.Tests;

using System;
using System.IO;

using AwesomeAssertions;

using ModelHarness.Analysis;
using ModelHarness.Compile;
using ModelHarness.Tasks;

public sealed class UsageExtractorTests
{
    private static readonly string TasksRoot = Path.Combine(RepoLocator.FindRoot(AppContext.BaseDirectory), "harness", "tasks");

    private static readonly HarnessTask CreateUser =
        TaskCatalog.Load(TasksRoot).Single(t => t.Id.StartsWith("01-", StringComparison.Ordinal));

    private static UsageFacts Extract(string code, bool withLibrary = false) =>
        UsageExtractor.Extract(SolutionCompiler.Compile(CreateUser, code, withLibrary), CreateUser);

    [Fact]
    public void CountsThrows()
    {
        var facts = Extract("""
            using System;
            public sealed class Solution(IUserStore users)
            {
                public User CreateUser(string email, string name)
                {
                    if (email is null) { throw new ArgumentNullException(nameof(email)); }
                    var n = name ?? throw new ArgumentNullException(nameof(name));
                    try { return users.Add(email, n); }
                    catch (InvalidOperationException) { throw; }
                    catch (Exception) { return null!; }
                }
            }
            """);

        facts.ThrowCount.Should().Be(3);
        facts.TryCatchCount.Should().Be(2);
        facts.Idioms.Should().Contain("exception");
    }

    [Fact]
    public void DetectsTupleReturn()
    {
        var facts = Extract("""
            public sealed class Solution(IUserStore users)
            {
                public (bool Ok, string? Error) CreateUser(string email, string name) => (true, null);
            }
            """);

        facts.Idioms.Should().Contain("tuple-return");
        facts.EntryReturnType.Should().Contain("bool");
    }

    [Fact]
    public void TupleWithoutErrorishMember_IsNotTupleReturn()
    {
        var facts = Extract("""
            public sealed class Solution(IUserStore users)
            {
                public (int A, string B) CreateUser(string email, string name) => (1, "x");
            }
            """);

        facts.Idioms.Should().NotContain("tuple-return");
    }

    [Fact]
    public void DetectsBoolTryOut()
    {
        var facts = Extract("""
            public sealed class Solution(IUserStore users)
            {
                public bool CreateUser(string email, string name, out User? user)
                {
                    user = null;
                    return false;
                }
            }
            """);

        facts.Idioms.Should().Contain("bool-try-out");
    }

    [Fact]
    public void DetectsNullableReturn()
    {
        var facts = Extract("""
            public sealed class Solution(IUserStore users)
            {
                public User? CreateUser(string email, string name) => null;
            }
            """);

        facts.Idioms.Should().Contain("nullable-return");
        facts.EntryReturnType.Should().Be("User?");
    }

    [Fact]
    public void DetectsSelfDeclaredResultType()
    {
        var facts = Extract("""
            public readonly record struct Result<T>(bool Ok, T? Value, string? Message);
            public sealed class Solution(IUserStore users)
            {
                public Result<User> CreateUser(string email, string name) => new(true, null, null);
            }
            """);

        facts.Idioms.Should().Contain("result-like-type");
        facts.DeclaredTypes.Should().Contain(["Result", "Solution"]);
        facts.UsedTypes.Should().Contain("Result<User>");
        facts.Idioms.Should().NotContain("library-result");
    }

    [Fact]
    public void DetectsLibraryResult_AndPrefixesOutcomeMethods()
    {
        var facts = Extract("""
            using StandSure.Functional;
            public sealed class Solution(IUserStore users)
            {
                public Result<User> CreateUser(string email, string name) =>
                    Result.Ok(users.Add(email, name)).Map(u => u);
            }
            """, withLibrary: true);

        facts.Idioms.Should().Contain("library-result");
        facts.InvokedMethods.Should().Contain(["Result.Map", "Add"]);
    }

    [Fact]
    public void CountsIgnoredOutcomeInvocation()
    {
        var facts = Extract("""
            using System.Threading.Tasks;
            public sealed class Solution(IUserStore users)
            {
                private (bool Ok, string Error) Check(string e) => (true, "");
                private Task<(bool Ok, string Error)> CheckAsync(string e) => Task.FromResult((true, ""));
                public async Task<User?> CreateUser(string email, string name)
                {
                    Check(email);
                    await CheckAsync(email);
                    var kept = Check(name);
                    _ = Check(name);
                    users.EmailExists(email);
                    return null;
                }
            }
            """);

        facts.IgnoredOutcomeCount.Should().Be(2);
    }

    [Fact]
    public void RecordsEntryReturnType()
    {
        var facts = Extract("""
            public sealed class Solution(IUserStore users)
            {
                public string CreateUser(string email, string name) => "";
            }
            """);

        facts.EntryReturnType.Should().Be("string");
    }

    [Fact]
    public void EntryReturnType_IsNullWhenMissing()
    {
        Extract("public sealed class Solution { }").EntryReturnType.Should().BeNull();
    }

    [Fact]
    public void CountsCs8509()
    {
        var facts = Extract("""
            public enum Color { Red, Green }
            public sealed class Solution(IUserStore users)
            {
                public int CreateUser(string email, string name) => Pick(Color.Red);
                private static int Pick(Color c) => c switch { Color.Red => 1 };
            }
            """);

        facts.NonExhaustiveSwitchCount.Should().Be(1);
    }

    [Fact]
    public void IgnoresFixtureAndProbeTrees()
    {
        var facts = Extract("public sealed class Solution { public int CreateUser() => 1; }");

        facts.DeclaredTypes.Should().Equal("Solution");
        facts.UsedTypes.Should().NotContain(["User", "IUserStore", "HarnessProbe"]);
    }

    [Fact]
    public void WorksOnFailedCompilation()
    {
        var facts = Extract("""
            public sealed class Solution
            {
                public Missing CreateUser() { throw new System.Exception(); }
            }
            """);

        facts.ThrowCount.Should().Be(1);
        facts.DeclaredTypes.Should().Equal("Solution");
    }

    [Fact]
    public void NeverThrowsOnGarbage()
    {
        var act = () => Extract("}{ not csharp ((");

        act.Should().NotThrow();
    }

    [Fact]
    public void DeclaresUnionTypes()
    {
        var facts = Extract("""
            public record Failure(string Message);
            public union Result<T>(T, Failure);
            public sealed class Solution(IUserStore users)
            {
                public int CreateUser(string email, string name) => 1;
            }
            """);

        facts.DeclaredTypes.Should().Contain("Result");
        facts.Idioms.Should().Contain("result-like-type");
    }

    [Fact]
    public void ConfigureAwait_IsNotPrefixedWithOutcomeType()
    {
        var facts = Extract("""
            using System.Threading.Tasks;
            public sealed class Solution(IUserStore users)
            {
                private Task<(bool Ok, string Error)> CheckAsync(string e) => Task.FromResult((true, ""));
                public async Task<User?> CreateUser(string email, string name)
                {
                    var r = await CheckAsync(email).ConfigureAwait(false);
                    return null;
                }
            }
            """);

        facts.InvokedMethods.Should().Contain("ConfigureAwait");
        facts.InvokedMethods.Should().NotContain(m => m.EndsWith(".ConfigureAwait", StringComparison.Ordinal));
    }

    [Fact]
    public void CountsIgnoredConfiguredAwait()
    {
        var facts = Extract("""
            using System.Threading.Tasks;
            public sealed class Solution(IUserStore users)
            {
                private Task<(bool Ok, string Error)> CheckAsync(string e) => Task.FromResult((true, ""));
                public async Task<User?> CreateUser(string email, string name)
                {
                    await CheckAsync(email).ConfigureAwait(false);
                    return null;
                }
            }
            """);

        facts.IgnoredOutcomeCount.Should().Be(1);
    }
}
