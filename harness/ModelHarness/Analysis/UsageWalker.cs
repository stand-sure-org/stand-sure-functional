namespace ModelHarness.Analysis;

using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

/// <summary>Collects usage facts from one model syntax tree, using its semantic model where symbols resolve.</summary>
internal sealed class UsageWalker(SemanticModel model) : CSharpSyntaxWalker
{
    private const string LibraryNamespace = "StandSure.Functional";

    private readonly HashSet<string> declared = new(StringComparer.Ordinal);
    private readonly HashSet<string> used = new(StringComparer.Ordinal);
    private readonly HashSet<string> invoked = new(StringComparer.Ordinal);

    public IReadOnlySet<string> DeclaredTypes => this.declared;

    public IReadOnlySet<string> UsedTypes => this.used;

    public IReadOnlySet<string> InvokedMethods => this.invoked;

    public int ThrowCount { get; private set; }

    public int CatchCount { get; private set; }

    public int IgnoredOutcomeCount { get; private set; }

    public bool BoolTryOut { get; private set; }

    public bool TupleReturn { get; private set; }

    public bool ResultLikeType { get; private set; }

    /// <summary>True when a type or method the code binds to lives in the library namespace (a semantic check). A bare
    /// <c>using StandSure.Functional;</c> does not count: the with-library prompt asks for it, so it says nothing about use.</summary>
    public bool LibraryResult { get; private set; }

    public override void DefaultVisit(SyntaxNode node)
    {
        if (node is TypeSyntax type)
        {
            this.RecordType(type);
        }

        base.DefaultVisit(node);
    }

