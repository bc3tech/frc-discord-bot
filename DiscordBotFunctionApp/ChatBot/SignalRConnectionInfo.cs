namespace DiscordBotFunctionApp.ChatBot;

using Common;

using Discord;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using System.Net.Http.Json;

/// <summary>
/// Contains necessary information for a SignalR client to connect to SignalR Service.
/// </summary>
internal sealed class SignalRConnectionInfo(IConfiguration appConfig, IHttpClientFactory httpClients, ILogger<SignalRConnectionInfo> negotiationLogger)
{
    public async ValueTask<ConnectionInfo?> CreateUserConnectionAsync(IUser discordUser, CancellationToken cancellationToken = default)
    {
        var client = httpClients.CreateClient(DiscordBotFunctionApp.Constants.ServiceKeys.ChatBotHttpClient);
        HttpResponseMessage hubNegotiateResponse = new();
        for (var i = 0; i < 10; i++)
        {
            try
            {
                hubNegotiateResponse = await client.PostAsync($@"{appConfig[DiscordBotFunctionApp.Constants.Configuration.SignalREndpoint]}?userid={discordUser.Username}", null, cancellationToken).ConfigureAwait(false);
                break;
            }
            catch (Exception e)
            {
                negotiationLogger.LogDebug(e, $@"Negotiation failed");
                await Task.Delay(1000, cancellationToken).ConfigureAwait(false);
            }
        }

        if (hubNegotiateResponse is null)
        {
            negotiationLogger.LogCritical("Unable to connect to server.");
            return null;
        }

        hubNegotiateResponse.EnsureSuccessStatusCode();

        ConnectionInfo? connInfo;
        try
        {
            connInfo = await hubNegotiateResponse.Content.ReadFromJsonAsync<ConnectionInfo>(cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            negotiationLogger.LogDebug(ex, "Error parsing negotiation response");
            negotiationLogger.LogCritical("Unable to connect to server. Exiting.");
            return null;
        }

        return Throws.IfNull(connInfo);
    }

    public sealed record ConnectionInfo(Uri Url, string? AccessToken);
}
