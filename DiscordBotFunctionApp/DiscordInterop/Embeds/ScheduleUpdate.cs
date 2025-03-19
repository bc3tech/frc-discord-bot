namespace DiscordBotFunctionApp.DiscordInterop.Embeds;
using DiscordBotFunctionApp.TbaInterop.Models;
using DiscordBotFunctionApp.TbaInterop.Models.Notifications;

using Microsoft.Extensions.Logging;

using System.Runtime.CompilerServices;

internal sealed class ScheduleUpdate(EmbedBuilderFactory builderFactory, ILogger<ScheduleUpdate> logger) : INotificationEmbedCreator
{
    public const NotificationType TargetType = NotificationType.schedule_updated;

    public async IAsyncEnumerable<SubscriptionEmbedding?> CreateAsync(WebhookMessage msg, ushort? highlightTeam = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var baseBuilder = builderFactory.GetBuilder();
        var notification = msg.GetDataAs<TbaInterop.Models.Notifications.ScheduleUpdate>();
        if (notification == default)
        {
            logger.FailedToDeserializeNotificationDataAsNotificationType(TargetType);
            yield break;
        }

        var embedding = baseBuilder
            .WithDescription(
                $"""
                # Schedule Update

                There's been a schedule update to **{notification.event_name}**

                View the detailed schedule [here](https://www.thebluealliance.com/event/{notification.event_key})
                """);

        yield return await Task.FromResult<SubscriptionEmbedding>(new(embedding.Build()));
    }
}
