namespace DiscordBotFunctionApp.ChatBot;

using Common;

using Discord;

using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using System.Collections.Concurrent;

internal sealed class HubConnectionFactory(IConfiguration appConfig, SignalRConnectionInfo connInfo)
{
    private static readonly ConcurrentDictionary<string, HubConnection> _userConnections = [];
    public async Task<HubConnection> StartConnectionForUserAsync(IUser discordUser, CancellationToken cancellation = default)
    {
        if (!_userConnections.TryGetValue(discordUser.Username, out var conn)
            || conn.State is HubConnectionState.Disconnected)
        {
            var negotiationResult = await connInfo.CreateUserConnectionAsync(discordUser, cancellation).ConfigureAwait(false);
            conn = new HubConnectionBuilder()
                .WithUrl(Throws.IfNull(negotiationResult).Url, o => o.AccessTokenProvider = () => Task.FromResult(negotiationResult.AccessToken))
                .ConfigureLogging(lb => lb
                    .AddConfiguration(appConfig.GetSection("Logging"))
                    .AddSimpleConsole(o =>
                    {
                        o.SingleLine = true;
                        o.ColorBehavior = Microsoft.Extensions.Logging.Console.LoggerColorBehavior.Enabled;
                        o.IncludeScopes = true;
                    })
                )
                .WithAutomaticReconnect()
                .WithStatefulReconnect()
                .Build();

            await conn.StartAsync(cancellation).ConfigureAwait(false);

            conn.On(DiscordBotFunctionApp.Constants.SignalR.Functions.PostStatus, async (string from, string message) =>
            {
                var dmChannel = await discordUser.CreateDMChannelAsync();
                await dmChannel.SendMessageAsync($@"-# **{from}**: {message}", flags: MessageFlags.SuppressNotification);
            });

            _userConnections.AddOrUpdate(discordUser.Username, conn, (_, _) => conn);
        }

        return conn;
    }
}
