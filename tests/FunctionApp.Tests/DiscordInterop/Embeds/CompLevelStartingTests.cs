namespace FunctionApp.Tests.DiscordInterop.Embeds;
using FunctionApp.DiscordInterop.Embeds;
using FunctionApp.TbaInterop.Models;
using FunctionApp.TbaInterop.Models.Notifications;

using Microsoft.Extensions.Logging;

using Moq;

using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

using Xunit;
using Xunit.Abstractions;

using CompLevelStarting = FunctionApp.DiscordInterop.Embeds.CompLevelStarting;

public class CompLevelStartingTests : EmbeddingTest
{
    private readonly CompLevelStarting _compLevelStarting;

    public CompLevelStartingTests(ITestOutputHelper outputHelper) : base(typeof(CompLevelStarting), outputHelper) => _compLevelStarting = this.Mocker.CreateInstance<CompLevelStarting>();

    [Fact]
    public async Task CreateAsync_ShouldReturnNull_WhenNotificationIsDefault()
    {
        // Arrange
        var webhookMessage = new WebhookMessage
        {
            MessageType = NotificationType.starting_comp_level,
            MessageData = default
        };

        // Act
        var result = await _compLevelStarting.CreateAsync(webhookMessage).ToListAsync();

        // Assert
        Assert.Single(result);
        Assert.Null(result.First());
        this.Logger.Verify(LogLevel.Warning, "Failed to deserialize notification data as starting_comp_level");
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnEmbed_WhenNotificationIsValid()
    {
        // Arrange
        var webhookMessage = JsonSerializer.Deserialize<WebhookMessage>("""
            {"message_type":"starting_comp_level","message_data":{"comp_level":"qm","event_key":"2025ohmv","event_name":"Miami Valley Regional","scheduled_time":1743166500},"IsBroadcast":true}
            """)!;

        // Act
        var result = await _compLevelStarting.CreateAsync(webhookMessage).ToListAsync();

        // Assert
        Assert.Single(result);
        var embed = result.First();
        Assert.NotNull(embed);
        Assert.Contains("⌚Scheduled start time", embed.Content.Description);
    }
}
