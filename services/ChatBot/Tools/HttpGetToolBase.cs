namespace ChatBot.Tools;

using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text.Json;

internal abstract class HttpGetToolBase(IHttpClientFactory httpClientFactory, ILogger logger)
{
    private const int MaxFailureResponseSnippetLength = 512;
    private static readonly JsonSerializerOptions s_jsonOptions = new(JsonSerializerDefaults.Web);
    private static readonly ReadOnlyDictionary<string, object?> s_skipPermissionAdditionalProperties = new(
        new Dictionary<string, object?>
        {
            ["skip_permission"] = true,
        });

    public abstract IReadOnlyList<AIFunction> Functions { get; }

    public abstract IReadOnlyList<string> ToolNames { get; }

    protected IHttpClientFactory HttpClientFactory => httpClientFactory;

    internal static AIFunctionFactoryOptions CreateSkippableFunctionOptions(string name, string? description = null)
        => new()
        {
            Name = name,
            Description = description,
            AdditionalProperties = s_skipPermissionAdditionalProperties,
        };

    protected async Task<string> SendGetAsync(
        string clientName,
        string path,
        string? query,
        Action<HttpRequestHeaders>? configureAdditionalHeaders = null,
        IReadOnlyList<CitationLink>? citations = null,
        CancellationToken cancellationToken = default)
    {
        string requestPath = string.IsNullOrWhiteSpace(query)
            ? path
            : $"{path}?{query.Trim().TrimStart('?')}";

        using var request = new HttpRequestMessage(HttpMethod.Get, requestPath);
        configureAdditionalHeaders?.Invoke(request.Headers);

        return await SendGetAsync(clientName, request, citations, cancellationToken).ConfigureAwait(false);
    }

    protected async Task<string> SendGetAsync(
        string clientName,
        HttpRequestMessage request,
        IReadOnlyList<CitationLink>? citations,
        CancellationToken cancellationToken)
    {
        string? requestTarget = request.RequestUri?.OriginalString;
        HttpClient client = httpClientFactory.CreateClient(clientName);
        if (client.BaseAddress is { } u && request.RequestUri is { } r)
        {
            if (requestTarget?.StartsWith('/') is true)
            {
                requestTarget = requestTarget[1..];
                r = new(requestTarget, UriKind.Relative);
            }

            request.RequestUri = new Uri(u, r);
        }

        long startTimestamp = Stopwatch.GetTimestamp();
        using HttpResponseMessage response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);
        string content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        double elapsedMilliseconds = Stopwatch.GetElapsedTime(startTimestamp).TotalMilliseconds;

        JsonElement? data = null;
        string? text = null;
        if (!string.IsNullOrWhiteSpace(content))
        {
            try
            {
                using JsonDocument document = JsonDocument.Parse(content);
                data = document.RootElement.Clone();
            }
            catch (JsonException)
            {
                text = content;
            }
        }

        if (!response.IsSuccessStatusCode)
        {
            logger.HttpAPIToolCallFailedForClientNameRequestPathWithStatusStatusCode(clientName, request.RequestUri, (int)response.StatusCode);
            logger.HttpAPIToolCallFailedAfterElapsedMilliseconds(clientName, request.RequestUri, elapsedMilliseconds);
            string responseSnippet = BuildFailureResponseSnippet(content);
            if (responseSnippet.Length > 0)
            {
                logger.HttpAPIToolCallFailedResponseSnippet(clientName, responseSnippet);
            }
        }

        return SerializeToolResponse(
            requestTarget,
            (int)response.StatusCode,
            response.IsSuccessStatusCode,
            data,
            text,
            response.IsSuccessStatusCode ? null : $"HTTP {(int)response.StatusCode} {response.ReasonPhrase}",
            citations);
    }

    protected static string SerializeToolResponse(
        string? apiRequestPath,
        int statusCode,
        bool ok,
        JsonElement? data,
        string? text,
        string? error,
        IReadOnlyList<CitationLink>? citations = null)
        => JsonSerializer.Serialize(
            new
            {
                apiRequest = string.IsNullOrWhiteSpace(apiRequestPath)
                    ? null
                    : new
                    {
                        path = apiRequestPath,
                        kind = "api",
                    },
                statusCode,
                ok,
                data,
                text,
                error,
                userReferencePages = citations is { Count: > 0 }
                    ? citations.Select(static citation => new
                    {
                        title = citation.Title,
                        url = citation.Url,
                    })
                    : null,
                citations = citations is { Count: > 0 } ? citations : null,
            },
            s_jsonOptions);

    protected static ReadOnlyCollection<string> GetPathSegments(string path)
    {
        ReadOnlySpan<char> trimmedPath = path.AsSpan().Trim().Trim('/');
        if (trimmedPath.IsEmpty)
        {
            return [];
        }

        var retVal = new List<string>();
        MemoryExtensions.SpanSplitEnumerator<char> segments = trimmedPath.Split('/');
        while (segments.MoveNext())
        {
            ReadOnlySpan<char> segment = trimmedPath[segments.Current].Trim();
            if (segment.IsEmpty)
            {
                continue;
            }

            retVal.Add(segment.ToString());
        }

        return retVal.AsReadOnly();
    }

    protected static string? TryGetSegment(ReadOnlySpan<string> segments, int index)
        => index >= 0 && index < segments.Length ? segments[index] : null;

    private static string BuildFailureResponseSnippet(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return string.Empty;
        }

        string normalized = content.ReplaceLineEndings(" ").Trim();
        return normalized.Length <= MaxFailureResponseSnippetLength
            ? normalized
            : $"{normalized[..MaxFailureResponseSnippetLength]}...";
    }

    protected sealed record CitationLink(string Title, string Url)
    {
        public override string ToString() => $"[{Title}]({Url})";
    }
}
