using System;
using System.Runtime.CompilerServices;

namespace StandSure.Functional;

/// <content>
/// Combinators: <c>Match</c>, <c>Map</c>, <c>Bind</c>, <c>Filter</c>, <c>GetValueOrDefault</c> and <c>ToResult</c>.
/// </content>
/// <remarks>
/// <para>
/// Every combinator treats <c>default(Option&lt;T&gt;)</c> as none. A combinator on none never calls its delegate.
/// </para>
/// <para>
/// A delegate that throws is not caught here; only <see cref="Try"/> catches. An option has no error channel, so a
/// <see langword="null"/> delegate that would have been called makes <c>Map</c>, <c>Bind</c> and <c>Filter</c> return
/// none and records a <c>result.null_delegate</c> failure in telemetry. <c>Match</c> returns <c>default(TOut)</c>
/// when the delegate for the active branch is <see langword="null"/>, and records the same failure. A delegate the
/// active branch does not use may be <see langword="null"/> without effect.
/// </para>
/// </remarks>
public readonly partial struct Option<T>
{
    /// <summary>Runs one of two functions depending on the case. The none function comes first.</summary>
    /// <typeparam name="TOut">The type both functions return.</typeparam>
    /// <param name="onNone">Runs on none, including <c>default</c>.
    /// A <see langword="null"/> value makes none return <c>default(TOut)</c>.</param>
    /// <param name="onSome">Runs on some with the value.
    /// A <see langword="null"/> value makes some return <c>default(TOut)</c>.</param>
    /// <returns>The result of the function that ran.</returns>
    public TOut Match<TOut>(Func<TOut> onNone, Func<T, TOut> onSome)
    {
        if (_isSome)
        {
            return onSome is null ? NullBranch<TOut>(nameof(onSome)) : onSome(_value);
        }

        return onNone is null ? NullBranch<TOut>(nameof(onNone)) : onNone();
    }

    // A null active branch still counts as a failure in telemetry (building the error records it), then yields default.
    private static TOut NullBranch<TOut>(string parameterName)
    {
        _ = ResultErrors.NullDelegate(parameterName);
        return default!;
    }

    /// <summary>Transforms the value; none passes through unchanged.</summary>
    /// <typeparam name="TOut">The new value type.</typeparam>
    /// <param name="map">The transformation. A <see langword="null"/> value turns some into none and records a
    /// <c>result.null_delegate</c> failure. A <see langword="null"/> return also gives none.</param>
    /// <returns>Some holding the mapped value, or none.</returns>
    public Option<TOut> Map<TOut>(Func<T, TOut> map)
    {
        if (!_isSome)
        {
            return default;
        }

        if (map is null)
        {
            _ = ResultErrors.NullDelegate(nameof(map));
            return default;
        }

        return new Option<TOut>(map(_value));
    }

    /// <summary>Chains an operation that itself returns an <see cref="Option{T}"/>; none short-circuits.</summary>
    /// <typeparam name="TOut">The value type of the chained option.</typeparam>
    /// <param name="bind">The next step. A <see langword="null"/> value turns some into none and records a
    /// <c>result.null_delegate</c> failure.</param>
    /// <returns>The result of <paramref name="bind"/>, or none without calling it.</returns>
    public Option<TOut> Bind<TOut>(Func<T, Option<TOut>> bind)
    {
        if (!_isSome)
        {
            return default;
        }

        if (bind is null)
        {
            _ = ResultErrors.NullDelegate(nameof(bind));
            return default;
        }

        return bind(_value);
    }

    /// <summary>Keeps some only when the predicate holds.</summary>
    /// <param name="predicate">The check on the value. A <see langword="null"/> value turns some into none and
    /// records a <c>result.null_delegate</c> failure.</param>
    /// <returns>This option if it is some and the predicate holds; otherwise none.</returns>
    public Option<T> Filter(Func<T, bool> predicate)
    {
        if (!_isSome)
        {
            return default;
        }

        if (predicate is null)
        {
            _ = ResultErrors.NullDelegate(nameof(predicate));
            return default;
        }

        return predicate(_value) ? this : default;
    }

    /// <summary>Gets the value, or a fallback for none (including <c>default</c>).</summary>
    /// <param name="fallback">The value returned when this option is none.</param>
    /// <returns>The value or <paramref name="fallback"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T GetValueOrDefault(T fallback) => _isSome ? _value : fallback;

    /// <summary>Converts to a <see cref="Result{T}"/>.</summary>
    /// <param name="ifNone">The error for none. A <see langword="null"/> value becomes
    /// <see cref="Error.Uninitialized"/>.</param>
    /// <returns>A success holding the value, or a failure with <paramref name="ifNone"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Result<T> ToResult(Error ifNone) => _isSome ? new Result<T>(_value) : new Result<T>(ifNone);
}
