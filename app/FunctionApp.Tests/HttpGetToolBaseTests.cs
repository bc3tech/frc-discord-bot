namespace FunctionApp.Tests;

using BC3Technologies.DiscordGpt.Core;

using ChatBot.Tools;

using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;

using System.Net;
using System.Net.Http;
using System.Text.Json;

public sealed class HttpGetToolBaseTests
{
    [Fact]
    public async Task SendGetAsyncWhenClientUsesBaseAddressSerializesRelativeRequestAndKeepsCitations()
    {
        // Arrange
        using var handler = new StubHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("""{"value":"ok"}"""),
        });
        using var client = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://api.statbotics.io"),
        };

        var tool = new TestHttpGetTool(new StubHttpClientFactory(client));

        // Act
        string response = await tool.QueryAsync(
            "/v3/team_year/2046/2025",
            "metric=epa",
            CancellationToken.None);

        using JsonDocument document = JsonDocument.Parse(response);

        // Assert
        Assert.Equal("/v3/team_year/2046/2025?metric=epa", document.RootElement.GetProperty("apiRequest").GetProperty("path").GetString());
        Assert.Equal("api", document.RootElement.GetProperty("apiRequest").GetProperty("kind").GetString());
        Assert.Equal("https://www.statbotics.io/team/2046/2025", document.RootElement.GetProperty("userReferencePages")[0].GetProperty("url").GetString());
        Assert.Equal("https://www.statbotics.io/team/2046/2025", document.RootElement.GetProperty("citations")[0].GetProperty("url").GetString());
        Assert.Equal("ok", document.RootElement.GetProperty("data").GetProperty("value").GetString());
        Assert.Equal("https://api.statbotics.io/v3/team_year/2046/2025?metric=epa", handler.LastRequestUri?.ToString());
    }

    [Fact]
    public async Task QueryStatboticsAsyncProvidesBrowsableReferencePage()
    {
        // Arrange
        using var handler = new StubHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("""{"team":2046}"""),
        });
        using var client = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://api.statbotics.io"),
        };

        var tool = new StatboticsTool(new StubHttpClientFactory(client), NullLogger<StatboticsTool>.Instance);
        IDiscordTool discordTool = tool;

        // Act
        string response = await tool.QueryStatboticsAsync("/v3/team_year/2046/2025", cancellationToken: CancellationToken.None);

        using JsonDocument document = JsonDocument.Parse(response);

        // Assert
        Assert.Equal("statbotics_api", discordTool.Name);
        Assert.Equal("statbotics_api", discordTool.AsFunction().Name);
        Assert.Equal("/v3/team_year/2046/2025", document.RootElement.GetProperty("apiRequest").GetProperty("path").GetString());
        Assert.Equal("https://www.statbotics.io/team/2046/2025", document.RootElement.GetProperty("userReferencePages")[0].GetProperty("url").GetString());
    }

    [Fact]
    public async Task QueryTbaAsyncProvidesBrowsableReferencePage()
    {
        // Arrange
        using var handler = new StubHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("""{"key":"frc2046"}"""),
        });
        using var client = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://www.thebluealliance.com/api/v3"),
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
            [
                new KeyValuePair<string, string?>("TbaApiKey", "test-key"),
            ])
            .Build();
        var tool = new TbaApiTool(configuration, new StubHttpClientFactory(client), NullLogger<TbaApiTool>.Instance);
        IDiscordTool discordTool = tool;

        // Act
        string response = await tool.QueryTbaAsync("/team/frc2046", cancellationToken: CancellationToken.None);

        using JsonDocument document = JsonDocument.Parse(response);

        // Assert
        Assert.Equal("tba_api", discordTool.Name);
        Assert.Equal("tba_api", discordTool.AsFunction().Name);
        Assert.Equal("/team/frc2046", document.RootElement.GetProperty("apiRequest").GetProperty("path").GetString());
        Assert.Equal("https://www.thebluealliance.com/team/2046", document.RootElement.GetProperty("userReferencePages")[0].GetProperty("url").GetString());
    }

    [Fact]
    public void TbaApiSurfaceToolExposesSurfaceFunction()
    {
        using var handler = new StubHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("""{"key":"frc2046"}"""),
        });
        using var client = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://www.thebluealliance.com/api/v3"),
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
            [
                new KeyValuePair<string, string?>("TbaApiKey", "test-key"),
            ])
            .Build();
        var queryTool = new TbaApiTool(configuration, new StubHttpClientFactory(client), NullLogger<TbaApiTool>.Instance);
        var surfaceTool = new TbaApiSurfaceTool(queryTool);

        Assert.Equal("tba_api_surface", surfaceTool.Name);
        Assert.Equal("tba_api_surface", surfaceTool.AsFunction().Name);
    }

    private sealed class TestHttpGetTool(IHttpClientFactory httpClientFactory)
        : HttpGetToolBase(httpClientFactory, NullLogger.Instance)
    {
        public override IReadOnlyList<AIFunction> Functions => [];

        public override IReadOnlyList<string> ToolNames => [];

        public Task<string> QueryAsync(
            string path,
            string? query,
            CancellationToken cancellationToken)
            => SendGetAsync(
                "test-client",
                path,
                query,
                citations: [new CitationLink("Statbotics team season page", "https://www.statbotics.io/team/2046/2025")],
                cancellationToken: cancellationToken);
    }

    private sealed class StubHttpClientFactory(HttpClient client) : IHttpClientFactory
    {
        public HttpClient CreateClient(string name) => client;
    }

    private sealed class StubHttpMessageHandler(HttpResponseMessage response) : HttpMessageHandler
    {
        public Uri? LastRequestUri { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LastRequestUri = request.RequestUri;
            return Task.FromResult(response);
        }
    }
}
