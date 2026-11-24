using System.Runtime.CompilerServices;

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
    internal static Error NullDelegate(string parameterName, [CallerMemberName] string member = "") =>
        Error.Unexpected(NullDelegateCode, $"The delegate '{parameterName}' passed to {member} was null.");

    /// <summary>
    /// The never-throw guard for a delegate argument. Delegate parameters are non-nullable, so the compiler warns a caller
    /// who passes <see langword="null"/>; callers with nullable checks off can still do it, and this guard turns that into a
    /// <c>result.null_delegate</c> failure instead of a <see cref="System.NullReferenceException"/>. It is a method rather
    /// than <c>x is null</c> so that nullable flow analysis (and Rider's "always false") does not read the guard as dead.
    /// </summary>
    internal static bool IsMissing(object? argument) => argument is null;

    /// <summary>A <see cref="System.Threading.Tasks.Task"/> argument, or a task returned by a delegate, was <see langword="null"/>.</summary>
    internal static Error NullTask(string description) =>
        Error.Unexpected(NullTaskCode, $"The task '{description}' was null.");

    /// <summary>A sequence argument was <see langword="null"/>.</summary>
    internal static Error NullSource(string parameterName) =>
        Error.Unexpected(NullSourceCode, $"The sequence '{parameterName}' was null.");
}
