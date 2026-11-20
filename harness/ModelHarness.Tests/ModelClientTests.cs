namespace ModelHarness.Tests;

using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;

using AwesomeAssertions;

using Microsoft.Extensions.Logging.Abstractions;

using ModelHarness.Models;

public sealed class ModelClientTests
{
    private static readonly ModelRequest Request = new("the-model", "sys prompt", "user prompt");

    [Fact]
    public async Task Anthropic_BuildsMessagesRequest_ParsesText()
    {
        var handler = new FakeHandler(HttpStatusCode.OK,
            """{"content":[{"type":"text","text":"hel"},{"type":"text","text":"lo"}],"usage":{"input_tokens":11,"output_tokens":7}}""");

        var client = new AnthropicClient(new HttpClient(handler), "test-key", NullLogger.Instance);

        var response = await client.CompleteAsync(Request, TestContext.Current.CancellationToken);

        response.Error.Should().BeNull();
        response.Text.Should().Be("hello");
        response.InputTokens.Should().Be(11);
        response.OutputTokens.Should().Be(7);
        response.ModelId.Should().Be("the-model");
        handler.Request!.RequestUri!.ToString().Should().Be("https://api.anthropic.com/v1/messages");
        handler.Request.Headers.GetValues("x-api-key").Should().Equal("test-key");
        handler.Request.Headers.GetValues("anthropic-version").Should().Equal("2023-06-01");
        using var doc = JsonDocument.Parse(handler.RequestBody!);
        doc.RootElement.GetProperty("model").GetString().Should().Be("the-model");
        doc.RootElement.GetProperty("max_tokens").GetInt32().Should().Be(4096);
        doc.RootElement.GetProperty("system").GetString().Should().Be("sys prompt");
        var message = doc.RootElement.GetProperty("messages")[0];
        message.GetProperty("role").GetString().Should().Be("user");
        message.GetProperty("content").GetString().Should().Be("user prompt");
        client.Family.Should().Be("anthropic");
    }

    [Fact]
    public async Task Gemini_BuildsGenerateContentRequest_ParsesText()
    {
        var handler = new FakeHandler(HttpStatusCode.OK,
            """{"candidates":[{"content":{"parts":[{"text":"hi "},{"text":"there"}]}}],"usageMetadata":{"promptTokenCount":5,"candidatesTokenCount":3}}""");

        var client = new GeminiClient(new HttpClient(handler), "test-key", NullLogger.Instance);

        var response = await client.CompleteAsync(Request, TestContext.Current.CancellationToken);

        response.Error.Should().BeNull();
        response.Text.Should().Be("hi there");
        response.InputTokens.Should().Be(5);
        response.OutputTokens.Should().Be(3);
        var uri = handler.Request!.RequestUri!.ToString();
        uri.Should().Be("https://generativelanguage.googleapis.com/v1beta/models/the-model:generateContent");
        uri.Should().NotContain("key");
        handler.Request.Headers.GetValues("x-goog-api-key").Should().Equal("test-key");
        using var doc = JsonDocument.Parse(handler.RequestBody!);
        doc.RootElement.GetProperty("systemInstruction").GetProperty("parts")[0].GetProperty("text").GetString().Should().Be("sys prompt");
        var content = doc.RootElement.GetProperty("contents")[0];
        content.GetProperty("role").GetString().Should().Be("user");
        content.GetProperty("parts")[0].GetProperty("text").GetString().Should().Be("user prompt");
        client.Family.Should().Be("gemini");
    }

