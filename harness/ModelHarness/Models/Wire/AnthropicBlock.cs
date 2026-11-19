namespace ModelHarness.Models.Wire;

using System.Collections.Generic;
using System.Text.Json.Serialization;

internal sealed class AnthropicBlock
{
    public string? Type { get; init; }

    public string? Text { get; init; }
}
