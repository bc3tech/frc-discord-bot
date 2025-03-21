﻿namespace DiscordBotFunctionApp.Apis;

using Common.Extensions;

using Microsoft.Extensions.Logging;

using System.Diagnostics.Metrics;
using System.Net.Http.Json;
using System.Text.Json.Nodes;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1001:Types that own disposable fields should be disposable", Justification = "Not valid for just HTTP Client")]
internal sealed class RESTCountries(Meter meter, ILogger<RESTCountries> _logger)
{
    private readonly HttpClient _httpClient = new() { BaseAddress = new("https://restcountries.com/v3.1/") };
    private readonly Counter<int> _numCountries = meter.CreateCounter<int>(Constants.Telemetry.Metrics.NumCountries);

    public async Task<string?> GetCountryCodeForFlagLookupAsync(string country, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _httpClient.GetAsync($@"name/{country}", cancellationToken: cancellationToken).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                response = await _httpClient.GetAsync($@"name/{country}?fullText=true", cancellationToken: cancellationToken).ConfigureAwait(false);
            }

            var responseContent = await response.Content.ReadFromJsonAsync<JsonArray>(cancellationToken: cancellationToken).ConfigureAwait(false);
            if (responseContent is null or { Count: 0 })
            {
                _logger.NoCountryFoundForCountry(country);
                return null;
            }

            _logger.NumCountriesCountryIesReturned(responseContent.Count);
            meter.LogMetric("NumCountries", responseContent.Count, [new("Country", country)]);
            var result = responseContent[0]!;

            _logger.FirstCountryCountry(result["name"]!["common"]);

            return result["cca2"]?.ToString();
        }
        catch (Exception e) when (e is not OperationCanceledException and not TaskCanceledException)
        {
            _logger.ErrorGettingCountryCodeForCountry(e, country);
            return null;
        }
    }
}
