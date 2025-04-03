namespace TheBlueAlliance.Model.MatchScoreBreakdown2025AllianceExtensions;

using static TheBlueAlliance.Model.MatchScoreBreakdown2025Alliance;

public static class AutoLineEnumExtensions
{
    public static string ToGlyph(this AutoLineRobot1Enum? enumValue) => ToGlyph(enumValue?.ToInvariantString());
    public static string ToGlyph(this AutoLineRobot2Enum? enumValue) => ToGlyph(enumValue?.ToInvariantString());
    public static string ToGlyph(this AutoLineRobot3Enum? enumValue) => ToGlyph(enumValue?.ToInvariantString());

    private static string ToGlyph(string? enumStringValue)
    {
        return enumStringValue switch
        {
            "No" => "`X`",
            "Yes" => "`✓`",
            _ => string.Empty
        };
    }
}
