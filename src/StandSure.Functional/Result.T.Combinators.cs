using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace StandSure.Functional;

/// <content>
/// Combinators: <c>Match</c>, <c>Map</c>, <c>Bind</c>, <c>MapError</c>, <c>Ensure</c>, <c>Tap</c>, <c>TapError</c> and
/// <c>GetValueOrDefault</c>.
/// </content>
/// <remarks>
/// <para>
/// Every combinator treats <c>default(Result&lt;T&gt;)</c> as a failure whose error is
/// <see cref="Error.Uninitialized"/>. A combinator on a failure never calls its delegate.
/// </para>
/// <para>
/// A delegate that throws is not caught here; only <see cref="Try"/> catches. A <see langword="null"/> delegate
/// never throws either. <c>Map</c>, <c>Bind</c>, <c>MapError</c>, <c>Ensure</c>, <c>Tap</c> and <c>TapError</c> return a failure with
/// code <c>result.null_delegate</c> when they would have had to call it. <c>Match</c> returns
/// <c>default(TOut)</c> when the delegate for the active branch is <see langword="null"/>, and records a
/// <c>result.null_delegate</c> failure in telemetry. A delegate the active
/// branch does not use may be <see langword="null"/> without effect.
/// </para>
/// </remarks>
public readonly partial struct Result<T>
{
    // For ResultTaskExtensions, which cannot see the private state. Valid only after IsSuccess / IsFailure was checked.
    internal T ValueUnchecked
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)] get => this.resultValue;
    }

    internal Error ErrorUnchecked
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)] get => this.FailureError;
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
        using var activity = FunctionalDiagnostics.Source.StartActivity();

        return this.MatchCore(onFailure, onSuccess, nameof(this.Match));
    }

    // Match's logic without a span, so the sync-delegate MatchAsync overload runs it inside its own span
    // and a null delegate names the public method the caller wrote.
    internal TOut MatchCore<TOut>(Func<Error, TOut> onFailure, Func<T, TOut> onSuccess, string member)
    {
        if (this.tag == TagOk)
        {
            return ResultErrors.IsMissing(onSuccess) ? NullBranch<TOut>(nameof(onSuccess), member) : onSuccess(this.resultValue);
        }

        return ResultErrors.IsMissing(onFailure) ? NullBranch<TOut>(nameof(onFailure), member) : onFailure(this.FailureError);
    }

    // A null active branch still counts as a failure in telemetry (building the error records it), then yields default.
    private static TOut NullBranch<TOut>(string parameterName, string member)
    {
        _ = ResultErrors.NullDelegate(parameterName, member);
        return default!;
    }

    /// <summary>Transforms the success value; a failure passes through unchanged.</summary>
    /// <typeparam name="TOut">The new value type.</typeparam>
    /// <param name="map">The transformation. A <see langword="null"/> value turns a success into a
    /// <c>result.null_delegate</c> failure.</param>
    /// <returns>A success holding the mapped value, or the original failure.</returns>
    public Result<TOut> Map<TOut>(Func<T, TOut> map)
    {
        using var activity = FunctionalDiagnostics.Source.StartActivity();

        return this.MapCore(map, nameof(this.Map));
    }

    // Map's logic without a span, so the sync-delegate MapAsync overload runs it inside its own span
    // and a null delegate names the public method the caller wrote.
    internal Result<TOut> MapCore<TOut>(Func<T, TOut> map, string member)
    {
        if (this.tag != TagOk)
        {
            return new Result<TOut>(this.FailureError);
        }

        return ResultErrors.IsMissing(map) ? new Result<TOut>(ResultErrors.NullDelegate(nameof(map), member)) : new Result<TOut>(map(this.resultValue));
    }

    /// <summary>Chains an operation that itself returns a <see cref="Result{T}"/>; a failure short-circuits.</summary>
    /// <typeparam name="TOut">The value type of the chained result.</typeparam>
    /// <param name="bind">The next step. A <see langword="null"/> value turns a success into a
    /// <c>result.null_delegate</c> failure.</param>
    /// <returns>The result of <paramref name="bind"/>, or the original failure without calling it.</returns>
    public Result<TOut> Bind<TOut>(Func<T, Result<TOut>> bind)
    {
        using var activity = FunctionalDiagnostics.Source.StartActivity();

        return this.BindCore(bind, nameof(this.Bind));
    }

    // Bind's logic without a span, so the sync-delegate BindAsync overload runs it inside its own span
    // and a null delegate names the public method the caller wrote.
    internal Result<TOut> BindCore<TOut>(Func<T, Result<TOut>> bind, string member)
    {
        if (this.tag != TagOk)
        {
            return new Result<TOut>(this.FailureError);
        }

        return ResultErrors.IsMissing(bind) ? new Result<TOut>(ResultErrors.NullDelegate(nameof(bind), member)) : bind(this.resultValue);
    }

    /// <summary>Transforms the error of a failure; a success passes through unchanged.</summary>
    /// <param name="map">The transformation. It receives <see cref="Error.Uninitialized"/> for <c>default</c>.
    /// A <see langword="null"/> return becomes <see cref="Error.Uninitialized"/>. A <see langword="null"/> delegate
    /// turns a failure into a <c>result.null_delegate</c> failure.</param>
    /// <returns>The original success, or a failure with the mapped error.</returns>
    public Result<T> MapError(Func<Error, Error> map)
    {
        using var activity = FunctionalDiagnostics.Source.StartActivity();

        if (this.tag == TagOk)
        {
            return this;
        }

        return ResultErrors.IsMissing(map) ? new Result<T>(ResultErrors.NullDelegate(nameof(map))) : new Result<T>(map(this.FailureError));
    }

    /// <summary>Turns a success into the given failure when the predicate is false.</summary>
    /// <param name="predicate">The check on the value. A <see langword="null"/> value turns a success into a
    /// <c>result.null_delegate</c> failure.</param>
    /// <param name="error">The error returned when the predicate is false. A <see langword="null"/> value becomes
    /// <see cref="Error.Uninitialized"/>.</param>
    /// <returns>This result if it is a failure or the predicate holds; otherwise a failure with <paramref name="error"/>.</returns>
    /// <remarks>The <see cref="Error"/> is built before the call and is recorded in telemetry when built, even if the
    /// predicate holds; prefer the factory overload <see cref="Ensure(Func{T, bool}, Func{T, Error})"/>.</remarks>
    public Result<T> Ensure(Func<T, bool> predicate, Error error)
    {
        using var activity = FunctionalDiagnostics.Source.StartActivity();

        return this.EnsureCore(predicate, error, nameof(this.Ensure));
    }

    // Ensure's logic without a span, so the sync-delegate EnsureAsync overload runs it inside its own span
    // and a null delegate names the public method the caller wrote.
    internal Result<T> EnsureCore(Func<T, bool> predicate, Error error, string member)
    {
        if (this.tag != TagOk)
        {
            return this;
        }

        if (ResultErrors.IsMissing(predicate))
        {
            return new Result<T>(ResultErrors.NullDelegate(nameof(predicate), member));
        }

        return predicate(this.resultValue) ? this : new Result<T>(error);
    }

    /// <summary>Turns a success into a failure built by <paramref name="error"/> when the predicate is false.</summary>
    /// <param name="predicate">The check on the value. A <see langword="null"/> value turns a success into a
    /// <c>result.null_delegate</c> failure.</param>
    /// <param name="error">Builds the error from the value; called only when the predicate is false, so nothing is
    /// built or recorded on the success path. A <see langword="null"/> value gives a <c>result.null_delegate</c>
    /// failure when it would be called; a <see langword="null"/> return becomes <see cref="Error.Uninitialized"/>.</param>
    /// <returns>This result if it is a failure or the predicate holds; otherwise a failure with the built error.</returns>
    // RS0027: the (predicate, Func<T, Error>) overload is a distinct type-based overload. Priority -1 keeps a null
    // literal binding to the Error overload; lambdas and method groups still reach this one.
