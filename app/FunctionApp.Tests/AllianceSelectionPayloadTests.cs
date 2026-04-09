namespace FunctionApp.Tests;

using FunctionApp.DiscordInterop.Embeds;
using FunctionApp.TbaInterop.Models;
using FunctionApp.TbaInterop.Models.Notifications;

using System.Text.Json;

using AllianceSelectionEmbed = FunctionApp.DiscordInterop.Embeds.AllianceSelection;
using AllianceSelectionNotification = FunctionApp.TbaInterop.Models.Notifications.AllianceSelection;

public sealed class AllianceSelectionPayloadTests
{
    [Fact]
    public void ResolveEventKeyUsesDirectEventKeyWhenPresent()
    {
        WebhookMessage message = CreateMessage(
            NotificationType.alliance_selection,
            """{"event_key":"2027cabl","event_name":"California Regional"}""");

        var notification = message.GetDataAs<AllianceSelectionNotification>();
        var eventKey = AllianceSelectionEmbed.ResolveEventKey(notification);

        Assert.Equal("2027cabl", eventKey);
    }

    [Fact]
    public void ResolveEventKeyFallsBackToNestedEventObjectKey()
    {
        WebhookMessage message = CreateMessage(
            NotificationType.alliance_selection,
            """{"event":{"key":"2028mitry","name":"Michigan District Troy Event"}}""");

        var notification = message.GetDataAs<AllianceSelectionNotification>();
        var eventKey = AllianceSelectionEmbed.ResolveEventKey(notification);

        Assert.Equal("2028mitry", eventKey);
    }

    [Fact]
    public void ResolveEventNameFallsBackToNestedEventObjectName()
    {
        WebhookMessage message = CreateMessage(
            NotificationType.alliance_selection,
            """{"event":{"key":"2028mitry","name":"Michigan District Troy Event"}}""");

        var notification = message.GetDataAs<AllianceSelectionNotification>();
        string eventName = AllianceSelectionEmbed.ResolveEventName(notification, "2028mitry");

        Assert.Equal("Michigan District Troy Event", eventName);
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
}