    [Fact]
    public async Task Ollama_BuildsChatRequest_ParsesText()
    {
        var handler = new FakeHandler(HttpStatusCode.OK,
            """{"message":{"role":"assistant","content":"local reply"},"prompt_eval_count":9,"eval_count":4,"done":true}""");

        var client = new OllamaClient(new HttpClient(handler), NullLogger.Instance);

        var response = await client.CompleteAsync(Request, TestContext.Current.CancellationToken);

        response.Error.Should().BeNull();
        response.Text.Should().Be("local reply");
        response.InputTokens.Should().Be(9);
        response.OutputTokens.Should().Be(4);
        handler.Request!.RequestUri!.ToString().Should().Be("http://localhost:11434/api/chat");
        using var doc = JsonDocument.Parse(handler.RequestBody!);
        doc.RootElement.GetProperty("model").GetString().Should().Be("the-model");
        doc.RootElement.GetProperty("stream").GetBoolean().Should().BeFalse();
        var messages = doc.RootElement.GetProperty("messages");
        messages[0].GetProperty("role").GetString().Should().Be("system");
        messages[0].GetProperty("content").GetString().Should().Be("sys prompt");
        messages[1].GetProperty("role").GetString().Should().Be("user");
        messages[1].GetProperty("content").GetString().Should().Be("user prompt");
        client.Family.Should().Be("ollama");
    }

    [Fact]
    public async Task Http500_ReturnsErrorNotThrow()
    {
        var ct = TestContext.Current.CancellationToken;

        var clients = new IModelClient[]
        {
            new AnthropicClient(new HttpClient(new FakeHandler(HttpStatusCode.InternalServerError, "boom")), "test-key", NullLogger.Instance),
            new GeminiClient(new HttpClient(new FakeHandler(HttpStatusCode.InternalServerError, "boom")), "test-key", NullLogger.Instance),
            new OllamaClient(new HttpClient(new FakeHandler(HttpStatusCode.InternalServerError, "boom")), NullLogger.Instance),
        };

        foreach (var client in clients)
        {
            var response = await client.CompleteAsync(Request, ct);

            response.Error.Should().Contain("500");
            response.Text.Should().BeEmpty();
            response.InputTokens.Should().Be(0);
            response.OutputTokens.Should().Be(0);
        }
    }

    [Fact]
    public async Task MissingKey_ReturnsErrorNotThrow()
    {
        var ct = TestContext.Current.CancellationToken;
        var anthropicHandler = new FakeHandler(HttpStatusCode.OK, "{}");
        var geminiHandler = new FakeHandler(HttpStatusCode.OK, "{}");

        var anthropic = await new AnthropicClient(new HttpClient(anthropicHandler), null, NullLogger.Instance).CompleteAsync(Request, ct);
        var gemini = await new GeminiClient(new HttpClient(geminiHandler), "", NullLogger.Instance).CompleteAsync(Request, ct);

        anthropic.Error.Should().Contain("ANTHROPIC_API_KEY");
        gemini.Error.Should().Contain("GEMINI_API_KEY");
        anthropicHandler.Calls.Should().Be(0);
        geminiHandler.Calls.Should().Be(0);
    }

    [Fact]
    public async Task MalformedBody_ReturnsErrorNotThrow()
    {
        var client = new AnthropicClient(new HttpClient(new FakeHandler(HttpStatusCode.OK, "not json")), "test-key", NullLogger.Instance);

        var response = await client.CompleteAsync(Request, TestContext.Current.CancellationToken);

        response.Error.Should().NotBeNull();
        response.Text.Should().BeEmpty();
    }

