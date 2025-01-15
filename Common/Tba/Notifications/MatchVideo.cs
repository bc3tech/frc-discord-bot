namespace Common.Tba.Notifications;

#nullable disable
[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Reduce boilerplate by just matching exact JSON body")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "Reduce boilerplate by just matching exact JSON body")]
public record MatchVideo
{
    public string event_name { get; set; }

    public MatchDetails match { get; set; }

    public class MatchDetails
    {
        public string comp_level { get; set; }
        public int match_number { get; set; }
        public Video[] videos { get; set; }
        public string time_string { get; set; }
        public int set_number { get; set; }
        public string key { get; set; }
        public int time { get; set; }
        public object score_breakdown { get; set; }
        public string event_key { get; set; }
    }

    public class Video
    {
        public string key { get; set; }
        public string type { get; set; }
    }
}
