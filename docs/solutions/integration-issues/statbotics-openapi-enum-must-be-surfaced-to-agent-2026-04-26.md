---
title: Statbotics returns HTTP 500 when query enum value is invalid; agent must see the enum
date: 2026-04-26
category: integration-issues
module: services/ChatBot/Tools
problem_type: integration_issue
component: assistant
symptoms:
  - "ChatBot.Tools.StatboticsTool warning: Http API tool call failed for statbotics-api ... with status 500"
  - "Agent gives up on a question it has the right tools for, after one or two failed Statbotics calls"
  - "Statbotics call URLs include type=<integer> (e.g. type=3) instead of a string enum value"
root_cause: wrong_api
resolution_type: code_fix
severity: medium
tags: [statbotics, openapi, enum, agent, surface-description, http-tools]
---

# Statbotics returns HTTP 500 when query enum value is invalid; agent must see the enum

## TL;DR

Calling `GET https://api.statbotics.io/v3/events?year=2026&type=3&limit=1000` returns
HTTP 500 (not 400). The Statbotics OpenAPI spec defines `type` as a string enum
(`regional`, `district`, `district_cmp`, `champs_div`, `champs_finals`, `offseason`,
`preseason`), but the bot's `statbotics_api_surface` tool was projecting parameters
as **names only**. The agent saw a `type` query parameter existed, guessed `type=3`
based on TBA conventions, and Statbotics' server crashed with a 500.

The fix is on our side: enrich the surface descriptor so each parameter ships with
its `in`, `required`, `type`, `enum`, `min`, `max`, and `description`. The agent now
has the information it needs to call the API correctly the first time.

## Symptoms

```
ChatBot.Tools.StatboticsTool: Warning: Http API tool call failed for statbotics-api
  https://api.statbotics.io/v3/events?year=2026&type=3&limit=1000 with status 500
ChatBot.Tools.StatboticsTool: Debug: Http API tool call for statbotics-api returned
  failure response snippet: []
```

Empty body, ~8 second response time (the upstream actually crashed instead of
short-circuiting on validation), and the agent abandoned the multi-turn task.

## Root cause

Two things lined up:

1. **Statbotics returns 500 for invalid enum values.** Confirmed against live API:
   - `?year=2026&type=champs_div` → 200 OK (5KB)
   - `?year=2026&type=3` → 500 Internal Server Error
   - `?year=2026&type=foo` → 500 Internal Server Error
   - `?year=2026` (no `type`) → 200 OK (159KB)

   This is an upstream bug — they should return 400 with an enum-mismatch message —
   but it's not ours to fix.

2. **Our surface description hid the enum.** `StatboticsTool.ReadParameters`
   collected only parameter `name` and `in`. The JSON returned to the agent looked
   like `"QueryParameters": ["year", "type", "week", "limit", ...]`. The agent had
   no signal that `type` required a specific string from a small set.

## Fix

Replaced the `string[] Parameters/PathParameters/QueryParameters` shape with a
structured `StatboticsParameter` record carrying `Name`, `In`, `Required`, `Type`,
`Enum`, `Minimum`, `Maximum`, and `Description`. `ReadParameters` now reads the
parameter's `schema` block from the embedded OpenAPI spec.

Also added explicit guidance in `DescribeApiSurfaceAsync` so the agent knows enums
are mandatory and that Statbotics returns 500 (not 400) on a miss.

Files touched:

- `services/ChatBot/Tools/StatboticsTool.cs`
  - New `StatboticsParameter` record
  - Updated `ReadParameters` to read `schema.type`, `schema.enum`, `schema.minimum`,
    `schema.maximum`, plus `description` and `required`
  - `StatboticsEndpoint.Matches` now searches by `parameter.Name`
  - Surface guidance string updated
- `tests/FunctionApp.Tests/HttpGetToolBaseTests.cs`
  - Updated assertion to read `parameter.GetProperty("Name").GetString()` and to
    verify the `type` query parameter exposes `Enum` containing `champs_div`

## Verification

- Build clean: `dotnet build tests/FunctionApp.Tests/FunctionApp.Tests.csproj`
  → 0 warnings, 0 errors
- All 17 `HttpGetToolBaseTests` pass

## Decision rubric

When you embed an OpenAPI spec for an LLM-facing tool surface:

- **Always** project parameter `enum` if present. Enums are the cheapest constraint
  the LLM can honor and the most expensive one to learn the hard way (especially
  when the upstream returns 5xx instead of 4xx).
- Project `type` (string vs integer vs boolean) — agents will guess wrong otherwise.
- Project `required`, `min`, `max`, and short `description`. Don't ship the entire
  schema tree; keep it cheap.
- Don't drop the parameter location (`in: path` vs `in: query`). The agent uses
  this to decide between path substitution and query string composition.

## Related

- `services/ChatBot/Tools/TbaApiTool.cs` had the same shape and the same blind
  spot. The TBA OpenAPI spec contains 47 enum declarations vs Statbotics' 6, but
  TBA's server returns proper 4xx on bad input, so it has been less visible.
  Fixed in the same series of commits — `TbaParameter` mirrors `StatboticsParameter`
  and the `tba_api_surface` tool now ships `In`, `Required`, `Type`, `Enum`,
  `Minimum`, `Maximum`, and `Description` for every documented parameter.

- See `copilot-sdk-tool-output-spills-need-real-disk-2026-04-26.md` for the
  unrelated session-FS issue surfaced by the same bot session log.
