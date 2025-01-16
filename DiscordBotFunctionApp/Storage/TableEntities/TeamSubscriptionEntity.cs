namespace DiscordBotFunctionApp.Storage.TableEntities;

using Azure;
using Azure.Data.Tables;

using Common;

using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text.Json;

internal record TeamSubscriptionEntity : ITableEntity, ISubscriptionEntity
{
    [IgnoreDataMember]
    public int Team { get => int.Parse(PartitionKey, CultureInfo.InvariantCulture); set => PartitionKey = value.ToString(CultureInfo.InvariantCulture); }
    [IgnoreDataMember]
    public string Event { get => RowKey; set => RowKey = value; }

    [IgnoreDataMember]
    public GuildSubscriptions Subscribers { get; set; } = [];

    [Obsolete("Only used for serialization", error: true)]
    public string Subscriptions
    {
        get => JsonSerializer.Serialize(Subscribers);
        set => Subscribers = Throws.IfNull(JsonSerializer.Deserialize<GuildSubscriptions>(value));
    }

    required public string PartitionKey { get; set; }
    public string RowKey { get; set; } = CommonConstants.ALL;
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
}
