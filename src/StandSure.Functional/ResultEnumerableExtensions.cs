using System;
using System.Collections.Generic;

namespace StandSure.Functional;

/// <summary>Collection helpers for sequences of <see cref="Result{T}"/>.</summary>
/// <remarks>
/// These helpers never throw. A <see langword="null"/> sequence yields a <c>result.null_source</c> failure (or empty
/// lists for <see cref="Partition{T}"/>), and a <see langword="null"/> delegate a <c>result.null_delegate</c> failure;
/// both are recorded in telemetry. A <c>default(Result&lt;T&gt;)</c> element counts as a failure with
/// <see cref="Error.Uninitialized"/>. A delegate that throws is not caught. All three helpers enumerate the
/// sequence once, eagerly.
/// </remarks>
public static class ResultEnumerableExtensions
{
    /// <param name="source">The results. A <see langword="null"/> value gives a <c>result.null_source</c> failure.</param>
    /// <typeparam name="T">The value type.</typeparam>
    extension<T>(IEnumerable<Result<T>>? source)
    {
        /// <summary>Turns a sequence of results into one result holding every value, or every failure.</summary>
        /// <returns>A success with the values in order (empty for an empty sequence), or a validation failure with code
        /// <c>result.combine_failed</c>, message "<c>n</c> of <c>m</c> failed" and <b>all</b> failures in
        /// <see cref="Error.Errors"/>.</returns>
        public Result<IReadOnlyList<T>> Combine()
        {
            if (source is null)
            {
                return new Result<IReadOnlyList<T>>(ResultErrors.NullSource(nameof(source)));
            }

            var (successes, failures, total) = Split(source);
            return Build(successes, failures, total);
        }
    }

    /// <summary>Maps every element to a result, then combines them as <see cref="Combine{T}"/> does.</summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <typeparam name="TOut">The mapped value type.</typeparam>
    /// <param name="source">The elements. A <see langword="null"/> value gives a <c>result.null_source</c> failure.</param>
    /// <param name="map">The mapper. A <see langword="null"/> value gives a <c>result.null_delegate</c> failure.
    /// It runs for every element: this helper does not stop at the first failure, so side effects of the mapper
    /// happen for all elements.</param>
    /// <returns>A success with every mapped value in order, or a validation failure holding all failures.</returns>
    public static Result<IReadOnlyList<TOut>> Traverse<T, TOut>(
        this IEnumerable<T>? source,
        Func<T, Result<TOut>>? map)
    {
        using var activity = FunctionalDiagnostics.Source.StartActivity();

        if (source is null)
        {
            return new Result<IReadOnlyList<TOut>>(ResultErrors.NullSource(nameof(source)));
        }

        if (map is null)
        {
            return new Result<IReadOnlyList<TOut>>(ResultErrors.NullDelegate(nameof(map)));
        }

        var values = new List<TOut>();
        var errors = new List<Error>();
        var total = 0;

        foreach (var item in source)
        {
            total++;
            Add(map(item), values, errors);
        }

        return Build(values, errors, total);
    }

    /// <summary>Separates the successes from the failures, keeping the order of each.</summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="source">The results. A <see langword="null"/> value gives two empty lists.</param>
    /// <returns>The success values, then the failure errors.</returns>
    public static (IReadOnlyList<T> Successes, IReadOnlyList<Error> Failures) Partition<T>(
        this IEnumerable<Result<T>>? source)
    {
        if (source is null)
        {
            _ = ResultErrors.NullSource(nameof(source));
            return ([], []);
        }

        var (successes, failures, _) = Split(source);
        return (successes, failures);
    }

    private static (List<T> Successes, List<Error> Failures, int Total) Split<T>(IEnumerable<Result<T>> source)
    {
        var successes = new List<T>();
        var failures = new List<Error>();
        var total = 0;

        foreach (var result in source)
        {
            total++;
            Add(result, successes, failures);
        }

        return (successes, failures, total);
    }

    private static void Add<T>(Result<T> result, List<T> successes, List<Error> failures)
    {
        if (result.IsSuccess)
        {
            successes.Add(result.ValueUnchecked);
            return;
        }

        failures.Add(result.ErrorUnchecked);
    }

    private static Result<IReadOnlyList<T>> Build<T>(List<T> values, List<Error> errors, int total)
    {
        if (errors.Count == 0)
        {
            return new Result<IReadOnlyList<T>>(values);
        }

        return new Result<IReadOnlyList<T>>(Error.Validation(
            ResultErrors.CombineFailedCode,
            $"{errors.Count} of {total} failed",
            errors));
    }
}
