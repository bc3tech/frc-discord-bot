namespace ChatBot;

using Microsoft.Extensions.Configuration;

internal static class ChatBotConstants
{
    internal static class ServiceKeys
    {
        public const string TableClient_UserChatAgentThreads = "userChatAgentThreads";
    }

    internal static class HttpClients
    {
        public const string MealSignupInfo = "ChatBot.MealSignupInfo";
    }

    internal static class Configuration
    {
        public static readonly string DefaultTeamNumber = nameof(DefaultTeamNumber);

        internal static class Azure
        {
            private static readonly string Name = nameof(Azure);

            public static readonly string ClientId = ConfigurationPath.Combine(Name, nameof(ClientId));
            public static readonly string TenantId = ConfigurationPath.Combine(Name, nameof(TenantId));
        }

        internal static class AI
        {
            private static readonly string Name = nameof(AI);

            internal static class Azure
            {
                private static readonly string Name = ConfigurationPath.Combine(AI.Name, nameof(Azure));

                public static readonly string ProjectEndpoint = ConfigurationPath.Combine(Name, nameof(ProjectEndpoint));
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
                    private static readonly string Name = ConfigurationPath.Combine(Azure.Name, nameof(EvaluationSettings));

                    public static readonly string Model = ConfigurationPath.Combine(Name, nameof(Model));
                    public static readonly string MaxAnswerEvaluationRetries = ConfigurationPath.Combine(Name, nameof(MaxAnswerEvaluationRetries));
                    public static readonly string TimeoutSeconds = ConfigurationPath.Combine(Name, nameof(TimeoutSeconds));
                }
            }

            internal static class Agent365
            {
                private static readonly string Name = ConfigurationPath.Combine(AI.Name, nameof(Agent365));

                public static readonly string Enabled = ConfigurationPath.Combine(Name, nameof(Enabled));
                public static readonly string TenantId = ConfigurationPath.Combine(Name, nameof(TenantId));
                public static readonly string BlueprintClientId = ConfigurationPath.Combine(Name, nameof(BlueprintClientId));
                public static readonly string ManagedIdentityClientId = ConfigurationPath.Combine(Name, nameof(ManagedIdentityClientId));
                public static readonly string AgentIdentityClientId = ConfigurationPath.Combine(Name, nameof(AgentIdentityClientId));
                public static readonly string AutoCreateIdentity = ConfigurationPath.Combine(Name, nameof(AutoCreateIdentity));
                public static readonly string AgentIdentityDisplayName = ConfigurationPath.Combine(Name, nameof(AgentIdentityDisplayName));
                public static readonly string Sponsors = ConfigurationPath.Combine(Name, nameof(Sponsors));
                public static readonly string TokenExchangeAudience = ConfigurationPath.Combine(Name, nameof(TokenExchangeAudience));
                public static readonly string ProbeScope = ConfigurationPath.Combine(Name, nameof(ProbeScope));
            }

            internal static class AgentLogging
            {
                private static readonly string Name = ConfigurationPath.Combine(AI.Name, nameof(AgentLogging));

                public static readonly string StreamInternalDialog = ConfigurationPath.Combine(Name, nameof(StreamInternalDialog));
            }
        }
    }
}
