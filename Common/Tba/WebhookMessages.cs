namespace Common.Tba;

using Common.Tba.Notifications;

using System.Text.Json;
using System.Text.Json.Serialization;

public interface IHasMessageData<T>
{
    T MessageData { get; }
}

public record WebhookMessage : IHasMessageData<JsonElement>
{
    [JsonPropertyName("message_type")]
    public NotificationType MessageType { get; init; }

    [JsonPropertyName("message_data")]
    required public JsonElement MessageData { get; init; }

    public T? GetDataAs<T>() => JsonSerializer.Deserialize<T>(MessageData);
}