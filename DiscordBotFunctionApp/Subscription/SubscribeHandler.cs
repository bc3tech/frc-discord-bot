namespace DiscordBotFunctionApp.Subscription;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

internal sealed class SubscribeHandler(ILogger<SubscribeHandler> logger)
{
    [Function("subscribe")]
    public async Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req, CancellationToken cancellationToken)
    {
        logger.LogDebug("Processing subscription request");

        var content = await new StreamReader(req.Body).ReadToEndAsync(cancellationToken).ConfigureAwait(false);
        logger.LogTrace("{SubscriptionRequestBody}", content);

        var sub = JsonSerializer.Deserialize<SubscriptionRequest>(content);
        logger.LogTrace("Deserialized request value: {SubscriptionRequest}", sub?.ToString() ?? "NULL");

        return sub is null
            ? new BadRequestObjectResult("Invalid request body.")
            : sub.Team is null && sub.Event is null
            ? new BadRequestObjectResult("Request must specify either a team or an event.")
            : new AcceptedResult();
    }
}

public record SubscriptionRequest([property: JsonPropertyName("guildId")] ulong GuildId, [property: JsonPropertyName("channelId")] ulong ChannelId, [property: JsonPropertyName("event")] string? Event, [property: JsonPropertyName("team")] uint? Team);