    public override void VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        this.Declare(node.Identifier);
        base.VisitClassDeclaration(node);
    }

    public override void VisitStructDeclaration(StructDeclarationSyntax node)
    {
        this.Declare(node.Identifier);
        base.VisitStructDeclaration(node);
    }

    public override void VisitRecordDeclaration(RecordDeclarationSyntax node)
    {
        this.Declare(node.Identifier);
        base.VisitRecordDeclaration(node);
    }

    public override void VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
    {
        this.Declare(node.Identifier);
        base.VisitInterfaceDeclaration(node);
    }

    public override void VisitUnionDeclaration(UnionDeclarationSyntax node)
    {
        this.Declare(node.Identifier);
        base.VisitUnionDeclaration(node);
    }

    public override void VisitEnumDeclaration(EnumDeclarationSyntax node)
    {
        this.Declare(node.Identifier);
        base.VisitEnumDeclaration(node);
    }

    public override void VisitDelegateDeclaration(DelegateDeclarationSyntax node)
    {
        this.Declare(node.Identifier);
        base.VisitDelegateDeclaration(node);
    }

    public override void VisitThrowStatement(ThrowStatementSyntax node)
    {
        this.ThrowCount++;
        base.VisitThrowStatement(node);
    }

    public override void VisitThrowExpression(ThrowExpressionSyntax node)
    {
        this.ThrowCount++;
        base.VisitThrowExpression(node);
    }

    public override void VisitCatchClause(CatchClauseSyntax node)
    {
        this.CatchCount++;
        base.VisitCatchClause(node);
    }

    public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
    {
        this.CheckMethodIdioms(node);
        base.VisitMethodDeclaration(node);
    }

    public override void VisitExpressionStatement(ExpressionStatementSyntax node)
    {
        var expression = node.Expression is AwaitExpressionSyntax awaited ? awaited.Expression : node.Expression;

        if (expression is InvocationExpressionSyntax invocation && this.IsOutcome(invocation))
        {
            this.IgnoredOutcomeCount++;
        }

        base.VisitExpressionStatement(node);
    }

    public override void VisitInvocationExpression(InvocationExpressionSyntax node)
    {
        this.RecordInvocation(node);
        base.VisitInvocationExpression(node);
    }

    private void Declare(SyntaxToken identifier)
    {
        var name = identifier.ValueText;

        if (name.Length == 0)
        {
            return;
        }

        this.declared.Add(name);

        if (OutcomeTypes.IsOutcomeName(name))
        {
            this.ResultLikeType = true;
        }
    }

    private void CheckMethodIdioms(MethodDeclarationSyntax node)
    {
        var returnType = Safe(() => model.GetTypeInfo(node.ReturnType).Type);

        if (returnType is { SpecialType: SpecialType.System_Boolean })
        {
            foreach (var parameter in node.ParameterList.Parameters)
            {
                if (parameter.Modifiers.Any(SyntaxKind.OutKeyword))
                {
                    this.BoolTryOut = true;
                }
            }
        }
        else if (node.ReturnType is PredefinedTypeSyntax { Keyword.ValueText: "bool" })
        {
            this.BoolTryOut |= node.ParameterList.Parameters.Any(p => p.Modifiers.Any(SyntaxKind.OutKeyword));
        }

        if (!node.Modifiers.Any(SyntaxKind.PublicKeyword))
        {
            return;
        }

        var inner = returnType is null ? null : OutcomeTypes.Unwrap(returnType);

        if (OutcomeTypes.IsOutcomeTuple(inner) || OutcomeTypes.IsOutcomeTuple(UnwrapSyntax(node.ReturnType)))
        {
            this.TupleReturn = true;
        }
    }

    private static TypeSyntax UnwrapSyntax(TypeSyntax type) =>
        type is GenericNameSyntax { Identifier.ValueText: "Task" or "ValueTask", TypeArgumentList.Arguments.Count: 1 } generic
            ? generic.TypeArgumentList.Arguments[0]
            : type;

    private void RecordType(TypeSyntax syntax)
    {
        var symbol = Safe(() => model.GetSymbolInfo(syntax).Symbol);

        if (symbol is IAliasSymbol alias)
        {
            symbol = alias.Target;
        }

        if (symbol is not ITypeSymbol type || type.TypeKind == TypeKind.Error || type.SpecialType == SpecialType.System_Void)
        {
            return;
        }

        this.used.Add(type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat));
        this.NoteLibrary(type);
    }

    private void RecordInvocation(InvocationExpressionSyntax node)
    {
        var simple = node.Expression switch
        {
            MemberAccessExpressionSyntax access => access.Name.Identifier.ValueText,
            MemberBindingExpressionSyntax binding => binding.Name.Identifier.ValueText,
            SimpleNameSyntax name => name.Identifier.ValueText,
            _ => null,
        };

        if (string.IsNullOrEmpty(simple))
        {
            return;
        }

        var method = Safe(() => model.GetSymbolInfo(node).Symbol) as IMethodSymbol;

        if (method is not null)
        {
            this.NoteLibrary(method.ContainingType);
        }

        var prefix = this.OutcomePrefix(node, method);
        this.invoked.Add(prefix is null ? simple : prefix + "." + simple);
    }

    private string? OutcomePrefix(InvocationExpressionSyntax node, IMethodSymbol? method)
    {
        if (node.Expression is MemberAccessExpressionSyntax access)
        {
            var receiver = Safe(() => model.GetTypeInfo(access.Expression).Type);

            if (OutcomeTypes.IsOutcomeItself(receiver))
            {
                return receiver!.Name;
            }
        }

        if (method?.ContainingType is { } owner && OutcomeTypes.IsOutcomeName(owner.Name))
        {
            return owner.Name;
        }

        return null;
    }

    private bool IsOutcome(InvocationExpressionSyntax invocation)
    {
        var type = Safe(() => model.GetTypeInfo(invocation).Type);
        return OutcomeTypes.IsOutcome(type);
    }

    private void NoteLibrary(ISymbol? symbol)
    {
        var ns = symbol?.ContainingNamespace?.ToDisplayString();

        if (ns is not null
            && (ns == LibraryNamespace || ns.StartsWith(LibraryNamespace + ".", StringComparison.Ordinal)))
        {
            this.LibraryResult = true;
        }
    }

    private static T? Safe<T>(Func<T?> func)
        where T : class
    {
        try
        {
            return func();
        }
        catch (Exception ex) when (ex is not OutOfMemoryException and not OperationCanceledException)
        {
            return null;
        }
    }
}
