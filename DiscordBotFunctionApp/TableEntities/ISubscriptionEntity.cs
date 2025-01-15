namespace DiscordBotFunctionApp.TableEntities;

internal interface ISubscriptionEntity
{
    GuildSubscriptions Subscribers { get; set; }
}