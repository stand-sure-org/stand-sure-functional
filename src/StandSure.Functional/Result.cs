using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace StandSure.Functional;

/// <summary>Factory methods for <see cref="Result{T}"/>.</summary>
public static class Result
{
    /// <summary>Creates a success.</summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="value">The value. When <typeparamref name="T"/> is <see cref="Error"/> or <see cref="object"/>,
    /// the result is still a success, whatever the value's runtime type.</param>
    /// <returns>A successful result.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T> Success<T>(T value) => new(value);

    /// <summary>Creates a failure.</summary>
    /// <typeparam name="T">The value type the result would have held.</typeparam>
    /// <param name="error">The error. A <see langword="null"/> error becomes <see cref="Error.Uninitialized"/>.</param>
    /// <returns>A failed result.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T> Failure<T>(Error error) => new(error);

    /// <summary>Creates a success that carries no value.</summary>
    /// <returns>A successful <see cref="Result{T}"/> of <see cref="Unit"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<Unit> Success() => new(Unit.Value);

    /// <summary>Alias for <see cref="Success{T}(T)"/>, kept for compatibility with common model output.
    /// <see cref="Success{T}(T)"/> is preferred.</summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="value">The value.</param>
    /// <returns>A successful result.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T> Ok<T>(T value) => Success(value);

    /// <summary>Alias for <see cref="Failure{T}(Error)"/>, kept for compatibility with common model output.
    /// <see cref="Failure{T}(Error)"/> is preferred.</summary>
    /// <typeparam name="T">The value type the result would have held.</typeparam>
    /// <param name="error">The error.</param>
    /// <returns>A failed result.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T> Fail<T>(Error error) => Failure<T>(error);

    /// <summary>Alias for <see cref="Success()"/>, kept for compatibility with common model output.
    /// <see cref="Success()"/> is preferred.</summary>
    /// <returns>A successful <see cref="Result{T}"/> of <see cref="Unit"/>.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<Unit> Ok() => Success();
}
