using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

using AwesomeAssertions;

using Microsoft.CodeAnalysis;

using Xunit;

namespace StandSure.Functional.Tests;

/// <summary>
/// <c>Unit.Default</c> is a deprecated alias of <see cref="Unit.Value"/>: models (and TryHelpers, language-ext) guess
/// <c>Default</c>, so the guess compiles with warning SSF0001 naming the right member instead of failing with CS0117.
/// </summary>
public sealed class UnitDefaultAliasTests
{
    // devstral-small-2's line from the 2026-11-25 harness run (all 10 records used it;
    // harness/results/task10-rerun-2026-11-25.md).
    private const string GuessedCode = """
                                       using StandSure.Functional;
                                       public static class Probe
                                       {
                                           public static Result<Unit> Run() => Result.Success(Unit.Default);
                                       }
                                       """;

    [Fact]
    public void Default_EqualsValue()
    {
#pragma warning disable SSF0001 // the alias under test
        Unit.Default.Should().Be(Unit.Value);
#pragma warning restore SSF0001
    }

    [Fact]
    public void GuessedDefault_Compiles_WithWarningThatNamesValue()
    {
        var diagnostics = CompileProbe.Create(GuessedCode).GetDiagnostics(TestContext.Current.CancellationToken);

        diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error).Should().BeEmpty();
        var warning = diagnostics.Should().ContainSingle(d => d.Id == "SSF0001").Subject;
        warning.Severity.Should().Be(DiagnosticSeverity.Warning);
        warning.GetMessage().Should().Contain("Unit.Value");
        warning.Descriptor.HelpLinkUri.Should().EndWith("/docs/diagnostics/SSF0001.md");
    }

    [Fact]
    public void Default_IsHiddenFromIntelliSense()
    {
        // The API snapshot omits [EditorBrowsable], so this test is the only guard on it.
        var field = typeof(Unit).GetField("Default", BindingFlags.Public | BindingFlags.Static);
        field.Should().NotBeNull();

        var browsable = field!.GetCustomAttribute<EditorBrowsableAttribute>();
        browsable.Should().NotBeNull();
        browsable!.State.Should().Be(EditorBrowsableState.Never);

        var obsolete = field.GetCustomAttribute<ObsoleteAttribute>();
        obsolete.Should().NotBeNull();
        obsolete!.IsError.Should().BeFalse();
    }
}
