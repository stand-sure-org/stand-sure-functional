namespace ModelHarness.Models.Wire;

using System.Collections.Generic;
using System.Text.Json.Serialization;

internal sealed class GeminiUsage
{
    public int PromptTokenCount { get; init; }

    public int CandidatesTokenCount { get; init; }
}
