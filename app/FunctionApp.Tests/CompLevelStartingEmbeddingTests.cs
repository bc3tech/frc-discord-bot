namespace FunctionApp.Tests;

using FunctionApp.DiscordInterop.Embeds;
using FunctionApp.TbaInterop.Models;
using FunctionApp.TbaInterop.Models.Notifications;

using Microsoft.Extensions.Logging.Abstractions;

using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

using TheBlueAlliance.Extensions;
using TheBlueAlliance.Model;

using CompLevelStartingEmbed = FunctionApp.DiscordInterop.Embeds.CompLevelStarting;

[SuppressMessage("Performance", "EA0014:Add CancellationToken as the parameter of asynchronous method", Justification = "xUnit [Fact]/[Theory] methods do not support a CancellationToken parameter in this test setup.")]
public sealed class CompLevelStartingEmbeddingTests
{
    [Theory]
    [InlineData("qm", "2026 California Regional")]
    [InlineData("sf", "2027 Michigan State Championship")]
    [InlineData("f", "2028 FIRST Championship")]
    public async Task CreateAsyncBuildsExpectedCompLevelHeadlineAcrossSeasonPayloads(string compLevel, string eventName)
    {
        using HttpClient httpClient = new();
        CompLevelStartingEmbed creator = new(CreateBuilderFactory(httpClient), TimeProvider.System, NullLogger<CompLevelStartingEmbed>.Instance);
        WebhookMessage message = CreateMessage(
            NotificationType.starting_comp_level,
            $$"""{"event_name":"{{eventName}}","comp_level":"{{compLevel}}","event_key":"2027test","scheduled_time":1744232100}""");

        SubscriptionEmbedding[] embeddings = await CollectEmbeddingsAsync(creator.CreateAsync(message, highlightTeam: null));

        var expectedCompLevel = Enum.Parse<CompLevel>(compLevel, ignoreCase: true).ToLongString();
        var description = Assert.Single(embeddings).Content.Description;
        Assert.Contains(expectedCompLevel, description, StringComparison.Ordinal);
        Assert.Contains($"Starting soon for {eventName}", description, StringComparison.Ordinal);
    }

    [Fact]
    public async Task CreateAsyncReturnsNoEmbeddingsForUnparseableNotificationPayload()
    {
        using HttpClient httpClient = new();
        CompLevelStartingEmbed creator = new(CreateBuilderFactory(httpClient), TimeProvider.System, NullLogger<CompLevelStartingEmbed>.Instance);
        WebhookMessage message = CreateMessage(
            NotificationType.starting_comp_level,
            """{"unknown":"shape"}""");

        SubscriptionEmbedding[] embeddings = await CollectEmbeddingsAsync(creator.CreateAsync(message));

        Assert.Empty(embeddings);
    }

    private static EmbedBuilderFactory CreateBuilderFactory(HttpClient httpClient)
    {
        EmbeddingColorizer colorizer = new(new FRCColors.Client(new TestHttpClientFactory(httpClient)), null);
        return new EmbedBuilderFactory(colorizer);
    }

    private static WebhookMessage CreateMessage(NotificationType messageType, string payloadJson)
    {
        using JsonDocument document = JsonDocument.Parse(payloadJson);
        return new WebhookMessage
        {
            MessageType = messageType,
            MessageData = document.RootElement.Clone()
        };
    }

    private static async Task<SubscriptionEmbedding[]> CollectEmbeddingsAsync(IAsyncEnumerable<SubscriptionEmbedding?> source)
    {
        List<SubscriptionEmbedding> results = [];
        await foreach (SubscriptionEmbedding? maybeEmbedding in source)
        {
            if (maybeEmbedding is not null)
            {
                results.Add(maybeEmbedding);
            }
        }

        return [.. results];
    }
}
