namespace FunctionApp.Storage.TableEntities;

using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

internal sealed record TeamSubscriptionEntity : BaseSubscriptionEntity
{
    [IgnoreDataMember]
    [NotNull]
    override public string Team { get => this.PartitionKey; set => this.PartitionKey = value; }
    [IgnoreDataMember]
    override public string Event { get => this.RowKey; set => this.RowKey = value; }
}
