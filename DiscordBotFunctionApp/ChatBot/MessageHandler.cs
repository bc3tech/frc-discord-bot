namespace DiscordBotFunctionApp.ChatBot;

using Azure.AI.Projects;
using Azure.Data.Tables;

using Discord;
using Discord.WebSocket;

using DiscordBotFunctionApp.Extensions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel.Agents.AzureAI;

using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

#pragma warning disable SKEXP0110 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
internal sealed partial class MessageHandler(AgentsClient agentsClient, AzureAIAgent agent, IDiscordClient discordClient, [FromKeyedServices(Constants.ServiceKeys.TableClient_UserChatAgentThreads)] TableClient userThreadMappings, ILogger<MessageHandler> logger)
{
    private const string DisclaimerText = "-# AI generated response; may have mistakes.";
    private static readonly EmbedBuilder _embedBuilder = new();

    public async Task HandleUserMessageAsync(IUserMessage msg, CancellationToken cancellationToken = default)
    {
        var responseChannel = await discordClient.GetDMChannelAsync(msg.Channel.Id).ConfigureAwait(false);
        using var typing = responseChannel.EnterTypingState();
        CancellationTokenSource sorryForTheDelayCanceler = new();

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
                    ["Username"] = msg.Author.Username,
                    ["GlobalName"] = msg.Author.GlobalName
                }, TableUpdateMode.Replace, cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            else
            {
                var threadId = existingThread.Value!["AgentThreadId"].ToString();
                thread = await agentsClient.GetThreadAsync(threadId, cancellationToken: cancellationToken).ConfigureAwait(false);
            }

            await agent.AddChatMessageAsync(thread.Id, new(Microsoft.SemanticKernel.ChatCompletion.AuthorRole.User, msg.CleanContent), cancellationToken).ConfigureAwait(false);
            IUserMessage? firstMessage = null, latestMessage = null, thinkingMessage = null;
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed; We intend for this to run in the background until canceled
            Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(5000, sorryForTheDelayCanceler.Token).ConfigureAwait(false);

                    for (int numDots = 3; !sorryForTheDelayCanceler.IsCancellationRequested; numDots++)
                    {
                        if (!sorryForTheDelayCanceler.IsCancellationRequested)
                        {
                            if (thinkingMessage is null)
                            {
                                thinkingMessage = await responseChannel.SendMessageAsync($"-# Sorry, still thinking...", options: sorryForTheDelayCanceler.Token.ToRequestOptions());
                            }
                            else
                            {
                                await thinkingMessage.ModifyAsync(p => p.Content = $"-# Sorry, still thinking{new string('.', numDots)}", options: sorryForTheDelayCanceler.Token.ToRequestOptions());
                            }

                            await Task.Delay(2000, sorryForTheDelayCanceler.Token).ConfigureAwait(false);
                        }
                    }
                }
                catch (Exception e) when (e is OperationCanceledException or TaskCanceledException) { }
            }, sorryForTheDelayCanceler.Token);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            try
            {
                await foreach (var response in agent.InvokeAsync(thread.Id, cancellationToken: cancellationToken).ConfigureAwait(false))
                {
                    if (response.Metadata?.TryGetValue("code", out var codeValue) is true && codeValue is true)
                    {
#if !DEBUG
                        continue;
#endif
                    }

                    await sorryForTheDelayCanceler.CancelAsync();
                    if (thinkingMessage is not null)
                    {
                        try
                        {
                            await thinkingMessage.DeleteAsync();
                            thinkingMessage = null;
                        }
                        catch { }
                    }

                    foreach (var i in response.Items.OfType<Microsoft.SemanticKernel.TextContent>())
                    {
                        if (string.IsNullOrEmpty(i.Text))
                        {
                            continue;
                        }

                        var sanitized = i.Text;
                        var annotations = AnnotationFinder().Matches(sanitized);
                        foreach (Match annotation in annotations)
                        {
                            sanitized = sanitized.Replace(annotation.Value, string.Empty);
                        }

                        cancellationToken.ThrowIfCancellationRequested();

                        if (sanitized.Length > 2000)
                        {
                            foreach (var bit in sanitized.Chunk(2000))
                            {
                                await postAsync(new(bit), response.Metadata?.TryGetValue("code", out var isCode) is true && isCode is true);
                            }
                        }
                        else
                        {
                            await postAsync(sanitized, response.Metadata?.TryGetValue("code", out var isCode) is true && isCode is true);
                        }
                    }
                }

                async Task postAsync(string msg, bool isCode)
                {
                    msg = isCode ? $"```\n{msg}\n```" : msg;
                    if (firstMessage is null)
                    {
                        firstMessage = latestMessage = await responseChannel.SendMessageAsync(msg, flags: MessageFlags.SuppressEmbeds, options: cancellationToken.ToRequestOptions()).ConfigureAwait(false);
                    }
                    else
                    {
                        latestMessage = await firstMessage.ReplyAsync(msg, flags: MessageFlags.SuppressEmbeds, options: cancellationToken.ToRequestOptions()).ConfigureAwait(false);
                    }
                }
            }
            catch (NullReferenceException nre)
            {
                // There's a bug in SK during streaming that, when it goes to annotate stuff, it bombs out. So, when this happens we just assume we're all done with the response for now.
                logger.NullRefHitWhileStreamingResponseBackFromAgent(nre);
            }

            Debug.Assert(latestMessage is not null);
            if (latestMessage.Content.Length + DisclaimerText.Length + 2 > 2000)    // +2 for the line breaks we add if we can
            {
                await latestMessage.ReplyAsync(DisclaimerText, options: cancellationToken.ToRequestOptions()).ConfigureAwait(false);
            }
            else
            {
                await latestMessage.ModifyAsync(p => p.Content = $"{latestMessage.Content}\n\n{DisclaimerText}", options: cancellationToken.ToRequestOptions()).ConfigureAwait(false);
            }
        }
        catch (Exception e)
        {
            logger.ErrorRespondingToDMFromDMUser(e, msg.Author.GlobalName);
            await responseChannel.SendMessageAsync(embed: _embedBuilder
                .WithDescription("Oh no! I hit an error when trying to answer you, sorry! Try again or let your admin know about this so they can investigate."
#if DEBUG
                + $"\n\n```\n{e}\n```"
#endif
                )
                .WithColor(Color.Red).Build());
        }
        finally
        {
            await sorryForTheDelayCanceler.CancelAsync().ConfigureAwait(false);
            sorryForTheDelayCanceler.Dispose();
            typing.Dispose();
        }
    }

    [GeneratedRegex(@"\w*【[^】]+】\w*", RegexOptions.Compiled)]
    private static partial Regex AnnotationFinder();
}
#pragma warning restore SKEXP0110 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
