namespace FunctionApp.ChatBot;

using Azure;
using Azure.AI.Agents.Persistent;
using Azure.Data.Tables;

using Common.Extensions;

using Discord;

using FunctionApp;
using FunctionApp.Extensions;

using Microsoft.Agents.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Text.Json;
using System.Text.RegularExpressions;

internal sealed partial class MessageHandler(
    PersistentAgentsClient agentsClient,
    ChatBotAgentResolver agentResolver,
    IDiscordClient discordClient,
    [FromKeyedServices(Constants.ServiceKeys.TableClient_UserChatAgentThreads)] TableClient userThreadMappings,
    TimeProvider time,
    Meter meter,
    ILogger<MessageHandler> logger)
{
    private const string DisclaimerText = "-# AI generated response; may have mistakes.";
    private static readonly EmbedBuilder _embedBuilder = new();

    public async Task HandleUserMessageAsync(IUserMessage msg, CancellationToken cancellationToken = default)
    {
        var interactionStartTime = time.GetUtcNow();
        var responseChannel = await discordClient.GetDMChannelAsync(msg.Channel.Id).ConfigureAwait(false);
        using var typing = responseChannel.EnterTypingState();
        CancellationTokenSource sorryForTheDelayCanceler = new();

        var serializedAuthor = JsonSerializer.Serialize(msg.Author);

        try
        {
            var thread = await GetOrCreateThreadAsync(msg.Author.Id, serializedAuthor, cancellationToken).ConfigureAwait(false);
            IUserMessage thinkingMessage = await responseChannel.SendMessageAsync("-# Working on it...", options: sorryForTheDelayCanceler.Token.ToRequestOptions()).ConfigureAwait(false);
            var userMessageContent = $"""
                {msg.CleanContent}

                ===
                User Display Name: {msg.Author.GlobalName}
                User Id: {msg.Author.Username}
                ===
                """;
            await agentsClient.Messages.CreateMessageAsync(thread.Id, MessageRole.User, userMessageContent, cancellationToken: cancellationToken).ConfigureAwait(false);

            IUserMessage? firstMessage = null, latestMessage = null;
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed; We intend for this to run in the background until canceled
            Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(2), time, sorryForTheDelayCanceler.Token).ConfigureAwait(false);
                    for (int numDots = 3; !sorryForTheDelayCanceler.IsCancellationRequested; numDots++)
                    {
                        if (!sorryForTheDelayCanceler.IsCancellationRequested)
                        {
                            await thinkingMessage.ModifyAsync(p => p.Content = $"-# Working on it{new string('.', numDots)}", options: sorryForTheDelayCanceler.Token.ToRequestOptions()).ConfigureAwait(false);

                            await Task.Delay(TimeSpan.FromSeconds(2), time, sorryForTheDelayCanceler.Token).ConfigureAwait(false);
                        }
                    }
                }
                catch (Exception e) when (e is not OperationCanceledException and not TaskCanceledException)
                {
                    logger.FailedWhileUpdatingTheChatbotProgressIndicator(e);
                }
            }, sorryForTheDelayCanceler.Token);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            var agent = await agentResolver.GetConfiguredAgentAsync(cancellationToken);
            var run = (await agentsClient.Runs.CreateRunAsync(thread.Id, agent.Id, cancellationToken: cancellationToken).ConfigureAwait(false)).Value;

            while (run.Status == RunStatus.Queued || run.Status == RunStatus.InProgress || run.Status == RunStatus.Cancelling)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(500), time, cancellationToken).ConfigureAwait(false);
                run = (await agentsClient.Runs.GetRunAsync(thread.Id, run.Id, cancellationToken).ConfigureAwait(false)).Value;
            }

            logger.ResponseResponse(JsonSerializer.Serialize(run));

            var usage = run.Usage;
            if (usage is not null)
            {
                meter.LogMetric("TokenUsage", usage.TotalTokens, new Dictionary<string, object?>
                {
                    ["ThreadId"] = thread.Id,
                    ["Usage"] = JsonSerializer.Serialize(usage),
                    ["RunId"] = run.Id,
                    ["Author"] = serializedAuthor
                });
            }

            if (run.Status == RunStatus.RequiresAction)
            {
                throw new InvalidOperationException("The configured Azure AI Foundry agent requested tool outputs that this bot does not submit automatically.");
            }

            if (run.Status != RunStatus.Completed)
            {
                throw new InvalidOperationException($"Azure AI Foundry agent run ended with status '{run.Status}'. {run.LastError?.Message}");
            }

            await foreach (var response in agentsClient.Messages.GetMessagesAsync(thread.Id, runId: run.Id, order: ListSortOrder.Ascending, cancellationToken: cancellationToken).ConfigureAwait(false))
            {
                logger.ResponseResponse(JsonSerializer.Serialize(response));

                if (response.Role != MessageRole.Agent)
                {
                    continue;
                }

                await sorryForTheDelayCanceler.CancelAsync().ConfigureAwait(false);
                if (thinkingMessage is not null)
                {
                    try
                    {
                        await thinkingMessage.DeleteAsync().ConfigureAwait(false);
                    }
                    catch (Exception e) when (e is not OperationCanceledException and not TaskCanceledException)
                    {
                        logger.FailedToRemoveTransientThinkingMessage(e);
                    }
                }

                foreach (var content in response.ContentItems.OfType<MessageTextContent>())
                {
                    if (string.IsNullOrEmpty(content.Text))
                    {
                        continue;
                    }

                    var sanitized = content.Text;
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
                            await PostAsync(new(bit), false).ConfigureAwait(false);
                        }
                    }
                    else
                    {
                        await PostAsync(sanitized, false).ConfigureAwait(false);
                    }
                }
            }

            if (latestMessage is null)
            {
                throw new InvalidOperationException("Azure AI Foundry agent run completed without returning any message content.");
            }

            Debug.Assert(latestMessage is not null);
            if (latestMessage.Content.Length + DisclaimerText.Length + 2 > 2000)
            {
                await latestMessage.ReplyAsync(DisclaimerText, options: cancellationToken.ToRequestOptions()).ConfigureAwait(false);
            }
            else
            {
                await latestMessage.ModifyAsync(p => p.Content = $"{latestMessage.Content}\n\n{DisclaimerText}", options: cancellationToken.ToRequestOptions()).ConfigureAwait(false);
            }

            async Task PostAsync(string content, bool isCode)
            {
                var outgoing = isCode ? $"```\n{content}\n```" : content;
                if (firstMessage is null)
                {
                    firstMessage = latestMessage = await responseChannel.SendMessageAsync(outgoing, flags: MessageFlags.SuppressEmbeds, options: cancellationToken.ToRequestOptions()).ConfigureAwait(false);
                    meter.LogMetric("AgentFirstMessageDelaySec", (time.GetUtcNow() - interactionStartTime).TotalSeconds);
                }
                else
                {
                    latestMessage = await firstMessage.ReplyAsync(outgoing, flags: MessageFlags.SuppressEmbeds, options: cancellationToken.ToRequestOptions()).ConfigureAwait(false);
                }
            }
        }
        catch (Exception e) when (e is not OperationCanceledException and not TaskCanceledException)
        {
            logger.ErrorRespondingToDMFromDMUser(e, msg.Author.GlobalName);
            await responseChannel.SendMessageAsync(embed: _embedBuilder
                .WithDescription("Oh no! I hit an error when trying to answer you, sorry! Try again or let your admin know about this so they can investigate."
#if DEBUG
                + $"\n\n```\n{e}\n```"
#endif
                )
                .WithColor(Color.Red).Build()).ConfigureAwait(false);
        }
        finally
        {
            await sorryForTheDelayCanceler.CancelAsync().ConfigureAwait(false);
            sorryForTheDelayCanceler.Dispose();
            typing.Dispose();
        }

        meter.LogMetric("InteractionTimeSec", (time.GetUtcNow() - interactionStartTime).TotalSeconds);
    }

    [GeneratedRegex(@"\w*【[^】]+】\w*", RegexOptions.Compiled)]
    private static partial Regex AnnotationFinder();

    private async Task<PersistentAgentThread> GetOrCreateThreadAsync(ulong userId, string serializedAuthor, CancellationToken cancellationToken)
    {
        var userIdString = userId.ToString();
        var existingThread = await userThreadMappings.GetEntityIfExistsAsync<TableEntity>(
            userIdString,
            userIdString,
            ["AgentThreadId"],
            cancellationToken: cancellationToken).ConfigureAwait(false);

        if (!existingThread.HasValue || string.IsNullOrWhiteSpace(existingThread.Value?["AgentThreadId"]?.ToString()))
        {
            return await CreateThreadAsync(userIdString, serializedAuthor, cancellationToken).ConfigureAwait(false);
        }

        var threadId = existingThread.Value!["AgentThreadId"]?.ToString();
        Debug.Assert(!string.IsNullOrWhiteSpace(threadId));

        try
        {
            return (await agentsClient.Threads.GetThreadAsync(threadId!, cancellationToken).ConfigureAwait(false)).Value;
        }
        catch (RequestFailedException e) when (e.Status == 404)
        {
            logger.AzureAIThreadThreadIdForDiscordUserUserIdWasMissingCreatingAReplacementThread(threadId!, userId);
            return await CreateThreadAsync(userIdString, serializedAuthor, cancellationToken).ConfigureAwait(false);
        }
    }

    private async Task<PersistentAgentThread> CreateThreadAsync(string userId, string serializedAuthor, CancellationToken cancellationToken)
    {
        var thread = (await agentsClient.Threads.CreateThreadAsync(cancellationToken: cancellationToken).ConfigureAwait(false)).Value;
        await userThreadMappings.UpsertEntityAsync(new TableEntity(userId, userId)
        {
            ["AgentThreadId"] = thread.Id,
            ["Author"] = serializedAuthor,
        }, TableUpdateMode.Replace, cancellationToken: cancellationToken).ConfigureAwait(false);

        return thread;
    }
}
