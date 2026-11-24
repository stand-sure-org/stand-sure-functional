using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.ExceptionServices;

namespace StandSure.Functional;

/// <summary>
/// Bridges code that throws into <see cref="Result{T}"/>. This is the only place in the library that catches exceptions.
/// </summary>
/// <remarks>
/// <para>
/// A thrown exception becomes a failure built with <see cref="Error.FromException(Exception)"/>, with these exceptions:
/// <see cref="OperationCanceledException"/> (including <see cref="TaskCanceledException"/>) and
/// <see cref="OutOfMemoryException"/> always propagate. An <see cref="AggregateException"/> whose flattened inner
/// exceptions are all <see cref="OperationCanceledException"/> (for example from <c>Task.Wait()</c>) propagates as its
/// first inner cancellation, with the original stack trace. Any other <see cref="AggregateException"/> becomes a failure.
/// </para>
/// <para>A <see langword="null"/> delegate returns a failure with code <c>result.null_delegate</c> instead of throwing,
/// and a <c>RunAsync</c> delegate that returns a <see langword="null"/> task returns a <c>result.null_task</c> failure.</para>
/// </remarks>
public static class Try
{
    private const string AsyncDelegateMessage = "Use Try.RunAsync(ct => ..., ct) for asynchronous work.";
    private const string AsyncDelegateCode = "try.async_delegate";

    /// <summary>Runs a function and captures a thrown exception as a failure.</summary>
    /// <typeparam name="T">The result type.</typeparam>
    /// <param name="fn">The function to run. A <see langword="null"/> value returns a <c>result.null_delegate</c> failure.</param>
    /// <returns>The function's value as a success, or the captured exception as a failure.</returns>
    public static Result<T?> Run<T>(Func<T?>? fn)
    {
        using var activity = FunctionalDiagnostics.Source.StartActivity();

        if (fn is null)
        {
            return Result.Failure<T?>(ResultErrors.NullDelegate(nameof(fn)));
        }

        try
        {
            return Result.Success(fn());
        }
        catch (AggregateException ae) when (AllCancellation(ae))
        {
            ThrowFirstCancellation(ae);
            return default;
        }
        catch (Exception ex) when (IsCapturable(ex))
        {
            return Result.Failure<T?>(Error.FromException(ex));
        }
    }

    /// <summary>Runs an action and captures a thrown exception as a failure.</summary>
    /// <param name="action">The action to run. A <see langword="null"/> value returns a <c>result.null_delegate</c> failure.</param>
    /// <returns>A success holding <see cref="Unit.Value"/>, or the captured exception as a failure.</returns>
    public static Result<Unit> Run(Action? action)
    {
        using var activity = FunctionalDiagnostics.Source.StartActivity();

        if (action is null)
        {
            return Result.Failure<Unit>(ResultErrors.NullDelegate(nameof(action)));
        }

        try
        {
            action();
            return Result.Success();
        }
        catch (AggregateException ae) when (AllCancellation(ae))
        {
            ThrowFirstCancellation(ae);
            return default;
        }
        catch (Exception ex) when (IsCapturable(ex))
        {
            return Result.Failure<Unit>(Error.FromException(ex));
        }
    }

