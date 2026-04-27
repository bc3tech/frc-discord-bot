namespace FunctionApp.Tests;

using global::ChatBot.Tools;

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
            BaseAddress = new Uri("https://api.statbotics.io/"),
        };

        var tool = new TestHttpGetTool(new StubHttpClientFactory(client));

        // Act
        string response = await tool.QueryAsync(
            "/v3/team_year/2046/2025",
            "metric=epa",
            CancellationToken.None);

        using JsonDocument document = JsonDocument.Parse(response);

        // Assert
        Assert.Equal("v3/team_year/2046/2025?metric=epa", document.RootElement.GetProperty("apiRequest").GetProperty("path").GetString());
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
            BaseAddress = new Uri("https://api.statbotics.io/"),
        };

        var tool = new StatboticsTool(new StubHttpClientFactory(client), NullLogger<StatboticsTool>.Instance);
        var discordTool = tool;

        // Act
        string response = await tool.QueryStatboticsAsync("/v3/team_year/2046/2025", cancellationToken: CancellationToken.None);

        using JsonDocument document = JsonDocument.Parse(response);

        // Assert
        Assert.Equal("statbotics_api", discordTool.Name);
        Assert.Equal("statbotics_api", discordTool.AsFunction().Name);
        Assert.Equal("v3/team_year/2046/2025", document.RootElement.GetProperty("apiRequest").GetProperty("path").GetString());
        Assert.Equal("https://www.statbotics.io/team/2046/2025", document.RootElement.GetProperty("userReferencePages")[0].GetProperty("url").GetString());
    }

    [Fact]
    public async Task DescribeStatboticsApiSurfaceShowsEventsYearAsQueryParameter()
    {
        // Arrange
        using var handler = new StubHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("""{"unused":true}"""),
        });
        using var client = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://api.statbotics.io/"),
        };

        var tool = new StatboticsTool(new StubHttpClientFactory(client), NullLogger<StatboticsTool>.Instance);

        // Act
        string response = await tool.DescribeApiSurfaceAsync("event", CancellationToken.None);

        using JsonDocument document = JsonDocument.Parse(response);
        JsonElement root = document.RootElement;

        // Assert
        Assert.Contains("path=/v3/events", root.GetProperty("guidance").GetString());
        Assert.Contains("query=year=2026", root.GetProperty("guidance").GetString());
        Assert.Contains(
            root.GetProperty("endpoints").EnumerateArray(),
            endpoint => endpoint.GetProperty("Template").GetString() == "/v3/events"
                && endpoint.GetProperty("QueryParameters").EnumerateArray().Any(parameter => parameter.GetProperty("Name").GetString() == "year")
                && endpoint.GetProperty("QueryParameters").EnumerateArray().Any(parameter =>
                    parameter.GetProperty("Name").GetString() == "type"
                    && parameter.TryGetProperty("Enum", out JsonElement enumElement)
                    && enumElement.EnumerateArray().Any(value => value.GetString() == "champs_div"))
                && endpoint.GetProperty("PathParameters").GetArrayLength() is 0);
    }

    [Fact]
    public async Task QueryStatboticsAsyncRejectsYearAsEventsPathSegment()
    {
        // Arrange
        using var handler = new StubHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("""{"shouldNotCall":true}"""),
        });
        using var client = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://api.statbotics.io/"),
        };

        var tool = new StatboticsTool(new StubHttpClientFactory(client), NullLogger<StatboticsTool>.Instance);

        // Act
        string response = await tool.QueryStatboticsAsync("/v3/events/2026", cancellationToken: CancellationToken.None);

        using JsonDocument document = JsonDocument.Parse(response);
        JsonElement root = document.RootElement;

        // Assert
        Assert.False(root.GetProperty("ok").GetBoolean());
        Assert.Equal(400, root.GetProperty("statusCode").GetInt32());
        Assert.Contains("path=/v3/events", root.GetProperty("guidance").GetString());
        Assert.Contains("query=year=2026", root.GetProperty("guidance").GetString());
        Assert.Contains("/v3/events", root.GetProperty("suggestions").GetString());
        Assert.Null(handler.LastRequestUri);
    }

    [Fact]
    public void StatboticsApiSurfaceToolExposesSurfaceFunction()
    {
        using var handler = new StubHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("""{"unused":true}"""),
        });
        using var client = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://api.statbotics.io/"),
        };

        var queryTool = new StatboticsTool(new StubHttpClientFactory(client), NullLogger<StatboticsTool>.Instance);
        var surfaceTool = new StatboticsApiSurfaceTool(queryTool);

        Assert.Equal("statbotics_api_surface", surfaceTool.Name);
        Assert.Equal("statbotics_api_surface", surfaceTool.AsFunction().Name);
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
            BaseAddress = new Uri("https://www.thebluealliance.com/api/v3/"),
        };

        IConfigurationRoot configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
            [
                new KeyValuePair<string, string?>("TbaApiKey", "test-key"),
            ])
            .Build();
        var tool = new TbaApiTool(configuration, new StubHttpClientFactory(client), NullLogger<TbaApiTool>.Instance);
        var discordTool = tool;

        // Act
        string response = await tool.QueryTbaAsync("/team/frc2046", cancellationToken: CancellationToken.None);

        using JsonDocument document = JsonDocument.Parse(response);

        // Assert
        Assert.Equal("tba_api", discordTool.Name);
        Assert.Equal("tba_api", discordTool.AsFunction().Name);
        Assert.Equal("team/frc2046", document.RootElement.GetProperty("apiRequest").GetProperty("path").GetString());
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

        IConfigurationRoot configuration = new ConfigurationBuilder()
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

    [Fact]
    public async Task ResolveLastCompetitionAsyncWithEventHintReturnsGroundedEventStatusAndAwards()
    {
        using var handler = new RoutingHttpMessageHandler(requestUri =>
        {
            string path = requestUri.AbsolutePath;
            return path switch
            {
                "/api/v3/team/frc2046/events/2024/simple" => new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(
                        """
                        [
                          {
                            "key":"2024pncmp",
                            "name":"Pacific Northwest District Championship",
                            "short_name":"PNW District Championship",
                            "event_code":"pncmp",
                            "start_date":"2024-04-03",
                            "end_date":"2024-04-06",
                            "week":6,
                            "district":{"abbreviation":"pnw","display_name":"Pacific Northwest"}
                          },
                          {
                            "key":"2024orwil",
                            "name":"Wilsonville District Event",
                            "short_name":"Wilsonville",
                            "event_code":"orwil",
                            "start_date":"2024-03-20",
                            "end_date":"2024-03-23",
                            "week":4,
                            "district":{"abbreviation":"pnw","display_name":"Pacific Northwest"}
                          }
                        ]
                        """),
                },
                "/api/v3/team/frc2046/event/2024pncmp/status" => new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("""{"qual":{"ranking":{"rank":4},"record":{"wins":10,"losses":2,"ties":0}},"playoff":{"status":"won"}}"""),
                },
                "/api/v3/team/frc2046/event/2024pncmp/awards" => new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("""[{"name":"District Championship Winner"}]"""),
                },
                _ => new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent("""{"error":"not-found"}"""),
                },
            };
        });
        using var client = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://www.thebluealliance.com/api/v3/"),
        };
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
            [
                new KeyValuePair<string, string?>("TbaApiKey", "test-key"),
            ])
            .Build();
        var tool = new TbaApiTool(configuration, new StubHttpClientFactory(client), NullLogger<TbaApiTool>.Instance);

        string response = await tool.ResolveLastCompetitionAsync(
            teamNumber: 2046,
            seasonYear: 2024,
            eventNameHint: "pnw district championships",
            cancellationToken: CancellationToken.None);

        using JsonDocument document = JsonDocument.Parse(response);
        JsonElement root = document.RootElement;

        Assert.True(root.GetProperty("ok").GetBoolean());
        Assert.Equal(2046, root.GetProperty("teamNumber").GetInt32());
        Assert.Equal(2024, root.GetProperty("seasonYear").GetInt32());
        Assert.Equal("2024pncmp", root.GetProperty("eventData").GetProperty("key").GetString());
        Assert.Equal(4, root.GetProperty("teamStatus").GetProperty("qual").GetProperty("ranking").GetProperty("rank").GetInt32());
        Assert.Equal("District Championship Winner", root.GetProperty("teamAwards")[0].GetProperty("name").GetString());
    }

    [Fact]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0046:Convert to conditional expression", Justification = "<Pending>")]
    public async Task ResolveLastCompetitionAsyncWhenCurrentSeasonHasNoEventsFallsBackToPriorSeason()
    {
        int currentYear = DateTime.UtcNow.Year;
        int fallbackYear = currentYear - 1;
        using var handler = new RoutingHttpMessageHandler(requestUri =>
        {
            string path = requestUri.AbsolutePath;
            if (path == $"/api/v3/team/frc2046/events/{currentYear}/simple")
            {
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("[]"),
                };
            }

            if (path == $"/api/v3/team/frc2046/events/{fallbackYear}/simple")
            {
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(
                        $$"""
                        [
                          {
                            "key":"{{fallbackYear}}pncmp",
                            "name":"Pacific Northwest District Championship",
                            "short_name":"PNW District Championship",
                            "event_code":"pncmp",
                            "start_date":"{{fallbackYear}}-04-03",
                            "end_date":"{{fallbackYear}}-04-06",
                            "week":6,
                            "district":{"abbreviation":"pnw","display_name":"Pacific Northwest"}
                          }
                        ]
                        """),
                };
            }

            if (path == $"/api/v3/team/frc2046/event/{fallbackYear}pncmp/status")
            {
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("""{"qual":{"ranking":{"rank":3}}}"""),
                };
            }

            if (path == $"/api/v3/team/frc2046/event/{fallbackYear}pncmp/awards")
            {
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("[]"),
                };
            }

            return new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent("""{"error":"not-found"}"""),
            };
        });
        using var client = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://www.thebluealliance.com/api/v3/"),
        };
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
            [
                new KeyValuePair<string, string?>("TbaApiKey", "test-key"),
            ])
            .Build();
        var tool = new TbaApiTool(configuration, new StubHttpClientFactory(client), NullLogger<TbaApiTool>.Instance);

        string response = await tool.ResolveLastCompetitionAsync(
            teamNumber: 2046,
            seasonYear: null,
            eventNameHint: null,
            cancellationToken: CancellationToken.None);

        using JsonDocument document = JsonDocument.Parse(response);
        JsonElement root = document.RootElement;

        Assert.True(root.GetProperty("ok").GetBoolean());
        Assert.Equal(fallbackYear, root.GetProperty("seasonYear").GetInt32());
        Assert.Equal($"{fallbackYear}pncmp", root.GetProperty("eventData").GetProperty("key").GetString());
    }

    [Fact]
    public void AugmentResponseReturnsOriginalWhenOkAndNonEmpty()
    {
        string original = """{"ok":true,"statusCode":200,"data":{"key":"frc2046"}}""";
        string result = TbaApiTool.AugmentResponseWithRecoveryHints(original, "/team/frc2046");
        Assert.Equal(original, result);
    }

    [Fact]
    public void AugmentResponseAddsHintsOnEmptyArray()
    {
        string original = """{"ok":true,"statusCode":200,"data":[]}""";
        string result = TbaApiTool.AugmentResponseWithRecoveryHints(original, "/team/frc2046/events/2026/simple");

        using JsonDocument doc = JsonDocument.Parse(result);
        JsonElement root = doc.RootElement;
        Assert.True(root.TryGetProperty("recoveryHints", out JsonElement hints));
        Assert.True(hints.GetArrayLength() > 0);
        Assert.Contains("2025", hints[0].GetString());
        Assert.True(root.TryGetProperty("guidance", out _));
    }

    [Fact]
    public void AugmentResponseAddsHintsOn404()
    {
        string original = """{"ok":false,"statusCode":404,"error":"HTTP 404 Not Found","data":null}""";
        string result = TbaApiTool.AugmentResponseWithRecoveryHints(original, "/event/2026pncmp/rankings");

        using JsonDocument doc = JsonDocument.Parse(result);
        JsonElement root = doc.RootElement;
        Assert.True(root.TryGetProperty("recoveryHints", out JsonElement hints));
        Assert.True(hints.GetArrayLength() > 0);
        // Should suggest listing events for the year
        Assert.Contains("2026", hints[0].GetString());
    }

    [Fact]
    public void AugmentResponseAddsMatchKeyRecoveryHint()
    {
        string original = """{"ok":false,"statusCode":404,"error":"HTTP 404 Not Found","data":null}""";
        string result = TbaApiTool.AugmentResponseWithRecoveryHints(original, "/match/2025pncmp_qm1");

        using JsonDocument doc = JsonDocument.Parse(result);
        JsonElement root = doc.RootElement;
        Assert.True(root.TryGetProperty("recoveryHints", out JsonElement hints));
        Assert.Contains("2025pncmp", hints[0].GetString());
    }

    [Fact]
    public void AugmentResponseAddsTeamEventScopedHint()
    {
        string original = """{"ok":false,"statusCode":404,"error":"HTTP 404 Not Found","data":null}""";
        string result = TbaApiTool.AugmentResponseWithRecoveryHints(original, "/team/frc2046/event/2026pncmp/status");

        using JsonDocument doc = JsonDocument.Parse(result);
        JsonElement root = doc.RootElement;
        Assert.True(root.TryGetProperty("recoveryHints", out JsonElement hints));
        // Should suggest listing events to find valid keys
        string firstHint = hints[0].GetString()!;
        Assert.Contains("frc2046", firstHint);
        Assert.Contains("events", firstHint);
    }

    [Fact]
    public void BuildRecoveryHintsForCurrentYearEventsEmpty()
    {
        int currentYear = DateTime.UtcNow.Year;
        List<string> hints = TbaApiTool.BuildRecoveryHints($"/team/frc2046/events/{currentYear}/simple", ok: true);
        Assert.NotEmpty(hints);
        Assert.Contains($"{currentYear - 1}", hints[0]);
    }

    [Fact]
    public void BuildRecoveryHintsPreservesExistingFieldsInAugmentedResponse()
    {
        string original = """{"ok":true,"statusCode":200,"data":[],"citations":[{"title":"TBA","url":"https://tba.com"}]}""";
        string result = TbaApiTool.AugmentResponseWithRecoveryHints(original, "/team/frc2046/events/2026/simple");

        using JsonDocument doc = JsonDocument.Parse(result);
        JsonElement root = doc.RootElement;
        Assert.True(root.TryGetProperty("citations", out JsonElement citations));
        Assert.Equal(1, citations.GetArrayLength());
        Assert.True(root.TryGetProperty("recoveryHints", out _));
    }

    [Fact]
    public async Task QueryTbaAsyncReturnsRecoveryHintsOn404()
    {
        using var handler = new RoutingHttpMessageHandler(requestUri =>
        {
            return new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent("""{"error":"not-found"}"""),
            };
        });
        using var client = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://www.thebluealliance.com/api/v3"),
        };
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
            [
                new KeyValuePair<string, string?>("TbaApiKey", "test-key"),
            ])
            .Build();
        var tool = new TbaApiTool(configuration, new StubHttpClientFactory(client), NullLogger<TbaApiTool>.Instance);

        string response = await tool.QueryTbaAsync("/team/frc2046/events/2026/simple", cancellationToken: CancellationToken.None);

        using JsonDocument doc = JsonDocument.Parse(response);
        JsonElement root = doc.RootElement;
        Assert.True(root.TryGetProperty("recoveryHints", out JsonElement hints));
        Assert.True(hints.GetArrayLength() > 0);
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

    private sealed class RoutingHttpMessageHandler(Func<Uri, HttpResponseMessage> responder) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Uri requestUri = request.RequestUri ?? throw new InvalidOperationException("Request URI was not set.");
            return Task.FromResult(responder(requestUri));
        }
    }
}
