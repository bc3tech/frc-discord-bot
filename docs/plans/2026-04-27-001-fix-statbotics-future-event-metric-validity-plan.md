---
title: "fix: Teach the chatbot which Statbotics metrics are valid for future events"
type: fix
status: active
date: 2026-04-27
origin: docs/brainstorms/2026-04-27-statbotics-future-event-metric-validity-requirements.md
---

# fix: Teach the chatbot which Statbotics metrics are valid for future events

## Overview

Statbotics' `/v3/team_events`, `/v3/team_matches`, `/v3/matches`, and `/v3/events`
endpoints serve rows for future events (status = `Upcoming`). On those rows,
some fields hold meaningful pre-event projections (e.g. `epa.total_points.mean`,
`epa.breakdown.*`), but many other fields are zero or null because no matches
have been played yet (records, ranks, district points, week, `epa.stats.{pre_elim,
mean, max}`, match scores, alliance assignments, etc.). The local agent
currently treats those zeros as real values and reports them to the user as
authoritative facts — e.g. "Team 27 has a 0-0 qualification record at Archimedes
2026 with rank 0".

This plan lands the **prompt-only** fix scoped in the origin requirements doc:
the local agent must refuse-and-redirect on completed-only metrics for future
events, and may only quote pre-event projections when present and clearly
labeled as projections. The hosted Foundry agent gets a corresponding
user-facing framing rule so it does not synthesize the missing numbers itself.
The `StatboticsTool.DescribeApiSurfaceAsync` `guidance` string is the
single source of truth for the per-field classification (R6).

## Problem Frame

When a user asks "what's our rank at champs?" three weeks before champs, the
local agent fetches `/v3/team_events?event=2026cmptx&team=2046`, sees
`record.qual.rank = null` and `record.qual.{wins,losses,ties} = 0`, and
either reports `"0-0, unranked"` or — worse — interprets the zeros as a real
record and inflates them with confident prose. The same failure mode applies to
match scores for unplayed matches, alliance assignments before alliance
selection, and `epa.stats.{pre_elim, mean, max}` aggregates that only fill in
once matches have actually played.

Empirical confirmation against `/v3/team_events?event=2026arc` (status =
Upcoming, fetched while planning):

- Pre-event projections **populated** (⚠️ valid but must be labeled): `epa.total_points.mean`
  (332.16), `epa.total_points.sd`, `epa.unitless`, `epa.norm`, `epa.conf`,
  `epa.breakdown.*` (all components incl. game-specific), `epa.stats.start`
  (338.57 — distinct from `total_points.mean`, indicating per-event projection
  vs incoming-EPA snapshot), `record.qual.num_teams` (75 — registered team
  count).
- Completed-only fields **zero or null** (❌ must not be reported as facts):
  `epa.stats.{pre_elim, mean, max}` = 0.0, all `record.qual.*` counts = 0,
  `record.qual.rank` = null, all `record.elim.*` zero/null, `record.total.*`
  = 0, `district_points` = null.

See origin: `docs/brainstorms/2026-04-27-statbotics-future-event-metric-validity-requirements.md`.

## Requirements Trace

- R1. The local agent must use `event.status` (or the equivalent `*.status` /
  `*.event_status` sibling field) as the gate for whether a Statbotics row is
  "completed enough" to quote completed-only metrics from
  (see origin §Requirements R1)
- R2. Refuse-and-redirect on completed-only metrics for `Upcoming` events,
  rather than reporting zeros or inventing prose; suggest the named substitute
  fields (e.g. EPA projection, registered team count) when possible
  (see origin §Decisions, §Requirements R2/R4)
- R3. When quoting pre-event projection fields (`team_event.epa.total_points.*`,
  `epa.unitless`, `epa.norm`, `epa.conf`, `epa.breakdown.*`, `epa.stats.start`),
  frame them explicitly as Statbotics' **projected** EPA for the team at this
  event, not as observed performance and not as the team's current season EPA
  snapshot (see origin §Requirements R3, refined by empirical finding that
  `total_points.mean` ≠ `stats.start`)
