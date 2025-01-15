namespace DiscordBotFunctionApp.Embeds;

using Common.Tba;

using Discord;

using System;

using Tba = Common.Tba.Notifications;

internal static class EmbeddingGenerator
{
    public static Embed CreateEmbedding(WebhookMessage tbaWebhookMessage, uint? highlightTeam = null)
    {
        return tbaWebhookMessage.MessageType switch
        {
            Tba.NotificationType.match_score => MatchScore.Create(tbaWebhookMessage.GetDataAs<Tba.MatchScore>()!, highlightTeam),
            _ => throw new ArgumentException($"Message type not convertible to Discord embedding: {tbaWebhookMessage.MessageType}")
        };
    }
}
