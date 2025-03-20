
namespace TheBlueAlliance
{
#pragma warning disable CS8019
    using Microsoft.Extensions.Logging;

    using System;
#pragma warning restore CS8019

    static partial class Log
    {

        [LoggerMessage(0, LogLevel.Warning, "Unknown video type {Type} for match {MatchKey}")]
        internal static partial void UnknownVideoTypeTypeForMatchMatchKey(this ILogger logger, string Type, string MatchKey);

        [LoggerMessage(1, LogLevel.Warning, "Webcast type {Type} not supported! (Channel value: {Channel})")]
        internal static partial void WebcastTypeTypeNotSupportedChannelValueChannel(this ILogger logger, string Type, Uri? Channel);
    }
}
