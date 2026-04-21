namespace ChatBot.Tools;

using BC3Technologies.DiscordGpt.Core;

using Microsoft.Extensions.AI;

internal sealed class TbaApiSurfaceTool(TbaApiTool tool) : IDiscordTool
{
    public string Name => TbaApiTool.DescribeSurfaceToolName;

    public string Description => TbaApiTool.DescribeSurfaceToolDescription;

    public AIFunction AsFunction() => tool.AsSurfaceFunction();
}
