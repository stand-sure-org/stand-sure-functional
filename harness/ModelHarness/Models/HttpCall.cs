namespace ModelHarness.Models;

using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

/// <summary>Shared never-throw HTTP plumbing for the model clients.</summary>
internal static class HttpCall
{
    internal readonly record struct Parsed(string Text, int InputTokens, int OutputTokens);

    public static ModelResponse Fail(string modelId, string error, TimeSpan latency = default)
    {
        return new ModelResponse(modelId, string.Empty, 0, 0, latency, error);
    }

    public static async Task<ModelResponse> SendAsync(
        HttpClient http,
        ILogger logger,
        string family,
        string modelId,
        Func<HttpRequestMessage> buildRequest,
        Func<string, Parsed> parse,
        CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        logger.RequestStarted(family, modelId);
        var stopwatch = Stopwatch.StartNew();
        try
        {
            using var request = buildRequest();
            using var response = await http.SendAsync(request, ct).ConfigureAwait(false);
            var body = await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                return Failed(logger, family, modelId, $"HTTP {(int)response.StatusCode} from {family}", stopwatch.Elapsed);
            }

            var parsed = parse(body);
            logger.RequestCompleted(family, modelId, stopwatch.ElapsedMilliseconds);
            return new ModelResponse(modelId, parsed.Text, parsed.InputTokens, parsed.OutputTokens, stopwatch.Elapsed, null);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            throw;
        }
        catch (OperationCanceledException)
        {
            return Failed(logger, family, modelId, "request timed out", stopwatch.Elapsed);
        }
        catch (HttpRequestException ex)
        {
            return Failed(logger, family, modelId, $"transport error: {ex.GetType().Name}", stopwatch.Elapsed);
        }
        catch (Exception ex) when (ex is JsonException or InvalidOperationException or FormatException)
        {
            return Failed(logger, family, modelId, "malformed response body", stopwatch.Elapsed);
        }
    }

    private static ModelResponse Failed(ILogger logger, string family, string modelId, string error, TimeSpan latency)
    {
        logger.RequestFailed(family, modelId, error);
        return Fail(modelId, error, latency);
    }
}
