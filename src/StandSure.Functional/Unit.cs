using System;

namespace StandSure.Functional;

/// <summary>
/// The type with exactly one value, used where a result carries no information
/// (the functional equivalent of <see langword="void"/>).
/// </summary>
/// <remarks>
/// Inspired by the <c>Unit</c> type in language-ext by Paul Louth (MIT licensed).
/// </remarks>
public readonly struct Unit : IEquatable<Unit>, IComparable<Unit>
{
    /// <summary>The single <see cref="Unit"/> value.</summary>
    // ReSharper disable once UnassignedReadonlyField
    public static readonly Unit Value;

    /// <summary>Always <see langword="true"/>: every <see cref="Unit"/> is equal.</summary>
    /// <param name="other">The other value.</param>
    /// <returns><see langword="true"/>.</returns>
    public bool Equals(Unit other) => true;

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is Unit;

    /// <inheritdoc />
    public override int GetHashCode() => 0;

    /// <summary>Always <c>0</c>: every <see cref="Unit"/> compares equal.</summary>
    /// <param name="other">The other value.</param>
    /// <returns>Zero.</returns>
    public int CompareTo(Unit other) => 0;

    /// <inheritdoc />
    public override string ToString() => "()";

    /// <summary>Always <see langword="true"/>.</summary>
    /// <param name="left">Left operand.</param>
    /// <param name="right">Right operand.</param>
    /// <returns><see langword="true"/>.</returns>
    public static bool operator ==(Unit left, Unit right) => true;

    /// <summary>Always <see langword="false"/>.</summary>
    /// <param name="left">Left operand.</param>
    /// <param name="right">Right operand.</param>
    /// <returns><see langword="false"/>.</returns>
    public static bool operator !=(Unit left, Unit right) => false;
}
