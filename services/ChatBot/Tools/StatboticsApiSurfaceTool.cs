namespace ChatBot.Tools;

using BC3Technologies.DiscordGpt.Core;

using Microsoft.Extensions.AI;

internal sealed class StatboticsApiSurfaceTool(StatboticsTool tool) : IDiscordTool
{
    public string Name => StatboticsTool.DescribeSurfaceToolName;

    public string Description => StatboticsTool.DescribeSurfaceToolDescription;

    public AIFunction AsFunction() => tool.AsSurfaceFunction();
}
