namespace ModelHarness.Models;

using System.Text.Json;
using System.Text.Json.Serialization;

[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
[JsonSerializable(typeof(ModelConfigFile))]
[JsonSerializable(typeof(Wire.AnthropicRequest))]
[JsonSerializable(typeof(Wire.AnthropicResponse))]
[JsonSerializable(typeof(Wire.GeminiRequest))]
[JsonSerializable(typeof(Wire.GeminiResponse))]
[JsonSerializable(typeof(Wire.OllamaRequest))]
[JsonSerializable(typeof(Wire.OllamaResponse))]
internal sealed partial class HarnessJson : JsonSerializerContext
{
}
