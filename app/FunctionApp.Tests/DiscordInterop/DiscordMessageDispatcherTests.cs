namespace FunctionApp.Tests.DiscordInterop;

using FunctionApp.DiscordInterop;

using System.Text.Json;

public sealed class DiscordMessageDispatcherTests
{
    [Fact]
    public void ExtractTeamsAndEvents_CollectsDistinctValuesAcrossSupportedKeys()
    {
        using var json = JsonDocument.Parse(
            """
            {
              "team_key": "frc33",
              "team_keys": ["frc33", "frc254"],
              "team_number": "971",
              "event_key": "2026miket",
              "event_keys": ["2026miket", "2026oncmp"]
            }
            """);

        var (teams, events) = DiscordMessageDispatcher.ExtractTeamsAndEvents(json.RootElement);

        Assert.Equal(3, teams.Count);
        Assert.Contains("frc33", teams);
        Assert.Contains("frc254", teams);
        Assert.Contains("971", teams);

        Assert.Equal(2, events.Count);
        Assert.Contains("2026miket", events);
        Assert.Contains("2026oncmp", events);
    }
}
