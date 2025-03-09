namespace DiscordBotFunctionApp.TbaInterop;
internal static class Translator
{
    public static string CompLevelToShortString(string compLevel)
    {
        return compLevel.ToLowerInvariant() switch
        {
            "qm" => "Quals",
            "qf" => "Quarters",
            "sf" => "Elims",
            "f" => "Finals",
            _ => compLevel
        };
    }

    public static string CompLevelToLongString(string compLevel)
    {
        return compLevel.ToLowerInvariant() switch
        {
            "qm" => "Qualifications",
            "qf" => "Quarterfinals",
            "sf" => "Playoffs",
            "f" => "Finals",
            _ => compLevel
        };
    }
}
