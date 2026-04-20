namespace ChatBot.Tools;

using Common;
using Common.Extensions;

using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.Json;

internal sealed class TbaApiTool(
    IConfiguration configuration,
    IHttpClientFactory httpClientFactory,
    ILogger<TbaApiTool> logger) : HttpGetToolBase(httpClientFactory, logger)
{
    private const string TbaApiSurfaceResourceName = "ChatBot.OpenApi.thebluealliance.json";
    private const string DescribeSurfaceToolName = "tba_api_surface";
    private const string QueryToolName = "tba_api";

    private readonly string _tbaApiKey = Throws.IfNullOrWhiteSpace(configuration["TbaApiKey"]);

    public override IReadOnlyList<AIFunction> Functions => field ??=
        [
            AIFunctionFactory.Create(DescribeApiSurfaceAsync, CreateSkippableFunctionOptions(DescribeSurfaceToolName)),
            AIFunctionFactory.Create(QueryTbaAsync, CreateSkippableFunctionOptions(QueryToolName)),
        ];

    public override IReadOnlyList<string> ToolNames => [DescribeSurfaceToolName, QueryToolName];

    [Description("Describes the legitimate The Blue Alliance API v3 surface from the embedded OpenAPI spec. Use this before calling tba_api whenever you are not already sure of the exact endpoint template. You can pass a topic like team, event, district, match, rankings, awards, media, alliances, or status to narrow the list.")]
    public Task<string> DescribeApiSurfaceAsync(
        [Description("Optional topic or keyword to filter to relevant endpoint templates. Examples: team, event, rankings, awards, district, match, media")] string? topic = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        TbaApiSurface surface = s_tbaApiSurface.Value;
        IEnumerable<TbaEndpoint> matchingEndpoints = string.IsNullOrWhiteSpace(topic)
            ? surface.Endpoints
            : surface.Endpoints.Where(endpoint => endpoint.Matches(topic));

        var selectedEndpoints = matchingEndpoints
            .Take(30)
            .Select(endpoint => new
            {
                endpoint.Template,
                endpoint.Description,
                endpoint.OperationId,
                endpoint.Tags,
                endpoint.Parameters,
            }).ToArray().AsReadOnly();

        return Task.FromResult(JsonSerializer.Serialize(new
        {
            baseUrl = "https://www.thebluealliance.com/api/v3",
            guidance = "Choose one of these exact path templates and substitute only the documented path parameters. Call tba_api with the concrete path after selecting the best matching template.",
            endpointCount = selectedEndpoints.Count,
            endpoints = selectedEndpoints,
        }));
    }

    [Description("Calls the official The Blue Alliance API (https://www.thebluealliance.com/apidocs/v3) for FRC competition data. Use only legitimate API v3 paths that exist in the real TBA API surface. If you are not sure of the exact endpoint template, call tba_api_surface first. Provide a safe relative API path beginning with /. Example: /team/frc2046/events/2025/simple. Optionally provide a query string without a leading question mark.")]
    public Task<string> QueryTbaAsync(
        [Description("Relative API path beginning with / that must match a legitimate TBA API v3 endpoint template once path parameters are substituted. Example: /team/frc2046/events/2025/simple")] string path,
        [Description("Optional query string without a leading question mark. Example: keys=1")] string? query = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        TbaEndpoint? endpoint = s_tbaApiSurface.Value.TryMatchConcretePath(path);
        if (endpoint is null)
        {
            string suggestions = string.Join(", ", s_tbaApiSurface.Value.SuggestTemplates(path, 5));
            return Task.FromResult(JsonSerializer.Serialize(new
            {
                apiRequest = new
                {
                    path,
                    kind = "api",
                },
                statusCode = 400,
                ok = false,
                error = "The requested path does not match any legitimate The Blue Alliance API v3 endpoint in the embedded OpenAPI spec.",
                guidance = "Call tba_api_surface first to discover the real endpoint template, then retry with a concrete path that matches it.",
                suggestions = suggestions.Length > 0 ? suggestions : null,
            }));
        }

        try
        {
            return SendGetAsync(
                clientName: "tba-api",
                path,
                query,
                citations: BuildCitations(path),
                cancellationToken: cancellationToken);
        }
        catch (HttpRequestException e) when (e.StatusCode is System.Net.HttpStatusCode.NotFound)
        {
            string suggestions = string.Join(", ", s_tbaApiSurface.Value.SuggestTemplates(path, 5));
            return Task.FromResult(JsonSerializer.Serialize(new
            {
                apiRequest = new
                {
                    path,
                    kind = "api",
                },
                statusCode = 400,
                ok = false,
                error = "The requested path does not match any legitimate The Blue Alliance API v3 endpoint in the embedded OpenAPI spec.",
                guidance = "Call tba_api_surface first to discover the real endpoint template, then retry with a concrete path that matches it.",
                suggestions = suggestions.Length > 0 ? suggestions : null,
            }));
        }
    }

    private static readonly Lazy<TbaApiSurface> s_tbaApiSurface = new(() =>
    {
        using Stream? stream = typeof(TbaApiTool).Assembly.GetManifestResourceStream(TbaApiSurfaceResourceName)
            ?? throw new InvalidOperationException($"Embedded TBA OpenAPI spec '{TbaApiSurfaceResourceName}' was not found.");

        using JsonDocument document = JsonDocument.Parse(stream);
        JsonElement paths = document.RootElement.GetProperty("paths");

        List<TbaEndpoint> endpoints = [];
        foreach (JsonProperty pathProperty in paths.EnumerateObject())
        {
            foreach (JsonProperty operationProperty in pathProperty.Value.EnumerateObject())
            {
                if (!IsHttpMethod(operationProperty.Name))
                {
                    continue;
                }

                JsonElement operation = operationProperty.Value;
                string description = operation.TryGetProperty("description", out JsonElement descriptionElement)
                    ? descriptionElement.GetString().UnlessNullOrWhitespaceThen(string.Empty)
                    : string.Empty;
                string operationId = operation.TryGetProperty("operationId", out JsonElement operationIdElement)
                    ? operationIdElement.GetString().UnlessNullOrWhitespaceThen(string.Empty)
                    : string.Empty;
                string[] tags = operation.TryGetProperty("tags", out JsonElement tagsElement)
                    ? [.. tagsElement.EnumerateArray().Select(tag => tag.GetString().UnlessNullOrWhitespaceThen(string.Empty)).Where(static tag => !string.IsNullOrWhiteSpace(tag))]
                    : [];
                string[] parameters = operation.TryGetProperty("parameters", out JsonElement parametersElement)
                    ? [.. parametersElement.EnumerateArray()
                            .Where(static parameter => parameter.ValueKind == JsonValueKind.Object && parameter.TryGetProperty("name", out _))
                            .Select(static parameter => parameter.GetProperty("name").GetString().UnlessNullOrWhitespaceThen(string.Empty))
                            .Where(static name => !string.IsNullOrWhiteSpace(name))]
                    : [];

                endpoints.Add(new TbaEndpoint(pathProperty.Name, description, operationId, tags, parameters));
            }
        }

        return new TbaApiSurface(endpoints);
    });

    private static bool IsHttpMethod(string methodName)
        => methodName.Equals("get", StringComparison.OrdinalIgnoreCase)
        || methodName.Equals("post", StringComparison.OrdinalIgnoreCase)
        || methodName.Equals("put", StringComparison.OrdinalIgnoreCase)
        || methodName.Equals("patch", StringComparison.OrdinalIgnoreCase)
        || methodName.Equals("delete", StringComparison.OrdinalIgnoreCase);

    private static ReadOnlyCollection<CitationLink> BuildCitations(string path)
    {
        var segments = new ReadOnlySpan<string>([.. GetPathSegments(path)]);
        if (segments.IsEmpty)
        {
            return [];
        }

        var citations = new List<CitationLink>();
        var type = TryGetSegment(segments, 0)?.ToLowerInvariant();
        switch (type)
        {
            case "team":
                AddTeamCitation(citations, TryGetSegment(segments, 1));
                if (TryGetSegment(segments, 2) is { } teamScope
                    && (teamScope.Equals("event", StringComparison.OrdinalIgnoreCase) || teamScope.Equals("events", StringComparison.OrdinalIgnoreCase)))
                {
                    AddEventCitation(citations, TryGetSegment(segments, 3), TryGetSegment(segments, 4));
                }

                break;
            case "event":
                AddEventCitation(citations, TryGetSegment(segments, 1), TryGetSegment(segments, 2));
                break;
            case "match":
                AddMatchCitation(citations, TryGetSegment(segments, 1));
                break;
            case "district":
                AddDistrictCitation(citations, TryGetSegment(segments, 1), TryGetSegment(segments, 2));
                break;
        }

        return citations.AsReadOnly();
    }

    private static void AddTeamCitation(IList<CitationLink> citations, string? teamKey)
    {
        if (string.IsNullOrWhiteSpace(teamKey))
        {
            return;
        }

        string teamNumber = teamKey.StartsWith("frc", StringComparison.OrdinalIgnoreCase)
            ? teamKey[3..]
            : teamKey;
        citations.Add(new("The Blue Alliance team page", $"https://www.thebluealliance.com/team/{teamNumber}"));
    }

    private static void AddEventCitation(IList<CitationLink> citations, string? eventKey, string? eventSubresource)
    {
        if (string.IsNullOrWhiteSpace(eventKey))
        {
            return;
        }

        string suffix = eventSubresource?.ToLowerInvariant() switch
        {
            "awards" => "#awards",
            "rankings" => "#rankings",
            _ => string.Empty,
        };

        citations.Add(new("The Blue Alliance event page", $"https://www.thebluealliance.com/event/{eventKey}{suffix}"));
    }

    private static void AddMatchCitation(IList<CitationLink> citations, string? matchKey)
    {
        if (string.IsNullOrWhiteSpace(matchKey))
        {
            return;
        }

        citations.Add(new("The Blue Alliance match page", $"https://www.thebluealliance.com/match/{matchKey}"));
    }

    private static void AddDistrictCitation(IList<CitationLink> citations, string? districtKey, string? year)
    {
        if (string.IsNullOrWhiteSpace(districtKey) || string.IsNullOrWhiteSpace(year))
        {
            return;
        }

        citations.Add(new("The Blue Alliance district page", $"https://www.thebluealliance.com/events/{districtKey}/{year}"));
    }

    private static int CountNonEmptySegments(ReadOnlySpan<char> path)
    {
        var collector = new Span<Range>();
        var segments = path.Split(collector, '/', StringSplitOptions.RemoveEmptyEntries);
        return collector.Length;
    }

    private sealed record TbaEndpoint(string Template, string Description, string OperationId, string[] Tags, string[] Parameters)
    {
        public bool Matches(string topic)
        {
            string normalizedTopic = topic.Trim();
            return normalizedTopic.Length is 0
                || Template.Contains(normalizedTopic, StringComparison.OrdinalIgnoreCase)
                || Description.Contains(normalizedTopic, StringComparison.OrdinalIgnoreCase)
                || OperationId.Contains(normalizedTopic, StringComparison.OrdinalIgnoreCase)
                || Tags.Any(tag => tag.Contains(normalizedTopic, StringComparison.OrdinalIgnoreCase))
                || Parameters.Any(parameter => parameter.Contains(normalizedTopic, StringComparison.OrdinalIgnoreCase));
        }

        public bool MatchesConcretePath(string concretePath)
        {
            ReadOnlySpan<char> template = Template.AsSpan();
            ReadOnlySpan<char> path = concretePath.AsSpan();
            if (CountNonEmptySegments(template) != CountNonEmptySegments(path))
            {
                return false;
            }

            var templateSegments = template.Split('/');
            var pathSegments = path.Split('/');
            while (templateSegments.MoveNext() && pathSegments.MoveNext())
            {
                ReadOnlySpan<char> templateSegment = template[templateSegments.Current];
                ReadOnlySpan<char> pathSegment = path[pathSegments.Current];

                if (templateSegment.IsEmpty || pathSegment.IsEmpty)
                {
                    continue;
                }

                if (templateSegment.Length >= 2 && templateSegment[0] == '{' && templateSegment[^1] == '}')
                {
                    if (pathSegment.IsEmpty)
                    {
                        return false;
                    }

                    continue;
                }

                if (!templateSegment.Equals(pathSegment, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }

            return true;
        }
    }

    private sealed class TbaApiSurface(IEnumerable<TbaEndpoint> endpoints)
    {
        public IReadOnlyList<TbaEndpoint> Endpoints { get; } = [.. endpoints];

        public TbaEndpoint? TryMatchConcretePath(string concretePath)
            => Endpoints.FirstOrDefault(endpoint => endpoint.MatchesConcretePath(concretePath));

        public IEnumerable<string> SuggestTemplates(string concretePath, int maxCount)
        {
            return Endpoints
                .Select(endpoint => new
                {
                    endpoint.Template,
                    Score = ScoreTemplate(endpoint.Template.AsSpan(), concretePath.AsSpan()),
                })
                .Where(static item => item.Score > 0)
                .OrderByDescending(item => item.Score)
                .ThenBy(item => item.Template, StringComparer.Ordinal)
                .Select(item => item.Template)
                .Distinct(StringComparer.Ordinal)
                .Take(maxCount);
        }

        private static int ScoreTemplate(ReadOnlySpan<char> template, ReadOnlySpan<char> concretePath)
        {
            int templateSegmentCount = CountNonEmptySegments(template);
            int concreteSegmentCount = CountNonEmptySegments(concretePath);
            if (templateSegmentCount is 0 || concreteSegmentCount is 0)
            {
                return 0;
            }

            int lengthScore = templateSegmentCount == concreteSegmentCount ? 3 : 0;
            int prefixScore = 0;

            var templateSegments = template.Split('/');
            var concreteSegments = concretePath.Split('/');
            while (templateSegments.MoveNext() && concreteSegments.MoveNext())
            {
                ReadOnlySpan<char> templateSegment = template[templateSegments.Current];
                ReadOnlySpan<char> concreteSegment = concretePath[concreteSegments.Current];

                if (templateSegment.IsEmpty || concreteSegment.IsEmpty)
                {
                    continue;
                }

                if (templateSegment.Length >= 2 && templateSegment[0] == '{' && templateSegment[^1] == '}')
                {
                    prefixScore += 1;
                    continue;
                }

                if (templateSegment.Equals(concreteSegment, StringComparison.OrdinalIgnoreCase))
                {
                    prefixScore += 3;
                    continue;
                }

                break;
            }

            return lengthScore + prefixScore;
        }
    }
}
