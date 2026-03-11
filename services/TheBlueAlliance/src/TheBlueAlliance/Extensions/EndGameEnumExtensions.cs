namespace TheBlueAlliance.Extensions;

using TheBlueAlliance.Model;
using TheBlueAlliance.Model.EndGameRobot2025Extensions;
using TheBlueAlliance.Model.TowerRobot2026Extensions;

public static class EndGameEnumExtensions
{
    public static string ToGlyph(this EndGameRobot2025 enumValue) => ToGlyph(enumValue.ToInvariantString());
    public static string ToGlyph(this TowerRobot2026 enumValue) => ToGlyph2026(enumValue.ToInvariantString());

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

    private static string ToGlyph2026(string? enumStringValue)
    {
        return enumStringValue switch
        {
            "None" => "`X`",
            "Level1" => "`1`",
            "Level2" => "`2`",
            "Level3" => "`3`",
            _ => string.Empty
        };
    }
}
