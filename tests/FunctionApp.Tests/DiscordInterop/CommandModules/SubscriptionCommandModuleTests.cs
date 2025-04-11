namespace FunctionApp.Tests.DiscordInterop.CommandModules;

using Azure;
using Azure.Data.Tables;

using Discord;
using Discord.Net;

using FunctionApp.DiscordInterop.CommandModules;
using FunctionApp.Storage.TableEntities;
using FunctionApp.Subscription;

using Microsoft.Extensions.Logging;

using Moq;

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using TestCommon;

using TheBlueAlliance.Api;
using TheBlueAlliance.Model;

using Xunit;
using Xunit.Abstractions;

public sealed class SubscriptionCommandModuleTests : TestWithDiscordInteraction<SubscriptionCommandModule>
{
    private readonly Mock<TableClient> _teamSubscriptionTableMock;
    private readonly Mock<TableClient> _eventSubscriptionTableMock;

    private static readonly Team _utTeam = JsonSerializer.Deserialize<Team>("""
                {
        	"address": null,
        	"city": "Maple Valley",
        	"country": "USA",
        	"gmaps_place_id": null,
        	"gmaps_url": null,
        	"key": "frc2046",
        	"lat": null,
        	"lng": null,
        	"location_name": null,
        	"motto": null,
        	"name": "Washington State OSPI/The Truck Shop/1-800-Got-Junk/West Coast Products&Tahoma Senior High School",
        	"nickname": "Bear Metal",
        	"postal_code": "98038",
        	"rookie_year": 2007,
        	"school_name": "Tahoma Senior High School",
        	"state_prov": "Washington",
        	"team_number": 2046,
        	"website": "http://tahomarobotics.org/"
        }
        """)!;
    private static readonly Team _utTeam2 = JsonSerializer.Deserialize<Team>("""
        {
        	"address": null,
        	"city": "Mountlake Terrace",
        	"country": "USA",
        	"gmaps_place_id": null,
        	"gmaps_url": null,
        	"key": "frc1778",
        	"lat": null,
        	"lng": null,
        	"location_name": null,
        	"motto": null,
        	"name": "Edmonds School District/Alderwood Terrace Rotary Club/OSPI/Swerve Drive Specialties/West Coast Products/Holland America Line/Gene Haas Foundation/Philips Ultrasound/Lynnwood Rotary/Puget Sound Plumbing & Heating/Cedar Plaza Ace Hardware/TAP Plastics/Surety Security/Boeing/SPEEA/Skapa Landscaping, LLC&Mountlake Terrace High School",
        	"nickname": "Chill Out",
        	"postal_code": "98043",
        	"rookie_year": 2006,
        	"school_name": "Mountlake Terrace High School",
        	"state_prov": "Washington",
        	"team_number": 1778,
        	"website": "http://www.chillout1778.org/"
        }
        """)!;
    private static readonly Event _utEvent = JsonSerializer.Deserialize<Event>("""
        {
            "key": "2025test",
            "name": "Test Event",
            "year": 2025,
            "week": 1,
            "event_type_string": "Regional",
            "location_name": "Test Location",
            "city": "Test City",
            "state_prov": "Test State",
            "country": "Test Country",
            "gmaps_url": "http://maps.google.com",
            "schedule_url": "http://schedule.com",
            "website": "http://event.com",
            "start_date": "2025-01-01",
            "end_date": "2025-01-03",
            "webcasts": [
                {
                    "channel": "channel",
                    "type": "youtube"
                }
            ],
            "address": "123 Test St",
            "district": null,
            "division_keys": [],
            "event_code": "TEST",
            "event_type": 0,
            "first_event_code": "test",
            "first_event_id": null,
            "gmaps_place_id": "place_id",
            "lat": 0.0,
            "lng": 0.0,
            "parent_event_key": null,
            "playoff_type": 0,
            "playoff_type_string": "Test Playoff",
            "postal_code": "12345",
            "short_name": "Test Event",
            "timezone": "America/New_York"
        }
        """)!;
    private static readonly Event _utEvent2 = JsonSerializer.Deserialize<Event>("""
        {
        	"address": "Showalter Hall, 526 5th St, Cheney, WA 99004, USA",
        	"city": "Cheney",
        	"country": "USA",
        	"district": {
        		"abbreviation": "pnw",
        		"display_name": "Pacific Northwest",
        		"key": "2025pnw",
        		"year": 2025
        	},
        	"division_keys": [],
        	"end_date": "2025-04-05",
        	"event_code": "pncmp",
        	"event_type": 2,
        	"event_type_string": "District Championship",
        	"first_event_code": "pncmp",
        	"first_event_id": null,
        	"gmaps_place_id": "ChIJkShwOwA5nlQRiP-x07Pwbfs",
        	"gmaps_url": "https://maps.google.com/?cid=18117401531122843528",
        	"key": "2025pncmp",
        	"lat": 47.4901128,
        	"lng": -117.5797878,
        	"location_name": "Showalter Hall",
        	"name": "Pacific Northwest FIRST District Championship",
        	"parent_event_key": null,
        	"playoff_type": 10,
        	"playoff_type_string": "Double Elimination Bracket (8 Alliances)",
        	"postal_code": "99004",
        	"short_name": "Pacific Northwest",
        	"start_date": "2025-04-02",
        	"state_prov": "WA",
        	"timezone": "America/Los_Angeles",
        	"webcasts": [
        		{
        			"channel": "firstinspires14",
        			"type": "twitch"
        		}
        	],
        	"website": "http://www.firstwa.org",
        	"week": 5,
        	"year": 2025
        }
        """)!;

