namespace Common.Extensions;

using System.Diagnostics.CodeAnalysis;

public static class StringExtensions
{
    public static ushort? ToTeamNumber(this string? s) => ushort.TryParse(s?.TrimStart(['f', 'r', 'c']), out var i) ? i : null;

    [return: NotNullIfNotNull(nameof(replacement))]
    public static string? UnlessNullOrWhitespaceThen(this string? s, string? replacement) => string.IsNullOrWhiteSpace(s) ? replacement : s;
}
