namespace FunctionApp.Tests.DiscordInterop.CommandModules;

using Common.Extensions;

using Discord;
using Discord.Interactions;

using FunctionApp.DiscordInterop.CommandModules;
using FunctionApp.DiscordInterop.Embeds;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Moq;

using System;
using System.Threading;
using System.Threading.Tasks;

using TestCommon;

using TheBlueAlliance.Api;

using Xunit;
using Xunit.Abstractions;

public class MatchesCommandModuleTests : TestWithLogger
{
    private readonly MatchesCommandModule _module;
    private readonly Mock<IInteractionContext> _mockContext;

    public MatchesCommandModuleTests(ITestOutputHelper outputHelper) : base(typeof(MatchesCommandModule), outputHelper)
    {
        this.Mocker.AddKeyedService<IEmbedCreator<(string eventKey, string teamKey)>>(nameof(UpcomingMatch), this.Mocker.WithSelfMock<IEmbedCreator<(string eventKey, string teamKey)>>());
        this.Mocker.AddKeyedService<IEmbedCreator<(string, bool)>>(nameof(MatchScore), this.Mocker.WithSelfMock<IEmbedCreator<(string, bool)>>());

        this.Mocker.WithSelfMock<IMatchApi>();

        _module = new MatchesCommandModule(this.Mocker);

        _mockContext = this.Mocker.GetMock<IInteractionContext>();
        ((IInteractionModuleBase)_module).SetContext(_mockContext.Object);

        _mockContext.SetupGet(c => c.Interaction)
            .Returns(this.Mocker.CreateSelfMock<IDiscordInteraction>());

        var interactionMock = this.Mocker.GetMock<IDiscordInteraction>();
        interactionMock.Setup(i => i.DeferAsync(It.IsAny<bool>(), It.IsAny<RequestOptions>()))
            .Returns(Task.FromResult(Mock.Of<IDisposable>()));

        var channelMock = this.Mocker.GetMock<IMessageChannel>();
        channelMock.Setup(i => i.EnterTypingState(It.IsAny<RequestOptions>()))
            .Returns(Mock.Of<IDisposable>());
        _mockContext.SetupGet(c => c.Channel).Returns(channelMock.Object);
    }

    [Fact]
    public async Task ShowNextAsync_ShouldGenerateResponse()
    {
        // Arrange
        var eventKey = "2025iscmp";
        var teamKey = "frc254";
        var post = false;

        var upcomingMatchEmbeddingCreatorMock = this.Mocker.GetMock<IEmbedCreator<(string eventKey, string teamKey)>>();
        upcomingMatchEmbeddingCreatorMock
            .Setup(creator => creator.CreateAsync(It.IsAny<(string, string)>(), It.IsAny<ushort?>(), It.IsAny<CancellationToken>()))
            .Returns(new ResponseEmbedding[] { new(new EmbedBuilder().Build()) }.ToAsyncEnumerable());

        // Act
        await _module.ShowNextAsync(eventKey, teamKey, post);

        // Assert
        upcomingMatchEmbeddingCreatorMock.Verify(creator => creator.CreateAsync(It.IsAny<(string, string)>(), It.IsAny<ushort?>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetScoreAsync_ShouldCallGetResultAsync()
    {
        // Arrange
        var eventKey = "2025iscmp";
        var compLevel = 1;
        var matchNumber = 1u;
        var summarize = false;
        var post = false;

        var matchScoreEmbeddingGeneratorMock = this.Mocker.GetMock<IEmbedCreator<(string, bool)>>();
        matchScoreEmbeddingGeneratorMock
            .Setup(creator => creator.CreateAsync(It.IsAny<(string, bool)>(), It.IsAny<ushort?>(), It.IsAny<CancellationToken>()))
            .Returns(new ResponseEmbedding[] { new(new EmbedBuilder().Build()) }.ToAsyncEnumerable());
        // Act
        await _module.GetScoreAsync(eventKey, compLevel, matchNumber, summarize, post);

        // Assert
        matchScoreEmbeddingGeneratorMock.Verify(creator => creator.CreateAsync(It.IsAny<(string, bool)>(), It.IsAny<ushort?>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetResultAsync_ShouldGenerateResponse()
    {
        // Arrange
        var eventKey = "2025iscmp";
        var compLevel = 1;
        var matchNumber = 1u;
        var summarize = false;
        var post = false;

        var matchScoreEmbeddingGeneratorMock = this.Mocker.GetMock<IEmbedCreator<(string, bool)>>();
        matchScoreEmbeddingGeneratorMock
            .Setup(creator => creator.CreateAsync(It.IsAny<(string, bool)>(), It.IsAny<ushort?>(), It.IsAny<CancellationToken>()))
            .Returns(new ResponseEmbedding[] { new(new EmbedBuilder().Build()) }.ToAsyncEnumerable());

        // Act
        await _module.GetResultAsync(eventKey, compLevel, matchNumber, summarize, post);

        // Assert
        matchScoreEmbeddingGeneratorMock.Verify(creator => creator.CreateAsync(It.IsAny<(string, bool)>(), It.IsAny<ushort?>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
