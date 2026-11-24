using System;
using System.Collections.Generic;

namespace StandSure.Functional;

/// <summary>The <c>Scan</c> sequence helper.</summary>
public static class EnumerableScanExtensions
{
    /// <summary>Like <c>Aggregate</c>, but yields every running value.</summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <typeparam name="TAcc">The accumulator type.</typeparam>
    /// <param name="source">The elements. A <see langword="null"/> value gives an empty sequence.</param>
    /// <param name="seed">The starting accumulator. It is <b>not</b> yielded.</param>
    /// <param name="accumulator">Combines the running value with the next element. A <see langword="null"/> value
    /// gives an empty sequence. A throwing accumulator propagates when the result is iterated.</param>
    /// <returns>For seed <c>0</c>, <c>+</c> and <c>[1, 2, 3]</c>: <c>1, 3, 6</c> (one value per element, so an empty
    /// source gives an empty sequence). Lazy: nothing is enumerated until the result is iterated, and the source is
    /// enumerated once. A <see langword="null"/> source or accumulator records <c>result.null_source</c> or
    /// <c>result.null_delegate</c> in telemetry; it never throws.</returns>
    public static IEnumerable<TAcc> Scan<T, TAcc>(
        this IEnumerable<T>? source,
        TAcc seed,
        Func<TAcc, T, TAcc> accumulator)
    {
        if (source is null)
        {
            _ = ResultErrors.NullSource(nameof(source));
            return [];
        }

        if (ResultErrors.IsMissing(accumulator))
        {
            _ = ResultErrors.NullDelegate(nameof(accumulator));
            return [];
        }

        return Iterate(source, seed, accumulator);

        static IEnumerable<TAcc> Iterate(IEnumerable<T> source, TAcc seed, Func<TAcc, T, TAcc> accumulator)
        {
            var running = seed;

            foreach (var item in source)
            {
                running = accumulator(running, item);
                yield return running;
            }
        }
    }
}
