using System.Text.Json;

namespace StandSure.Functional.Json;

/// <summary>Reads an <see cref="Error"/> and rejects values the source generator's nullability checks cannot see.</summary>
internal static class ErrorJson
{
    internal static Error Read(ref Utf8JsonReader reader)
    {
        var error = JsonSerializer.Deserialize(ref reader, FunctionalJsonContext.Default.Error)
                    ?? throw new JsonException("An Error must not be null.");

        Validate(error);
        return error;
    }

    private static void Validate(Error error)
    {
        foreach (var value in error.Metadata.Values)
        {
            if (value is null)
            {
                throw new JsonException("An Error metadata value must not be null.");
            }
        }

        foreach (var child in error.Errors)
        {
            if (child is null)
            {
                throw new JsonException("An Error child must not be null.");
            }

            Validate(child);
        }

        if (error.Inner is not null)
        {
            Validate(error.Inner);
        }
    }
}
