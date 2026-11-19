using System;
using System.Runtime.CompilerServices;

namespace StandSure.Functional;

/// <content>
/// Combinators: <c>Match</c>, <c>Map</c>, <c>Bind</c>, <c>MapError</c>, <c>Ensure</c>, <c>Tap</c> and
/// <c>GetValueOrDefault</c>.
/// </content>
/// <remarks>
/// <para>
/// Every combinator treats <c>default(Result&lt;T&gt;)</c> as a failure whose error is
/// <see cref="Error.Uninitialized"/>. A combinator on a failure never calls its delegate.
/// </para>
/// <para>
/// A delegate that throws is not caught here; only <see cref="Try"/> catches. A <see langword="null"/> delegate
/// never throws either. <c>Map</c>, <c>Bind</c>, <c>MapError</c>, <c>Ensure</c> and <c>Tap</c> return a failure with
/// code <c>result.null_delegate</c> when they would have had to call it. <c>Match</c> returns
/// <c>default(TOut)</c> when the delegate for the active branch is <see langword="null"/>. A delegate the active
/// branch does not use may be <see langword="null"/> without effect.
/// </para>
/// </remarks>
public readonly partial struct Result<T>
{
    // For ResultTaskExtensions, which cannot see the private state. Valid only after IsSuccess / IsFailure was checked.
    internal T ValueUnchecked
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _value;
    }

    internal Error ErrorUnchecked
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => FailureError;
    }

    /// <summary>Runs one of two functions depending on the case. The failure function comes first.</summary>
    /// <typeparam name="TOut">The type both functions return.</typeparam>
    /// <param name="onFailure">Runs on a failure, including <c>default</c>, with the error.
    /// A <see langword="null"/> value makes a failure return <c>default(TOut)</c>.</param>
    /// <param name="onSuccess">Runs on a success with the value.
    /// A <see langword="null"/> value makes a success return <c>default(TOut)</c>.</param>
    /// <returns>The result of the function that ran.</returns>
    public TOut Match<TOut>(Func<Error, TOut> onFailure, Func<T, TOut> onSuccess)
    {
        if (_tag == TagOk)
        {
            return onSuccess is null ? default! : onSuccess(_value);
        }

        return onFailure is null ? default! : onFailure(FailureError);
    }

    /// <summary>Transforms the success value; a failure passes through unchanged.</summary>
    /// <typeparam name="TOut">The new value type.</typeparam>
    /// <param name="map">The transformation. A <see langword="null"/> value turns a success into a
    /// <c>result.null_delegate</c> failure.</param>
    /// <returns>A success holding the mapped value, or the original failure.</returns>
    public Result<TOut> Map<TOut>(Func<T, TOut> map)
    {
        if (_tag != TagOk)
        {
            return new Result<TOut>(FailureError);
        }

        if (map is null)
        {
            return new Result<TOut>(ResultErrors.NullDelegate(nameof(map)));
        }

        return new Result<TOut>(map(_value));
    }

    /// <summary>Chains an operation that itself returns a <see cref="Result{T}"/>; a failure short-circuits.</summary>
    /// <typeparam name="TOut">The value type of the chained result.</typeparam>
    /// <param name="bind">The next step. A <see langword="null"/> value turns a success into a
    /// <c>result.null_delegate</c> failure.</param>
    /// <returns>The result of <paramref name="bind"/>, or the original failure without calling it.</returns>
    public Result<TOut> Bind<TOut>(Func<T, Result<TOut>> bind)
    {
        if (_tag != TagOk)
        {
            return new Result<TOut>(FailureError);
        }

        if (bind is null)
        {
            return new Result<TOut>(ResultErrors.NullDelegate(nameof(bind)));
        }

        return bind(_value);
    }

    /// <summary>Transforms the error of a failure; a success passes through unchanged.</summary>
    /// <param name="map">The transformation. It receives <see cref="Error.Uninitialized"/> for <c>default</c>.
    /// A <see langword="null"/> return becomes <see cref="Error.Uninitialized"/>. A <see langword="null"/> delegate
    /// turns a failure into a <c>result.null_delegate</c> failure.</param>
    /// <returns>The original success, or a failure with the mapped error.</returns>
    public Result<T> MapError(Func<Error, Error> map)
    {
        if (_tag == TagOk)
        {
            return this;
        }

        if (map is null)
        {
            return new Result<T>(ResultErrors.NullDelegate(nameof(map)));
        }

        return new Result<T>(map(FailureError));
    }

    /// <summary>Turns a success into the given failure when the predicate is false.</summary>
    /// <param name="predicate">The check on the value. A <see langword="null"/> value turns a success into a
    /// <c>result.null_delegate</c> failure.</param>
    /// <param name="error">The error returned when the predicate is false. A <see langword="null"/> value becomes
    /// <see cref="Error.Uninitialized"/>.</param>
    /// <returns>This result if it is a failure or the predicate holds; otherwise a failure with <paramref name="error"/>.</returns>
    public Result<T> Ensure(Func<T, bool> predicate, Error error)
    {
        if (_tag != TagOk)
        {
            return this;
        }

        if (predicate is null)
        {
            return new Result<T>(ResultErrors.NullDelegate(nameof(predicate)));
        }

        return predicate(_value) ? this : new Result<T>(error);
    }

    /// <summary>Turns a success into a validation failure when the predicate is false, describing it with the
    /// source text of the predicate.</summary>
    /// <param name="predicate">The check on the value. A <see langword="null"/> value turns a success into a
    /// <c>result.null_delegate</c> failure.</param>
    /// <param name="expression">Filled in by the compiler with the source text of <paramref name="predicate"/>;
    /// do not pass it. When <see langword="null"/>, the message is generic.</param>
    /// <returns>This result if it is a failure or the predicate holds; otherwise a <see cref="ErrorKind.Validation"/>
    /// failure with code <c>result.ensure_failed</c> and <paramref name="expression"/> as its message.</returns>
    // RS0027: the (predicate, Error) overload is a distinct type-based overload, not a later optional-parameter addition.
#pragma warning disable RS0027
    [OverloadResolutionPriority(-1)]
    public Result<T> Ensure(Func<T, bool> predicate, [CallerArgumentExpression(nameof(predicate))] string? expression = null)
    {
        if (_tag != TagOk)
        {
            return this;
        }

        if (predicate is null)
        {
            return new Result<T>(ResultErrors.NullDelegate(nameof(predicate)));
        }

        if (predicate(_value))
        {
            return this;
        }

        return new Result<T>(Error.Validation(ResultErrors.EnsureFailedCode, expression ?? "The predicate returned false."));
    }
#pragma warning restore RS0027

    /// <summary>Runs a side effect on the success value and returns this result unchanged.</summary>
    /// <param name="action">The side effect. A <see langword="null"/> value turns a success into a
    /// <c>result.null_delegate</c> failure.</param>
    /// <returns>This result.</returns>
    public Result<T> Tap(Action<T> action)
    {
        if (_tag != TagOk)
        {
            return this;
        }

        if (action is null)
        {
            return new Result<T>(ResultErrors.NullDelegate(nameof(action)));
        }

        action(_value);
        return this;
    }

    /// <summary>Gets the success value, or a fallback for a failure (including <c>default</c>).</summary>
    /// <param name="fallback">The value returned when this result is not a success.</param>
    /// <returns>The value or <paramref name="fallback"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T GetValueOrDefault(T fallback) => _tag == TagOk ? _value : fallback;
}
