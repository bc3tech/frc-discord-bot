namespace ChatBot;

using Microsoft.Extensions.Configuration;

internal static class ChatBotConstants
{
    internal static class ServiceKeys
    {
        public const string TableClient_UserChatAgentThreads = "userChatAgentThreads";
        public const string TableClient_ConversationTraces = "conversationtraces";
        public const string TableClient_CopilotSessions = "copilotsessions";
        public const string BlobContainer_CopilotSessions = "copilot-sessions";
    }

    internal static class HttpClients
    {
        public const string MealSignupInfo = "ChatBot.MealSignupInfo";
        public const string TbaApi = "tba-api";
        public const string StatboticsApi = "statbotics-api";
    }

    internal static class Configuration
    {
        public static readonly string DefaultTeamNumber = nameof(DefaultTeamNumber);

        internal static class Copilot
        {
            private static readonly string Name = ConfigurationPath.Combine(nameof(AI), nameof(Copilot));

            public static readonly string Model = ConfigurationPath.Combine(Name, nameof(Model));
            public static readonly string ReasoningEffort = ConfigurationPath.Combine(Name, nameof(ReasoningEffort));
            public static readonly string ResponseTimeoutSeconds = ConfigurationPath.Combine(Name, nameof(ResponseTimeoutSeconds));
            public static readonly string LogLevel = ConfigurationPath.Combine(Name, nameof(LogLevel));

            internal static class Telemetry
            {
                public static readonly string Name = ConfigurationPath.Combine(Copilot.Name, nameof(Telemetry));
            }
        }

        internal static class Foundry
        {
            private static readonly string Name = ConfigurationPath.Combine(nameof(AI), nameof(Foundry));

            public static readonly string Endpoint = ConfigurationPath.Combine(Name, nameof(Endpoint));
            public static readonly string AgentId = ConfigurationPath.Combine(Name, nameof(AgentId));
            public static readonly string MealSignupGeniusId = ConfigurationPath.Combine(Name, nameof(MealSignupGeniusId));
            public static readonly string LocalAgentModel = ConfigurationPath.Combine(Name, nameof(LocalAgentModel));
            public static readonly string OpenAIApiVersion = ConfigurationPath.Combine(Name, nameof(OpenAIApiVersion));
            public static readonly string MaxLocalAgentHandoffs = ConfigurationPath.Combine(Name, nameof(MaxLocalAgentHandoffs));
            public static readonly string MaxWorkflowSteps = ConfigurationPath.Combine(Name, nameof(MaxWorkflowSteps));
            public static readonly string WorkflowSoftTimeoutSeconds = ConfigurationPath.Combine(Name, nameof(WorkflowSoftTimeoutSeconds));
            public static readonly string WorkflowHardTimeoutSeconds = ConfigurationPath.Combine(Name, nameof(WorkflowHardTimeoutSeconds));
            public static readonly string MaxPrematureLocalLookupRetries = ConfigurationPath.Combine(Name, nameof(MaxPrematureLocalLookupRetries));

            internal static class EvaluationSettings
            {
                private static readonly string Name = ConfigurationPath.Combine(Foundry.Name, nameof(EvaluationSettings));

                public static readonly string Model = ConfigurationPath.Combine(Name, nameof(Model));
                public static readonly string MaxAnswerEvaluationRetries = ConfigurationPath.Combine(Name, nameof(MaxAnswerEvaluationRetries));
                public static readonly string TimeoutSeconds = ConfigurationPath.Combine(Name, nameof(TimeoutSeconds));
            }
        }

        internal static class AI
        {
            private static readonly string Name = nameof(AI);

            internal static class AgentLogging
            {
                private static readonly string Name = ConfigurationPath.Combine(AI.Name, nameof(AgentLogging));
            }
        }
    }
}
