namespace ModelHarness.Models;

using System;

public sealed record ModelResponse(
    string ModelId,
    string Text,
    int InputTokens,
    int OutputTokens,
    TimeSpan Latency,
    string? Error);
