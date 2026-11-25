using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace StandSure.Functional;

/// <summary>
/// Combinators for <see cref="Task{TResult}"/> of <see cref="Result{T}"/>, so a chain can mix awaited steps without
/// intermediate <c>await</c> statements.
/// </summary>
/// <remarks>
/// <para>
/// Each combinator comes in two forms: one takes an ordinary lambda, the other a lambda that returns a
/// <see cref="Task{TResult}"/>. An <c>async</c> lambda always binds to the second form. A lambda that merely returns
/// a task, such as <c>x =&gt; Task.FromResult(x)</c>, also binds to the second form, because it is the more specific
/// overload.
/// </para>
/// <para>
/// <c>MatchAsync</c> takes either two ordinary lambdas or two task-returning lambdas. Mixing one of each does not
/// compile (the compiler reports a misleading conversion error on the ordinary lambda); make both lambdas the same kind,
/// for example by wrapping the plain result in <c>Task.FromResult</c>.
/// </para>
/// <para>
/// All awaits use <c>ConfigureAwait(false)</c>. An exception thrown by the source task or by a lambda is not caught;
/// it faults the returned task. A <see langword="null"/> source task counts as a failure with code
/// <c>result.null_task</c>, and so does a <see langword="null"/> task returned by a lambda. A <see langword="null"/>
/// delegate behaves as it does on <see cref="Result{T}"/>: <c>result.null_delegate</c> failure, or
/// <c>default(TOut)</c> from <c>MatchAsync</c>, which also records a <c>result.null_delegate</c> failure in telemetry.
/// <c>MapErrorAsync</c> and <c>TapErrorAsync</c> keep the original failure when their delegate, or the task it
/// returns, is <see langword="null"/>, and record the problem in telemetry only (as <c>MapError</c> and <c>TapError</c> do). <c>default(Result&lt;T&gt;)</c> flows as the
/// <see cref="Error.Uninitialized"/> failure.
/// </para>
/// </remarks>
[StackTraceHidden]
public static class ResultTaskExtensions
{
    private const string SourceName = "source";

    /// <summary>Transforms the success value of an awaited result.</summary>
    /// <typeparam name="T">The input value type.</typeparam>
    /// <typeparam name="TOut">The output value type.</typeparam>
    /// <param name="source">The pending result.</param>
    /// <param name="map">The transformation.</param>
    /// <returns>The mapped result, or the original failure.</returns>
    public static async Task<Result<TOut>> MapAsync<T, TOut>(this Task<Result<T>> source, Func<T, TOut> map)
    {
        using var activity = FunctionalDiagnostics.Source.StartActivity();

        var result = await Resolve(source).ConfigureAwait(false);
        return result.MapCore(map, nameof(MapAsync));
    }

    /// <summary>Transforms the success value of an awaited result with an asynchronous function.</summary>
    /// <typeparam name="T">The input value type.</typeparam>
    /// <typeparam name="TOut">The output value type.</typeparam>
    /// <param name="source">The pending result.</param>
    /// <param name="map">The asynchronous transformation.</param>
    /// <returns>The mapped result, or the original failure.</returns>
    public static async Task<Result<TOut>> MapAsync<T, TOut>(this Task<Result<T>> source, Func<T, Task<TOut>?> map)
    {
        using var activity = FunctionalDiagnostics.Source.StartActivity();

        var result = await Resolve(source).ConfigureAwait(false);

        if (result.IsFailure)
        {
            return Result.Failure<TOut>(result.ErrorUnchecked);
        }

        if (ResultErrors.IsMissing(map))
        {
            return Result.Failure<TOut>(ResultErrors.NullDelegate(nameof(map)));
        }

        var task = map(result.ValueUnchecked);
        return task is null ? Result.Failure<TOut>(ResultErrors.NullTask(nameof(map))) : Result.Success(await task.ConfigureAwait(false));
    }

