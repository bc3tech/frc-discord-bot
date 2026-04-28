---
title: Surface row-state field validity via tool discovery, then lockstep the framing across prompts
date: 2026-04-27
category: best-practices
module: services/ChatBot
problem_type: best_practice
component: assistant
severity: medium
applies_when:
  - An agent consumes an API where field validity depends on row state (event status, lifecycle phase, conditional availability)
  - The same behavior rule must appear verbatim in multiple prompt files (hosted agent, local agent, agent yaml)
  - An LLM should refuse rather than report a 0/null/empty value as if it were data
tags: [statbotics, agent, prompt-ssot, lockstep, refuse-and-redirect, discovery, future-event]
---

# Surface row-state field validity via tool discovery, then lockstep the framing across prompts

## Context

The Statbotics chatbot was confidently reporting **"team 2046 is 0-0, unranked at Archimedes 2026"** for an event that hasn't happened yet. Statbotics returns rows for upcoming events with all numeric fields zeroed — that is missing data, not real performance, and the API itself surfaces a `status` field (`Upcoming` / `Ongoing` / `Completed`) so callers can tell the difference. Our agent had no idea.

Three things made this hard to fix cleanly:

1. **The taxonomy is large.** Per-resource (`event`, `team_event`, `match`, `team_match`), each field falls into one of three buckets: always-valid (✅), pre-event projection (⚠️ — quote with framing), or completed-only (❌ — must refuse). That's a roughly 25-line classification matrix.
2. **Two agents need it.** The hosted Foundry agent does user-facing framing; the local agent does API calls and result gating. Duplicating the matrix in both prompt files guarantees drift.
3. **LLMs find loopholes.** A prompt that says *"don't report 0/null as a real value"* will still get answers like *"the rank is currently 0, undefined"*. The agent rationalizes the caveat as honest disclosure.

The same shape recurs whenever a tool surfaces conditional data (lifecycle phases, optional fields, deprecated paths). This doc captures the pattern that worked.

## Guidance

Combine three patterns:

### 1. Discovery-as-SSOT for the taxonomy

The authoritative field-validity matrix lives **once**, in the tool's surface-description guidance string. The agent reads it on every `*_api_surface` call.

```csharp
// services/ChatBot/Tools/StatboticsTool.cs
guidance = """
    ...
    FUTURE-EVENT FIELD VALIDITY (event/team_event/match/team_match): rows for events that
    have not played return 0 or null for many numeric fields — that is missing data, not
    bad performance. Gate quoting on each row's status field; treat status values
    "Upcoming" and "Ongoing" as not-yet-completed.

    event (status):
      ✅ key, name, year, country, state, district, start_date, end_date, type, week, ...
      ❌ epa.{mean,sd,max,top_8,top_24}, metrics.*, district_points.

    team_event (event_status — note the field name):
      ✅ team, event, year, country, state, district, type, week, num_teams.
      ⚠️ pre-event PROJECTION — quote ONLY with explicit "Statbotics' projected" framing: ...
      ❌ epa.stats.{pre_elim,mean,max}, record.{qual,elim,total}.* (incl. rank, alliance), ...

    Behavior on ❌ fields for Upcoming/Ongoing rows: REFUSE-AND-REDIRECT. Do NOT quote
    the field at all, even with a caveat such as "currently 0" or "shows as null" — those
    values are missing data, not honest disclosure...
    """;
```

The prompt files **delegate** to this. They never re-encode the matrix:

```text
# services/ChatBot/local_agent_prompt.txt
For the full per-resource ✅/⚠️/❌ field classification see the
`statbotics_api_surface` `guidance` (the authoritative list).
```

### 2. Hosted-vs-local layering — split framing from taxonomy

Don't push the technical matrix into the user-facing prompts. Split responsibilities:

- **Local prompt** (technical layer): names exact field paths, performs the gating, calls `REFUSE-AND-REDIRECT`.
- **Hosted prompt** (user-facing layer): names domain outcomes (records, ranks, district points, match scores) without API field paths, and just trusts the local agent's refusal.

```yaml
# services/ChatBot/Agents/foundry-agent.yaml
6. STATBOTICS FUTURE-EVENT FRAMING: For events that have not happened yet, never invent
   records, ranks, district points, match scores, or alliance assignments — those are
   0/null in the data, not real values. If `query_local` reports a field unavailable
   because the event is upcoming, communicate that warmly and offer the substitute it
   provides...
```

The hosted agent never sees `record.qual.rank` or `epa.stats.pre_elim`. It doesn't need to.

### 3. Lockstep markers when N files must move together

When the same rule is paraphrased across multiple files (here: `agent_prompt.txt`, `foundry-agent.yaml`, `local_agent_prompt.txt`), add an explicit lockstep marker in each that names the others by path.

