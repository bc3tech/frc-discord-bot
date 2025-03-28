namespace TheBlueAlliance.Model;
using Microsoft.Extensions.Logging;

using System;

using TheBlueAlliance.Model.WebcastExtensions;

public partial record Webcast
{
    public (string Name, Uri? Url) GetFullUrl(ILogger? log = null)
    {
        var retVal = (this.Type.ToInvariantString(), this.Type switch
        {
            TypeEnum.Youtube => new($"https://youtube.com/watch?v={this.Channel}"),
            TypeEnum.Twitch => new($"https://twitch.tv/{this.Channel}"),
            TypeEnum.Livestream => new($"https://vimeo.com/channels/{this.Channel}"),
            TypeEnum.DirectLink => new(this.Channel),
            _ => (Uri?)null
        });

        if (retVal.Item2 is null)
        {
            log?.WebcastTypeTypeNotSupportedChannelValueChannel(retVal.Item1, retVal.Item2);
        }

        return retVal;
    }
}
