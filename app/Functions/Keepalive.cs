namespace FunctionApp.Functions;

using FunctionApp;

using FunctionApp.DiscordInterop;

using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

using System;

internal sealed class Keepalive(TimeProvider time, ILogger<Keepalive> logger)
{
    [Function("Heartbeat")]
    public void Run([TimerTrigger("0 */4 * * * *"
#if DEBUG
    , RunOnStartup = true
#endif
        )] TimerInfo myTimer)
    {
        logger.KeepaliveArg0(time.GetLocalNow());

        if (myTimer.ScheduleStatus is not null)
        {
            logger.NextTimerScheduleAtArg0(new DateTimeOffset(myTimer.ScheduleStatus.Next).ToLocalTime(time).DateTime);
        }
    }
}
