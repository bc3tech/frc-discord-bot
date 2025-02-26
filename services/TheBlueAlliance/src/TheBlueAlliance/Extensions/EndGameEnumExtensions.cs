namespace TheBlueAlliance.Model.MatchScoreBreakdown2025AllianceExtensions;

using static TheBlueAlliance.Model.MatchScoreBreakdown2025Alliance;

public static class EndGameEnumExtensions
{
    public static string ToGlyph(this EndGameRobot1Enum? enumValue) => ToGlyph(enumValue?.ToInvariantString());
    public static string ToGlyph(this EndGameRobot2Enum? enumValue) => ToGlyph(enumValue?.ToInvariantString());
    public static string ToGlyph(this EndGameRobot3Enum? enumValue) => ToGlyph(enumValue?.ToInvariantString());

    private static string ToGlyph(string? enumStringValue)
    {
        return enumStringValue switch
        {
            "None" => "`X`",
            "Parked" => "`✓`",
            "ShallowCage" => "🔼",
            "DeepCage" => "🔽",
            _ => string.Empty
        };
    }

}
