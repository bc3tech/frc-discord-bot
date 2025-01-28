namespace TheBlueAlliance.Api;

using System.Text.Json;
using System.Text.Json.Serialization;

using TheBlueAlliance.Api.Notifications;

public record WebhookMessage
{
    [JsonPropertyName("message_type")]
    public NotificationType MessageType { get; init; }

    [JsonPropertyName("message_data")]
    required public JsonElement MessageData { get; init; }

    public T? GetDataAs<T>() => JsonSerializer.Deserialize<T>(this.MessageData);
}