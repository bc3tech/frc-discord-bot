namespace DiscordBotFunctionApp;

using System;

using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

internal sealed class Keepalive(ILogger<Keepalive> logger)
{
    [Function("Heartbeat")]
    public void Run([TimerTrigger("0 */4 * * * *"
#if DEBUG
    , RunOnStartup = true
#endif
        )] TimerInfo myTimer)
    {
        logger.LogInformation($"Keepalive ({TimeProvider.System.GetLocalNow():g})");

        if (myTimer.ScheduleStatus is not null)
        {
            logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next.ToLocalTime():g}");
        }
    }
}
