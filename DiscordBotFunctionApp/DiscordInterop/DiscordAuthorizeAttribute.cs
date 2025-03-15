namespace DiscordBotFunctionApp.DiscordInterop;

using Discord;
using Discord.Interactions;

using DiscordBotFunctionApp.Extensions;

using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
internal sealed class DiscordAuthorizeAttribute(GuildPermission requiredGuildPermissionSet) : Attribute
{
    public GuildPermission RequiredGuildPermissionSet { get; } = requiredGuildPermissionSet;
}

internal static class DiscordAuthorizeExtensions
{
    public static async Task ThrowIfUnauthorizedAsync(this InteractionModuleBase module, string? errorMessage = null, CancellationToken cancellationToken = default, [CallerMemberName] string? callingMethod = null)
    {
        if (module.Context.Guild is not null)
        {
            // Check to see if the user has the required permissions
            var guildUser = await module.Context.Guild.GetUserAsync(module.Context.User.Id, options: cancellationToken.ToRequestOptions()).ConfigureAwait(false);
            if (guildUser is not null)
            {
                // Get the method definition of the calling method
                var method = new StackFrame(1).GetMethod();
                Debug.Assert(method is not null);

                // Get the class definition of the claling method
                var classType = method!.DeclaringType;
                Debug.Assert(classType is not null);

                // Check to see if the class has the DiscordAuthorizeAttribute
                var discordAuthorizeAttributes = classType!.GetCustomAttributes(typeof(DiscordAuthorizeAttribute), inherit: true);

                checkAttributes();

                discordAuthorizeAttributes = method.GetCustomAttributes(typeof(DiscordAuthorizeAttribute), inherit: true);

                checkAttributes();

                void checkAttributes()
                {
                    foreach (DiscordAuthorizeAttribute discordAuthorizeAttribute in discordAuthorizeAttributes)
                    {
                        if (!guildUser.GuildPermissions.Has(discordAuthorizeAttribute.RequiredGuildPermissionSet))
                        {
                            throw new UnauthorizedAccessException(errorMessage);
                        }
                    }
                }
            }
        }
    }
}