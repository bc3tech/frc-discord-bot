---
title: "feat: Decompose frc-data local agent into domain-specific agents"
type: feat
status: deferred
date: 2026-04-25
origin: docs/brainstorms/2026-04-25-local-agent-decomposition-requirements.md
recommendation: defer
---

# feat: Decompose frc-data local agent into domain-specific agents

## Overview

Split the single `frc-data` local agent into domain-specific agents so each gets a
focused prompt and scoped tool access. **After research and architectural analysis,
this plan recommends deferral** — the expected benefit is marginal under current
constraints and the routing risks are real. The plan documents the analysis and
includes ready-to-execute implementation units if the decision is overridden.

## Problem Frame

The `frc-data` local agent handles three domains (TBA competition data, Statbotics
metrics, and meal signups) with a single 137-line prompt and 6 tools. The original
brainstorm hypothesized that splitting into domain-specific agents would reduce prompt
dilution and improve answer quality.

Research found:

1. **No evidence of quality degradation** at 137 lines — the prior document review
   reached the same conclusion (see origin: `docs/brainstorms/2026-04-25-local-agent-decomposition-requirements.md`)
2. **TBA + Statbotics are tightly coupled** — cross-domain queries like "EPA at our
   last event" need TBA (resolve event key) + Statbotics (get EPA) in a single agent
   turn. The Foundry agent emits one `query_local` per response and cannot fan out to
   multiple local agents in one turn.
3. **Meal-specific content is only ~6 lines** scattered through the prompt — however,
   a standalone meal agent prompt needs shared boilerplate (preamble, hallucination
   prevention, output style) bringing it to ~25 lines, while the competition prompt
   drops to ~120 lines (losing ~17 lines total: the 6 meal-specific lines plus ~11
   lines of scope/routing text that becomes unnecessary).
4. **SDK inference routing is non-deterministic** — `query_local` cannot target a
   specific agent by name; the SDK picks based on `Description` matching with
   `Infer = true`. Wrong-agent selection becomes an architecture bug, not a prompt bug.

## Requirements Trace

- R1. Each local agent should handle a single well-defined domain
- R2. Tool access should be scoped per agent (each agent's config lists only its domain's tools; tool registrations remain global via `AddTool<>`)
- R3. Cross-domain FRC queries (TBA + Statbotics) must continue working in a single turn
- R4. No regression in answer quality or routing reliability
- R5. Foundry agent prompt needs no structural changes (query_local stays generic)

## Scope Boundaries

- TBA and Statbotics MUST stay in the same agent (R3 constraint)
- The only viable split axis is competition data vs. meal logistics
- The Foundry agent's `query_local` JSON format is unchanged
- No changes to tool implementations (tools stay registered globally)
- No changes to session config composition or ISessionConfigSource chain

## Context & Research

### Relevant Code and Patterns

- `services/ChatBot/DependencyInjectionExtensions.cs:154-173` — current single-agent registration
- `gpt/src/BC3Technologies.DiscordGpt.Copilot/CopilotBuilderAgentSkillToolExtensions.cs:26-39` — `WithLocalAgent()` API, additive via `options.LocalAgents.Add(agent)`
- `gpt/src/BC3Technologies.DiscordGpt.Copilot/DefaultSessionConfigSource.cs:41-44` — copies all agents to `SessionConfig.CustomAgents`
- `services/ChatBot/local_agent_prompt.txt` — current 137-line unified prompt
- `services/ChatBot/Agents/foundry-agent.yaml:68-82` — `query_local` routing rules

### Institutional Learnings

- Builder-scoped DI: `WithLocalAgent` hangs off `CopilotBuilder`, not `DiscordGptBuilder` (see `docs/solutions/best-practices/builder-scoped-di-extensions-for-harness-concerns-2026-04-22.md`)
- Session config composition: multiple `ISessionConfigSource` implementations chain via `TryAddEnumerable` (see `docs/solutions/best-practices/conversation-scoped-copilot-sessions-2026-04-23.md`)

### SDK Multi-Agent Mechanics

- `CustomAgentConfig` properties: `Name`, `DisplayName`, `Description`, `Prompt`, `Tools` (string array), `Infer` (bool)
- Multiple `WithLocalAgent()` calls are additive — `LocalAgents` is `IList<CustomAgentConfig>`
- SDK inference uses `Description` field to match when `Infer = true`
- No explicit agent targeting in `query_local` — SDK owns the routing decision

## Key Technical Decisions

- **Keep TBA + Statbotics together**: Cross-domain queries are first-class, not edge cases. Splitting them would break common resolution paths where TBA resolves an event key that Statbotics then uses. The Foundry agent can only emit one `query_local` per turn.
- **2-agent split (not 3)**: The only viable decomposition is competition (TBA+Statbotics) vs. meal logistics. A 3-way split (TBA / Statbotics / meal) breaks cross-domain queries.
- **Recommendation: DEFER**: The prompt reduction is marginal (~17 lines), and inference routing adds non-deterministic failure modes without measured quality problems to solve.

