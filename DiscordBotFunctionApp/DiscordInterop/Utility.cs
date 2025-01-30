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
}
