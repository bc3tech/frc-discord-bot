namespace Common.Tba.Notifications;

using Microsoft.Kiota.Abstractions.Serialization;

public interface IWebhookNotification { }

public interface IRequireCombinedSerialization<TModel> : IWebhookNotification where TModel : IParsable
{
    TModel? Model { get; }
}

public interface IRequireCombinedSerializations<TModel> : IWebhookNotification where TModel : IParsable
{
    IEnumerable<TModel>? Model { get; }
}