    public SubscriptionCommandModuleTests(ITestOutputHelper outputHelper) : base(outputHelper)
    {
        _teamSubscriptionTableMock = new Mock<TableClient>();
        _eventSubscriptionTableMock = new Mock<TableClient>();

        this.Mocker.Use(new SubscriptionManager(_teamSubscriptionTableMock.Object, _eventSubscriptionTableMock.Object, this.Mocker.Get<ILoggerFactory>().CreateLogger<SubscriptionManager>()));

        this.Mocker.GetMock<IEventApi>()
            .Setup(e => e.GetEvent(_utEvent.Key, It.IsAny<string>()))
            .Returns(_utEvent);
        this.Mocker.GetMock<IEventApi>()
            .Setup(e => e.GetEvent(_utEvent2.Key, It.IsAny<string>()))
            .Returns(_utEvent2);
        this.Mocker.GetMock<IEventApi>()
            .Setup(e => e.GetEvent(It.IsNotIn(_utEvent.Key, _utEvent2.Key), It.IsAny<string>()))
            .Returns(default(Event?));

        this.Mocker.GetMock<ITeamApi>()
            .Setup(t => t.GetTeam(_utTeam.Key, It.IsAny<string>()))
            .Returns(_utTeam);
        this.Mocker.GetMock<ITeamApi>()
            .Setup(t => t.GetTeam(_utTeam2.Key, It.IsAny<string>()))
            .Returns(_utTeam2);
        this.Mocker.GetMock<ITeamApi>()
            .Setup(t => t.GetTeam(It.IsNotIn(_utTeam.Key, _utTeam2.Key), It.IsAny<string>()))
            .Returns(default(Team?));

        this.Module = this.Mocker.CreateInstance<SubscriptionCommandModule>();
    }

    [Fact]
    public async Task ShowAsync_ShouldShowSubscriptions()
    {
        // Arrange
        var guildId = 12345UL;
        var channelId = 67890UL;
        this.MockContext.SetupGet(c => c.Interaction.GuildId).Returns(guildId);
        this.MockContext.SetupGet(c => c.Interaction.ChannelId).Returns(channelId);

        _teamSubscriptionTableMock.Setup(t => t.QueryAsync<TeamSubscriptionEntity>(default(string?), default, null, It.IsAny<CancellationToken>()))
            .Returns(AsyncPageable<TeamSubscriptionEntity>
                .FromPages([Page<TeamSubscriptionEntity>
                    .FromValues([
                        new TeamSubscriptionEntity {
                            PartitionKey = _utTeam.Key,
                            RowKey = _utEvent.Key,
                            Subscribers = new() {
                            {
                                guildId.ToString(), [channelId]
                            }
                        }
                    }
                ], It.IsAny<string>(), Mock.Of<Response>())
            ]));

        _eventSubscriptionTableMock.Setup(e => e.QueryAsync<EventSubscriptionEntity>(default(string?), default, null, It.IsAny<CancellationToken>()))
            .Returns(AsyncPageable<EventSubscriptionEntity>
                .FromPages([Page<EventSubscriptionEntity>
                    .FromValues([
                        new EventSubscriptionEntity {
                            PartitionKey = _utEvent.Key,
                            RowKey = _utTeam.Key,
                            Subscribers = new() {
                            {
                                guildId.ToString(), [channelId]
                            }
                        }
                    }
                ], It.IsAny<string>(), Mock.Of<Response>())
            ]));

        MessageProperties p = new();
        this.MockInteraction.Setup(i => i.ModifyOriginalResponseAsync(It.IsAny<Action<MessageProperties>>(), It.IsAny<RequestOptions>()))
            .Callback<Action<MessageProperties>, RequestOptions>((a, _) => a(p));

        // Act
        await this.Module.ShowAsync();

        // Assert
        this.MockInteraction.Verify(i => i.ModifyOriginalResponseAsync(It.IsAny<Action<MessageProperties>>(), It.IsAny<RequestOptions>()), Times.Once);
        Assert.Contains()
    }

