namespace FunctionApp.DiscordInterop.Embeds;

using Azure.Storage.Blobs;
using Azure.Storage.Sas;

using Common.Extensions;

using Discord;

using FunctionApp.TbaInterop;
using FunctionApp.TbaInterop.Models;
using FunctionApp.TbaInterop.Models.Notifications;

using Microsoft.Extensions.Logging;

using System.Runtime.CompilerServices;
using System.Text.Encodings.Web;

using TheBlueAlliance.Api;
using TheBlueAlliance.Caching;

internal sealed class Award(IEventApi events,
                            ITeamApi teamApi,
                            TeamCache teams,
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

        var tbaAwards = await events.GetEventAwardsAsync(notification.event_key, cancellationToken: cancellationToken).ConfigureAwait(false);
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

        foreach (var award in eventAwards)
        {
            var thumbnailUri = ((AwardType)award.AwardType).IsBlueBanner() ? blueBannerImageUri : trophyImageUri;

            var embedUrl = $"https://www.thebluealliance.com/event/{notification.event_key}?name={UrlEncoder.Default.Encode(award.Name)}#awards";
            var embedding = baseBuilder
                .WithTitle(notification.event_name)
                .WithUrl(embedUrl)
                .WithDescription(
                    $"""
                    # Award!
                    ## {award.Name}
                    ### {(award.RecipientList.Count > 1 ? "Recipients" : "Recipient")}
                    {string.Join("\n", award.RecipientList!.Select(t => $"- {teams[t.TeamKey].GetLabelWithHighlight(highlightTeam)}{(!string.IsNullOrWhiteSpace(t.Awardee) ? $" [{t.Awardee}]" : string.Empty)}"))}
                    """)
                .WithThumbnailUrl(thumbnailUri);

            yield return new(embedding.Build());

            await foreach (var i in GetImagesForAwardAsync(award, cancellationToken))
            {
                // https://github.com/discord/discord-api-docs/discussions/3253#discussioncomment-953023
                yield return new(new EmbedBuilder()
                    .WithUrl(embedUrl)
                    .WithImageUrl(i)
                    .Build());
            }
        }
    }

    private async IAsyncEnumerable<string> GetImagesForAwardAsync(TheBlueAlliance.Model.Award award, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        foreach (var t in award.RecipientList
            .Where(i => !string.IsNullOrWhiteSpace(i.TeamKey))
            .Select(i => i.TeamKey)
            .Distinct())
        {
            var media = await teamApi.GetTeamMediaByYearAsync(t, award.Year, cancellationToken: cancellationToken).ConfigureAwait(false);

            var image = media?.FirstOrDefault(i => i.Preferred is true && !string.IsNullOrWhiteSpace(i.DirectUrl));
            if (image is not null)
            {
                yield return image.DirectUrl!;
            }
        }
    }
}
