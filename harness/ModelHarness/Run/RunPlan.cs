namespace ModelHarness.Run;

using System;
using System.Collections.Generic;

using ModelHarness.Models;
using ModelHarness.Tasks;

/// <summary>Everything one invocation runs: the cross product of models, tasks and runs under one condition.</summary>
/// <param name="RunId">One id per CLI invocation.</param>
/// <param name="Condition"><c>baseline</c> or <c>with-library</c>.</param>
/// <param name="SystemPrompt">The already composed system prompt for the condition.</param>
/// <param name="Runs">Repetitions per (model, task); run numbers are 1-based.</param>
public sealed record RunPlan(
    Guid RunId,
    string Condition,
    string SystemPrompt,
    int Runs,
    IReadOnlyList<ModelConfig> Models,
    IReadOnlyList<HarnessTask> Tasks)
{
    public const string Baseline = "baseline";

    public const string WithLibrary = "with-library";

    public bool IsWithLibrary => string.Equals(Condition, WithLibrary, StringComparison.Ordinal);
}
