using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AwesomeAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace StandSure.Functional.Tests;

public sealed class ResultUnionTests
{
    private static string Describe(Result<int> r) => r switch
    {
        Error e => e.Code,
        int i => i.ToString(System.Globalization.CultureInfo.InvariantCulture),
    };

    [Fact]
    public void ImplicitFromValue_IsSuccess()
    {
        Result<int> r = 42;

        r.IsSuccess.Should().BeTrue();
        r.IsFailure.Should().BeFalse();
        r.TryGetValue(out int value).Should().BeTrue();
        value.Should().Be(42);
        r.Value.Should().Be(42);
    }

    [Fact]
    public void ImplicitFromError_IsFailure()
    {
        var error = Error.NotFound("user.missing", "No such user.");
        Result<int> r = error;

        r.IsFailure.Should().BeTrue();
        r.IsSuccess.Should().BeFalse();
        r.TryGetValue(out Error? e).Should().BeTrue();
        e.Should().BeSameAs(error);
        r.TryGetValue(out int _).Should().BeFalse();
        r.Value.Should().BeSameAs(error);
    }

    [Fact]
    public void Default_IsFailureUninitialized()
    {
        var r = default(Result<int>);

        r.IsFailure.Should().BeTrue();
        r.IsSuccess.Should().BeFalse();
        r.HasValue.Should().BeTrue();
        r.TryGetValue(out Error? e).Should().BeTrue();
        e.Should().BeSameAs(Error.Uninitialized);
        r.Value.Should().BeSameAs(Error.Uninitialized);
        Describe(r).Should().Be(Error.Uninitialized.Code);

        var (value, error) = r;
        value.Should().Be(0);
        error.Should().BeSameAs(Error.Uninitialized);
    }

    [Fact]
    public void DefaultArrayElement_IsFailure()
    {
        (new Result<int>[3])[1].IsFailure.Should().BeTrue();
    }

    [Fact]
    public void NullErrorInput_IsFailureUninitialized_AndDoesNotThrow()
    {
        var r = Result.Fail<int>(null!);

        r.IsFailure.Should().BeTrue();
        r.TryGetValue(out Error? e).Should().BeTrue();
        e.Should().BeSameAs(Error.Uninitialized);
    }

    [Fact]
    public void Switch_IsExhaustiveWithoutDiscard()
    {
        Describe(7).Should().Be("7");
        Describe(Error.Conflict("dup", "Duplicate.")).Should().Be("dup");
    }

    [Fact]
    public void Deconstruct_GoStyle_EarlyReturn()
    {
        static string Handle(Result<string> r)
        {
            var (value, error) = r;
            if (error is not null)
            {
                return "failed:" + error.Code;
            }

            return "ok:" + value;
        }

        Handle(Result.Ok("hi")).Should().Be("ok:hi");
        Handle(Error.Validation("bad", "Bad input.")).Should().Be("failed:bad");
        Handle(default).Should().Be("failed:" + Error.Uninitialized.Code);
    }

    [Fact]
    public void ResultOfObject_TagWinsOverRuntimeType()
    {
        var r = Result.Ok<object>(Error.NotFound("x", "y"));

        r.IsSuccess.Should().BeTrue();
        var arm = r switch
        {
            Error e => "error:" + e.Code,
            object o => "object:" + o.GetType().Name,
        };
        arm.Should().Be("object:Error");
    }

    [Fact]
    public void ResultOfObject_ImplicitFromError_IsFailure()
    {
        Result<object> r = Error.NotFound("x", "y");

        r.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void ResultOfError_OkIsSuccess()
    {
        Result.Ok(Error.NotFound("x", "y")).IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void NullReferenceValue_IsSuccess()
    {
        var r = Result.Ok<string?>(null);

        r.IsSuccess.Should().BeTrue();
        r.TryGetValue(out string? value).Should().BeTrue();
        value.Should().BeNull();
        var (v, e) = r;
        v.Should().BeNull();
        e.Should().BeNull();
    }

    [Fact]
    public void OkUnit_IsSuccess()
    {
        var r = Result.Ok();

        r.IsSuccess.Should().BeTrue();
        r.TryGetValue(out Unit value).Should().BeTrue();
        value.Should().Be(Unit.Value);
    }

    [Fact]
    public void MissingCase_IsCs8509()
    {
        const string exhaustive = """
            using StandSure.Functional;
            public static class Probe
            {
                public static string Describe(Result<int> r) => r switch { Error e => e.Code, int i => "int" };
            }
            """;
        const string missingCase = """
            using StandSure.Functional;
            public static class Probe
            {
                public static string Describe(Result<int> r) => r switch { int i => "int" };
            }
            """;

        // Positive control: proves references and language version resolve, so the CS8509 below is real.
        Compile(exhaustive).Where(d => d.Severity == DiagnosticSeverity.Error).Should().BeEmpty();
        Compile(missingCase).Select(d => d.Id).Should().Contain("CS8509");
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
            "MissingCaseProbe",
            [CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(LanguageVersion.Preview))],
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, nullableContextOptions: NullableContextOptions.Enable));

        return compilation.GetDiagnostics();
    }
}
