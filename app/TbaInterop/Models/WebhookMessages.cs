namespace FunctionApp.TbaInterop.Models;

using FunctionApp.TbaInterop.Models.Notifications;

using Microsoft.Extensions.DependencyInjection;

using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

using TheBlueAlliance.Api;
using TheBlueAlliance.Extensions;
using TheBlueAlliance.Model;

internal sealed record WebhookMessage
{
    [JsonPropertyName("message_type")]
    public NotificationType MessageType { get; init; }

    [JsonPropertyName("message_data")]
    required public JsonElement MessageData { get; init; }

    public T? GetDataAs<T>() => MessageData.Deserialize<T>();

    public bool IsBroadcast => MessageType is NotificationType.schedule_updated or NotificationType.starting_comp_level or NotificationType.alliance_selection;

    public (string PartitionKey, string RowKey, string Title)? GetThreadDetails(IServiceProvider services)
    {
        ThreadedEntity? threadedEntity;
        string threadTitle = string.Empty;

        switch (MessageType)
        {
            case NotificationType.match_video:
            case NotificationType.event_match_video:
                {
                    MatchVideo? data = GetDataAs<MatchVideo>();
                    threadedEntity = data;
                    Debug.Assert(!string.IsNullOrWhiteSpace(data?.event_name), "Bad data!");
                    if (data is not null)
                    {
                        threadTitle = $"{data.event_name!} | {data.match!.CompLevel.ToShortString()} {data.match.SetNumber}.{data.match.MatchNumber}";
                    }
                }

                break;
            case NotificationType.match_score:
                {
                    MatchScore? data = GetDataAs<MatchScore>();
                    threadedEntity = data;
                    Debug.Assert(!string.IsNullOrWhiteSpace(data?.event_name), "Bad data!");
                    if (data is not null)
                    {
                        threadTitle = $"{data.event_name!} | {data.match!.CompLevel.ToShortString()} {data.match.SetNumber}.{data.match.MatchNumber}";
                    }
                }

                break;
            case NotificationType.upcoming_match:
                {
                    UpcomingMatch? data = GetDataAs<UpcomingMatch>();
                    threadedEntity = data;
                    Debug.Assert(!string.IsNullOrWhiteSpace(data?.event_name), "Bad data!");
                    if (data is not null)
                    {
                        Match? matchData = services.GetRequiredService<IMatchApi>().GetMatch(data.match_key);
                        threadTitle = matchData is not null
                            ? $"{data.event_name} | {matchData.CompLevel.ToShortString()} {matchData.SetNumber}.{matchData.MatchNumber}"
                            : data.event_name;
                    }
                }

                break;
            default:
                return null;
        }

        return threadedEntity is null ? null : (threadedEntity.PartitionKey, threadedEntity.RowKey, threadTitle);
    }
}