namespace ModelHarness.Models.Wire;

using System.Collections.Generic;
using System.Text.Json.Serialization;

internal sealed class AnthropicRequest
{
    public required string Model { get; init; }

    [JsonPropertyName("max_tokens")]
    public required int MaxTokens { get; init; }

    public required string System { get; init; }

    public required List<AnthropicMessage> Messages { get; init; }
}
