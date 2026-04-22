namespace FunctionApp.DiscordInterop.CommandModules;

using Common.Discord;
using Common.Extensions;

using Discord;
using Discord.Interactions;
using Discord.Net;

using FunctionApp.DiscordInterop.Embeds;

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
        using IDisposable scope = Logger.CreateMethodScope();
        ResponseEmbedding[] embeds = [];
        ushort numEmbeddingsCreated = 0, erroredEmbeddings = 0;
        await foreach (ResponseEmbedding? m in embeddingCreator.CreateAsync(input, highlightTeam, cancellationToken: cancellationToken).ConfigureAwait(false))
        {
            if (m is null)
            {
                continue;
            }

            cancellationToken.ThrowIfCancellationRequested();
            try
            {

                embeds = [.. embeds, m];
                Embed[] discordEmbeds = [.. embeds.Select(i => i.Content)];
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
                    await UpdateOriginalResponseAsync(p => p.Embeds = discordEmbeds, cancellationToken).ConfigureAwait(false);
                }

                numEmbeddingsCreated++;
            }
            catch (Exception ex) when (ex is not TaskCanceledException and not OperationCanceledException)
            {
                Logger.ErrorDuringResponseGeneation(ex);
                erroredEmbeddings++;
            }
        }

        cancellationToken.ThrowIfCancellationRequested();
        if (numEmbeddingsCreated is 0 || erroredEmbeddings is not 0)
        {
            await UpdateOriginalResponseAsync(
                p => p.Content = "I encountered one/more errors processing your request. You can try aga, or contact your admin with this news so they can troubleshoot.",
                cancellationToken)
                .ConfigureAwait(false);
        }
    }

    protected virtual async Task<IDisposable?> TryDeferAsync(bool ephemeral = false, CancellationToken cancellationToken = default)
    {
        try
        {
            await DeferAsync(ephemeral: ephemeral, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
            return Context.Channel.EnterTypingState();
        }
        catch (HttpException e) when (e.DiscordCode is DiscordErrorCode.UnknownInteraction or DiscordErrorCode.InteractionHasAlreadyBeenAcknowledged)
        {
            Logger.InteractionAlreadyAcknowledgedSkippingResponse();
            return null;
        }
    }

    protected virtual Task UpdateOriginalResponseAsync(Action<MessageProperties> updateMessage, CancellationToken cancellationToken = default)
        => ModifyOriginalResponseAsync(updateMessage, options: cancellationToken.ToRequestOptions());

    protected virtual Task SendResponseAsync(string responseContent, bool ephemeral = false, CancellationToken cancellationToken = default)
        => RespondAsync(responseContent, ephemeral: ephemeral, options: cancellationToken.ToRequestOptions());

    protected virtual Task DeleteResponseAsync(CancellationToken cancellationToken = default)
        => DeleteOriginalResponseAsync();

    protected virtual Task SendMessageAsync(IMessageChannel channel, string message, CancellationToken cancellationToken = default)
        => channel.SendMessageAsync(message, options: cancellationToken.ToRequestOptions());
}
