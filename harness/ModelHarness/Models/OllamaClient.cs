namespace ModelHarness.Models;

using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using ModelHarness.Models.Wire;

/// <summary>Ollama local chat API over direct HTTP (default <c>http://localhost:11434</c>).</summary>
public sealed class OllamaClient(HttpClient http, ILogger logger, Uri? baseAddress = null) : IModelClient
{
    private readonly Uri endpoint = new(baseAddress ?? new Uri("http://localhost:11434"), "/api/chat");

    public string Family => "ollama";

    public Task<ModelResponse> CompleteAsync(ModelRequest request, CancellationToken ct)
    {
        return HttpCall.SendAsync(http, logger, Family, request.ModelId, () => Build(request), Parse, ct);
    }

    private HttpRequestMessage Build(ModelRequest request)
    {
        var dto = new OllamaRequest
        {
            Model = request.ModelId,
            Stream = false,
            Messages =
            [
                new OllamaMessage { Role = "system", Content = request.System },
                new OllamaMessage { Role = "user", Content = request.User },
            ],
        };
        return new HttpRequestMessage(HttpMethod.Post, endpoint)
        {
            Content = new StringContent(
                JsonSerializer.Serialize(dto, HarnessJson.Default.OllamaRequest), Encoding.UTF8, "application/json"),
        };
    }

    private static HttpCall.Parsed Parse(string body)
    {
        var dto = JsonSerializer.Deserialize(body, HarnessJson.Default.OllamaResponse)
            ?? throw new JsonException("empty body");
        if (dto.Error is not null)
        {
            return HttpCall.Parsed.ProviderError(null, dto.Error);
        }

        if (dto.Message is null && dto.DoneReason is null)
        {
            return HttpCall.Parsed.WrongShape();
        }

        var text = dto.Message?.Content ?? string.Empty;

        return new HttpCall.Parsed(
            text, dto.PromptEvalCount, dto.EvalCount, dto.DoneReason, string.Equals(dto.DoneReason, "length", StringComparison.Ordinal));
    }
}
