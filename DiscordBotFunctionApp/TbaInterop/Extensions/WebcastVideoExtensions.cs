namespace DiscordBotFunctionApp.TbaInterop.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TheBlueAlliance.Model;

public static class WebcastVideoExtensions
{
    public static string CreateUrlForVideoObject(this (Webcast.TypeEnum Type, string Key) objDetail)
    {
        return objDetail.Type switch
        {
            Webcast.TypeEnum.Youtube => $"https://youtube.com/watch?v={objDetail.Key}",
            Webcast.TypeEnum.Twitch => $"https://twitch.tv/{objDetail.Key}",
            Webcast.TypeEnum.Livestream => $"https://vimeo.com/channels/{objDetail.Key}",
            Webcast.TypeEnum.DirectLink => objDetail.Key,
            _ => string.Empty
        };
    }
}
