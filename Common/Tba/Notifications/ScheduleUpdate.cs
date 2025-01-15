namespace Common.Tba.Notifications;

#nullable disable
[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Reduce boilerplate by just matching exact JSON body")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "Reduce boilerplate by just matching exact JSON body")]
public record ScheduleUpdate
{
    public string event_key { get; set; }
    public string event_name { get; set; }
    public int first_match_time { get; set; }
}
