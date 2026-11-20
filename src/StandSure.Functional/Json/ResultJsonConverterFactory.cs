using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace StandSure.Functional.Json;

/// <summary>Creates converters for <see cref="Result{T}"/>.</summary>
/// <remarks>
/// Wire format: <c>{"value":T}</c> or <c>{"error":{...}}</c>. Reading: a non-null <c>error</c> wins; otherwise a
/// present <c>value</c> is a success (<c>{"value":null}</c> is a success holding <c>default</c>); an empty object, or
/// <c>value</c> and <c>error</c> both null, is the <see cref="Error.Uninitialized"/> failure. Unknown properties are
/// skipped. A JSON token that is not an object throws <see cref="JsonException"/>, as for any converter.
/// </remarks>
public sealed class ResultJsonConverterFactory : JsonConverterFactory
{
    /// <inheritdoc />
    public override bool CanConvert(Type typeToConvert) =>
        typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(Result<>);

    /// <inheritdoc />
    [UnconditionalSuppressMessage("AOT", "IL3050", Justification = "JIT convenience only. Native AOT callers register ResultJsonConverter<T> explicitly; see its remarks.")]
    [UnconditionalSuppressMessage("AOT", "IL2055", Justification = "JIT convenience only. Native AOT callers register ResultJsonConverter<T> explicitly; see its remarks.")]
    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options) =>
        (JsonConverter?)Activator.CreateInstance(
            typeof(ResultJsonConverter<>).MakeGenericType(typeToConvert.GetGenericArguments()[0]));
}
