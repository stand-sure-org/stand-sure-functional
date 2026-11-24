using System;
using System.Collections.Generic;

namespace StandSure.Functional;

public readonly partial struct Result<T> : IEquatable<Result<T>>
{
    /// <summary>Compares two results by their contents.</summary>
    /// <remarks>
    /// Two successes are equal when their values are (<see cref="EqualityComparer{T}.Default"/>); two failures are equal
    /// when their errors are. <c>default(Result&lt;T&gt;)</c> equals a failure holding <see cref="Error.Uninitialized"/>.
    /// </remarks>
    /// <param name="left">The first result.</param>
    /// <param name="right">The second result.</param>
    /// <returns><see langword="true"/> when both hold the same outcome.</returns>
    public static bool operator ==(Result<T> left, Result<T> right) => left.Equals(right);

    /// <summary>Compares two results by their contents.</summary>
    /// <param name="left">The first result.</param>
    /// <param name="right">The second result.</param>
    /// <returns><see langword="true"/> when they hold different outcomes.</returns>
    public static bool operator !=(Result<T> left, Result<T> right) => !left.Equals(right);

    /// <summary>Compares this result with another by their contents.</summary>
    /// <param name="other">The result to compare with.</param>
    /// <returns><see langword="true"/> when both are successes with equal values, or both are failures with equal errors.</returns>
    public bool Equals(Result<T> other)
    {
        if (this.IsSuccess != other.IsSuccess)
        {
            return false;
        }

        return this.IsSuccess
            ? EqualityComparer<T>.Default.Equals(this.resultValue, other.resultValue)
            : this.FailureError.Equals(other.FailureError);
    }

    /// <summary>Compares this result with an object.</summary>
    /// <param name="obj">The object to compare with.</param>
    /// <returns><see langword="true"/> when <paramref name="obj"/> is a <see cref="Result{T}"/> holding the same outcome.</returns>
    public override bool Equals(object? obj) => obj is Result<T> other && this.Equals(other);

    /// <summary>Gets a hash code that agrees with <see cref="Equals(Result{T})"/>.</summary>
    /// <returns>The hash code.</returns>
    public override int GetHashCode() =>
        this.IsSuccess ? HashCode.Combine(true, this.resultValue) : HashCode.Combine(false, this.FailureError);
}
