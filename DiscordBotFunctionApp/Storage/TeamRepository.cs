namespace DiscordBotFunctionApp.Storage;

using Common.Extensions;

using Microsoft.Extensions.Logging;

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Text;

using TheBlueAlliance.Api;
using TheBlueAlliance.Model;

internal sealed class TeamRepository(ITeamApi tbaApiClient, ILogger<TeamRepository> logger)
{
    private ConcurrentDictionary<string, Team>? _teams;

    public Team? this[string teamKey] => _teams?[teamKey];

    public async ValueTask<IReadOnlyDictionary<string, Team>> GetTeamsAsync(CancellationToken cancellationToken)
    {
        using var scope = logger.CreateMethodScope();
        if (_teams is null)
        {
            logger.LoadingTeamsFromTBA();
            List<Team> teams = [];
            int i = 0;
            try
            {
                do
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var newTeams = await tbaApiClient.GetTeamsAsync(i++, cancellationToken: cancellationToken).ConfigureAwait(false);
                    if (newTeams?.Count is null or 0)
                    {
                        break;
                    }

                    teams.AddRange(newTeams);
                } while (true);

                logger.RetrievedTeamCountTeams(teams.Count);
                logger.LogMetric("TeamCount", teams.Count);
                _teams = new(teams.ToDictionary(t => t.Key!));
            }
            catch (Exception ex)
            {
                Debug.Fail(ex.Message);
                logger.AnErrorOccurredWhileLoadingTeamsFromTheTBAAPIErrorMessage(ex, ex.Message);
                _teams = [];
            }
        }

        return _teams;
    }

    public string GetLabelForTeam(ushort? teamNumber, bool includeNumber = true, bool includeName = true, bool includeLocation = true) => teamNumber.HasValue ? teamNumber.Value is 0 ? "All" : GetLabelForTeam($"frc{teamNumber.Value}", includeNumber, includeName, includeLocation) : string.Empty;

    public string GetLabelForTeam(string teamKey, bool includeNumber = true, bool includeName = true, bool includeLocation = true)
    {
        using var scope = logger.CreateMethodScope();
        if (_teams?.TryGetValue(teamKey, out var t) is not true || t is null)
        {
            logger.TeamTeamNumberNotFoundInCache(teamKey);
            t = tbaApiClient.GetTeam(teamKey);
            if (t is not null)
            {
                t = _teams?.GetOrAdd(teamKey, t);
            }
        }

        if (t is not null)
        {
            var details = new StringBuilder();
            if (includeNumber)
            {
                details.Append($"{t.TeamNumber}");
            }

            if (includeName && !string.IsNullOrEmpty(t.Nickname))
            {
                if (details.Length > 0)
                {
                    details.Append(' ');
                }

                details.Append(t.Nickname);
            }

            if (includeLocation)
            {
                var location = new StringBuilder();
                if (!string.IsNullOrEmpty(t.City))
                {
                    location.Append(t.City);
                }

                if (!string.IsNullOrWhiteSpace(t.StateProv))
                {
                    if (location.Length > 0)
                    {
                        location.Append(", ");
                    }

                    location.Append(t.StateProv);
                }

                if (!string.IsNullOrWhiteSpace(t.Country))
                {
                    if (location.Length > 0)
                    {
                        location.Append(", ");
                    }

                    location.Append(t.Country);
                }

                if (location.Length > 0)
                {
                    details.Append($" - {location}");
                }
            }

            details.ToString();
        }

        logger.TeamTeamKeyNotKnownAtAll(teamKey);

        return string.Empty;
    }

    public string GetTeamLabelWithHighlight(string teamKey, ulong? highlightIfIsTeamNumber)
    {
        var teamLabel = GetLabelForTeam(teamKey);
        return highlightIfIsTeamNumber is not null && teamLabel.StartsWith(highlightIfIsTeamNumber.ToString()!, StringComparison.Ordinal)
            ? $"**{teamLabel}**"
            : teamLabel;
    }
}
