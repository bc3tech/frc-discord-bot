namespace DataDumper;

using Azure.Storage.Blobs;

using Common;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using System.Text.Json;

using TheBlueAlliance.Api;
using TheBlueAlliance.Client;

internal sealed class ConsoleHost(BlobServiceClient storage, ITeamApi teams, IEventApi events, IMatchApi matches, IConfiguration appConfig, ILogger<ConsoleHost> logger) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        logger.StartingTBADataDumper();

        try
        {
            List<Task> workers = [];

            logger.ConnectingToStorageAccountYouMayBePromptedToLogIn();
            var eventDataContainer = storage.GetBlobContainerClient("tba-events");
            await eventDataContainer.CreateIfNotExistsAsync(cancellationToken: cancellationToken);
            var matchesDataContainer = storage.GetBlobContainerClient("tba-matches");
            await matchesDataContainer.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

            var startYear = int.Parse(Throws.IfNullOrWhiteSpace(appConfig["StartYear"]));
            logger.PullingEventsMatchesSinceStartYear(startYear);

            var currentYear = TimeProvider.System.GetUtcNow().Year;
            workers.Add(Parallel.ForEachAsync(Enumerable.Range(startYear, currentYear - startYear + 1), cancellationToken,
                async (year, ct) =>
            {
                logger.GettingEventsFromTBAForYear(year);
                try
                {
                    var eventsList = await events.GetEventsByYearAsync(year, cancellationToken: cancellationToken);
                    if (eventsList?.Count is null or 0)
                    {
                        return;
                    }

                    logger.GotNumEventsEventsFromTBA(eventsList.Count);
                    await uploadEventsAsync(eventDataContainer, eventsList, cancellationToken);

                    await Parallel.ForEachAsync(eventsList, ct, async (i, ct2) =>
                    {
                        try
                        {
                            var eventMatches = await matches.GetEventMatchesAsync(i.Key, cancellationToken: cancellationToken);
                            logger.GotNumMatchesMatchesForEventKey(eventMatches?.Count ?? 0, i.Key);

                            await uploadMatchesAsync(matchesDataContainer, eventMatches, cancellationToken);
                        }
                        catch (ApiException ex)
                        {
                            logger.ErrorGettingMatchesForEventKey(ex, i.Key);
                        }
                    }).ConfigureAwait(false);
                }
                catch (ApiException ex)
                {
                    logger.ErrorGettingEventsForYear(ex, year);
                    return;
                }
            }));

            var teamDataContainer = storage.GetBlobContainerClient("tba-teams");
            await teamDataContainer.CreateIfNotExistsAsync(cancellationToken: cancellationToken);
            for (int pageNum = 0; ; pageNum++)
            {
                try
                {
                    var teamsList = await teams.GetTeamsAsync(pageNum, cancellationToken: cancellationToken);
                    if (teamsList?.Count is null or 0)
                    {
                        break;
                    }

                    logger.GotNumTeamsTeamsFromTBA(teamsList.Count);

                    workers.Add(uploadTeamsAsync(teamDataContainer, teamsList, cancellationToken));
                }
                catch (ApiException ex)
                {
                    logger.ErrorGettingTeamsForPagePageNum(ex, pageNum);
                    break;
                }
            }

            await Task.WhenAll(workers);

            Console.WriteLine("DONE");
        }
        catch (Exception ex) when (ex is TaskCanceledException or OperationCanceledException)
        {
            logger.OperationCancelled();
        }
        catch (Exception ex)
        {
            logger.Error(ex);
        }

        async Task uploadTeamsAsync(BlobContainerClient teamDataContainer, System.Collections.ObjectModel.Collection<TheBlueAlliance.Model.Team> teamsList, CancellationToken cancellationToken)
        {
            List<Task> workers = [];
            foreach (var team in teamsList)
            {
                logger.UploadingTeamTeamKey(team.Key);

                var blob = teamDataContainer.GetBlobClient($"{team.Key}.json");
                workers.Add(blob.UploadAsync(new BinaryData(JsonSerializer.Serialize(team)), overwrite: true, cancellationToken: cancellationToken).ContinueWith((_) => logger.UploadedTeamTeamKey(team.Key), scheduler: TaskScheduler.Default, cancellationToken: cancellationToken, continuationOptions: TaskContinuationOptions.None));
            }

            await Task.WhenAll(workers);
        }

        async Task uploadEventsAsync(BlobContainerClient eventDataContainer, System.Collections.ObjectModel.Collection<TheBlueAlliance.Model.Event> eventsList, CancellationToken cancellationToken)
        {
            foreach (var evt in eventsList)
            {
                logger.UploadingEventEventKey(evt.Key);
                var blob = eventDataContainer.GetBlobClient($"{evt.Key}.json");
                await blob.UploadAsync(new BinaryData(JsonSerializer.Serialize(evt)), overwrite: true, cancellationToken: cancellationToken);
                logger.UploadedEventEventKey(evt.Key);
            }
        }

        async Task uploadMatchesAsync(BlobContainerClient matchesDataContainer, System.Collections.ObjectModel.Collection<TheBlueAlliance.Model.Match>? eventMatches, CancellationToken cancellationToken)
        {
            if (eventMatches is not null and { Count: > 0 })
            {
                foreach (var match in eventMatches)
                {
                    logger.UploadingMatchMatchKey(match.Key);
                    var blob = matchesDataContainer.GetBlobClient($"{match.Key}.json");
                    await blob.UploadAsync(new BinaryData(JsonSerializer.Serialize(match)), overwrite: true, cancellationToken: cancellationToken);
                    logger.UploadedMatchMatchKey(match.Key);
                }
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}