- R4. The agent must name acceptable substitute fields when refusing
  (registered team count is valid; rank/record/district points are not)
  (see origin §Requirements R4)
- R5. The behavior is scoped to event-shaped resources (`/v3/events`,
  `/v3/team_events`, `/v3/matches`, `/v3/team_matches`). `TeamYear` and `Year`
  resources are explicitly out of scope for this plan (see origin §Decisions)
- R6. The completed-only field list is encoded in the
  `StatboticsTool.DescribeApiSurfaceAsync` `guidance` string as the single
  source of truth, so the local agent sees it on every tool-discovery turn
  (see origin §Requirements R7)
- R7. `agent_prompt.txt` and `foundry-agent.yaml` MUST be updated in lockstep
  (see origin §Requirements R8). The hosted agent does not call `statbotics_api` directly — it routes via
  `query_local` — so its rule is a **user-facing framing** rule, not a
  field-classification rule
- R8. No tool-side enforcement, no schema gating, no response mutation — this
  is a prompt-only fix (see origin §Decisions)

## Scope Boundaries

- **In scope**: prompt edits to `StatboticsTool.cs` `guidance` string,
  `local_agent_prompt.txt`, `agent_prompt.txt`, `foundry-agent.yaml`
- **Out of scope**:
  - Tool-side enforcement (response filtering, schema gating, response mutation)
  - `TeamYear` / `Year` resources
  - Ongoing-event mid-quals partial-zero failure (e.g. `record.elim.*` is still
    zero during qualification rounds of an in-progress event). The dominant
    failure mode is `Upcoming`. Mitigation in this plan is the broader "treat
    0/null with skepticism" framing, not a phase-by-phase rule. Revisit if
    user-reported.
  - Year-level rollups, awards, alliance selection results, OPR/DPR/CCWM
  - Automated CI lockstep enforcement between `agent_prompt.txt` and
    `foundry-agent.yaml` (manual maintainer process per file header comment)
  - Tests for prompt content (no existing prompt-content test infrastructure)

## Context & Research

### Relevant Code and Patterns

- `services/ChatBot/Tools/StatboticsTool.cs` — `DescribeApiSurfaceAsync` (lines
  43–75) returns a JSON envelope `{baseUrl, guidance, endpointCount, endpoints}`.
  The `guidance` field is a single multi-line string literal at line 71. This
  is the SSOT location per R6/R7.
- `services/ChatBot/local_agent_prompt.txt` — already has a `STATBOTICS PATH RULE`
  block (line 68) and `STATBOTICS EFFICIENT QUERY PATTERNS` block (line 76).
  These are the natural anchors for a new `STATBOTICS FUTURE-EVENT RULE` block.
- `services/ChatBot/agent_prompt.txt` — has `STATBOTICS PATH RULE` (line 174)
  and `STATBOTICS EFFICIENCY RULE` (line 179). Same anchor pattern.
- `services/ChatBot/Agents/foundry-agent.yaml` — `definition.instructions:` block
  (line 22, `instructions: |-`). Header comment (lines 14–15) confirms manual
  maintainer sync to Foundry portal. **Note:** the YAML has *less* Statbotics
  content than `agent_prompt.txt` today — only the `query_local` routing
  guidance at line 107 mentions Statbotics. The hosted agent does not call
  `statbotics_api` directly. So its update is a small user-facing framing
  addition, not a copy of the local-agent rule.

### Institutional Learnings

- `docs/solutions/integration-issues/statbotics-openapi-enum-must-be-surfaced-to-agent-2026-04-26.md`
  — sister solution: enum values must reach the agent via the discovery surface,
  not just the OpenAPI spec. Same SSOT pattern: discovery-surface guidance is
  high-leverage because every Statbotics tool turn is preceded by a discovery
  call. This plan follows the same pattern (rule lives in `guidance`, not
  buried in OpenAPI).

