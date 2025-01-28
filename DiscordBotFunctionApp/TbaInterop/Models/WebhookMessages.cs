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
}