namespace DiscordBotFunctionApp.Storage.TableEntities;

using System.Collections.Generic;
using System.Globalization;

internal sealed class GuildSubscriptions : Dictionary<string, HashSet<ulong>>
{
    const string DmGuildIdentifier = "dm";
    public IEnumerable<ulong?> Guilds => Keys.Select(DiscordGuildId);
    private static ulong? DiscordGuildId(string guildId) => guildId is DmGuildIdentifier ? null : ulong.Parse(guildId, CultureInfo.InvariantCulture);

    public IReadOnlySet<ulong> SubscriptionsForGuild(ulong? guildId) => this.GetValueOrDefault(guildId?.ToString(CultureInfo.InvariantCulture) ?? DmGuildIdentifier, []);

    public void AddSubscription(ulong? guildId, ulong subscription) => AddSubscription(guildId?.ToString(CultureInfo.InvariantCulture) ?? DmGuildIdentifier, subscription.ToString(CultureInfo.InvariantCulture));
    public void AddSubscription(ulong? guildId, string subscription) => AddSubscription(guildId?.ToString(CultureInfo.InvariantCulture) ?? DmGuildIdentifier, subscription);
    public void AddSubscription(string guildId, string subscription)
    {
        TryGetValue(guildId, out var subscriptions);
        this[guildId] = [.. subscriptions ?? [], subscription.Equals(CommonConstants.ALL, StringComparison.OrdinalIgnoreCase) ? 0 : ulong.Parse(subscription, CultureInfo.InvariantCulture)];
    }

    public bool Exists(ulong? guildId, ulong subscription) => this.TryGetValue(guildId?.ToString(CultureInfo.InvariantCulture) ?? DmGuildIdentifier, out var channels) && channels.Contains(subscription);

    public void RemoveSubscription(ulong? guildId, ulong subscription) => RemoveSubscription(guildId?.ToString(CultureInfo.InvariantCulture) ?? DmGuildIdentifier, subscription.ToString(CultureInfo.InvariantCulture));

    public void RemoveSubscription(ulong? guildId, string subscription) => RemoveSubscription(guildId?.ToString(CultureInfo.InvariantCulture) ?? DmGuildIdentifier, subscription);

    public void RemoveSubscription(string guildId, string subscription)
    {
        if (TryGetValue(guildId, out var subscriptions))
        {
            subscriptions.Remove(subscription.Equals(CommonConstants.ALL, StringComparison.OrdinalIgnoreCase) ? 0 : ulong.Parse(subscription, CultureInfo.InvariantCulture));
        }
    }
}
