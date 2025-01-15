namespace DiscordBotFunctionApp.TableEntities;

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
    public int Team { get => int.Parse(this.PartitionKey, CultureInfo.InvariantCulture); set => this.PartitionKey = value.ToString(CultureInfo.InvariantCulture); }
    [IgnoreDataMember]
    public string Event { get => this.RowKey; set => this.RowKey = value; }

    [IgnoreDataMember]
    public GuildSubscriptions Subscribers { get; set; } = [];

    [Obsolete("Only used for serialization", error: true)]
    public string Subscriptions
    {
        get => JsonSerializer.Serialize(this.Subscribers);
        set => this.Subscribers = Throws.IfNull(JsonSerializer.Deserialize<GuildSubscriptions>(value));
    }

    required public string PartitionKey { get; set; }
    public string RowKey { get; set; } = CommonConstants.ALL;
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
}
