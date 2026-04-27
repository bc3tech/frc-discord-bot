namespace FunctionApp.Tests;

using global::ChatBot.Configuration;

using GitHub.Copilot.SDK;

using Microsoft.Extensions.Configuration;

public sealed class ModelSessionConfigSourceTests
{
    private static ModelSessionConfigSource CreateSource(Dictionary<string, string?> configValues) =>
        new(new ConfigurationBuilder().AddInMemoryCollection(configValues).Build());

    [Fact]
    public async Task ConfigureAsync_SetsModelFromConfiguration()
    {
        var source = CreateSource(new() { ["AI:Copilot:Model"] = "gpt-5.4-mini" });
        var config = new SessionConfig();

        await source.ConfigureAsync(config, CancellationToken.None);

        Assert.Equal("gpt-5.4-mini", config.Model);
    }

    [Fact]
    public async Task ConfigureAsync_DoesNotTouchProvider()
    {
        var source = CreateSource(new() { ["AI:Copilot:Model"] = "gpt-5.4-mini" });
        var existingProvider = new ProviderConfig { BaseUrl = "https://example.com" };
        var config = new SessionConfig { Provider = existingProvider };

        await source.ConfigureAsync(config, CancellationToken.None);

        Assert.Same(existingProvider, config.Provider);
        Assert.Equal("https://example.com", config.Provider.BaseUrl);
    }

    [Fact]
    public async Task ConfigureAsync_OverridesExistingModel()
    {
        var source = CreateSource(new() { ["AI:Copilot:Model"] = "gpt-5.4-mini" });
        var config = new SessionConfig { Model = "gpt-5.4-nano" };

        await source.ConfigureAsync(config, CancellationToken.None);

        Assert.Equal("gpt-5.4-mini", config.Model);
    }

    [Fact]
    public async Task ConfigureAsync_ThrowsWhenModelConfigMissing()
    {
        var source = CreateSource([]);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => source.ConfigureAsync(new SessionConfig(), CancellationToken.None).AsTask());
    }
}
