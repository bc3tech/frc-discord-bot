namespace DiscordBotFunctionApp.Storage;

using Common.Extensions;

using Microsoft.Extensions.Logging;

using System.Collections.Concurrent;
using System.Diagnostics;

using TheBlueAlliance.Api;
using TheBlueAlliance.Model;

internal sealed class TeamRepository(ITeamApi apiClient, ILogger<TeamRepository> logger)
{
    private static readonly ConcurrentDictionary<string, Team> _teams = [];
    private static readonly ConcurrentQueue<Task> LogMetricTasks = [];

    public async ValueTask InitializeAsync(CancellationToken cancellationToken)
    {
        using var scope = logger.CreateMethodScope();

        logger.LoadingTeamsFromTBA();
        int i = 0;
        try
        {
            do
            {
                cancellationToken.ThrowIfCancellationRequested();
                var newTeams = await apiClient.GetTeamsAsync(i++, cancellationToken: cancellationToken).ConfigureAwait(false);
                if (newTeams?.Count is null or 0)
                {
                    break;
                }

                logger.RetrievedTeamCountTeams(newTeams.Count);

                foreach (var t in newTeams)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    if (_teams.TryAdd(t.Key!, t))
                    {
                        LogMetricTasks.Enqueue(Task.Run(() => logger.LogMetric("TeamAdded", 1), cancellationToken));
                    }
                }
            } while (true);
        }
        catch (Exception ex)
        {
            Debug.Fail(ex.Message);
            logger.AnErrorOccurredWhileLoadingTeamsFromTheTBAAPIErrorMessage(ex, ex.Message);
        }

        logger.CachedTeamCountTeamsFromTBA(_teams.Count);

        Task.Run(async () =>
        {
            await Task.WhenAll(LogMetricTasks);
            LogMetricTasks.Clear();
        }, cancellationToken);
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

            logger.TeamTeamNumberNotFoundInCache(teamKey);
            t = apiClient.GetTeam(teamKey);
            return t is not null ? _teams.GetOrAdd(teamKey, t) : throw new KeyNotFoundException();
        }
    }

    public Team this[ushort teamNumber] => this[$"frc{teamNumber}"];

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Not the API we're going for")]
    public IReadOnlyDictionary<string, Team> AllTeams => _teams;
}
