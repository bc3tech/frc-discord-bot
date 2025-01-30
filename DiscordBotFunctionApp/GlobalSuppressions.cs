// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Performance", "EA0004:Make types declared in an executable internal", Justification = "Must be public to work with Discord.Net", Scope = "type", Target = "~T:DiscordBotFunctionApp.DiscordInterop.CommandModules.EventsCommandModule")]
[assembly: SuppressMessage("Resilience", "EA0014:The async method doesn't support cancellation", Justification = "Can't use CTs in command signatures as not compatible with injection model", Scope = "type", Target = "~T:DiscordBotFunctionApp.DiscordInterop.CommandModules.EventsCommandModule")]

[assembly: SuppressMessage("Performance", "EA0004:Make types declared in an executable internal", Justification = "Must be public to work with Discord.Net", Scope = "type", Target = "~T:DiscordBotFunctionApp.DiscordInterop.CommandModules.SubscriptionCommandModule")]
[assembly: SuppressMessage("Resilience", "EA0014:The async method doesn't support cancellation", Justification = "Can't use CTs in command signatures as not compatible with injection model", Scope = "type", Target = "~T:DiscordBotFunctionApp.DiscordInterop.CommandModules.SubscriptionCommandModule")]

[assembly: SuppressMessage("Performance", "EA0004:Make types declared in an executable internal", Justification = "Must be public to work with Discord.Net", Scope = "type", Target = "~T:DiscordBotFunctionApp.DiscordInterop.CommandModules.TeamsCommandModule")]
[assembly: SuppressMessage("Resilience", "EA0014:The async method doesn't support cancellation", Justification = "Can't use CTs in command signatures as not compatible with injection model", Scope = "type", Target = "~T:DiscordBotFunctionApp.DiscordInterop.CommandModules.TeamsCommandModule")]

[assembly: SuppressMessage("Performance", "EA0004:Make types declared in an executable internal", Justification = "Must be public to work with Discord.Net", Scope = "type", Target = "~T:DiscordBotFunctionApp.DiscordInterop.CommandModules.CommandModuleBase")]
