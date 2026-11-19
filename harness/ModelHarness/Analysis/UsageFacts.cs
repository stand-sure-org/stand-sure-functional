namespace ModelHarness.Analysis;

using System.Collections.Generic;

/// <summary>Facts extracted from one model answer by <see cref="UsageExtractor"/>.</summary>
/// <param name="DeclaredTypes">Simple names of every type (class, struct, record, interface, enum, delegate) the model declared, sorted ordinally.</param>
/// <param name="UsedTypes">Distinct types the model's code references, in the compiler's minimal display format (<c>Result&lt;User&gt;</c>, <c>int</c>); <c>void</c> excluded.</param>
/// <param name="InvokedMethods">
/// Distinct invoked method names. Simple name (<c>Add</c>), except when the receiver or containing type is outcome-like,
/// where it is <c>Type.Method</c> with the generic-free type name (<c>Result.Map</c>, <c>Option.Some</c>).
/// </param>
/// <param name="EntryReturnType">Minimal display string of the entry method's return type on <c>Solution</c>, or <see langword="null"/>.</param>
/// <param name="ThrowCount">Throw statements plus throw expressions.</param>
/// <param name="TryCatchCount">Number of catch clauses (a try with two catches counts 2; try/finally alone counts 0).</param>
/// <param name="IgnoredOutcomeCount">Expression statements whose invocation yields an outcome type (or a Task/ValueTask of one) and whose value is discarded.</param>
/// <param name="NonExhaustiveSwitchCount">CS8509 diagnostics located in the model's source.</param>
/// <param name="Idioms">Distinct tags: <c>tuple-return</c>, <c>bool-try-out</c>, <c>nullable-return</c>, <c>exception</c>, <c>result-like-type</c>, <c>library-result</c>.</param>
public sealed record UsageFacts(
    IReadOnlyList<string> DeclaredTypes,
    IReadOnlyList<string> UsedTypes,
    IReadOnlyList<string> InvokedMethods,
    string? EntryReturnType,
    int ThrowCount,
    int TryCatchCount,
    int IgnoredOutcomeCount,
    int NonExhaustiveSwitchCount,
    IReadOnlyList<string> Idioms)
{
    /// <summary>Facts for an answer with nothing to analyse.</summary>
    public static UsageFacts Empty { get; } = new([], [], [], null, 0, 0, 0, 0, []);
}
