namespace StatboticsKnownValues.SourceGen;

using System;
using System.Collections.Generic;

/// <summary>
/// Hardcoded mapping from Statbotics ORM class name to the corresponding /v3 list endpoint
/// template. Adding a new Statbotics list endpoint requires updating this table; the
/// source generator emits STATBOT003 if a parsed *ORM class isn't in this table AND
/// isn't in <see cref="IntentionallyIgnored"/>. Intentional friction — see plan
/// §"Hardcoded endpoint→ORM mapping".
/// </summary>
internal static class EndpointMapping
{
    public static readonly IReadOnlyDictionary<string, string> OrmClassToEndpoint =
        new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["TeamORM"] = "/v3/teams",
            ["YearORM"] = "/v3/years",
            ["TeamYearORM"] = "/v3/team_years",
            ["EventORM"] = "/v3/events",
            ["TeamEventORM"] = "/v3/team_events",
            ["MatchORM"] = "/v3/matches",
            ["TeamMatchORM"] = "/v3/team_matches",
        };

    /// <summary>
    /// ORM classes that exist upstream but are not /v3 list endpoints (e.g., internal
    /// cache tables). The source generator silently ignores these instead of emitting
    /// STATBOT003. If a new ORM class appears upstream, the generator will warn so the
    /// maintainer can decide: add it to OrmClassToEndpoint (real endpoint) or here
    /// (internal/non-API).
    /// </summary>
    public static readonly HashSet<string> IntentionallyIgnored =
        new(StringComparer.Ordinal)
        {
            "ETagORM",
        };
}
