namespace TheBlueAlliance.Api;
public static class Translator
{
    public static string CompLevelToShortString(string compLevel)
    {
        return compLevel.ToLowerInvariant() switch
        {
            "qm" => "Quals",
            "qf" => "Quarters",
            "sf" => "Semis",
            "f" => "Finals",
            _ => compLevel
        };
    }

    public static string CompLevelToLongString(string compLevel)
    {
        return compLevel.ToLowerInvariant() switch
        {
            "qm" => "Qualification",
            "qf" => "Quarterfinals",
            "sf" => "Semifinals",
            "f" => "Finals",
            _ => compLevel
        };
    }
}
