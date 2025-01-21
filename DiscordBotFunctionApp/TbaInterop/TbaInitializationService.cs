namespace DiscordBotFunctionApp.TbaInterop;

using DiscordBotFunctionApp.Storage;

using Microsoft.Extensions.Hosting;

using System.Threading;
using System.Threading.Tasks;

internal sealed class TbaInitializationService(EventRepository eventsRepo, TeamRepository teamsRepo) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await Task.WhenAll(
            // Preload the events so Autocomplete is fast
            eventsRepo.GetEventsAsync(cancellationToken).AsTask(),
            // Preload the teams so Autocomplete is fast
            teamsRepo.GetTeamsAsync(cancellationToken).AsTask()
        ).ConfigureAwait(false);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
