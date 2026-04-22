namespace ChatBot.Tools;

using BC3Technologies.DiscordGpt.Core;

using Common;
using Common.Extensions;

using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

internal sealed class TbaApiTool(
    IConfiguration configuration,
    IHttpClientFactory httpClientFactory,
    ILogger<TbaApiTool> logger) : HttpGetToolBase(httpClientFactory, logger), IDiscordTool
{
    private const string TbaApiSurfaceResourceName = "ChatBot.OpenApi.thebluealliance.json";
    internal const string DescribeSurfaceToolName = "tba_api_surface";
    internal const string QueryToolName = "tba_api";
    internal const string LastCompetitionToolName = "tba_last_comp";
    internal const string DescribeSurfaceToolDescription = "Describes the legitimate The Blue Alliance API v3 endpoint surface.";
    internal const string QueryToolDescription = "Calls the official The Blue Alliance API v3 for FRC competition data.";
    internal const string LastCompetitionToolDescription = "Resolves a team's most recent completed competition and grounded result details from TBA.";

    private readonly string _tbaApiKey = Throws.IfNullOrWhiteSpace(configuration["TbaApiKey"]);

    public override IReadOnlyList<AIFunction> Functions => field ??=
        [
            AsSurfaceFunction(),
            AsFunction(),
            AsLastCompetitionFunction(),
        ];

    public override IReadOnlyList<string> ToolNames => [DescribeSurfaceToolName, QueryToolName, LastCompetitionToolName];

    public string Name => QueryToolName;

    public string Description => QueryToolDescription;

    public AIFunction AsFunction()
        => AIFunctionFactory.Create(
            QueryTbaAsync,
            CreateSkippableFunctionOptions(QueryToolName, QueryToolDescription));

    internal AIFunction AsSurfaceFunction()
        => AIFunctionFactory.Create(
            DescribeApiSurfaceAsync,
            CreateSkippableFunctionOptions(DescribeSurfaceToolName, DescribeSurfaceToolDescription));

    internal AIFunction AsLastCompetitionFunction()
        => AIFunctionFactory.Create(
            ResolveLastCompetitionAsync,
            CreateSkippableFunctionOptions(LastCompetitionToolName, LastCompetitionToolDescription));

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
    public async Task<string> QueryTbaAsync(
        [Description("Relative API path beginning with / that must match a legitimate TBA API v3 endpoint template once path parameters are substituted. Example: /team/frc2046/events/2025/simple")] string path,
        [Description("Optional query string without a leading question mark. Example: keys=1")] string? query = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        TbaEndpoint? endpoint = s_tbaApiSurface.Value.TryMatchConcretePath(path);
        if (endpoint is null)
        {
            string suggestions = string.Join(", ", s_tbaApiSurface.Value.SuggestTemplates(path, 5));
            return JsonSerializer.Serialize(new
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
            });
        }

        string response = await SendGetAsync(
            clientName: "tba-api",
            path,
            query,
            citations: BuildCitations(path),
            cancellationToken: cancellationToken).ConfigureAwait(false);

        return AugmentResponseWithRecoveryHints(response, path);
    }

    internal static string AugmentResponseWithRecoveryHints(string serializedResponse, string path)
    {
        using JsonDocument doc = JsonDocument.Parse(serializedResponse);
        JsonElement root = doc.RootElement;

        bool ok = root.TryGetProperty("ok", out JsonElement okElement) && okElement.GetBoolean();
        bool isEmpty = ok && IsEmptyData(root);

        if (ok && !isEmpty)
        {
            return serializedResponse;
        }

        List<string> hints = BuildRecoveryHints(path, ok);
        if (hints.Count is 0)
        {
            return serializedResponse;
        }

        // Re-serialize with recovery hints appended
        var augmented = new Dictionary<string, JsonElement>();
        foreach (JsonProperty property in root.EnumerateObject())
        {
            augmented[property.Name] = property.Value.Clone();
        }

        augmented["recoveryHints"] = JsonSerializer.SerializeToElement(hints);
        if (isEmpty)
        {
            augmented["guidance"] = JsonSerializer.SerializeToElement(
                "The request succeeded but returned no data. This usually means the season hasn't started, the resource doesn't exist yet, or the parameters are wrong. Follow the recovery hints before asking the user.");
        }

        return JsonSerializer.Serialize(augmented);
    }

    private static bool IsEmptyData(JsonElement root)
    {
        return !root.TryGetProperty("data", out JsonElement data) || data.ValueKind switch
        {
            JsonValueKind.Null or JsonValueKind.Undefined => true,
            JsonValueKind.Array => data.GetArrayLength() is 0,
            JsonValueKind.Object => !data.EnumerateObject().Any(),
            JsonValueKind.String => data.GetString() is null or { Length: 0 },
            _ => false,
        };
    }

    internal static List<string> BuildRecoveryHints(string path, bool ok)
    {
        List<string> hints = [];
        ReadOnlyCollection<string> segments = GetPathSegments(path);
        if (segments.Count is 0)
        {
            return hints;
        }

        string rootType = segments[0].ToLowerInvariant();
        int? yearInPath = TryExtractYear(segments);

        switch (rootType)
        {
            case "team" when segments.Count >= 4 && segments[2].Equals("events", StringComparison.OrdinalIgnoreCase):
                // /team/frc{N}/events/{year}/...
                if (yearInPath is not null)
                {
                    hints.Add($"No events found for this team in {yearInPath}. The {yearInPath} season may not have data yet. Try the previous year: {path.Replace($"/{yearInPath}/", $"/{yearInPath - 1}/")}");
                }

                break;

            case "team" when segments.Count >= 4 && segments[2].Equals("event", StringComparison.OrdinalIgnoreCase):
                // /team/frc{N}/event/{eventKey}/...
                string teamKey = segments[1];
                hints.Add($"Event key '{segments[3]}' not found or has no data for team {teamKey}. List the team's events first to find valid event keys: /team/{teamKey}/events/{yearInPath ?? DateTime.UtcNow.Year}/simple");
                if (yearInPath is not null)
                {
                    hints.Add($"If the current year has no data, try: /team/{teamKey}/events/{yearInPath - 1}/simple");
                }

                break;

            case "event":
                // /event/{eventKey}/...
                if (segments.Count >= 2)
                {
                    string eventKey = segments[1];
                    int inferredYear = TryExtractYearFromEventKey(eventKey) ?? DateTime.UtcNow.Year;
                    hints.Add($"Event '{eventKey}' not found or has no data. List all events for the year to find the correct key: /events/{inferredYear}/simple");
                    if (inferredYear == DateTime.UtcNow.Year)
                    {
                        hints.Add($"If the current year has no events yet, try: /events/{inferredYear - 1}/simple");
                    }
                }

                break;

            case "match":
                // /match/{matchKey}/...
                if (segments.Count >= 2)
                {
                    string matchKey = segments[1];
                    string? eventKeyFromMatch = TryExtractEventKeyFromMatchKey(matchKey);
                    if (eventKeyFromMatch is not null)
                    {
                        hints.Add($"Match '{matchKey}' not found. List matches for the event: /event/{eventKeyFromMatch}/matches/simple");
                    }
                    else
                    {
                        hints.Add($"Match key '{matchKey}' not found. Verify the event key first, then list matches: /event/{{eventKey}}/matches/simple");
                    }
                }

                break;

            case "events":
                // /events/{year}/...
                if (yearInPath is not null)
                {
                    hints.Add($"No events found for {yearInPath}. The season may not have started. Try: /events/{yearInPath - 1}/simple");
                }

                break;

            case "district":
                // /district/{districtKey}/...
                if (yearInPath is not null && segments.Count >= 2)
                {
                    hints.Add($"No data for district '{segments[1]}' in {yearInPath}. Try the previous year or verify the district abbreviation via /districts/{yearInPath}/simple");
                }

                break;
        }

        // Generic year-based fallback if no specific hint was generated
        if (hints.Count is 0 && yearInPath is not null && yearInPath >= DateTime.UtcNow.Year)
        {
            hints.Add($"No data found for {yearInPath}. The season may not have started yet. Try replacing the year with {yearInPath - 1}.");
        }

        if (!ok)
        {
            hints.Add("Do NOT ask the user for missing parameters. Try the suggested fallback paths first.");
        }

        return hints;
    }

    private static int? TryExtractYear(IReadOnlyList<string> segments)
    {
        foreach (string segment in segments)
        {
            if (segment.Length is 4 && int.TryParse(segment, CultureInfo.InvariantCulture, out int year) && year is >= 1992 and <= 2100)
            {
                return year;
            }
        }

        return null;
    }

    private static int? TryExtractYearFromEventKey(string eventKey)
    {
        // TBA event keys start with a 4-digit year, e.g. "2025pnwcmp"
        return eventKey.Length >= 4 && int.TryParse(eventKey.AsSpan(0, 4), CultureInfo.InvariantCulture, out int year) && year is >= 1992 and <= 2100
            ? year
            : null;
    }

    private static string? TryExtractEventKeyFromMatchKey(string matchKey)
    {
        // TBA match keys: "{eventKey}_qm1", "{eventKey}_sf1m1", etc.
        int underscoreIndex = matchKey.LastIndexOf('_');
        return underscoreIndex > 0 ? matchKey[..underscoreIndex] : null;
    }

    [Description("Resolves the team's most recent completed competition in TBA and returns grounded status + awards. Prefer this for questions like 'last comp', 'how did we do at our last event', or when the user gives a natural-language event name without a key. If seasonYear is omitted, the tool starts with the current year and falls back to earlier seasons.")]
    public async Task<string> ResolveLastCompetitionAsync(
        [Description("FRC team number. Example: 2046")] int teamNumber,
        [Description("Optional season year. Example: 2026. Omit to start from current year and automatically fall back.")] int? seasonYear = null,
        [Description("Optional natural-language event hint. Example: 'pnw district championships'.")] string? eventNameHint = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (teamNumber <= 0)
        {
            return JsonSerializer.Serialize(new
            {
                ok = false,
                error = "teamNumber must be a positive integer.",
            });
        }

        int currentYear = DateTime.UtcNow.Year;
        int startYear = seasonYear ?? currentYear;
        List<int> attemptedYears = [];
        TbaEventSummary? selectedEvent = null;
        int selectedYear = startYear;
        for (int year = startYear; year >= Math.Max(startYear - 2, 1992); year--)
        {
            attemptedYears.Add(year);
            TbaApiResult eventsResult = await GetTbaDataAsync($"/team/frc{teamNumber}/events/{year}/simple", cancellationToken).ConfigureAwait(false);
            if (!eventsResult.Ok || eventsResult.Data is null || eventsResult.Data.Value.ValueKind is not JsonValueKind.Array)
            {
                continue;
            }

            TbaEventSummary[] events = eventsResult.Data.Value
                .Deserialize<TbaEventSummary[]>(s_jsonOptions)
                ?? [];
            if (events.Length is 0)
            {
                continue;
            }

            selectedEvent = SelectEvent(events, eventNameHint);
            if (selectedEvent is not null)
            {
                selectedYear = year;
                break;
            }
        }

        if (selectedEvent is null || string.IsNullOrWhiteSpace(selectedEvent.Key))
        {
            return JsonSerializer.Serialize(new
            {
                ok = false,
                teamNumber,
                requestedSeasonYear = seasonYear,
                attemptedYears,
                usedEventNameHint = !string.IsNullOrWhiteSpace(eventNameHint),
                error = "Unable to resolve a completed event for this team from TBA. Try providing an exact event key.",
            });
        }

        string eventKey = selectedEvent.Key!;
        Task<TbaApiResult> statusTask = GetTbaDataAsync($"/team/frc{teamNumber}/event/{eventKey}/status", cancellationToken);
        Task<TbaApiResult> awardsTask = GetTbaDataAsync($"/team/frc{teamNumber}/event/{eventKey}/awards", cancellationToken);
        await Task.WhenAll(statusTask, awardsTask).ConfigureAwait(false);

        return JsonSerializer.Serialize(new
        {
            ok = true,
            teamNumber,
            seasonYear = selectedYear,
            requestedSeasonYear = seasonYear,
            attemptedYears,
            usedEventNameHint = !string.IsNullOrWhiteSpace(eventNameHint),
            eventHint = string.IsNullOrWhiteSpace(eventNameHint) ? null : eventNameHint.Trim(),
            eventData = selectedEvent,
            teamStatus = (await statusTask).Data,
            teamAwards = (await awardsTask).Data,
            userReferencePages = new[]
            {
                new { title = "The Blue Alliance event page", url = $"https://www.thebluealliance.com/event/{eventKey}" },
                new { title = "The Blue Alliance team page", url = $"https://www.thebluealliance.com/team/{teamNumber}" },
            },
            citations = new[]
            {
                new CitationLink("The Blue Alliance event page", $"https://www.thebluealliance.com/event/{eventKey}"),
                new CitationLink("The Blue Alliance team page", $"https://www.thebluealliance.com/team/{teamNumber}"),
            },
        });
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

    private readonly record struct TbaApiResult(bool Ok, JsonElement? Data, string? Error);

    private sealed record TbaEventSummary(
        [property: JsonPropertyName("key")] string? Key,
        [property: JsonPropertyName("name")] string? Name,
        [property: JsonPropertyName("short_name")] string? ShortName,
        [property: JsonPropertyName("event_code")] string? EventCode,
        [property: JsonPropertyName("start_date")] string? StartDate,
        [property: JsonPropertyName("end_date")] string? EndDate,
        [property: JsonPropertyName("week")] int? Week,
        [property: JsonPropertyName("district")] TbaDistrictSummary? District);

    private sealed record TbaDistrictSummary(
        [property: JsonPropertyName("abbreviation")] string? Abbreviation,
        [property: JsonPropertyName("display_name")] string? DisplayName);

    private static readonly JsonSerializerOptions s_jsonOptions = new(JsonSerializerDefaults.Web);

    private static TbaEventSummary? SelectEvent(IEnumerable<TbaEventSummary> events, string? eventNameHint)
    {
        DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
        TbaEventSummary[] materializedEvents = [.. events.Where(static e => !string.IsNullOrWhiteSpace(e.Key))];
        if (materializedEvents.Length is 0)
        {
            return null;
        }

        TbaEventSummary[] completed = [.. materializedEvents.Where(e => IsCompletedEvent(e, today))];
        TbaEventSummary[] candidates = completed.Length > 0 ? completed : materializedEvents;
        if (!string.IsNullOrWhiteSpace(eventNameHint))
        {
            string normalizedHint = eventNameHint.Trim().ToLowerInvariant();
            string[] hintTokens = TokenizeHint(normalizedHint);

            TbaEventSummary? bestHintMatch = candidates
                .Select(e => new
                {
                    Event = e,
                    Score = ScoreEventNameHint(e, normalizedHint, hintTokens),
                })
                .Where(static x => x.Score > 0)
                .OrderByDescending(static x => x.Score)
                .ThenByDescending(x => ParseEventDate(x.Event.EndDate) ?? DateOnly.MinValue)
                .Select(static x => x.Event)
                .FirstOrDefault();
            if (bestHintMatch is not null)
            {
                return bestHintMatch;
            }
        }

        return candidates
            .OrderByDescending(e => ParseEventDate(e.EndDate) ?? DateOnly.MinValue)
            .ThenByDescending(static e => e.Week ?? int.MinValue)
            .ThenByDescending(static e => e.Key, StringComparer.Ordinal)
            .FirstOrDefault();
    }

    private static bool IsCompletedEvent(TbaEventSummary evt, DateOnly today)
    {
        DateOnly? endDate = ParseEventDate(evt.EndDate);
        if (endDate is not null)
        {
            return endDate.Value <= today;
        }

        DateOnly? startDate = ParseEventDate(evt.StartDate);
        return startDate is not null && startDate.Value <= today;
    }

    private static DateOnly? ParseEventDate(string? value)
        => DateOnly.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly date)
            ? date
            : null;

    private static int ScoreEventNameHint(TbaEventSummary evt, string normalizedHint, IReadOnlyList<string> hintTokens)
    {
        string searchSpace = string.Join(
            ' ',
            new[]
            {
                evt.Name,
                evt.ShortName,
                evt.EventCode,
                evt.Key,
                evt.District?.Abbreviation,
                evt.District?.DisplayName,
            }.Where(static value => !string.IsNullOrWhiteSpace(value)))
            .ToLowerInvariant();

        int score = 0;
        if (searchSpace.Contains(normalizedHint, StringComparison.Ordinal))
        {
            score += 50;
        }

        foreach (string token in hintTokens)
        {
            if (searchSpace.Contains(token, StringComparison.Ordinal))
            {
                score += 10;
                continue;
            }

            if (token is "pnw" && searchSpace.Contains("pacific northwest", StringComparison.Ordinal))
            {
                score += 10;
            }
        }

        return score;
    }

    private static string[] TokenizeHint(string normalizedHint)
    {
        ReadOnlySpan<char> source = normalizedHint.AsSpan();
        Span<Range> ranges = stackalloc Range[Math.Min(source.Length, 64)];
        int tokenCount = source.SplitAny(ranges, [' ', '-', '_', '/', ','], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (tokenCount is 0)
        {
            return [];
        }

        var tokens = new List<string>(tokenCount);
        for (int i = 0; i < tokenCount; i++)
        {
            ReadOnlySpan<char> token = source[ranges[i]].Trim();
            if (token.Length > 1)
            {
                tokens.Add(token.ToString());
            }
        }

        return [.. tokens];
    }

    private async Task<TbaApiResult> GetTbaDataAsync(string path, CancellationToken cancellationToken)
    {
        string response = await SendGetAsync(
            clientName: "tba-api",
            path: path,
            query: null,
            citations: BuildCitations(path),
            cancellationToken: cancellationToken).ConfigureAwait(false);

        using JsonDocument doc = JsonDocument.Parse(response);
        JsonElement root = doc.RootElement;
        bool ok = root.TryGetProperty("ok", out JsonElement okElement) && okElement.GetBoolean();
        JsonElement? data = root.TryGetProperty("data", out JsonElement dataElement) && dataElement.ValueKind is not JsonValueKind.Null
            ? dataElement.Clone()
            : null;
        string? error = root.TryGetProperty("error", out JsonElement errorElement) && errorElement.ValueKind is JsonValueKind.String
            ? errorElement.GetString()
            : null;

        return new TbaApiResult(ok, data, error);
    }
}
