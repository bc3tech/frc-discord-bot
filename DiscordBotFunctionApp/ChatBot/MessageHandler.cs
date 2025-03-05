namespace DiscordBotFunctionApp.ChatBot;

using Azure.AI.Projects;
using Azure.Data.Tables;

using Discord;
using Discord.WebSocket;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel.Agents.AzureAI;

using System.Text.RegularExpressions;

#pragma warning disable SKEXP0110 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
internal sealed partial class MessageHandler(AgentsClient agentsClient, AzureAIAgent agent, DiscordSocketClient discordClient, [FromKeyedServices(Constants.ServiceKeys.TableClient_UserChatAgentThreads)] TableClient userThreadMappings, ILogger<MessageHandler> logger)
{
    private static readonly EmbedBuilder _embedBuilder = new();

    public async Task HandleUserMessageAsync(IUserMessage msg, CancellationToken cancellationToken = default)
    {
        var responseChannel = await discordClient.GetDMChannelAsync(msg.Channel.Id).ConfigureAwait(false);
        using var typing = responseChannel.EnterTypingState();

        try
        {
            AgentThread thread;
            var existingThread = await userThreadMappings.GetEntityIfExistsAsync<TableEntity>(msg.Author.Id.ToString(), msg.Author.Id.ToString(), ["AgentThreadId"], cancellationToken: cancellationToken).ConfigureAwait(false);
            if (!existingThread.HasValue || string.IsNullOrWhiteSpace(existingThread.Value?["AgentThreadId"]?.ToString()))
            {
                thread = await agentsClient.CreateThreadAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
                await userThreadMappings.UpsertEntityAsync(new TableEntity(msg.Author.Id.ToString(), msg.Author.Id.ToString())
                {
                    ["AgentThreadId"] = thread.Id,
                }, TableUpdateMode.Replace, cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            else
            {
                var threadId = existingThread.Value!["AgentThreadId"].ToString();
                thread = await agentsClient.GetThreadAsync(threadId, cancellationToken: cancellationToken).ConfigureAwait(false);
            }

            await agent.AddChatMessageAsync(thread.Id, new(Microsoft.SemanticKernel.ChatCompletion.AuthorRole.User, msg.CleanContent), cancellationToken).ConfigureAwait(false);
            await foreach (var response in agent.InvokeAsync(thread.Id, cancellationToken: cancellationToken).ConfigureAwait(false))
            {
                foreach (var c in response.Items.OfType<Microsoft.SemanticKernel.TextContent>())
                {
                    var sanitized = c.ToString();
                    var annotations = AnnotationFinder().Matches(sanitized);
                    foreach (Match annotation in annotations)
                    {
                        sanitized = sanitized.Replace(annotation.Value, string.Empty);
                    }

                    cancellationToken.ThrowIfCancellationRequested();
                    await responseChannel.SendMessageAsync(
                        $"""
                        {sanitized}

                        -# AI generated response; may have mistakes.                        
                        """, flags: MessageFlags.SuppressEmbeds);
                }
            }
        }
        catch (Exception e)
        {
            logger.ErrorRespondingToDMFromDMUser(e, msg.Author.GlobalName);
            await responseChannel.SendMessageAsync(embed: _embedBuilder
                .WithDescription("Oh no! I hit an error when trying to answer you, sorry! Try again or let your admin know about this so they can investigate.")
                .WithColor(Color.Red).Build());
        }
    }

    [GeneratedRegex("【[^】]+】", RegexOptions.Compiled)]
    private static partial Regex AnnotationFinder();
}
#pragma warning restore SKEXP0110 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
