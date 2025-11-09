namespace Common.Extensions;
public static class BoolExtensions
{
    public static string ToGlyph(this bool? b) => b switch
    {
        true => ":white_check_mark:",
        false => ":x:",
        _ => string.Empty
    };
}