    [Fact]
    public async Task ShowAsync_WithNoSubscriptions_ShouldRespondWithMessage()
    {
        // Arrange
        var guildId = 12345UL;
        var channelId = 67890UL;
        this.MockContext.SetupGet(c => c.Interaction.GuildId).Returns(guildId);
        this.MockContext.SetupGet(c => c.Interaction.ChannelId).Returns(channelId);

        _teamSubscriptionTableMock.Setup(t => t.QueryAsync<TeamSubscriptionEntity>(default(string?), default, null, It.IsAny<CancellationToken>()))
            .Returns(AsyncPageable<TeamSubscriptionEntity>
                .FromPages([Page<TeamSubscriptionEntity>
                    .FromValues([], It.IsAny<string>(), Mock.Of<Response>())
            ]));

        _eventSubscriptionTableMock.Setup(e => e.QueryAsync<EventSubscriptionEntity>(default(string?), default, null, It.IsAny<CancellationToken>()))
            .Returns(AsyncPageable<EventSubscriptionEntity>
                .FromPages([Page<EventSubscriptionEntity>
                    .FromValues([], It.IsAny<string>(), Mock.Of<Response>())
            ]));

        MessageProperties p = new();
        this.MockInteraction.Setup(i => i.ModifyOriginalResponseAsync(It.IsAny<Action<MessageProperties>>(), It.IsAny<RequestOptions>()))
            .Callback<Action<MessageProperties>, RequestOptions>((a, _) => a(p));

        // Act
        await this.Module.ShowAsync();

        // Assert
        this.MockInteraction.Verify(i => i.ModifyOriginalResponseAsync(It.IsAny<Action<MessageProperties>>(), It.IsAny<RequestOptions>()), Times.Once);
        Assert.True(p.Content.IsSpecified);
        Assert.Contains("No subscriptions found for this channel.", p.Content.Value);
    }

