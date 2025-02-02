﻿namespace DiscordBotFunctionApp.DiscordInterop.Embeds;

using Common.Extensions;

using DiscordBotFunctionApp.Apis;
using DiscordBotFunctionApp.Storage;

using Microsoft.Extensions.Logging;

using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

using TheBlueAlliance.Api;

internal sealed class TeamDetail(RESTCountries _countryCodeLookup, EmbedBuilderFactory builderFactory, TeamRepository _teamsRepo, Statbotics.Api.ITeamApi teamStats, IDistrictApi districts, ILogger<TeamDetail> logger) : IEmbedCreator<string>
{
    public async IAsyncEnumerable<ResponseEmbedding> CreateAsync(string teamKey, ushort? highlightTeam = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using var scope = logger.CreateMethodScope();

        if ((await _teamsRepo.GetTeamsAsync(default).ConfigureAwait(false)).TryGetValue(teamKey, out var teamDetails) && teamDetails is not null)
        {
            var jsonResult = JsonSerializer.Serialize(await teamStats.ReadTeamV3TeamTeamGetAsync(teamKey.ToTeamNumber().ToString(), cancellationToken).ConfigureAwait(false));
            var teamResult = JsonSerializer.Deserialize<StatboticsInterop.Models.Team>(jsonResult)!;
            var locationString = await createLocationStringAsync(teamDetails, _countryCodeLookup).ConfigureAwait(false);
            var builder = builderFactory.GetBuilder()
                .WithTitle($"**{teamDetails.Nickname}**")
                .WithUrl(teamDetails.Website)
                .WithThumbnailUrl($"https://www.thebluealliance.com/avatar/{TimeProvider.System.GetLocalNow().Year - 1}/{teamKey}.png")
                .WithDescription(teamDetails.Name)
                .AddField("Location", locationString)
                .AddField("Active?", teamResult.Active ? "Yes" : "No");

            var district = (await districts.GetTeamDistrictsAsync(teamKey, cancellationToken: cancellationToken).ConfigureAwait(false))?.FirstOrDefault();
            if (district is not null && !string.IsNullOrWhiteSpace(district.DisplayName))
            {
                builder.AddField("District", district.DisplayName);
            }

            if (!string.IsNullOrWhiteSpace(teamDetails.SchoolName))
            {
                builder.AddField("School", teamDetails.SchoolName);
            }

            builder
                .AddField("Rookie Year", $"{teamDetails.RookieYear}")
                .AddField("All-time Record", $"{teamResult.Records.Full.Wins}-{teamResult.Records.Full.Losses}-{teamResult.Records.Full.Ties} ({teamResult.Records.Full.Wins / ((double)teamResult.Records.Full.Wins + teamResult.Records.Full.Losses + teamResult.Records.Full.Ties):.000})");

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
}
