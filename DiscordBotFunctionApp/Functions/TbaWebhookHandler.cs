namespace DiscordBotFunctionApp.Functions;

using Common.Tba;
using Common.Tba.Notifications;

using DiscordBotFunctionApp.DiscordInterop;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

using System.Text.Json;
using System.Threading.Tasks;

internal sealed class TbaWebhookHandler(DiscordMessageDispatcher dispatcher, ILogger<TbaWebhookHandler> logger)
{
    [Function("TbaWebhookHandler")]
    public async Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Function, "post", Route = "tba/webhook")] HttpRequestData req, CancellationToken cancellationToken)
    {
        var bodyContent = await req.ReadAsStringAsync().ConfigureAwait(false);
        if (string.IsNullOrWhiteSpace(bodyContent))
        {
            return new BadRequestObjectResult("No body content.");
        }

        var message = JsonSerializer.Deserialize<WebhookMessage>(bodyContent);
        if (message is not null)
        {
            if (message.MessageType is NotificationType.verification)
            {
                logger.LogInformation("Received verification message from The Blue Alliance. Key: {VerificationKey}", (await message.GetDataAsAsync<Verification>(cancellationToken).ConfigureAwait(false))!.verification_key);
                return new OkResult();
            }
            else if (message.MessageType is NotificationType.ping)
            {
                var pingData = await message.GetDataAsAsync<Ping>(cancellationToken).ConfigureAwait(false)!;
                logger.LogInformation("Received ping message from The Blue Alliance:\nTitle: {PingTitle}\nDescription: {PingDesc}", pingData.title, pingData.desc);
                return new OkResult();
            }
            else
            {
                var handled = await dispatcher.ProcessWebhookMessageAsync(message, cancellationToken).ConfigureAwait(false);
                if (handled)
                {
                    return new OkResult();
                }
            }
        }

        logger.LogError("Unknown/unhandled message: {WebhookMessage}", bodyContent);
        return new BadRequestObjectResult("Unknown message type or body.");
    }
}