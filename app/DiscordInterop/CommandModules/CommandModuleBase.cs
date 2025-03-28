namespace FunctionApp.DiscordInterop.CommandModules;

using Common.Extensions;

using Discord;
using Discord.Interactions;
using Discord.Net;

using FunctionApp.DiscordInterop.Embeds;
using FunctionApp.Extensions;

using Microsoft.Extensions.Logging;

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

public abstract class CommandModuleBase(ILogger logger) : InteractionModuleBase
{
    protected ILogger Logger { get; } = logger;

    internal virtual async Task GenerateResponseAsync<T>(IEmbedCreator<T> embeddingCreator, T input, ushort? highlightTeam = null, Func<ImmutableArray<Embed>, Task>? modifyCallback = null, CancellationToken cancellationToken = default)
    {
        using var scope = this.Logger.CreateMethodScope();
        ResponseEmbedding[] embeds = [];
        ushort numEmbeddingsCreated = 0, erroredEmbeddings = 0;
        await foreach (var m in embeddingCreator.CreateAsync(input, highlightTeam, cancellationToken: cancellationToken).ConfigureAwait(false))
        {
            if (m is null)
            {
                continue;
            }

            cancellationToken.ThrowIfCancellationRequested();
            try
            {
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
                    await ModifyOriginalResponseAsync(p => p.Embeds = discordEmbeds, options: cancellationToken.ToRequestOptions()).ConfigureAwait(false);
                }

                numEmbeddingsCreated++;
            }
            catch (Exception ex) when (ex is not TaskCanceledException and not OperationCanceledException)
            {
                this.Logger.ErrorDuringResponseGeneation(ex);
                erroredEmbeddings++;
            }
        }

        cancellationToken.ThrowIfCancellationRequested();
        if (numEmbeddingsCreated is 0 || erroredEmbeddings is not 0)
        {
            await ModifyOriginalResponseAsync(p => p.Content = "I encountered one/more errors processing your request. You can try again, or contact your admin with this news so they can troubleshoot.").ConfigureAwait(false);
        }
    }

    protected async Task<IDisposable?> TryDeferAsync(bool ephemeral = false, CancellationToken cancellationToken = default)
    {
        try
        {
            await DeferAsync(ephemeral: ephemeral, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
            return this.Context.Channel.EnterTypingState();
        }
        catch (HttpException e) when (e.DiscordCode is DiscordErrorCode.UnknownInteraction or DiscordErrorCode.InteractionHasAlreadyBeenAcknowledged)
        {
            this.Logger.InteractionAlreadyAcknowledgedSkippingResponse();
            return null;
        }
    }
}