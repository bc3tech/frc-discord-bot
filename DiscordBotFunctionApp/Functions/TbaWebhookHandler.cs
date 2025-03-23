namespace DiscordBotFunctionApp.Functions;

using Azure.Data.Tables;

using Common;

using DiscordBotFunctionApp.DiscordInterop;
using DiscordBotFunctionApp.TbaInterop.Models;
using DiscordBotFunctionApp.TbaInterop.Models.Notifications;

using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using System.Collections.Concurrent;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

internal sealed class TbaWebhookHandler(DiscordMessageDispatcher dispatcher,
                                        [FromKeyedServices(DiscordBotFunctionApp.Constants.ServiceKeys.TableClient_ProcessedMessages)] TableClient messagesTable,
                                        IConfiguration appConfig,
                                        ILogger<TbaWebhookHandler> logger)
{
    private static readonly ConcurrentDictionary<string, Task> _processingTasks = new();

    [Function("TbaWebhookHandler")]
    public async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Function, "post", Route = "tba/webhook")] HttpRequestData req, FunctionContext context, CancellationToken cancellationToken)
    {
        var bodyContent = await req.ReadAsStringAsync().ConfigureAwait(false);
        if (string.IsNullOrWhiteSpace(bodyContent))
        {
            return req.CreateResponse(System.Net.HttpStatusCode.BadRequest);
        }

        logger.ReceivedWebhookPayloadWebhookPayload(bodyContent);

        if (await IsDuplicateAsync(bodyContent, cancellationToken).ConfigureAwait(false))
        {
            logger.DuplicateWebhookPayload();
            return req.CreateResponse(System.Net.HttpStatusCode.Conflict);
        }

        var message = JsonSerializer.Deserialize<WebhookMessage>(bodyContent);
        if (message is not null)
        {
            logger.WebhookPayloadDeserializedIntoWebhookMessageSerializedWebhookMessage(message, JsonSerializer.Serialize(message));

            if (message.MessageType is NotificationType.verification)
            {
                logger.ReceivedVerificationMessageFromTheBlueAllianceKeyVerificationKey(message.GetDataAs<Verification>()!.verification_key);
                return req.CreateResponse(System.Net.HttpStatusCode.OK);
            }
            else if (message.MessageType is NotificationType.ping)
            {
                var pingData = message.GetDataAs<Ping>();
                logger.ReceivedPingMessageFromTheBlueAllianceTitlePingTitleDescriptionPingDesc(pingData.title, pingData.desc);
                return req.CreateResponse(System.Net.HttpStatusCode.OK);
            }
            else
            {
                if (!_processingTasks.TryAdd(context.InvocationId, dispatcher.ProcessWebhookMessageAsync(message, cancellationToken)))
                {
                    logger.WebhookTaskAlreadyInProgressForInvocationIDInvocationId(context.InvocationId);
                }

                return MakeAcceptedAtResponse(req, context.InvocationId);
            }
        }

        logger.UnknownUnhandledMessage();
        var unknownResponse = req.CreateResponse(System.Net.HttpStatusCode.BadRequest);
        unknownResponse.WriteString("Unknown message type or body.");
        return unknownResponse;
    }

    private const string WebhookStatusRoute = "/api/tba/webhook/status";
    private static HttpResponseData MakeAcceptedAtResponse(HttpRequestData fromRequest, string invocationId)
    {
        var acceptedResult = fromRequest.CreateResponse(System.Net.HttpStatusCode.Accepted);
        var location = new UriBuilder(fromRequest.Url)
        {
            Path = WebhookStatusRoute,
            Query = $"?invocationId={invocationId}"
        };

        acceptedResult.Headers.Add("Location", location.ToString());
        return acceptedResult;
    }

    [Function("GetStatus")]
    public HttpResponseData GetStatus([HttpTrigger(AuthorizationLevel.Function, "get", Route = "tba/webhook/status")] HttpRequestData req, FunctionContext context)
    {
        var targetInvocationId = Throws.IfNullOrWhiteSpace(req.Query["invocationId"]);
        var task = _processingTasks[targetInvocationId];

        if (task.IsCompletedSuccessfully)
        {
            return req.CreateResponse(System.Net.HttpStatusCode.OK);
        }

        if (task.IsFaulted)
        {
            var exception = task.Exception;
            if (exception is not null)
            {
                logger.ErrorProcessingWebhookMessage(exception, targetInvocationId);
                var r = req.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
#if DEBUG
                r.WriteString(exception.InnerException?.Message ?? exception.Message);
#endif
                return r;
            }
            else
            {
                logger.WebhookTaskInvocationIdFaultedWithoutException(targetInvocationId);
            }
        }

        return MakeAcceptedAtResponse(req, targetInvocationId);
    }

    private async Task<bool> IsDuplicateAsync(string bodyContent, CancellationToken cancellationToken)
    {
        if (bool.TryParse(appConfig[DiscordBotFunctionApp.Constants.Configuration.AllowDuplicateWebhooks], out var b) && b)
        {
            return false;
        }

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