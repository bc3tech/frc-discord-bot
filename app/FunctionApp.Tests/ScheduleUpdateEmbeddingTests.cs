namespace FunctionApp.Tests;

using Discord;

using FunctionApp.DiscordInterop.Embeds;
using FunctionApp.TbaInterop.Models;
using FunctionApp.TbaInterop.Models.Notifications;

using Microsoft.Extensions.Logging.Abstractions;

using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

using ScheduleUpdateEmbed = FunctionApp.DiscordInterop.Embeds.ScheduleUpdate;

[SuppressMessage("Performance", "EA0014:Add CancellationToken as the parameter of asynchronous method", Justification = "xUnit [Fact]/[Theory] methods do not support a CancellationToken parameter in this test setup.")]
public sealed class ScheduleUpdateEmbeddingTests
{
    [Theory]
    [InlineData("2026cabl", "California Regional")]
    [InlineData("2027cabl", "California Regional")]
    public async Task CreateAsyncBuildsExpectedEmbedForCanonicalSeasonPayload(string eventKey, string eventName)
    {
        using HttpClient httpClient = new();
        ScheduleUpdateEmbed creator = new(CreateBuilderFactory(httpClient), NullLogger<ScheduleUpdateEmbed>.Instance);
        WebhookMessage message = CreateMessage(
            NotificationType.schedule_updated,
            $$"""{"event_key":"{{eventKey}}","event_name":"{{eventName}}","first_match_time":1744232100}""");

        SubscriptionEmbedding[] embeddings = await CollectEmbeddingsAsync(creator.CreateAsync(message));

        Embed embed = Assert.Single(embeddings).Content;
        Assert.Equal($"📢{eventName} Schedule Update⏰", embed.Title);
        Assert.Equal($"https://www.thebluealliance.com/event/{eventKey}", embed.Url);
        Assert.Equal("Click for details", embed.Description);
    }

    [Fact]
    public async Task CreateAsyncBuildsEmbedWhenEventDetailsArriveUnderNestedEventObject()
    {
        using HttpClient httpClient = new();
        ScheduleUpdateEmbed creator = new(CreateBuilderFactory(httpClient), NullLogger<ScheduleUpdateEmbed>.Instance);
        WebhookMessage message = CreateMessage(
            NotificationType.schedule_updated,
            """{"first_match_time":1744232100,"event":{"key":"2028mitry","name":"Michigan District Troy Event"}}""");

        SubscriptionEmbedding[] embeddings = await CollectEmbeddingsAsync(creator.CreateAsync(message));

        Embed embed = Assert.Single(embeddings).Content;
        Assert.Equal("📢Michigan District Troy Event Schedule Update⏰", embed.Title);
        Assert.Equal("https://www.thebluealliance.com/event/2028mitry", embed.Url);
    }

    [Fact]
    public async Task CreateAsyncReturnsNoEmbeddingsWhenEventDetailsCannotBeResolved()
    {
        using HttpClient httpClient = new();
        ScheduleUpdateEmbed creator = new(CreateBuilderFactory(httpClient), NullLogger<ScheduleUpdateEmbed>.Instance);
        WebhookMessage message = CreateMessage(
            NotificationType.schedule_updated,
            """{"first_match_time":1744232100,"event":{"short_name":""}}""");

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