### External References

- `avgupta456/statbotics` ref `98c3868`, files
  `backend/src/db/models/{event,team_event,team_match,match,team_year,year}.py`.
  Authoritative serialization shape via `to_dict()`. All zero-default fields
  (`Float, default=0`, `Integer, default=0`) are populated with literal zeros
  pre-event, not omitted from the response. Confirmed by live API call to
  `/v3/team_events?event=2026arc` while planning.

## Key Technical Decisions

- **SSOT for field classification: `StatboticsTool.cs` `guidance` string.**
  Rationale: `statbotics_api_surface` is mandated to be called before every
  `statbotics_api` call (per existing prompts), so every local-agent turn that
  touches Statbotics sees the rule. Putting the field list in the prompt files
  too would create drift risk. The prompts reference the rule conceptually; the
  guidance string carries the per-field detail.
- **Refuse-and-redirect, not caveat-and-report.** Origin decision. Reporting
  "0-0, unranked, ⚠️ event hasn't happened yet" is still misleading because
  users skim past caveats. Pure refusal with named substitutes is clearer.
- **Hosted agent gets framing rule only, not field taxonomy.** The Foundry
  hosted agent never calls `statbotics_api` — it routes to the local agent via
  `query_local`. So its update is "if the local agent reports 'event hasn't
  happened', communicate that warmly; never invent numbers; never override the
  refusal with a different framing." The local agent owns the field-level
  policy.
- **R3 framing language: "projected EPA at this event", not "team's current
  season EPA snapshot".** Refined from origin during planning. Empirical data
  shows `team_event.epa.total_points.mean` (332.16) ≠ `team_event.epa.stats.start`
  (338.57) on Upcoming events for the same row. The two fields carry different
  signals: `stats.start` ≈ team's incoming/season EPA snapshot;
  `total_points.mean` ≈ Statbotics' per-event projection (likely
  schedule-/divisional-pool-adjusted). The prompt should distinguish them.
- **Guidance string size budget: ≤2KB added.** Terse bullet format grouped by
  resource. The full envelope is fetched on every Statbotics turn and lives in
  the model's context; bloat directly costs every chat turn.
- **List-endpoint behavior: per-row partition.** The status is per-row
  (`team_event.event_status`, etc.), not a property of the request. The agent
  must treat `Upcoming` rows in a mixed-status response as completed-only-
  invalid even when the query also returned `Completed` rows. Stated explicitly
  in the prompt.

## Open Questions

### Resolved During Planning

- *Are pre-event `team_event.epa.*` fields actually populated, or zero?* —
  Resolved by live API call to `/v3/team_events?event=2026arc`: pre-event
  projections (`total_points.{mean,sd}`, `unitless`, `norm`, `conf`,
  `breakdown.*`, `stats.start`) are populated with meaningful, non-zero values;
  `stats.{pre_elim, mean, max}` are zero. Adversarial reviewer's empirical
  challenge resolved in favor of the original ⚠️ classification.
- *Is `team_event.epa` the team's current season EPA snapshot?* — Resolved
  empirically: `total_points.mean` and `stats.start` differ on the same
  Upcoming row, indicating the EPA is per-event-projected, not a pure copy of
  TeamYear EPA. R3 framing updated accordingly.
- *Should hosted agent get the full field taxonomy?* — No. It doesn't call
  `statbotics_api`. It only needs the user-facing framing rule.
- *Size budget for the augmented guidance string?* — ≤2KB; terse bullets
  grouped by resource.
- *List-endpoint behavior with mixed-status results?* — Per-row partition.
  Stated in the prompt.

### Deferred to Implementation

