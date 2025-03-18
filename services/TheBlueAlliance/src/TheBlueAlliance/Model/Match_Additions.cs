namespace TheBlueAlliance.Model;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;

using TheBlueAlliance.Model.MatchExtensions;

public partial record Match
{
    public int? GetAllianceRankingPoints(WinningAllianceEnum allianceColor)
    {
        var alliance = this.ScoreBreakdown?.ActualInstance!.GetType().GetProperty(allianceColor.ToInvariantString())!.GetValue(this.ScoreBreakdown.ActualInstance);
        var rpValue = (int?)alliance?.GetType().GetProperty("Rp")?.GetValue(alliance);
        return CorrectRpValue(rpValue);
    }

    private static int? CorrectRpValue(int? rpValue) => rpValue is null or > 6 or < 0 ? null : rpValue;

    public IEnumerable<(string Name, Uri Link)> GetVideoUrls(ILogger? log = null)
    {
        foreach (var v in this.Videos ?? [])
        {
            if (v.Type is "youtube")
            {
                yield return ("YouTube", new($"https://youtube.com/watch?v={v.Key}"));
            }
            else if (v.Type is "tba")
            {
                yield return ("Blue Alliance", new($"https://twitch.tv/{v.Key}"));
            }
            else
            {
                log?.UnknownVideoTypeTypeForMatchMatchKey(v.Type, this.Key);
            }
        }
    }
}
