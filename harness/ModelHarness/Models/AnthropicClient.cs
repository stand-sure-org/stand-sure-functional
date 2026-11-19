namespace ModelHarness.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using ModelHarness.Models.Wire;

/// <summary>Anthropic Messages API over direct HTTP. The key comes from <c>ANTHROPIC_API_KEY</c> (passed in by the caller).</summary>
public sealed class AnthropicClient(HttpClient http, string? apiKey, ILogger logger, int maxTokens = 4096) : IModelClient
{
    public const string KeyVariable = "ANTHROPIC_API_KEY";

    private static readonly Uri Endpoint = new("https://api.anthropic.com/v1/messages");

    public string Family => "anthropic";

    public Task<ModelResponse> CompleteAsync(ModelRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            logger.MissingApiKey(Family, request.ModelId, KeyVariable);
            return Task.FromResult(HttpCall.Fail(request.ModelId, $"{KeyVariable} is not set"));
        }

        return HttpCall.SendAsync(http, logger, Family, request.ModelId, () => Build(request), Parse, ct);
    }

    private HttpRequestMessage Build(ModelRequest request)
    {
        var dto = new AnthropicRequest
        {
            Model = request.ModelId,
            MaxTokens = maxTokens,
            System = request.System,
            Messages = [new AnthropicMessage { Role = "user", Content = request.User }],
        };
        var message = new HttpRequestMessage(HttpMethod.Post, Endpoint)
        {
            Content = new StringContent(
                JsonSerializer.Serialize(dto, HarnessJson.Default.AnthropicRequest), Encoding.UTF8, "application/json"),
        };
        message.Headers.Add("x-api-key", apiKey);
        message.Headers.Add("anthropic-version", "2023-06-01");
        return message;
    }

    private static HttpCall.Parsed Parse(string body)
    {
        var dto = JsonSerializer.Deserialize(body, HarnessJson.Default.AnthropicResponse)
            ?? throw new JsonException("empty body");
        var text = string.Concat((dto.Content ?? []).Where(b => b.Type == "text").Select(b => b.Text));
        if (text.Length == 0)
        {
            throw new JsonException("no text content");
        }

        return new HttpCall.Parsed(text, dto.Usage?.InputTokens ?? 0, dto.Usage?.OutputTokens ?? 0);
    }
}
