namespace Common.Tba.Notifications;

#nullable disable
[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Reduce boilerplate by just matching exact JSON body")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "Reduce boilerplate by just matching exact JSON body")]
public class UpcomingMatch
{
    public string event_key { get; set; }
    public string match_key { get; set; }
    public string event_name { get; set; }
    public string[] team_keys { get; set; }
    public int scheduled_time { get; set; }
    public int predicted_time { get; set; }
    public Webcast webcast { get; set; }
}

[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Reduce boilerplate by just matching exact JSON body")]
public class Webcast
{
    public string type { get; set; }
    public string channel { get; set; }
}