namespace DiscordBotFunctionApp.DiscordInterop;

using Discord;
using Discord.Interactions;
using Discord.Net;

using DiscordBotFunctionApp.Storage;

using FIRST.Model;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

using TheBlueAlliance.Model.MatchExtensions;

using CompLevelEnum = TheBlueAlliance.Model.Match.CompLevelEnum;

internal sealed class AutoCompleteHandlers
{
    const ushort MAX_RESULTS = 25;  // Discord spec
    const ushort MAX_LENGTH = 97;   // 100, but we append '...' if it hits this

    [return: NotNullIfNotNull(nameof(val))]
    static string? Ellipsify(string? val) => val?.Length > MAX_LENGTH ? $"{val[..MAX_LENGTH]}..." : val;

    internal sealed class EventsAutoCompleteHandler : AutocompleteHandler
    {
        private ILogger<EventsAutoCompleteHandler>? _logger;

        public override Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
        {
            _logger ??= services.GetRequiredService<ILoggerFactory>().CreateLogger<EventsAutoCompleteHandler>();

            try
            {
                var userSearchString = autocompleteInteraction.Data.Current.Value as string ?? string.Empty;
                var eventsRepo = services.GetService<EventRepository>();
                Debug.Assert(eventsRepo is not null);
#pragma warning disable EA0011 // Consider removing unnecessary conditional access operator (?) - found instances where, even though decorated with [JsonRequired] and not nullable, values were coming through as `null`
                return Task.FromResult(AutocompletionResult.FromSuccess(
                    eventsRepo.AllEvents
                        .OrderByDescending(i => i.Value.StartDate > DateOnly.FromDateTime(TimeProvider.System.GetUtcNow().ToPacificTime().Date) ? DateOnly.MinValue : i.Value.StartDate)
                        .ThenBy(i => i.Value.ShortName)
                        .Where(i => i.Key.Contains(userSearchString, StringComparison.OrdinalIgnoreCase)
                            || i.Value.Name?.Contains(userSearchString, StringComparison.OrdinalIgnoreCase) is true
                            || i.Value.Year.ToString(CultureInfo.InvariantCulture).Contains(userSearchString, StringComparison.OrdinalIgnoreCase)
                            || i.Value.City?.Contains(userSearchString, StringComparison.OrdinalIgnoreCase) is true
                            || i.Value.Country?.Contains(userSearchString, StringComparison.OrdinalIgnoreCase) is true
                            || i.Value.StateProv?.Contains(userSearchString, StringComparison.OrdinalIgnoreCase) is true)
                        .Take(MAX_RESULTS)
                        .Select(i => new AutocompleteResult(Ellipsify(i.Value.GetLabel(includeYear: true, includeCity: true, includeStateProv: true, includeCountry: true)), i.Key))));
#pragma warning restore EA0011 // Consider removing unnecessary conditional access operator (?)
            }
            catch (Exception ex) when (ex is HttpException { DiscordCode: DiscordErrorCode.UnknownInteraction or DiscordErrorCode.InteractionHasAlreadyBeenAcknowledged }
            or InteractionException)
            {
                _logger.InteractionAlreadyAcknowledgedSkippingResponse();
            }

            return Task.FromResult(AutocompletionResult.FromSuccess());
        }
    }

    internal sealed class TeamsAutoCompleteHandler : AutocompleteHandler
    {
        private ILogger<TeamsAutoCompleteHandler>? _logger;
        public override Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
        {
            _logger ??= services.GetRequiredService<ILoggerFactory>().CreateLogger<TeamsAutoCompleteHandler>();

            try
            {
                var userSearchString = autocompleteInteraction.Data.Current.Value as string ?? string.Empty;
                var teamsRepo = services.GetService<TeamRepository>();
                Debug.Assert(teamsRepo is not null);
#pragma warning disable EA0011 // Consider removing unnecessary conditional access operator (?) - found instances where, even though decorated with [JsonRequired] and not nullable, values were coming through as `null`
                return Task.FromResult(AutocompletionResult.FromSuccess(
                    teamsRepo.AllTeams
                        .OrderBy(t => t.Value.TeamNumber)
                        .Where(i => i.Value.Name?.Contains(userSearchString, StringComparison.OrdinalIgnoreCase) is true
                            || i.Value.Nickname?.Contains(userSearchString, StringComparison.OrdinalIgnoreCase) is true
                            || i.Value.TeamNumber.ToString(CultureInfo.InvariantCulture).Contains(userSearchString, StringComparison.OrdinalIgnoreCase)
                            || i.Value.City?.Contains(userSearchString, StringComparison.OrdinalIgnoreCase) is true
                            || i.Value.Country?.Contains(userSearchString, StringComparison.OrdinalIgnoreCase) is true
                            || i.Value.StateProv?.Contains(userSearchString, StringComparison.OrdinalIgnoreCase) is true)
                        .Take(MAX_RESULTS)
                        .Select(i => new AutocompleteResult(Ellipsify(i.Value.GetLabel(asMarkdownLink: false, includeLocation: false)), i.Key))));
#pragma warning restore EA0011 // Consider removing unnecessary conditional access operator (?)
            }
            catch (Exception ex) when (ex is HttpException { DiscordCode: DiscordErrorCode.UnknownInteraction or DiscordErrorCode.InteractionHasAlreadyBeenAcknowledged }
            or InteractionException)
            {
                _logger.InteractionAlreadyAcknowledgedSkippingResponse();
            }

            return Task.FromResult(AutocompletionResult.FromSuccess());
        }
    }

    internal sealed class CompStageAutocompleteHandler : AutocompleteHandler
    {
        public override Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
        {
            return Task.FromResult(AutocompletionResult.FromSuccess([
                new AutocompleteResult("Qualifications", (int)CompLevelEnum.Qm),
                new AutocompleteResult("Playoffs/Eliminations", (int)CompLevelEnum.Sf),
                new AutocompleteResult("Finals", (int)CompLevelEnum.F),
            ]));
        }
    }
}
