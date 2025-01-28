namespace TheBlueAlliance.Api.Extensions;
using TheBlueAlliance.Model;
using TheBlueAlliance.Model.MatchExtensions;

public static class MatchModelExtensions
{
    public static int GetAllianceRankingPoints(this Match match, Match.WinningAllianceEnum allianceColor)
    {
        var alliance = match.ScoreBreakdown.ActualInstance!.GetType().GetProperty(allianceColor.ToInvariantString())!.GetValue(match.ScoreBreakdown.ActualInstance)!;
        var rpValue = (int)alliance.GetType().GetProperty("Rp")!.GetValue(alliance)!;
        return rpValue;
    }
}
