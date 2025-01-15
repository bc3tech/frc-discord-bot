namespace Common.Extensions;
public static class StringExtensions
{
    public static int? ToTeamNumber(this string? s) => int.TryParse(s?.TrimStart(['f', 'r', 'c']), out var i) ? i : null;
}
