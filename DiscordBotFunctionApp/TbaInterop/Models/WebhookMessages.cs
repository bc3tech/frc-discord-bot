namespace DiscordBotFunctionApp.TbaInterop.Models;

using DiscordBotFunctionApp.TbaInterop.Models.Notifications;

using System.Text.Json;
using System.Text.Json.Serialization;

internal sealed record WebhookMessage
{
    [JsonPropertyName("message_type")]
    public NotificationType MessageType { get; init; }

    [JsonPropertyName("message_data")]
    required public JsonElement MessageData { get; init; }

    public T? GetDataAs<T>() => MessageData.Deserialize<T>();

    public bool ThreadReplies() => 
        MessageType is NotificationType.upcoming_match 
        or NotificationType.event_match_video 
        or NotificationType.match_score 
        or NotificationType.match_video;

    public (string PartitionKey, string RowKey)? GetThreadLocator()
    {
        ThreadedEntity? threadedEntity;
        switch (this.MessageType)
        {
            case NotificationType.match_video:
            case NotificationType.event_match_video:
                threadedEntity = GetDataAs<MatchVideo>();
                break;
            case NotificationType.match_score:
                threadedEntity = GetDataAs<MatchScore>();
                break;
            case NotificationType.upcoming_match:
                threadedEntity = GetDataAs<UpcomingMatch>();
                break;
            default:
                return null;
        }

        return threadedEntity is null ? null : (threadedEntity.PartitionKey, threadedEntity.RowKey);
    }
}