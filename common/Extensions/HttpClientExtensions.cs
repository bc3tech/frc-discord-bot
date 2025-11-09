namespace Common.Extensions;
using System.Text;

public static class HttpClientExtensions
{
    public static HttpResponseMessage Get(this HttpClient client, Uri url, CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        return client.Send(request, cancellationToken);
    }

    public static T? GetFromJson<T>(this HttpClient client, Uri url, CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        var response = client.Send(request, cancellationToken);
        if (response.IsSuccessStatusCode)
        {
            using var reader = new StreamReader(response.Content.ReadAsStream(cancellationToken), Encoding.UTF8, leaveOpen: true);
            return System.Text.Json.JsonSerializer.Deserialize<T>(reader.ReadToEnd());
        }
        else
        {
            throw new HttpRequestException($"Request failed with status code {response.StatusCode}");
        }
    }
}
