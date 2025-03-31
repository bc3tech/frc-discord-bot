namespace TheBlueAlliance.Extensions;

using TheBlueAlliance.Model.MatchExtensions;
using TheBlueAlliance.Model.MatchSimpleExtensions;

public static class WinningAllianceEnumExtensions
{
    public static string ToGlyph(this Model.Match.WinningAllianceEnum b, Model.Match.WinningAllianceEnum winner) => (b == winner) ? ":white_check_mark:" : ":x:";
    public static string ToGlyph(this Model.Match.WinningAllianceEnum b, Model.MatchSimple.WinningAllianceEnum winner) => (b.ToInvariantString() == winner.ToInvariantString()) ? ":white_check_mark:" : ":x:";
    public static string ToGlyph(this Model.MatchSimple.WinningAllianceEnum b, Model.MatchSimple.WinningAllianceEnum winner) => ToGlyph(winner, b);
    public static string ToGlyph(this Model.MatchSimple.WinningAllianceEnum b, Model.Match.WinningAllianceEnum winner) => ToGlyph(winner, b);
}
