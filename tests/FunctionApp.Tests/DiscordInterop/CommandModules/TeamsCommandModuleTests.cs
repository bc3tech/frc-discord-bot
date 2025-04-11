namespace FunctionApp.Tests.DiscordInterop.CommandModules;

using Discord;
using Discord.Net;

using FunctionApp.DiscordInterop.CommandModules;
using FunctionApp.DiscordInterop.Embeds;

using Moq;

using System.Threading;
using System.Threading.Tasks;

using TestCommon;

using Xunit;
using Xunit.Abstractions;

public class TeamsCommandModuleTests : TestWithDiscordInteraction<TeamsCommandModule>
{
    private readonly Mock<IEmbedCreator<string>> _mockTeamDetailEmbedCreator;
    private readonly Mock<IEmbedCreator<(int? Year, string TeamKey, string? EventKey)>> _mockTeamRankEmbedCreator;
    private readonly Mock<IEmbedCreator<(string?, ushort)>> _mockScheduleEmbedCreator;

    public TeamsCommandModuleTests(ITestOutputHelper outputHelper) : base(outputHelper)
    {
        _mockTeamDetailEmbedCreator = new Mock<IEmbedCreator<string>>();
        this.Mocker.Use(_mockTeamDetailEmbedCreator);
        this.Mocker.AddKeyedService(nameof(TeamDetail), _mockTeamDetailEmbedCreator.Object);

        _mockTeamRankEmbedCreator = new Mock<IEmbedCreator<(int? Year, string TeamKey, string? EventKey)>>();
        this.Mocker.Use(_mockTeamRankEmbedCreator);
        this.Mocker.AddKeyedService(nameof(TeamRank), _mockTeamRankEmbedCreator.Object);

        _mockScheduleEmbedCreator = new Mock<IEmbedCreator<(string?, ushort)>>();
        this.Mocker.Use(_mockScheduleEmbedCreator);
        this.Mocker.AddKeyedService(nameof(Schedule), _mockScheduleEmbedCreator.Object);

        this.Module = this.Mocker.CreateInstance<TeamsCommandModule>();
    }

