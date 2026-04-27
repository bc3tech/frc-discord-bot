---
date: 2026-07-18
topic: tool-display-and-followup-ux
---

# Tool Display Names & False Follow-Up Promises

## Problem Frame

Discord users see raw tool names like "Running: statbotics_api" and "report_intent (0.0s)" in progress feedback — these are meaningless developer identifiers, not user-facing descriptions. Additionally, the model sometimes promises "I'll post the top 10 in a moment" but never follows through, because the Copilot SDK is single-turn and has no mechanism for follow-up messages.

## Requirements

**Tool Display**
- R1. When `report_intent` fires, display its `intent` argument value (e.g., "💭 Looking up team stats") instead of the raw tool name. If the argument is missing/empty, suppress the message entirely.
- R2. When `report_intent` completes, suppress the completion message (e.g., don't show "✅ report_intent (0.0s)") — the intent text on start is sufficient.
- R3. All other tools continue to display with their current names — no name-mapping for now.

**Follow-Up Promises**
- R4. Add a system prompt instruction in `foundry-agent.yaml` telling the model to never promise follow-up messages, deferred results, or multi-part responses. All results must be included in the single `final` response.

## Success Criteria
- Progress feedback for `report_intent` shows the human-readable intent description, not the raw tool name
- `report_intent` completion lines are suppressed
- The model no longer generates text like "I'll post the results in a moment" or "I'll follow up with..."

## Scope Boundaries
- No configurable display-name mapping for arbitrary tools
- No multi-turn response infrastructure
- No changes to non-`report_intent` tool display names

## Key Decisions
- **Level 1 targeted approach**: Special-case `report_intent` rather than building a general tool-name mapping. This covers the most impactful case with minimal code.
- **System prompt fix for follow-ups**: The cheapest correct fix — tell the model not to do it rather than trying to implement the capability.

## Next Steps
→ `/ce-plan` for structured implementation planning
