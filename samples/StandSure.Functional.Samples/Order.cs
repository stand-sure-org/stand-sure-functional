namespace StandSure.Functional.Samples;

internal sealed record Order(string Id, string Currency, decimal Amount, string? CustomerEmail);
