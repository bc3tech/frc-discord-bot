namespace DiscordBotFunctionApp.Storage.TableEntities;
internal interface ISubscriptionEntity
{
    GuildSubscriptions Subscribers { get; set; }
}