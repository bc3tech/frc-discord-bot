namespace Common.Tba.Notifications;

using Microsoft.Kiota.Abstractions.Serialization;

using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

public static class NotificationSerializer
{
    //public static T? Deserialize<T>(JsonElement data) where T : IWebhookNotification
    public static Task<T?> DeserializeAsync<T>(JsonElement data, CancellationToken cancellationToken) where T : IWebhookNotification => Task.FromResult(JsonSerializer.Deserialize<T>(data.ToString()));
    public static Task<T?> DeserializeWithManyAsync<T, TModel>(JsonElement data, CancellationToken cancellationToken) where T : IRequireCombinedSerializations<TModel>
        where TModel : IParsable => DoCombinedDeserializationAsync<T, TModel>(data, cancellationToken);

    public static Task<T?> DeserializeAsync<T, TModel>(JsonElement data, CancellationToken cancellationToken) where T : IRequireCombinedSerialization<TModel>
        where TModel : IParsable => DoCombinedDeserializationAsync<T, TModel>(data, cancellationToken);

    private static async Task<T?> DoCombinedDeserializationAsync<T, TModel>(JsonElement data, CancellationToken cancellationToken)
        where TModel : IParsable
    {
        var deserializedObject = JsonSerializer.Deserialize<T>(data.ToString());
        if (deserializedObject is not null)
        {
            var modelProperty = GetModelJsonPropertyName<T>();
            if (modelProperty is not null)
            {
                modelProperty!.Value.property.SetValue(deserializedObject, await KiotaJsonSerializer.DeserializeAsync<TModel>(data.GetProperty(modelProperty.Value.jsonName).ToString(), cancellationToken));
            }
        }

        return deserializedObject;
    }

    public static async Task<string?> SerializeAsync<TModel>(IRequireCombinedSerialization<TModel> notification, CancellationToken cancellationToken) where TModel : IParsable
    {
        var json = JsonSerializer.Serialize(notification);
        var o = JsonNode.Parse(json);
        Debug.Assert(o is not null);
        if (o is not null)
        {
            var modelProperty = GetModelJsonPropertyName<TModel>();
            Debug.Assert(modelProperty is not null);
            if (modelProperty is not null)
            {
                if (notification.Model is not null)
                {
                    o[modelProperty.Value.jsonName] = await KiotaJsonSerializer.SerializeAsStringAsync(notification.Model, serializeOnlyChangedValues: false, cancellationToken);
                }
                else
                {
                    o[modelProperty.Value.jsonName] = null;
                }
            }

            return o.ToJsonString();
        }

        return null;
    }

    private static (PropertyInfo property, string jsonName)? GetModelJsonPropertyName<T>()
    {
        var modelProperty = typeof(T).GetProperty("Model");
        var jsonPropertyNameAttribute = modelProperty?.GetCustomAttributes(typeof(JsonPropertyNameAttribute), inherit: false).FirstOrDefault() as JsonPropertyNameAttribute;
        if (modelProperty is not null && jsonPropertyNameAttribute is not null)
        {
            return (modelProperty, jsonPropertyNameAttribute!.Name);
        }

        return null;
    }
}
