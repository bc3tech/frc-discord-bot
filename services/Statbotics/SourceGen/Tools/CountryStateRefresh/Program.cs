namespace StatboticsKnownValues.SourceGen.Tools.CountryStateRefresh;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

internal static class Program
{
    private const string DefaultOutputPath = "vendor/statbotics-extras/country-state.json";
    private const string TeamsEndpoint = "https://api.statbotics.io/v3/teams";
    private const int PageSize = 1000;
    private const int SafetyCap = 50_000;

    public static async Task<int> Main(string[] args)
    {
        string outputPath = ParseOutputPath(args) ?? DefaultOutputPath;

        Console.WriteLine($"Refreshing country/state snapshot to {outputPath}");
        Console.WriteLine($"Source: {TeamsEndpoint}?limit={PageSize}&offset=N (paginated)");

        using var httpClient = new HttpClient { Timeout = TimeSpan.FromMinutes(2) };

        var countries = new HashSet<string>(StringComparer.Ordinal);
        var states = new HashSet<string>(StringComparer.Ordinal);
        int totalTeams = 0;
        int offset = 0;

        try
        {
            while (offset < SafetyCap)
            {
                string url = $"{TeamsEndpoint}?limit={PageSize}&offset={offset}";
                Console.WriteLine($"  fetching offset={offset}...");

                using var response = await httpClient.GetAsync(url, CancellationToken.None).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();

                await using Stream stream = await response.Content.ReadAsStreamAsync(CancellationToken.None).ConfigureAwait(false);
                Team[]? page = await JsonSerializer.DeserializeAsync<Team[]>(
                    stream,
                    new JsonSerializerOptions(JsonSerializerDefaults.Web)).ConfigureAwait(false);

                if (page is null || page.Length == 0)
                {
                    break;
                }

                foreach (Team team in page)
                {
                    if (!string.IsNullOrWhiteSpace(team.Country))
                    {
                        countries.Add(team.Country!);
                    }

                    if (!string.IsNullOrWhiteSpace(team.State))
                    {
                        states.Add(team.State!);
                    }
                }

                totalTeams += page.Length;
                offset += PageSize;

                if (page.Length < PageSize)
                {
                    break;
                }
            }
        }
        catch (HttpRequestException ex)
        {
            Console.Error.WriteLine($"HTTP error after {totalTeams} teams (offset={offset}): {ex.Message}");
            return 2;
        }

        Console.WriteLine($"Fetched {totalTeams} teams: {countries.Count} distinct countries, {states.Count} distinct states.");

        var snapshot = new Snapshot(
            LastRefreshedAt: DateTime.UtcNow.ToString("o"),
            SourceEndpoint: TeamsEndpoint,
            Countries: countries.OrderBy(c => c, StringComparer.Ordinal).ToArray(),
            States: states.OrderBy(s => s, StringComparer.Ordinal).ToArray());

        string json = JsonSerializer.Serialize(snapshot, new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.Never,
            // Write non-ASCII characters as raw UTF-8 (e.g., "Türkiye" instead of
            // "T\u00FCrkiye"). The source generator's hand-rolled JSON parser doesn't
            // decode \uXXXX escapes; writing raw UTF-8 keeps the round-trip correct.
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        });

        Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
        await File.WriteAllTextAsync(outputPath, json + Environment.NewLine).ConfigureAwait(false);

        Console.WriteLine($"Wrote {outputPath}");
        return 0;
    }

    private static string? ParseOutputPath(string[] args)
    {
        for (int i = 0; i < args.Length - 1; i++)
        {
            if (args[i] is "--output" or "-o")
            {
                return args[i + 1];
            }
        }

        return null;
    }

    private sealed record Team(
        [property: JsonPropertyName("country")] string? Country,
        [property: JsonPropertyName("state")] string? State);

    private sealed record Snapshot(
        [property: JsonPropertyName("lastRefreshedAt")] string LastRefreshedAt,
        [property: JsonPropertyName("sourceEndpoint")] string SourceEndpoint,
        [property: JsonPropertyName("countries")] string[] Countries,
        [property: JsonPropertyName("states")] string[] States);
}
