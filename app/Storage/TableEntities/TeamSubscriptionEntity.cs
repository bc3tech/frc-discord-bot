namespace FunctionApp.Storage.TableEntities;

using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

internal sealed record TeamSubscriptionEntity : BaseSubscriptionEntity
{
    [IgnoreDataMember]
    [NotNull]
    override public string Team { get => PartitionKey; set => PartitionKey = value; }
    [IgnoreDataMember]
    override public string Event { get => RowKey; set => RowKey = value; }
}
