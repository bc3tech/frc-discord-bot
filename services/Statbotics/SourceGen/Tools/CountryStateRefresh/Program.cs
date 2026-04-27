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

    private static readonly JsonSerializerOptions s_responseOptions = new(JsonSerializerDefaults.Web);

    private static readonly JsonSerializerOptions s_outputOptions = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.Never,
        // Write non-ASCII characters as raw UTF-8 (e.g., "Türkiye" instead of
        // "T\u00FCrkiye"). The source generator's hand-rolled JSON parser doesn't
        // decode \uXXXX escapes; writing raw UTF-8 keeps the round-trip correct.
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    public static async Task<int> Main(string[] args)
    {
        using var cts = new CancellationTokenSource();
        Console.CancelKeyPress += (_, e) =>
        {
            e.Cancel = true;
            cts.Cancel();
        };

        string outputPath = ParseOutputPath(args) ?? DefaultOutputPath;

        await Console.Out.WriteLineAsync($"Refreshing country/state snapshot to {outputPath}").ConfigureAwait(false);
        await Console.Out.WriteLineAsync($"Source: {TeamsEndpoint}?limit={PageSize}&offset=N (paginated)").ConfigureAwait(false);

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
                await Console.Out.WriteLineAsync($"  fetching offset={offset}...").ConfigureAwait(false);

                using var response = await httpClient.GetAsync(url, cts.Token).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();

                await using Stream stream = await response.Content.ReadAsStreamAsync(cts.Token).ConfigureAwait(false);
                Team[]? page = await JsonSerializer.DeserializeAsync<Team[]>(stream, s_responseOptions, cts.Token).ConfigureAwait(false);

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
            await Console.Error.WriteLineAsync($"HTTP error after {totalTeams} teams (offset={offset}): {ex.Message}").ConfigureAwait(false);
            return 2;
        }

        await Console.Out.WriteLineAsync($"Fetched {totalTeams} teams: {countries.Count} distinct countries, {states.Count} distinct states.").ConfigureAwait(false);

        var snapshot = new Snapshot(
            LastRefreshedAt: DateTime.UtcNow.ToString("o", System.Globalization.CultureInfo.InvariantCulture),
            SourceEndpoint: TeamsEndpoint,
            Countries: countries.OrderBy(c => c, StringComparer.Ordinal).ToArray(),
            States: states.OrderBy(s => s, StringComparer.Ordinal).ToArray());

        string json = JsonSerializer.Serialize(snapshot, s_outputOptions);

        Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
        await File.WriteAllTextAsync(outputPath, json + Environment.NewLine, cts.Token).ConfigureAwait(false);

        await Console.Out.WriteLineAsync($"Wrote {outputPath}").ConfigureAwait(false);
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