#pragma warning disable RS0027
    [OverloadResolutionPriority(-1)]
    public Result<T> Ensure(Func<T, bool> predicate, Func<T, Error> error)
    {
        using var activity = FunctionalDiagnostics.Source.StartActivity();

        return this.EnsureLazyCore(predicate, error, nameof(this.Ensure));
    }

    // Ensure's logic without a span, so the sync-delegate EnsureAsync overload runs it inside its own span
    // and a null delegate names the public method the caller wrote.
    internal Result<T> EnsureLazyCore(Func<T, bool> predicate, Func<T, Error> error, string member)
    {
        if (this.tag != TagOk)
        {
            return this;
        }

        if (ResultErrors.IsMissing(predicate))
        {
            return new Result<T>(ResultErrors.NullDelegate(nameof(predicate), member));
        }

        if (predicate(this.resultValue))
        {
            return this;
        }

        return ResultErrors.IsMissing(error) ? new Result<T>(ResultErrors.NullDelegate(nameof(error), member)) : new Result<T>(error(this.resultValue));
    }
#pragma warning restore RS0027

    /// <summary>Turns a success into a validation failure when the predicate is false, describing it with the
    /// source text of the predicate.</summary>
    /// <param name="predicate">The check on the value. A <see langword="null"/> value turns a success into a
    /// <c>result.null_delegate</c> failure.</param>
    /// <param name="expression">Filled in by the compiler with the source text of <paramref name="predicate"/>;
    /// do not pass it. When <see langword="null"/>, the message is generic.</param>
    /// <returns>This result if it is a failure or the predicate holds; otherwise a <see cref="ErrorKind.Validation"/>
    /// failure with code <c>result.ensure_failed</c> and <paramref name="expression"/> as its message.</returns>
    /// <remarks>
    /// Hidden from IntelliSense: the message is source code (<c>"u =&gt; u.Age &gt;= 13"</c>), and an HTTP mapping can
    /// show it to users. Prefer <see cref="Ensure(Func{T, bool}, Func{T, Error})"/> with an error that has a stable
    /// code and a human message.
    /// </remarks>
    // RS0027: the (predicate, Error) overload is a distinct type-based overload, not a later optional-parameter addition.
