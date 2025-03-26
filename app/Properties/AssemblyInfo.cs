using Discord;

using Microsoft.Extensions.EnumStrings;

using System.Runtime.CompilerServices;

[assembly: EnumStrings(typeof(ChannelType))]

[assembly: InternalsVisibleTo("FunctionApp.Tests")]