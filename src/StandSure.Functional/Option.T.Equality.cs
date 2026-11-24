using System;
using System.Collections.Generic;

namespace StandSure.Functional;

public readonly partial struct Option<T> : IEquatable<Option<T>>
{
    /// <summary>Compares two options by their contents.</summary>
    /// <remarks>Every none is equal to every other none; two somes are equal when their values are
    /// (<see cref="EqualityComparer{T}.Default"/>).</remarks>
    /// <param name="left">The first option.</param>
    /// <param name="right">The second option.</param>
    /// <returns><see langword="true"/> when both are none, or both hold equal values.</returns>
    public static bool operator ==(Option<T> left, Option<T> right) => left.Equals(right);

    /// <summary>Compares two options by their contents.</summary>
    /// <param name="left">The first option.</param>
    /// <param name="right">The second option.</param>
    /// <returns><see langword="true"/> when one is none and the other is not, or both hold different values.</returns>
    public static bool operator !=(Option<T> left, Option<T> right) => !left.Equals(right);

    /// <summary>Compares this option with another by their contents.</summary>
    /// <param name="other">The option to compare with.</param>
    /// <returns><see langword="true"/> when both are none, or both hold equal values.</returns>
    public bool Equals(Option<T> other)
    {
        if (this.isSome != other.isSome)
        {
            return false;
        }

        return !this.isSome || EqualityComparer<T>.Default.Equals(this.optionValue, other.optionValue);
    }

    /// <summary>Compares this option with an object.</summary>
    /// <param name="obj">The object to compare with.</param>
    /// <returns><see langword="true"/> when <paramref name="obj"/> is an <see cref="Option{T}"/> with the same contents.</returns>
    public override bool Equals(object? obj) => obj is Option<T> other && this.Equals(other);

    /// <summary>Gets a hash code that agrees with <see cref="Equals(Option{T})"/>.</summary>
    /// <returns>The hash code.</returns>
    public override int GetHashCode() => this.isSome ? HashCode.Combine(true, this.optionValue) : 0;
}
