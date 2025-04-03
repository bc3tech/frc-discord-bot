namespace FunctionApp.Tests.DiscordInterop.Embeds;

using FunctionApp.DiscordInterop.Embeds;
using FunctionApp.TbaInterop.Models;
using FunctionApp.TbaInterop.Models.Notifications;

using Microsoft.Extensions.Logging;

using Moq;

using System.Reactive;
using System.Text.Json;
using System.Threading.Tasks;

using Xunit;
using Xunit.Abstractions;

using static System.Runtime.InteropServices.JavaScript.JSType;

using ScheduleUpdate = FunctionApp.DiscordInterop.Embeds.ScheduleUpdate;

public class ScheduleUpdateTests : EmbeddingTest
{
    private readonly ScheduleUpdate _scheduleUpdate;

    public ScheduleUpdateTests(ITestOutputHelper outputHelper) : base(typeof(ScheduleUpdate), outputHelper) => _scheduleUpdate = this.Mocker.CreateInstance<ScheduleUpdate>();

    private static WebhookMessage CreateWebhookMessage(string json) => JsonSerializer.Deserialize<WebhookMessage>(json)!;

    [Fact]
    public async Task CreateAsync_ValidNotification_ReturnsEmbedding()
    {
        var json = "{\"message_type\": \"schedule_updated\", \"message_data\": {\"event_name\": \"Test Event\", \"event_key\": \"2022test\"}}";
        var msg = CreateWebhookMessage(json);
        var embedding = await _scheduleUpdate.CreateAsync(msg).FirstOrDefaultAsync();

        Assert.NotNull(embedding);
        Assert.Contains("Test Event Schedule Update", embedding!.Content.Title);
        Assert.Contains("https://www.thebluealliance.com/event/2022test", embedding.Content.Url);
    }

    [Fact]
    public async Task CreateAsync_InvalidNotification_LogsAndReturnsNull()
    {
        var json = "{\"message_type\": \"schedule_updated\", \"message_data\": {}}";
        var msg = CreateWebhookMessage(json);
        var embedding = await _scheduleUpdate.CreateAsync(msg).FirstOrDefaultAsync();

        Assert.Null(embedding);
        this.Logger.Verify(LogLevel.Warning, "Failed to deserialize notification data as schedule_updated");
    }
}