using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace StandSure.Functional.Json;

/// <summary>
/// Writes and reads <see cref="Error"/> through the library's source-generated metadata, whatever resolver the
/// caller's options use. The metadata is bound to <see cref="FunctionalJsonContext"/>'s own options, so nested
/// errors never re-enter the caller's converters.
/// </summary>
internal sealed class ErrorJsonConverter : JsonConverter<Error>
{
    internal static readonly ErrorJsonConverter Instance = new();

    public override Error Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        ErrorJson.Read(ref reader);

    public override void Write(Utf8JsonWriter writer, Error value, JsonSerializerOptions options) =>
        JsonSerializer.Serialize(writer, value, FunctionalJsonContext.Default.Error);
}
