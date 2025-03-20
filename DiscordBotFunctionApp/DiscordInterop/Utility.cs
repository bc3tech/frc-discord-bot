namespace DiscordBotFunctionApp.DiscordInterop;

using Discord;

internal static class Utility
{
    public static string CreateCountryFlagEmojiRef(string countryCode) => countryCode.Length is not 2
        ? throw new ArgumentException("Country code must be exactly 2 characters long", nameof(countryCode))
        : $":flag_{countryCode.ToLowerInvariant()}:";

    public static Uri CreateCountryFlagUrl(string countryCode, ushort dimension = 32) => new($"https://flagsapi.com/{countryCode}/shiny/{dimension}.png");

    public static DateTimeOffset ToLocalTime(this DateTimeOffset dateTimeOffset, TimeProvider timeProvider) => TimeZoneInfo.ConvertTimeBySystemTimeZoneId(dateTimeOffset, timeProvider.LocalTimeZone.Id);

    public static Color? GetLightestColorOf(params Color?[] colors)
    {
        // Use the formula for relative luminance
        var luminances = colors.Length is 0 ? [] : colors.Where(c => c is not null)
            .Select(c => c!.Value)
            .Select(c => (c, (0.2126 * c.R) + (0.7152 * c.G) + (0.0722 * c.B)));
        return luminances.Any() ? luminances.MaxBy(l => l.Item2).c : null;
    }
}
