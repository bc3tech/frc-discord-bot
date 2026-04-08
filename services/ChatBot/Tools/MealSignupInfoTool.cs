namespace ChatBot.Tools;

using ChatBot.Configuration;

using Common.Extensions;

using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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

internal sealed partial class MealSignupInfoTool(IHttpClientFactory httpClientFactory, IOptions<AiOptions> configuration, ILogger<MealSignupInfoTool> logger) : HttpGetToolBase(httpClientFactory, logger)
{
    private const string ToolName = "fetch_meal_signup_info";
    private const string ToolDescription = "Fetch the current Bear Metal meal signup data from SignupGenius. Use this for meal-signup questions such as what food is needed, who signed up, open slots, quantities, dates, delivery times, comments, and other current SignupGenius state.";

    private readonly string _mealSignupGeniusId = configuration.Value.MealSignupGeniusId;

    public override IReadOnlyList<AIFunction> Functions => field ??= [AIFunctionFactory.Create(FetchMealSignupInfoResponseBodyAsync, ToolName, ToolDescription)];

    [Description("Fetches the current Bear Metal meal signup data from SignupGenius. Use this whenever the request is about meal signup state, food assignments, open slots, quantities, dates, delivery times, or who signed up.")]
    public async Task<string> FetchMealSignupInfoResponseBodyAsync(CancellationToken cancellationToken)
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
                JsonElement rawData = document.RootElement.Clone();
                data = JsonSerializer.SerializeToElement(new
                {
                    summary = BuildMealSignupSummary(rawData),
                    raw = rawData,
                });
            }
            catch (JsonException)
            {
                text = content;
            }
        }

        response.EnsureSuccessStatusCode();

        return SerializeToolResponse(
            request.RequestUri,
            (int)response.StatusCode,
            response.IsSuccessStatusCode,
            data,
            text,
            null,
            [new CitationLink("SignupGenius", sourcePage.ToString())]);
    }

    private static object? BuildMealSignupSummary(JsonElement root)
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

        List<object> slots = [];
        foreach (JsonProperty slotProperty in slotsElement.EnumerateObject())
        {
            JsonElement slot = slotProperty.Value;
            if (slot.ValueKind is not JsonValueKind.Object
                || !TryParseSignupTime(GetStringProperty(slot, "starttime"), out DateTime startTime))
            {
                continue;
            }

            string location = GetStringProperty(slot, "location").UnlessNullOrWhitespaceThen("Unspecified location");

            List<object> items = [];
            int participantCount = 0;
            int openSlotCount = 0;
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

                    List<object> participantSummaries = BuildParticipantSummaries(
                        participantsElement,
                        GetScalarPropertyText(item, "slotitemid"));

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

            slots.Add(new
            {
                slotId = GetScalarPropertyText(slot, "slotid") ?? slotProperty.Name,
                startTime = GetStringProperty(slot, "starttime"),
                localDate = startTime.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                localDayOfWeek = startTime.ToString("dddd", CultureInfo.InvariantCulture),
                localTime = startTime.ToString("h:mm tt", CultureInfo.InvariantCulture),
                mealLabel = GetMealLabel(startTime),
                location,
                slotNote = GetStringProperty(slot, "comments").UnlessNullOrWhitespaceThen(null),
                participantCount,
                openSlotCount,
                items,
            });
        }

        return new
        {
            slotCount = slots.Count,
            slots,
        };
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
