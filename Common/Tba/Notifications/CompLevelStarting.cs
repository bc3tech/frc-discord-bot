namespace Common.Tba.Notifications;

#nullable disable
[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Reduce boilerplate by just matching exact JSON body")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "Reduce boilerplate by just matching exact JSON body")]
public record CompLevelStarting
{
    public string event_name { get; set; }
    public string comp_level { get; set; }
    public string event_key { get; set; }
    public int scheduled_time { get; set; }
}
