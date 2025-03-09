namespace DiscordBotFunctionApp.DiscordInterop.Embeds;

using DiscordBotFunctionApp.TbaInterop;
using DiscordBotFunctionApp.TbaInterop.Models;
using DiscordBotFunctionApp.TbaInterop.Models.Notifications;

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

internal sealed class CompLevelStarting(EmbedBuilderFactory builderFactory) : INotificationEmbedCreator
{
    public const NotificationType TargetType = NotificationType.starting_comp_level;

    public async IAsyncEnumerable<SubscriptionEmbedding?> CreateAsync(WebhookMessage input, ushort? highlightTeam = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var notification = input.GetDataAs<TbaInterop.Models.Notifications.CompLevelStarting>();
        if (notification is null)
        {
            await Task.CompletedTask;
            yield break;
        }

        var compLevel = notification.comp_level;
        var eventName = notification.event_name;
        var embed = builderFactory.GetBuilder(highlightTeam)
            .WithDescription($"# {eventName} {Translator.CompLevelToLongString(compLevel)} are starting soon!\n\nScheduled start time: {DateTimeOffset.FromUnixTimeSeconds(notification.scheduled_time ?? 0).ToPacificTime():t}");

        yield return new(embed.Build());
    }
}
