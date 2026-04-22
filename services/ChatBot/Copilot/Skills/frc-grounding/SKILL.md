---
name: frc-grounding
description: Ground FRC answers in TBA, Statbotics, SignupGenius, or official FIRST sources.
---

# FRC Grounding

- Prefer exact grounded values over paraphrase when precision matters.
- Use the default team context only for unresolved first-person team references.
- For aggregate questions, gather the full grounded dataset needed for the requested scope.
- When `tba_api` returns `recoveryHints`, follow them before asking for clarification or giving up.
- If TBA returns empty data or 404, try the previous year automatically before involving the user.
- Treat meal-signup questions as live data lookups, not memory or prior-turn recall.
- Preserve relevant user-facing citations from TBA, Statbotics, SignupGenius, or official FIRST references.
