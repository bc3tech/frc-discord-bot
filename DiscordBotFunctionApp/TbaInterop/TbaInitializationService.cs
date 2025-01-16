namespace DiscordBotFunctionApp.TbaInterop;

using DiscordBotFunctionApp.Storage;

using Microsoft.Extensions.Hosting;

using System.Threading;
using System.Threading.Tasks;

internal class TbaInitializationService(EventRepository eventsRepo, TeamRepository teamsRepo) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // Preload the events so Autocomplete is fast
        await eventsRepo.GetEventsAsync(cancellationToken);

        // Preload the teams so Autocomplete is fast
        await teamsRepo.GetTeamsAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
