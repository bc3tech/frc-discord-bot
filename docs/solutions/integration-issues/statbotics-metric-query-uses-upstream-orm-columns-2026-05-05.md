---
title: Statbotics `metric` query parameter must use endpoint-specific ORM column names
date: 2026-05-05
category: integration-issues
module: ChatBot
problem_type: integration_issue
component: assistant
severity: medium
symptoms:
  - Local Statbotics API guidance treated `metric` values as if Python client constants or nested response fields were authoritative
  - Agents could choose invalid metric names for REST list endpoints (e.g., `metric=epa` on `/v3/events`)
  - Stale enum values like `epa_top8` and `epa_top24` did not match upstream REST metric column names
root_cause: wrong_api
resolution_type: code_fix
tags:
  - statbotics
  - openapi
  - api-enums
  - agent-prompts
  - metric-query
  - orm-columns
  - frc-data
---

# Statbotics `metric` query parameter must use endpoint-specific ORM column names

## Problem

The Statbotics REST API `metric` query parameter accepts only endpoint-specific backend SQLAlchemy ORM column names — not Python client constants, nested response fields, or made-up abbreviations. Local API guidance was unclear on this distinction, causing agents to construct plausible-looking but invalid queries like `GET /v3/events?year=2026&metric=epa`.

The biggest trap: the same metric name can be valid on one endpoint and invalid on another. `metric=epa` works on `/v3/team_events` (uses `TeamEventORM`) but not on `/v3/events` (uses `EventORM`, which has no `epa` column).

## Symptoms

- Agent constructs `GET /v3/events?metric=epa` and receives a 400 or 500 error
- Guidance implied that plain `epa` was a universal sort key across all Statbotics endpoints
- Stale client-style names (`epa_top8`, `auto_epa_start`) appeared valid when they were not REST-compatible

## What Didn't Work

**Treating Python client constants as authoritative.** Client libraries may expose shorthand names or legacy aliases that don't match backend ORM column names. Example: Python client `epa_top8` should be REST ORM column `epa_top_8`.

**Assuming nested response fields map to `metric` values.** Response JSON often has nested structure (e.g., `{"epa": {"mean": 45.2}}`) but the `metric` query parameter sorts against flat ORM columns (`epa_mean`).

**Treating metrics as endpoint-agnostic.** Each Statbotics endpoint uses a different ORM class (`EventORM`, `TeamEventORM`, `TeamORM`, etc.), and valid `metric` values depend entirely on which ORM model backs the endpoint.

## Solution

**Ground all `metric` guidance in the endpoint's backend ORM class**, not in Python client constants or response JSON shape. The upstream source is `backend/src/db/read/main.py`, where the REST handler applies sorting via `model_orm.__dict__[metric]` — so valid `metric` values are exactly the column names on the ORM class for that endpoint.

### `/v3/events` — uses `EventORM` columns

Plain `metric=epa` is invalid because `EventORM` has no `epa` column. Use aggregate event-level columns instead:

```
/v3/events?year=2026&metric=epa_mean
/v3/events?year=2026&metric=epa_max
/v3/events?year=2026&metric=epa_top_8
/v3/events?year=2026&metric=epa_top_24
/v3/events?year=2026&metric=epa_sd
```

Accuracy/result columns (`epa_acc`, `epa_mse`, `epa_rp_*`) should only be treated as useful after matches exist.

### `/v3/team_events` — uses `TeamEventORM` columns

Plain `metric=epa` is **valid and preferred** for upcoming/future projected team EPA:

```
/v3/team_events?event=2026wasno&metric=epa
```

Other valid team-event EPA columns:

```
/v3/team_events?event=2026wasno&metric=epa_start
```

Completed-event ranking/aggregate fields only become useful after the team has played matches:

```
/v3/team_events?event=2026wasno&metric=rank
/v3/team_events?event=2026wasno&metric=rps
/v3/team_events?event=2026wasno&metric=record
/v3/team_events?event=2026wasno&metric=winrate
/v3/team_events?event=2026wasno&metric=epa_mean
/v3/team_events?event=2026wasno&metric=epa_max
```

