using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace StandSure.Functional.Tests;

/// <summary>A caller-side converter that reads a JSON <c>null</c> as -1.</summary>
internal sealed class NullAsMinusOneConverter : JsonConverter<int>
{
    public override bool HandleNull => true;

    public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        reader.TokenType == JsonTokenType.Null ? -1 : reader.GetInt32();

    public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options) => writer.WriteNumberValue(value);
}
