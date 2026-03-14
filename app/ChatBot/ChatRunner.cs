namespace FunctionApp.ChatBot;

using Azure.AI.Agents.Persistent;

using Common.Extensions;

using FunctionApp;

using Microsoft.Extensions.Logging;

using System.Diagnostics.Metrics;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;

internal sealed class ChatRunner(PersistentAgentsClient agentsClient, ChatBotAgentResolver agentResolver, Meter meter, ILogger<ChatRunner> logger)
{
    public async IAsyncEnumerable<string> GetCompletionsAsync(string prompt, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var agent = await agentResolver.GetConfiguredAgentAsync(cancellationToken).ConfigureAwait(false);
        var thread = (await agentsClient.Threads.CreateThreadAsync(
            messages: [new ThreadMessageOptions(MessageRole.User, prompt)],
            cancellationToken: cancellationToken).ConfigureAwait(false)).Value;

        try
        {
            var run = (await agentsClient.Runs.CreateRunAsync(thread, agent, cancellationToken).ConfigureAwait(false)).Value;

            while (run.Status == RunStatus.Queued || run.Status == RunStatus.InProgress || run.Status == RunStatus.Cancelling)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(500), cancellationToken).ConfigureAwait(false);
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
                if (response.Role != MessageRole.Agent)
                {
                    continue;
                }

                logger.ResponseResponse(JsonSerializer.Serialize(response));

                foreach (var content in response.ContentItems.OfType<MessageTextContent>())
                {
                    if (!string.IsNullOrEmpty(content.Text))
                    {
                        yield return content.Text;
                    }
                }
            }
        }
        finally
        {
            try
            {
                await agentsClient.Threads.DeleteThreadAsync(thread.Id, CancellationToken.None).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                logger.FailedToDeleteTemporaryAzureAIFoundryThread(e, thread.Id);
            }
        }
    }
}
