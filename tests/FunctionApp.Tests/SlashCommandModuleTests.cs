namespace FunctionApp.Tests;

using Discord;

using FunctionApp.DiscordInterop.CommandModules;
using FunctionApp.DiscordInterop.Embeds;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

using TheBlueAlliance.Model;

[SuppressMessage("Performance", "EA0014:Add CancellationToken as the parameter of asynchronous method", Justification = "xUnit [Fact] methods do not support a CancellationToken parameter in this test setup.")]
public sealed class SlashCommandModuleTests
{
    [Fact]
    public async Task TeamsShowAsyncWithNumericTeamKeyNormalizesInputBeforeGeneratingEmbeds()
    {
        RecordingEmbedCreator<string> detailCreator = new();
        RecordingEmbedCreator<(int? Year, string TeamKey, string? EventKey)> rankCreator = new();
        RecordingEmbedCreator<(string?, ushort)> scheduleCreator = new();
        using ServiceProvider services = CreateTeamsServiceProvider(detailCreator, rankCreator, scheduleCreator);

        TestTeamsCommandModule module = new(services);
        await module.ShowAsync("2046", post: true);

        Assert.Equal("frc2046", Assert.Single(detailCreator.Inputs));
        Assert.Empty(rankCreator.Inputs);
        Assert.Empty(scheduleCreator.Inputs);
    }

    [Fact]
    public async Task TeamsGetRankAsyncWithNumericTeamKeyNormalizesInputBeforeGeneratingEmbeds()
    {
        RecordingEmbedCreator<string> detailCreator = new();
        RecordingEmbedCreator<(int? Year, string TeamKey, string? EventKey)> rankCreator = new();
        RecordingEmbedCreator<(string?, ushort)> scheduleCreator = new();
        using ServiceProvider services = CreateTeamsServiceProvider(detailCreator, rankCreator, scheduleCreator);

        TestTeamsCommandModule module = new(services);
        await module.GetRankAsync("2046", eventKey: "2027cabl", year: 2027, post: true);

        (int? year, string teamKey, string? eventKey) = Assert.Single(rankCreator.Inputs);
        Assert.Equal(2027, year);
        Assert.Equal("frc2046", teamKey);
        Assert.Equal("2027cabl", eventKey);
    }

    [Fact]
    public async Task TeamsGetScheduleAsyncPassesTeamNumberAsHighlightTeam()
    {
        RecordingEmbedCreator<string> detailCreator = new();
        RecordingEmbedCreator<(int? Year, string TeamKey, string? EventKey)> rankCreator = new();
        RecordingEmbedCreator<(string?, ushort)> scheduleCreator = new();
        using ServiceProvider services = CreateTeamsServiceProvider(detailCreator, rankCreator, scheduleCreator);

        TestTeamsCommandModule module = new(services);
        await module.GetScheduleAsync(teamKey: "frc2046", eventKey: "2027cabl", numMatches: 4, post: true);

        (string? eventKey, ushort numMatches) = Assert.Single(scheduleCreator.Inputs);
        Assert.Equal("2027cabl", eventKey);
        Assert.Equal((ushort)4, numMatches);
        Assert.Equal((ushort)2046, Assert.Single(scheduleCreator.HighlightTeams));
    }

    [Fact]
    public async Task TeamsShowAsyncWithEmptyTeamKeyWritesValidationMessage()
    {
        RecordingEmbedCreator<string> detailCreator = new();
        RecordingEmbedCreator<(int? Year, string TeamKey, string? EventKey)> rankCreator = new();
        RecordingEmbedCreator<(string?, ushort)> scheduleCreator = new();
        using ServiceProvider services = CreateTeamsServiceProvider(detailCreator, rankCreator, scheduleCreator);

        TestTeamsCommandModule module = new(services);
        await module.ShowAsync(string.Empty, post: true);

        Assert.Contains("Team key is required.", module.Messages);
        Assert.Empty(detailCreator.Inputs);
    }

    [Fact]
    public async Task EventsGetDetailsAsyncWithEmptyEventKeyWritesValidationMessage()
    {
        RecordingEmbedCreator<string> detailCreator = new();
        RecordingEmbedCreator<(string?, ushort)> scheduleCreator = new();
        using ServiceProvider services = CreateEventsServiceProvider(detailCreator, scheduleCreator);

        TestEventsCommandModule module = new(services);
        await module.GetDetailsAsync(string.Empty, post: true);

        Assert.Contains("Event key is required.", module.Messages);
        Assert.Empty(detailCreator.Inputs);
    }

