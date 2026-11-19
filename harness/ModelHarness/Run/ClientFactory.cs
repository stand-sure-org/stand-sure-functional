namespace ModelHarness.Run;

using System;
using System.Collections.Generic;
using System.Net.Http;

using Microsoft.Extensions.Logging;

using ModelHarness.Models;

/// <summary>Builds the model clients, reading API keys through <see cref="IEnvironmentReader"/>.</summary>
public static class ClientFactory
{
    /// <summary>Local models can be slow; generous default for the Ollama <see cref="HttpClient"/>.</summary>
    public static readonly TimeSpan DefaultOllamaTimeout = TimeSpan.FromMinutes(10);

    /// <summary>Timeout for hosted APIs.</summary>
    public static readonly TimeSpan HostedTimeout = TimeSpan.FromMinutes(5);

    /// <summary>A missing key is not an error here: the client reports it per call as a transport error.</summary>
    /// <param name="httpFactory">Creates an <see cref="HttpClient"/> with the given timeout.</param>
    public static Dictionary<string, IModelClient> Create(
        IEnvironmentReader environment,
        ILogger logger,
        Func<TimeSpan, HttpClient> httpFactory,
        TimeSpan ollamaTimeout)
    {
        ArgumentNullException.ThrowIfNull(environment);
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(httpFactory);

        IModelClient[] clients =
        [
            new AnthropicClient(httpFactory(HostedTimeout), environment.Get(AnthropicClient.KeyVariable), logger),
            new GeminiClient(httpFactory(HostedTimeout), environment.Get(GeminiClient.KeyVariable), logger),
            new OllamaClient(httpFactory(ollamaTimeout), logger),
        ];
        var map = new Dictionary<string, IModelClient>(StringComparer.Ordinal);
        foreach (var client in clients)
        {
            map[client.Family] = client;
        }

        return map;
    }
}
