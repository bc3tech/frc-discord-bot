namespace DiscordBotFunctionApp.Storage;

using Common.Extensions;

using Microsoft.Extensions.Logging;

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;

using TheBlueAlliance.Api;
using TheBlueAlliance.Model;

internal sealed class TeamRepository(ITeamApi _apiClient, ILogger<TeamRepository> _logger)
{
    private static readonly ConcurrentDictionary<string, Team> _teams = [];

    public async ValueTask InitializeAsync(CancellationToken cancellationToken)
    {
        using var scope = _logger.CreateMethodScope();

        _logger.LoadingTeamsFromTBA();
        int i = 0;
        try
        {
            do
            {
                cancellationToken.ThrowIfCancellationRequested();
                var newTeams = await _apiClient.GetTeamsAsync(i++, cancellationToken: cancellationToken).ConfigureAwait(false);
                if (newTeams?.Count is null or 0)
                {
                    break;
                }

                _logger.RetrievedTeamCountTeams(newTeams.Count);

                foreach (var t in newTeams)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    if (_teams.TryAdd(t.Key!, t))
                    {
                        _logger.LogMetric("TeamAdded", 1);
                    }
                }
            } while (true);
        }
        catch (Exception ex)
        {
            Debug.Fail(ex.Message);
            _logger.AnErrorOccurredWhileLoadingTeamsFromTheTBAAPIErrorMessage(ex, ex.Message);
        }

        _logger.CachedTeamCountTeamsFromTBA(_teams.Count);
    }

    /// <summary>
    /// Gets the team associated with the specified team key.
    /// If the team is not found in the cache, it retrieves the team from the TBA API and adds it to the cache.
    /// </summary>
    /// <param name="teamKey">The key of the team to retrieve.</param>
    /// <returns>The team associated with the specified team key.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the team is not found in the cache or the TBA API.</exception>
    public Team this[string teamKey]
    {
        get
        {
            if (_teams.TryGetValue(teamKey, out var t) && t is not null)
            {
                return t;
            }

            _logger.TeamTeamNumberNotFoundInCache(teamKey);
            t = _apiClient.GetTeam(teamKey);
            return t is not null ? _teams.GetOrAdd(teamKey, t) : throw new KeyNotFoundException();
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Not the API we're going for")]
    public IReadOnlyDictionary<string, Team> AllTeams => _teams;

    public string GetLabelForTeam(ushort? teamNumber, bool includeNumber = true, bool includeName = true, bool includeLocation = true) => teamNumber.HasValue ? teamNumber.Value is 0 ? "All" : GetLabelForTeam($"frc{teamNumber.Value}", includeNumber, includeName, includeLocation) : string.Empty;

    public string GetLabelForTeam(string teamKey, bool includeNumber = true, bool includeName = true, bool includeLocation = true)
    {
        using var scope = _logger.CreateMethodScope();
        var t = this[teamKey];
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

        return details.ToString();
    }

    public string GetTeamLabelWithHighlight(string teamKey, ulong? highlightIfIsTeamNumber)
    {
        var teamLabel = GetLabelForTeam(teamKey);
        return highlightIfIsTeamNumber is not null && teamLabel.StartsWith(highlightIfIsTeamNumber.ToString()!, StringComparison.Ordinal)
            ? $"**{teamLabel}**"
            : teamLabel;
    }
}
