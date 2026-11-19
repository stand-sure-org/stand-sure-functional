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
    [UnconditionalSuppressMessage("AOT", "IL3050", Justification = "Result<T> is only reached with T statically known to the caller.")]
    [UnconditionalSuppressMessage("AOT", "IL2055", Justification = "Result<T> is only reached with T statically known to the caller.")]
    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options) =>
        (JsonConverter?)Activator.CreateInstance(
            typeof(ResultJsonConverter<>).MakeGenericType(typeToConvert.GetGenericArguments()[0]));

    private sealed class ResultJsonConverter<T> : JsonConverter<Result<T>>
    {
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
                        error = JsonSerializer.Deserialize(ref reader, FunctionalJsonContext.Default.Error);
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
}
