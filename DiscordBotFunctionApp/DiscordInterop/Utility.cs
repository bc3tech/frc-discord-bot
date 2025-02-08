namespace DiscordBotFunctionApp.DiscordInterop;

using Discord;

internal static class Utility
{
    public static RequestOptions CreateCancelRequestOptions(CancellationToken cancellationToken) => new() { CancelToken = cancellationToken };

    public static string CreateCountryFlagEmojiRef(string countryCode) => countryCode.Length is not 2
        ? throw new ArgumentException("Country code must be exactly 2 characters long", nameof(countryCode))
        : $":flag_{countryCode.ToLowerInvariant()}:";

    public static Uri CreateCountryFlagUrl(string countryCode, ushort dimension = 32) => new($"https://flagsapi.com/{countryCode}/shiny/{dimension}.png");

    public static DateTimeOffset ToPacificTime(this DateTimeOffset dateTimeOffset) => TimeZoneInfo.ConvertTimeBySystemTimeZoneId(dateTimeOffset, "Pacific Standard Time");

    public static Color? GetLightestColorOf(params Color?[] colors)
    {
        // Use the formula for relative luminance
        var luminances = colors.Where(c => c is not null)
            .Select(c => c!.Value)
            .Select(c => (c, 0.2126 * c.R + 0.7152 * c.G + 0.0722 * c.B));
        return luminances.Any() ? luminances.MaxBy(l => l.Item2).c : null;
    }
}
