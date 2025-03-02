namespace DiscordBotFunctionApp.DiscordInterop.CommandModules;

using Discord;
using Discord.Interactions;

using DiscordBotFunctionApp.DiscordInterop.Embeds;

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

public abstract class CommandModuleBase : InteractionModuleBase
{
    internal virtual async Task GenerateResponseAsync<T>(IEmbedCreator<T> embeddingCreator, T input, Func<ImmutableArray<Embed>, Task>? modifyCallback = null, CancellationToken cancellationToken = default)
    {
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
                await this.ModifyOriginalResponseAsync(p => p.Embeds = discordEmbeds).ConfigureAwait(false);
            }
        }
    }
}