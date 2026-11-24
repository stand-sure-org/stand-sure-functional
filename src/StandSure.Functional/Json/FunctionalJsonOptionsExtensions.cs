using System;
using System.Text.Json;

namespace StandSure.Functional.Json;

/// <summary>Registers the StandSure.Functional JSON wire format.</summary>
public static class FunctionalJsonOptionsExtensions
{
    /// <summary>Registers the wire format for <see cref="Error"/> on the options.</summary>
    /// <param name="options">The options to extend. Must not yet be in use (read-only).</param>
    /// <returns>The same <paramref name="options"/>, for chaining.</returns>
    /// <remarks>
    /// <see cref="Result{T}"/> and <see cref="Option{T}"/> already carry their converters and serialize
    /// <see cref="Error"/> without this call. Call it so that <see cref="Error"/> also serializes in the wire format when
    /// used directly. The success value type <c>T</c> is resolved through <paramref name="options"/>: on options with no
    /// resolver, System.Text.Json's default applies as usual; when the caller has set one (for example a
    /// source-generated context in a native AOT app), the library's metadata is placed ahead of it.
    /// </remarks>
    public static JsonSerializerOptions AddFunctional(this JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        options.Converters.Add(ErrorJsonConverter.Instance);

        // An empty chain must stay empty: System.Text.Json fills in its own default resolver at first use only then.
        // A caller's own context returns no metadata for Error, and the converter is never consulted for a type the
        // resolver does not know, so the library's metadata goes first.
        if (options.TypeInfoResolverChain.Count == 0)
        {
            return options;
        }

        options.TypeInfoResolverChain.Insert(0, FunctionalJsonContext.Default);
        return options;
    }
}
