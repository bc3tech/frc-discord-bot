namespace DiscordBotFunctionApp.DiscordInterop.Embeds;

using DiscordBotFunctionApp.TbaInterop;
using DiscordBotFunctionApp.TbaInterop.Models;
using DiscordBotFunctionApp.TbaInterop.Models.Notifications;

using Microsoft.Extensions.Logging;

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

internal sealed class CompLevelStarting(EmbedBuilderFactory builderFactory, TimeProvider time, ILogger<CompLevelStarting> logger) : INotificationEmbedCreator
{
    public const NotificationType TargetType = NotificationType.starting_comp_level;

    public async IAsyncEnumerable<SubscriptionEmbedding?> CreateAsync(WebhookMessage input, ushort? highlightTeam = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var notification = input.GetDataAs<TbaInterop.Models.Notifications.CompLevelStarting>();
        if (notification == default)
        {
            logger.FailedToDeserializeNotificationDataAsNotificationType(TargetType);
            await Task.CompletedTask;
            yield return null;
            yield break;
        }

        var compLevel = notification.comp_level;
        var eventName = notification.event_name;
        var embed = builderFactory.GetBuilder(highlightTeam)
            .WithDescription($"""
                # {Translator.CompLevelToLongString(compLevel)} 
                ## Starting soon for {eventName}
                Scheduled start time: {DateTimeOffset.FromUnixTimeSeconds(notification.scheduled_time ?? 0).ToLocalTime(time):t}
                """);

        yield return new(embed.Build());
    }
}
