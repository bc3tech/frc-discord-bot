namespace DiscordBotFunctionApp;
using Discord.Rest;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using System.Threading.Tasks;

internal sealed class DiscordRestApiHandler(DiscordRestClient discordClient, IConfiguration appConfig, ILogger<DiscordRestApiHandler> logger)
{
    [Function("interactions")]
    public async Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req, CancellationToken cancellationToken)
    {
        var bodyString = await new StreamReader(req.Body).ReadToEndAsync(cancellationToken);
        var interaction = await discordClient.ParseHttpInteractionAsync(appConfig[Constants.Configuration.Discord.PublicKey], req.Headers["x-signature-ed25519"], req.Headers["x-signature-timestamp"], bodyString);
        if (interaction.Type is Discord.InteractionType.Ping)
        {
            logger.LogInformation("Received PING interaction");
            return new OkObjectResult(new { type = 1 });
        }
        else
        {
            logger.LogWarning("Received unknown interaction type: {InteractionType}", interaction.Type);
            return new BadRequestObjectResult("Unknown interaction type.");
        }
    }
}
