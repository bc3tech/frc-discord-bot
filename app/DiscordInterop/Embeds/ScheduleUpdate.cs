namespace FunctionApp.DiscordInterop.Embeds;

using Discord;

using FunctionApp;

using FunctionApp.TbaInterop.Models;
using FunctionApp.TbaInterop.Models.Notifications;

using Microsoft.Extensions.Logging;

using System.Runtime.CompilerServices;
using System.Text.Json;

internal sealed class ScheduleUpdate(EmbedBuilderFactory builderFactory, ILogger<ScheduleUpdate> logger) : INotificationEmbedCreator
{
    public const NotificationType TargetType = NotificationType.schedule_updated;

    public async IAsyncEnumerable<SubscriptionEmbedding?> CreateAsync(WebhookMessage msg, ushort? highlightTeam = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        EmbedBuilder baseBuilder = builderFactory.GetBuilder();
        TbaInterop.Models.Notifications.ScheduleUpdate notification = msg.GetDataAs<TbaInterop.Models.Notifications.ScheduleUpdate>();
        if (notification == default)
        {
            logger.FailedToDeserializeNotificationDataAsNotificationType(TargetType);
            yield return null;
            yield break;
        }

        if (!TryResolveEventDetails(msg.MessageData, notification, out var eventKey, out var eventName))
        {
            logger.EventKeyIsMissingFromNotificationData();
            yield return null;
            yield break;
        }

        EmbedBuilder embedding = baseBuilder
            .WithTitle($"📢{eventName} Schedule Update⏰")
            .WithUrl($"https://www.thebluealliance.com/event/{eventKey}")
            .WithDescription("Click for details");

        yield return await Task.FromResult<SubscriptionEmbedding>(new(embedding.Build()));
    }

    internal static bool TryResolveEventDetails(JsonElement messageData, TbaInterop.Models.Notifications.ScheduleUpdate notification, out string eventKey, out string eventName)
    {
        eventKey = notification.event_key;
        eventName = notification.event_name;

        if (messageData.ValueKind is JsonValueKind.Object
            && messageData.TryGetProperty("event", out JsonElement eventData)
            && eventData.ValueKind is JsonValueKind.Object)
        {
            if (string.IsNullOrWhiteSpace(eventKey)
                && eventData.TryGetProperty("key", out JsonElement keyElement)
                && keyElement.GetString() is { } key
                && !string.IsNullOrWhiteSpace(key))
            {
                eventKey = key;
            }

            if (string.IsNullOrWhiteSpace(eventName)
                && eventData.TryGetProperty("name", out JsonElement nameElement)
                && nameElement.GetString() is { } name
                && !string.IsNullOrWhiteSpace(name))
            {
                eventName = name;
            }

            if (string.IsNullOrWhiteSpace(eventName)
                && eventData.TryGetProperty("short_name", out JsonElement shortNameElement)
                && shortNameElement.GetString() is { } shortName
                && !string.IsNullOrWhiteSpace(shortName))
            {
                eventName = shortName;
            }
        }

        return !string.IsNullOrWhiteSpace(eventKey) && !string.IsNullOrWhiteSpace(eventName);
    }
}
