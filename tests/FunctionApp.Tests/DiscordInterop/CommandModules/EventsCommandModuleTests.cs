namespace FunctionApp.Tests.DiscordInterop.CommandModules;

using Discord;
using Discord.Interactions;
using Discord.Net;

using FunctionApp.Apis;
using FunctionApp.DiscordInterop.CommandModules;
using FunctionApp.DiscordInterop.Embeds;

using Google.Protobuf.WellKnownTypes;

using Microsoft.Extensions.Logging;

using Moq;

using System;
using System.Text.Json;
using System.Threading.Tasks;

using TestCommon;

using TheBlueAlliance.Api;
using TheBlueAlliance.Interfaces.Caching;
using TheBlueAlliance.Model;

using Xunit;
using Xunit.Abstractions;

public class EventsCommandModuleTests : TestWithLogger
{
    private readonly Mock<IInteractionContext> _mockContext;
    private readonly EventsCommandModule _eventsCommandModule;

    public EventsCommandModuleTests(ITestOutputHelper outputHelper) : base(typeof(EventsCommandModule), outputHelper)
    {
        _mockContext = this.Mocker.GetMock<IInteractionContext>();

        this.Mocker.WithSelfMock<IRESTCountries>();
        this.Mocker.Use(new EmbedBuilderFactory(new EmbeddingColorizer(Mock.Of<FRCColors.IClient>(), this.Mocker.Get<ILoggerFactory>().CreateLogger<EmbeddingColorizer>())));
        this.Mocker.WithSelfMock<IEventCache>();
        this.Mocker.WithSelfMock<ITeamCache>();
        this.Mocker.WithSelfMock<IMatchApi>();
        this.Mocker.WithSelfMock<Statbotics.Api.IEventApi>();
        this.Mocker.Use(this.Mocker.Get<ILoggerFactory>().CreateLogger<EventDetail>());

        this.Mocker.AddKeyedService<IEmbedCreator<string>>(nameof(EventDetail), this.Mocker.CreateInstance<EventDetail>());
        this.Mocker.AddKeyedService<IEmbedCreator<(string?, ushort)>>(nameof(Schedule), this.Mocker.CreateInstance<Schedule>());

        this.Mocker.Use(_mockContext.Object);

        var guildMock = this.Mocker.GetMock<IGuild>();
        guildMock.SetupGet(g => g.Name).Returns("Test Guild");
        guildMock.SetupGet(g => g.Id).Returns(12345UL);

        _eventsCommandModule = new EventsCommandModule(this.Mocker);

        ((IInteractionModuleBase)_eventsCommandModule).SetContext(_mockContext.Object);

        _mockContext.SetupGet(c => c.Interaction)
            .Returns(this.Mocker.CreateSelfMock<IDiscordInteraction>());

        var channelMock = this.Mocker.GetMock<IMessageChannel>();
        channelMock.Setup(i => i.EnterTypingState(It.IsAny<RequestOptions>()))
            .Returns(Mock.Of<IDisposable>());
        //.Returns(() =>
        //{
        //    var d = new Mock<IDisposable>();
        //    d.Setup(i => i.Dispose()).Verifiable();

        //    var obj = d.Object;
        //    return obj;
        //});
        _mockContext.SetupGet(c => c.Channel).Returns(channelMock.Object);
    }

    [Fact]
    public async Task GetDetailsAsync_ShouldReturnEventDetails()
    {
        // Arrange
        var eventKey = "2025test";
        var mockEventJson = @"{
                ""key"": ""2025test"",
                ""name"": ""Test Event"",
                ""start_date"": ""2025-01-01"",
                ""end_date"": ""2025-12-31"",
                ""timezone"": ""UTC"",
                ""location_string"": ""Test Location"",
                ""schedule_url"": ""http://example.com/schedule"",
                ""tba_url"": ""http://example.com/tba"",
                ""first_url"": ""http://example.com/first"",
                ""address"": ""123 Test St"",
                ""city"": ""Test City"",
                ""country"": ""Test Country"",
                ""district"": null,
                ""division_keys"": [],
                ""event_code"": ""TEST"",
                ""event_type"": 0,
                ""event_type_string"": ""Test Event Type"",
                ""first_event_code"": ""TEST"",
                ""first_event_id"": ""12345"",
                ""gmaps_place_id"": ""test-place-id"",
                ""gmaps_url"": ""http://example.com/gmaps"",
                ""lat"": 0.0,
                ""lng"": 0.0,
                ""location_name"": ""Test Location"",
                ""parent_event_key"": ""parent-test"",
                ""playoff_type"": null,
                ""playoff_type_string"": ""Test Playoff"",
                ""postal_code"": ""12345"",
                ""short_name"": ""Test Short Name"",
                ""state_prov"": ""Test State"",
                ""webcasts"": [],
                ""website"": ""http://example.com"",
                ""week"": null,
                ""year"": 2025
            }";
        var mockEvent = JsonSerializer.Deserialize<Event>(mockEventJson)!;
        this.Mocker.GetMock<IEventCache>()
            .SetupGet(i => i[eventKey])
            .Returns(mockEvent);

        var interaction = this.Mocker.GetMock<IDiscordInteraction>();
        MessageProperties messageToUser = new();
        interaction.Setup(i => i.ModifyOriginalResponseAsync(It.IsAny<Action<MessageProperties>>(), It.IsAny<RequestOptions>()))
            .Callback<Action<MessageProperties>, RequestOptions>((a, _) => a(messageToUser));
        _mockContext.SetupGet(c => c.Interaction).Returns(interaction.Object);

        // Act
        await _eventsCommandModule.GetDetailsAsync(eventKey, false);