## Deferral Triggers

Revisit this plan when any of these become true:

1. **Measured wrong-tool calls** — telemetry shows the meal tool being called for competition queries or vice versa
2. **Measured answer degradation** — evals show quality drops on meal queries specifically due to competition context pollution
3. **SDK adds explicit agent targeting** — `query_local` can name a specific agent, making routing deterministic
4. **Foundry agent supports multi-hop local** — hosted agent can invoke multiple local agents in one turn
5. **Local prompt exceeds ~300 lines** — prompt length becomes a concrete constraint rather than theoretical concern
6. **Time-based revisit** — revisit in 6 months regardless of data, to prevent indefinite deferral

> **Note:** Triggers 1-2 require telemetry instrumentation that does not currently exist.
> The "More Impactful Alternatives" section below recommends building that telemetry as
> a prerequisite. Without it, these triggers are unobservable and the deferral becomes
> self-reinforcing rather than evidence-responsive.

## More Impactful Alternatives

Instead of decomposing agents, consider these higher-ROI investments:

1. **Telemetry instrumentation**: Log which tools are called per query, wrong-tool rates, no-data rates, and routing-miss cases. This provides the data needed to justify future decomposition.
2. **Stronger tool descriptions**: Improve tool-level routing within the single agent by making tool descriptions more precise about their domain.
3. **Evals**: Build a test suite of representative queries (competition, meal, cross-domain) and measure answer quality as a baseline for future comparison.

---

## Implementation Units (if deferral is overridden)

The following units are ready to execute if the decision is made to proceed despite the recommendation.

- [ ] **Unit 1: Create domain-specific prompt files**

**Goal:** Split `local_agent_prompt.txt` into two focused prompt files.

**Requirements:** R1, R2

**Dependencies:** None

**Files:**
- Create: `services/ChatBot/frc_competition_agent_prompt.txt`
- Create: `services/ChatBot/meal_agent_prompt.txt`
- Modify: `services/ChatBot/ChatBot.csproj` — add `<None Update>` entries for both new prompt files with `CopyToOutputDirectory=PreserveNewest` and `TargetPath=ChatBot\{filename}`, mirroring the existing `local_agent_prompt.txt` entry
- Modify: `services/ChatBot/local_agent_prompt.txt` (delete after Unit 3 verification)

**Approach:**
- Competition prompt inherits: general context, scope (minus meal), pronoun resolution (team focus), temporal scope, discovery patterns, Statbotics rules, glossary, event types, hallucination prevention, output style (~120 lines)
- Meal prompt inherits: general context, scope (meal only), pronoun resolution (user/household focus), hallucination prevention, output style (~25 lines)
- Both prompts get the "You are NOT user-facing" preamble and output style rules
- Competition prompt retains ALL shared glossary and temporal rules (they apply to both TBA and Statbotics)

**Patterns to follow:**
- Current `local_agent_prompt.txt` structure and tone
- `LoadPromptFile()` pattern in `DependencyInjectionExtensions.cs`

**Test scenarios:**
- Happy path: Competition prompt contains TBA discovery, Statbotics path rules, glossary, temporal scope, event type mapping
- Happy path: Meal prompt contains meal-specific pronoun resolution and output style
- Edge case: Competition prompt does NOT mention `fetch_meal_signup_info`
- Edge case: Meal prompt does NOT mention TBA, Statbotics, or competition concepts
- Happy path: Both prompts contain hallucination prevention and "NOT user-facing" preamble

**Verification:**
- Both prompt files exist and are well-formed
- No orphaned references to tools outside each agent's scope

- [ ] **Unit 2: Register two local agents in DI**

**Goal:** Replace single `WithLocalAgent` call with two domain-specific agent registrations.

**Requirements:** R1, R2, R3, R5

**Dependencies:** Unit 1

**Files:**
- Modify: `services/ChatBot/DependencyInjectionExtensions.cs`

**Approach:**
- Replace the single `.WithLocalAgent(cfg => { ... })` block (lines 154-173) with two calls
- Competition agent: `Name = "frc-competition"`, `DisplayName = "FRC Competition Data"`, description focuses on TBA + Statbotics domains, `Tools` = TBA + Statbotics tool names, `Infer = true`
- Meal agent: `Name = "meal-logistics"`, `DisplayName = "Meal Signup Lookup"`, description focuses on meal signups only, `Tools` = `["fetch_meal_signup_info"]`, `Infer = true`
- Agent `Description` fields must be distinctive enough for SDK inference to route correctly — competition description should NOT mention meals, meal description should NOT mention competition data
- Tool registrations (`.AddTool<>()` calls) remain unchanged — tools are registered globally, agents just reference them by name

