namespace FunctionApp.Tests;

using Discord;

using FunctionApp.DiscordInterop.Embeds;
using FunctionApp.TbaInterop.Models;
using FunctionApp.TbaInterop.Models.Notifications;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.Json;

[SuppressMessage("Performance", "EA0014:Add CancellationToken as the parameter of asynchronous method", Justification = "xUnit [Fact] methods do not support a CancellationToken parameter in this test setup.")]
public sealed class WebhookEmbeddingGeneratorTests
{
    [Fact]
    public async Task CreateEmbeddingsAsyncDispatchesToCreatorMatchingMessageType()
    {
        RecordingNotificationEmbedCreator scheduleCreator = new("schedule");
        RecordingNotificationEmbedCreator awardsCreator = new("awards");

        ServiceProvider services = new ServiceCollection()
            .AddKeyedSingleton<INotificationEmbedCreator>(NotificationType.schedule_updated.ToInvariantString(), scheduleCreator)
            .AddKeyedSingleton<INotificationEmbedCreator>(NotificationType.awards_posted.ToInvariantString(), awardsCreator)
            .BuildServiceProvider();

        WebhookEmbeddingGenerator generator = new(services, NullLogger<WebhookEmbeddingGenerator>.Instance);
        WebhookMessage message = CreateMessage(NotificationType.schedule_updated, """{"event_key":"2027cabl","event_name":"California Regional"}""");

        SubscriptionEmbedding[] embeddings = await CollectEmbeddingsAsync(generator.CreateEmbeddingsAsync(message, highlightTeam: 2046));

        Embed embed = Assert.Single(embeddings).Content;
        Assert.Equal("schedule", embed.Title);
        Assert.Equal(1, scheduleCreator.CallCount);
        Assert.Equal((ushort)2046, scheduleCreator.LastHighlightTeam);
        Assert.Equal(0, awardsCreator.CallCount);
    }

    [Fact]
    public async Task CreateEmbeddingsAsyncSkipsNullEmbeddingsFromCreator()
    {
        RecordingNotificationEmbedCreator creator = new("valid", emitNullBeforeValid: true);

        ServiceProvider services = new ServiceCollection()
            .AddKeyedSingleton<INotificationEmbedCreator>(NotificationType.schedule_updated.ToInvariantString(), creator)
            .BuildServiceProvider();

        WebhookEmbeddingGenerator generator = new(services, NullLogger<WebhookEmbeddingGenerator>.Instance);
        WebhookMessage message = CreateMessage(NotificationType.schedule_updated, """{"event_key":"2027cabl","event_name":"California Regional"}""");

        SubscriptionEmbedding[] embeddings = await CollectEmbeddingsAsync(generator.CreateEmbeddingsAsync(message));

        Embed embed = Assert.Single(embeddings).Content;
        Assert.Equal("valid", embed.Title);
    }

    [Fact]
    public async Task CreateEmbeddingsAsyncReturnsNoEmbeddingsWhenMessageTypeHasNoRegisteredCreator()
    {
        ServiceProvider services = new ServiceCollection().BuildServiceProvider();
        WebhookEmbeddingGenerator generator = new(services, NullLogger<WebhookEmbeddingGenerator>.Instance);
        WebhookMessage message = CreateMessage(NotificationType.schedule_updated, """{"event_key":"2027cabl","event_name":"California Regional"}""");

        SubscriptionEmbedding[] embeddings = await CollectEmbeddingsAsync(generator.CreateEmbeddingsAsync(message));

        Assert.Empty(embeddings);
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

    private static async Task<SubscriptionEmbedding[]> CollectEmbeddingsAsync(IAsyncEnumerable<SubscriptionEmbedding> source)
    {
        List<SubscriptionEmbedding> results = [];
        await foreach (SubscriptionEmbedding embedding in source)
        {
            results.Add(embedding);
        }

        return [.. results];
    }

    private sealed class RecordingNotificationEmbedCreator(string title, bool emitNullBeforeValid = false) : INotificationEmbedCreator
    {
        public int CallCount { get; private set; }

        public ushort? LastHighlightTeam { get; private set; }

        public async IAsyncEnumerable<SubscriptionEmbedding?> CreateAsync(WebhookMessage input, ushort? highlightTeam = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            CallCount++;
            LastHighlightTeam = highlightTeam;

            if (emitNullBeforeValid)
            {
                yield return null;
            }

            yield return new(new EmbedBuilder().WithTitle(title).Build());
            await Task.CompletedTask;
        }
    }
}
