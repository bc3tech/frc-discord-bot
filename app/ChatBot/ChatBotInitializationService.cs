namespace FunctionApp.ChatBot;

using Azure.AI.Agents.Persistent;

using Discord;
using Discord.WebSocket;

using FunctionApp.DiscordInterop.CommandModules;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

internal sealed class ChatBotInitializationService(IServiceProvider services, IDiscordClient discordClient) : IHostedService
{
    private readonly DiscordSocketClient client = discordClient as DiscordSocketClient ?? throw new ArgumentException(nameof(discordClient));
    private PersistentAgent? mealAgent;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        client.ButtonExecuted += async (button) =>
        {
            if (button.Data.CustomId is
                "chat-reset-confirm" or
                "chat-reset-cancel")
            {
                await ChatCommandModule.HandleButtonClickAsync(services, button);
            }
        };

        IConfiguration configuration = services.GetRequiredService<IConfiguration>();
        string? mealAgentId = configuration[Constants.Configuration.Azure.AI.Agents.MealAgentId];

        if (string.IsNullOrWhiteSpace(mealAgentId))
        {
            return;
        }

        PersistentAgentsClient persistentAgentsClient = services.GetRequiredService<PersistentAgentsClient>();
        mealAgent = (await persistentAgentsClient.Administration.GetAgentAsync(mealAgentId, cancellationToken).ConfigureAwait(false)).Value;
        var mealSignupInfoResponseBody = await FetchMealSignupInfoResponseBodyAsync(cancellationToken).ConfigureAwait(false);

        mealAgent = await ReplaceMealSignupsFileAsync(
            persistentAgentsClient,
            mealAgent,
            mealSignupInfoResponseBody,
            cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private static async Task<PersistentAgent> ReplaceMealSignupsFileAsync(
        PersistentAgentsClient persistentAgentsClient,
        PersistentAgent agent,
        byte[] mealSignupInfoResponseBody,
        CancellationToken cancellationToken)
    {
        PersistentAgentFileInfo uploadedFile = (await persistentAgentsClient.Files.UploadFileAsync(
            new MemoryStream(mealSignupInfoResponseBody),
            PersistentAgentFilePurpose.Agents,
            "meal_signups.json",
            cancellationToken).ConfigureAwait(false)).Value;

        List<string> fileIdsToDelete = [];
        ToolResources toolResources = agent.ToolResources;
        Debug.Assert(toolResources.CodeInterpreter is not null);

        List<string> updatedFileIds = [];

        foreach (string fileId in toolResources.CodeInterpreter.FileIds)
        {
            PersistentAgentFileInfo existingFile = (await persistentAgentsClient.Files.GetFileAsync(fileId, cancellationToken).ConfigureAwait(false)).Value;

            if (string.Equals(existingFile.Filename, "meal_signups.json", StringComparison.OrdinalIgnoreCase))
            {
                fileIdsToDelete.Add(fileId);
            }
            else
            {
                updatedFileIds.Add(fileId);
            }
        }

        updatedFileIds.Add(uploadedFile.Id);

        toolResources.CodeInterpreter.FileIds.Clear();

        foreach (string fileId in updatedFileIds)
        {
            toolResources.CodeInterpreter.FileIds.Add(fileId);
        }

        try
        {
            PersistentAgent updatedAgent = (await persistentAgentsClient.Administration.UpdateAgentAsync(
                agent.Id,
                toolResources: toolResources,
                cancellationToken: cancellationToken).ConfigureAwait(false)).Value;

            foreach (string fileId in fileIdsToDelete)
            {
                await persistentAgentsClient.Files.DeleteFileAsync(fileId, cancellationToken).ConfigureAwait(false);
            }

            return updatedAgent;
        }
        catch
        {
            await persistentAgentsClient.Files.DeleteFileAsync(uploadedFile.Id, cancellationToken).ConfigureAwait(false);
            throw;
        }
    }

    private static async Task<byte[]> FetchMealSignupInfoResponseBodyAsync(CancellationToken cancellationToken)
    {
        using HttpClientHandler handler = new()
        {
            AutomaticDecompression = DecompressionMethods.All,
        };

        HttpClient httpClient = new(handler);
        using HttpRequestMessage request = CreateSignupGeniusRequest();
        using HttpResponseMessage response = await httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsByteArrayAsync(cancellationToken).ConfigureAwait(false);
    }

    private static HttpRequestMessage CreateSignupGeniusRequest()
    {
        HttpRequestMessage request = new(HttpMethod.Post, "https://www.signupgenius.com/SUGboxAPI.cfm?go=s.getSignupInfo")
        {
            Content = new ByteArrayContent(Encoding.UTF8.GetBytes("""{"forSignUpView":true,"urlid":"10C0E45A9A623AAFFC52-60668965-2026","portalid":0}""")),
        };

        request.Headers.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/146.0.0.0 Safari/537.36 Edg/146.0.0.0");
        request.Headers.Referrer = new Uri("https://www.signupgenius.com/go/10C0E45A9A623AAFFC52-60668965-2026?useFullSite=true");
        request.Headers.TryAddWithoutValidation("accept", "application/json, text/plain, */*");
        request.Headers.TryAddWithoutValidation("accept-language", "en-US,en;q=0.9");
        request.Headers.TryAddWithoutValidation("origin", "https://www.signupgenius.com");
        request.Headers.TryAddWithoutValidation("priority", "u=1, i");
        request.Headers.TryAddWithoutValidation("sec-ch-ua", """
            "Chromium";v="146", "Not-A.Brand";v="24", "Microsoft Edge";v="146"
            """);
        request.Headers.TryAddWithoutValidation("sec-ch-ua-mobile", "?0");
        request.Headers.TryAddWithoutValidation("sec-ch-ua-platform", """
            "Windows"
            """);
        request.Content.Headers.TryAddWithoutValidation("Content-Type", "application/json, text/plain; charset=UTF-8");

        return request;
    }
}
