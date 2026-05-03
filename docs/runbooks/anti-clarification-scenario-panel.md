# Anti-Clarification Scenario Panel

**Purpose:** Verifies the anti-clarification policy holds across canonical
scenarios. The seven scenarios below are the contractual verification surface
for requirements R1-R13 defined in
`docs/plans/2026-04-27-002-feat-agent-anti-clarification-policy-plan.md`.

**Origin:** Bear Metal Bot anti-clarification policy plan, brainstorm
2026-04-27. The seven scenarios derive from the plan's `Success Criteria`
section and remain the authoritative qualitative gate for any change to the
four prompt-loading entry points listed under `When to run` below.

**When to run:** After every change to any of these files (the four-file
lockstep set):

- `services/ChatBot/Agents/foundry-agent.yaml`
- `services/ChatBot/agent_prompt.txt`
- `services/ChatBot/local_agent_prompt.txt`
- `services/ChatBot/Tools/StatboticsTool.cs` (matrix entries, guidance string,
  competition-window boolean, or `matrix_match` block)

Also run after any change to `FrcSystemPromptChatClient.cs` (R6 backstop
regex), `Log.cs` (Warning event id 50), or
`DependencyInjectionExtensions.cs` (`TimeProvider`, `IChatClient`
registration).

**Expected duration:** ~30-45 minutes for one full panel pass (21 runs across
the live Discord bot, plus Goodhart-guard scan).

**Prerequisites:**

- A Discord channel where the deployed Bear Metal Bot is reachable.
- The Foundry portal has been manually synced after the most recent
  `foundry-agent.yaml` edit (yaml definition fields are NOT auto-synced from
  source control — see yaml header lines 14-15).
- The bot service has been restarted after any `.txt` prompt edit (no
  hot-reload; prompts are loaded at startup via `LoadPromptFile`).
- Maintainer has access to the bot's Application Insights / log stream for the
  Goodhart-guard scan.

---

## Sampling Protocol (v1 verification rigor)

For each of the seven scenarios S1-S7, the maintainer executes **3 runs** in
the Discord channel using **3 paraphrased input variants** (defaults provided
per scenario; substitute equivalents if needed). After all 3 runs, score the
scenario against its Pass criteria checklist:

| Result        | Action                                                                                       |
| ------------- | -------------------------------------------------------------------------------------------- |
| **3/3 PASS**  | Scenario PASS. Record in panel summary.                                                      |
| **2/3 PASS**  | Scenario WARN. Record the failing variant for the next iteration; the change MAY ship.       |
| **<2/3 PASS** | Scenario FAIL. Block the change from shipping. Iterate on the prompt edit and re-run S1-S7.  |

**Total panel size:** N = 21 (7 scenarios × 3 variants). The full panel must
reach PASS or WARN on every scenario before a change ships.

### v1 limitation (explicit)

N=21 is **not** statistically rigorous. The panel cannot reliably detect drift
that affects fewer than ~30% of inferences for any single scenario. This is
acceptable for v1 because:

1. The alternative is no verification surface at all.
2. Telemetry-grade quantitative measurement is gated on a v2 effort that
   requires SDK telemetry hooks not present today.
3. The Goodhart-guard scan (below) provides the ongoing-drift backstop.

A future v2 will add per-`next_step` telemetry counters and re-ask-rate
metrics directly off SDK events.

---

## Scenarios

### S1 — Future-event metric default (R1, R2, R7, R12, R13)

**Setup:** A future Statbotics event with row state `Upcoming` is being
referenced (e.g., FRC World Championship at brainstorm date 2026-04-27, where
Worlds is still ahead). The user uses the team shorthand "us" (= Team 2046)
and divisional shorthand "our division at worlds" (= Newton).

**User message variants (3):**

1. `top 10 in our division at worlds`
2. `give me the top ten Newton teams`
3. `rank Newton this year top-to-bottom`

