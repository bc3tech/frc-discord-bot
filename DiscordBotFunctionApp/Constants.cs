namespace DiscordBotFunctionApp;

using Microsoft.Extensions.Configuration;

internal static class Constants
{
    public static class ServiceKeys
    {
        public const string TableClient_TeamSubscriptions = "teamSubscriptions";
        public const string TableClient_EventSubscriptions = "eventSubscriptions";
        public const string TableClient_Threads = "threads";
        public const string TableClient_UserChatAgentThreads = "userChatAgentThreads";
        public const string TableClient_VectorStoreFiles = "vectorStoreFiles";

        public const string TheBlueAllianceHttpClient = nameof(TheBlueAllianceHttpClient);
        public const string StatboticsHttpClient = nameof(StatboticsHttpClient);
        public const string FIRSTHttpClient = nameof(FIRSTHttpClient);
        public const string ChatBotHttpClient = nameof(ChatBotHttpClient);
    }

    public static class Configuration
    {
        public const string TbaApiKey = nameof(TbaApiKey);
        public const string MatchSummariesDocumentUrl = nameof(MatchSummariesDocumentUrl);

        public static class Discord
        {
            public const string _Name = nameof(Discord);

            public static readonly string Token = ConfigurationPath.Combine(_Name, nameof(Token));
            public static readonly string LogLevel = ConfigurationPath.Combine(_Name, nameof(LogLevel));
        }

        public static class FIRST
        {
            public const string _Name = nameof(FIRST);

            public static readonly string Username = ConfigurationPath.Combine(_Name, nameof(Username));
            public static readonly string Password = ConfigurationPath.Combine(_Name, nameof(Password));
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

            public static class AI
            {
                public static readonly string _Name = ConfigurationPath.Combine(Azure._Name, nameof(AI));

                public static readonly string ProjectConnectionString = ConfigurationPath.Combine(_Name, nameof(ProjectConnectionString));
                public static readonly string ApiKey = ConfigurationPath.Combine(_Name, nameof(ApiKey));

                public static class Project
                {
                    public static readonly string _Name = ConfigurationPath.Combine(AI._Name, nameof(Project));
                    public static readonly string ConnectionString = ConfigurationPath.Combine(_Name, nameof(ConnectionString));

                    public static class Credentials
                    {
                        public static readonly string _Name = ConfigurationPath.Combine(Project._Name, nameof(Credentials));

                        public static readonly string TenantId = ConfigurationPath.Combine(_Name, nameof(TenantId));
                        public static readonly string ClientId = ConfigurationPath.Combine(_Name, nameof(ClientId));
                        public static readonly string ClientSecret = ConfigurationPath.Combine(_Name, nameof(ClientSecret));
                    }
                }

                public static class Agents
                {
                    public static readonly string _Name = ConfigurationPath.Combine(AI._Name, nameof(Agents));

                    public static readonly string AgentId = ConfigurationPath.Combine(_Name, nameof(AgentId));
                }
            }
        }
    }

    public static class SignalR
    {
        public static class Users
        {
            public const string EndUser = nameof(EndUser);
            public const string Orchestrator = nameof(Orchestrator);
        }

        public static class Functions
        {
            public const string GetAnswer = nameof(GetAnswer);
            public const string GetStreamedAnswer = nameof(GetStreamedAnswer);
            public const string SendStreamedAnswerBack = nameof(SendStreamedAnswerBack);
            public const string Introduce = nameof(Introduce);
            public const string Reintroduce = nameof(Reintroduce);
            public const string ExpertJoined = nameof(ExpertJoined);
            public const string ExpertLeft = nameof(ExpertLeft);
            public const string PostStatus = nameof(PostStatus);
        }
    }
}