    [Fact]
    public async Task Cancellation_Propagates()
    {
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();
        var client = new AnthropicClient(new HttpClient(new FakeHandler(HttpStatusCode.OK, "{}")), "test-key", NullLogger.Instance);

        var act = () => client.CompleteAsync(Request, cts.Token);

        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task Anthropic_MaxTokens_IsTruncated()
    {
        var handler = new FakeHandler(HttpStatusCode.OK,
            """{"content":[{"type":"text","text":"cut"}],"stop_reason":"max_tokens","usage":{"input_tokens":1,"output_tokens":2}}""");

        var client = new AnthropicClient(new HttpClient(handler), "k", NullLogger.Instance);

        var response = await client.CompleteAsync(Request, TestContext.Current.CancellationToken);

        response.StopReason.Should().Be("max_tokens");
        response.Truncated.Should().BeTrue();
    }

    [Fact]
    public async Task Anthropic_EndTurn_IsNotTruncated()
    {
        var handler = new FakeHandler(HttpStatusCode.OK,
            """{"content":[{"type":"text","text":"ok"}],"stop_reason":"end_turn","usage":{"input_tokens":1,"output_tokens":2}}""");

        var client = new AnthropicClient(new HttpClient(handler), "k", NullLogger.Instance);

        var response = await client.CompleteAsync(Request, TestContext.Current.CancellationToken);

        response.StopReason.Should().Be("end_turn");
        response.Truncated.Should().BeFalse();
    }

    [Theory]
    [InlineData("STOP", false)]
    [InlineData("MAX_TOKENS", true)]
    [InlineData("SAFETY", true)]
    [InlineData("OTHER", true)]
    public async Task Gemini_FinishReason_DrivesTruncated(string finishReason, bool truncated)
    {
        var handler = new FakeHandler(HttpStatusCode.OK,
            $$$"""{"candidates":[{"content":{"parts":[{"text":"t"}]},"finishReason":"{{{finishReason}}}"}],"usageMetadata":{"promptTokenCount":1,"candidatesTokenCount":1}}""");

        var client = new GeminiClient(new HttpClient(handler), "k", NullLogger.Instance);

        var response = await client.CompleteAsync(Request, TestContext.Current.CancellationToken);

        response.StopReason.Should().Be(finishReason);
        response.Truncated.Should().Be(truncated);
    }

    [Theory]
    [InlineData("stop", false)]
    [InlineData("length", true)]
    public async Task Ollama_DoneReason_DrivesTruncated(string doneReason, bool truncated)
    {
        var handler = new FakeHandler(HttpStatusCode.OK,
            $$$"""{"message":{"role":"assistant","content":"t"},"done":true,"done_reason":"{{{doneReason}}}","prompt_eval_count":1,"eval_count":1}""");

        var client = new OllamaClient(new HttpClient(handler), NullLogger.Instance);

        var response = await client.CompleteAsync(Request, TestContext.Current.CancellationToken);

        response.StopReason.Should().Be(doneReason);
        response.Truncated.Should().Be(truncated);
    }

    [Fact]
    public async Task MissingStopReason_IsNullAndNotTruncated()
    {
        var handler = new FakeHandler(HttpStatusCode.OK,
            """{"content":[{"type":"text","text":"ok"}],"usage":{"input_tokens":1,"output_tokens":2}}""");

        var client = new AnthropicClient(new HttpClient(handler), "k", NullLogger.Instance);

        var response = await client.CompleteAsync(Request, TestContext.Current.CancellationToken);

        response.StopReason.Should().BeNull();
        response.Truncated.Should().BeFalse();
    }

    [Theory]
    [InlineData("""{"candidates":[{"content":{"parts":[]},"finishReason":"SAFETY"}],"usageMetadata":{"promptTokenCount":4,"candidatesTokenCount":0}}""",
        "SAFETY")]
    [InlineData("""{"candidates":[{"finishReason":"MAX_TOKENS"}],"usageMetadata":{"promptTokenCount":4}}""", "MAX_TOKENS")]
    [InlineData("""{"promptFeedback":{"blockReason":"PROHIBITED_CONTENT"},"usageMetadata":{"promptTokenCount":4}}""", "PROHIBITED_CONTENT")]
    public async Task Gemini_EmptyText_KeepsStopReasonAndIsNotAnError(string body, string expectedReason)
    {
        var client = new GeminiClient(new HttpClient(new FakeHandler(HttpStatusCode.OK, body)), "k", NullLogger.Instance);

        var response = await client.CompleteAsync(Request, TestContext.Current.CancellationToken);

        response.Error.Should().BeNull();
        response.Text.Should().BeEmpty();
        response.StopReason.Should().Be(expectedReason);
        response.Truncated.Should().BeTrue();
        response.InputTokens.Should().Be(4);
    }

    [Fact]
    public async Task Anthropic_MaxTokensWithNoTextBlocks_KeepsStopReasonAndTokens()
    {
        var handler = new FakeHandler(HttpStatusCode.OK,
            """{"content":[],"stop_reason":"max_tokens","usage":{"input_tokens":6,"output_tokens":4096}}""");

        var client = new AnthropicClient(new HttpClient(handler), "k", NullLogger.Instance);

        var response = await client.CompleteAsync(Request, TestContext.Current.CancellationToken);

        response.Error.Should().BeNull();
        response.Text.Should().BeEmpty();
        response.StopReason.Should().Be("max_tokens");
        response.Truncated.Should().BeTrue();
        response.InputTokens.Should().Be(6);
        response.OutputTokens.Should().Be(4096);
    }

    [Fact]
    public async Task Ollama_LengthWithEmptyContent_KeepsStopReasonAndTokens()
    {
        var handler = new FakeHandler(HttpStatusCode.OK,
            """{"message":{"role":"assistant","content":""},"done":true,"done_reason":"length","prompt_eval_count":7,"eval_count":3}""");

        var client = new OllamaClient(new HttpClient(handler), NullLogger.Instance);

        var response = await client.CompleteAsync(Request, TestContext.Current.CancellationToken);

        response.Error.Should().BeNull();
        response.Text.Should().BeEmpty();
        response.StopReason.Should().Be("length");
        response.Truncated.Should().BeTrue();
        response.InputTokens.Should().Be(7);
        response.OutputTokens.Should().Be(3);
    }

    [Theory]
    [InlineData("anthropic", "{}", "unexpected response shape")]
    [InlineData("anthropic", """{"type":"error","error":{"type":"overloaded_error","message":"Overloaded"}}""", "overloaded_error: Overloaded")]
    [InlineData("gemini", "{}", "unexpected response shape")]
    [InlineData("gemini", """{"error":{"code":429,"message":"quota exceeded","status":"RESOURCE_EXHAUSTED"}}""", "RESOURCE_EXHAUSTED: quota exceeded")]
    [InlineData("ollama", "{}", "unexpected response shape")]
    [InlineData("ollama", """{"error":"model 'x' not found"}""", "model 'x' not found")]
    public async Task WrongShapeOrErrorBody_YieldsErrorNotEmptySuccess(string family, string body, string expected)
    {
        IModelClient client = family switch
        {
            "anthropic" => new AnthropicClient(new HttpClient(new FakeHandler(HttpStatusCode.OK, body)), "k", NullLogger.Instance),
            "gemini" => new GeminiClient(new HttpClient(new FakeHandler(HttpStatusCode.OK, body)), "k", NullLogger.Instance),
            _ => new OllamaClient(new HttpClient(new FakeHandler(HttpStatusCode.OK, body)), NullLogger.Instance),
        };

        var response = await client.CompleteAsync(Request, TestContext.Current.CancellationToken);

        response.Error.Should().Contain(expected);
        response.Text.Should().BeEmpty();
    }

    [Fact]
    public async Task ProviderErrorMessage_IsCappedAt200Chars()
    {
        var body = $$$"""{"error":"{{{new string('x', 500)}}}"}""";
        var client = new OllamaClient(new HttpClient(new FakeHandler(HttpStatusCode.OK, body)), NullLogger.Instance);

        var response = await client.CompleteAsync(Request, TestContext.Current.CancellationToken);

        response.Error!.Length.Should().BeLessThan(260);
    }

    [Fact]
    public void Config_LoadsEnabledModelsOnly()
    {
        const string json = """
                            {"models":[
                              {"family":"anthropic","id":"a","enabled":true},
                              {"family":"ollama","id":"b","enabled":false},
                              {"family":"gemini","id":"c","enabled":true}]}
                            """;

        var models = ModelConfig.LoadEnabled(json);

        models.Select(m => m.Id).Should().Equal("a", "c");
        models[1].Family.Should().Be("gemini");
    }

    [Fact]
    public void ShippedModelsJson_HasExpectedEnabledModels()
    {
        var models = ModelConfig.LoadEnabledFromFile(Path.Combine(AppContext.BaseDirectory, "models.json"));

        models.Select(m => m.Id).Should().Contain(["claude-opus-5-5", "claude-sonnet-5-5", "claude-haiku-5-5"]);
        models.Where(m => m.Family == "ollama").Select(m => m.Id).Should().BeEquivalentTo(["qwen3-coder:30b", "devstral-small-2:24b"]);
        models.Should().NotContain(m => m.Id.StartsWith("TBD", StringComparison.Ordinal));
    }
}
