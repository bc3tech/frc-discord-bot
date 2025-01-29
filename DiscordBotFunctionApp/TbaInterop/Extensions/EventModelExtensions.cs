namespace DiscordBotFunctionApp.TbaInterop.Extensions;

using TheBlueAlliance.Model;
using TheBlueAlliance.Model.WebcastExtensions;

internal static class EventModelExtensions
{
    public static IEnumerable<(string Name, string Url)> GetWebcastFullUrls(this Event eventModel)
    {
        if (eventModel.Webcasts is null)
        {
            yield break;
        }

        foreach (var w in eventModel.Webcasts)
        {
            yield return (w.Type.ToInvariantString(), w.Type switch
            {
                Webcast.TypeEnum.Youtube => $"https://youtube.com/watch?v={w.Channel}",
                Webcast.TypeEnum.Twitch => $"https://twitch.tv/{w.Channel}",
                Webcast.TypeEnum.Livestream => $"https://vimeo.com/channels/{w.Channel}",
                Webcast.TypeEnum.DirectLink => w.Channel,
                _ => string.Empty
            });
        }
    }
}