    [Fact]
    public async Task EventsGetScheduleAsyncPassesEventInputAndHighlightTeam()
    {
        RecordingEmbedCreator<string> detailCreator = new();
        RecordingEmbedCreator<(string?, ushort)> scheduleCreator = new();
        using ServiceProvider services = CreateEventsServiceProvider(detailCreator, scheduleCreator);

        TestEventsCommandModule module = new(services);
        await module.GetScheduleAsync(eventKey: "2027cabl", teamKey: "frc1678", numMatches: 3, post: true);

        (string? eventKey, ushort numMatches) = Assert.Single(scheduleCreator.Inputs);
        Assert.Equal("2027cabl", eventKey);
        Assert.Equal((ushort)3, numMatches);
        Assert.Equal((ushort)1678, Assert.Single(scheduleCreator.HighlightTeams));
    }

    [Fact]
    public async Task MatchesShowNextAsyncWithNumericTeamKeyNormalizesInputBeforeGeneratingEmbeds()
    {
        RecordingEmbedCreator<(string eventKey, string teamKey)> upcomingCreator = new();
        RecordingEmbedCreator<(string matchKey, bool summarize)> scoreCreator = new();
        using ServiceProvider services = CreateMatchesServiceProvider(upcomingCreator, scoreCreator);

        TestMatchesCommandModule module = new(services);
        await module.ShowNextAsync(eventKey: "2027cabl", teamKey: "2046", post: true);

        (string eventKey, string teamKey) = Assert.Single(upcomingCreator.Inputs);
        Assert.Equal("2027cabl", eventKey);
        Assert.Equal("frc2046", teamKey);
        Assert.Equal((ushort)2046, Assert.Single(upcomingCreator.HighlightTeams));
    }

    [Fact]
    public async Task MatchesGetResultAsyncBuildsExpectedMatchKey()
    {
        RecordingEmbedCreator<(string eventKey, string teamKey)> upcomingCreator = new();
        RecordingEmbedCreator<(string matchKey, bool summarize)> scoreCreator = new();
        using ServiceProvider services = CreateMatchesServiceProvider(upcomingCreator, scoreCreator);

        TestMatchesCommandModule module = new(services);
        await module.GetResultAsync(eventKey: "2027cabl", compLevel: (int)CompLevel.Qm, matchNumber: 8, summarize: true, post: true);

        (string matchKey, bool summarize) = Assert.Single(scoreCreator.Inputs);
        Assert.Equal("2027cabl_qm8", matchKey);
        Assert.True(summarize);
        Assert.Empty(upcomingCreator.Inputs);
    }

    [Fact]
    public async Task MatchesShowNextAsyncWhenEmbeddingCreationThrowsWritesFriendlyErrorMessage()
    {
        RecordingEmbedCreator<(string eventKey, string teamKey)> upcomingCreator = new(throwOnCreate: true);
        RecordingEmbedCreator<(string matchKey, bool summarize)> scoreCreator = new();
        using ServiceProvider services = CreateMatchesServiceProvider(upcomingCreator, scoreCreator);

        TestMatchesCommandModule module = new(services);
        await module.ShowNextAsync(eventKey: "2027cabl", teamKey: "frc2046", post: true);

        Assert.Contains("Sorry, I encountered an error processing your request. Maybe try again? Or contact your admin with this news so they can troubleshoot.", module.Messages);
    }

    [Fact]
    public async Task PingAsyncWhenDeferSucceedsRespondsWithPong()
    {
        TestPingCommandModule module = new();

        await module.PingAsync();

        (string content, bool ephemeral) = Assert.Single(module.Responses);
        Assert.Equal("Pong!", content);
        Assert.True(ephemeral);
    }

    [Fact]
    public async Task PingAsyncWhenDeferReturnsNullDoesNotRespond()
    {
        TestPingCommandModule module = new()
        {
            DeferReturnsNull = true
        };

        await module.PingAsync();

        Assert.Empty(module.Responses);
    }

    private static ServiceProvider CreateTeamsServiceProvider(
        RecordingEmbedCreator<string> detailCreator,
        RecordingEmbedCreator<(int? Year, string TeamKey, string? EventKey)> rankCreator,
        RecordingEmbedCreator<(string?, ushort)> scheduleCreator)
    {
        return new ServiceCollection()
            .AddLogging()
            .AddKeyedSingleton<IEmbedCreator<string>>(nameof(TeamDetail), detailCreator)
            .AddKeyedSingleton<IEmbedCreator<(int? Year, string TeamKey, string? EventKey)>>(nameof(TeamRank), rankCreator)
            .AddKeyedSingleton<IEmbedCreator<(string?, ushort)>>(nameof(Schedule), scheduleCreator)
            .BuildServiceProvider();
    }

