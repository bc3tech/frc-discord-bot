namespace DiscordBotFunctionApp.Functions;

using DiscordBotFunctionApp.DiscordInterop;
using DiscordBotFunctionApp.TbaInterop.Models;
using DiscordBotFunctionApp.TbaInterop.Models.Notifications;

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

        logger.ReceivedWebhookPayloadWebhookPayload(bodyContent);

        var message = JsonSerializer.Deserialize<WebhookMessage>(bodyContent);
        if (message is not null)
        {
            logger.WebhookPayloadDeserializedIntoWebhookMessageSerializedWebhookMessage(message, JsonSerializer.Serialize(message));

            if (message.MessageType is NotificationType.verification)
            {
                logger.ReceivedVerificationMessageFromTheBlueAllianceKeyVerificationKey(message.GetDataAs<Verification>()!.verification_key);
                return new OkResult();
            }
            else if (message.MessageType is NotificationType.ping)
            {
                var pingData = message.GetDataAs<Ping>();
                logger.ReceivedPingMessageFromTheBlueAllianceTitlePingTitleDescriptionPingDesc(pingData.title, pingData.desc);
                return new OkResult();
            }
            else
            {
                try
                {
                    var handled = await dispatcher.ProcessWebhookMessageAsync(message, cancellationToken).ConfigureAwait(false);
                    if (handled)
                    {
                        return new OkResult();
                    }
                    else
                    {
                        logger.UnhandledWebhookMessageWebhookPayload(bodyContent);
                    }
                }
                catch (Exception e) when (e is TaskCanceledException or OperationCanceledException)
                {
                    logger.OperationCancelled(e);
                    return new OkObjectResult("Operation cancelled.");
                }
            }
        }

        logger.UnknownUnhandledMessage();
        return new BadRequestObjectResult("Unknown message type or body.");
    }
}