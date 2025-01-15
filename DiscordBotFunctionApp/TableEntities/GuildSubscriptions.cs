namespace DiscordBotFunctionApp.TableEntities;
using System.Collections.Generic;
using System.Globalization;

internal class GuildSubscriptions : Dictionary<string, HashSet<ulong>>
{
    public IEnumerable<ulong> Guilds => this.Keys.Select(i => DiscordGuildId(i));
    public ulong DiscordGuildId(string guildId) => ulong.Parse(guildId, CultureInfo.InvariantCulture);

    public IReadOnlySet<ulong> SubscriptionsForGuild(ulong guildId) => this[guildId.ToString(CultureInfo.InvariantCulture)];

    public void AddSubscription(ulong guildId, ulong subscription) => AddSubscription(guildId.ToString(CultureInfo.InvariantCulture), subscription.ToString(CultureInfo.InvariantCulture));
    public void AddSubscription(ulong guildId, string subscription) => AddSubscription(guildId.ToString(CultureInfo.InvariantCulture), subscription);
    public void AddSubscription(string guildId, string subscription)
    {
        this.TryGetValue(guildId, out var subscriptions);
        this[guildId] = [.. subscriptions ?? [], ulong.Parse(subscription, CultureInfo.InvariantCulture)];
    }

    //private class GuildSubscriptionsConverter : JsonConverter<GuildSubscriptions>
    //{
    //    public override GuildSubscriptions? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    //    {
    //        if (reader.TokenType is not JsonTokenType.StartObject)
    //        {
    //            throw new JsonException();
    //        }

    //        var guildSubscriptions = new GuildSubscriptions();

    //        while (reader.Read())
    //        {
    //            if (reader.TokenType is JsonTokenType.EndObject)
    //            {
    //                return guildSubscriptions;
    //            }

    //            if (reader.TokenType is not JsonTokenType.PropertyName)
    //            {
    //                throw new JsonException();
    //            }

    //            var guildId = ulong.Parse(Throws.IfNullOrWhiteSpace(reader.GetString()), CultureInfo.InvariantCulture);

    //            reader.Read();
    //            if (reader.TokenType is not JsonTokenType.StartArray)
    //            {
    //                throw new JsonException();
    //            }

    //            var subscriptions = new HashSet<ulong>();

    //            while (reader.Read())
    //            {
    //                if (reader.TokenType is JsonTokenType.EndArray)
    //                {
    //                    break;
    //                }

    //                if (reader.TokenType is not JsonTokenType.String)
    //                {
    //                    throw new JsonException();
    //                }

    //                subscriptions.Add(ulong.Parse(Throws.IfNullOrWhiteSpace(reader.GetString()), CultureInfo.InvariantCulture));
    //            }

    //            guildSubscriptions.Add(guildId, subscriptions);
    //        }

    //        throw new JsonException();
    //    }

    //    public override void Write(Utf8JsonWriter writer, GuildSubscriptions value, JsonSerializerOptions options)
    //    {
    //        writer.WriteStartObject();
    //        foreach (var (guildId, subscriptions) in value)
    //        {
    //            writer.WriteStartArray(guildId.ToString(CultureInfo.InvariantCulture));
    //            foreach (var subscription in subscriptions)
    //            {
    //                writer.WriteNumberValue(subscription);
    //            }

    //            writer.WriteEndArray();
    //        }

    //        writer.WriteEndObject();
    //    }
    //}
}
