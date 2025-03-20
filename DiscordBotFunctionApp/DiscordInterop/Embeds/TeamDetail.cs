namespace DiscordBotFunctionApp.DiscordInterop.Embeds;

using Common.Extensions;

using Discord;

using DiscordBotFunctionApp.Apis;
using DiscordBotFunctionApp.Storage;

using Microsoft.Extensions.Logging;

using Statbotics.Model;

using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

using TheBlueAlliance.Api;

internal sealed class TeamDetail(RESTCountries _countryCodeLookup,
                                 EmbedBuilderFactory builderFactory,
                                 TeamRepository _teamsRepo,
                                 ITeamApi tbaTeamApi,
                                 Statbotics.Api.ITeamApi teamStats,
                                 IDistrictApi districts,
                                 TimeProvider time,
                                 ILogger<TeamDetail> logger) : IEmbedCreator<string>
{
    public async IAsyncEnumerable<ResponseEmbedding?> CreateAsync(string teamKey, ushort? highlightTeam = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using var scope = logger.CreateMethodScope();

        var teamDetails = _teamsRepo[teamKey];

        var jsonResult = JsonSerializer.Serialize(await teamStats.ReadTeamV3TeamTeamGetAsync(teamKey.TeamKeyToTeamNumber()!.ToString()!, cancellationToken).ConfigureAwait(false));
        var teamResult = JsonSerializer.Deserialize<Team>(jsonResult)!;
        var locationString = await createLocationStringAsync(teamDetails, _countryCodeLookup).ConfigureAwait(false);
        var imageUrl = (await tbaTeamApi.GetTeamMediaByYearAsync(teamKey, time.GetUtcNow().Year, cancellationToken: cancellationToken).ConfigureAwait(false))?
            .FirstOrDefault(i => !string.IsNullOrWhiteSpace(i.DirectUrl));
        var builder = builderFactory.GetBuilder()
            .WithTitle($"**{teamDetails.Nickname}**")
            .WithUrl($"{teamDetails.Website}#{teamKey}")
            .WithThumbnailUrl($"https://www.thebluealliance.com/avatar/{time.GetLocalNow().Year}/{teamKey}.png")
            .WithDescription(teamDetails.Name)
            .WithImageUrl(imageUrl?.DirectUrl)
            .AddField("Location", locationString)
            .AddField("Active?", teamResult.Active ? "Yes" : "No");

        var lightestColor = Utility.GetLightestColorOf(
            teamResult.Colors?.Primary is not null ? Color.Parse(teamResult.Colors.Primary) : null,
            teamResult.Colors?.Secondary is not null ? Color.Parse(teamResult.Colors.Secondary) : null);
        if (lightestColor is not null)
        {
            builder.WithColor(lightestColor.Value);
        }

        var district = (await districts.GetTeamDistrictsAsync(teamKey, cancellationToken: cancellationToken).ConfigureAwait(false))?.FirstOrDefault();
        if (district is not null && !string.IsNullOrWhiteSpace(district.DisplayName))
        {
            builder.AddField("District", district.DisplayName);
        }

        if (!string.IsNullOrWhiteSpace(teamDetails.SchoolName))
        {
            builder.AddField("School", teamDetails.SchoolName);
        }

        if (teamDetails.RookieYear.HasValue)
        {
            builder.AddField("Rookie Year", $"{teamDetails.RookieYear}");
        }

        var fullRecord = teamResult.Records?.Full;
        if (fullRecord is not null)
        {
            builder.AddField("All-time Record", $"{fullRecord.Wins}-{fullRecord.Losses}-{fullRecord.Ties} ({fullRecord.Wins / ((float)fullRecord.Wins + fullRecord.Losses + fullRecord.Ties):.000})");
        }

        yield return new(builder.Build());

        async static Task<string> createLocationStringAsync(TheBlueAlliance.Model.Team teamDetail, RESTCountries _countryCodeLookup)
        {
            StringBuilder sb = new();
            if (!string.IsNullOrWhiteSpace(teamDetail.LocationName))
            {
                sb.Append($"{teamDetail.LocationName}, ");
            }

            if (!string.IsNullOrWhiteSpace(teamDetail.City))
            {
                sb.Append($"{teamDetail.City}, ");
            }

            if (!string.IsNullOrWhiteSpace(teamDetail.StateProv))
            {
                sb.Append($"{teamDetail.StateProv}, ");
            }

            if (!string.IsNullOrWhiteSpace(teamDetail.Country))
            {
                sb.Append($"{teamDetail.Country}");

                var countryCode = (await _countryCodeLookup.GetCountryCodeForFlagLookupAsync(teamDetail.Country, default).ConfigureAwait(false))!;
                if (!string.IsNullOrWhiteSpace(countryCode))
                {
                    sb.Append($" {Utility.CreateCountryFlagEmojiRef(countryCode)}");
                }
            }

            return sb.ToString();
        }
    }
}