#pragma warning disable RS0027
    [OverloadResolutionPriority(-1)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Result<T> Ensure(Func<T, bool> predicate, [CallerArgumentExpression(nameof(predicate))] string? expression = null)
    {
        using var activity = FunctionalDiagnostics.Source.StartActivity();

        if (this.tag != TagOk)
        {
            return this;
        }

        if (ResultErrors.IsMissing(predicate))
        {
            return new Result<T>(ResultErrors.NullDelegate(nameof(predicate)));
        }

        return predicate(this.resultValue)
            ? this
            : new Result<T>(Error.Validation(ResultErrors.EnsureFailedCode, expression ?? "The predicate returned false."));
    }
#pragma warning restore RS0027

    /// <summary>Runs a side effect on the success value and returns this result unchanged.</summary>
    /// <param name="action">The side effect. A <see langword="null"/> value turns a success into a
    /// <c>result.null_delegate</c> failure.</param>
    /// <returns>This result.</returns>
    public Result<T> Tap(Action<T> action)
    {
        using var activity = FunctionalDiagnostics.Source.StartActivity();

        if (this.tag != TagOk)
        {
            return this;
        }

        if (ResultErrors.IsMissing(action))
        {
            return new Result<T>(ResultErrors.NullDelegate(nameof(action)));
        }

        action(this.resultValue);
        return this;
    }

    /// <summary>Runs a side effect on the error of a failure and returns this result unchanged.</summary>
    /// <param name="action">The side effect. It receives <see cref="Error.Uninitialized"/> for <c>default</c>.
    /// A <see langword="null"/> value turns a failure into a <c>result.null_delegate</c> failure.</param>
    /// <returns>This result.</returns>
    public Result<T> TapError(Action<Error> action)
    {
        using var activity = FunctionalDiagnostics.Source.StartActivity();

        if (this.tag == TagOk)
        {
            return this;
        }

        if (ResultErrors.IsMissing(action))
        {
            return new Result<T>(ResultErrors.NullDelegate(nameof(action)));
        }

        action(this.FailureError);
        return this;
    }

    /// <summary>Gets the success value, or a fallback for a failure (including <c>default</c>).</summary>
    /// <param name="fallback">The value returned when this result is not a success.</param>
    /// <returns>The value or <paramref name="fallback"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T GetValueOrDefault(T fallback) => this.tag == TagOk ? this.resultValue : fallback;
}
