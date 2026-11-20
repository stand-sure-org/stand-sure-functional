using System.Collections.Generic;

namespace StandSure.Functional;

/// <summary>Collection helpers for sequences of <see cref="Option{T}"/>.</summary>
public static class OptionEnumerableExtensions
{
    /// <summary>Yields the value of every some, skipping every none (including <c>default</c>).</summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="source">The options. A <see langword="null"/> value gives an empty sequence and records a
    /// <c>result.null_source</c> error in telemetry; it never throws.</param>
    /// <returns>A lazy sequence: nothing is enumerated until the result is iterated, and the source is enumerated once.</returns>
    public static IEnumerable<T> Choose<T>(this IEnumerable<Option<T>>? source)
    {
        if (source is null)
        {
            _ = ResultErrors.NullSource(nameof(source));
            return [];
        }

        return Iterate(source);

        static IEnumerable<T> Iterate(IEnumerable<Option<T>> source)
        {
            foreach (var option in source)
            {
                if (option.TryGetValue(out T? value))
                {
                    yield return value;
                }
            }
        }
    }
}
