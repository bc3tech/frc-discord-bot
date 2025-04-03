namespace FunctionApp.Storage.TableEntities;

using System.Runtime.Serialization;

internal sealed record EventSubscriptionEntity : BaseSubscriptionEntity
{
    [IgnoreDataMember]
    override public string Event { get => this.PartitionKey; set => this.PartitionKey = value; }

    [IgnoreDataMember]
    override public string Team { get => this.RowKey; set => this.RowKey = value; }
}