    /// <summary>Chains a result-returning step onto an awaited result.</summary>
    /// <typeparam name="T">The input value type.</typeparam>
    /// <typeparam name="TOut">The output value type.</typeparam>
    /// <param name="source">The pending result.</param>
    /// <param name="bind">The next step.</param>
    /// <returns>The result of <paramref name="bind"/>, or the original failure without calling it.</returns>
    public static async Task<Result<TOut>> BindAsync<T, TOut>(this Task<Result<T>> source, Func<T, Result<TOut>> bind)
    {
        using var activity = FunctionalDiagnostics.Source.StartActivity();

        var result = await Resolve(source).ConfigureAwait(false);
        return result.BindCore(bind, nameof(BindAsync));
    }

    /// <summary>Chains an asynchronous result-returning step onto an awaited result.</summary>
    /// <typeparam name="T">The input value type.</typeparam>
    /// <typeparam name="TOut">The output value type.</typeparam>
    /// <param name="source">The pending result.</param>
    /// <param name="bind">The next asynchronous step.</param>
    /// <returns>The result of <paramref name="bind"/>, or the original failure without calling it.</returns>
    public static async Task<Result<TOut>> BindAsync<T, TOut>(this Task<Result<T>> source, Func<T, Task<Result<TOut>>?> bind)
    {
        using var activity = FunctionalDiagnostics.Source.StartActivity();

        var result = await Resolve(source).ConfigureAwait(false);

        if (result.IsFailure)
        {
            return Result.Failure<TOut>(result.ErrorUnchecked);
        }

        if (ResultErrors.IsMissing(bind))
        {
            return Result.Failure<TOut>(ResultErrors.NullDelegate(nameof(bind)));
        }

        var task = bind(result.ValueUnchecked);

        if (task is null)
        {
            return Result.Failure<TOut>(ResultErrors.NullTask(nameof(bind)));
        }

        return await task.ConfigureAwait(false);
    }

    /// <summary>Runs one of two functions on an awaited result. The failure function comes first.</summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <typeparam name="TOut">The type both functions return.</typeparam>
    /// <param name="source">The pending result.</param>
    /// <param name="onFailure">Runs on a failure, including <c>default</c>.</param>
    /// <param name="onSuccess">Runs on a success.</param>
    /// <returns>The result of the function that ran, or <c>default(TOut)</c> when that function is <see langword="null"/>.</returns>
    public static async Task<TOut> MatchAsync<T, TOut>(
        this Task<Result<T>> source,
        Func<Error, TOut> onFailure,
        Func<T, TOut> onSuccess)
    {
        using var activity = FunctionalDiagnostics.Source.StartActivity();

        var result = await Resolve(source).ConfigureAwait(false);
        return result.MatchCore(onFailure, onSuccess, nameof(MatchAsync));
    }

    /// <summary>Runs one of two asynchronous functions on an awaited result. The failure function comes first.</summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <typeparam name="TOut">The type both functions return.</typeparam>
    /// <param name="source">The pending result.</param>
    /// <param name="onFailure">Runs on a failure, including <c>default</c>.</param>
    /// <param name="onSuccess">Runs on a success.</param>
    /// <returns>The result of the function that ran, or <c>default(TOut)</c> when that function or the task it
    /// returned is <see langword="null"/>.</returns>
    public static async Task<TOut> MatchAsync<T, TOut>(
        this Task<Result<T>> source,
        Func<Error, Task<TOut>> onFailure,
        Func<T, Task<TOut>?> onSuccess)
    {
        using var activity = FunctionalDiagnostics.Source.StartActivity();

        var result = await Resolve(source).ConfigureAwait(false);
        Task<TOut>? task;

        if (result.IsSuccess)
        {
            if (ResultErrors.IsMissing(onSuccess))
            {
                _ = ResultErrors.NullDelegate(nameof(onSuccess));
                return default!;
            }

            task = onSuccess(result.ValueUnchecked);
        }
        else
        {
            if (ResultErrors.IsMissing(onFailure))
            {
                _ = ResultErrors.NullDelegate(nameof(onFailure));
                return default!;
            }

            task = onFailure(result.ErrorUnchecked);
        }

        if (task is null)
        {
            _ = ResultErrors.NullTask("match");
            return default!;
        }

        return await task.ConfigureAwait(false);
    }

