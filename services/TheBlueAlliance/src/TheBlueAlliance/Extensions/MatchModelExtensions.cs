namespace DiscordBotFunctionApp.TbaInterop.Extensions;

using Microsoft.Extensions.Logging;

using TheBlueAlliance.Model;
using TheBlueAlliance.Model.MatchExtensions;

public static class MatchModelExtensions
{
    public static int? GetAllianceRankingPoints(this Match match, Match.WinningAllianceEnum allianceColor)
    {
        var alliance = match.ScoreBreakdown?.ActualInstance!.GetType().GetProperty(allianceColor.ToInvariantString())!.GetValue(match.ScoreBreakdown.ActualInstance);
        var rpValue = (int?)alliance?.GetType().GetProperty("Rp")?.GetValue(alliance);
        return rpValue;
    }

    public static IEnumerable<(string Name, Uri Link)> GetVideoUrls(this Match match, ILogger? log = null)
    {
        foreach (var v in match.Videos ?? [])
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
                log?.LogWarning("Unknown video type {Type} for match {MatchKey}", v.Type, match.Key);
            }
        }
    }
}
