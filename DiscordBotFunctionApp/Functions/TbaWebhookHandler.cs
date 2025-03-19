namespace DiscordBotFunctionApp.Functions;

using Azure.Data.Tables;

using DiscordBotFunctionApp.DiscordInterop;
using DiscordBotFunctionApp.TbaInterop.Models;
using DiscordBotFunctionApp.TbaInterop.Models.Notifications;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

internal sealed class TbaWebhookHandler(DiscordMessageDispatcher dispatcher, [FromKeyedServices(Constants.ServiceKeys.TableClient_ProcessedMessages)] TableClient messagesTable, ILogger<TbaWebhookHandler> logger)
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

        if (await IsDuplicateAsync(bodyContent, cancellationToken).ConfigureAwait(false))
        {
            logger.DuplicateWebhookPayload();
            return new ConflictResult();
        }

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

    private async Task<bool> IsDuplicateAsync(string bodyContent, CancellationToken cancellationToken)
    {
        try
        {
            var bodyBytes = Encoding.UTF8.GetBytes(bodyContent);
            var hashBytes = System.Security.Cryptography.SHA3_512.HashData(bodyBytes);
            var base64UrlEncodedBody = UrlEncoder.Default.Encode(Convert.ToBase64String(hashBytes));
            var existingMessage = await messagesTable.GetEntityIfExistsAsync<TableEntity>(base64UrlEncodedBody, base64UrlEncodedBody, cancellationToken: cancellationToken).ConfigureAwait(false);

            if (!existingMessage.HasValue)
            {
                logger.NotDuplicatePayloadSaving();
                await messagesTable.UpsertEntityAsync(new TableEntity(base64UrlEncodedBody, base64UrlEncodedBody), cancellationToken: cancellationToken).ConfigureAwait(false);
                return false;
            }

            return true;
        }
        catch (Exception e) when (e is not OperationCanceledException and not TaskCanceledException)
        {
            logger.ErrorCheckingForDuplicateWebhookPayload(e);
            return false;
        }
    }
}