namespace StatboticsKnownValues.SourceGen;

using Microsoft.CodeAnalysis;

/// <summary>
/// Source-generator diagnostic descriptors. STATBOT001 fires when an expected ORM file
/// produces zero columns. STATBOT002 fires when the country/state JSON snapshot is
/// missing. STATBOT003 fires when a parsed ORM class isn't in EndpointMapping.
/// </summary>
internal static class Diagnostics
{
    public static readonly DiagnosticDescriptor OrmFileProducedZeroColumns = new(
        id: "STATBOT001",
        title: "Statbotics ORM file produced zero columns",
        messageFormat: "Statbotics ORM file '{0}' produced zero columns; upstream conventions may have changed",
        category: "StatboticsKnownValues",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor CountryStateSnapshotMissing = new(
        id: "STATBOT002",
        title: "Statbotics country/state snapshot missing",
        messageFormat: "Statbotics country/state snapshot file not found among AdditionalFiles; run Tools/CountryStateRefresh and verify ChatBot.csproj wiring",
        category: "StatboticsKnownValues",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor OrmClassNotInMapping = new(
        id: "STATBOT003",
        title: "Statbotics ORM class not in endpoint mapping",
        messageFormat: "Statbotics ORM class '{0}' was parsed but is not in EndpointMapping.OrmClassToEndpoint; either add a mapping entry or narrow the parser scope",
        category: "StatboticsKnownValues",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);
}
