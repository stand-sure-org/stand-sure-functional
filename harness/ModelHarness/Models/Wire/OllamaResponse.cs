namespace ModelHarness.Models.Wire;

using System.Collections.Generic;
using System.Text.Json.Serialization;

internal sealed class OllamaResponse
{
    public OllamaMessage? Message { get; init; }

    [JsonPropertyName("prompt_eval_count")]
    public int PromptEvalCount { get; init; }

    [JsonPropertyName("eval_count")]
    public int EvalCount { get; init; }

    [JsonPropertyName("done_reason")]
    public string? DoneReason { get; init; }
}
