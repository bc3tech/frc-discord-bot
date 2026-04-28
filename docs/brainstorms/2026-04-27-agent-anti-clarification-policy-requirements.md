---
date: 2026-04-27
topic: agent-anti-clarification-policy
---

# Agent Anti-Clarification Policy

## Problem Frame

The Bear Metal Bot is taking 5+ minutes per turn to ask clarifying questions that its existing framing should have answered, then taking another 5+ minutes after the user replies to ask *another* clarifying question — sometimes prefixed with action language like *"Thanks — I'll pull X. One quick check..."* that promises work the bot is not actually starting.

Two observed instances:

1. **"Top 10 teams in our division at worlds"** (Worlds 2026 = Upcoming) → bot asked *"by Statbotics' projected EPA or by TBA / qualification rankings?"* TBA quals are ❌ in the field-validity matrix we just landed for Upcoming events. There was only one valid answer; the bot asked anyway.
2. **User answers "epa"** → bot showed a `query_local` "Fetching Statbotics EPA" progress card, then emitted *"Thanks — I'll pull Statbotics' projected EPA for Newton Division and give you the top 10 by epa.total_points.mean. One quick check: team numbers only or numbers + names?"* Three failures in one message: action-language promise, leaked internal field path (`epa.total_points.mean`), and a blocking cosmetic question after the data was already retrieved.

The recent prompt-SSOT-with-lockstep work made the agent **more careful** but also **more deferential** — it sees framing nuance and asks the user instead of using the framing to pre-empt the question.

**Latency math.** `MaxLocalAgentHandoffs=6`, `MaxWorkflowSteps=24`, `MaxAnswerEvaluationRetries=1`, plus `WorkflowSoftTimeoutSeconds=15` / `WorkflowHardTimeoutSeconds=20` per step (`services/ChatBot/Configuration/AiOptions.cs:48-70`). A 5-minute turn corresponds to roughly 3-4 hosted↔local cycles plus the evaluator. Each avoided clarification round eliminates one full cycle plus the user think-time gap (and any dependent retries). Eliminating *one* clarification round per question is expected to materially reduce perceived latency without any performance work, though the magnitude depends on round mix (clarification rounds are typically short on bot-side compute but long on user think-time, while data-fetch rounds are the reverse) and per-round duration is not yet measured.

The deeper pattern: the agent treats clarifying questions as the *safe* default. We need to flip the default to *act-with-named-assumptions*, reserving `ask_user` for irreducible semantic ambiguity.

### Current vs Proposed Behavior

| Scenario | Current behavior | Proposed behavior |
|---|---|---|
| Worlds (Upcoming) "top 10 in our division" | `ask_user`: "EPA or TBA quals?" → wait → fetch | `final`: top-10 by EPA, named assumption inline |
| User answers "epa" after a forced first ask | `query_local` → data in hand → `ask_user`: "numbers + names?" | `final`: numbers + names (default) with one-line opt-out |
| User says "stats for Brandon" with multiple Brandons | `ask_user`: "which Brandon?" | `ask_user`: same — irreducible ambiguity, legitimate |
| User asks future-match score | (current) zero/null reported, or `ask_user`: "did you mean projection?" | `final`: REFUSE-AND-REDIRECT to projection (already covered by Statbotics SSOT) |
| Cosmetic preference (table vs list, short vs long) | `ask_user` | `final` with default + opt-out footer |

## Requirements

**Use known constraints to pre-empt clarifications**

- R1. When a tool's discovery guidance (e.g., the Statbotics field-validity matrix) narrows the answer to a single valid metric for the user's question, the agent must use that metric directly and name the assumption inline ("since worlds hasn't started, this is by Statbotics' projected EPA — say if you'd rather wait for qual rankings"). It must not ask the user to choose between metrics when one of the offered choices is invalid for the row state.
- R2. The agent must not pose a question whose options include any path the agent already knows is invalid (Upcoming/Ongoing → ❌ fields, no `event_key` yet → match-key endpoints, etc.). If filtering invalid options leaves exactly one path, take it.
- R12. **Question-must-reduce-work test.** Before emitting `ask_user`, the agent must verify that *at least one possible answer would change the work it does next*. If the bot has to perform the same lookup (e.g., resolve "our division at worlds" → Newton) regardless of the user's answer, the question adds zero value and must be skipped.

**Default-and-correct beats ask-and-block for cosmetic choices**

