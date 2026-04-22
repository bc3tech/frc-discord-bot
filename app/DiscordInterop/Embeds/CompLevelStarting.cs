namespace FunctionApp.DiscordInterop.Embeds;

using Discord;

using FunctionApp.DiscordInterop;
using FunctionApp.TbaInterop.Models;
using FunctionApp.TbaInterop.Models.Notifications;

using Microsoft.Extensions.Logging;

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

using TheBlueAlliance.Extensions;
using TheBlueAlliance.Model;

internal sealed class CompLevelStarting(EmbedBuilderFactory builderFactory, TimeProvider time, ILogger<CompLevelStarting> logger) : INotificationEmbedCreator
{
    public const NotificationType TargetType = NotificationType.starting_comp_level;

    public async IAsyncEnumerable<SubscriptionEmbedding?> CreateAsync(WebhookMessage input, ushort? highlightTeam = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        TbaInterop.Models.Notifications.CompLevelStarting notification = input.GetDataAs<TbaInterop.Models.Notifications.CompLevelStarting>();
        if (notification == default)
        {
            logger.FailedToDeserializeNotificationDataAsNotificationType(TargetType);
            await Task.CompletedTask;
            yield return null;
            yield break;
        }

        CompLevel compLevel = Enum.Parse<CompLevel>(notification.comp_level, ignoreCase: true);
        var eventName = notification.event_name;
        EmbedBuilder embed = builderFactory.GetBuilder(highlightTeam)
            .WithDescription($"""
                # 📢⏰{compLevel.ToLongString()}⏰📢
                ## Starting soon for {eventName}
                ⌚Scheduled start time: {DateTimeOffset.FromUnixTimeSeconds(notification.scheduled_time ?? 0).ToLocalTime(time):t}
                """);

        yield return new(embed.Build());
    }
}
