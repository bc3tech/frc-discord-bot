namespace ChatBot.Tools;

using BC3Technologies.DiscordGpt.Core;

using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.Json;

internal sealed class StatboticsTool(IHttpClientFactory httpClientFactory, ILogger<StatboticsTool> logger)
    : HttpGetToolBase(httpClientFactory, logger), IDiscordTool
{
    private const string StatboticsApiSurfaceResourceName = "ChatBot.OpenApi.statbotics.json";
    internal const string DescribeSurfaceToolName = "statbotics_api_surface";
    internal const string QueryToolName = "statbotics_api";
    internal const string DescribeSurfaceToolDescription = "Describes the legitimate Statbotics API v3 endpoint surface.";
    internal const string QueryToolDescription = "Calls the public Statbotics API for advanced FRC metrics.";

    public override IReadOnlyList<AIFunction> Functions => field ??=
        [
            AsSurfaceFunction(),
            AIFunctionFactory.Create(QueryStatboticsAsync, CreateSkippableFunctionOptions(QueryToolName)),
        ];

    public override IReadOnlyList<string> ToolNames => [DescribeSurfaceToolName, QueryToolName];

    public string Name => QueryToolName;

    public string Description => QueryToolDescription;

    public AIFunction AsFunction()
        => AIFunctionFactory.Create(
            QueryStatboticsAsync,
            CreateSkippableFunctionOptions(QueryToolName, QueryToolDescription));

    internal AIFunction AsSurfaceFunction()
        => AIFunctionFactory.Create(
            DescribeApiSurfaceAsync,
            CreateSkippableFunctionOptions(DescribeSurfaceToolName, DescribeSurfaceToolDescription));

    [Description("Describes the legitimate Statbotics API v3 surface from the embedded OpenAPI spec. Use this before calling statbotics_api whenever you are not already sure of the exact endpoint template. You can pass a topic like team, team_year, event, match, prediction, rank, or year to narrow the list. Query parameters remain in the statbotics_api query argument; do not put them in the path.")]
    public Task<string> DescribeApiSurfaceAsync(
        [Description("Optional topic or keyword to filter to relevant endpoint templates. Examples: team, team_year, event, match, prediction, rank, year")] string? topic = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        StatboticsApiSurface surface = s_statboticsApiSurface.Value;
        IEnumerable<StatboticsEndpoint> matchingEndpoints = string.IsNullOrWhiteSpace(topic)
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
                endpoint.PathParameters,
                endpoint.QueryParameters,
            }).ToArray().AsReadOnly();

        return Task.FromResult(JsonSerializer.Serialize(new
        {
            baseUrl = "https://api.statbotics.io",
            guidance = """
                Choose one exact path template and substitute only documented path parameters. Put filters such as year, country, state, district, team, event, metric, limit, and offset in the statbotics_api query argument. Example: list current-year events with path=/v3/events and query=year=2026, not /v3/events/2026. When a query parameter declares an Enum, you MUST use one of those exact string values (e.g. type=champs_div, never type=3); the Statbotics server returns HTTP 500 for any value not in the enum.

                FUTURE-EVENT FIELD VALIDITY (event/team_event/match/team_match): rows for events that have not played return 0 or null for many numeric fields — that is missing data, not bad performance. Gate quoting on each row's status field; treat status values "Upcoming" and "Future" as not-yet-played. List endpoints partition per-row.

                event (status):
                  ✅ key, name, year, country, state, district, start_date, end_date, type, week, video, num_teams.
                  ❌ epa.{mean,sd,max,top_8,top_24}, metrics.*, district_points.

                team_event (event_status — note the field name):
                  ✅ team, event, year, country, state, district, type, week, num_teams.
                  ⚠️ pre-event PROJECTION — quote ONLY with explicit "Statbotics' projected" framing; these are predictions, NOT observed performance, and total_points.mean is per-event-projected (differs from the team's season EPA): epa.total_points.{mean,sd}, epa.unitless, epa.norm, epa.conf, epa.breakdown.*, epa.stats.start.
                  ❌ epa.stats.{pre_elim,mean,max}, record.{qual,elim,total}.* (incl. rank, alliance), district_points.

                match (status):
                  ✅ key, event, comp_level, set_number, match_number, time, predicted_time, alliances.*.team_keys, video.
                  ⚠️ projection — frame as predicted: pred.*.
                  ❌ result.*, breakdown.*, all observed score/RP fields.

                team_match (status):
                  ✅ team, match, event, year, comp_level, set_number, match_number, time, alliance.
                  ⚠️ projection: epa.* (pre-match projected contribution).
                  ❌ dq, surrogate post-match flags, observed contribution fields.

                Behavior on ❌ fields for Upcoming/Future rows: REFUSE-AND-REDIRECT. Explain the event/match has not happened yet and offer a substitute when one exists (EPA projection, registered team count). Do NOT report 0/null as a real value.
                """,
            endpointCount = selectedEndpoints.Count,
            endpoints = selectedEndpoints,
        }));
    }

    [Description("Calls the public Statbotics API (https://www.statbotics.io/docs/rest) for FRC advanced metrics. Use this for EPA, Elo, predictions, rankings backed by Statbotics fields, team-year summaries, and event or match performance metrics. Use only legitimate /v3 paths from statbotics_api_surface. For list endpoints, put filters in query, not path: events for a year use path /v3/events with query year=2026, never /v3/events/2026. Optionally provide a query string without a leading question mark.")]
    public Task<string> QueryStatboticsAsync(
        [Description("Relative API path beginning with /v3/ that must match a legitimate Statbotics endpoint template once path parameters are substituted. Examples: /v3/team_year/2046/2025 or /v3/events")] string path,
        [Description("Optional query string without a leading question mark. Use this for list filters. Examples: year=2026, team=2046&year=2025, metric=epa&limit=10")] string? query = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        StatboticsEndpoint? endpoint = s_statboticsApiSurface.Value.TryMatchConcretePath(path);
        if (endpoint is null)
        {
            string suggestions = string.Join(", ", s_statboticsApiSurface.Value.SuggestTemplates(path, 5));
            return Task.FromResult(JsonSerializer.Serialize(new
            {
                apiRequest = new
                {
                    path,
                    kind = "api",
                },
                statusCode = 400,
                ok = false,
                error = "The requested path does not match any legitimate Statbotics API v3 endpoint in the embedded OpenAPI spec.",
                guidance = "Call statbotics_api_surface first to discover the real endpoint template, then retry with a concrete path that matches it. For list filters, keep the path at the collection endpoint and put filters in query; for example path=/v3/events and query=year=2026.",
                suggestions = suggestions.Length > 0 ? suggestions : null,
            }));
        }

        return SendGetAsync(ChatBotConstants.HttpClients.StatboticsApi, path, query, citations: BuildCitations(path), cancellationToken: cancellationToken);
    }

    private static readonly Lazy<StatboticsApiSurface> s_statboticsApiSurface = new(() =>
    {
        using Stream? stream = typeof(StatboticsTool).Assembly.GetManifestResourceStream(StatboticsApiSurfaceResourceName)
            ?? throw new InvalidOperationException($"Embedded Statbotics OpenAPI spec '{StatboticsApiSurfaceResourceName}' was not found.");

        using JsonDocument document = JsonDocument.Parse(stream);
        JsonElement paths = document.RootElement.GetProperty("paths");

        List<StatboticsEndpoint> endpoints = [];
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
                    ? descriptionElement.GetString() ?? string.Empty
                    : string.Empty;
                string operationId = operation.TryGetProperty("operationId", out JsonElement operationIdElement)
                    ? operationIdElement.GetString() ?? string.Empty
                    : string.Empty;
                string[] tags = operation.TryGetProperty("tags", out JsonElement tagsElement)
                    ? [.. tagsElement.EnumerateArray().Select(tag => tag.GetString() ?? string.Empty).Where(static tag => !string.IsNullOrWhiteSpace(tag))]
                    : [];
                (StatboticsParameter[] parameters, StatboticsParameter[] pathParameters, StatboticsParameter[] queryParameters) = operation.TryGetProperty("parameters", out JsonElement parametersElement)
                    ? ReadParameters(parametersElement)
                    : ([], [], []);

                endpoints.Add(new StatboticsEndpoint(pathProperty.Name, description, operationId, tags, parameters, pathParameters, queryParameters));
            }
        }

        return new StatboticsApiSurface(endpoints);
    });

    private static (StatboticsParameter[] Parameters, StatboticsParameter[] PathParameters, StatboticsParameter[] QueryParameters) ReadParameters(JsonElement parametersElement)
    {
        var parameters = new List<StatboticsParameter>();
        var pathParameters = new List<StatboticsParameter>();
        var queryParameters = new List<StatboticsParameter>();
        foreach (JsonElement parameter in parametersElement.EnumerateArray())
        {
            if (parameter.ValueKind != JsonValueKind.Object || !parameter.TryGetProperty("name", out JsonElement nameElement))
            {
                continue;
            }

            string? name = nameElement.GetString();
            if (string.IsNullOrWhiteSpace(name))
            {
                continue;
            }

            string? location = parameter.TryGetProperty("in", out JsonElement locationElement) ? locationElement.GetString() : null;
            bool required = parameter.TryGetProperty("required", out JsonElement requiredElement) && requiredElement.ValueKind == JsonValueKind.True;
            string? description = parameter.TryGetProperty("description", out JsonElement descriptionElement) ? descriptionElement.GetString() : null;

            string? type = null;
            string[]? enumValues = null;
            double? minimum = null;
            double? maximum = null;
            if (parameter.TryGetProperty("schema", out JsonElement schemaElement) && schemaElement.ValueKind == JsonValueKind.Object)
            {
                if (schemaElement.TryGetProperty("type", out JsonElement typeElement))
                {
                    type = typeElement.GetString();
                }

                if (schemaElement.TryGetProperty("enum", out JsonElement enumElement) && enumElement.ValueKind == JsonValueKind.Array)
                {
                    enumValues = [.. enumElement.EnumerateArray()
                        .Select(static value => value.ValueKind == JsonValueKind.String ? value.GetString() : value.GetRawText())
                        .Where(static value => !string.IsNullOrWhiteSpace(value))
                        .Select(static value => value!)];
                }

                if (schemaElement.TryGetProperty("minimum", out JsonElement minimumElement) && minimumElement.ValueKind == JsonValueKind.Number)
                {
                    minimum = minimumElement.GetDouble();
                }

                if (schemaElement.TryGetProperty("maximum", out JsonElement maximumElement) && maximumElement.ValueKind == JsonValueKind.Number)
                {
                    maximum = maximumElement.GetDouble();
                }
            }

            var descriptor = new StatboticsParameter(name, location, required, type, enumValues, minimum, maximum, description);

            parameters.Add(descriptor);
            if (string.Equals(location, "path", StringComparison.OrdinalIgnoreCase))
            {
                pathParameters.Add(descriptor);
            }
            else if (string.Equals(location, "query", StringComparison.OrdinalIgnoreCase))
            {
                queryParameters.Add(descriptor);
            }
        }

        return ([.. parameters], [.. pathParameters], [.. queryParameters]);
    }

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

        int offset = segments[0].ToString().Equals("v3", StringComparison.OrdinalIgnoreCase) ? 1 : 0;
        if (segments.Length <= offset)
        {
            return [];
        }

        var citations = new List<CitationLink>();
        var citationType = TryGetSegment(segments, offset)?.ToLowerInvariant();
        switch (citationType)
        {
            case "team":
                AddTeamCitation(citations, TryGetSegment(segments, offset + 1));
                break;
            case "team_year":
                AddTeamYearCitation(citations, TryGetSegment(segments, offset + 1), TryGetSegment(segments, offset + 2));
                break;
            case "team_event":
            case "team_matches":
            case "team_match":
                AddTeamCitation(citations, TryGetSegment(segments, offset + 1));
                AddEventCitation(citations, TryGetSegment(segments, offset + 2));
                break;
            case "event":
            case "event_predictions":
            case "event_matches":
                AddEventCitation(citations, TryGetSegment(segments, offset + 1));
                break;
            case "match":
                AddMatchCitation(citations, TryGetSegment(segments, offset + 1));
                break;
        }

        return citations.AsReadOnly();
    }

    private static void AddTeamCitation(IList<CitationLink> citations, string? teamNumber)
    {
        if (string.IsNullOrWhiteSpace(teamNumber))
        {
            return;
        }

        citations.Add(new("Statbotics team page", $"https://www.statbotics.io/team/{teamNumber}"));
    }

    private static void AddTeamYearCitation(IList<CitationLink> citations, string? teamNumber, string? year)
    {
        if (string.IsNullOrWhiteSpace(teamNumber))
        {
            return;
        }

        if (!string.IsNullOrWhiteSpace(year))
        {
            citations.Add(new("Statbotics team season page", $"https://www.statbotics.io/team/{teamNumber}/{year}"));
        }

        AddTeamCitation(citations, teamNumber);
    }

    private static void AddEventCitation(IList<CitationLink> citations, string? eventKey)
    {
        if (string.IsNullOrWhiteSpace(eventKey))
        {
            return;
        }

        citations.Add(new("Statbotics event page", $"https://www.statbotics.io/event/{eventKey}"));
    }

    private static void AddMatchCitation(IList<CitationLink> citations, string? matchKey)
    {
        if (string.IsNullOrWhiteSpace(matchKey))
        {
            return;
        }

        citations.Add(new("Statbotics match page", $"https://www.statbotics.io/match/{matchKey}"));
    }

    private static int CountNonEmptySegments(ReadOnlySpan<char> path)
    {
        int count = 0;
        MemoryExtensions.SpanSplitEnumerator<char> segments = path.Split('/');
        while (segments.MoveNext())
        {
            if (!path[segments.Current].IsEmpty)
            {
                count++;
            }
        }

        return count;
    }

    private sealed record StatboticsParameter(
        string Name,
        string? In,
        bool Required,
        string? Type,
        string[]? Enum,
        double? Minimum,
        double? Maximum,
        string? Description);

    private sealed record StatboticsEndpoint(
        string Template,
        string Description,
        string OperationId,
        string[] Tags,
        StatboticsParameter[] Parameters,
        StatboticsParameter[] PathParameters,
        StatboticsParameter[] QueryParameters)
    {
        public bool Matches(string topic)
        {
            string normalizedTopic = topic.Trim();
            return normalizedTopic.Length is 0
                || Template.Contains(normalizedTopic, StringComparison.OrdinalIgnoreCase)
                || Description.Contains(normalizedTopic, StringComparison.OrdinalIgnoreCase)
                || OperationId.Contains(normalizedTopic, StringComparison.OrdinalIgnoreCase)
                || Tags.Any(tag => tag.Contains(normalizedTopic, StringComparison.OrdinalIgnoreCase))
                || Parameters.Any(parameter => parameter.Name.Contains(normalizedTopic, StringComparison.OrdinalIgnoreCase));
        }

        public bool MatchesConcretePath(string concretePath)
        {
            ReadOnlySpan<char> template = Template.AsSpan();
            ReadOnlySpan<char> path = concretePath.AsSpan();
            if (CountNonEmptySegments(template) != CountNonEmptySegments(path))
            {
                return false;
            }

            MemoryExtensions.SpanSplitEnumerator<char> templateSegments = template.Split('/');
            MemoryExtensions.SpanSplitEnumerator<char> pathSegments = path.Split('/');
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

    private sealed class StatboticsApiSurface(IEnumerable<StatboticsEndpoint> endpoints)
    {
        public IReadOnlyList<StatboticsEndpoint> Endpoints { get; } = [.. endpoints];

        public StatboticsEndpoint? TryMatchConcretePath(string concretePath)
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

            MemoryExtensions.SpanSplitEnumerator<char> templateSegments = template.Split('/');
            MemoryExtensions.SpanSplitEnumerator<char> concreteSegments = concretePath.Split('/');
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
                    prefixScore++;
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
