namespace StatboticsKnownValues.SourceGen;

using System;
using System.Collections.Generic;

/// <summary>
/// Hardcoded mapping from Statbotics ORM class name to the corresponding /v3 list endpoint
/// template. Adding a new Statbotics list endpoint requires updating this table; the
/// source generator emits STATBOT003 if a parsed *ORM class isn't in this table.
/// Intentional friction — see plan §"Hardcoded endpoint→ORM mapping".
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
}
