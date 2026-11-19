using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace StandSure.Functional.Json;

/// <summary>JSON converter for <see cref="Option{T}"/>.</summary>
/// <typeparam name="T">The option value type.</typeparam>
/// <remarks>
/// <para>Wire format is described on <see cref="OptionJsonConverterFactory"/>.</para>
/// <para>
/// Native AOT: the factory builds this type by reflection, which cannot work for a value-type <typeparamref name="T"/>
/// the compiler never instantiated. Register it explicitly instead, with
/// <c>[JsonSourceGenerationOptions(Converters = [typeof(OptionJsonConverter&lt;MyStruct&gt;)])]</c> on your context,
/// or <c>options.Converters.Add(new OptionJsonConverter&lt;MyStruct&gt;())</c>, and add <c>Option&lt;MyStruct&gt;</c>
/// and <c>MyStruct</c> to your context with <c>[JsonSerializable]</c>.
/// </para>
/// </remarks>
public sealed class OptionJsonConverter<T> : JsonConverter<Option<T>>
{
    /// <inheritdoc />
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

    /// <inheritdoc />
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
