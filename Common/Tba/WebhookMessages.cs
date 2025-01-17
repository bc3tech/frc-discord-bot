namespace Common.Tba;

using Common.Tba.Notifications;

using Microsoft.Kiota.Abstractions.Serialization;

using System.Text.Json;
using System.Text.Json.Serialization;

public record WebhookMessage 
{
    [JsonPropertyName("message_type")]
    public NotificationType MessageType { get; init; }

    [JsonPropertyName("message_data")]
    required public JsonElement MessageData { get; init; }

    public Task<TNotification?> GetDataAsAsync<TNotification, TModel>(CancellationToken cancellationToken = default) where TNotification : IRequireCombinedSerialization<TModel>
        where TModel : IParsable => NotificationSerializer.DeserializeAsync<TNotification, TModel>(MessageData, cancellationToken);

    public Task<TNotification?> GetManyDataAsAsync<TNotification, TModel>(CancellationToken cancellationToken = default) where TNotification : IRequireCombinedSerializations<TModel>
        where TModel : IParsable => NotificationSerializer.DeserializeWithManyAsync<TNotification, TModel>(MessageData, cancellationToken);

    public Task<TNotification?> GetDataAsAsync<TNotification>(CancellationToken cancellationToken = default)
        where TNotification : IWebhookNotification => NotificationSerializer.DeserializeAsync<TNotification>(MessageData, cancellationToken);
}