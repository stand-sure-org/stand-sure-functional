namespace ModelHarness.Models.Wire;

using System.Collections.Generic;
using System.Text.Json.Serialization;

internal sealed class OllamaRequest
{
    public required string Model { get; init; }

    public required List<OllamaMessage> Messages { get; init; }

    public required bool Stream { get; init; }
}
