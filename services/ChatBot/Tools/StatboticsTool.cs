namespace ChatBot.Tools;

using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

using System.Collections.ObjectModel;
using System.ComponentModel;

internal sealed class StatboticsTool(IHttpClientFactory httpClientFactory, ILogger<StatboticsTool> logger) : HttpGetToolBase(httpClientFactory, logger)
{
    public override IReadOnlyList<AIFunction> Functions => field ??=
        [
            AIFunctionFactory.Create(QueryStatboticsAsync, name: "statbotics_api"),
        ];

    [Description("Calls the public Statbotics API (https://www.statbotics.io/docs/rest) for FRC advanced metrics. Use this for EPA, Elo, predictions, rankings backed by Statbotics fields, team-year summaries, and event or match performance metrics. Provide a safe relative API path beginning with /v3/. Example: /v3/team_year/2046/2025. Optionally provide a query string without a leading question mark.")]
    public Task<string> QueryStatboticsAsync(
        [Description("Relative API path beginning with /v3/. Example: /v3/team_year/2046/2025")] string path,
        [Description("Optional query string without a leading question mark. Example: metric=epa&limit=10")] string? query = null,
        CancellationToken cancellationToken = default)
        => SendGetAsync("statbotics-api", path, query, citations: BuildCitations(path), cancellationToken: cancellationToken);

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
}
