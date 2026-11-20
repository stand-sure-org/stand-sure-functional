namespace ModelHarness.Models.Wire;

using System.Collections.Generic;
using System.Text.Json.Serialization;

internal sealed class GeminiRequest
{
    public required GeminiContent SystemInstruction { get; init; }

    public required List<GeminiContent> Contents { get; init; }
}
