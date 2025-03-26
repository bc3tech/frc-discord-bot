using DiscordBotFunctionApp;

namespace FunctionApp.Storage.TableEntities;

using Microsoft.Extensions.Logging;

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

    public bool Exists(ulong? guildId, ulong subscription) => TryGetValue(guildId?.ToString(CultureInfo.InvariantCulture) ?? DmGuildIdentifier, out var channels) && channels.Contains(subscription);

    public void RemoveSubscription(ulong? guildId, ulong subscription, ILogger? logger = null) => RemoveSubscription(guildId?.ToString(CultureInfo.InvariantCulture) ?? DmGuildIdentifier, subscription.ToString(CultureInfo.InvariantCulture), logger);

    public void RemoveSubscription(ulong? guildId, string subscription, ILogger? logger = null) => RemoveSubscription(guildId?.ToString(CultureInfo.InvariantCulture) ?? DmGuildIdentifier, subscription, logger);

    public void RemoveSubscription(string guildId, string subscription, ILogger? logger = null)
    {
        if (TryGetValue(guildId, out var subscriptions))
        {
            if (!subscriptions.Remove(subscription.Equals(CommonConstants.ALL, StringComparison.OrdinalIgnoreCase) ? 0 : ulong.Parse(subscription, CultureInfo.InvariantCulture)))
            {
                logger?.AttemptedToRemoveSubscriptionSubscriptionFromGuildGuildIdButItWasnTFound(subscription, guildId);
            }
        }
        else
        {
            logger?.AttemptedToRemoveSubscriptionFromNonExistentGuild();
        }
    }
}
