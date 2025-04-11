namespace FunctionApp.Tests.DiscordInterop.Embeds;

using Discord;

using FunctionApp.DiscordInterop.Embeds;

using Moq;

using System;
using System.Drawing;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using TestCommon;

using Xunit;
using Xunit.Abstractions;

using Color = Discord.Color;

public class EmbeddingColorizerTests : TestWithLogger
{
    private readonly Mock<FRCColors.IClient> _mockColorClient;
    private readonly EmbeddingColorizer _embeddingColorizer;

    public EmbeddingColorizerTests(ITestOutputHelper outputHelper) : base(typeof(EmbeddingColorizer), outputHelper)
    {
        _mockColorClient = this.Mocker.GetMock<FRCColors.IClient>();
        _embeddingColorizer = this.Mocker.CreateInstance<EmbeddingColorizer>();
    }

    [Fact]
    public async Task SetEmbeddingColorAsync_ShouldSetColor_WhenColorsAreAvailable()
    {
        // Arrange
        var teamKey = "frc123";
        var embedBuilder = new EmbedBuilder();
        var primaryColor = new Color(255, 0, 0);
        var secondaryColor = new Color(0, 255, 0);

        _mockColorClient.Setup(client => client.GetColorsForTeamAsync(It.IsAny<ushort>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((primaryColor, secondaryColor));

        // Act
        var result = await _embeddingColorizer.SetEmbeddingColorAsync(teamKey, embedBuilder, CancellationToken.None);

        // Assert
        Assert.True(result);
        Assert.Equal(secondaryColor, embedBuilder.Color);
    }

    [Fact]
    public async Task SetEmbeddingColorAsync_ShouldNotSetColor_WhenColorsAreNotAvailable()
    {
        // Arrange
        var teamKey = "frc123";
        var embedBuilder = new EmbedBuilder();

        _mockColorClient.Setup(client => client.GetColorsForTeamAsync(It.IsAny<ushort>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((null, null));

        // Act
        var result = await _embeddingColorizer.SetEmbeddingColorAsync(teamKey, embedBuilder, CancellationToken.None);

        // Assert
        Assert.False(result);
        Assert.Null(embedBuilder.Color);
    }

    [Fact]
    public void SetEmbeddingColor_ShouldSetColor_WhenColorsAreAvailable()
    {
        // Arrange
        var teamKey = "frc123";
        var embedBuilder = new EmbedBuilder();
        var primaryColor = new Color(255, 0, 0);
        var secondaryColor = new Color(0, 255, 0);

        _mockColorClient.Setup(client => client.GetColorsForTeam(It.IsAny<ushort>(), It.IsAny<CancellationToken>()))
            .Returns((primaryColor, secondaryColor));

        // Act
        var result = _embeddingColorizer.SetEmbeddingColor(teamKey, embedBuilder, CancellationToken.None);

        // Assert
        Assert.True(result);
        Assert.Equal(secondaryColor, embedBuilder.Color);
    }

    [Fact]
    public void SetEmbeddingColor_ShouldNotSetColor_WhenColorsAreNotAvailable()
    {
        // Arrange
        var teamKey = "frc123";
        var embedBuilder = new EmbedBuilder();

        _mockColorClient.Setup(client => client.GetColorsForTeam(It.IsAny<ushort>(), It.IsAny<CancellationToken>()))
            .Returns((null, null));

        // Act
        var result = _embeddingColorizer.SetEmbeddingColor(teamKey, embedBuilder, CancellationToken.None);

        // Assert
        Assert.False(result);
        Assert.Null(embedBuilder.Color);
    }

    [Fact]
    public async Task SetEmbeddingColorAsync_ShouldLogError_WhenExceptionIsThrown()
    {
        // Arrange
        var teamKey = "frc123";
        var embedBuilder = new EmbedBuilder();
        var exception = new HttpRequestException("Test exception");

        _mockColorClient.Setup(client => client.GetColorsForTeamAsync(It.IsAny<ushort>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);

        // Act
        var result = await _embeddingColorizer.SetEmbeddingColorAsync(teamKey, embedBuilder, CancellationToken.None);

        // Assert
        Assert.False(result);
        Assert.Null(embedBuilder.Color);
        this.Logger.Verify(Microsoft.Extensions.Logging.LogLevel.Error);
    }

    [Fact]
    public void SetEmbeddingColor_ShouldLogError_WhenExceptionIsThrown()
    {
        // Arrange
        var teamKey = "frc123";
        var embedBuilder = new EmbedBuilder();
        var exception = new JsonException("Test exception");

        _mockColorClient.Setup(client => client.GetColorsForTeam(It.IsAny<ushort>(), It.IsAny<CancellationToken>()))
            .Throws(exception);

        // Act
        var result = _embeddingColorizer.SetEmbeddingColor(teamKey, embedBuilder, CancellationToken.None);

        // Assert
        Assert.False(result);
        Assert.Null(embedBuilder.Color);
        this.Logger.Verify(Microsoft.Extensions.Logging.LogLevel.Error);
    }
}

