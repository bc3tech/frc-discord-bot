namespace ChatBot.Tools;

using BC3Technologies.DiscordGpt.Core;

using Microsoft.Extensions.AI;

using System.Collections.ObjectModel;

internal sealed class MealSignupInfoDiscordTool(MealSignupInfoTool tool) : IDiscordTool
{
    public string Name => "fetch_meal_signup_info";

    public string Description => "Fetch the current Bear Metal meal signup data from SignupGenius.";

    public AIFunction AsFunction()
        => AIFunctionFactory.Create(
            tool.FetchMealSignupInfoResponseBodyAsync,
            DiscordToolFactory.CreateSkippableOptions(Name, Description));
}

internal sealed class TbaApiSurfaceDiscordTool(TbaApiTool tool) : IDiscordTool
{
    public string Name => "tba_api_surface";

    public string Description => "Describes the valid The Blue Alliance API v3 endpoint surface.";

    public AIFunction AsFunction()
        => AIFunctionFactory.Create(
            tool.DescribeApiSurfaceAsync,
            DiscordToolFactory.CreateSkippableOptions(Name, Description));
}

internal sealed class TbaApiQueryDiscordTool(TbaApiTool tool) : IDiscordTool
{
    public string Name => "tba_api";

    public string Description => "Calls the official The Blue Alliance API v3 for FRC competition data.";

    public AIFunction AsFunction()
        => AIFunctionFactory.Create(
            tool.QueryTbaAsync,
            DiscordToolFactory.CreateSkippableOptions(Name, Description));
}

internal sealed class StatboticsDiscordTool(StatboticsTool tool) : IDiscordTool
{
    public string Name => "statbotics_api";

    public string Description => "Calls the public Statbotics API for advanced FRC metrics.";

    public AIFunction AsFunction()
        => AIFunctionFactory.Create(
            tool.QueryStatboticsAsync,
            DiscordToolFactory.CreateSkippableOptions(Name, Description));
}

internal static class DiscordToolFactory
{
    private static readonly ReadOnlyDictionary<string, object?> s_skipPermissionProperties = new(
        new Dictionary<string, object?>
        {
            ["skip_permission"] = true,
        });

    public static AIFunctionFactoryOptions CreateSkippableOptions(string name, string description)
        => new()
        {
            Name = name,
            Description = description,
            AdditionalProperties = s_skipPermissionProperties,
        };
}
