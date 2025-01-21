namespace DiscordBotFunctionApp.DiscordInterop.Embeds;

using Azure.Storage.Blobs;
using Azure.Storage.Sas;

using Common.Extensions;

using Discord;

using DiscordBotFunctionApp.Storage;

using Microsoft.Extensions.Logging;

using System.Runtime.CompilerServices;
using System.Text.Json;

using TheBlueAlliance.Api;
using TheBlueAlliance.Api.Api;
using TheBlueAlliance.Api.Model;
using TheBlueAlliance.Api.Notifications;

internal sealed class Award(IEventApi tbaApi, BlobContainerClient imageBlobs, EmbedBuilderFactory builderFactory, TeamRepository teams, ILogger<Award> logger) : IEmbedCreator
{
    public static NotificationType TargetType { get; } = NotificationType.awards_posted;

    public async IAsyncEnumerable<Embed> CreateAsync(WebhookMessage msg, ushort? highlightTeam = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var baseBuilder = builderFactory.GetBuilder();
        var notification = JsonSerializer.Deserialize<AwardsPosted>(msg.MessageData);
        if (notification is null)
        {
            logger.LogWarning("Failed to deserialize notification data as {NotificationType}", TargetType);
            yield return baseBuilder.Build();
            yield break;
        }

        var eventAwards = await tbaApi.GetEventAwardsAsync(notification.event_key, cancellationToken: cancellationToken).ConfigureAwait(false);
        if (eventAwards is null)
        {
            logger.LogWarning("Failed to retrieve detailed awards data for {EventKey}", notification.event_key);
            yield return baseBuilder.Build();
            yield break;
        }

        if (eventAwards.Count is 0)
        {
            logger.LogWarning("No awards found for {EventKey}", notification.event_key);
            yield return baseBuilder.Build();
            yield break;
        }

        if (highlightTeam is not null)
        {
            eventAwards = [.. eventAwards.Where(i => i.RecipientList.Any(j => j.TeamKey.ToTeamNumber() == highlightTeam) is true)];
        }

        var trophyBlob = imageBlobs.GetBlobClient("trophy.png");
        var trophyImageUri = imageBlobs.CanGenerateSasUri
            ? trophyBlob.GenerateSasUri(BlobSasPermissions.Read, DateTimeOffset.MaxValue).ToString()
            : trophyBlob.Uri.ToString();
        var blueBannerBlob = imageBlobs.GetBlobClient("bluebanner.png");
        var blueBannerImageUri = imageBlobs.CanGenerateSasUri
            ? blueBannerBlob.GenerateSasUri(BlobSasPermissions.Read, DateTimeOffset.MaxValue).ToString()
            : blueBannerBlob.Uri.ToString();

        foreach (var latestAward in eventAwards)
        {
            var imageUri = ((AwardType)latestAward.AwardType).IsBlueBanner() ? blueBannerImageUri : trophyImageUri;

            var embedding = baseBuilder
                .WithDescription(
    $@"# Award!
## {latestAward.Name}
### {(latestAward.RecipientList.Length > 1 ? "Recipients" : "Recipient")}
{string.Join("\n", latestAward.RecipientList!
        .Select(t => $"- {teams.GetTeamLabelWithHighlight(t.TeamKey, highlightTeam)}{(!string.IsNullOrWhiteSpace(t.Awardee) ? $" [{t.Awardee}]" : string.Empty)}"))}

View more event awards [here](https://www.thebluealliance.com/event/{notification.event_key}#awards)
")
                .WithThumbnailUrl(imageUri)
                .Build();

            yield return embedding;
        }
    }
}
