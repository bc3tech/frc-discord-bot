namespace ChatBot.Tools;

using Microsoft.Extensions.AI;

internal interface IProvideFunctionTools
{
    IReadOnlyList<AIFunction> Functions { get; }
}

internal static class IProvideFunctionToolsExtensions
{
    public static IReadOnlyList<AIFunction> CombineFunctions(this IEnumerable<IProvideFunctionTools> providers) => [.. providers.SelectMany(i => i.Functions)];
}