**Expected `next_step`:** `final` (no clarifying ask).

**Expected answer-shape:**

- R1: inline assumption phrase present, e.g., "Using Statbotics' projected
  EPA for Newton 2026 (the event has not yet started)…"
- R13: the metric phrase quoted MUST match the `human_readable_name` for
  `epa.total_points.mean` row in the `team_event` matrix VERBATIM.
- R11: no raw field paths visible (`epa.total_points.mean`,
  `data.epa.unitless`, etc.).
- R2: no clarifying question offered (since `statbotics_api_surface` narrows
  the future-event metric to a single ⚠️ projection row).
- R10 (if applicable): cosmetic footer permitted IF a separate cosmetic
  default was applied (e.g., names+numbers vs numbers-only); never on the
  R1-defaulted dimension itself.

**Pass criteria (per run):**

- [ ] Bot answered with a numbered/bulleted top-10 list of Newton teams.
- [ ] Inline R1 assumption phrase present and correctly quotes "Statbotics'
      projected EPA" verbatim.
- [ ] No raw field paths in user-facing text.
- [ ] No clarifying question.

**Common failure modes:**

- "Do you want EPA or qual ranking?" — R1/R2 fail (the question doesn't
  reduce work; both lead to the same lookup since quals haven't happened).
- "Top 10 by epa.total_points.mean…" — R11 fail (raw field path leaked).
- "Top 10 by EPA score" — R13 fail (paraphrase, not the matrix's verbatim
  human-readable name).

---

### S2 — Successful local lookup → no follow-up clarification (R9, R3, R10)

**Setup:** The user asks a question where the local agent successfully fetches
data (e.g., a past event with completed matches). After the lookup, the
hosted agent must route directly to the answer, NOT ask for cosmetic
refinement.

**User message variants (3):**

1. `who did we play in finals at PNW dcmp last year`
2. `our finals partners at last year's district champs`
3. `2025 PNW dcmp finals — who were we with`

**Expected `next_step`:** `final`.

**Expected answer-shape:**

- Direct answer with team numbers + names (cosmetic default per R3).
- R10 footer permitted (one-line opt-out hint), e.g., "(Numbers only if
  you'd prefer.)" — must NOT repeat if the prior bot turn already carried
  one.
- No question like "Do you want me to include team names?" or "Should I
  show match scores too?"

**Pass criteria (per run):**

- [ ] Bot answered with finals alliance partners (numbers AND names).
- [ ] No clarifying question after the successful lookup.
- [ ] R10 footer either present (correct one-line voice) or correctly
      suppressed (if prior turn carried one).

**Common failure modes:**

- "Got it — would you like just numbers, or numbers and names?" — R9 fail.
- "Coming right up! …" with em-dash filler — R10 voice fail.

---

### S3 — Cosmetic preference → default-and-correct, no blocking ask (R3, R4, R10)

**Setup:** The user asks for output where multiple cosmetic shapes are valid
(list vs table, numbers vs numbers+names, short vs long).

**User message variants (3):**

1. `show me bear metal's events this year`
2. `what events are we doing in 2026`
3. `2046 schedule for 2026`

**Expected `next_step`:** `final`.

**Expected answer-shape:**

- Default shape applied silently (e.g., bulleted list with event name +
  date + location).
- R10 footer permitted: e.g., "(Want a table instead?)"
- No blocking ask: NEVER "Should I show this as a list or a table?" before
  answering.

**Pass criteria (per run):**

- [ ] Bot answered immediately with a sensible default shape.
- [ ] No blocking clarifying ask about format/shape.
- [ ] R10 footer (if present) is one short sentence in plain Bear Metal voice.

**Common failure modes:**

- Bot asks "Would you prefer a list or a table?" before answering — R3 fail.
- Footer reads "Let me know if you'd like a different format!" with
  exclamation — R10 voice fail.

---

### S4 — Action-language ban in clarifying ask (R5, R6, R7)

**Setup:** A scenario where the carve-out under R9 legitimately fires (e.g.,
ambiguous pronoun referent — see S6 for the canonical case). The bot
emits an `ask_user` step. Verify the question itself does NOT contain
action language or disguised statements.

**User message variants (3):**

1. `what about her` (no prior referent in the last 3 turns)
2. `give me their stats` (no prior team referent)
3. `how did we do` (no prior event referent)

**Expected `next_step`:** `ask_user`.

**Expected answer-shape:**

- R5/R6: question contains NO action-language verbs ("I'll pull…", "Let me
  fetch…", "Coming right up…", "I'll go grab…").
- R7: question follows the structural template (one focused question, no
  options that include known-invalid paths).
- R11: no internal token names ("ask_user", "next_step", "final",
  "messageToUser") in user-facing text.

**Pass criteria (per run):**

- [ ] Bot asked exactly one focused clarifying question.
- [ ] No action-language verbs in the question.
- [ ] No internal tokens visible to the user.

**Common failure modes:**

- "Coming right up — which team did you mean?" — R5/R6 fail (action +
  question disguised).
- "I'll fetch that as soon as you tell me which event." — R5/R6 fail.

---

### S5 — Decision-grade carve-out (R4, U4b)

**Setup:** During an FRC competition window (`is_frc_competition_window =
true`, current month ∈ {Feb, Mar, Apr}), the user asks a question where the
wrong default could mislead a live decision. The bot should lean toward
stated assumptions inline rather than pure silent defaulting.

**User message variants (3):**

1. `who's our best alliance pick right now` (during a live event)
2. `should we pick 1234 or 5678` (during alliance selection)
3. `what's our predicted score next match` (between matches)

**Expected `next_step`:** `final` with explicit inline assumptions, OR
`ask_user` if the question genuinely cannot be defaulted.

**Expected answer-shape:**

- R1: inline assumption explicitly states the metric/window being used
  (e.g., "Using Statbotics' projection for the next match…").
- R4: invitation to correct in the same answer, NOT a blocking ask before
  answering.
- R13: metric phrase verbatim from the matrix.

**Pass criteria (per run):**

- [ ] Inline assumption present and explicit.
- [ ] No blocking ask before any data is shown.
- [ ] Verbatim matrix human-readable name used.

**Common failure modes:**

- Silent default with no caveat — R4 carve-out missed (could mislead live
  decision).
- Pure blocking ask with no provisional answer — R4 over-applied.

**Note on month gating:** if the panel is run outside Feb-Mar-Apr, this
scenario verifies that the bot does NOT artificially over-state assumptions
(i.e., the carve-out is correctly off).

---

### S6 — Pronoun without resolvable referent (R9 carve-out (c))

**Setup:** The user sends a pronoun whose referent cannot be resolved from
the prior 3 conversation turns. This is the canonical legitimate post-fetch
ambiguity for R9 carve-out (c).

**User message variants (3):** (in a fresh conversation thread with no prior
context)

1. `what's their record`
2. `did they win`
3. `how are they doing`

**Expected `next_step`:** `ask_user`.

**Expected answer-shape:**

- One focused question asking for the referent (e.g., "Which team or
  alliance are you asking about?").
- R5/R6: no action language.
- R7: question does NOT include options the agent already knows are
  invalid.

**Pass criteria (per run):**

- [ ] Bot asked for the referent in one focused question.
- [ ] No action-language verbs.
- [ ] No internal tokens.

**Common failure modes:**

- Bot guesses (e.g., assumes "they" = Team 2046) and answers — R9 fail
  (guessing on unresolved pronoun rather than asking).
- Bot asks two questions stacked together — R7 structural fail.

---

### S7 — Internal-token / field-path leak audit (R11)

**Setup:** Across any scenario S1-S6, audit the bot's user-facing text for
internal-token leaks. This is a passive overlay scenario rather than a
distinct user prompt — the maintainer scans the transcripts of S1-S6 runs
for R11 violations.

**User message variants (3):** the full transcripts of S1-S6 (3 paraphrases
each = 18 prior runs serve as the audit surface). Optionally, also issue:

1. `what are the available next_step values` (probe — bot must NOT echo
   the internal token verbatim in its answer)
2. `what does messageToUser mean` (same)
3. `explain ask_user vs final` (same)

**Expected `next_step`:** `final`.

**Expected answer-shape:**

- R11: NO appearance of any of the banned internal tokens in user-facing
  text: `next_step`, `ask_user`, `final`, `messageToUser`, raw Statbotics
  field paths (`epa.total_points.mean`, `data.epa.unitless`, etc.),
  `matrix_match`, `is_frc_competition_window`.
- The bot uses translated human-language equivalents (e.g., "I'll ask you
  a question" instead of "next_step=ask_user").

**Pass criteria (per audit pass):**

- [ ] No banned tokens appear in any S1-S6 transcript.
- [ ] Probe variants (1)-(3) above do NOT cause the bot to echo the token
      back as part of its answer. (Definitional explanation IS allowed if
      the user's question is meta about the bot's own internals; the bot
      must use plain-language translations.)

**Common failure modes:**

- "I'll set next_step=ask_user — which team did you mean?" — R11 fail.
- "The matrix_match block says…" — R11 fail.

---

## Per-Rule Cross-Reference Grid

| Requirement | Validating scenarios |
| ----------- | -------------------- |
| R1          | S1, S2, S5           |
| R2          | S1                   |
| R3          | S2, S3               |
| R4          | S3, S5               |
| R5          | S4, S6               |
| R6          | S4, S6               |
| R7          | S4, S6               |
| R8          | (Statbotics-only scope; verified by S1, S5 implicitly) |
| R9          | S2, S6               |
| R10         | S2, S3               |
| R11         | S7 (audit overlay across S1-S6) |
| R12         | S1                   |
| R13         | S1, S2, S5           |

Every requirement R1-R13 is covered by at least one scenario.

---

## Goodhart-Guard (v1 qualitative)

The scenario panel measures whether the policy holds **on the canonical
inputs**. The Goodhart-guard catches drift that the panel cannot — cases
where the bot satisfies the panel but users in production still feel
over-clarified or under-served.

**Weekly scan protocol:**

1. Open the bot's Discord channel(s) where it is deployed.
2. Review the most recent ~50 production turns.
3. Look for clustering of these signals:
   - "no I meant…" or "that's wrong" within 2 turns of a bot answer.
   - Explicit user re-asks within 2 turns of a bot answer.
   - User repeating the same question with added context (sign that the
     bot's first answer missed the intent).
   - Visible bot text leaking R11-banned tokens.
4. If clustering is observed following a release of any prompt-touching
   change, treat as a Goodhart violation **even if the panel passed**, and
   roll back the change.

**Per-scenario inline scan:** during each scenario's 3 runs, the maintainer
also checks the bot's next 1-2 turns AFTER each run for immediate
violations (e.g., the bot then re-asks a clarifying question on a follow-up,
or leaks a token in a follow-up answer).

This guard is qualitative; quantitative re-ask-rate measurement is a v2
success criterion gated on telemetry not present in the current SDK.

---

## Panel Pass/Fail Summary Template

After running all 7 scenarios, fill in the summary:

| Scenario | Result (PASS / WARN / FAIL) | Notes |
| -------- | --------------------------- | ----- |
| S1       |                             |       |
| S2       |                             |       |
| S3       |                             |       |
| S4       |                             |       |
| S5       |                             |       |
| S6       |                             |       |
| S7       |                             |       |
| Goodhart |                             |       |

**Ship gate:** every row PASS or WARN; no FAILs. If any row is FAIL, iterate
on the prompt edit (or C# backstop) and re-run the entire panel.