    /// <summary>Not supported: an asynchronous delegate would succeed as soon as it returned its task, before the work
    /// ran or failed. Calling it does not compile; use <see cref="RunAsync(Func{CancellationToken, Task}, CancellationToken)"/>.</summary>
    /// <param name="fn">Never invoked.</param>
    /// <returns>A <c>try.async_delegate</c> failure (reachable only through reflection).</returns>
    [Obsolete(AsyncDelegateMessage, error: true)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Result<Unit> Run(Func<Task> fn) => Result.Failure<Unit>(AsyncDelegate());

    /// <summary>Not supported: an asynchronous delegate would succeed as soon as it returned its task, before the work
    /// ran or failed. Calling it does not compile; use <see cref="RunAsync{T}(Func{CancellationToken, Task{T}}, CancellationToken)"/>.</summary>
    /// <typeparam name="T">The task's result type.</typeparam>
    /// <param name="fn">Never invoked.</param>
    /// <returns>A <c>try.async_delegate</c> failure (reachable only through reflection).</returns>
    [Obsolete(AsyncDelegateMessage, error: true)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Result<T> Run<T>(Func<Task<T>> fn) => Result.Failure<T>(AsyncDelegate());

    /// <summary>Runs an asynchronous function and captures a thrown exception as a failure.</summary>
    /// <typeparam name="T">The result type.</typeparam>
    /// <param name="fn">The function to run; it receives <paramref name="cancellationToken"/>.
    /// A <see langword="null"/> value returns a <c>result.null_delegate</c> failure.</param>
    /// <param name="cancellationToken">Passed to <paramref name="fn"/>.</param>
    /// <returns>The task's value as a success, or the captured exception as a failure. An exception thrown
    /// synchronously by <paramref name="fn"/> is captured the same way as one thrown inside the task. A
    /// <see langword="null"/> task returns a <c>result.null_task</c> failure.</returns>
    [SuppressMessage("ApiDesign",
        "RS0026:Do not add multiple overloads with optional parameters",
        Justification = "Specified API: the overloads differ by delegate type and are new, so there is no backcompat risk.")]
    [SuppressMessage("ApiDesign",
        "RS0027:API with optional parameter(s) should have the most parameters amongst its public overloads",
        Justification = "Specified API.")]
    public static async Task<Result<T>> RunAsync<T>(
        Func<CancellationToken, Task<T>?>? fn,
        CancellationToken cancellationToken = default)
    {
        using var activity = FunctionalDiagnostics.Source.StartActivity();

        if (fn is null)
        {
            return Result.Failure<T>(ResultErrors.NullDelegate(nameof(fn)));
        }

        try
        {
            var task = fn(cancellationToken);

            if (task is null)
            {
                return Result.Failure<T>(ResultErrors.NullTask(nameof(fn)));
            }

            T value = await task.ConfigureAwait(false);
            return Result.Success(value);
        }
        catch (AggregateException ae) when (AllCancellation(ae))
        {
            ThrowFirstCancellation(ae);
            return default;
        }
        catch (Exception ex) when (IsCapturable(ex))
        {
            return Result.Failure<T>(Error.FromException(ex));
        }
    }

    /// <summary>Runs an asynchronous function that returns no value and captures a thrown exception as a failure.</summary>
    /// <param name="fn">The function to run; it receives <paramref name="cancellationToken"/>.
    /// A <see langword="null"/> value returns a <c>result.null_delegate</c> failure.</param>
    /// <param name="cancellationToken">Passed to <paramref name="fn"/>.</param>
    /// <returns>A success holding <see cref="Unit.Value"/>, or the captured exception as a failure.</returns>
    [SuppressMessage("ApiDesign",
        "RS0026:Do not add multiple overloads with optional parameters",
        Justification = "Specified API: the overloads differ by delegate type and are new, so there is no backcompat risk.")]
    [SuppressMessage("ApiDesign",
        "RS0027:API with optional parameter(s) should have the most parameters amongst its public overloads",
        Justification = "Specified API.")]
    public static async Task<Result<Unit>> RunAsync(
        Func<CancellationToken, Task?>? fn,
        CancellationToken cancellationToken = default)
    {
        using var activity = FunctionalDiagnostics.Source.StartActivity();

        if (fn is null)
        {
            return Result.Failure<Unit>(ResultErrors.NullDelegate(nameof(fn)));
        }

        try
        {
            var task = fn(cancellationToken);

            if (task is null)
            {
                return Result.Failure<Unit>(ResultErrors.NullTask(nameof(fn)));
            }

            await task.ConfigureAwait(false);
            return Result.Success();
        }
        catch (AggregateException ae) when (AllCancellation(ae))
        {
            ThrowFirstCancellation(ae);
            return default;
        }
        catch (Exception ex) when (IsCapturable(ex))
        {
            return Result.Failure<Unit>(Error.FromException(ex));
        }
    }

    private static Error AsyncDelegate() =>
        Error.Unexpected(AsyncDelegateCode, "Try.Run was given an asynchronous delegate. " + AsyncDelegateMessage);

    private static bool IsCapturable(Exception ex) =>
        ex is not OperationCanceledException and not OutOfMemoryException;

    private static bool AllCancellation(AggregateException ae)
    {
        IReadOnlyList<Exception> inner = ae.Flatten().InnerExceptions;

        if (inner.Count == 0)
        {
            return false;
        }

        foreach (Exception e in inner)
        {
            if (e is not OperationCanceledException)
            {
                return false;
            }
        }

        return true;
    }

    private static void ThrowFirstCancellation(AggregateException ae) =>
        ExceptionDispatchInfo.Capture(ae.Flatten().InnerExceptions[0]).Throw();
}
