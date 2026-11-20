using System.Runtime.CompilerServices;

namespace StandSure.Functional;

/// <summary>Factory methods for <see cref="Option{T}"/>.</summary>
public static class Option
{
    /// <summary>Creates an option holding a value.</summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="value">The value. A <see langword="null"/> value produces <see cref="Option{T}.None"/>.</param>
    /// <returns>An option that is some when <paramref name="value"/> is not <see langword="null"/>; otherwise none.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> Some<T>(T? value) => new(value);

    /// <summary>Creates an option holding nothing.</summary>
    /// <typeparam name="T">The value type the option would have held.</typeparam>
    /// <returns>The none option.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> None<T>() => default;
}
