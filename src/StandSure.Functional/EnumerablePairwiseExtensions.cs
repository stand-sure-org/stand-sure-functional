using System.Collections.Generic;

namespace StandSure.Functional;

/// <summary>The <c>Pairwise</c> sequence helper.</summary>
public static class EnumerablePairwiseExtensions
{
    /// <summary>Yields each element together with the one before it.</summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="source">The elements. A <see langword="null"/> value gives an empty sequence and records a
    /// <c>result.null_source</c> error in telemetry; it never throws.</param>
    /// <returns>For <c>[1, 2, 3]</c>: <c>(1, 2), (2, 3)</c>. An empty or single-element source gives an empty sequence.
    /// Lazy: nothing is enumerated until the result is iterated, and the source is enumerated once.</returns>
    public static IEnumerable<(T Previous, T Current)> Pairwise<T>(this IEnumerable<T> source)
    {
        if (source is null)
        {
            _ = ResultErrors.NullSource(nameof(source));
            return [];
        }

        return Iterate(source);

        static IEnumerable<(T Previous, T Current)> Iterate(IEnumerable<T> source)
        {
            using var e = source.GetEnumerator();
            if (!e.MoveNext())
            {
                yield break;
            }

            var previous = e.Current;
            while (e.MoveNext())
            {
                var current = e.Current;
                yield return (previous, current);
                previous = current;
            }
        }
    }
}
