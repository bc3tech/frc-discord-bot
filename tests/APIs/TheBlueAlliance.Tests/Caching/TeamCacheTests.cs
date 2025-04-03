namespace TheBlueAlliance.Tests.Caching;
using Microsoft.Extensions.Logging;

using Moq;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using TestCommon;

using TheBlueAlliance.Api;
using TheBlueAlliance.Caching;
using TheBlueAlliance.Model;

using Xunit.Abstractions;

public class TeamCacheTests : TestWithLogger
{
    private static readonly Team _utTeam = new(
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

        this.Mocker.With<TeamCache>();

        ((ConcurrentDictionary<string, Team>)typeof(TeamCache).GetField("_teams", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null)!).Clear();
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
        var cache = this.Mocker.Get<TeamCache>();
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
        var cache = this.Mocker.Get<TeamCache>();
        await cache.InitializeAsync(CancellationToken.None).ConfigureAwait(true);

        var result = cache[_utTeam.Key];

        // Assert
        Assert.Equal(_utTeam, result);
        this.Mocker.GetMock<ITeamApi>()
            .Verify(api => api.GetTeam(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public void Indexer_ShouldFetchTeamFromApiIfNotInCache()
    {
        // Arrange
        this.Mocker.GetMock<ITeamApi>()
            .Setup(api => api.GetTeam(_utTeam.Key, It.IsAny<string>()))
            .Returns(_utTeam);

        // Act
        var cache = this.Mocker.Get<TeamCache>();
        Assert.Empty(cache.AllTeams);
        var result = cache[_utTeam.Key];

        // Assert
        Assert.Equal(_utTeam, result);
        Assert.Contains(_utTeam.Key, cache.AllTeams.Keys);
        this.Mocker.GetMock<ITeamApi>()
         .Verify(api => api.GetTeam(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public void Indexer_ShouldThrowIfTeamNotFound()
    {
        // Arrange
        var teamKey = "nonexistent";
        this.Mocker.GetMock<ITeamApi>()
            .Setup(api => api.GetTeam(teamKey, It.IsAny<string>()))
            .Returns((Team?)null);

        // Act & Assert
        var ex = Assert.Throws<TeamNotFoundException>(() => this.Mocker.Get<TeamCache>()[teamKey]);
        Assert.Equal($"No team with key {teamKey} could be found", ex.Message);
        Assert.Equal(teamKey, ex.Data["TeamKey"]);
    }

    [Fact]
    public void InitializeAsync_ShouldHandleApiExceptions()
    {
        // Arrange
        this.Mocker.GetMock<ITeamApi>()
            .Setup(api => api.GetTeamsAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Throws<HttpRequestException>();

        // Act & Assert
        DebugHelper.AssertDebugException(this.Mocker.Get<TeamCache>().InitializeAsync(CancellationToken.None).AsTask());
        this.Logger.Verify(LogLevel.Error);
    }
}
