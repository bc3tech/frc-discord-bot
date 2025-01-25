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
            logger.LogDebug("Loading Teams from TBA...");
            List<Team> teams = [];
            int i = 0;
            try
            {
                do
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var newTeams = await tbaApiClient.GetTeamsAsync(i++, cancellationToken: cancellationToken).ConfigureAwait(false);
                    if (newTeams.Count is 0)
                    {
                        break;
                    }

                    teams.AddRange(newTeams);
                } while (true);

                logger.LogInformation("Retrieved {TeamCount} teams", teams.Count);
                _teams = teams.ToDictionary(t => t.Key!);
            }
            catch (Exception ex)
            {
                Debug.Fail(ex.Message);
                logger.LogError(ex, "An error occurred while loading teams from the TBA API: {ErrorMessage}", ex.Message);
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

        logger.LogWarning("Team {TeamNumber} not found in cache", teamKey);

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
