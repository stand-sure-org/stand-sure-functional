namespace ModelHarness.Models.Wire;

using System.Collections.Generic;
using System.Text.Json.Serialization;

internal sealed class OllamaMessage
{
    public string? Role { get; init; }

    public string? Content { get; init; }
}