    /// <summary>Turns an awaited success into the given failure when the predicate is false.</summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="source">The pending result.</param>
    /// <param name="predicate">The check on the value.</param>
    /// <param name="error">The error returned when the predicate is false.</param>
    /// <returns>The original result when it is a failure or the predicate holds; otherwise a failure with <paramref name="error"/>.</returns>
    /// <remarks>The <see cref="Error"/> is built before the call and is recorded in telemetry when built, even if the
    /// predicate holds; prefer the factory overload <see cref="EnsureAsync{T}(Task{Result{T}}, Func{T, bool}, Func{T, Error})"/>.</remarks>
    public static async Task<Result<T>> EnsureAsync<T>(this Task<Result<T>> source, Func<T, bool> predicate, Error error)
    {
        using var activity = FunctionalDiagnostics.Source.StartActivity();

        var result = await Resolve(source).ConfigureAwait(false);
        return result.EnsureCore(predicate, error, nameof(EnsureAsync));
    }

    /// <summary>Turns an awaited success into the given failure when the asynchronous predicate is false.</summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="source">The pending result.</param>
    /// <param name="predicate">The asynchronous check on the value.</param>
    /// <param name="error">The error returned when the predicate is false.</param>
    /// <returns>The original result when it is a failure or the predicate holds; otherwise a failure with <paramref name="error"/>.</returns>
    /// <remarks>The <see cref="Error"/> is built before the call and is recorded in telemetry when built, even if the
    /// predicate holds; prefer the factory overload <see cref="EnsureAsync{T}(Task{Result{T}}, Func{T, Task{bool}}, Func{T, Error})"/>.</remarks>
    public static async Task<Result<T>> EnsureAsync<T>(this Task<Result<T>> source, Func<T, Task<bool>?> predicate, Error error)
    {
        using var activity = FunctionalDiagnostics.Source.StartActivity();

        var result = await Resolve(source).ConfigureAwait(false);

        if (result.IsFailure)
        {
            return result;
        }

        if (ResultErrors.IsMissing(predicate))
        {
            return Result.Failure<T>(ResultErrors.NullDelegate(nameof(predicate)));
        }

        var task = predicate(result.ValueUnchecked);

        if (task is null)
        {
            return Result.Failure<T>(ResultErrors.NullTask(nameof(predicate)));
        }

        return await task.ConfigureAwait(false) ? result : Result.Failure<T>(error);
    }

    /// <summary>Turns an awaited success into a failure built by <paramref name="error"/> when the predicate is false.</summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="source">The pending result.</param>
    /// <param name="predicate">The check on the value.</param>
    /// <param name="error">Builds the error from the value; called only when the predicate is false, so nothing is
    /// built or recorded on the success path. A <see langword="null"/> value gives a <c>result.null_delegate</c>
    /// failure when it would be called.</param>
    /// <returns>The original result when it is a failure or the predicate holds; otherwise a failure with the built error.</returns>
    // Priority -1 keeps a null literal binding to the Error overloads; lambdas and method groups still reach this one.
    [OverloadResolutionPriority(-1)]
    public static async Task<Result<T>> EnsureAsync<T>(this Task<Result<T>> source, Func<T, bool> predicate, Func<T, Error> error)
    {
        using var activity = FunctionalDiagnostics.Source.StartActivity();

        var result = await Resolve(source).ConfigureAwait(false);
        return result.EnsureLazyCore(predicate, error, nameof(EnsureAsync));
    }

