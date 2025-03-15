namespace DiscordBotFunctionApp.Functions;

using DiscordBotFunctionApp.DiscordInterop;
using DiscordBotFunctionApp.TbaInterop.Models;
using DiscordBotFunctionApp.TbaInterop.Models.Notifications;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;

internal sealed class TbaWebhookHandler(DiscordMessageDispatcher dispatcher, ILogger<TbaWebhookHandler> logger)
{
    private static readonly ActivitySource activitySource = new(Assembly.GetExecutingAssembly().GetName().FullName);

    [Function("TbaWebhookHandler")]
    public async Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Function, "post", Route = "tba/webhook")] HttpRequestData req, FunctionContext ctx, CancellationToken cancellationToken)
    {
        using var fxnRun = activitySource.StartActivity(ctx.FunctionDefinition.Name);
        Debug.Assert(fxnRun is not null, "fxnRun is null - this should never happen");
        fxnRun.AddTag(nameof(ctx.InvocationId), ctx.InvocationId);

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