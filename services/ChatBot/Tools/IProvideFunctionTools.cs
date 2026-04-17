namespace ChatBot.Tools;

using Microsoft.Extensions.AI;

internal interface IProvideFunctionTools
{
    FunctionToolScope Scope { get; }

    IReadOnlyList<AIFunction> Functions { get; }

    IReadOnlyList<string> ToolNames { get; }
}

internal enum FunctionToolScope
{
    LocalFrcData = 0,
}

internal static class IProvideFunctionToolsExtensions
{
    public static IReadOnlyList<AIFunction> CombineFunctions(this IEnumerable<IProvideFunctionTools> providers) => [.. providers.SelectMany(i => i.Functions)];

    public static IReadOnlyList<AIFunction> CombineFunctions(this IEnumerable<IProvideFunctionTools> providers, FunctionToolScope scope)
        => [.. providers.Where(provider => provider.Scope == scope).SelectMany(provider => provider.Functions)];

    public static IReadOnlyList<string> CombineToolNames(this IEnumerable<IProvideFunctionTools> providers, FunctionToolScope scope)
        => [.. providers.Where(provider => provider.Scope == scope).SelectMany(provider => provider.ToolNames)];
}
