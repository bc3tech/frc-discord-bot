namespace DiscordBotFunctionApp.Storage;

using Common.Extensions;

using Microsoft.Extensions.Logging;

using System.Diagnostics;

using TheBlueAlliance.Api;
using TheBlueAlliance.Model;

internal sealed class TeamRepository(ITeamApi tbaApiClient, ILogger<TeamRepository> logger)
{
    private Dictionary<string, Team>? _teams;

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
                _teams = teams.ToDictionary(t => t.Key!);
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

    public string GetLabelForTeam(ushort? teamNumber) => teamNumber.HasValue ? teamNumber.Value is 0 ? "All" : GetLabelForTeam($"frc{teamNumber.Value}") : string.Empty;

    public string GetLabelForTeam(string teamKey)
    {
        using var scope = logger.CreateMethodScope();
        if (_teams?.TryGetValue(teamKey, out var t) is true && t is not null)
        {
            return $"{t.TeamNumber} {(!string.IsNullOrWhiteSpace(t.Nickname) ? t.Nickname : string.Empty)}{(!string.IsNullOrWhiteSpace(t.City) && !string.IsNullOrWhiteSpace(t.Country) ? $" - {t.City}, {t.Country}" : string.Empty)}";
        }

        logger.TeamTeamNumberNotFoundInCache(teamKey);

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