**Patterns to follow:**
- Current `WithLocalAgent` pattern at `DependencyInjectionExtensions.cs:154-173`
- `CustomAgentConfig` property naming from SDK

**Test scenarios:**
- Happy path: Both agents registered with distinct names and descriptions
- Happy path: Competition agent has 5 tools (3 TBA + 2 Statbotics), meal agent has 1 tool
- Edge case: No tool overlap between agents
- Error path: Build succeeds with both agents registered (no DI conflicts)
- Integration: SDK session config contains both agents in `CustomAgents` collection

**Verification:**
- `dotnet build` succeeds with zero errors
- Both agents appear in `SessionConfig.CustomAgents` during session creation

- [ ] **Unit 3: Update tests and validate routing**

**Goal:** Update existing tests that reference `frc-data` agent name and verify routing works.

**Requirements:** R4

**Dependencies:** Unit 2

**Files:**
- Modify: test files referencing `frc-data` agent name (grep for exact files)
- Test: `gpt/tests/BC3Technologies.DiscordGpt.Copilot.Tests/`
- Test: `tests/FunctionApp.Tests/`

**Approach:**
- Find all test references to `"frc-data"` agent name and update to new names
- Verify integration tests still pass (Foundry agent → query_local → SDK inference → correct local agent)
- Add diagnostic logging for which local agent was selected (helps debug routing issues)

**Test scenarios:**
- Happy path: Competition query routes to `frc-competition` agent
- Happy path: Meal query routes to `meal-logistics` agent
- Edge case: Ambiguous query (e.g., "what's happening this week") — verify routing doesn't break
- Error path: Query about a domain not covered by any agent gets handled gracefully

**Verification:**
- All existing tests pass
- Build succeeds across solution and app
- Manual smoke test: competition query, meal query, cross-domain query all work

- [ ] **Unit 4: Cleanup and documentation**

**Goal:** Remove old prompt file and update documentation.

**Requirements:** R1

**Dependencies:** Unit 3

**Files:**
- Delete: `services/ChatBot/local_agent_prompt.txt`
- Modify: `services/ChatBot/Agents/foundry-agent.yaml` (update comment about "a local in-process agent" to "local in-process agents")
- Modify: `gpt/README.md` (if it references single local agent)

**Approach:**
- Delete the unified prompt file only after Unit 3 confirms everything works
- Update foundry-agent.yaml comments at line 8 ("the local in-process agent") and line 34 ("a local in-process agent for TBA, Statbotics, and meal-signup tools") to reflect multi-agent architecture
- Do NOT change the foundry agent's routing instructions — `query_local` stays generic

**Test expectation:** none — documentation-only changes after functional verification in Unit 3

**Verification:**
- Old `local_agent_prompt.txt` is deleted
- No dangling references to it in codebase
- Build still succeeds

## System-Wide Impact

- **Interaction graph:** Foundry agent → `query_local` → SDK inference → one of {frc-competition, meal-logistics}. The inference step is new and non-deterministic.
- **Error propagation:** Wrong-agent selection surfaces as weak/empty answers, not errors. Hard to distinguish from genuine "no data" responses.
- **State lifecycle risks:** None — agents are stateless within a session turn.
- **API surface parity:** No external API changes.
- **Integration coverage:** Cross-domain queries (TBA + Statbotics) must be tested to confirm they still resolve in one turn within the competition agent.
- **Unchanged invariants:** Foundry agent JSON format, tool implementations, session config composition chain, ISessionConfigSource behavior.

## Risks & Dependencies

| Risk | Mitigation |
|------|------------|
| SDK inference routes to wrong agent | Make descriptions maximally distinctive; add routing telemetry |
| Cross-domain meal+competition query misroutes | Unlikely given domain distinctiveness, but no fallback mechanism exists |
| Meal agent is too thin to be reliably selected | Strengthen description; monitor routing telemetry |
| No measurable quality improvement | Accept as learning; easy to revert (re-merge prompts, single WithLocalAgent) |

## Sources & References

- **Origin document:** [docs/brainstorms/2026-04-25-local-agent-decomposition-requirements.md](docs/brainstorms/2026-04-25-local-agent-decomposition-requirements.md)
- Related code: `services/ChatBot/DependencyInjectionExtensions.cs:154-173`
- Related code: `gpt/src/BC3Technologies.DiscordGpt.Copilot/CopilotBuilderAgentSkillToolExtensions.cs:26-39`
- SDK class: `CustomAgentConfig` (GitHub.Copilot.SDK)
- Prior analysis: Rubber-duck critique confirmed deferral recommendation
