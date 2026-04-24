namespace ChatBot.Tools;

using BC3Technologies.DiscordGpt.Core;

using Common;
using Common.Extensions;

using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

internal sealed partial class MealSignupInfoTool(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<MealSignupInfoTool> logger)
    : HttpGetToolBase(httpClientFactory, logger), IDiscordTool
{
    private const string ToolName = "fetch_meal_signup_info";
    private const string ToolDescription = "Fetch Bear Metal meal signup data from SignupGenius. Use this for meal-signup questions: who signed up, what food is needed, open slots, quantities, dates, delivery times, comments. ALWAYS narrow the request with parameters — the full season has 100+ slots. Use 'today' for tonight's dinner, 'this_week' for the current week, 'upcoming' (default) for the next few signups. Combine with participantNameFilter to find a specific person's signups.";

    private const int DefaultLimit = 10;
    private const int MaxLimit = 50;

    private const string DateFilterToday = "today";
    private const string DateFilterTomorrow = "tomorrow";
    private const string DateFilterThisWeek = "this_week";
    private const string DateFilterUpcoming = "upcoming";
    private const string DateFilterPast = "past";
    private const string DateFilterAll = "all";

    private readonly string _mealSignupGeniusId = Throws.IfNullOrWhiteSpace(configuration[ChatBotConstants.Configuration.Foundry.MealSignupGeniusId]);

    public override IReadOnlyList<AIFunction> Functions => field ??=
        [
            AIFunctionFactory.Create(
                FetchMealSignupInfoResponseBodyAsync,
                CreateSkippableFunctionOptions(ToolName, ToolDescription)),
        ];

    public override IReadOnlyList<string> ToolNames => [ToolName];

    public string Name => ToolName;

    public string Description => ToolDescription;

    public AIFunction AsFunction()
        => AIFunctionFactory.Create(
            FetchMealSignupInfoResponseBodyAsync,
            CreateSkippableFunctionOptions(ToolName, ToolDescription));

    [Description("Fetches Bear Metal meal signup data from SignupGenius, narrowed by the supplied filters. The full season has 100+ slots — ALWAYS pass a date scope appropriate to the question (e.g., 'today' for tonight, 'this_week' for the current week). Returns slots in chronological order with each slot's items, participants, and open count.")]
    public async Task<string> FetchMealSignupInfoResponseBodyAsync(
        [Description("Date scope for the returned slots. One of: 'today' (slots starting today), 'tomorrow', 'this_week' (today through the upcoming Sunday), 'upcoming' (today and all future, default), 'past' (slots already started), 'all' (no date filter). Use 'today' for 'tonight'/'today's' questions; use 'upcoming' for 'next' questions.")]
        string? dateFilter = DateFilterUpcoming,
        [Description("Optional case-insensitive substring match against the slot's meal label ('Lunch', 'Dinner', or 'Meal at h:mm tt'). Pass 'dinner' to limit results to dinner signups, etc. Leave null/empty to include all meal types.")]
        string? mealFilter = null,
        [Description("Optional case-insensitive substring match against participant names. Use this for first-person queries — pass the User Display Name to find slots that person signed up for. Returns slots where at least one participant name contains this value.")]
        string? participantNameFilter = null,
        [Description("Maximum number of slots to return after filtering. Default 10, max 50. Use a small value when answering a specific question; raise it when listing multiple weeks.")]
        int limit = DefaultLimit,
        CancellationToken cancellationToken = default)
    {
        using HttpRequestMessage request = CreateSignupGeniusRequest();
        Uri? sourcePage = new(string.Format(default, SignupGeniusReferrerUrlCompositeFormat, _mealSignupGeniusId));
        using HttpResponseMessage response = await this.HttpClientFactory
            .CreateClient(ChatBotConstants.HttpClients.MealSignupInfo)
            .SendAsync(request, cancellationToken)
            .ConfigureAwait(false);

        string content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        JsonElement? data = null;
        string? text = null;
        if (!string.IsNullOrWhiteSpace(content))
        {
            try
            {
                using JsonDocument document = JsonDocument.Parse(content);
                data = JsonSerializer.SerializeToElement(
                    BuildMealSignupSummary(
                        document.RootElement,
                        dateFilter,
                        mealFilter,
                        participantNameFilter,
                        limit));
            }
            catch (JsonException)
            {
                text = content;
            }
        }

        response.EnsureSuccessStatusCode();

        return SerializeToolResponse(
            request.RequestUri?.OriginalString,
            (int)response.StatusCode,
            response.IsSuccessStatusCode,
            data,
            text,
            null,
            [new CitationLink("SignupGenius", sourcePage.ToString())]);
    }

    private static object? BuildMealSignupSummary(
        JsonElement root,
        string? dateFilter,
        string? mealFilter,
        string? participantNameFilter,
        int limit)
    {
        if (!root.TryGetProperty("DATA", out JsonElement dataElement)
            || !dataElement.TryGetProperty("slots", out JsonElement slotsElement)
            || slotsElement.ValueKind is not JsonValueKind.Object)
        {
            return null;
        }

        JsonElement participantsElement = dataElement.TryGetProperty("participants", out JsonElement participants)
            ? participants
            : default;

        string normalizedDateFilter = (dateFilter ?? DateFilterUpcoming).Trim().ToLowerInvariant();
        if (string.IsNullOrEmpty(normalizedDateFilter))
        {
            normalizedDateFilter = DateFilterUpcoming;
        }

        string? normalizedMealFilter = mealFilter.UnlessNullOrWhitespaceThen(null)?.Trim();
        string? normalizedParticipantFilter = participantNameFilter.UnlessNullOrWhitespaceThen(null)?.Trim();
        int effectiveLimit = limit <= 0 ? DefaultLimit : Math.Min(limit, MaxLimit);

        DateTime now = DateTime.Now;
        DateTime today = now.Date;

        List<(DateTime StartTime, object Slot, bool HasParticipantMatch)> projected = [];
        int totalSlotCount = 0;
        foreach (JsonProperty slotProperty in slotsElement.EnumerateObject())
        {
            JsonElement slot = slotProperty.Value;
            if (slot.ValueKind is not JsonValueKind.Object
                || !TryParseSignupTime(GetStringProperty(slot, "starttime"), out DateTime startTime))
            {
                continue;
            }

            totalSlotCount++;

            string location = GetStringProperty(slot, "location").UnlessNullOrWhitespaceThen("Unspecified location");
            string mealLabel = GetMealLabel(startTime);

            List<object> items = [];
            int participantCount = 0;
            int openSlotCount = 0;
            bool hasParticipantMatch = false;
            if (TryGetObjectProperty(slot, "items", out JsonElement itemsElement) && itemsElement.ValueKind is JsonValueKind.Array)
            {
                foreach (JsonElement item in itemsElement.EnumerateArray())
                {
                    if (item.ValueKind is not JsonValueKind.Object)
                    {
                        continue;
                    }

                    int quantity = GetInt32Property(item, "qty");
                    int quantityTaken = GetInt32Property(item, "qtyTaken");
                    int participantCountForItem = GetInt32Property(item, "participantCount");

                    string? slotItemId = GetScalarPropertyText(item, "slotitemid");
                    List<object> participantSummaries = BuildParticipantSummaries(participantsElement, slotItemId);

                    if (!hasParticipantMatch && normalizedParticipantFilter is not null)
                    {
                        hasParticipantMatch = AnyParticipantNameMatches(
                            participantsElement,
                            slotItemId,
                            normalizedParticipantFilter);
                    }

                    participantCount += Math.Max(participantSummaries.Count, participantCountForItem);
                    openSlotCount += Math.Max(quantity - quantityTaken, 0);

                    items.Add(new
                    {
                        slotItemId = GetScalarPropertyText(item, "slotitemid"),
                        label = GetStringProperty(item, "item"),
                        note = GetStringProperty(item, "itemcomment").UnlessNullOrWhitespaceThen(null),
                        quantity,
                        quantityTaken,
                        openSlots = Math.Max(quantity - quantityTaken, 0),
                        participants = participantSummaries,
                    });
                }
            }

            if (!MatchesDateFilter(startTime, today, normalizedDateFilter))
            {
                continue;
            }

            if (normalizedMealFilter is not null
                && !mealLabel.Contains(normalizedMealFilter, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (normalizedParticipantFilter is not null && !hasParticipantMatch)
            {
                continue;
            }

            object slotRecord = new
            {
                slotId = GetScalarPropertyText(slot, "slotid") ?? slotProperty.Name,
                startTime = GetStringProperty(slot, "starttime"),
                localDate = startTime.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                localDayOfWeek = startTime.ToString("dddd", CultureInfo.InvariantCulture),
                localTime = startTime.ToString("h:mm tt", CultureInfo.InvariantCulture),
                mealLabel,
                location,
                slotNote = GetStringProperty(slot, "comments").UnlessNullOrWhitespaceThen(null),
                participantCount,
                openSlotCount,
                items,
            };

            projected.Add((startTime, slotRecord, hasParticipantMatch));
        }

        List<object> filteredSlots = projected
            .OrderBy(static x => x.StartTime)
            .Select(static x => x.Slot)
            .ToList();

        int matchedCount = filteredSlots.Count;
        bool truncated = matchedCount > effectiveLimit;
        if (truncated)
        {
            filteredSlots = filteredSlots.Take(effectiveLimit).ToList();
        }

        return new
        {
            referenceTime = now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
            appliedFilters = new
            {
                dateFilter = normalizedDateFilter,
                mealFilter = normalizedMealFilter,
                participantNameFilter = normalizedParticipantFilter,
                limit = effectiveLimit,
            },
            totalSlotCount,
            matchedSlotCount = matchedCount,
            returnedSlotCount = filteredSlots.Count,
            truncated,
            slots = filteredSlots,
        };
    }

    private static bool MatchesDateFilter(DateTime startTime, DateTime today, string dateFilter)
    {
        DateTime startDate = startTime.Date;
        return dateFilter switch
        {
            DateFilterAll => true,
            DateFilterToday => startDate == today,
            DateFilterTomorrow => startDate == today.AddDays(1),
            DateFilterThisWeek => startDate >= today && startDate <= EndOfCurrentWeek(today),
            DateFilterPast => startTime < DateTime.Now,
            DateFilterUpcoming => startDate >= today,
            _ => startDate >= today,
        };
    }

    private static DateTime EndOfCurrentWeek(DateTime today)
    {
        // Week ends Sunday (DayOfWeek.Sunday == 0). Distance to next Sunday inclusive.
        int daysUntilSunday = ((int)DayOfWeek.Sunday - (int)today.DayOfWeek + 7) % 7;
        return today.AddDays(daysUntilSunday);
    }

    private static bool AnyParticipantNameMatches(JsonElement participantsElement, string? slotItemId, string nameFilter)
    {
        if (string.IsNullOrWhiteSpace(slotItemId)
            || participantsElement.ValueKind is not JsonValueKind.Object
            || !participantsElement.TryGetProperty(slotItemId, out JsonElement participantElement))
        {
            return false;
        }

        if (participantElement.ValueKind is JsonValueKind.Array)
        {
            foreach (JsonElement participant in participantElement.EnumerateArray())
            {
                if (participant.ValueKind is JsonValueKind.Object
                    && ParticipantNameContains(participant, nameFilter))
                {
                    return true;
                }
            }

            return false;
        }

        return participantElement.ValueKind is JsonValueKind.Object
            && ParticipantNameContains(participantElement, nameFilter);
    }

    private static bool ParticipantNameContains(JsonElement participant, string nameFilter)
    {
        string? firstName = GetStringProperty(participant, "firstname").UnlessNullOrWhitespaceThen(null);
        string? lastName = GetStringProperty(participant, "lastname").UnlessNullOrWhitespaceThen(null);
        string? fallbackName = GetStringProperty(participant, "nonmembername").UnlessNullOrWhitespaceThen(null);

        string fullName = string.Join(' ', new[] { firstName, lastName }.Where(static value => !string.IsNullOrWhiteSpace(value)))
            .UnlessNullOrWhitespaceThen(fallbackName) ?? string.Empty;

        return fullName.Contains(nameFilter, StringComparison.OrdinalIgnoreCase);
    }

    private static List<object> BuildParticipantSummaries(JsonElement participantsElement, string? slotItemId)
    {
        List<object> participants = [];
        if (string.IsNullOrWhiteSpace(slotItemId)
            || participantsElement.ValueKind is not JsonValueKind.Object
            || !participantsElement.TryGetProperty(slotItemId, out JsonElement participantElement))
        {
            return participants;
        }

        if (participantElement.ValueKind is JsonValueKind.Array)
        {
            foreach (JsonElement participant in participantElement.EnumerateArray())
            {
                if (participant.ValueKind is JsonValueKind.Object)
                {
                    participants.Add(BuildParticipantSummary(participant));
                }
            }

            return participants;
        }

        if (participantElement.ValueKind is JsonValueKind.Object)
        {
            participants.Add(BuildParticipantSummary(participantElement));
        }

        return participants;
    }

    private static object BuildParticipantSummary(JsonElement participant)
    {
        string? firstName = GetStringProperty(participant, "firstname").UnlessNullOrWhitespaceThen(null);
        string? lastName = GetStringProperty(participant, "lastname").UnlessNullOrWhitespaceThen(null);
        string? fallbackName = GetStringProperty(participant, "nonmembername").UnlessNullOrWhitespaceThen(null);

        string? fullName = string.Join(' ', new[] { firstName, lastName }.Where(static value => !string.IsNullOrWhiteSpace(value)))
            .UnlessNullOrWhitespaceThen(fallbackName);

        return new
        {
            name = fullName,
            comment = GetStringProperty(participant, "mycomment").UnlessNullOrWhitespaceThen(null),
        };
    }

    private static bool TryGetObjectProperty(JsonElement element, string propertyName, out JsonElement value)
    {
        if (element.ValueKind is JsonValueKind.Object && element.TryGetProperty(propertyName, out value))
        {
            return true;
        }

        value = default;
        return false;
    }

    private static string? GetStringProperty(JsonElement element, string propertyName)
        => TryGetObjectProperty(element, propertyName, out JsonElement value) && value.ValueKind is JsonValueKind.String
            ? value.GetString()
            : null;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0046:Convert to conditional expression", Justification = "Make code ugly")]
    private static string? GetScalarPropertyText(JsonElement element, string propertyName)
    {
        if (!TryGetObjectProperty(element, propertyName, out JsonElement value))
        {
            return null;
        }

        return value.ValueKind switch
        {
            JsonValueKind.String => value.GetString(),
            JsonValueKind.Number or JsonValueKind.True or JsonValueKind.False => value.ToString(),
            _ => null,
        };
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0046:Convert to conditional expression", Justification = "Make code ugly")]
    private static int GetInt32Property(JsonElement element, string propertyName)
    {
        if (!TryGetObjectProperty(element, propertyName, out JsonElement value))
        {
            return 0;
        }

        return value.ValueKind switch
        {
            JsonValueKind.Number when value.TryGetInt32(out int parsedNumber) => parsedNumber,
            JsonValueKind.String when int.TryParse(value.GetString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int parsedString) => parsedString,
            _ => 0,
        };
    }

    private static bool TryParseSignupTime(string? rawValue, out DateTime startTime)
    {
        string normalized = rawValue.UnlessNullOrWhitespaceThen(string.Empty);
        normalized = TimeZoneSuffixPattern().Replace(normalized, string.Empty).Trim();

        return DateTime.TryParseExact(
            normalized,
            ["MMMM, dd yyyy HH:mm:ss", "MMMM, d yyyy HH:mm:ss"],
            CultureInfo.InvariantCulture,
            DateTimeStyles.AllowWhiteSpaces,
            out startTime);
    }

    private static string GetMealLabel(DateTime startTime)
        => startTime.Hour switch
        {
            12 => "Lunch",
            17 => "Dinner",
            _ => $"Meal at {startTime:h:mm tt}"
        };

    private const string SignupGeniusBaseAddress = "https://www.signupgenius.com/SUGboxAPI.cfm?go=s.getSignupInfo";
    private const string JsonContentTypeHeaderValue = "application/json";
    private static readonly CompositeFormat SignupGeniusRequestJsonCompositeFormat = CompositeFormat.Parse("""{{"forSignUpView":true,"urlid":"{0}","portalid":0}}""");
    private static readonly CompositeFormat SignupGeniusReferrerUrlCompositeFormat = CompositeFormat.Parse("https://www.signupgenius.com/go/{0}?useFullSite=true");
    private HttpRequestMessage CreateSignupGeniusRequest()
    {
        HttpRequestMessage request = new(HttpMethod.Post, string.Empty)
        {
            Content = new StringContent(
                string.Format(default, SignupGeniusRequestJsonCompositeFormat, _mealSignupGeniusId),
                Encoding.UTF8,
                JsonContentTypeHeaderValue)
        };

        request.Headers.Referrer = new Uri(string.Format(default, SignupGeniusReferrerUrlCompositeFormat, _mealSignupGeniusId));

        return request;
    }

    private const string PriorityHeaderName = "priority";
    private const string SecChUaHeaderName = "sec-ch-ua";
    private const string SecChUaMobileHeaderName = "sec-ch-ua-mobile";
    private const string SecChUaPlatformHeaderName = "sec-ch-ua-platform";

    private const string AcceptHeaderValue = "application/json, text/plain, */*";
    private const string AcceptLanguageHeaderValue = "en-US,en;q=0.9";
    private const string OriginHeaderValue = "https://www.signupgenius.com";
    private const string PriorityHeaderValue = "u=1, i";
    private const string SecChUaHeaderValue = "\"Chromium\";v=\"146\", \"Not-A.Brand\";v=\"24\", \"Microsoft Edge\";v=\"146\"";
    private const string SecChUaMobileHeaderValue = "?0";
    private const string SecChUaPlatformHeaderValue = "\"Windows\"";
    private const string SignupGeniusUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/146.0.0.0 Safari/537.36 Edg/146.0.0.0";

    public static void ConfigureHttpClient(HttpClient httpClient)
    {
        httpClient.BaseAddress = new(SignupGeniusBaseAddress);
        httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(SignupGeniusUserAgent);
        httpClient.DefaultRequestHeaders.TryAddWithoutValidation(HeaderNames.Accept, AcceptHeaderValue);
        httpClient.DefaultRequestHeaders.TryAddWithoutValidation(HeaderNames.AcceptLanguage, AcceptLanguageHeaderValue);
        httpClient.DefaultRequestHeaders.TryAddWithoutValidation(HeaderNames.Origin, OriginHeaderValue);
        httpClient.DefaultRequestHeaders.TryAddWithoutValidation(PriorityHeaderName, PriorityHeaderValue);
        httpClient.DefaultRequestHeaders.TryAddWithoutValidation(SecChUaHeaderName, SecChUaHeaderValue);
        httpClient.DefaultRequestHeaders.TryAddWithoutValidation(SecChUaMobileHeaderName, SecChUaMobileHeaderValue);
        httpClient.DefaultRequestHeaders.TryAddWithoutValidation(SecChUaPlatformHeaderName, SecChUaPlatformHeaderValue);
    }

    [GeneratedRegex(" [+-]\\d{4}$", RegexOptions.Compiled)]
    private static partial Regex TimeZoneSuffixPattern();
}
