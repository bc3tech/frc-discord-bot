namespace Common.Tba.Notifications;

using Microsoft.Kiota.Abstractions.Serialization;

using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

public static class NotificationSerializer
{
    public static Task<T?> DeserializeAsync<T>(JsonElement data, CancellationToken cancellationToken) where T : IWebhookNotification => Task.FromResult(JsonSerializer.Deserialize<T>(data.ToString()));

    public static Task<T?> DeserializeWithManyAsync<T, TModel>(JsonElement data, CancellationToken cancellationToken) where T : IRequireCombinedSerializations<TModel>
        where TModel : IParsable => DoCombinedDeserializationAsync<T, TModel>(data, cancellationToken, isCollection: true);

    public static Task<T?> DeserializeAsync<T, TModel>(JsonElement data, CancellationToken cancellationToken) where T : IRequireCombinedSerialization<TModel>
        where TModel : IParsable => DoCombinedDeserializationAsync<T, TModel>(data, cancellationToken);

    private static async Task<T?> DoCombinedDeserializationAsync<T, TModel>(JsonElement data, CancellationToken cancellationToken, bool isCollection = false)
        where TModel : IParsable
    {
        var deserializedObject = JsonSerializer.Deserialize<T>(data.ToString());
        if (deserializedObject is not null)
        {
            var modelProperty = GetModelJsonPropertyName<T>();
            if (modelProperty is not null)
            {
                if (isCollection)
                {
                    modelProperty!.Value.property.SetValue(deserializedObject,
                        await KiotaJsonSerializer.DeserializeCollectionAsync<TModel>(data.GetProperty(modelProperty.Value.jsonName).ToString(), cancellationToken).ConfigureAwait(false));
                }
                else
                {
                    modelProperty!.Value.property.SetValue(deserializedObject,
                        await KiotaJsonSerializer.DeserializeAsync<TModel>(data.GetProperty(modelProperty.Value.jsonName).ToString(), cancellationToken).ConfigureAwait(false));
                }
            }
        }

        return deserializedObject;
    }

    private static (PropertyInfo property, string jsonName)? GetModelJsonPropertyName<T>()
    {
        var modelProperty = typeof(T).GetProperty("Model");
        var jsonPropertyNameAttribute = modelProperty?.GetCustomAttributes(typeof(JsonPropertyNameAttribute), inherit: false).FirstOrDefault() as JsonPropertyNameAttribute;
        return modelProperty is not null && jsonPropertyNameAttribute is not null ? (modelProperty, jsonPropertyNameAttribute!.Name) : null;
    }
}