        // Assert
        interaction.Verify(i => i.ModifyOriginalResponseAsync(It.IsAny<Action<MessageProperties>>(), It.IsAny<RequestOptions>()), Times.AtLeastOnce());
        Assert.True(messageToUser.Embeds.IsSpecified);
        Assert.Single(messageToUser.Embeds.Value);
        Assert.Contains(mockEvent.Name, messageToUser.Embeds.Value[0].Title);
    }

    // ...

    [Fact]
    public async Task AddEventAsync_ShouldAddEvent()
    {
        // Arrange
        var eventKey = "2025test";
        var mockEventJson = @"{
                ""key"": ""2025test"",
                ""name"": ""Test Event"",
                ""start_date"": ""2025-01-01"",
                ""end_date"": ""2025-12-31"",
                ""timezone"": ""UTC"",
                ""location_string"": ""Test Location"",
                ""schedule_url"": ""http://example.com/schedule"",
                ""tba_url"": ""http://example.com/tba"",
                ""first_url"": ""http://example.com/first"",
                ""address"": ""123 Test St"",
                ""city"": ""Test City"",
                ""country"": ""Test Country"",
                ""district"": null,
                ""division_keys"": [],
                ""event_code"": ""TEST"",
                ""event_type"": 0,
                ""event_type_string"": ""Test Event Type"",
                ""first_event_code"": ""TEST"",
                ""first_event_id"": ""12345"",
                ""gmaps_place_id"": ""test-place-id"",
                ""gmaps_url"": ""http://example.com/gmaps"",
                ""lat"": 0.0,
                ""lng"": 0.0,
                ""location_name"": ""Test Location"",
                ""parent_event_key"": ""parent-test"",
                ""playoff_type"": null,
                ""playoff_type_string"": ""Test Playoff"",
                ""postal_code"": ""12345"",
                ""short_name"": ""Test Short Name"",
                ""state_prov"": ""Test State"",
                ""webcasts"": [],
                ""website"": ""http://example.com"",
                ""week"": null,
                ""year"": 2025
            }";
        var mockEvent = JsonSerializer.Deserialize<Event>(mockEventJson)!;
        this.Mocker.GetMock<IEventCache>()
            .SetupGet(i => i[eventKey])
            .Returns(mockEvent);

        var guildUser = this.Mocker.GetMock<IGuildUser>();
        guildUser.SetupGet(u => u.GuildPermissions).Returns(new GuildPermissions(createEvents: true));

        var botGuildUser = new Mock<IGuildUser>();
        botGuildUser.SetupGet(u => u.GuildPermissions).Returns(new GuildPermissions(manageEvents: true));

        var guildMock = this.Mocker.GetMock<IGuild>();
        guildMock.Setup(i => i.GetUserAsync(It.IsAny<ulong>(), It.IsAny<CacheMode>(), It.IsAny<RequestOptions>()))
            .ReturnsAsync(guildUser.Object);
        guildMock.Setup(i => i.GetCurrentUserAsync(It.IsAny<CacheMode>(), It.IsAny<RequestOptions>()))
            .ReturnsAsync(botGuildUser.Object);
        string eventDescription = string.Empty;
        guildMock.Setup(i => i.CreateEventAsync(It.IsAny<string>(), It.IsAny<DateTimeOffset>(), It.IsAny<GuildScheduledEventType>(), It.IsAny<GuildScheduledEventPrivacyLevel>(), It.IsAny<string>(), It.IsAny<DateTimeOffset>(), It.IsAny<ulong?>(), It.IsAny<string>(), It.IsAny<Image?>(), It.IsAny<RequestOptions>()))
            .ReturnsAsync((string name, DateTimeOffset startTime, GuildScheduledEventType type, GuildScheduledEventPrivacyLevel privacy, string description, DateTimeOffset endTime, ulong? channelId, string location, Image? coverImage, RequestOptions _) =>
            {
                var e = new Mock<IGuildScheduledEvent>();
                e.SetupGet(i => i.Name).Returns(name);
                e.SetupGet(i => i.StartTime).Returns(startTime);
                e.SetupGet(i => i.EndTime).Returns(endTime);
                e.SetupGet(i => i.Type).Returns(type);
                e.SetupGet(i => i.PrivacyLevel).Returns(privacy);
                e.SetupGet(i => i.ChannelId).Returns(channelId);
                e.SetupGet(i => i.GuildId).Returns(this.Mocker.Get<IGuild>().Id);
                e.SetupGet(i => i.Location).Returns(location);
                e.SetupGet(i => i.Description).Returns(description);
                eventDescription = description;

                return e.Object;
            });

        _mockContext.Setup(c => c.Guild).Returns(guildMock.Object);
        _mockContext.Setup(c => c.User).Returns(guildUser.Object);

        var interaction = this.Mocker.GetMock<IDiscordInteraction>();
        MessageProperties messageToUser = new();
        interaction.Setup(i => i.ModifyOriginalResponseAsync(It.IsAny<Action<MessageProperties>>(), It.IsAny<RequestOptions>()))
            .Callback<Action<MessageProperties>, RequestOptions>((a, _) => a(messageToUser));
        _mockContext.SetupGet(c => c.Interaction).Returns(interaction.Object);

        // Act
        await _eventsCommandModule.AddEventAsync(eventKey, "Test Event", "Test Description", this.Mocker.Get<IMessageChannel>(), false);

        // Assert
        this.Mocker.GetMock<IGuild>().Verify(g => g.CreateEventAsync(It.IsAny<string>(), It.IsAny<DateTimeOffset>(), It.IsAny<GuildScheduledEventType>(), It.IsAny<GuildScheduledEventPrivacyLevel>(), It.IsAny<string>(), It.IsAny<DateTimeOffset>(), It.IsAny<ulong?>(), It.IsAny<string>(), It.IsAny<Image?>(), It.IsAny<RequestOptions>()), Times.Once);

        Assert.Contains("[Event created]", messageToUser.Content.GetValueOrDefault(string.Empty));
        Assert.Contains("[Test Location, Test City, Test State, Test Country]", eventDescription);
    }

    [Fact]
    public async Task GetScheduleAsync_ShouldReturnSchedule()
    {
        // Arrange
        var eventKey = "2025test";
        var mockEventJson = @"{
                ""key"": ""2025test"",
                ""name"": ""Test Event"",
                ""start_date"": ""2025-01-01"",
                ""end_date"": ""2025-12-31"",
                ""timezone"": ""UTC"",
                ""location_string"": ""Test Location"",
                ""schedule_url"": ""http://example.com/schedule"",
                ""tba_url"": ""http://example.com/tba"",
                ""first_url"": ""http://example.com/first"",
                ""address"": ""123 Test St"",
                ""city"": ""Test City"",
                ""country"": ""Test Country"",
                ""district"": null,
                ""division_keys"": [],
                ""event_code"": ""TEST"",
                ""event_type"": 0,
                ""event_type_string"": ""Test Event Type"",
                ""first_event_code"": ""TEST"",
                ""first_event_id"": ""12345"",
                ""gmaps_place_id"": ""test-place-id"",
                ""gmaps_url"": ""http://example.com/gmaps"",
                ""lat"": 0.0,
                ""lng"": 0.0,
                ""location_name"": ""Test Location"",
                ""parent_event_key"": ""parent-test"",
                ""playoff_type"": null,
                ""playoff_type_string"": ""Test Playoff"",
                ""postal_code"": ""12345"",
                ""short_name"": ""Test Short Name"",
                ""state_prov"": ""Test State"",
                ""webcasts"": [],
                ""website"": ""http://example.com"",
                ""week"": null,
                ""year"": 2025
            }";
        var mockEvent = JsonSerializer.Deserialize<Event>(mockEventJson)!;
        this.Mocker.GetMock<IEventCache>()
            .SetupGet(i => i[eventKey])
            .Returns(mockEvent);

        var interaction = this.Mocker.GetMock<IDiscordInteraction>();
        MessageProperties messageToUser = new();
        interaction.Setup(i => i.ModifyOriginalResponseAsync(It.IsAny<Action<MessageProperties>>(), It.IsAny<RequestOptions>()))
            .Callback<Action<MessageProperties>, RequestOptions>((a, _) => a(messageToUser));
        _mockContext.SetupGet(c => c.Interaction).Returns(interaction.Object);

        // Act
        await _eventsCommandModule.GetScheduleAsync(eventKey, null, 6, false);

        // Assert
        interaction.Verify(i => i.ModifyOriginalResponseAsync(It.IsAny<Action<MessageProperties>>(), It.IsAny<RequestOptions>()), Times.Once);
        Assert.True(messageToUser.Embeds.IsSpecified);
        Assert.Single(messageToUser.Embeds.Value);
        Assert.Equal("Schedule", messageToUser.Embeds.Value[0].Title);
    }

    [Fact]
    public async Task GetDetailsAsync_ShouldReturnError_WhenEventKeyIsNullOrWhitespace()
    {
        // Arrange
        var eventKey = " ";
        var interaction = this.Mocker.GetMock<IDiscordInteraction>();
        MessageProperties messageToUser = new();
        interaction.Setup(i => i.ModifyOriginalResponseAsync(It.IsAny<Action<MessageProperties>>(), It.IsAny<RequestOptions>()))
            .Callback<Action<MessageProperties>, RequestOptions>((a, _) => a(messageToUser));
        _mockContext.SetupGet(c => c.Interaction).Returns(interaction.Object);

        // Act
        await _eventsCommandModule.GetDetailsAsync(eventKey, false);

        // Assert
        interaction.Verify(i => i.ModifyOriginalResponseAsync(It.IsAny<Action<MessageProperties>>(), It.IsAny<RequestOptions>()), Times.Once);
        Assert.Equal("Event key is required.", messageToUser.Content.GetValueOrDefault(string.Empty));
    }

    [Fact]
    public async Task AddEventAsync_ShouldReturnError_WhenEventKeyIsNullOrWhitespace()
    {
        // Arrange
        var eventKey = " ";
        var interaction = this.Mocker.GetMock<IDiscordInteraction>();
        MessageProperties messageToUser = new();
        interaction.Setup(i => i.ModifyOriginalResponseAsync(It.IsAny<Action<MessageProperties>>(), It.IsAny<RequestOptions>()))
            .Callback<Action<MessageProperties>, RequestOptions>((a, _) => a(messageToUser));
        _mockContext.SetupGet(c => c.Interaction).Returns(interaction.Object);

        // Act
        await _eventsCommandModule.AddEventAsync(eventKey, null, null, null, false);

        // Assert
        interaction.Verify(i => i.ModifyOriginalResponseAsync(It.IsAny<Action<MessageProperties>>(), It.IsAny<RequestOptions>()), Times.Once);
        Assert.Equal("Event key is required.", messageToUser.Content.GetValueOrDefault(string.Empty));
    }

    [Fact]
    public async Task AddEventAsync_ShouldReturnError_WhenContextGuildIsNull()
    {
        // Arrange
        var eventKey = "2025test";
        var interaction = this.Mocker.GetMock<IDiscordInteraction>();
        MessageProperties messageToUser = new();
        interaction.Setup(i => i.ModifyOriginalResponseAsync(It.IsAny<Action<MessageProperties>>(), It.IsAny<RequestOptions>()))
            .Callback<Action<MessageProperties>, RequestOptions>((a, _) => a(messageToUser));
        _mockContext.SetupGet(c => c.Interaction).Returns(interaction.Object);
        _mockContext.SetupGet(c => c.Guild).Returns((IGuild?)null);

        // Act
        await _eventsCommandModule.AddEventAsync(eventKey, null, null, null, false);

        // Assert
        interaction.Verify(i => i.ModifyOriginalResponseAsync(It.IsAny<Action<MessageProperties>>(), It.IsAny<RequestOptions>()), Times.Once);
        Assert.Equal("This command can only be used in a server.", messageToUser.Content.GetValueOrDefault(string.Empty));
    }

    [Fact]
    public async Task AddEventAsync_ShouldReturnError_WhenUserLacksCreateEventsPermission()
    {
        // Arrange
        var eventKey = "2025test";
        var interaction = this.Mocker.GetMock<IDiscordInteraction>();
        MessageProperties messageToUser = new();
        interaction.Setup(i => i.ModifyOriginalResponseAsync(It.IsAny<Action<MessageProperties>>(), It.IsAny<RequestOptions>()))
            .Callback<Action<MessageProperties>, RequestOptions>((a, _) => a(messageToUser));
        _mockContext.SetupGet(c => c.Interaction).Returns(interaction.Object);

        var guildUser = this.Mocker.GetMock<IGuildUser>();
        guildUser.SetupGet(u => u.GuildPermissions).Returns(new GuildPermissions(createEvents: false));

        var guildMock = this.Mocker.GetMock<IGuild>();
        guildMock.Setup(i => i.GetUserAsync(It.IsAny<ulong>(), It.IsAny<CacheMode>(), It.IsAny<RequestOptions>()))
            .ReturnsAsync(guildUser.Object);

        _mockContext.Setup(c => c.Guild).Returns(guildMock.Object);
        _mockContext.Setup(c => c.User).Returns(guildUser.Object);

        // Act
        await _eventsCommandModule.AddEventAsync(eventKey, null, null, null, false);

        // Assert
        interaction.Verify(i => i.ModifyOriginalResponseAsync(It.IsAny<Action<MessageProperties>>(), It.IsAny<RequestOptions>()), Times.Once);
        Assert.Equal("You do not have permission to create events in this server.", messageToUser.Content.GetValueOrDefault(string.Empty));
    }

    [Fact]
    public async Task AddEventAsync_ShouldReturnError_WhenBotLacksManageEventsPermission()
    {
        // Arrange
        var eventKey = "2025test";
        var interaction = this.Mocker.GetMock<IDiscordInteraction>();
        MessageProperties messageToUser = new();
        interaction.Setup(i => i.ModifyOriginalResponseAsync(It.IsAny<Action<MessageProperties>>(), It.IsAny<RequestOptions>()))
            .Callback<Action<MessageProperties>, RequestOptions>((a, _) => a(messageToUser));
        _mockContext.SetupGet(c => c.Interaction).Returns(interaction.Object);

        var guildUser = this.Mocker.GetMock<IGuildUser>();
        guildUser.SetupGet(u => u.GuildPermissions).Returns(new GuildPermissions(createEvents: true));

        var botGuildUser = new Mock<IGuildUser>();
        botGuildUser.SetupGet(u => u.GuildPermissions).Returns(new GuildPermissions(manageEvents: false));

        var guildMock = this.Mocker.GetMock<IGuild>();
        guildMock.Setup(i => i.GetUserAsync(It.IsAny<ulong>(), It.IsAny<CacheMode>(), It.IsAny<RequestOptions>()))
            .ReturnsAsync(guildUser.Object);
        guildMock.Setup(i => i.GetCurrentUserAsync(It.IsAny<CacheMode>(), It.IsAny<RequestOptions>()))
            .ReturnsAsync(botGuildUser.Object);

        _mockContext.Setup(c => c.Guild).Returns(guildMock.Object);
        _mockContext.Setup(c => c.User).Returns(guildUser.Object);

        // Act
        await _eventsCommandModule.AddEventAsync(eventKey, null, null, null, false);

        // Assert
        interaction.Verify(i => i.ModifyOriginalResponseAsync(It.IsAny<Action<MessageProperties>>(), It.IsAny<RequestOptions>()), Times.Once);
        Assert.Equal("I do not have permission to create events in this server. Talk to your admin about granting me this!", messageToUser.Content.GetValueOrDefault(string.Empty));
    }

    [Fact]
    public async Task GetDetailsAsync_ShouldReturnError_WhenTryDeferAsyncReturnsNull()
    {
        // Arrange
        var eventKey = "2025test";
        this.Mocker.GetMock<IMessageChannel>()
            .Setup(i => i.EnterTypingState(It.IsAny<RequestOptions>()))
            .Throws(new HttpException(System.Net.HttpStatusCode.InternalServerError, this.Mocker.CreateSelfMock<IRequest>(), DiscordErrorCode.UnknownInteraction)); // forces the defer to return null

        var interaction = this.Mocker.GetMock<IDiscordInteraction>();
        _mockContext.SetupGet(c => c.Interaction).Returns(interaction.Object);

        // Act
        await _eventsCommandModule.GetDetailsAsync(eventKey, false);

        // Assert
        interaction.Verify(i => i.ModifyOriginalResponseAsync(It.IsAny<Action<MessageProperties>>(), It.IsAny<RequestOptions>()), Times.Never);
    }

    [Fact]
    public async Task GetScheduleAsync_ShouldReturnError_WhenTryDeferAsyncReturnsNull()
    {
        // Arrange
        var eventKey = "2025test";
        this.Mocker.GetMock<IMessageChannel>()
            .Setup(i => i.EnterTypingState(It.IsAny<RequestOptions>()))
            .Throws(new HttpException(System.Net.HttpStatusCode.InternalServerError, this.Mocker.CreateSelfMock<IRequest>(), DiscordErrorCode.InteractionHasAlreadyBeenAcknowledged)); // forces the defer to return null

        var interaction = this.Mocker.GetMock<IDiscordInteraction>();
        _mockContext.SetupGet(c => c.Interaction).Returns(interaction.Object);

        // Act
        await _eventsCommandModule.GetScheduleAsync(eventKey, null, 6, false);

        // Assert
        interaction.Verify(i => i.ModifyOriginalResponseAsync(It.IsAny<Action<MessageProperties>>(), It.IsAny<RequestOptions>()), Times.Never);
    }

    [Fact]
    public async Task AddEventAsync_ShouldReturnError_WhenTryDeferAsyncReturnsNull()
    {
        // Arrange
        var eventKey = "2025test";
        this.Mocker.GetMock<IMessageChannel>()
            .Setup(i => i.EnterTypingState(It.IsAny<RequestOptions>()))
            .Throws(new HttpException(System.Net.HttpStatusCode.InternalServerError, this.Mocker.CreateSelfMock<IRequest>(), DiscordErrorCode.InteractionHasAlreadyBeenAcknowledged)); // forces the defer to return null

        var interaction = this.Mocker.GetMock<IDiscordInteraction>();
        _mockContext.SetupGet(c => c.Interaction).Returns(interaction.Object);

        // Act
        await _eventsCommandModule.AddEventAsync(eventKey, null, null, null, false);

        // Assert
        interaction.Verify(i => i.ModifyOriginalResponseAsync(It.IsAny<Action<MessageProperties>>(), It.IsAny<RequestOptions>()), Times.Never);
    }

    [Fact]
    public async Task AddEventAsync_ShouldReturnError_WhenEventNotFound()
    {
        // Arrange
        var eventKey = "2025test";
        this.Mocker.GetMock<IEventCache>()
            .SetupGet(i => i[eventKey])
            .Throws<KeyNotFoundException>();

        var interaction = this.Mocker.GetMock<IDiscordInteraction>();
        MessageProperties messageToUser = new();
        interaction.Setup(i => i.ModifyOriginalResponseAsync(It.IsAny<Action<MessageProperties>>(), It.IsAny<RequestOptions>()))
            .Callback<Action<MessageProperties>, RequestOptions>((a, _) => a(messageToUser));
        _mockContext.SetupGet(c => c.Interaction).Returns(interaction.Object);

        var guildUser = this.Mocker.GetMock<IGuildUser>();
        guildUser.SetupGet(u => u.GuildPermissions).Returns(new GuildPermissions(createEvents: true));

        var botGuildUser = new Mock<IGuildUser>();
        botGuildUser.SetupGet(u => u.GuildPermissions).Returns(new GuildPermissions(manageEvents: true));

        var guildMock = this.Mocker.GetMock<IGuild>();
        guildMock.Setup(i => i.GetUserAsync(It.IsAny<ulong>(), It.IsAny<CacheMode>(), It.IsAny<RequestOptions>()))
            .ReturnsAsync(guildUser.Object);
        guildMock.Setup(i => i.GetCurrentUserAsync(It.IsAny<CacheMode>(), It.IsAny<RequestOptions>()))
            .ReturnsAsync(botGuildUser.Object);

        _mockContext.Setup(c => c.Guild).Returns(guildMock.Object);
        _mockContext.Setup(c => c.User).Returns(guildUser.Object);

        // Act
        await _eventsCommandModule.AddEventAsync(eventKey, null, null, null, false);

        // Assert
        interaction.Verify(i => i.ModifyOriginalResponseAsync(It.IsAny<Action<MessageProperties>>(), It.IsAny<RequestOptions>()), Times.Once);
        Assert.Equal("Event not found.", messageToUser.Content.GetValueOrDefault(string.Empty));
    }

    [Fact]
    public async Task AddEventAsync_ShouldReturnError_WhenGeneralExceptionThrown()
    {
        // Arrange
        var eventKey = "2025test";

        this.Mocker.GetMock<IEventCache>()
            .SetupGet(i => i[eventKey])
            .Throws<Exception>();

        var interaction = this.Mocker.GetMock<IDiscordInteraction>();
        MessageProperties messageToUser = new();
        interaction.Setup(i => i.ModifyOriginalResponseAsync(It.IsAny<Action<MessageProperties>>(), It.IsAny<RequestOptions>()))
            .Callback<Action<MessageProperties>, RequestOptions>((a, _) => a(messageToUser));
        _mockContext.SetupGet(c => c.Interaction).Returns(interaction.Object);

        var guildUser = this.Mocker.GetMock<IGuildUser>();
        guildUser.SetupGet(u => u.GuildPermissions).Returns(new GuildPermissions(createEvents: true));

        var botGuildUser = new Mock<IGuildUser>();
        botGuildUser.SetupGet(u => u.GuildPermissions).Returns(new GuildPermissions(manageEvents: true));

        var guildMock = this.Mocker.GetMock<IGuild>();
        guildMock.Setup(i => i.GetUserAsync(It.IsAny<ulong>(), It.IsAny<CacheMode>(), It.IsAny<RequestOptions>()))
            .ReturnsAsync(guildUser.Object);
        guildMock.Setup(i => i.GetCurrentUserAsync(It.IsAny<CacheMode>(), It.IsAny<RequestOptions>()))
            .ReturnsAsync(botGuildUser.Object);

        _mockContext.Setup(c => c.Guild).Returns(guildMock.Object);
        _mockContext.Setup(c => c.User).Returns(guildUser.Object);

        // Act
        await _eventsCommandModule.AddEventAsync(eventKey, null, null, null, false);

        // Assert
        interaction.Verify(i => i.ModifyOriginalResponseAsync(It.IsAny<Action<MessageProperties>>(), It.IsAny<RequestOptions>()), Times.Once);
        Assert.Equal("An error occurred while creating the event. Try again or contact your admin to investigate.", messageToUser.Content.GetValueOrDefault(string.Empty));
    }

    [Fact]
    public async Task AddEventAsync_ShouldPostLinkInContextChannel_WhenPostIsTrueAndChannelIsNull()
    {
        // Arrange
        var eventKey = "2025test";
        var mockEventJson = @"{
                ""key"": ""2025test"",
                ""name"": ""Test Event"",
                ""start_date"": ""2025-01-01"",
                ""end_date"": ""2025-12-31"",
                ""timezone"": ""UTC"",
                ""location_string"": ""Test Location"",
                ""schedule_url"": ""http://example.com/schedule"",
                ""tba_url"": ""http://example.com/tba"",
                ""first_url"": ""http://example.com/first"",
                ""address"": ""123 Test St"",
                ""city"": ""Test City"",
                ""country"": ""Test Country"",
                ""district"": null,
                ""division_keys"": [],
                ""event_code"": ""TEST"",
                ""event_type"": 0,
                ""event_type_string"": ""Test Event Type"",
                ""first_event_code"": ""TEST"",
                ""first_event_id"": ""12345"",
                ""gmaps_place_id"": ""test-place-id"",
                ""gmaps_url"": ""http://example.com/gmaps"",
                ""lat"": 0.0,
                ""lng"": 0.0,
                ""location_name"": ""Test Location"",
                ""parent_event_key"": ""parent-test"",
                ""playoff_type"": null,
                ""playoff_type_string"": ""Test Playoff"",
                ""postal_code"": ""12345"",
                ""short_name"": ""Test Short Name"",
                ""state_prov"": ""Test State"",
                ""webcasts"": [],
                ""website"": ""http://example.com"",
                ""week"": null,
                ""year"": 2025
            }";
        var mockEvent = JsonSerializer.Deserialize<Event>(mockEventJson)!;
        this.Mocker.GetMock<IEventCache>()
            .SetupGet(i => i[eventKey])
            .Returns(mockEvent);

        var guildUser = this.Mocker.GetMock<IGuildUser>();
        guildUser.SetupGet(u => u.GuildPermissions).Returns(new GuildPermissions(createEvents: true));

        var botGuildUser = new Mock<IGuildUser>();
        botGuildUser.SetupGet(u => u.GuildPermissions).Returns(new GuildPermissions(manageEvents: true));

        var guildMock = this.Mocker.GetMock<IGuild>();
        guildMock.Setup(i => i.GetUserAsync(It.IsAny<ulong>(), It.IsAny<CacheMode>(), It.IsAny<RequestOptions>()))
            .ReturnsAsync(guildUser.Object);
        guildMock.Setup(i => i.GetCurrentUserAsync(It.IsAny<CacheMode>(), It.IsAny<RequestOptions>()))
            .ReturnsAsync(botGuildUser.Object);
        guildMock.Setup(i => i.CreateEventAsync(It.IsAny<string>(), It.IsAny<DateTimeOffset>(), It.IsAny<GuildScheduledEventType>(), It.IsAny<GuildScheduledEventPrivacyLevel>(), It.IsAny<string>(), It.IsAny<DateTimeOffset>(), It.IsAny<ulong?>(), It.IsAny<string>(), It.IsAny<Image?>(), It.IsAny<RequestOptions>()))
            .ReturnsAsync(() =>
            {
                var e = new Mock<IGuildScheduledEvent>();
                e.SetupGet(i => i.Name).Returns("Test Event");
                e.SetupGet(i => i.Description).Returns("Test Description");
                return e.Object;
            });

        _mockContext.Setup(c => c.Guild).Returns(guildMock.Object);
        _mockContext.Setup(c => c.User).Returns(guildUser.Object);

        var interaction = this.Mocker.GetMock<IDiscordInteraction>();
        MessageProperties messageToUser = new();
        interaction.Setup(i => i.ModifyOriginalResponseAsync(It.IsAny<Action<MessageProperties>>(), It.IsAny<RequestOptions>()))
            .Callback<Action<MessageProperties>, RequestOptions>((a, _) => a(messageToUser));
        _mockContext.SetupGet(c => c.Interaction).Returns(interaction.Object);

        var channelMock = this.Mocker.GetMock<IMessageChannel>();
        _mockContext.SetupGet(c => c.Channel).Returns(channelMock.Object);

        // Act
        await _eventsCommandModule.AddEventAsync(eventKey, "Test Event", "Test Description", null, true);

        // Assert
        channelMock.Verify(c => c.SendMessageAsync(It.Is<string>(s => s.Contains("https://discord.com/events/")), It.IsAny<bool>(), It.IsAny<Embed>(), It.IsAny<RequestOptions>(), It.IsAny<AllowedMentions>(), It.IsAny<MessageReference>(), It.IsAny<MessageComponent>(), It.IsAny<ISticker[]>(), It.IsAny<Embed[]>(), It.IsAny<MessageFlags>(), It.IsAny<PollProperties>()), Times.Once);
    }

    [Fact]
    public async Task AddEventAsync_ShouldPostLinkInSpecifiedChannel_WhenPostIsTrueAndChannelIsNotNull()
    {
        // Arrange
        var eventKey = "2025test";
        var mockEventJson = @"{
                ""key"": ""2025test"",
                ""name"": ""Test Event"",
                ""start_date"": ""2025-01-01"",
                ""end_date"": ""2025-12-31"",
                ""timezone"": ""UTC"",
                ""location_string"": ""Test Location"",
                ""schedule_url"": ""http://example.com/schedule"",
                ""tba_url"": ""http://example.com/tba"",
                ""first_url"": ""http://example.com/first"",
                ""address"": ""123 Test St"",
                ""city"": ""Test City"",
                ""country"": ""Test Country"",
                ""district"": null,
                ""division_keys"": [],
                ""event_code"": ""TEST"",
                ""event_type"": 0,
                ""event_type_string"": ""Test Event Type"",
                ""first_event_code"": ""TEST"",
                ""first_event_id"": ""12345"",
                ""gmaps_place_id"": ""test-place-id"",
                ""gmaps_url"": ""http://example.com/gmaps"",
                ""lat"": 0.0,
                ""lng"": 0.0,
                ""location_name"": ""Test Location"",
                ""parent_event_key"": ""parent-test"",
                ""playoff_type"": null,
                ""playoff_type_string"": ""Test Playoff"",
                ""postal_code"": ""12345"",
                ""short_name"": ""Test Short Name"",
                ""state_prov"": ""Test State"",
                ""webcasts"": [],
                ""website"": ""http://example.com"",
                ""week"": null,
                ""year"": 2025
            }";
        var mockEvent = JsonSerializer.Deserialize<Event>(mockEventJson)!;
        this.Mocker.GetMock<IEventCache>()
            .SetupGet(i => i[eventKey])
            .Returns(mockEvent);

        var guildUser = this.Mocker.GetMock<IGuildUser>();
        guildUser.SetupGet(u => u.GuildPermissions).Returns(new GuildPermissions(createEvents: true));

        var botGuildUser = new Mock<IGuildUser>();
        botGuildUser.SetupGet(u => u.GuildPermissions).Returns(new GuildPermissions(manageEvents: true));

        var guildMock = this.Mocker.GetMock<IGuild>();
        guildMock.Setup(i => i.GetUserAsync(It.IsAny<ulong>(), It.IsAny<CacheMode>(), It.IsAny<RequestOptions>()))
            .ReturnsAsync(guildUser.Object);
        guildMock.Setup(i => i.GetCurrentUserAsync(It.IsAny<CacheMode>(), It.IsAny<RequestOptions>()))
            .ReturnsAsync(botGuildUser.Object);
        guildMock.Setup(i => i.CreateEventAsync(It.IsAny<string>(), It.IsAny<DateTimeOffset>(), It.IsAny<GuildScheduledEventType>(), It.IsAny<GuildScheduledEventPrivacyLevel>(), It.IsAny<string>(), It.IsAny<DateTimeOffset>(), It.IsAny<ulong?>(), It.IsAny<string>(), It.IsAny<Image?>(), It.IsAny<RequestOptions>()))
            .ReturnsAsync(() =>
            {
                var e = new Mock<IGuildScheduledEvent>();
                e.SetupGet(i => i.Name).Returns("Test Event");
                e.SetupGet(i => i.Description).Returns("Test Description");
                return e.Object;
            });

        _mockContext.Setup(c => c.Guild).Returns(guildMock.Object);
        _mockContext.Setup(c => c.User).Returns(guildUser.Object);

        var interaction = this.Mocker.GetMock<IDiscordInteraction>();
        MessageProperties messageToUser = new();
        interaction.Setup(i => i.ModifyOriginalResponseAsync(It.IsAny<Action<MessageProperties>>(), It.IsAny<RequestOptions>()))
            .Callback<Action<MessageProperties>, RequestOptions>((a, _) => a(messageToUser));
        _mockContext.SetupGet(c => c.Interaction).Returns(interaction.Object);

        var channelMock = this.Mocker.GetMock<IMessageChannel>();

        // Act
        await _eventsCommandModule.AddEventAsync(eventKey, "Test Event", "Test Description", channelMock.Object, true);

        // Assert
        channelMock.Verify(c => c.SendMessageAsync(It.Is<string>(s => s.Contains("https://discord.com/events/")), It.IsAny<bool>(), It.IsAny<Embed>(), It.IsAny<RequestOptions>(), It.IsAny<AllowedMentions>(), It.IsAny<MessageReference>(), It.IsAny<MessageComponent>(), It.IsAny<ISticker[]>(), It.IsAny<Embed[]>(), It.IsAny<MessageFlags>(), It.IsAny<PollProperties>()), Times.Once);
    }

    [Fact]
    public async Task GetScheduleAsync_ShouldReturnError_WhenEventKeyIsNullOrWhitespace()
    {
        // Arrange
        var eventKey = " ";
        var interaction = this.Mocker.GetMock<IDiscordInteraction>();
        MessageProperties messageToUser = new();
        interaction.Setup(i => i.ModifyOriginalResponseAsync(It.IsAny<Action<MessageProperties>>(), It.IsAny<RequestOptions>()))
            .Callback<Action<MessageProperties>, RequestOptions>((a, _) => a(messageToUser));
        _mockContext.SetupGet(c => c.Interaction).Returns(interaction.Object);

        // Act
        await _eventsCommandModule.GetScheduleAsync(eventKey, null, 6, false);

        // Assert
        interaction.Verify(i => i.ModifyOriginalResponseAsync(It.IsAny<Action<MessageProperties>>(), It.IsAny<RequestOptions>()), Times.Once);
        Assert.Equal("Event key is required.", messageToUser.Content.GetValueOrDefault(string.Empty));
    }

    [Fact]
    public async Task AddEventAsync_ShouldAppendLocationString_WhenGmapsUrlIsNullOrEmpty()
    {
        // Arrange
        var eventKey = "2025test";
        var mockEventJson = @"{
                ""key"": ""2025test"",
                ""name"": ""Test Event"",
                ""start_date"": ""2025-01-01"",
                ""end_date"": ""2025-12-31"",
                ""timezone"": ""UTC"",
                ""location_string"": ""Test Location"",
                ""schedule_url"": ""http://example.com/schedule"",
                ""tba_url"": ""http://example.com/tba"",
                ""first_url"": ""http://example.com/first"",
                ""address"": ""123 Test St"",
                ""city"": ""Test City"",
                ""country"": ""Test Country"",
                ""district"": null,
                ""division_keys"": [],
                ""event_code"": ""TEST"",
                ""event_type"": 0,
                ""event_type_string"": ""Test Event Type"",
                ""first_event_code"": ""TEST"",
                ""first_event_id"": ""12345"",
                ""gmaps_place_id"": ""test-place-id"",
                ""gmaps_url"": """",
                ""lat"": 0.0,
                ""lng"": 0.0,
                ""location_name"": ""Test Location"",
                ""parent_event_key"": ""parent-test"",
                ""playoff_type"": null,
                ""playoff_type_string"": ""Test Playoff"",
                ""postal_code"": ""12345"",
                ""short_name"": ""Test Short Name"",
                ""state_prov"": ""Test State"",
                ""webcasts"": [],
                ""website"": ""http://example.com"",
                ""week"": null,
                ""year"": 2025
            }";
        var mockEvent = JsonSerializer.Deserialize<Event>(mockEventJson)!;
        this.Mocker.GetMock<IEventCache>()
            .SetupGet(i => i[eventKey])
            .Returns(mockEvent);

        var guildUser = this.Mocker.GetMock<IGuildUser>();
        guildUser.SetupGet(u => u.GuildPermissions).Returns(new GuildPermissions(createEvents: true));

        var botGuildUser = new Mock<IGuildUser>();
        botGuildUser.SetupGet(u => u.GuildPermissions).Returns(new GuildPermissions(manageEvents: true));

        var guildMock = this.Mocker.GetMock<IGuild>();
        guildMock.Setup(i => i.GetUserAsync(It.IsAny<ulong>(), It.IsAny<CacheMode>(), It.IsAny<RequestOptions>()))
            .ReturnsAsync(guildUser.Object);
        guildMock.Setup(i => i.GetCurrentUserAsync(It.IsAny<CacheMode>(), It.IsAny<RequestOptions>()))
            .ReturnsAsync(botGuildUser.Object);
        string eventDescription = string.Empty;
        guildMock.Setup(i => i.CreateEventAsync(It.IsAny<string>(), It.IsAny<DateTimeOffset>(), It.IsAny<GuildScheduledEventType>(), It.IsAny<GuildScheduledEventPrivacyLevel>(), It.IsAny<string>(), It.IsAny<DateTimeOffset>(), It.IsAny<ulong?>(), It.IsAny<string>(), It.IsAny<Image?>(), It.IsAny<RequestOptions>()))
            .ReturnsAsync((string name, DateTimeOffset startTime, GuildScheduledEventType type, GuildScheduledEventPrivacyLevel privacy, string description, DateTimeOffset endTime, ulong? channelId, string location, Image? coverImage, RequestOptions _) =>
            {
                var e = new Mock<IGuildScheduledEvent>();
                e.SetupGet(i => i.Name).Returns(name);
                e.SetupGet(i => i.StartTime).Returns(startTime);
                e.SetupGet(i => i.EndTime).Returns(endTime);
                e.SetupGet(i => i.Type).Returns(type);
                e.SetupGet(i => i.PrivacyLevel).Returns(privacy);
                e.SetupGet(i => i.ChannelId).Returns(channelId);
                e.SetupGet(i => i.GuildId).Returns(this.Mocker.Get<IGuild>().Id);
                e.SetupGet(i => i.Location).Returns(location);
                e.SetupGet(i => i.Description).Returns(description);
                eventDescription = description;

                return e.Object;
            });

        _mockContext.Setup(c => c.Guild).Returns(guildMock.Object);
        _mockContext.Setup(c => c.User).Returns(guildUser.Object);

        var interaction = this.Mocker.GetMock<IDiscordInteraction>();
        MessageProperties messageToUser = new();
        interaction.Setup(i => i.ModifyOriginalResponseAsync(It.IsAny<Action<MessageProperties>>(), It.IsAny<RequestOptions>()))
            .Callback<Action<MessageProperties>, RequestOptions>((a, _) => a(messageToUser));
        _mockContext.SetupGet(c => c.Interaction).Returns(interaction.Object);

        var channelMock = this.Mocker.GetMock<IMessageChannel>();

        // Act
        await _eventsCommandModule.AddEventAsync(eventKey, "Test Event", "Test Description", channelMock.Object, false);

        // Assert
        interaction.Verify(i => i.ModifyOriginalResponseAsync(It.IsAny<Action<MessageProperties>>(), It.IsAny<RequestOptions>()), Times.Once);
        Assert.Contains("Test Location, Test City, Test State, Test Country", eventDescription);
        Assert.DoesNotContain("[Test Location, Test City, Test State, Test Country]", eventDescription);
    }
}
