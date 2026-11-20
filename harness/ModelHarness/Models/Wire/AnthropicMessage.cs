namespace ModelHarness.Models.Wire;

using System.Collections.Generic;
using System.Text.Json.Serialization;

internal sealed class AnthropicMessage
{
    public required string Role { get; init; }

    public required string Content { get; init; }
}
