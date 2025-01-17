
using Common.Tba.Api.Models;

using Microsoft.Extensions.EnumStrings;

[assembly: EnumStrings(typeof(Match_winning_alliance), ExtensionClassModifiers = "public static")]

namespace Common.Tba.Extensions;
using Common.Tba.Api.Models;

using System;

public static class MatchModelExtensions
{
    public static int GetAllianceRankingPoints(this Match match, Match_winning_alliance allianceColor)
    {
        var matchYear = ushort.Parse(match!.Key![..4]);

        var rpProperty = Type.GetType($"Common.Tba.Api.Models.Match_Score_Breakdown_{matchYear}_Alliance")!.GetProperty("Rp")!;
        var breakdownProperty = typeof(Match.Match_score_breakdown).GetProperty($"MatchScoreBreakdown{matchYear}")!;

        var breakdown = breakdownProperty.GetValue(match.ScoreBreakdown)!;
        var alliance = breakdown.GetType().GetProperty(allianceColor.ToInvariantString())!.GetValue(breakdown);
        var rpValue = (int)rpProperty.GetValue(alliance)!;
        return rpValue;
    }
}
