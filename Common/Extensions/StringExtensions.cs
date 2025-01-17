namespace Common.Extensions;
public static class StringExtensions
{
    public static ushort? ToTeamNumber(this string? s) => ushort.TryParse(s?.TrimStart(['f', 'r', 'c']), out var i) ? i : null;
}
