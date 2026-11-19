using System;
using System.Collections.Generic;
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
/// <para>A <see langword="null"/> delegate returns a failure with code <c>try.null_delegate</c> instead of throwing.</para>
/// </remarks>
public static class Try
{
    private const string NullDelegateCode = "try.null_delegate";

    /// <summary>Runs a function and captures a thrown exception as a failure.</summary>
    /// <typeparam name="T">The result type.</typeparam>
    /// <param name="fn">The function to run. A <see langword="null"/> value returns a <c>try.null_delegate</c> failure.</param>
    /// <returns>The function's value as a success, or the captured exception as a failure.</returns>
    public static Result<T> Run<T>(Func<T> fn)
    {
        if (fn is null)
        {
            return Result.Fail<T>(NullDelegate());
        }

        try
        {
            return Result.Ok(fn());
        }
        catch (AggregateException ae) when (AllCancellation(ae))
        {
            ThrowFirstCancellation(ae);
            return default;
        }
        catch (Exception ex) when (IsCapturable(ex))
        {
            return Result.Fail<T>(Error.FromException(ex));
        }
    }

    /// <summary>Runs an action and captures a thrown exception as a failure.</summary>
    /// <param name="action">The action to run. A <see langword="null"/> value returns a <c>try.null_delegate</c> failure.</param>
    /// <returns>A success holding <see cref="Unit.Value"/>, or the captured exception as a failure.</returns>
    public static Result<Unit> Run(Action action)
    {
        if (action is null)
        {
            return Result.Fail<Unit>(NullDelegate());
        }

        try
        {
            action();
            return Result.Ok();
        }
        catch (AggregateException ae) when (AllCancellation(ae))
        {
            ThrowFirstCancellation(ae);
            return default;
        }
        catch (Exception ex) when (IsCapturable(ex))
        {
            return Result.Fail<Unit>(Error.FromException(ex));
        }
    }

    /// <summary>Runs an asynchronous function and captures a thrown exception as a failure.</summary>
    /// <typeparam name="T">The result type.</typeparam>
    /// <param name="fn">The function to run; it receives <paramref name="cancellationToken"/>.
    /// A <see langword="null"/> value returns a <c>try.null_delegate</c> failure.</param>
    /// <param name="cancellationToken">Passed to <paramref name="fn"/>.</param>
    /// <returns>The task's value as a success, or the captured exception as a failure. An exception thrown
    /// synchronously by <paramref name="fn"/> is captured the same way as one thrown inside the task.</returns>
    [SuppressMessage("ApiDesign", "RS0026:Do not add multiple overloads with optional parameters", Justification = "Specified API: the overloads differ by delegate type and are new, so there is no backcompat risk.")]
    [SuppressMessage("ApiDesign", "RS0027:API with optional parameter(s) should have the most parameters amongst its public overloads", Justification = "Specified API.")]
    public static async Task<Result<T>> RunAsync<T>(
        Func<CancellationToken, Task<T>> fn,
        CancellationToken cancellationToken = default)
    {
        if (fn is null)
        {
            return Result.Fail<T>(NullDelegate());
        }

        try
        {
            T value = await fn(cancellationToken).ConfigureAwait(false);
            return Result.Ok(value);
        }
        catch (AggregateException ae) when (AllCancellation(ae))
        {
            ThrowFirstCancellation(ae);
            return default;
        }
        catch (Exception ex) when (IsCapturable(ex))
        {
            return Result.Fail<T>(Error.FromException(ex));
        }
    }

    /// <summary>Runs an asynchronous function that returns no value and captures a thrown exception as a failure.</summary>
    /// <param name="fn">The function to run; it receives <paramref name="cancellationToken"/>.
    /// A <see langword="null"/> value returns a <c>try.null_delegate</c> failure.</param>
    /// <param name="cancellationToken">Passed to <paramref name="fn"/>.</param>
    /// <returns>A success holding <see cref="Unit.Value"/>, or the captured exception as a failure.</returns>
    [SuppressMessage("ApiDesign", "RS0026:Do not add multiple overloads with optional parameters", Justification = "Specified API: the overloads differ by delegate type and are new, so there is no backcompat risk.")]
    [SuppressMessage("ApiDesign", "RS0027:API with optional parameter(s) should have the most parameters amongst its public overloads", Justification = "Specified API.")]
    public static async Task<Result<Unit>> RunAsync(
        Func<CancellationToken, Task> fn,
        CancellationToken cancellationToken = default)
    {
        if (fn is null)
        {
            return Result.Fail<Unit>(NullDelegate());
        }

        try
        {
            await fn(cancellationToken).ConfigureAwait(false);
            return Result.Ok();
        }
        catch (AggregateException ae) when (AllCancellation(ae))
        {
            ThrowFirstCancellation(ae);
            return default;
        }
        catch (Exception ex) when (IsCapturable(ex))
        {
            return Result.Fail<Unit>(Error.FromException(ex));
        }
    }

    private static Error NullDelegate() =>
        Error.Unexpected(NullDelegateCode, "The delegate passed to Try was null.");

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