- R3. Cosmetic and formatting preferences (team-numbers-only vs numbers+names, list vs table, short vs long form, language register) must default to a sensible choice with a one-line correction prompt — never a blocking `ask_user`. Example default: include team numbers AND names when both are cheaply available; if the user wants only numbers they can say so.
- R4. The agent must prefer "act with named assumptions" over "ask first" whenever the worst case of acting wrong is a one-message correction, not data corruption or wasted compute exceeding ~30 seconds. **Carve-out for decision-grade contexts:** when the user's question is plausibly being used for live competition decisions (alliance selection, pit-strategy, "next match" prep) and the wrong default could materially mislead the decision in the seconds-to-minutes window, fall back to R7-style stated-assumption + ask. Heuristic: if an FRC competition event for Team 2046 is currently Ongoing, lean toward stating assumptions explicitly inline and inviting correction in the same `final`, rather than pure silent defaulting.
- R10. **Default-with-opt-out footer pattern.** When defaulting under R3/R4 *for purely cosmetic preferences* (formatting, sort order, count, verbosity), the agent's `final` answer must include a brief one-line opt-out hint at the end (e.g., "(Numbers only if you'd prefer.)"). The footer is a *prompt for next message*, not a clarifying question — the answer to the original question is delivered in the same turn.

  **R10 / R1 coordination.** R1's inline assumption-naming (semantic — which metric, which year, which event) and R10's opt-out footer (cosmetic — format, count, sort) do NOT compose. When R1 fires, the inline clause IS the opt-out and no R10 footer is added. R10 is reserved for cases where there is no semantic assumption to name — only a formatting default. This prevents "double opt-out" boilerplate where every answer ends with two em-dash invitations.

  **Footer voice/persistence.** Footer wording must match the upbeat-Bear-Metal voice (`foundry-agent.yaml:31-33`): warm, short, parenthetical, no em-dash filler. Do not repeat the same footer in consecutive turns of the same conversation thread — once the user has been offered an opt-out for a given default, suppress it for follow-up messages on the same topic.

**Eliminate the "promise + ask" anti-pattern**

- R5. The agent must never combine action language ("I'll pull X", "Coming right up", "Let me grab Y", "Thanks — I'll…") with a clarifying question in the same user-facing message. Either act, or ask — not both. The existing `messageToUser` rules at `services/ChatBot/Agents/foundry-agent.yaml:186-196` ban this language for `query_local` and `final` but are silent on `ask_user`; that gap is the structural cause of the screenshot-2 failure.
- R6. The `ask_user` payload's `messageToUser` field (allowed alongside `question` in the foundry-agent JSON schema, `services/ChatBot/Agents/foundry-agent.yaml:154-164`) must forbid **action-language** (verbs that promise or describe work the bot is about to do) when `next_step=ask_user`. Allowed: stated assumptions per R7 ("Defaulting to numbers + names; say if you'd prefer numbers only.") and short context-setting ("Quick clarification:"). Forbidden: anything beginning with "I'll", "Let me", "Thanks — I'll", "Coming right up", "Pulling…", "Fetching…", or naming a tool the bot is about to call. R7's stated-assumptions content is exactly what `messageToUser` is for on an `ask_user` turn — assumption statements are not action-language because they describe a *default already taken*, not future work.

  Worked-example pairs (good vs bad `ask_user` payloads):

  | ❌ Bad `messageToUser` | ✅ Good `messageToUser` | `question` |
  |---|---|---|
  | "Thanks — I'll pull EPA in a sec." | (null) | "Did you mean Brandon Hurlburt or Brandon Smith?" |
  | "Pulling Newton Division now. One quick check:" | "Defaulting to top 10 by Statbotics projected EPA — that's the only valid metric for an Upcoming event." | "Want me to include team names alongside numbers, or numbers only?" |
  | "Let me get the schedule. Quick clarification:" | "Going with the 2026 season unless noted." | "Did you mean the District Championship or Regional?" |

