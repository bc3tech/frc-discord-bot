namespace DiscordBotFunctionApp.DiscordInterop.CommandModules;

using Discord;
using Discord.Interactions;
using Discord.Net;

using DiscordBotFunctionApp.DiscordInterop.Embeds;
using DiscordBotFunctionApp.Extensions;

using Microsoft.Extensions.Logging;

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

public abstract class CommandModuleBase(ILogger logger) : InteractionModuleBase
{
    protected ILogger Logger { get; } = logger;

    internal virtual async Task GenerateResponseAsync<T>(IEmbedCreator<T> embeddingCreator, T input, Func<ImmutableArray<Embed>, Task>? modifyCallback = null, CancellationToken cancellationToken = default)
    {
        using var typing = this.Context.Channel.EnterTypingState();
        ResponseEmbedding[] embeds = [];
        await foreach (var m in embeddingCreator.CreateAsync(input, cancellationToken: cancellationToken).ConfigureAwait(false))
        {
            if (m is null)
            {
                continue;
            }

            cancellationToken.ThrowIfCancellationRequested();

            embeds = [.. embeds, m];
            var discordEmbeds = embeds.Select(i => i.Content).ToArray();
            if (!m.Transient)
            {
                discordEmbeds = [.. embeds.Where(i => !i.Transient).Select(i => i.Content)];
            }

            if (modifyCallback is not null)
            {
                await modifyCallback([.. discordEmbeds]).ConfigureAwait(false);
            }
            else
            {
                await this.ModifyOriginalResponseAsync(p => p.Embeds = discordEmbeds, options: cancellationToken.ToRequestOptions()).ConfigureAwait(false);
            }
        }
    }

    protected async Task<IDisposable?> TryDeferAsync(bool ephemeral = false, CancellationToken cancellationToken = default)
    {
        try
        {
            await this.DeferAsync(ephemeral: ephemeral, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
            return this.Context.Channel.EnterTypingState();
        }
        catch (HttpException e) when (e.DiscordCode is DiscordErrorCode.UnknownInteraction or DiscordErrorCode.InteractionHasAlreadyBeenAcknowledged)
        {
            this.Logger.InteractionAlreadyAcknowledgedSkippingResponse();
            return null;
        }
    }
}