using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace StandSure.Functional.Json;

/// <summary>JSON converter for <see cref="Result{T}"/>.</summary>
/// <typeparam name="T">The result value type.</typeparam>
/// <remarks>
/// <para>Wire format is described on <see cref="ResultJsonConverterFactory"/>.</para>
/// <para>
/// Native AOT: the factory builds this type by reflection, which cannot work for a value-type <typeparamref name="T"/>
/// the compiler never instantiated. Register it explicitly instead, with
/// <c>[JsonSourceGenerationOptions(Converters = [typeof(ResultJsonConverter&lt;MyStruct&gt;)])]</c> on your context,
/// or <c>options.Converters.Add(new ResultJsonConverter&lt;MyStruct&gt;())</c>, and add <c>Result&lt;MyStruct&gt;</c>
/// and <c>MyStruct</c> to your context with <c>[JsonSerializable]</c>.
/// </para>
/// </remarks>
public sealed class ResultJsonConverter<T> : JsonConverter<Result<T>>
{
    /// <inheritdoc />
    public override Result<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException("A Result must be a JSON object.");
        }

        var hasValue = false;
        var valueIsNull = false;
        T? value = default;
        var hasError = false;
        Error? error = null;

        while (reader.Read() && reader.TokenType == JsonTokenType.PropertyName)
        {
            if (reader.ValueTextEquals("value"u8))
            {
                reader.Read();
                hasValue = true;
                valueIsNull = reader.TokenType == JsonTokenType.Null;
                if (!valueIsNull)
                {
                    value = JsonSerializer.Deserialize(ref reader, (JsonTypeInfo<T>)options.GetTypeInfo(typeof(T)));
                }
            }
            else if (reader.ValueTextEquals("error"u8))
            {
                reader.Read();
                hasError = true;
                if (reader.TokenType != JsonTokenType.Null)
                {
                    error = ErrorJson.Read(ref reader);
                }
            }
            else
            {
                reader.Read();
                reader.Skip();
            }
        }

        if (error is not null)
        {
            return Result.Fail<T>(error);
        }

        if (hasValue && !(valueIsNull && hasError))
        {
            return Result.Ok(value!);
        }

        return Result.Fail<T>(Error.Uninitialized);
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, Result<T> result, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        var (value, error) = result;
        if (error is not null)
        {
            writer.WritePropertyName("error"u8);
            JsonSerializer.Serialize(writer, error, FunctionalJsonContext.Default.Error);
        }
        else
        {
            writer.WritePropertyName("value"u8);
            JsonSerializer.Serialize(writer, value, (JsonTypeInfo<T?>)options.GetTypeInfo(typeof(T)));
        }

        writer.WriteEndObject();
    }
}