- R9. **No `ask_user` after a successful `query_local` in the same workflow turn**, *unless* the data returned is genuinely ambiguous (multiple equally-valid candidate rows for the user's referent — e.g., two teams with the same number across leagues), empty in a way that requires a user choice (not just a fallback the agent should auto-try per R1/R2), or contains a multi-entity disambiguation the agent cannot resolve from context. Cosmetic preferences (formatting, sort order, count) NEVER qualify under this carve-out — those route to defaults per R3/R4. The rule's intent is to catch the screenshot-2 failure mode (data in hand → cosmetic ask), not to silence legitimate post-fetch disambiguation.
- R11. **No internal field paths, schema names, JSON keys, endpoint slugs, or tool-name fragments in any user-facing string** — including `ask_user.question`, `ask_user.messageToUser`, `final.answer`, and `final.messageToUser`. Today's rule covers `query_local` and `final` (yaml `:186-196`); extend the ban to all four user-facing slots. Examples banned: `epa.total_points.mean`, `team_event.event_status`, `tba_api`, `/v3/team_events`. (Implementation: prompt rule + examples in all three lockstep prompt files; orchestrator post-process is rejected per Scope Boundaries.)
- R13. **Stated assumptions must be derived from actual tool-call parameters, not free-form generation.** When R1, R7, or R10 produce an inline assumption string ("by Statbotics' projected EPA", "for the 2026 season", "for Newton Division"), the values cited (metric name, year, division) must come from the same input that drove the local-agent call or the resolved tool response — not from the model's free-form narration. Without this, an inline-assumption pattern can paradoxically *increase* user trust in incorrect output (label says "EPA" while the underlying call fetched quals due to a routing bug). For Statbotics, the metric name is determined by the field-validity matrix in `Tools/StatboticsTool.cs` guidance.

**Bundle clarifications, never iterate**

- R7. When the agent does need to ask a real clarifying question, it must ask exactly one focused question and convert any other open-but-defaultable items into stated assumptions inside the same `messageToUser`. It must not chain multiple clarification turns for one user request.

  **Verb policy for stated assumptions.** Use present-tense default verbs: "Defaulting to:", "Going with:", "Using:", "Assuming:". Do NOT use future-tense action verbs ("I'll go with", "I'll use", "Let me default to") — those read as action-language and violate R5/R6.

  **Structural template.** When `messageToUser` carries stated assumptions, format them as a labeled block separate from the question:
  ```
  Defaulting to: numbers + names; current 2026 season.
  ```
  …with the actual clarifying question in the `question` field. Never bundle assumptions and the question into one run-on paragraph.

**Scope: applies to all tool surfaces with row-state validity**

- R8. R1, R2, R7, R9, R11, and R12 apply not only to Statbotics future-event handling but to any tool whose discovery guidance encodes row-state field validity (e.g., TBA event status, MealSignup past/future, future tool integrations). The lockstep markers added in the 2026-04-27 Statbotics work must be extended to reference this anti-clarification policy as a sibling SSOT.

  **v1 scope: Statbotics only.** The anti-clarification policy ships v1 against the Statbotics field-validity matrix only (the surface where the screenshot failures originated). Extension to TBA event status and MealSignup past/future filters is v2, gated on v1 metrics holding (per Success Criteria) and an explicit inventory of row-state validity in those tools' discovery surfaces. This avoids landing a multi-tool policy change against an unenumerated surface.

## Approaches Considered

R1-R5, R7, R8, R9-R13 are prompt-only changes (additive rules in the three lockstep prompt files; R8 specifically extends existing lockstep markers and tool-guidance strings). The interesting design choice was **R6 — how to make `messageToUser` action-language go away on `ask_user` turns**. Three options were evaluated:

| Approach | Description | Pros | Cons | Verdict |
|---|---|---|---|---|
| **A. Prompt rule + examples (Selected)** | Add explicit `messageToUser` rules for the `ask_user` branch in `foundry-agent.yaml` (mirror existing `query_local`/`final` rules at `:186-196`) plus the worked-example pairs in R6 | Lowest cost, fits existing pattern, naturally lockstep with the three-file prompt SSOT, and aligned with the Scope Boundary against orchestrator changes | Drift risk: model can ignore rules under load; existing rules already cover similar territory yet failures still occur | **Selected** |
| **B. Schema-level constraint** | Tighten the JSON schema for `ask_user`: split single-object schema (`foundry-agent.yaml:256-307`) into a `oneOf`-discriminated union keyed on `next_step`, with `messageToUser` only present on the `query_local`/`final` branches | Structural — model cannot emit forbidden field at all | **Infeasible without a major schema refactor.** Current schema is `strict: true`, single object, `messageToUser` is in `required` (`:298-306`). A `oneOf` split is a substantial change to a contract used by every workflow code path; risk and surface area exceed the marginal value over Approach A | Rejected |
| **C. Workflow-level post-process** | Strip action-language phrases from `messageToUser` in the orchestrator when `next_step=ask_user` | Catches anything the model emits regardless of prompt or schema | Violates the explicit Scope Boundary "no orchestrator code changes"; pushes policy into code rather than declarative prompts | Rejected |

**Decision: Approach A.** Approach B was the original recommendation but investigation of the actual schema (`foundry-agent.yaml:256-307`) showed the field-removal variant requires moving from a single-object strict schema to a `oneOf` discriminated union — a structural change to the workflow contract that exceeds the brainstorm's scope. Approach A is the durable choice; if drift recurs in v2, escalate to Approach B as a separate brainstorm scoped to the schema refactor.

The same approach selection applies to **R11** (no internal field paths in user-facing strings) — extends the existing `query_local`/`final` prompt rule to all four user-facing slots. Orchestrator-side regex enforcement is rejected per the Scope Boundary against orchestrator changes.

## Success Criteria

**Headline scenario panel.** A fixed set of seven canonical scenarios must produce the expected `next_step` and answer-shape on every release. The panel replaces the "X% reduction in ask_user" headline because aggregate counts can be gamed by question-mix shifts.

| # | User question | Event state | Expected `next_step` | Expected wording shape |
|---|---|---|---|---|
| S1 | "Top 10 in our division at worlds" | Upcoming | `final` | EPA top-10, inline assumption per R1, opt-out footer per R10 |
| S2 | "Top 10 by EPA" (after S1) | Upcoming | `final` | EPA top-10, no follow-up cosmetic question |
| S3 | "What was Team 2046's match score in their last regional?" | Completed | `final` | Score reported, no clarification |
| S4 | "Stats for Brandon" | (any) | `ask_user` | Disambiguation question; no action-language; no `messageToUser` action verbs |
| S5 | "Top 10 at Newton in 2026 quals" | Upcoming | `final` | REFUSE-AND-REDIRECT to projected EPA per Statbotics SSOT; no zero-padding |
| S6 | "Show me a table of the top 5" (after a list-shaped answer) | (any) | `final` | Reformatted as table, no clarification |
| S7 | "Who was on our alliance" with two recent regionals | Completed | `ask_user` | Disambiguation between events; no action-language |

The panel is the contractual definition of "policy holds." Per-class telemetry (cosmetic-default rate, row-state-resolved rate, irreducible-ambiguity-ask rate) is supplementary.

**Per-rule observable criteria.**

- No user-facing message exists that pairs action-language with a clarifying question (R5/R6).
- No internal field paths (`epa.total_points.mean`-style strings) appear in any user-facing message slot (R11).
- No `ask_user` emission ever follows a successful `query_local` within the same workflow turn *outside the R9 carve-out conditions* (R9). Verifiable from session telemetry by sequencing `next_step` values per turn — see Outstanding Question on telemetry shape.
- Stated assumption strings (R13) match the values that drove the underlying tool call in 100% of sampled cases (manual review of a 50-turn random sample post-launch).
- No regression on legitimate ambiguity: S4 and S7 in the panel above MUST continue to produce `ask_user`.

**Goodhart guard.** No statistically meaningful increase in user follow-up corrections (messages of the form "no I meant…", "that's wrong", explicit re-asks within 2 turns of a defaulted answer) over the same measurement window. The panel-pass and per-rule criteria are only valid if this counter-metric holds.

**Latency note.** Median time-to-first-token for questions matching S1/S2 should drop measurably (downstream effect of R1/R2/R12). Not a primary lever; not gated on hitting a specific threshold.

## Scope Boundaries

- **Latency engineering is out of scope.** No model swap, no context-window pruning, no tool-discovery caching, no concurrency changes. Latency improvement is a hoped-for downstream effect of fewer turns, not a primary goal. If turn-count reduction does not meaningfully improve perceived latency, that becomes a separate brainstorm.
- **No code changes to the workflow orchestrator** (`Workflows/`, `MessageHandler.cs`, agent loop). Changes are limited to prompt surfaces and the foundry-agent JSON schema:
  - `services/ChatBot/agent_prompt.txt`
  - `services/ChatBot/local_agent_prompt.txt`
  - `services/ChatBot/Agents/foundry-agent.yaml`
  - `services/ChatBot/Tools/StatboticsTool.cs` guidance string (and equivalent for other discovery tools, per R8)
- **No changes to the Discord button flow** for ask_user (the buttons themselves stay; we're reducing how often the schema is invoked, not how it renders).
- **Not a rewrite of existing rules.** The existing `foundry-agent.yaml` rules at lines 101 and 167-170 already partially express this; new rules are additive and must lockstep with them, not replace them.

## Key Decisions

- **Latency is downstream of clarification count.** Fix clarifications first; revisit latency only if perceived speed doesn't improve enough. Workflow caps (`MaxLocalAgentHandoffs=6`, `MaxWorkflowSteps=24`, plus 15/20s soft/hard step timeouts) bound the observed 5-minute turns; eliminating clarification rounds is expected to materially reduce them, with the exact reduction dependent on per-round duration mix (not yet measured).
- **Defaults beat questions for cosmetic preferences.** Questions are reserved for irreducible semantic ambiguity (which Brandon, which year when both are empty, etc.).
- **R6 lands as Approach A (prompt rule + examples), not the schema-level constraint originally floated.** Investigation of `foundry-agent.yaml:256-307` confirmed the schema is `strict: true` single-object with `messageToUser` in `required`; field removal would require a `oneOf` discriminated union refactor that exceeds the brainstorm's scope. If drift recurs in v2, the schema refactor becomes its own brainstorm.
- **Existing prompt rules already partially express this.** `messageToUser` rules at `foundry-agent.yaml:186-196` ban promise-language for `query_local`/`final`; `agent_prompt.txt:80` bans premature `ask_user` for unresolved competition-data lookups; `foundry-agent.yaml:166-170` already says "If data is already fetched or fetchable, go straight to `final`." The new rules generalize and strengthen these existing patterns rather than introducing a new policy direction.
- **Lockstep across hosted / local / yaml prompts is preserved.** New rules inherit the same lockstep marker treatment from the recent Statbotics prompt-SSOT work, so future edits stay in sync.
- **R9 (no ask after query_local) is the highest-impact single rule.** It catches the worst failure mode (data already retrieved, then user blocked) where the cost has already been paid. The R9 carve-out (genuinely ambiguous data, multi-candidate referent) prevents the rule from forcing confidently-wrong public-channel answers.
- **R8 ships v1 as Statbotics-only.** Broader rollout to TBA event status and MealSignup past/future filters is v2, gated on v1 metrics holding and an explicit inventory of those tools' row-state validity surfaces.

## Dependencies / Assumptions

- The 2026-04-27 Statbotics future-event prompt-SSOT work (commit `c8833cb`) is merged and the StatboticsTool guidance string is the established SSOT pattern. R1, R2, and R8 build directly on it.
- Workflow telemetry currently records `next_step` values per turn (assumed; planning to verify). If not, R-side success measurement degrades to manual review of session logs.

## Outstanding Questions

### Resolve Before Planning

(None — the R6 schema-validation spike was resolved during the deepening pass: the schema is `strict: true` single-object with `messageToUser` in `required` (`foundry-agent.yaml:298-306`), so Approach B requires a `oneOf` schema refactor outside this brainstorm's scope. R6 lands as Approach A. See Approaches Considered and Key Decisions.)

### Deferred to Planning

- [Affects R8 v2][Needs research] When R8 expands to v2, inventory other tool surfaces with row-state validity. Known candidates: `tba_api_surface` (TBA event has status field; `Tools/TbaTool.cs`), `fetch_meal_signup_info` (past/future filter on slot dates). v2-only — not blocking v1.
- [Affects R5/R11][Technical] Confirm where the hosted agent's user-facing wording for `next_step=final` actually lives — `agent_prompt.txt` only, `foundry-agent.yaml` only, or both. The two prompts overlap; planning to confirm the action-language ban and field-path ban land in the right file(s) with proper lockstep markers. Both files are in scope per Scope Boundaries; the question is which carries the canonical rule and which references it.
- [Affects R9 / Success Criteria S-panel][Technical] Can the workflow telemetry distinguish "ask_user after query_local in same turn" from "ask_user as the first emit"? Required for R9 measurement and for the per-class telemetry that backs the scenario panel. If not present, planning needs to add a small log-shape addition (acceptable per Scope Boundaries: telemetry instrumentation is not the same as orchestrator policy code).
- [Affects R13][Technical] Implementation mechanism for "stated assumptions derived from tool-call parameters." Options: (a) prompt rule that requires the model to quote the resolved values back, (b) string-template constants in `Tools/StatboticsTool.cs` guidance that the model must use verbatim. Planning to pick.

## Next Steps

→ `/ce-plan` for structured implementation planning
