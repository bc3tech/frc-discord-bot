namespace DiscordBotFunctionApp.Storage.TableEntities;
using Azure.Data.Tables;

internal interface ISubscriptionEntity : ITableEntity
{
    GuildSubscriptions Subscribers { get; set; }
    ushort? Team { get; set; }
    string Event { get; set; }
}