namespace ModelHarness.Models.Wire;

using System.Collections.Generic;
using System.Text.Json.Serialization;

internal sealed class AnthropicResponse
{
    public List<AnthropicBlock>? Content { get; init; }

    public AnthropicUsage? Usage { get; init; }

    public string? Type { get; init; }

    public AnthropicError? Error { get; init; }

    [JsonPropertyName("stop_reason")]
    public string? StopReason { get; init; }
}
