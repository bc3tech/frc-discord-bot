namespace FunctionApp.DiscordInterop.Embeds;

using FunctionApp;

using FunctionApp.TbaInterop.Models;
using FunctionApp.TbaInterop.Models.Notifications;

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
            yield return null;
            yield break;
        }

        var embedding = baseBuilder
            .WithTitle($"📢{notification.event_name} Schedule Update⏰")
            .WithUrl($"https://www.thebluealliance.com/event/{notification.event_key}")
            .WithDescription("Click for details");

        yield return await Task.FromResult<SubscriptionEmbedding>(new(embedding.Build()));
    }
}
