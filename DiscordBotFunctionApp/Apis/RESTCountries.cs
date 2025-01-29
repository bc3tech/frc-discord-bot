namespace DiscordBotFunctionApp.Apis;

using Microsoft.Extensions.Logging;

using System.Net.Http.Json;
using System.Text.Json.Nodes;

internal sealed class RESTCountries(IHttpClientFactory httpClientFactory, ILogger<RESTCountries> _logger)
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient(nameof(RESTCountries));

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
                _logger.LogWarning("No country found for {Country}", country);
                return null;
            }

            _logger.LogDebug("{NumCountries} country(ies) returned.", responseContent.Count);
            var result = responseContent[0]!;

            _logger.LogTrace("First country: {Country}", result["name"]!["common"]!.ToString());

            return result["cca2"]?.ToString();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting country code for {Country}", country);
            return null;
        }
    }
}
