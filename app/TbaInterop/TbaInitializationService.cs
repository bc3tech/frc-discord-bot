namespace FunctionApp.TbaInterop;

using Microsoft.Extensions.Hosting;

using System.Threading;
using System.Threading.Tasks;

using TheBlueAlliance.Interfaces.Caching;

internal sealed class TbaInitializationService(IEventCache eventsRepo, ITeamCache teamsRepo) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken) => Task.WhenAll(
            // Preload the events so Autocomplete is fast
            eventsRepo.InitializeAsync(cancellationToken).AsTask(),
            // Preload the teams so Autocomplete is fast
            teamsRepo.InitializeAsync(cancellationToken).AsTask()
        );

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
