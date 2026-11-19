namespace ModelHarness.Models.Wire;

using System.Collections.Generic;
using System.Text.Json.Serialization;

internal sealed class GeminiResponse
{
    public List<GeminiCandidate>? Candidates { get; init; }

    public GeminiUsage? UsageMetadata { get; init; }

    public GeminiPromptFeedback? PromptFeedback { get; init; }

    public GeminiError? Error { get; init; }
}
