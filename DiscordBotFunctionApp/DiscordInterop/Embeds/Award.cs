namespace DiscordBotFunctionApp.DiscordInterop.Embeds;

using Azure.Storage.Blobs;
using Azure.Storage.Sas;

using Common.Extensions;

using DiscordBotFunctionApp.Storage;
using DiscordBotFunctionApp.TbaInterop;
using DiscordBotFunctionApp.TbaInterop.Models;
using DiscordBotFunctionApp.TbaInterop.Models.Notifications;

using Microsoft.Extensions.Logging;

using System.Runtime.CompilerServices;

using TheBlueAlliance.Api;

internal sealed class Award(IEventApi tbaApi,
                            TeamRepository teams,
                            EmbedBuilderFactory builderFactory,
                            BlobContainerClient imageBlobs,
                            ILogger<Award> logger) : INotificationEmbedCreator
{
    public const NotificationType TargetType = NotificationType.awards_posted;

    public async IAsyncEnumerable<SubscriptionEmbedding?> CreateAsync(WebhookMessage msg, ushort? highlightTeam = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var baseBuilder = builderFactory.GetBuilder(highlightTeam);
        var notification = msg.GetDataAs<AwardsPosted>();
        if (notification == default)
        {
            logger.FailedToDeserializeNotificationDataAsNotificationType(TargetType);
            yield return null;
            yield break;
        }

        var tbaAwards = await tbaApi.GetEventAwardsAsync(notification.event_key, cancellationToken: cancellationToken).ConfigureAwait(false);
        var eventAwards = notification.awards?.Length is not null and not 0 ? notification.awards! : tbaAwards?.ToArray();
        if (eventAwards is null)
        {
            logger.FailedToRetrieveDetailedAwardsDataForEventKey(notification.event_key);
            yield return null;
            yield break;
        }

        if (eventAwards.Length is 0)
        {
            logger.NoAwardsFoundForEventKey(notification.event_key);
            yield return null;
            yield break;
        }

        if (highlightTeam is not null)
        {
            eventAwards = [.. eventAwards.Where(i => i.RecipientList.Any(j => j.TeamKey.TeamKeyToTeamNumber() == highlightTeam) is true)];
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
                    $"""
                        # Award!

                        ## {notification.event_name}: {latestAward.Name}

                        ### {(latestAward.RecipientList.Count > 1 ? "Recipients" : "Recipient")}

                        {string.Join("\n", latestAward.RecipientList!.Select(t => $"- {teams[t.TeamKey].GetLabelWithHighlight(highlightTeam)}{(!string.IsNullOrWhiteSpace(t.Awardee) ? $" [{t.Awardee}]" : string.Empty)}"))}
                
                        View more event awards [here](https://www.thebluealliance.com/event/{notification.event_key}#awards)
                    """)
                .WithThumbnailUrl(imageUri);

            yield return new(embedding.Build());
        }
    }
}
