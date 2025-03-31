namespace Common.Extensions;

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;

public static partial class StringExtensions
{
    private static readonly Regex TeamNumberFinder = TeamNumberRegex();

    public static ushort? TeamKeyToTeamNumber(this string? s)
    {
        if (string.IsNullOrWhiteSpace(s))
        {
            return null;
        }

        if (TeamNumberFinder.Match(s) is not { Success: true } match || match.Groups.Count is not 2)
        {
            Debug.Assert(string.IsNullOrWhiteSpace(s), "Converting team key to number failed"); // Alert developer if a team key was given, but couldn't find a team number in it
            return null;
        }

        if (!ushort.TryParse(match.Groups[1].Value, out var i))
        {
            Debug.Assert(string.IsNullOrWhiteSpace(s), "Converting team key to number failed"); // Alert developer if a team number was found, but couldn't be parsed to a ushort as that should never happen
            return null;
        }

        return i;
    }

    [return: NotNullIfNotNull(nameof(replacement))]
    public static string? UnlessNullOrWhitespaceThen(this string? s, string? replacement) => string.IsNullOrWhiteSpace(s) ? replacement : s;

    [GeneratedRegex(@"(\d+)", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant)]
    private static partial Regex TeamNumberRegex();

    public static string Compress(this string s)
    {
        byte[] buffer = Encoding.UTF8.GetBytes(s);
        using var memoryStream = new MemoryStream();
        using var brotliStream = new BrotliStream(memoryStream, CompressionMode.Compress, true);
        brotliStream.Write(buffer, 0, buffer.Length);
        brotliStream.Flush();

        var r = Convert.ToBase64String(memoryStream.ToArray());
        Debug.Assert(!string.IsNullOrWhiteSpace(r));
        return r;
    }

    public static string Decompress(this string s)
    {
        using var brotliStream = new BrotliStream(new MemoryStream(Convert.FromBase64String(s)), CompressionMode.Decompress);
        using var resultStream = new MemoryStream();
        brotliStream.CopyTo(resultStream);
        return Encoding.UTF8.GetString(resultStream.ToArray());
    }
}