### Correcting stale client-style names

Some older names were client conventions, not REST ORM columns:

```diff
-/v3/events?year=2026&metric=epa_top8
+/v3/events?year=2026&metric=epa_top_8

-/v3/events?year=2026&metric=epa_top24
+/v3/events?year=2026&metric=epa_top_24

-/v3/team_events?event=2026wasno&metric=auto_epa_start
+/v3/team_events?event=2026wasno&metric=auto_epa
```

### Local changes applied

Updated local API definitions and agent guidance to reflect endpoint-specific ORM columns:

- **`app\Apis\statbotics.json`**: Updated `metric` enums and descriptions for `/v3/events` and `/v3/team_events` to include only ORM-backed column names. Corrected stale names (`epa_top_8`, `epa_top_24`, etc.).

- **`services\ChatBot\Tools\StatboticsTool.cs`**: Updated `statbotics_api_surface` guidance to clarify endpoint-specific metric validity and avoid mixing naming systems.

- **`services\ChatBot\agent_prompt.txt` and `local_agent_prompt.txt`**: Updated production/local agent instructions with endpoint-specific Statbotics `metric` guidance for future-event queries.

- **`tests\FunctionApp.Tests\HttpGetToolBaseTests.cs`**: Added/updated tests covering Statbotics API surface guidance.

## Why This Works

The Statbotics REST backend applies the `metric` parameter by indexing the ORM model directly: `model_orm.__dict__[metric]`. If the ORM class doesn't have that column, the request fails. There is no "fallback" or "alias resolution" — the `metric` value must be an actual ORM column name on the endpoint's backend class.

Grounding guidance in the source-of-truth backend ORM columns prevents three common naming-system confusions:

1. **REST query parameter values** (what's valid for `metric=`)
2. **Python client constants** (convenience shortcuts)
3. **Nested JSON response fields** (human-friendly structure)

Only the first one matters for the `metric` query parameter. By treating the ORM model as the authority, we eliminate ambiguity.

## Prevention

### Before documenting or recommending a `metric` value:

1. Check the endpoint's backend ORM class in the `avgupta456/statbotics` repository (`backend/src/db/read/models.py` or `backend/src/db/orm.py`)
2. Verify the ORM class has a column with that exact name
3. Confirm the column is meaningful for the query context (e.g., future vs. past events)
4. Use the ORM column name as-is in guidance — no translation or abbreviation

### When adding or updating Statbotics integration:

- **OpenAPI enums** (`app\Apis\statbotics.json`): List only ORM-backed column names. Verify against upstream schema.
- **Agent prompts**: Include endpoint-specific metric guidance. Show example `metric=` values with endpoint paths to avoid cross-endpoint confusion.
- **Tool surface documentation** (`StatboticsTool.cs`): Clarify that query parameters stay in the query string and reference upstream ORM columns.
- **Tests**: Assert that example `metric` values match the intended endpoint's ORM model.

### Related practices:

- Document which endpoints use which ORM models (e.g., "EventORM for `/v3/events`").
- For future-event queries, note which metrics are pre-filled vs. only available post-match.
- Link to upstream backend models as the source-of-truth in comments and tool descriptions.

## Related Issues

- **[statbotics-openapi-enum-must-be-surfaced-to-agent-2026-04-26.md](docs/solutions/integration-issues/statbotics-openapi-enum-must-be-surfaced-to-agent-2026-04-26.md)** — Broader pattern: surface upstream API constraints (enum, type constraints, etc.) to agents via OpenAPI and tool guidance to prevent invalid query construction.
- **[prompt-ssot-via-discovery-with-lockstep-2026-04-27.md](docs/solutions/best-practices/prompt-ssot-via-discovery-with-lockstep-2026-04-27.md)** — Related: keeping agent prompts and OpenAPI specs in lockstep so guidance always reflects current API surface.
- **avgupta456/statbotics** `backend/src/db/read/main.py` and `backend/src/db/orm.py` — upstream REST implementation and ORM model definitions (source-of-truth).