    [Fact]
    public async Task ShowAsync_ShouldHandleInterationAcknowledged()
    {
        // Arrange
        this.MockInteraction
            .Setup(i => i.DeferAsync(It.IsAny<bool>(), It.IsAny<RequestOptions>()))
            .Throws(new HttpException(System.Net.HttpStatusCode.InternalServerError, Mock.Of<IRequest>(), DiscordErrorCode.InteractionHasAlreadyBeenAcknowledged));

        // Act
        await this.Module.ShowAsync();

        // Assert
        this.MockInteraction.Verify(i => i.ModifyOriginalResponseAsync(It.IsAny<Action<MessageProperties>>(), It.IsAny<RequestOptions>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateSubscription()
    {
        using var i = DebugHelper.IgnoreDebugAssertExceptions();

        // Arrange
        var guildId = 12345UL;
        var channelId = 67890UL;
        this.MockContext.SetupGet(c => c.Interaction.GuildId).Returns(guildId);
        this.MockContext.SetupGet(c => c.Interaction.ChannelId).Returns(channelId);

        _teamSubscriptionTableMock.Setup(t => t.GetEntityIfExistsAsync<TeamSubscriptionEntity>(It.IsAny<string>(), It.IsAny<string>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Response.FromValue(new TeamSubscriptionEntity
            {
                PartitionKey = _utTeam.Key,
                RowKey = _utEvent.Key,
                Subscribers = new() {
                            {
                                guildId.ToString(), [channelId]
                            }
                        }
            }, Mock.Of<Response>()));

        _eventSubscriptionTableMock.Setup(e => e.GetEntityIfExistsAsync<EventSubscriptionEntity>(It.IsAny<string>(), It.IsAny<string>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Response.FromValue(new EventSubscriptionEntity
            {
                PartitionKey = _utEvent.Key,
                RowKey = _utTeam.Key,
                Subscribers = new() {
                            {
                                guildId.ToString(), [channelId]
                            }
                        }
            }, Mock.Of<Response>()));

        MessageProperties messageToUser = new();
        this.MockInteraction.Setup(i => i.ModifyOriginalResponseAsync(It.IsAny<Action<MessageProperties>>(), It.IsAny<RequestOptions>()))
            .Callback<Action<MessageProperties>, RequestOptions>((a, _) => a(messageToUser));

        // Act
        await this.Module.CreateAsync(_utTeam.Key, _utEvent.Key);

        // Assert
        this.MockInteraction.Verify(i => i.ModifyOriginalResponseAsync(It.IsAny<Action<MessageProperties>>(), It.IsAny<RequestOptions>()), Times.Once);
        Assert.True(messageToUser.Content.IsSpecified);
        Assert.Contains("This channel is now subscribed to", messageToUser.Content.Value);
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteSubscription()
    {
        // Arrange
        var guildId = 12345UL;
        var channelId = 67890UL;
        this.MockContext.SetupGet(c => c.Interaction.GuildId).Returns(guildId);
        this.MockContext.SetupGet(c => c.Interaction.ChannelId).Returns(channelId);

        _teamSubscriptionTableMock.Setup(t => t.QueryAsync<TeamSubscriptionEntity>(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
            .Returns(AsyncPageable<TeamSubscriptionEntity>
                .FromPages([Page<TeamSubscriptionEntity>
                    .FromValues([
                        new TeamSubscriptionEntity {
                            PartitionKey = _utTeam.Key,
                            RowKey = _utEvent.Key,
                            Subscribers = new() {
                            {
                                guildId.ToString(), [channelId]
                            }
                        }
                    },
                        new TeamSubscriptionEntity {
                            PartitionKey = _utTeam2.Key,
                            RowKey = _utEvent2.Key,
                            Subscribers = new() {
                            {
                                guildId.ToString(), [channelId]
                            }
                        }
                    }
                ], It.IsAny<string>(), Mock.Of<Response>())
            ]));

        _eventSubscriptionTableMock.Setup(t => t.QueryAsync<EventSubscriptionEntity>(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
            .Returns(AsyncPageable<EventSubscriptionEntity>
                .FromPages([Page<EventSubscriptionEntity>
                    .FromValues([
                        new EventSubscriptionEntity {
                            RowKey  = _utTeam.Key,
                            PartitionKey= _utEvent.Key,
                            Subscribers = new() {
                            {
                                guildId.ToString(), [channelId]
                            }
                        }
                    },
                        new EventSubscriptionEntity {
                            RowKey  = _utTeam.Key,
                            PartitionKey= _utEvent.Key,
                            Subscribers = new() {
                            {
                                guildId.ToString(), [channelId]
                            }
                        }
                    }
                ], It.IsAny<string>(), Mock.Of<Response>())
            ]));

        MessageProperties messageToUser = new();
        this.MockInteraction.Setup(i => i.ModifyOriginalResponseAsync(It.IsAny<Action<MessageProperties>>(), It.IsAny<RequestOptions>()))
            .Callback<Action<MessageProperties>, RequestOptions>((a, _) => a(messageToUser));

        // Act
        await this.Module.DeleteAsync();

        // Assert
        this.MockInteraction.Verify(i => i.ModifyOriginalResponseAsync(It.IsAny<Action<MessageProperties>>(), It.IsAny<RequestOptions>()), Times.Once);
        Assert.True(messageToUser.Components.IsSpecified);
        Assert.Equal(2, messageToUser.Components.Value.Components.Count);

        var firstRow = messageToUser.Components.Value.Components.FirstOrDefault();
        Assert.NotNull(firstRow);
        Assert.Single(firstRow.Components);
        var menu = firstRow.Components.FirstOrDefault() as SelectMenuComponent;
        Assert.NotNull(menu);
        Assert.Equal(2, menu.Options.Count);

        var secondRow = messageToUser.Components.Value.Components.Skip(1).FirstOrDefault();
        Assert.NotNull(secondRow);
        Assert.Single(secondRow.Components);
        var button = secondRow.Components.FirstOrDefault() as ButtonComponent;
        Assert.NotNull(button);
        Assert.Equal(ButtonStyle.Secondary, button.Style);
    }

    [Fact]
    public async Task HandleInteractionAsync_ShouldHandleInteraction()
    {
        // Arrange
        var guildId = 12345UL;
        var channelId = 67890UL;

        var data = this.Mocker.GetMock<IComponentInteractionData>();
        data.SetupGet(d => d.CustomId).Returns("subscription-delete-selection");
        data.SetupGet(d => d.Values).Returns([$"{_utEvent.Key}|{_utTeam.Key}|{guildId}|{channelId}"]);

        this.Mocker.CreateSelfMock<IComponentInteraction>();
        var component = this.Mocker.GetMock<IComponentInteraction>();
        component.SetupGet(c => c.Data).Returns(data.Object);

        _teamSubscriptionTableMock.Setup(t => t.GetEntityIfExistsAsync<TeamSubscriptionEntity>(It.IsAny<string>(), It.IsAny<string>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Response.FromValue(new TeamSubscriptionEntity
            {
                PartitionKey = _utTeam.Key,
                RowKey = _utEvent.Key,
                Subscribers = new() { { guildId.ToString(), [channelId] } }
            }, Mock.Of<Response>()));

        TeamSubscriptionEntity? upsertedEntity = null;
        _teamSubscriptionTableMock.Setup(t => t.UpsertEntityAsync(It.IsAny<TeamSubscriptionEntity>(), It.IsAny<TableUpdateMode>(), It.IsAny<CancellationToken>()))
            .Callback((TeamSubscriptionEntity e, TableUpdateMode _, CancellationToken _) => upsertedEntity = e)
            .ReturnsAsync(Mock.Of<Response>());

        _eventSubscriptionTableMock.Setup(e => e.GetEntityIfExistsAsync<EventSubscriptionEntity>(It.IsAny<string>(), It.IsAny<string>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Response.FromValue(new EventSubscriptionEntity
            {
                PartitionKey = _utEvent.Key,
                RowKey = _utTeam.Key,
                Subscribers = new() { { guildId.ToString(), [channelId] } }
            }, Mock.Of<Response>()));

        _teamSubscriptionTableMock.Setup(t => t.QueryAsync<TeamSubscriptionEntity>(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
            .Returns(AsyncPageable<TeamSubscriptionEntity>
                .FromPages([Page<TeamSubscriptionEntity>
                    .FromValues([
                        new TeamSubscriptionEntity {
                            PartitionKey = _utTeam.Key,
                            RowKey = _utEvent.Key,
                            Subscribers = new() {
                            {
                                guildId.ToString(), [channelId]
                            }
                        }
                    },
                        new TeamSubscriptionEntity {
                            PartitionKey = _utTeam2.Key,
                            RowKey = _utEvent2.Key,
                            Subscribers = new() {
                            {
                                guildId.ToString(), [channelId]
                            }
                        }
                    }
                ], It.IsAny<string>(), Mock.Of<Response>())
            ]));

        _eventSubscriptionTableMock.Setup(t => t.QueryAsync<EventSubscriptionEntity>(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
            .Returns(AsyncPageable<EventSubscriptionEntity>
                .FromPages([Page<EventSubscriptionEntity>
                    .FromValues([
                        new EventSubscriptionEntity {
                            RowKey  = _utTeam.Key,
                            PartitionKey= _utEvent.Key,
                            Subscribers = new() {
                            {
                                guildId.ToString(), [channelId]
                            }
                        }
                    },
                        new EventSubscriptionEntity {
                            RowKey  = _utTeam.Key,
                            PartitionKey= _utEvent.Key,
                            Subscribers = new() {
                            {
                                guildId.ToString(), [channelId]
                            }
                        }
                    }
                ], It.IsAny<string>(), Mock.Of<Response>())
            ]));

        var messageMock = new Mock<IUserMessage>();
        messageMock.SetupGet(m => m.Components).Returns([new ActionRowBuilder().WithSelectMenu(new SelectMenuBuilder("subscription-delete-selection", [new SelectMenuOptionBuilder("test", $"{_utEvent.Key}|{_utTeam.Key}|{guildId}|{channelId}"), new SelectMenuOptionBuilder("leftover", "should|be|left|over")])).Build()]);
        component.SetupGet(c => c.Message).Returns(messageMock.Object);

        MessageProperties updatedMessageAfterSelectionHandled = new();
        component.Setup(c => c.UpdateAsync(It.IsAny<Action<MessageProperties>>(), It.IsAny<RequestOptions>()))
            .Callback((Action<MessageProperties> a, RequestOptions _) => a(updatedMessageAfterSelectionHandled));

        // Act
        var result = await this.Module.HandleInteractionAsync(this.Mocker, component.Object, CancellationToken.None);

        // Assert
        Assert.True(result);
        Assert.NotNull(upsertedEntity);
        Assert.False(upsertedEntity.Subscribers.Exists(guildId, channelId));
        Assert.NotNull(updatedMessageAfterSelectionHandled);
        Assert.True(updatedMessageAfterSelectionHandled.Components.IsSpecified);
        Assert.NotNull(updatedMessageAfterSelectionHandled.Components.Value.Components);
        Assert.Equal("should|be|left|over", updatedMessageAfterSelectionHandled.Components.Value.Components.Single().Components.OfType<SelectMenuComponent>().Single().Options.Single().Value);
    }
}