namespace ModelHarness.Models.Wire;

using System.Collections.Generic;
using System.Text.Json.Serialization;

internal sealed class GeminiContent
{
    public string? Role { get; init; }

    public List<GeminiPart>? Parts { get; init; }
}
