namespace ModelHarness.Models;

using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using ModelHarness.Models.Wire;

/// <summary>Gemini generateContent over direct HTTP. The key (<c>GEMINI_API_KEY</c>) goes in a header, never the URL.</summary>
public sealed class GeminiClient(HttpClient http, string? apiKey, ILogger logger) : IModelClient
{
    public const string KeyVariable = "GEMINI_API_KEY";

    public string Family => "gemini";

    public Task<ModelResponse> CompleteAsync(ModelRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            logger.MissingApiKey(this.Family, request.ModelId, KeyVariable);
            return Task.FromResult(HttpCall.Fail(request.ModelId, $"{KeyVariable} is not set"));
        }

        return HttpCall.SendAsync(http, logger, this.Family, request.ModelId, () => this.Build(request), Parse, ct);
    }

    private HttpRequestMessage Build(ModelRequest request)
    {
        var dto = new GeminiRequest
        {
            SystemInstruction = new GeminiContent { Parts = [new GeminiPart { Text = request.System }] },
            Contents = [new GeminiContent { Role = "user", Parts = [new GeminiPart { Text = request.User }] }],
        };

        var uri = new Uri($"https://generativelanguage.googleapis.com/v1beta/models/{Uri.EscapeDataString(request.ModelId)}:generateContent");

        var message = new HttpRequestMessage(HttpMethod.Post, uri)
        {
            Content = new StringContent(
                JsonSerializer.Serialize(dto, HarnessJson.Default.GeminiRequest),
                Encoding.UTF8,
                "application/json"),
        };

        message.Headers.Add("x-goog-api-key", apiKey);
        return message;
    }

    private static HttpCall.Parsed Parse(string body)
    {
        var dto = JsonSerializer.Deserialize(body, HarnessJson.Default.GeminiResponse)
                  ?? throw new JsonException("empty body");

        if (dto.Error is not null)
        {
            return HttpCall.Parsed.ProviderError(dto.Error.Status, dto.Error.Message);
        }

        if (dto.Candidates is null && dto.PromptFeedback is null)
        {
            return HttpCall.Parsed.WrongShape();
        }

        var parts = dto.Candidates?.FirstOrDefault()?.Content?.Parts ?? [];
        var text = string.Concat(parts.Select(p => p.Text));
        var finish = dto.Candidates?.FirstOrDefault()?.FinishReason;

        if (string.IsNullOrEmpty(finish))
        {
            finish = dto.PromptFeedback?.BlockReason;
        }

        var truncated = !string.IsNullOrEmpty(finish) && !string.Equals(finish, "STOP", StringComparison.Ordinal);

        return new HttpCall.Parsed(
            text,
            dto.UsageMetadata?.PromptTokenCount ?? 0,
            dto.UsageMetadata?.CandidatesTokenCount ?? 0,
            finish,
            truncated);
    }
}
