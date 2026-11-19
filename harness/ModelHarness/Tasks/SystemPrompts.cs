namespace ModelHarness.Tasks;

using System;

/// <summary>
/// Builds the system prompts from <c>harness/prompts/</c>.
/// </summary>
/// <remarks>
/// <c>system-baseline.md</c> is the baseline system prompt verbatim (trim trailing whitespace before sending).
/// <c>system-with-library.md</c> is the same text followed by <see cref="ApiSummaryMarker"/>, which the runner
/// replaces with the contents of <c>docs/llm-api-summary.md</c> via <see cref="ComposeWithLibrary"/>.
/// </remarks>
public static class SystemPrompts
{
    /// <summary>The placeholder in <c>system-with-library.md</c> replaced by the API summary page.</summary>
    public const string ApiSummaryMarker = "{{API_SUMMARY}}";

    /// <summary>Replaces <see cref="ApiSummaryMarker"/> in <paramref name="template"/> with <paramref name="apiSummary"/>.</summary>
    /// <returns>The composed prompt, <c>\n</c> line endings, no trailing whitespace.</returns>
    public static string ComposeWithLibrary(string template, string apiSummary)
    {
        ArgumentNullException.ThrowIfNull(template);
        ArgumentNullException.ThrowIfNull(apiSummary);
        if (!template.Contains(ApiSummaryMarker, StringComparison.Ordinal))
        {
            throw new ArgumentException($"Template has no {ApiSummaryMarker} marker.", nameof(template));
        }

        return template
            .Replace(ApiSummaryMarker, apiSummary.Trim(), StringComparison.Ordinal)
            .Replace("\r\n", "\n", StringComparison.Ordinal)
            .TrimEnd();
    }
}
