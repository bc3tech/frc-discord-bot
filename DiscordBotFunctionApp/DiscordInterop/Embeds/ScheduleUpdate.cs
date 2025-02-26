namespace DiscordBotFunctionApp.DiscordInterop.Embeds;
using DiscordBotFunctionApp.TbaInterop.Models;
using DiscordBotFunctionApp.TbaInterop.Models.Notifications;

using Microsoft.Extensions.Logging;

using System.Runtime.CompilerServices;
using System.Text.Json;

internal sealed class ScheduleUpdate(EmbedBuilderFactory builderFactory, ILogger<ScheduleUpdate> logger) : INotificationEmbedCreator
{
    public static NotificationType TargetType { get; } = NotificationType.schedule_updated;

    public async IAsyncEnumerable<SubscriptionEmbedding> CreateAsync(WebhookMessage msg, ushort? highlightTeam = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var baseBuilder = builderFactory.GetBuilder();
        var notification = JsonSerializer.Deserialize<TbaInterop.Models.Notifications.ScheduleUpdate>(msg.MessageData);
        if (notification is null)
        {
            logger.LogWarning("Failed to deserialize notification data as {NotificationType}", TargetType);
            yield break;
        }

        var embedding = baseBuilder
            .WithDescription(
$@"# Schedule Update
## {notification.event_name}
Next match start time: {DateTimeOffset.FromUnixTimeSeconds((long)notification.first_match_time!).ToPacificTime():t}

View the detailed event schedule [here](https://www.thebluealliance.com/event/{notification.event_key})")
            .Build();

        yield return await Task.FromResult<SubscriptionEmbedding>(new(embedding));
    }
}
