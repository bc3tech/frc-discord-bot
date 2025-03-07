namespace DiscordBotFunctionApp.ChatBot;

using Azure;
using Azure.AI.Inference;
using Azure.AI.Projects;

using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents.AzureAI;

using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;

#pragma warning disable SKEXP0110 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
internal class ChatRunner(AgentsClient agentsClient, AzureAIAgent agent, ILogger<ChatRunner> logger)
{
    public async IAsyncEnumerable<string> GetCompletionsAsync(string prompt, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var thread = (await agentsClient.CreateThreadAsync(messages: [new ThreadMessageOptions(MessageRole.User, prompt)], cancellationToken: cancellationToken).ConfigureAwait(false)).Value;

        await foreach (var response in agent.InvokeAsync(thread.Id, cancellationToken: cancellationToken)
            .Where(i => i.Role == Microsoft.SemanticKernel.ChatCompletion.AuthorRole.Assistant))
        {
            logger.ResponseResponse(JsonSerializer.Serialize(response));

            var usage = response.Metadata?["Usage"] as RunStepCompletionUsage;
            if (usage is not null)
            {
                logger.LogMetric("TokenUsage", usage.TotalTokens, new Dictionary<string, object>
                {
                    ["ThreadId"] = thread.Id,
                    ["Usage"] = JsonSerializer.Serialize(usage),
                    ["RunId"] = response.Metadata!["RunId"]?.ToString() ?? "Unknown",
                });
            }

            if (response.Metadata?.TryGetValue("code", out var codeValue) is true && codeValue is true)
            {
#if !DEBUG
                continue;
#endif
            }

            foreach (var i in response.Items.OfType<TextContent>())
            {
                if (!string.IsNullOrEmpty(i.Text))
                {
                    yield return i.Text;
                }
            }
        }
    }
}
#pragma warning restore SKEXP0110 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
