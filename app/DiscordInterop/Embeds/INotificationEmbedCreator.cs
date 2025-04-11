namespace FunctionApp.DiscordInterop.Embeds;

using Discord;

using FunctionApp.TbaInterop.Models;

internal interface INotificationEmbedCreator : IEmbedCreator<WebhookMessage, SubscriptionEmbedding> { }

internal interface IEmbedCreator<TInput> : IEmbedCreator<TInput, ResponseEmbedding> { }

internal interface IEmbedCreator<TInput, TResponse> : IEmbedCreator where TResponse : ResponseEmbedding
{
    IAsyncEnumerable<TResponse?> CreateAsync(TInput input, ushort? highlightTeam = null, CancellationToken cancellationToken = default);
}

internal interface IEmbedCreator { }

internal record ResponseEmbedding(Embed Content, bool Transient = false, IEnumerable<IMessageComponent>? Actions = null)
{
    public static implicit operator Embed(ResponseEmbedding embedding) => embedding.Content;
}

internal sealed record SubscriptionEmbedding(Embed Content, IEnumerable<IMessageComponent>? Actions = null) : ResponseEmbedding(Content, false, Actions) { }