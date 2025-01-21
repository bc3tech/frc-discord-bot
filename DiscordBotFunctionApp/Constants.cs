namespace DiscordBotFunctionApp;

using Microsoft.Extensions.Configuration;

internal static class Constants
{
    public static class ServiceKeys
    {
        public const string TableClient_TeamSubscriptions = "teamSubscriptions";
        public const string TableClient_EventSubscriptions = "eventSubscriptions";
    }

    public static class Configuration
    {
        public const string TbaApiKey = nameof(TbaApiKey);

        public static class Logging
        {
            public const string _Name = nameof(Logging);
            public const string LogLevel = nameof(LogLevel);

            public static class Discord
            {
                public static readonly string _Name = ConfigurationPath.Combine(Logging._Name, nameof(Discord));
                public static readonly string LogLevel = ConfigurationPath.Combine(_Name, nameof(LogLevel));
            }
        }

        public static class Discord
        {
            public const string _Name = nameof(Discord);
            public static readonly string LogLevel = ConfigurationPath.Combine(_Name, nameof(LogLevel));
            public static readonly string Token = ConfigurationPath.Combine(_Name, nameof(Token));
        }

        public static class Azure
        {
            public const string _Name = nameof(Azure);

            public static class Storage
            {
                public static readonly string _Name = ConfigurationPath.Combine(Azure._Name, nameof(Storage));
                public static readonly string TableEndpoint = ConfigurationPath.Combine(_Name, nameof(TableEndpoint));
                public static readonly string BlobsEndpoint = ConfigurationPath.Combine(_Name, nameof(BlobsEndpoint));
                public static readonly string Tables = ConfigurationPath.Combine(_Name, nameof(Tables));
            }
        }
    }
}
