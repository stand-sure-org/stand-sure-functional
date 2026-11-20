namespace ModelHarness.Tests;

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

using AwesomeAssertions;

using Microsoft.Extensions.Logging.Abstractions;

using ModelHarness.Models;
using ModelHarness.Run;

public sealed class RunSupportTests
{
    private static readonly ModelRequest Request = new("m", "sys", "user");

    private sealed class FakeEnvironment(Dictionary<string, string?> values) : IEnvironmentReader
    {
        public string? Get(string name) => values.GetValueOrDefault(name);
    }

    [Fact]
    public async Task ClientFactory_MissingKeys_ClientsReturnErrorNotThrow()
    {
        var handler = new FakeHandler(HttpStatusCode.OK, "{}");

        var clients = ClientFactory.Create(
            new FakeEnvironment([]),
            NullLogger.Instance,
            _ => new HttpClient(handler),
            TimeSpan.FromMinutes(10));

        var anthropic = await clients["anthropic"].CompleteAsync(Request, TestContext.Current.CancellationToken);
        var gemini = await clients["gemini"].CompleteAsync(Request, TestContext.Current.CancellationToken);

        anthropic.Error.Should().Contain("ANTHROPIC_API_KEY");
        gemini.Error.Should().Contain("GEMINI_API_KEY");
        handler.Calls.Should().Be(0);
        clients.Keys.Should().BeEquivalentTo("anthropic", "gemini", "ollama");
    }

    [Fact]
    public async Task ClientFactory_PassesKeysFromEnvironment()
    {
        var handler = new FakeHandler(HttpStatusCode.OK,
            """{"content":[{"type":"text","text":"x"}],"usage":{"input_tokens":1,"output_tokens":1}}""");

        var env = new FakeEnvironment(new() { ["ANTHROPIC_API_KEY"] = "secret-a", ["GEMINI_API_KEY"] = "secret-g" });
        var clients = ClientFactory.Create(env, NullLogger.Instance, _ => new HttpClient(handler), TimeSpan.FromMinutes(10));

        var response = await clients["anthropic"].CompleteAsync(Request, TestContext.Current.CancellationToken);

        response.Error.Should().BeNull();
        handler.Request!.Headers.GetValues("x-api-key").Should().Equal("secret-a");
    }

    [Fact]
    public void ClientFactory_OllamaGetsConfiguredTimeout()
    {
        var timeouts = new List<TimeSpan>();

        var clients = ClientFactory.Create(new FakeEnvironment([]),
            NullLogger.Instance,
            t =>
            {
                timeouts.Add(t);
                return new HttpClient { Timeout = t };
            },
            TimeSpan.FromMinutes(17));

        clients.Should().ContainKey("ollama");
        timeouts.Should().Contain(TimeSpan.FromMinutes(17));
        ClientFactory.DefaultOllamaTimeout.Should().Be(TimeSpan.FromMinutes(10));
    }

    [Fact]
    public void Cli_ParsesRunCommand()
    {
        var result = CliOptions.Parse(
        [
            "run", "--condition", "with-library", "--runs", "3", "--models", "a,b", "--tasks", "01-x,02-y", "--output", "/tmp/o", "--ollama-timeout-minutes",
            "20"
        ]);

        result.Error.Should().BeNull();
        var o = result.Options!;
        o.Condition.Should().Be("with-library");
        o.Runs.Should().Be(3);
        o.Models.Should().Equal("a", "b");
        o.Tasks.Should().Equal("01-x", "02-y");
        o.OutputDirectory.Should().Be("/tmp/o");
        o.OllamaTimeout.Should().Be(TimeSpan.FromMinutes(20));
    }

    [Fact]
    public void Cli_Defaults()
    {
        var o = CliOptions.Parse(["run", "--condition", "baseline"]).Options!;

        o.Runs.Should().Be(5);
        o.Models.Should().BeNull();
        o.Tasks.Should().BeNull();
        o.OutputDirectory.Should().BeNull();
        o.OllamaTimeout.Should().Be(TimeSpan.FromMinutes(10));
    }

    [Theory]
    [InlineData("run")]
    [InlineData("run --condition nope")]
    [InlineData("run --condition baseline --runs 0")]
    [InlineData("run --condition baseline --runs abc")]
    [InlineData("run --condition baseline --bogus 1")]
    [InlineData("run --condition baseline --models")]
    [InlineData("go --condition baseline")]
    [InlineData("")]
    public void Cli_RejectsBadInput(string line)
    {
        var args = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        var result = CliOptions.Parse(args);

        result.Options.Should().BeNull();
        result.Error.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void OutputFileName_UsesUtcTimestampAndCondition()
    {
        var name = HarnessRunner.OutputFileName(new DateTimeOffset(2026, 11, 19, 13, 5, 9, TimeSpan.FromHours(2)), "with-library");

        name.Should().Be("20261119T110509Z-with-library.jsonl");
    }
}
