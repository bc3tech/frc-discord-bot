namespace FunctionApp;

using Microsoft.Extensions.Configuration;

internal static class Constants
{
    public static class ServiceKeys
    {
        public const string TableClient_TeamSubscriptions = "teamSubscriptionsv2";
        public const string TableClient_EventSubscriptions = "eventSubscriptions";
        public const string TableClient_Threads = "threads";
        public const string TableClient_UserChatAgentThreads = "userChatAgentThreads";
        public const string TableClient_ProcessedMessages = "processedMessages";

        public const string TheBlueAllianceHttpClient = nameof(TheBlueAllianceHttpClient);
        public const string StatboticsHttpClient = nameof(StatboticsHttpClient);
        public const string FIRSTHttpClient = nameof(FIRSTHttpClient);
        public const string ChatBotHttpClient = nameof(ChatBotHttpClient);
    }

    public static class Telemetry
    {
        public const string AppMeterName = "FunctionApp.Meter";
        public const string ChatBotActivitySourceName = "FunctionApp.ChatBot";
        public static class Metrics
        {
            public const string NumCountries = nameof(NumCountries);
        }
    }

    public static class Configuration
    {
        public const string TbaApiKey = nameof(TbaApiKey);
        public const string AllowDuplicateWebhooks = nameof(AllowDuplicateWebhooks);
        public const string MaxDaysToKeepStateData = nameof(MaxDaysToKeepStateData);

        public static class Discord
        {
            public const string _Name = nameof(Discord);

            public static readonly string Token = ConfigurationPath.Combine(_Name, nameof(Token));
            public static readonly string LogLevel = ConfigurationPath.Combine(_Name, nameof(LogLevel));
        }

        public static class FRC
        {
            public const string _Name = nameof(FRC);

            public static readonly string Username = ConfigurationPath.Combine(_Name, nameof(Username));
            public static readonly string Password = ConfigurationPath.Combine(_Name, nameof(Password));
        }

        public static class Azure
        {
            public const string _Name = nameof(Azure);

            public static readonly string TenantId = ConfigurationPath.Combine(_Name, nameof(TenantId));
            public static readonly string ClientId = ConfigurationPath.Combine(_Name, nameof(ClientId));

            public static class Storage
            {
                public static readonly string _Name = ConfigurationPath.Combine(Azure._Name, nameof(Storage));

                public static readonly string TableEndpoint = ConfigurationPath.Combine(_Name, nameof(TableEndpoint));
                public static readonly string BlobsEndpoint = ConfigurationPath.Combine(_Name, nameof(BlobsEndpoint));
                public static readonly string Tables = ConfigurationPath.Combine(_Name, nameof(Tables));
            }
        }

        public static class AI
        {
            public const string _Name = nameof(AI);

            public static class Foundry
            {
                public static readonly string _Name = ConfigurationPath.Combine(AI._Name, nameof(Foundry));

                public static readonly string Endpoint = ConfigurationPath.Combine(_Name, nameof(Endpoint));
                public static readonly string AgentId = ConfigurationPath.Combine(_Name, nameof(AgentId));
                public static readonly string MealSignupGeniusId = ConfigurationPath.Combine(_Name, nameof(MealSignupGeniusId));
                public static readonly string LocalAgentModel = ConfigurationPath.Combine(_Name, nameof(LocalAgentModel));
                public static readonly string OpenAIApiVersion = ConfigurationPath.Combine(_Name, nameof(OpenAIApiVersion));
                public static readonly string MaxLocalAgentHandoffs = ConfigurationPath.Combine(_Name, nameof(MaxLocalAgentHandoffs));
                public static readonly string MaxWorkflowSteps = ConfigurationPath.Combine(_Name, nameof(MaxWorkflowSteps));
                public static readonly string WorkflowSoftTimeoutSeconds = ConfigurationPath.Combine(_Name, nameof(WorkflowSoftTimeoutSeconds));
                public static readonly string WorkflowHardTimeoutSeconds = ConfigurationPath.Combine(_Name, nameof(WorkflowHardTimeoutSeconds));
                public static readonly string MaxPrematureLocalLookupRetries = ConfigurationPath.Combine(_Name, nameof(MaxPrematureLocalLookupRetries));

                public static class EvaluationSettings
                {
                    public static readonly string _Name = ConfigurationPath.Combine(Foundry._Name, nameof(EvaluationSettings));

                    public static readonly string Model = ConfigurationPath.Combine(_Name, nameof(Model));
                    public static readonly string MaxAnswerEvaluationRetries = ConfigurationPath.Combine(_Name, nameof(MaxAnswerEvaluationRetries));
                    public static readonly string TimeoutSeconds = ConfigurationPath.Combine(_Name, nameof(TimeoutSeconds));
                }
            }

            public static class AgentLogging
            {
                public static readonly string _Name = ConfigurationPath.Combine(AI._Name, nameof(AgentLogging));

                public static readonly string StreamInternalDialog = ConfigurationPath.Combine(_Name, nameof(StreamInternalDialog));
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

    public static class InteractionElements
    {
        public const string CancelButtonDeleteMessage = nameof(CancelButtonDeleteMessage);
    }
}
