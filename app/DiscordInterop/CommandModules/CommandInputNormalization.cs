namespace FunctionApp.DiscordInterop.CommandModules;

using TheBlueAlliance.Model;

internal static class CommandInputNormalization
{
    public static string NormalizeTeamKey(string teamKey)
        => int.TryParse(teamKey, out var teamNumber) ? $"frc{teamNumber}" : teamKey;

    public static string BuildMatchKey(string eventKey, int compLevel, uint matchNumber)
    {
        string normalizedMatchKey = (CompLevel)compLevel switch
        {
            CompLevel.Qm => $"qm{matchNumber}",
            CompLevel.Sf => $"sf{matchNumber}m1",
            CompLevel.F => $"f1m{matchNumber}",
            _ => throw new ArgumentOutOfRangeException(nameof(compLevel), compLevel, null)
        };

        return $"{eventKey}_{normalizedMatchKey}";
    }
}