    /// <summary>Turns an awaited success into a failure built by <paramref name="error"/> when the asynchronous predicate is false.</summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="source">The pending result.</param>
    /// <param name="predicate">The asynchronous check on the value.</param>
    /// <param name="error">Builds the error from the value; called only when the predicate is false, so nothing is
    /// built or recorded on the success path. A <see langword="null"/> value gives a <c>result.null_delegate</c>
    /// failure when it would be called.</param>
    /// <returns>The original result when it is a failure or the predicate holds; otherwise a failure with the built error.</returns>
    [OverloadResolutionPriority(-1)]
    public static async Task<Result<T>> EnsureAsync<T>(this Task<Result<T>> source, Func<T, Task<bool>?> predicate, Func<T, Error> error)
    {
        using var activity = FunctionalDiagnostics.Source.StartActivity();

        var result = await Resolve(source).ConfigureAwait(false);

        if (result.IsFailure)
        {
            return result;
        }

        if (ResultErrors.IsMissing(predicate))
        {
            return Result.Failure<T>(ResultErrors.NullDelegate(nameof(predicate)));
        }

        var task = predicate(result.ValueUnchecked);

        if (task is null)
        {
            return Result.Failure<T>(ResultErrors.NullTask(nameof(predicate)));
        }

        if (await task.ConfigureAwait(false))
        {
            return result;
        }

        return Result.Failure<T>(ResultErrors.IsMissing(error) ? ResultErrors.NullDelegate(nameof(error)) : error(result.ValueUnchecked));
    }

    /// <summary>Transforms the error of an awaited failure; a success passes through unchanged.</summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="source">The task producing the result.</param>
    /// <param name="map">The transformation. It receives <see cref="Error.Uninitialized"/> for <c>default</c>.
    /// A <see langword="null"/> return becomes <see cref="Error.Uninitialized"/>. A <see langword="null"/> delegate
    /// keeps the original failure and records a <c>result.null_delegate</c> failure in telemetry.</param>
    /// <returns>The original success, or a failure with the mapped error.</returns>
    public static async Task<Result<T>> MapErrorAsync<T>(this Task<Result<T>> source, Func<Error, Error> map)
    {
        using var activity = FunctionalDiagnostics.Source.StartActivity();

        var result = await Resolve(source).ConfigureAwait(false);
        return result.MapErrorCore(map, nameof(MapErrorAsync));
    }

    /// <summary>Transforms the error of an awaited failure with an asynchronous function.</summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="source">The task producing the result.</param>
    /// <param name="map">The transformation. It receives <see cref="Error.Uninitialized"/> for <c>default</c>.
    /// A task that yields <see langword="null"/> gives <see cref="Error.Uninitialized"/>. A <see langword="null"/>
    /// delegate, or a <see langword="null"/> task returned by it, keeps the original failure and records the problem
    /// (<c>result.null_delegate</c> or <c>result.null_task</c>) in telemetry.</param>
    /// <returns>The original success, or a failure with the mapped error.</returns>
    public static async Task<Result<T>> MapErrorAsync<T>(this Task<Result<T>> source, Func<Error, Task<Error>?> map)
    {
        using var activity = FunctionalDiagnostics.Source.StartActivity();

        var result = await Resolve(source).ConfigureAwait(false);

        if (result.IsSuccess)
        {
            return result;
        }

        if (ResultErrors.IsMissing(map))
        {
            _ = ResultErrors.NullDelegate(nameof(map));
            return result;
        }

        var task = map(result.ErrorUnchecked);

        if (task is null)
        {
            // Keep the caller's error, as for a missing delegate (#41); building the error records it.
            _ = ResultErrors.NullTask(nameof(map));
            return result;
        }

        return Result.Failure<T>(await task.ConfigureAwait(false));
    }

