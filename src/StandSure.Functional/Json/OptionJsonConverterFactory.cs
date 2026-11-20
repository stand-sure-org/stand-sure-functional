using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace StandSure.Functional.Json;

/// <summary>Creates converters for <see cref="Option{T}"/>.</summary>
/// <remarks>
/// Wire format: <c>{"value":T}</c> for some, <c>{}</c> for none. Reading: a missing or null <c>value</c> is none;
/// unknown properties are skipped. A JSON token that is not an object throws <see cref="JsonException"/>.
/// </remarks>
public sealed class OptionJsonConverterFactory : JsonConverterFactory
{
    /// <inheritdoc />
    public override bool CanConvert(Type typeToConvert) =>
        typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(Option<>);

    /// <inheritdoc />
    [UnconditionalSuppressMessage("AOT", "IL3050", Justification = "JIT convenience only. Native AOT callers register OptionJsonConverter<T> explicitly; see its remarks.")]
    [UnconditionalSuppressMessage("AOT", "IL2055", Justification = "JIT convenience only. Native AOT callers register OptionJsonConverter<T> explicitly; see its remarks.")]
    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options) =>
        (JsonConverter?)Activator.CreateInstance(
            typeof(OptionJsonConverter<>).MakeGenericType(typeToConvert.GetGenericArguments()[0]));
}
