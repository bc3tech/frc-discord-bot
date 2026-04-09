namespace FunctionApp.Tests;

internal sealed class TestHttpClientFactory(HttpClient httpClient) : IHttpClientFactory
{
    public HttpClient CreateClient(string name) => httpClient;
}
