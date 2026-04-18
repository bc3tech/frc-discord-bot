namespace ChatBot.Copilot;

using ChatBot.Agents;
using ChatBot.Configuration;
using ChatBot.Tools;

using GitHub.Copilot.SDK;

using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;

internal sealed class CopilotAgentCatalog(
    IOptions<AiOptions> options,
    PromptCatalog promptCatalog,
    IEnumerable<IProvideFunctionTools> toolProviders,
    CopilotFoundryProviderFactory providerFactory)
{
    public const string ParentAgentName = "bear-metal-assistant";
    public const string FrcDataSpecialistAgentName = "frc-data-specialist";

    private static readonly string SkillDirectoryPath = Path.Combine(AppContext.BaseDirectory, "ChatBot", "Copilot", "Skills");
    private readonly AiOptions _options = options.Value;
    private readonly PromptCatalog _promptCatalog = promptCatalog;
    private readonly IReadOnlyList<string> _localFrcToolNames = toolProviders.CombineToolNames(FunctionToolScope.LocalFrcData);
    private readonly CopilotFoundryProviderFactory _providerFactory = providerFactory;

    public SessionConfig CreateSessionConfig(IReadOnlyList<AIFunction> tools, SessionEventHandler? onEvent = null, CancellationToken cancellationToken = default)
        => new()
        {
            Agent = ParentAgentName,
            CustomAgents = CreateCustomAgents(),
            EnableConfigDiscovery = false,
            Model = _options.Copilot.Model,
            OnEvent = onEvent,
            OnPermissionRequest = PermissionHandler.ApproveAll,
            Provider = _providerFactory.CreateProviderConfig(cancellationToken),
            ReasoningEffort = _options.Copilot.ReasoningEffort,
            SkillDirectories = [SkillDirectoryPath],
            Streaming = true,
            Tools = [.. tools],
            WorkingDirectory = Directory.GetCurrentDirectory(),
        };

    public ResumeSessionConfig CreateResumeSessionConfig(IReadOnlyList<AIFunction> tools, SessionEventHandler? onEvent = null, CancellationToken cancellationToken = default)
        => new()
        {
            Agent = ParentAgentName,
            CustomAgents = CreateCustomAgents(),
            EnableConfigDiscovery = false,
            Model = _options.Copilot.Model,
            OnEvent = onEvent,
            OnPermissionRequest = PermissionHandler.ApproveAll,
            Provider = _providerFactory.CreateProviderConfig(cancellationToken),
            ReasoningEffort = _options.Copilot.ReasoningEffort,
            SkillDirectories = [SkillDirectoryPath],
            Streaming = true,
            Tools = [.. tools],
            WorkingDirectory = Directory.GetCurrentDirectory(),
        };

    private List<CustomAgentConfig> CreateCustomAgents()
        =>
        [
            new()
            {
                Name = ParentAgentName,
                DisplayName = "Bear Metal Assistant",
                Description = "User-facing Bear Metal Discord assistant.",
                Prompt = _promptCatalog.Format(
                    _promptCatalog.CopilotParentSystemMessage,
                    ("DEFAULT_TEAM_NUMBER", _options.DefaultTeamNumber.ToString(System.Globalization.CultureInfo.InvariantCulture))),
                Tools = [FoundrySpecialistTool.ToolName],
            },
            new()
            {
                Name = FrcDataSpecialistAgentName,
                DisplayName = "FRC Data Specialist",
                Description = "Specialist for TBA, Statbotics, and meal-signup lookups.",
                Infer = true,
                Prompt = _promptCatalog.Format(
                    _promptCatalog.CopilotFrcDataAgentPrompt,
                    ("DEFAULT_TEAM_NUMBER", _options.DefaultTeamNumber.ToString(System.Globalization.CultureInfo.InvariantCulture))),
                Tools = [.. _localFrcToolNames],
            },
        ];
}
