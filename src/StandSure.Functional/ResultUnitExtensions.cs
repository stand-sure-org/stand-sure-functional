using System;
using System.Diagnostics;

namespace StandSure.Functional;

/// <summary>
/// Side-effect forms of <see cref="Result{T}"/> combinators. <c>void</c> has no place in a chain of results, so these
/// adapt <see cref="Action{T}"/> and parameterless steps to <see cref="Unit"/> and forward to the ordinary combinator.
/// </summary>
/// <remarks>
/// Each method forwards to the <see cref="Result{T}"/> method of the same name, which starts the span and applies the
/// usual <see langword="null"/>-delegate rules. A lambda that returns a value binds to the ordinary overload instead.
/// </remarks>
[StackTraceHidden]
public static class ResultUnitExtensions
{
    /// <summary>Runs one of two actions depending on the case. The failure action comes first.</summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="onFailure">Runs on a failure, including <c>default</c>, with the error.</param>
    /// <param name="onSuccess">Runs on a success with the value.</param>
    /// <returns><see cref="Unit.Value"/>, so the call also works as an expression.</returns>
    /// <remarks>A <see langword="null"/> action for the active branch runs nothing and records a
    /// <c>result.null_delegate</c> failure in telemetry.</remarks>
    public static Unit Match<T>(this Result<T> result, Action<Error> onFailure, Action<T> onSuccess) =>
        result.Match(UnitAdapters.ToFunc(onFailure), UnitAdapters.ToFunc(onSuccess));

    /// <summary>Chains a step that needs no input after a step that produced nothing; a failure short-circuits.</summary>
    /// <typeparam name="TOut">The value type of the chained result.</typeparam>
    /// <param name="result">The result of the step that produced nothing.</param>
    /// <param name="bind">The next step. A <see langword="null"/> value turns a success into a
    /// <c>result.null_delegate</c> failure.</param>
    /// <returns>The result of <paramref name="bind"/>, or the original failure without calling it.</returns>
    public static Result<TOut> Bind<TOut>(this Result<Unit> result, Func<Result<TOut>> bind) =>
        result.Bind(UnitAdapters.IgnoringUnit(bind));
}
