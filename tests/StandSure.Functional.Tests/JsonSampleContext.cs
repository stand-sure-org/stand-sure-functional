using System.Text.Json.Serialization;

namespace StandSure.Functional.Tests;

/// <summary>A caller's own source-generated context that knows only its own type, as in a native AOT app.</summary>
[JsonSerializable(typeof(JsonSample))]
internal sealed partial class JsonSampleContext : JsonSerializerContext;
