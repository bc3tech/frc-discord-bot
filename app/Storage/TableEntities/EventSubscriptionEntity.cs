namespace DiscordBotFunctionApp.Storage.TableEntities;
using System.Runtime.Serialization;

internal sealed record EventSubscriptionEntity : BaseSubscriptionEntity
{
    [IgnoreDataMember]
    override public string Event { get => PartitionKey; set => PartitionKey = value; }

    [IgnoreDataMember]
    override public string Team { get => RowKey; set => RowKey = value; }
}

