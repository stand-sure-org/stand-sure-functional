using System;
using System.Diagnostics;

namespace StandSure.Functional;

/// <summary>
/// Side-effect forms of <see cref="Option{T}"/> combinators. They adapt actions to <see cref="Unit"/> and forward to
/// the ordinary combinator, which starts the span and applies the usual <see langword="null"/>-delegate rules.
/// </summary>
[StackTraceHidden]
public static class OptionUnitExtensions
{
    /// <summary>Runs one of two actions depending on the case. The none action comes first.</summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="option">The option.</param>
    /// <param name="onNone">Runs on none, including <c>default</c>.</param>
    /// <param name="onSome">Runs on some with the value.</param>
    /// <returns><see cref="Unit.Value"/>, so the call also works as an expression.</returns>
    /// <remarks>A <see langword="null"/> action for the active branch runs nothing and records a
    /// <c>result.null_delegate</c> failure in telemetry.</remarks>
    public static Unit Match<T>(this Option<T> option, Action onNone, Action<T> onSome) =>
        option.Match(UnitAdapters.ToFunc(onNone), UnitAdapters.ToFunc(onSome));
}