    private static ServiceProvider CreateEventsServiceProvider(
        RecordingEmbedCreator<string> detailCreator,
        RecordingEmbedCreator<(string?, ushort)> scheduleCreator)
    {
        return new ServiceCollection()
            .AddLogging()
            .AddKeyedSingleton<IEmbedCreator<string>>(nameof(EventDetail), detailCreator)
            .AddKeyedSingleton<IEmbedCreator<(string?, ushort)>>(nameof(Schedule), scheduleCreator)
            .BuildServiceProvider();
    }

    private static ServiceProvider CreateMatchesServiceProvider(
        RecordingEmbedCreator<(string eventKey, string teamKey)> upcomingCreator,
        RecordingEmbedCreator<(string matchKey, bool summarize)> scoreCreator)
    {
        return new ServiceCollection()
            .AddLogging()
            .AddKeyedSingleton<IEmbedCreator<(string eventKey, string teamKey)>>(nameof(UpcomingMatch), upcomingCreator)
            .AddKeyedSingleton<IEmbedCreator<(string matchKey, bool summarize)>>(nameof(MatchScore), scoreCreator)
            .BuildServiceProvider();
    }

    private sealed class RecordingEmbedCreator<TInput>(bool throwOnCreate = false) : IEmbedCreator<TInput>
    {
        public List<TInput> Inputs { get; } = [];

        public List<ushort?> HighlightTeams { get; } = [];

        public async IAsyncEnumerable<ResponseEmbedding?> CreateAsync(
            TInput input,
            ushort? highlightTeam = null,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Inputs.Add(input);
            HighlightTeams.Add(highlightTeam);
            yield return throwOnCreate
                ? throw new InvalidOperationException("intentional test exception")
                : new(new EmbedBuilder().WithTitle("Test Embed").Build());
        }
    }

    private sealed class TestTeamsCommandModule(IServiceProvider services) : TeamsCommandModule(services)
    {
        public List<string> Messages { get; } = [];

        protected override Task<IDisposable?> TryDeferAsync(bool ephemeral = false, CancellationToken cancellationToken = default)
            => Task.FromResult(Disposable.WithNoopDispose);

        protected override Task UpdateOriginalResponseAsync(Action<MessageProperties> updateMessage, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            MessageProperties properties = new();
            updateMessage(properties);
            if (properties.Content.IsSpecified && properties.Content.Value is not null)
            {
                Messages.Add(properties.Content.Value);
            }

            return Task.CompletedTask;
        }
    }

    private sealed class TestEventsCommandModule(IServiceProvider services) : EventsCommandModule(services)
    {
        public List<string> Messages { get; } = [];

        protected override Task<IDisposable?> TryDeferAsync(bool ephemeral = false, CancellationToken cancellationToken = default)
            => Task.FromResult(Disposable.WithNoopDispose);

        protected override Task UpdateOriginalResponseAsync(Action<MessageProperties> updateMessage, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            MessageProperties properties = new();
            updateMessage(properties);
            if (properties.Content.IsSpecified && properties.Content.Value is not null)
            {
                Messages.Add(properties.Content.Value);
            }

            return Task.CompletedTask;
        }
    }

    private sealed class TestMatchesCommandModule(IServiceProvider services) : MatchesCommandModule(services)
    {
        public List<string> Messages { get; } = [];

        protected override Task<IDisposable?> TryDeferAsync(bool ephemeral = false, CancellationToken cancellationToken = default)
            => Task.FromResult(Disposable.WithNoopDispose);

        protected override Task UpdateOriginalResponseAsync(Action<MessageProperties> updateMessage, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            MessageProperties properties = new();
            updateMessage(properties);
            if (properties.Content.IsSpecified && properties.Content.Value is not null)
            {
                Messages.Add(properties.Content.Value);
            }

            return Task.CompletedTask;
        }

        protected override void HandleShowNextError(Exception exception, string teamKey, string eventKey)
        {
            _ = exception;
            _ = teamKey;
            _ = eventKey;
        }
    }

    private sealed class TestPingCommandModule : PingCommandModule
    {
        public TestPingCommandModule()
            : base(NullLogger<PingCommandModule>.Instance)
        {
        }

        public bool DeferReturnsNull { get; set; }

        public List<(string Content, bool Ephemeral)> Responses { get; } = [];

        protected override Task<IDisposable?> TryDeferAsync(bool ephemeral = false, CancellationToken cancellationToken = default)
            => Task.FromResult(DeferReturnsNull ? null : Disposable.WithNoopDispose);

        protected override Task SendResponseAsync(string responseContent, bool ephemeral = false, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Responses.Add((responseContent, ephemeral));
            return Task.CompletedTask;
        }
    }

    private sealed class Disposable : IDisposable
    {
        public static IDisposable? WithNoopDispose { get; } = new Disposable();

        public void Dispose()
        {
        }
    }
}
