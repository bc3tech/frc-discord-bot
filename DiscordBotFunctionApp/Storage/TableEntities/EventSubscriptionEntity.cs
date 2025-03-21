namespace DiscordBotFunctionApp.Storage.TableEntities;

using Azure;

using Common;

using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text.Json;

internal sealed record EventSubscriptionEntity : ISubscriptionEntity
{
    [IgnoreDataMember]
    public string Event { get => PartitionKey; set => PartitionKey = value; }

    [IgnoreDataMember]
    public ushort? Team
    {
        get => ushort.TryParse(RowKey, CultureInfo.InvariantCulture, out var t) ? t : null;
        set => RowKey = value.HasValue ? value.Value.ToString(CultureInfo.InvariantCulture) : CommonConstants.ALL;
    }

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

