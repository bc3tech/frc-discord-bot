namespace DiscordBotFunctionApp.ChatBot;

using Azure.AI.Inference;
using Azure.AI.Projects;

using Discord;
using Discord.WebSocket;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.AzureAI;

using System.Collections.Concurrent;
using System.Text.RegularExpressions;

#pragma warning disable SKEXP0110 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
internal sealed partial class MessageHandler(AgentsClient agentsClient, AzureAIAgent agent, DiscordSocketClient discordClient, ILogger<MessageHandler> logger)
{
    private static readonly EmbedBuilder _embedBuilder = new();

    private readonly ConcurrentDictionary<ulong, string> _threadIdMap = new();
    public async Task HandleUserMessageAsync(IUserMessage msg, CancellationToken cancellationToken = default)
    {
        var responseChannel = await discordClient.GetDMChannelAsync(msg.Channel.Id).ConfigureAwait(false);
        using var typing = responseChannel.EnterTypingState();

        try
        {
            AgentThread thread;
            if (!_threadIdMap.TryGetValue(msg.Author.Id, out var threadId))
            {
                thread = await agentsClient.CreateThreadAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
                threadId = thread.Id;
                threadId = _threadIdMap.GetOrAdd(msg.Author.Id, threadId);
            }
            else
            {
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
