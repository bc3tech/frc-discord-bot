namespace DiscordBotFunctionApp.TbaInterop.Models;

using DiscordBotFunctionApp.TbaInterop.Models.Notifications;

using Microsoft.Extensions.DependencyInjection;

using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

using TheBlueAlliance.Api;
using TheBlueAlliance.Model.MatchExtensions;

internal sealed record WebhookMessage
{
    [JsonPropertyName("message_type")]
    public NotificationType MessageType { get; init; }

    [JsonPropertyName("message_data")]
    required public JsonElement MessageData { get; init; }

    public T? GetDataAs<T>() => MessageData.Deserialize<T>();

    public (string PartitionKey, string RowKey, string Title)? GetThreadDetails(IServiceProvider services)
    {
        ThreadedEntity? threadedEntity;
        string threadTitle = string.Empty;

        switch (this.MessageType)
        {
            case NotificationType.match_video:
            case NotificationType.event_match_video:
                {
                    var data = GetDataAs<MatchVideo>();
                    threadedEntity = data;
                    Debug.Assert(!string.IsNullOrWhiteSpace(data?.event_name), "Bad data!");
                    if (data is not null)
                    {
                        threadTitle = $"{data.event_name!} | {Translator.CompLevelToShortString(data.match!.CompLevel.ToInvariantString())} {data.match.SetNumber}.{data.match.MatchNumber}";
                    }
                }

                break;
            case NotificationType.match_score:
                {
                    var data = GetDataAs<MatchScore>();
                    threadedEntity = data;
                    Debug.Assert(!string.IsNullOrWhiteSpace(data?.event_name), "Bad data!");
                    if (data is not null)
                    {
                        threadTitle = $"{data.event_name!} | {Translator.CompLevelToShortString(data.match!.CompLevel.ToInvariantString())} {data.match.SetNumber}.{data.match.MatchNumber}";
                    }
                }

                break;
            case NotificationType.upcoming_match:
                {
                    var data = GetDataAs<UpcomingMatch>();
                    threadedEntity = data;
                    Debug.Assert(!string.IsNullOrWhiteSpace(data?.event_name), "Bad data!");
                    if (data is not null)
                    {
                        var matchData = services.GetRequiredService<IMatchApi>().GetMatch(data.match_key);
                        threadTitle = matchData is not null
                            ? $"{data.event_name} | {Translator.CompLevelToShortString(matchData.CompLevel.ToInvariantString())} {matchData.SetNumber}.{matchData.MatchNumber}"
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