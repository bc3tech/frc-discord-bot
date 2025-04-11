namespace FunctionApp.DiscordInterop.Embeds;

using Common.Extensions;

using FunctionApp;

using FunctionApp.DiscordInterop;
using FunctionApp.Extensions;

using Microsoft.Extensions.Logging;

using System.Globalization;
using System.Threading.Tasks;

internal sealed class EmbeddingColorizer(FRCColors.Client colorClient, ILogger<EmbeddingColorizer>? logger)
{
    public Task<bool> SetEmbeddingColorAsync(string teamKey, Discord.EmbedBuilder embedding, CancellationToken cancellationToken) => SetEmbeddingColorAsync(teamKey.TeamKeyToTeamNumber(), embedding, cancellationToken);

    public async Task<bool> SetEmbeddingColorAsync(ushort? forTeam, Discord.EmbedBuilder embedding, CancellationToken cancellationToken)
    {
        if (forTeam.HasValue)
        {
            try
            {
                var (primaryColor, secondaryColor) = await colorClient.GetColorsForTeamAsync(forTeam.Value, cancellationToken).ConfigureAwait(false);
                (bool flowControl, bool value) = SetEmbeddingColor(embedding, primaryColor, secondaryColor);
                if (!flowControl)
                {
                    return value;
                }
            }
            catch (Exception ex)
            {
                logger?.ErrorWhenTryingToFetchColorForTeamNumber(ex, forTeam.Value);
            }
        }

        return false;
    }

    public bool SetEmbeddingColor(string teamKey, Discord.EmbedBuilder embedding, CancellationToken cancellationToken = default) => SetEmbeddingColor(teamKey.TeamKeyToTeamNumber(), embedding, cancellationToken);

    public bool SetEmbeddingColor(ushort? forTeam, Discord.EmbedBuilder embedding, CancellationToken cancellationToken = default)
    {
        if (forTeam.HasValue)
        {
            try
            {
                var (primaryColor, secondaryColor) = colorClient.GetColorsForTeam(forTeam.Value, cancellationToken);
                (bool flowControl, bool value) = SetEmbeddingColor(embedding, primaryColor, secondaryColor);
                if (!flowControl)
                {
                    return value;
                }
            }
            catch (Exception ex)
            {
                logger?.ErrorWhenTryingToFetchColorForTeamNumber(ex, forTeam.Value);
            }
        }

        return false;
    }

    private static (bool flowControl, bool value) SetEmbeddingColor(Discord.EmbedBuilder embedding, System.Drawing.Color? primaryColor, System.Drawing.Color? secondaryColor)
    {
        var themeColor = Utility.GetLightestColorOf([primaryColor?.ToDiscordColor(), secondaryColor?.ToDiscordColor()]);
        if (themeColor.HasValue)
        {
            embedding.WithColor(themeColor.Value);
            return (flowControl: false, value: true);
        }

        return (flowControl: true, value: default);
    }
}
