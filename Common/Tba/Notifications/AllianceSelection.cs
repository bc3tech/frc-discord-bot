namespace Common.Tba.Notifications;
#nullable disable
[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Reduce boilerplate by just matching exact JSON body")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "Reduce boilerplate by just matching exact JSON body")]
public record AllianceSelection(string event_name, string event_key, AllianceSelectionEvent _event);

[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Reduce boilerplate by just matching exact JSON body")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "Reduce boilerplate by just matching exact JSON body")]
public record AllianceSelectionEvent(string key, string website, bool official, string end_date, string name, string short_name, object facebook_eid, string event_district_string, string venue_address, int event_district, string location, string event_code, int year, object[] webcast, AllianceFormation[] alliances, string event_type_string, string start_date, int event_type);

[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Reduce boilerplate by just matching exact JSON body")]
public record AllianceFormation(object[] declines, string[] picks);
