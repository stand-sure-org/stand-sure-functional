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
    public static Result<T> Ok<T>(T value) => new(value);

    /// <summary>Creates a failure.</summary>
    /// <typeparam name="T">The value type the result would have held.</typeparam>
    /// <param name="error">The error. A <see langword="null"/> error becomes <see cref="Error.Uninitialized"/>.</param>
    /// <returns>A failed result.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T> Fail<T>(Error error) => new(error);

    /// <summary>Creates a success that carries no value.</summary>
    /// <returns>A successful <see cref="Result{T}"/> of <see cref="Unit"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<Unit> Ok() => new(Unit.Value);
}