    [Fact]
    public async Task ShowAsync_ShouldShowTeamDetails()
    {
        // Arrange
        var teamKey = "frc254";
        var post = false;

        _mockTeamDetailEmbedCreator.Setup(m => m.CreateAsync(It.IsAny<string>(), It.IsAny<ushort?>(), It.IsAny<CancellationToken>()))
            .Returns(new[] { new ResponseEmbedding(new EmbedBuilder().WithDescription("Rank details").Build()) }.ToAsyncEnumerable());

        // Act
        await this.Module.ShowAsync(teamKey, post);

        // Assert
        _mockTeamDetailEmbedCreator.Verify(m => m.CreateAsync(It.IsAny<string>(), It.IsAny<ushort?>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ShowAsync_ShouldShowTeamDetailsWhenGivenOnlyTeamNumber()
    {
        // Arrange
        var teamKey = "254";
        var post = false;

        _mockTeamDetailEmbedCreator.Setup(m => m.CreateAsync(It.IsAny<string>(), It.IsAny<ushort?>(), It.IsAny<CancellationToken>()))
            .Returns(new[] { new ResponseEmbedding(new EmbedBuilder().WithDescription("Rank details").Build()) }.ToAsyncEnumerable());

        // Act
        await this.Module.ShowAsync(teamKey, post);

        // Assert
        _mockTeamDetailEmbedCreator.Verify(m => m.CreateAsync(It.IsAny<string>(), It.IsAny<ushort?>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ShowAsync_HandlesInteractionAcknowledgedAtDefer()
    {
        // Arrange
        var teamKey = "frc254";
        var post = false;

        this.MockInteraction.Setup(m => m.DeferAsync(It.IsAny<bool>(), It.IsAny<RequestOptions>()))
            .Throws(() => new HttpException(System.Net.HttpStatusCode.InternalServerError, Mock.Of<IRequest>(), DiscordErrorCode.InteractionHasAlreadyBeenAcknowledged));

        // Act
        await this.Module.ShowAsync(teamKey, post);

        // Assert
        this.MockInteraction.Verify(m => m.RespondAsync(It.IsAny<string>(), It.IsAny<Embed[]>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<AllowedMentions>(), It.IsAny<MessageComponent>(), It.IsAny<Embed>(), It.IsAny<RequestOptions>(), It.IsAny<PollProperties>()), Times.Never);
        _mockTeamDetailEmbedCreator.Verify(m => m.CreateAsync(It.IsAny<string>(), It.IsAny<ushort?>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ShowAsync_RequiresTeamKey()
    {
        // Arrange
        var post = false;

        string? response = null;
        this.MockInteraction.Setup(m => m.RespondAsync(It.IsAny<string>(), It.IsAny<Embed[]>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<AllowedMentions>(), It.IsAny<MessageComponent>(), It.IsAny<Embed>(), It.IsAny<RequestOptions>(), It.IsAny<PollProperties>()))
            .Callback((string msg, Embed[] _, bool _, bool _, AllowedMentions _, MessageComponent _, Embed _, RequestOptions _, PollProperties _) => response = msg);

        // Act
        await this.Module.ShowAsync(string.Empty, post);

        // Assert
        this.MockInteraction.Verify(m => m.RespondAsync(It.IsAny<string>(), It.IsAny<Embed[]>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<AllowedMentions>(), It.IsAny<MessageComponent>(), It.IsAny<Embed>(), It.IsAny<RequestOptions>(), It.IsAny<PollProperties>()), Times.Once);
        Assert.NotNull(response);
        Assert.Equal("Team key is required.", response);
        _mockTeamDetailEmbedCreator.Verify(m => m.CreateAsync(It.IsAny<string>(), It.IsAny<ushort?>(), It.IsAny<CancellationToken>()), Times.Never);
    }
    [Fact]
    public async Task GetRankAsync_ShouldGetTeamRank()
    {
        // Arrange
        var teamKey = "frc254";
        var eventKey = "2025iscmp";
        var year = (ushort?)2025;
        var post = false;

        _mockTeamRankEmbedCreator
            .Setup(m => m.CreateAsync(It.IsAny<(int? Year, string TeamKey, string? EventKey)>(), It.IsAny<ushort?>(), It.IsAny<CancellationToken>()))
            .Returns(new[] { new ResponseEmbedding(new EmbedBuilder().WithDescription("Rank details").Build()) }.ToAsyncEnumerable());

        // Act
        await this.Module.GetRankAsync(teamKey, eventKey, year, post);

        // Assert
        _mockTeamRankEmbedCreator.Verify(m => m.CreateAsync(It.IsAny<(int? Year, string TeamKey, string? EventKey)>(), It.IsAny<ushort?>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetRankAsync_ShouldGetTeamRankWhenGivenOnlyTeamNumber()
    {
        // Arrange
        var teamKey = "254";
        var eventKey = "2025iscmp";
        var year = (ushort?)2025;
        var post = false;

        _mockTeamRankEmbedCreator
            .Setup(m => m.CreateAsync(It.IsAny<(int? Year, string TeamKey, string? EventKey)>(), It.IsAny<ushort?>(), It.IsAny<CancellationToken>()))
            .Returns(new[] { new ResponseEmbedding(new EmbedBuilder().WithDescription("Rank details").Build()) }.ToAsyncEnumerable());

        // Act
        await this.Module.GetRankAsync(teamKey, eventKey, year, post);

        // Assert
        _mockTeamRankEmbedCreator.Verify(m => m.CreateAsync(It.IsAny<(int? Year, string TeamKey, string? EventKey)>(), It.IsAny<ushort?>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetScheduleAsync_ShouldGetTeamSchedule()
    {
        // Arrange
        var teamKey = "frc254";
        var eventKey = "2025iscmp";
        var numMatches = (ushort)6;
        var post = false;

        _mockScheduleEmbedCreator
            .Setup(m => m.CreateAsync(It.IsAny<(string?, ushort)>(), It.IsAny<ushort?>(), It.IsAny<CancellationToken>()))
            .Returns(new[] { new ResponseEmbedding(new EmbedBuilder().WithDescription("Rank details").Build()) }.ToAsyncEnumerable());

        // Act
        await this.Module.GetScheduleAsync(teamKey, eventKey, numMatches, post);

        // Assert
        _mockScheduleEmbedCreator.Verify(m => m.CreateAsync(It.IsAny<(string?, ushort)>(), It.IsAny<ushort?>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetScheduleAsync_HandlesInteractionAcknowledgedAtDefer()
    {
        // Arrange
        var teamKey = "frc254";
        var eventKey = "2025iscmp";
        var numMatches = (ushort)6;
        var post = false;

        this.MockInteraction.Setup(m => m.DeferAsync(It.IsAny<bool>(), It.IsAny<RequestOptions>()))
            .Throws(() => new HttpException(System.Net.HttpStatusCode.InternalServerError, Mock.Of<IRequest>(), DiscordErrorCode.InteractionHasAlreadyBeenAcknowledged));

        // Act
        await this.Module.GetScheduleAsync(teamKey, eventKey, numMatches, post);

        // Assert
        this.MockInteraction.Verify(m => m.RespondAsync(It.IsAny<string>(), It.IsAny<Embed[]>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<AllowedMentions>(), It.IsAny<MessageComponent>(), It.IsAny<Embed>(), It.IsAny<RequestOptions>(), It.IsAny<PollProperties>()), Times.Never);
        _mockScheduleEmbedCreator.Verify(m => m.CreateAsync(It.IsAny<(string?, ushort)>(), It.IsAny<ushort?>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetRankAsync_HandlesInteractionAcknowledgedAtDefer()
    {
        // Arrange
        var teamKey = "frc254";
        var eventKey = "2025iscmp";
        var numMatches = (ushort)6;
        var post = false;

        this.MockInteraction.Setup(m => m.DeferAsync(It.IsAny<bool>(), It.IsAny<RequestOptions>()))
            .Throws(() => new HttpException(System.Net.HttpStatusCode.InternalServerError, Mock.Of<IRequest>(), DiscordErrorCode.InteractionHasAlreadyBeenAcknowledged));

        // Act
        await this.Module.GetRankAsync(teamKey, eventKey, numMatches, post);

        // Assert
        this.MockInteraction.Verify(m => m.RespondAsync(It.IsAny<string>(), It.IsAny<Embed[]>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<AllowedMentions>(), It.IsAny<MessageComponent>(), It.IsAny<Embed>(), It.IsAny<RequestOptions>(), It.IsAny<PollProperties>()), Times.Never);
        _mockTeamRankEmbedCreator.Verify(m => m.CreateAsync(It.IsAny<(int?, string, string?)>(), It.IsAny<ushort?>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetRankAsync_RequiresTeamKey()
    {
        // Arrange
        var eventKey = "2025iscmp";
        var numMatches = (ushort)6;
        var post = false;

        string? response = null;
        this.MockInteraction.Setup(m => m.RespondAsync(It.IsAny<string>(), It.IsAny<Embed[]>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<AllowedMentions>(), It.IsAny<MessageComponent>(), It.IsAny<Embed>(), It.IsAny<RequestOptions>(), It.IsAny<PollProperties>()))
            .Callback((string msg, Embed[] _, bool _, bool _, AllowedMentions _, MessageComponent _, Embed _, RequestOptions _, PollProperties _) => response = msg);

        // Act
        await this.Module.GetRankAsync(string.Empty, eventKey, numMatches, post);

        // Assert
        this.MockInteraction.Verify(m => m.RespondAsync(It.IsAny<string>(), It.IsAny<Embed[]>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<AllowedMentions>(), It.IsAny<MessageComponent>(), It.IsAny<Embed>(), It.IsAny<RequestOptions>(), It.IsAny<PollProperties>()), Times.Once);
        Assert.NotNull(response);
        Assert.Equal("Team key is required.", response);
        _mockTeamRankEmbedCreator.Verify(m => m.CreateAsync(It.IsAny<(int?, string, string?)>(), It.IsAny<ushort?>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}