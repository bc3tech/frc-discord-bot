namespace Common.Discord;

using global::Discord;

using Microsoft.Extensions.Logging;

using System.Net;

using static Helpers;

public static class Extensions
{
    public static RequestOptions ToRequestOptions(this CancellationToken cancellationToken, RetryMode retryMode = RetryMode.AlwaysRetry) => new()
    {
        CancelToken = cancellationToken,
        RetryMode = retryMode,
    };

    public static Color ToDiscordColor(this System.Drawing.Color systemColor) => new(systemColor.R, systemColor.G, systemColor.B);

    public static MessageProperties WithNoEmbeds(this MessageProperties properties, string content)
    {
        properties.Flags = MessageFlags.SuppressEmbeds;
        properties.Content = content;
        return properties;
    }

    public static bool IsRetriableDiscordWrite(this Exception exception, int attempt, out TimeSpan delay)
    {
        delay = TimeSpan.Zero;
        if (attempt >= DiscordWriteMaxRetryAttempts)
        {
            return false;
        }

        if (exception is not global::Discord.Net.HttpException httpException)
        {
            return false;
        }

        delay = httpException.HttpCode switch
        {
            HttpStatusCode.TooManyRequests => TimeSpan.FromSeconds(Math.Min(2 * attempt, 10)),
            HttpStatusCode.RequestTimeout or HttpStatusCode.InternalServerError or HttpStatusCode.BadGateway or HttpStatusCode.ServiceUnavailable or HttpStatusCode.GatewayTimeout
                => TimeSpan.FromSeconds(Math.Min(attempt, 5)),
            _ => TimeSpan.Zero,
        };

        return delay > TimeSpan.Zero;
    }

    public static async Task<bool> TryModifyDiscordMessageAsync(this IUserMessage message, string content, TimeProvider? time = null, ILogger? logger = null, CancellationToken cancellationToken = default)
        => await ExecuteDiscordWriteWithRetryAsync(
            "modify response message",
            async requestOptions =>
            {
                await message.ModifyAsync(properties => properties.Content = content, options: requestOptions).ConfigureAwait(false);
                return true;
            },
            required: false, time: time ?? TimeProvider.System,
            logger: logger, cancellationToken: cancellationToken).ConfigureAwait(false);

    public static async Task SendFailureEmbedAsync(this IMessageChannel responseChannel, string description, TimeProvider? time = null, ILogger? logger = null, CancellationToken cancellationToken = default)
    {
        Embed embed = new EmbedBuilder()
            .WithDescription(description)
            .WithColor(Color.Red)
            .Build();

        bool sent = await ExecuteDiscordWriteWithRetryAsync(
            "send failure message",
            requestOptions => responseChannel.SendMessageAsync(embed: embed, options: requestOptions),
            required: false, time: time ?? TimeProvider.System,
            logger: logger, cancellationToken: cancellationToken).ConfigureAwait(false) is not null;

        if (!sent)
        {
            logger?.DiscordFailureEmbedCouldNotBeDelivered();
        }
    }

    public static async Task<bool> TryDeleteDiscordMessageAsync(this IUserMessage message, string operation, TimeProvider? time = null, ILogger? logger = null, CancellationToken cancellationToken = default)
        => await ExecuteDiscordWriteWithRetryAsync(
            operation,
            async requestOptions =>
            {
                await message.DeleteAsync(requestOptions).ConfigureAwait(false);
                return true;
            },
            required: false, time: time ?? TimeProvider.System,
            logger: logger, cancellationToken: cancellationToken).ConfigureAwait(false);

}
