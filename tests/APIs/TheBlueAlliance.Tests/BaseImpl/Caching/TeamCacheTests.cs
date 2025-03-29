namespace TheBlueAlliance.Tests.BaseImpl.Caching;
using FunctionApp.Storage.Caching;

using Microsoft.Extensions.Logging;

using Moq;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Threading;
using System.Threading.Tasks;

using TestCommon;

using TheBlueAlliance.Api;
using TheBlueAlliance.Interfaces.Caching;
using TheBlueAlliance.Model;

using Xunit.Abstractions;

public class TeamCacheTests : TestWithLogger
{
    private static readonly Team _utTeam = new Team(
            address: "1234 Robotics Ave",
            city: "Roboville",
            country: "USA",
            gmapsPlaceId: null,
            gmapsUrl: null,
            key: "frc1234",
            lat: 40.7128,
            lng: -74.0060,
            locationName: null,
            name: "Robo Warriors",
            nickname: "Warriors",
            postalCode: "12345",
            rookieYear: 2005,
            schoolName: "Robo High School",
            stateProv: "NY",
            teamNumber: 1234,
            website: "http://www.robowarriors.com"
        );

    public TeamCacheTests(ITestOutputHelper outputHelper) : base(typeof(TeamCache), outputHelper)
    {
        this.Mocker.WithSelfMock<ITeamApi>();
        this.Mocker.Use(new Meter(nameof(TeamCacheTests)));

        this.Mocker.With<ITeamCache, TeamCache>();

        ((ConcurrentDictionary<string, Team>)typeof(TeamCache).GetField("_teams", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic).GetValue(null)).Clear();
    }

    [Fact]
    public async Task InitializeAsync_ShouldLoadTeamsFromApi()
    {
        // Arrange
        this.Mocker.GetMock<ITeamApi>()
            .Setup(api => api.GetTeamsAsync(0, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([_utTeam]);
        this.Mocker.GetMock<ITeamApi>()
            .Setup(api => api.GetTeamsAsync(1, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        var cache = this.Mocker.Get<ITeamCache>();
        await cache.InitializeAsync(CancellationToken.None);

        // Assert
        Assert.Contains(_utTeam.Key, cache.AllTeams.Keys);
        this.Logger.Verify(LogLevel.Information);
    }

    [Fact]
    public async Task Indexer_ShouldReturnTeamFromCacheAsync()
    {
        // Arrange
        this.Mocker.GetMock<ITeamApi>()
            .Setup(api => api.GetTeamsAsync(0, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([_utTeam]);
        this.Mocker.GetMock<ITeamApi>()
            .Setup(api => api.GetTeamsAsync(1, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        var cache = this.Mocker.Get<ITeamCache>();
        await cache.InitializeAsync(CancellationToken.None).ConfigureAwait(true);

        var result = cache[_utTeam.Key];

        // Assert
        Assert.Equal(_utTeam, result);
    }

    [Fact]
    public void Indexer_ShouldFetchTeamFromApiIfNotInCache()
    {
        // Arrange
        this.Mocker.GetMock<ITeamApi>()
            .Setup(api => api.GetTeam(_utTeam.Key, It.IsAny<string>()))
            .Returns(_utTeam);

        // Act
        var cache = this.Mocker.Get<ITeamCache>();
        Assert.Empty(cache.AllTeams);
        var result = cache[_utTeam.Key];

        // Assert
        Assert.Equal(_utTeam, result);
        Assert.Contains(_utTeam.Key, cache.AllTeams.Keys);
    }

    [Fact]
    public void Indexer_ShouldThrowKeyNotFoundExceptionIfTeamNotFound()
    {
        // Arrange
        var teamKey = "nonexistent";
        this.Mocker.GetMock<ITeamApi>()
            .Setup(api => api.GetTeam(teamKey, It.IsAny<string>()))
            .Returns((Team)null);

        // Act & Assert
        Assert.Throws<KeyNotFoundException>(() => this.Mocker.Get<ITeamCache>()[teamKey]);
    }

    [Fact]
    public async Task InitializeAsync_ShouldHandleApiExceptions()
    {
        // Arrange
        this.Mocker.GetMock<ITeamApi>()
            .Setup(api => api.GetTeamsAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("API error"));

        // Act & Assert
        await AssertDebugExceptionAsync(this.Mocker.Get<ITeamCache>().InitializeAsync(CancellationToken.None).AsTask());
        this.Logger.Verify(LogLevel.Error);
    }
}
