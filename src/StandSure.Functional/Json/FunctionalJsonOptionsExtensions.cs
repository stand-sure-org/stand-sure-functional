using System;
using System.Text.Json;

namespace StandSure.Functional.Json;

/// <summary>Registers the StandSure.Functional JSON wire format.</summary>
public static class FunctionalJsonOptionsExtensions
{
    /// <summary>Adds the source-generated metadata for <see cref="Error"/> and <see cref="ErrorKind"/> to the options.</summary>
    /// <param name="options">The options to extend. Must not yet be in use (read-only).</param>
    /// <returns>The same <paramref name="options"/>, for chaining.</returns>
    /// <remarks>
    /// <see cref="Result{T}"/> and <see cref="Option{T}"/> already carry their converters and serialize
    /// <see cref="Error"/> without this call. Call it so that <see cref="Error"/> and <see cref="ErrorKind"/> also
    /// serialize in the wire format when used directly. The success value type <c>T</c> is resolved through
    /// <paramref name="options"/>, so register it (or a reflection resolver) there.
    /// </remarks>
    public static JsonSerializerOptions AddFunctional(this JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        options.TypeInfoResolverChain.Insert(0, FunctionalJsonContext.Default);
        return options;
    }
}
