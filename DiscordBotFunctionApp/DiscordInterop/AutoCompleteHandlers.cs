namespace DiscordBotFunctionApp.DiscordInterop;

using Common.Extensions;
using Common.Tba.Api.Models;

using Discord;
using Discord.Interactions;

using DiscordBotFunctionApp.Storage;

using Microsoft.Extensions.DependencyInjection;

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

internal sealed class AutoCompleteHandlers
{
    const ushort MAX_RESULTS = 25;  // Discord spec
    const ushort MAX_LENGTH = 97;   // 100, but we append '...' if it hits this

    [return: NotNullIfNotNull(nameof(val))]
    static string? Ellipsify(string? val) => val?.Length > MAX_LENGTH ? $"{val.Take(MAX_LENGTH)}..." : val;

    public sealed class EventsAutoCompleteHandler : AutocompleteHandler
    {
        public async override Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
        {
            var userSearchString = autocompleteInteraction.Data.Current.Value as string ?? string.Empty;
            var eventsRepo = services.GetService<EventRepository>();
            Debug.Assert(eventsRepo is not null);
            var frcEvents = await eventsRepo.GetEventsAsync(default).ConfigureAwait(false);
            return AutocompletionResult.FromSuccess(
                frcEvents.Where(i => i.Key.Contains(userSearchString, StringComparison.OrdinalIgnoreCase)
                    || i.Value.Name?.Contains(userSearchString, StringComparison.OrdinalIgnoreCase) is true
                    || i.Value.Year?.ToString(CultureInfo.InvariantCulture).Contains(userSearchString, StringComparison.OrdinalIgnoreCase) is true
                    || i.Value.City?.Contains(userSearchString, StringComparison.OrdinalIgnoreCase) is true
                    || i.Value.Country?.Contains(userSearchString, StringComparison.OrdinalIgnoreCase) is true
                    || i.Value.StateProv?.Contains(userSearchString, StringComparison.OrdinalIgnoreCase) is true
                    || i.Value.LocationName?.Contains(userSearchString, StringComparison.OrdinalIgnoreCase) is true)
                .Take(MAX_RESULTS)
                .Select(i => new AutocompleteResult(Ellipsify(eventsRepo.GetLabelForEvent(i.Key)), i.Key)));
        }
    }

    internal sealed class TeamsAutoCompleteHandler : AutocompleteHandler
    {
        public override async Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
        {
            var userSearchString = autocompleteInteraction.Data.Current.Value as string ?? string.Empty;
            var teamsRepo = services.GetService<TeamRepository>();
            Debug.Assert(teamsRepo is not null);
            var frcEvents = await teamsRepo.GetTeamsAsync(default).ConfigureAwait(false);
            return AutocompletionResult.FromSuccess(frcEvents
                .Where(i => i.Value.Name?.Contains(userSearchString, StringComparison.OrdinalIgnoreCase) is true
                    || i.Value.Nickname?.Contains(userSearchString, StringComparison.OrdinalIgnoreCase) is true
                    || i.Value.TeamNumber?.ToString(CultureInfo.InvariantCulture).Contains(userSearchString, StringComparison.OrdinalIgnoreCase) is true
                    || i.Value.City?.Contains(userSearchString, StringComparison.OrdinalIgnoreCase) is true
                    || i.Value.Country?.Contains(userSearchString, StringComparison.OrdinalIgnoreCase) is true
                    || i.Value.StateProv?.Contains(userSearchString, StringComparison.OrdinalIgnoreCase) is true)
                .Take(MAX_RESULTS)
                .Select(i => new AutocompleteResult(Ellipsify(teamsRepo.GetLabelForTeam(i.Key)), i.Key.ToTeamNumber())));
        }
    }
}
