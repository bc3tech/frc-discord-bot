namespace Common.Tests.Extensions;
using Common.Extensions;

using System.Text.Json;

using Xunit;

public class JsonElementExtensionsTests
{
    [Fact]
    public void TryGetPropertyAnywhere_ForRootLevelProperty_FindsIt()
    {
        var json = @"{""simple"":42}";
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        Assert.True(root.TryGetPropertyAnywhere("simple", out var found));
        Assert.NotNull(found);
        Assert.Single(found!);
        Assert.Equal(42, found!.First().GetInt32());
    }

    [Fact]
    public void TryGetPropertyAnywhere_ForNestedObject_FindsIt()
    {
        var json = @"{""outer"":{""inner"":""value""}}";
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        Assert.True(root.TryGetPropertyAnywhere("inner", out var found));
        Assert.NotNull(found);
        Assert.Single(found!);
        Assert.Equal("value", found!.First().GetString());
    }

    [Fact]
    public void TryGetPropertyAnywhere_ForArrayItemProperty_FindsIt()
    {
        var json = @"{""arr"":[{""target"":100},{""other"":200}]}";
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        Assert.True(root.TryGetPropertyAnywhere("target", out var found));
        Assert.NotNull(found);
        Assert.Single(found!);
        Assert.Equal(100, found!.First().GetInt32());
    }

    [Fact]
    public void TryGetPropertyAnywhere_ForMultipleMatches_FindsThemAll()
    {
        var json = @"{""first"":123,""nested"":{""first"":456},""arr"":[{""first"":789}]}";
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        Assert.True(root.TryGetPropertyAnywhere("first", out var found));
        Assert.NotNull(found);
        Assert.Equal(3, found!.Count);
    }

    [Fact]
    public void TryGetPropertyAnywhere_WhenPropertyMissing_ReturnsFalse()
    {
        var json = @"{""a"":1,""b"":2,""c"":{""d"":3}}";
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        Assert.False(root.TryGetPropertyAnywhere("missing", out var found));
        Assert.Empty(found!);
    }
}