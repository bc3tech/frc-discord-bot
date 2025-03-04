namespace DiscordBotFunctionApp.TbaInterop.Extensions;

using TheBlueAlliance.Model;
using TheBlueAlliance.Model.WebcastExtensions;

public static class WebcastModelExtensions
{
    public static (string Name, string Url) GetFullUrl(this Webcast w)
    {
        return (w.Type.ToInvariantString(), w.Type switch
        {
            Webcast.TypeEnum.Youtube => $"https://youtube.com/watch?v={w.Channel}",
            Webcast.TypeEnum.Twitch => $"https://twitch.tv/{w.Channel}",
            Webcast.TypeEnum.Livestream => $"https://vimeo.com/channels/{w.Channel}",
            Webcast.TypeEnum.DirectLink => w.Channel,
            _ => string.Empty
        });
    }
}
