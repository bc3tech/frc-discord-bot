namespace FunctionApp.Storage.TableEntities;

using Azure;
using Azure.Data.Tables;

using Common;

using System.Runtime.Serialization;
using System.Text.Json;

internal interface ISubscriptionEntity : ITableEntity
{
    GuildSubscriptions Subscribers { get; set; }
    string Team { get; set; }
    string Event { get; set; }
}

internal abstract record BaseSubscriptionEntity : ISubscriptionEntity
{
    [IgnoreDataMember]
    public GuildSubscriptions Subscribers { get; set; } = [];

    [Obsolete("Only used for serialization", error: true)]
    public string Subscriptions
    {
        get => JsonSerializer.Serialize(Subscribers);
        set => Subscribers = Throws.IfNull(JsonSerializer.Deserialize<GuildSubscriptions>(value));
    }

    [IgnoreDataMember]
    abstract public string Team { get; set; }
    [IgnoreDataMember]
    abstract public string Event { get; set; }

    required public string PartitionKey { get; set; }
    public string RowKey { get; set; } = CommonConstants.ALL;

    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
}