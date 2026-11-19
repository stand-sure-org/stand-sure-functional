using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

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
    [UnconditionalSuppressMessage("AOT", "IL3050", Justification = "Option<T> is only reached with T statically known to the caller.")]
    [UnconditionalSuppressMessage("AOT", "IL2055", Justification = "Option<T> is only reached with T statically known to the caller.")]
    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options) =>
        (JsonConverter?)Activator.CreateInstance(
            typeof(OptionJsonConverter<>).MakeGenericType(typeToConvert.GetGenericArguments()[0]));

    private sealed class OptionJsonConverter<T> : JsonConverter<Option<T>>
    {
        public override Option<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException("An Option must be a JSON object.");
            }

            var result = default(Option<T>);
            while (reader.Read() && reader.TokenType == JsonTokenType.PropertyName)
            {
                var isValue = reader.ValueTextEquals("value"u8);
                reader.Read();
                if (isValue && reader.TokenType != JsonTokenType.Null)
                {
                    var value = JsonSerializer.Deserialize(ref reader, (JsonTypeInfo<T>)options.GetTypeInfo(typeof(T)));
                    result = Option.Some(value!);
                }
                else
                {
                    reader.Skip();
                }
            }

            return result;
        }

        public override void Write(Utf8JsonWriter writer, Option<T> option, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            if (option.TryGetValue(out T? value))
            {
                writer.WritePropertyName("value"u8);
                JsonSerializer.Serialize(writer, value, (JsonTypeInfo<T?>)options.GetTypeInfo(typeof(T)));
            }

            writer.WriteEndObject();
        }
    }
}
