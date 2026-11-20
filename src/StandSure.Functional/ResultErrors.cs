namespace StandSure.Functional;

/// <summary>Errors that the <see cref="Result{T}"/> combinators return instead of throwing on bad arguments.</summary>
internal static class ResultErrors
{
    internal const string NullDelegateCode = "result.null_delegate";
    internal const string NullTaskCode = "result.null_task";
    internal const string NullSourceCode = "result.null_source";
    internal const string CombineFailedCode = "result.combine_failed";
    internal const string EnsureFailedCode = "result.ensure_failed";

    /// <summary>A delegate argument was <see langword="null"/>.</summary>
    internal static Error NullDelegate(string parameterName) =>
        Error.Unexpected(NullDelegateCode, $"The delegate '{parameterName}' was null.");

    /// <summary>A <see cref="System.Threading.Tasks.Task"/> argument, or a task returned by a delegate, was <see langword="null"/>.</summary>
    internal static Error NullTask(string description) =>
        Error.Unexpected(NullTaskCode, $"The task '{description}' was null.");

    /// <summary>A sequence argument was <see langword="null"/>.</summary>
    internal static Error NullSource(string parameterName) =>
        Error.Unexpected(NullSourceCode, $"The sequence '{parameterName}' was null.");
}
