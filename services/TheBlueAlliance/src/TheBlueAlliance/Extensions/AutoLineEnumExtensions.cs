namespace TheBlueAlliance.Extensions;

using TheBlueAlliance.Model;
using TheBlueAlliance.Model.AutoLineRobot2024Extensions;

public static class AutoLineEnumExtensions
{
    public static string ToGlyph(this AutoLineRobot2024 enumValue) => ToGlyph(enumValue.ToInvariantString());

    public static string ToGlyph(this AutoLineRobot2024? enumValue) => ToGlyph(enumValue?.ToInvariantString());

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
