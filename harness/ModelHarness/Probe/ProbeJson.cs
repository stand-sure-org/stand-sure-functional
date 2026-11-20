namespace ModelHarness.Probe;

using System.Text.Json.Serialization;

[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
[JsonSerializable(typeof(ProbeRunnerLine))]
[JsonSerializable(typeof(ProbeResult))]
internal sealed partial class ProbeJson : JsonSerializerContext
{
}