    /// <summary>Runs a side effect on the value of an awaited success and returns the result unchanged.</summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="source">The task producing the result.</param>
    /// <param name="action">The side effect. A <see langword="null"/> value turns a success into a
    /// <c>result.null_delegate</c> failure.</param>
    /// <returns>The awaited result.</returns>
    public static async Task<Result<T>> TapAsync<T>(this Task<Result<T>> source, Action<T> action)
    {
        using var activity = FunctionalDiagnostics.Source.StartActivity();

        var result = await Resolve(source).ConfigureAwait(false);
        return result.TapCore(action, nameof(TapAsync));
    }

    /// <summary>Runs an asynchronous side effect on the value of an awaited success and returns the result
    /// unchanged.</summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="source">The task producing the result.</param>
    /// <param name="action">The side effect. A <see langword="null"/> value turns a success into a
    /// <c>result.null_delegate</c> failure, and a <see langword="null"/> task returned by it into a
    /// <c>result.null_task</c> failure.</param>
    /// <returns>The awaited result.</returns>
    public static async Task<Result<T>> TapAsync<T>(this Task<Result<T>> source, Func<T, Task?> action)
    {
        using var activity = FunctionalDiagnostics.Source.StartActivity();

        var result = await Resolve(source).ConfigureAwait(false);

        if (result.IsFailure)
        {
            return result;
        }

        if (ResultErrors.IsMissing(action))
        {
            return Result.Failure<T>(ResultErrors.NullDelegate(nameof(action)));
        }

        var task = action(result.ValueUnchecked);

        if (task is null)
        {
            return Result.Failure<T>(ResultErrors.NullTask(nameof(action)));
        }

        await task.ConfigureAwait(false);
        return result;
    }

    /// <summary>Runs a side effect on the error of an awaited failure and returns the result unchanged.</summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="source">The task producing the result.</param>
    /// <param name="action">The side effect. It receives <see cref="Error.Uninitialized"/> for <c>default</c>.
    /// A <see langword="null"/> value keeps the failure unchanged and records a <c>result.null_delegate</c> failure
    /// in telemetry.</param>
    /// <returns>The awaited result.</returns>
    public static async Task<Result<T>> TapErrorAsync<T>(this Task<Result<T>> source, Action<Error> action)
    {
        using var activity = FunctionalDiagnostics.Source.StartActivity();

        var result = await Resolve(source).ConfigureAwait(false);
        return result.TapErrorCore(action, nameof(TapErrorAsync));
    }

    /// <summary>Runs an asynchronous side effect on the error of an awaited failure (for example a rollback) and
    /// returns the result unchanged.</summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="source">The task producing the result.</param>
    /// <param name="action">The side effect. It receives <see cref="Error.Uninitialized"/> for <c>default</c>.
    /// A <see langword="null"/> value, or a <see langword="null"/> task returned by it, keeps the failure unchanged
    /// and records the problem (<c>result.null_delegate</c> or <c>result.null_task</c>) in telemetry.</param>
    /// <returns>The awaited result.</returns>
    public static async Task<Result<T>> TapErrorAsync<T>(this Task<Result<T>> source, Func<Error, Task?> action)
    {
        using var activity = FunctionalDiagnostics.Source.StartActivity();

        var result = await Resolve(source).ConfigureAwait(false);

        if (result.IsSuccess)
        {
            return result;
        }

        if (ResultErrors.IsMissing(action))
        {
            _ = ResultErrors.NullDelegate(nameof(action));
            return result;
        }

        var task = action(result.ErrorUnchecked);

        if (task is null)
        {
            // Observational: keep the caller's error (#41); building the error records it.
            _ = ResultErrors.NullTask(nameof(action));
            return result;
        }

        await task.ConfigureAwait(false);
        return result;
    }

    // A null source task is a failure rather than a NullReferenceException.
    private static Task<Result<T>> Resolve<T>(Task<Result<T>>? source) =>
        source ?? Task.FromResult(Result.Failure<T>(ResultErrors.NullTask(SourceName)));
}
