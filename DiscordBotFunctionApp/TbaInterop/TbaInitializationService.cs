namespace DiscordBotFunctionApp.TbaInterop;

using DiscordBotFunctionApp.Storage;

using Microsoft.Extensions.Hosting;

using System.Threading;
using System.Threading.Tasks;

internal sealed class TbaInitializationService(EventRepository eventsRepo, TeamRepository teamsRepo) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken) => Task.WhenAll(
            // Preload the events so Autocomplete is fast
            eventsRepo.InitializeAsync(cancellationToken).AsTask(),
            // Preload the teams so Autocomplete is fast
            teamsRepo.InitializeAsync(cancellationToken).AsTask()
        );

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
