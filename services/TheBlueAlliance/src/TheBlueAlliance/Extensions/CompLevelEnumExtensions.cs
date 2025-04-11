namespace TheBlueAlliance.Extensions;

using TheBlueAlliance.Model;
using TheBlueAlliance.Model.MatchExtensions;
using TheBlueAlliance.Model.MatchSimpleExtensions;

public static class CompLevelEnumExtensions
{
    public static string ToShortString(this Match.CompLevelEnum compLevel) => ToShortString(compLevel.ToInvariantString());

    public static string ToShortString(this MatchSimple.CompLevelEnum compLevel) => ToShortString(compLevel.ToInvariantString());
    private static string ToShortString(string compLevel) => compLevel.ToLowerInvariant() switch
    {
        "qm" => "Quals",
        "qf" => "Quarters",
        "sf" => "Elims",
        "f" => "Finals",
        _ => compLevel
    };

    public static string ToLongString(this Match.CompLevelEnum compLevel) => ToLongString(compLevel.ToInvariantString());

    public static string ToLongString(this MatchSimple.CompLevelEnum compLevel) => ToLongString(compLevel.ToInvariantString());

    private static string ToLongString(string compLevel) => compLevel.ToLowerInvariant() switch
    {
        "qm" => "Qualifications",
        "qf" => "Quarterfinals",
        "sf" => "Playoffs",
        "f" => "Finals",
        _ => compLevel
    };
}
