namespace Common.Tba.Notifications;

using Microsoft.Extensions.EnumStrings;

using System.Text.Json.Serialization;

[EnumStrings(ExtensionClassModifiers = "public static")]
[JsonConverter(typeof(JsonStringEnumConverter<NotificationType>))]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "Used to match string values coming from API automatically via EnumString generator")]
public enum NotificationType
{
    upcoming_match = 0,
    match_score = 1,
    starting_comp_level = 2,
    alliance_selection = 3,
    awards_posted = 4,
    media_posted = 5,
    district_points_updated = 6,
    schedule_updated = 7,
    final_results = 8,
    ping = 9,
    broadcast = 10,
    match_video = 11,
    event_match_video = 12,
    update_favorites = 100,
    update_subscriptions = 101,
    verification = 200
}
