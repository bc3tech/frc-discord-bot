namespace DiscordBotFunctionApp.TbaInterop.Extensions;

using Microsoft.Extensions.Logging;

using TheBlueAlliance.Model;
using TheBlueAlliance.Model.WebcastExtensions;

public static class WebcastModelExtensions
{
    public static (string Name, Uri? Url) GetFullUrl(this Webcast w, ILogger? log = null)
    {
        var retVal = (w.Type.ToInvariantString(), w.Type switch
        {
            Webcast.TypeEnum.Youtube => new($"https://youtube.com/watch?v={w.Channel}"),
            Webcast.TypeEnum.Twitch => new($"https://twitch.tv/{w.Channel}"),
            Webcast.TypeEnum.Livestream => new($"https://vimeo.com/channels/{w.Channel}"),
            Webcast.TypeEnum.DirectLink => new(w.Channel),
            _ => (Uri?)null
        });

        if (retVal.Item2 is null)
        {
            log?.LogWarning("Webcast type {Type} not supported! (Channel value: {Channel})", retVal.Item1, retVal.Item2);
        }

        return retVal;
    }
}