- *Exact phrasing of the local-agent refusal template.* — Implementer should
  write a 1–2 sentence template that names the missing data, the reason
  (event hasn't happened), and an acceptable substitute when one exists. Match
  the existing prompt's terse, recovery-hint-style tone.
- *Whether `record.qual.num_teams` belongs in the ⚠️ list or its own ✅ tier.* —
  Implementer's judgment. Empirical data shows it's populated pre-event
  (registered count, valid signal). Treating it as ✅-always-valid is fine if
  the prompt makes clear it's the *registered* team count, not the team count
  that ended up playing.
- *Status field name variation across resources.* — `event.status`,
  `team_event.event_status`, `match.status`, `team_match.status`. Implementer
  should encode the per-resource status field name in the guidance string so
  the local agent doesn't have to guess.
- *Ongoing-event partial-zero handling (mid-quals `record.elim.*` = 0).* —
  Out of scope for this plan; broader "treat 0/null on event-shaped rows with
  skepticism, prefer EPA projections when scores aren't available" framing
  should mitigate the worst cases. Revisit if user-reported.

## High-Level Technical Design

> *This illustrates the intended approach and is directional guidance for review,
> not implementation specification. The implementing agent should treat it as
> context, not code to reproduce.*

```text
┌─────────────────────────────────────────────────────────────────────┐
│ StatboticsTool.cs ── DescribeApiSurfaceAsync.guidance (SSOT)        │
│   "When event/team_event/match/team_match has status=Upcoming:      │
│    ❌ DO NOT report from these fields (zero/null is not a real      │
│       value): record.*, district_points, week, epa.stats.{pre_elim, │
│       mean, max}, match scores, alliance assignments, ...            │
│    ⚠️  These are pre-event PROJECTIONS — quote with that framing:    │
│       team_event.epa.total_points.{mean,sd}, epa.unitless,          │
│       epa.norm, epa.conf, epa.breakdown.*, epa.stats.start          │
│    ✅ These are valid even pre-event:                                │
│       record.qual.num_teams (registered count), event metadata"     │
└─────────────────────────────────────────────────────────────────────┘
                              │
        ┌─────────────────────┼─────────────────────────┐
        ▼                     ▼                         ▼
┌───────────────────┐  ┌──────────────────┐  ┌─────────────────────┐
│ local_agent_      │  │ agent_prompt.txt │  │ foundry-agent.yaml  │
│ prompt.txt        │  │ (hosted, runtime)│  │ (hosted, manual sync)│
│                   │  │                  │  │                     │
│ STATBOTICS        │  │ Mirror of yaml — │  │ User-facing framing │
│ FUTURE-EVENT RULE │  │ user-facing      │  │ rule only — never   │
│ (calls API,       │  │ framing rule for │  │ invent numbers when │
│ enforces refuse-  │  │ hosted agent     │  │ local agent reports │
│ and-redirect on   │  │                  │  │ "event hasn't       │
│ Upcoming rows)    │  │                  │  │ happened yet"       │
└───────────────────┘  └──────────────────┘  └─────────────────────┘
```

## Implementation Units

- [ ] **Unit 1: Encode the field-validity classification in `StatboticsTool.cs` `guidance` (SSOT)**

**Goal:** Add a terse, ≤2KB future-event field-classification block to the
`guidance` string returned by `DescribeApiSurfaceAsync`. This is the single
source of truth that drives every local-agent Statbotics turn.

**Requirements:** R1, R3, R4, R5, R6

**Dependencies:** none

**Files:**
- Modify: `services/ChatBot/Tools/StatboticsTool.cs` (the `guidance` literal at
  line 71 in `DescribeApiSurfaceAsync`)

**Approach:**
- Append (do not replace) a new section to the existing guidance string,
  conventionally formatted to match the existing tone (terse bullets, all caps
  section header).
- Group by resource: `event.*`, `team_event.*`, `match.*`, `team_match.*`. For
  each resource, list the gating status field name and three classification
  tiers: ❌ completed-only (do not report as facts when status=Upcoming),
  ⚠️ pre-event projection (quote with explicit "projected" framing),
  ✅ always valid.
- Encode the empirical findings: `epa.stats.start` distinct from
  `epa.total_points.mean`; `record.qual.num_teams` is the registered count and
  always valid; per-row partition for mixed-status list responses.
- Keep the ≤2KB ceiling. If approaching the limit, prefer abbreviating
  field-list groups (e.g. `record.{qual,elim,total}.*`) over dropping
  guidance.

**Patterns to follow:**
- Existing guidance string structure in the same method
- Terse section-header style used in `local_agent_prompt.txt`
  (`STATBOTICS PATH RULE`, `STATBOTICS EFFICIENT QUERY PATTERNS`)
- Sister solution at `docs/solutions/integration-issues/statbotics-openapi-enum-must-be-surfaced-to-agent-2026-04-26.md`
  for the "discovery surface as SSOT" pattern

**Test scenarios:**
- Test expectation: none — pure prompt/string content change in a tool that
  is not exercised by automated tests today. Manual verification via running
  the bot and asking a future-event question is the verification path
  (see Verification below).

**Verification:**
- The string compiles (project builds clean)
- Total `guidance` payload is ≤2KB larger than baseline
- A manual smoke test ("what's Team 27's rank at Archimedes 2026?") shows the
  local agent refuses with a substitute (registered count or pre-event EPA
  projection) instead of reporting `0-0, unranked`

---

- [ ] **Unit 2: Add `STATBOTICS FUTURE-EVENT RULE` to `local_agent_prompt.txt`**

**Goal:** Teach the local agent (the one that actually calls `statbotics_api`)
to apply the field-validity classification: refuse-and-redirect on completed-
only fields for `Upcoming` rows, frame ⚠️ projection fields as projections,
and partition list responses per-row.

**Requirements:** R1, R2, R3, R4, R5

**Dependencies:** Unit 1 (the guidance string is referenced by-name from the
prompt)

**Files:**
- Modify: `services/ChatBot/local_agent_prompt.txt` (add a new section after
  `STATBOTICS EFFICIENT QUERY PATTERNS` ending around line 86, before
  `FRC GLOSSARY` at line 89)

**Approach:**
- Add a `STATBOTICS FUTURE-EVENT RULE` section. Keep it terse (≤15 lines,
  matching the surrounding sections' density).
- State the refuse-and-redirect behavior: when a row's status is `Upcoming`,
  do NOT report `record.*`, `district_points`, `week`, match scores, alliance
  assignments, `epa.stats.{pre_elim, mean, max}` — these are zero/null because
  the event hasn't happened, not because the team is bad.
- State that pre-event EPA fields (`epa.total_points.*`, `epa.unitless`,
  `epa.norm`, `epa.conf`, `epa.breakdown.*`, `epa.stats.start`) MAY be quoted
  but MUST be framed as Statbotics' **projection** for the team at the event
  (not as observed performance, not as the team's current season EPA snapshot).
- State the per-row partition rule for list endpoints.
- Reference the discovery surface as the authoritative field list:
  "see `statbotics_api_surface` guidance for the full per-resource
  classification."
- Mention acceptable substitutes (registered team count, EPA projection) so
  the agent has somewhere to redirect.

**Patterns to follow:**
- Existing section style in the same file (`STATBOTICS PATH RULE` line 68,
  `STATBOTICS EFFICIENT QUERY PATTERNS` line 76)
- Recovery-hint phrasing tone elsewhere in the prompt

**Test scenarios:**
- Test expectation: none — prose prompt change with no automated test
  infrastructure for prompt content. Manual smoke test is the verification.

**Verification:**
- Manual smoke test: ask the bot "what's our qualification record at
  Archimedes 2026?" before April 29 → bot responds explaining the event
  hasn't happened, optionally offering Statbotics' EPA projection or
  registered team count, instead of reporting `0-0`.
- Manual smoke test: ask "what's Team 254's projected EPA at Archimedes 2026?"
  → bot responds with the projection clearly labeled as a pre-event
  projection, not as observed performance.

---

- [ ] **Unit 3: Add user-facing framing rule to `agent_prompt.txt` AND `foundry-agent.yaml` in lockstep**

**Goal:** Ensure the hosted agent (which does not call `statbotics_api` directly)
does not paper over the local agent's refusal with invented numbers, and
communicates the refusal warmly to the user.

**Requirements:** R7

**Dependencies:** Unit 2 (the hosted-agent rule references the local-agent
behavior contract)

**Files:**
- Modify: `services/ChatBot/agent_prompt.txt` (add to the existing
  `STATBOTICS PATH RULE` neighborhood near line 174, OR add a small new
  paragraph in the `query_local` framing section)
- Modify: `services/ChatBot/Agents/foundry-agent.yaml` (mirror the same
  paragraph in `definition.instructions`, near the existing Statbotics
  `query_local` guidance at line 107)

**Approach:**
- Add a short paragraph (≤6 lines): "When the local agent reports that a
  Statbotics field isn't available because an event hasn't happened yet,
  communicate that to the user warmly and offer the substitute the local
  agent named. Never invent a record, rank, district point total, or match
  score for a future event. If the local agent provides a Statbotics EPA
  projection, frame it as a *projection*, not as observed performance."
- The two files MUST receive identical paragraphs (modulo YAML indentation).
  Do both edits in the same commit so they cannot drift in review.
- Do NOT copy the local-agent's full field-classification taxonomy here. The
  hosted agent never sees the raw API response — it only sees the local
  agent's already-filtered narrative.

**Patterns to follow:**
- Existing terse paragraph style in both files
- The R8 lockstep convention (file header on `foundry-agent.yaml` line 14
  documents the manual sync expectation)

**Test scenarios:**
- Test expectation: none — prose prompt change with no automated test
  infrastructure.

**Verification:**
- Both files contain the new paragraph with identical wording (modulo YAML
  indentation and string-literal escaping)
- Manual smoke test: a future-event question routed through the hosted agent
  produces a warm refusal that surfaces the local agent's substitute (e.g.
  EPA projection), not a fabricated record/rank
- Maintainer checklist note: after merge, manually copy `definition` from
  `foundry-agent.yaml` into the Foundry portal per the existing sync workflow

## System-Wide Impact

- **Interaction graph:** `local_agent_prompt.txt` governs the in-process agent
  that actually calls `statbotics_api`. The hosted agent (`agent_prompt.txt` /
  `foundry-agent.yaml`) routes via `query_local` and never calls the Statbotics
  tool directly, so the field taxonomy lives only on the local side. The
  hosted-agent change is a user-facing framing rule.
- **Error propagation:** No new error paths. The local agent's "refuse-and-
  redirect" surfaces as a normal text response, not as a tool error.
- **API surface parity:** Sister concern is the enum-validation work tracked in
  `docs/brainstorms/2026-04-26-statbotics-query-validation-requirements.md` —
  same tool surface (`StatboticsTool.cs` `guidance`), different concern. The
  `guidance` block must accommodate both additions cumulatively. Coordinate
  ordering at merge time if both land near each other.
- **Integration coverage:** No automated test infrastructure for prompt
  content exists. Manual smoke tests against `2026arc` (Upcoming until April 29,
  2026) and `2026cmptx` (Champs, Upcoming) are the verification path.
  Post-April-29 test event: any 2026 event still in `Upcoming` status from
  `/v3/events?year=2026`.
- **Unchanged invariants:** `StatboticsTool.QueryStatboticsAsync` signature
  and behavior, OpenAPI spec, the `StatboticsTool.cs` JSON envelope shape
  (`{baseUrl, guidance, endpointCount, endpoints}`), the existing
  `STATBOTICS PATH RULE` and `STATBOTICS EFFICIENT QUERY PATTERNS` blocks,
  TBA/meal-signup tool behavior, and `TeamYear`/`Year` Statbotics handling
  all remain unchanged.

## Risks & Dependencies

| Risk | Mitigation |
|------|------------|
| `agent_prompt.txt` and `foundry-agent.yaml` drift over time (no CI lockstep) | Both files edited in the same commit. R8 already documents the lockstep expectation. CI enforcement explicitly out of scope. |
| Cross-resource status field name confusion (`event.status` vs `team_event.event_status` vs `match.status`) leads to local agent applying wrong gate | Encode per-resource status field name in `guidance` (Unit 1 deferred-to-impl note). Implementer verifies field names against the upstream `to_dict()` definitions. |
| Refusal feels brusque to users who just want a rough answer | Refusal template names a substitute (EPA projection, registered count). Hosted-agent framing rule keeps the tone warm. |
| Empirical findings change between planning and implementation if Statbotics changes their pre-event field population | Low likelihood — Statbotics has been stable for years. Mitigation: implementer re-runs the planning-time sample query against an Upcoming event before finalizing the guidance string. |
| `record.qual.num_teams` later turns out NOT to be populated for some Upcoming events (we tested only `2026arc`) | Implementer's deferred-to-impl note: verify against ≥2 Upcoming events before classifying as ✅-always-valid. If inconsistent, downgrade to ⚠️ "may be populated; verify before quoting." |
| Ongoing-event mid-quals partial-zero failure (`record.elim.*` = 0 during qualification rounds) is not addressed | Out of scope; broader "treat 0/null with skepticism on event-shaped rows" framing in Unit 2 partially mitigates. Revisit if user-reported. |

## Documentation / Operational Notes

- After merge, the maintainer must manually copy `definition` from
  `foundry-agent.yaml` into the Foundry portal (existing sync workflow per
  `services/ChatBot/Agents/foundry-agent.yaml` line 14 header comment). The
  hosted agent will not pick up the rule until that sync runs.
- No new env vars, no migration, no rollout flag — the change takes effect on
  next deploy of the local agent and on next Foundry portal sync for the
  hosted agent.
- After deploy, post-validate by issuing a future-event question to the bot
  in a non-prod channel.

## Sources & References

- **Origin document:** [docs/brainstorms/2026-04-27-statbotics-future-event-metric-validity-requirements.md](../brainstorms/2026-04-27-statbotics-future-event-metric-validity-requirements.md)
- Sister brainstorm (coordinate at merge): [docs/brainstorms/2026-04-26-statbotics-query-validation-requirements.md](../brainstorms/2026-04-26-statbotics-query-validation-requirements.md)
- Sister solution (same SSOT pattern): [docs/solutions/integration-issues/statbotics-openapi-enum-must-be-surfaced-to-agent-2026-04-26.md](../solutions/integration-issues/statbotics-openapi-enum-must-be-surfaced-to-agent-2026-04-26.md)
- Code: `services/ChatBot/Tools/StatboticsTool.cs` (line 71 `guidance` literal),
  `services/ChatBot/local_agent_prompt.txt` (line 76 STATBOTICS section),
  `services/ChatBot/agent_prompt.txt` (line 174 STATBOTICS section),
  `services/ChatBot/Agents/foundry-agent.yaml` (line 22 `instructions:` block,
  line 107 Statbotics routing guidance)
- Upstream Statbotics ORM: `avgupta456/statbotics` ref `98c3868`,
  `backend/src/db/models/{event,team_event,team_match,match,team_year,year}.py`
- Empirical sample: `GET https://api.statbotics.io/v3/team_events?event=2026arc&limit=5`
  (2026 Archimedes Division, status=Upcoming, queried during planning)