```yaml
# services/ChatBot/Agents/foundry-agent.yaml
# LOCKSTEP: The `STATBOTICS FUTURE-EVENT FRAMING` rule (rule #6 below) must stay
# in sync with the matching paragraph in `services/ChatBot/agent_prompt.txt` and
# the `STATBOTICS FUTURE-EVENT FRAMING` section in `services/ChatBot/local_agent_prompt.txt`.
# Edit all three together; the field-validity SSOT lives in `Tools/StatboticsTool.cs`.
```

```text
# services/ChatBot/local_agent_prompt.txt (footer of the section)
Keep this section in lockstep with the `STATBOTICS FUTURE-EVENT FRAMING`
paragraph in `agent_prompt.txt` and rule #6 in `Agents/foundry-agent.yaml`.
```

Then add a discovery-guidance assertion to a unit test so a refactor that drops the lockstep guidance from the SSOT fails the build:

```csharp
// tests/FunctionApp.Tests/HttpGetToolBaseTests.cs
Assert.Contains("FUTURE-EVENT FIELD VALIDITY", guidance);
Assert.Contains("REFUSE-AND-REDIRECT", guidance);
Assert.Contains("\"Upcoming\" and \"Ongoing\"", guidance);
```

### 4. Close the loophole in REFUSE-AND-REDIRECT phrasing

Generic *"don't report N as a real value"* is rationalizable. The fix:

```diff
- REFUSE-AND-REDIRECT. Explain the event/match has not happened yet and offer
- a substitute when one exists (EPA projection, registered team count). Do NOT
- report 0/null as a real value.
+ REFUSE-AND-REDIRECT. Do NOT quote the field at all, even with a caveat such
+ as "currently 0" or "shows as null" — those values are missing data, not
+ honest disclosure. State plainly the event/match is not completed, then
+ offer a substitute when one fits...
```

Name the loophole phrasings explicitly. The LLM will not invent a new one once the obvious ones are forbidden.

## Why This Matters

- **Without discovery-as-SSOT**, the field matrix lives in 2-3 prompt files. Adding a new field or correcting a classification means editing each file and trusting reviewers to spot drift. They won't.
- **Without hosted-vs-local layering**, the user-facing prompt grows to ~25 lines of API arcana. It bloats the hosted agent's context, leaks implementation detail into responses, and makes "frame this nicely for the user" harder, not easier.
- **Without lockstep markers**, a future maintainer edits one file in isolation and the rules drift silently. The marker won't *prevent* drift but it converts a silent failure into a visible one — anyone reading any of the three files immediately sees the contract.
- **Without closed-loophole phrasing**, the agent will find the loophole. Always.

The cost of the pattern is one classification matrix in a tool guidance string and three short lockstep comments. The benefit is that the next time someone adds a Statbotics field, a status enum value, or a new resource type, there's exactly one place to update.

## When to Apply

- Tool surfaces an API where row-level state determines field validity (event status, lifecycle phase, optional/required transitions).
- The same behavior rule must appear paraphrased across hosted-agent / local-agent / agent-config files.
- The agent should refuse rather than report a falsy value, and the LLM has shown it will rationalize caveated reporting.
- A taxonomy is larger than ~10 lines and would otherwise have to be duplicated.

## Examples

### Before (the bug)

User asks: *"What's team 2046's qualification record at Archimedes 2026?"* (event status = Upcoming)

Agent calls `/v3/team_events?team=2046&event=2026arc`, gets back `record.qual.wins = 0, record.qual.losses = 0, record.qual.rank = 0`, and confidently answers:

> Team 2046 is 0-0 and unranked at Archimedes 2026.

Indistinguishable from "the team played and lost everything."

### After (the pattern applied)

Same call, but the agent's tool-discovery guidance now classifies `record.qual.*` as ❌ for Upcoming rows and mandates REFUSE-AND-REDIRECT with no caveated 0-reporting:

> Archimedes 2026 hasn't happened yet, so there's no qualification record to report. Statbotics' projected EPA for team 2046 at the event is 332.16 — that's a per-event projection, not observed performance.

The hosted agent receives the local agent's refusal and frames it warmly without ever seeing the `record.qual` field name.

## Related

- [`statbotics-openapi-enum-must-be-surfaced-to-agent-2026-04-26.md`](../integration-issues/statbotics-openapi-enum-must-be-surfaced-to-agent-2026-04-26.md) — The discovery-as-SSOT pattern was first established for query-parameter enums (Statbotics returns 500 on bad enum values; the agent must see the enum). This doc generalizes the same pattern to row-level field validity, and adds the multi-prompt lockstep treatment that wasn't needed for the enum case.
- Commits `93f759d` (initial fix) and `c8833cb` (review fixes — status enum correction, lockstep markers, tightened refuse wording, test assertions).
