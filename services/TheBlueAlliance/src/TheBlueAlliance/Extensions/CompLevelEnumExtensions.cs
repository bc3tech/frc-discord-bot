namespace TheBlueAlliance.Extensions;

using TheBlueAlliance.Model;
using TheBlueAlliance.Model.CompLevelExtensions;

public static class CompLevelEnumExtensions
{
    public static string ToShortString(this CompLevel? compLevel) => ToShortString(compLevel?.ToInvariantString());

    public static string ToShortString(this CompLevel compLevel) => ToShortString(compLevel.ToInvariantString());

    private static string ToShortString(string? compLevel) => compLevel?.ToLowerInvariant() switch
    {
        "qm" => "Quals",
        "qf" => "Quarters",
        "sf" => "Elims",
        "f" => "Finals",
        null => "Unknown",
        _ => compLevel
    };

    public static string ToLongString(this CompLevel? compLevel) => ToLongString(compLevel?.ToInvariantString());

    public static string ToLongString(this CompLevel compLevel) => ToLongString(compLevel.ToInvariantString());

    private static string ToLongString(string? compLevel) => compLevel?.ToLowerInvariant() switch
    {
        "qm" => "Qualifications",
        "qf" => "Quarterfinals",
        "sf" => "Playoffs",
        "f" => "Finals",
        null => "Unknown",
        _ => compLevel
    };
}
