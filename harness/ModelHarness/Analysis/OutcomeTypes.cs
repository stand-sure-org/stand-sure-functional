namespace ModelHarness.Analysis;

using System;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

/// <summary>Classifies types as "outcome types".</summary>
/// <remarks>
/// An outcome type is a type whose name contains <c>Result</c>, <c>Option</c>, <c>Outcome</c> or <c>Either</c>, or a
/// <c>ValueTuple</c> that includes a <c>bool</c> or an error-ish member. An error-ish member is a tuple element whose name
/// contains <c>Error</c>, <c>Message</c>, <c>Reason</c> or <c>Failure</c>, or whose type is <c>Exception</c> (or a subclass
/// named so). Unnamed elements (<c>Item1</c>) qualify only through their type (<c>bool</c>, <c>Exception</c>).
/// <c>Task&lt;T&gt;</c> and <c>ValueTask&lt;T&gt;</c> are outcome types when <c>T</c> is.
/// </remarks>
internal static class OutcomeTypes
{
    private static readonly string[] NameMarkers = ["Result", "Option", "Outcome", "Either"];
    private static readonly string[] ErrorMarkers = ["Error", "Message", "Reason", "Failure"];

    public static bool IsOutcomeName(string name) =>
        NameMarkers.Any(m => name.Contains(m, StringComparison.Ordinal));

    public static bool IsErrorishName(string? name) =>
        name is not null && ErrorMarkers.Any(m => name.Contains(m, StringComparison.Ordinal));

    /// <summary>Strips one layer of Task/ValueTask.</summary>
    public static ITypeSymbol Unwrap(ITypeSymbol type) =>
        type is INamedTypeSymbol { IsGenericType: true, TypeArguments.Length: 1 } named
        && named.Name is "Task" or "ValueTask" or "ConfiguredTaskAwaitable" or "ConfiguredValueTaskAwaitable"
            ? named.TypeArguments[0]
            : type;

    /// <summary>True when the type itself (no Task unwrapping) is an outcome type.</summary>
    public static bool IsOutcomeItself(ITypeSymbol? type) =>
        type is not null && type.TypeKind != TypeKind.Error && (IsOutcomeName(type.Name) || IsOutcomeTuple(type));

    public static bool IsOutcome(ITypeSymbol? type)
    {
        if (type is null || type.TypeKind == TypeKind.Error)
        {
            return false;
        }

        var inner = Unwrap(type);
        return IsOutcomeName(inner.Name) || IsOutcomeTuple(inner);
    }

    public static bool IsOutcomeTuple(ITypeSymbol? type)
    {
        if (type is not INamedTypeSymbol { IsTupleType: true } tuple)
        {
            return false;
        }

        return tuple.TupleElements.Any(e =>
            e.Type.SpecialType == SpecialType.System_Boolean
            || e.Type.Name.Contains("Exception", StringComparison.Ordinal)
            || (!e.IsImplicitlyDeclared && IsErrorishName(e.Name)));
    }

    /// <summary>Syntax fallback for tuple return types when the semantic type is unavailable.</summary>
    public static bool IsOutcomeTuple(TypeSyntax? syntax)
    {
        if (syntax is not TupleTypeSyntax tuple)
        {
            return false;
        }

        return tuple.Elements.Any(e =>
            e.Type is PredefinedTypeSyntax { Keyword.ValueText: "bool" }
            || e.Type.ToString().Contains("Exception", StringComparison.Ordinal)
            || IsErrorishName(e.Identifier.ValueText));
    }
}
