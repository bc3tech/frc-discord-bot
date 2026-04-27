---
date: 2026-04-26
topic: statbotics-query-validation
---

# Statbotics Query Validation & Error Teaching

## Problem Frame

When the bot answers a question that requires Statbotics data (e.g. "top 10 teams in our division at worlds by EPA"), the model uses the `statbotics_api_surface` tool to discover endpoints, then calls `statbotics_api` with a `path` and free-form `query` string. The OpenAPI surface explicitly declares enum-valued query parameters (e.g. `type ∈ {regional, district, district_cmp, champs_div, einstein, offseason}`) and the surface tool's response text warns the model to use exact string values, never integers. The model still sends `type=3`, the Statbotics server returns HTTP 500 with an empty body, and the model has no actionable feedback to recover. Combined with large-response handling that requires multiple round-trips, a single bad enum value derails the entire conversation and the user gets a low-quality answer.

The validation guidance currently lives only in the surface tool's *response text* and a free-form `query` string, with no enforcement at the function-calling schema level and no parsing of the failed request when the server rejects it. The fix needs to remove the model's ability to repeat this mistake silently and turn server failures into teaching feedback.

## Requirements

**Pre-validation (client side, before HTTP call)**
- R1. `StatboticsTool` MUST parse the model-supplied `query` string against the loaded OpenAPI document for the chosen `path`/operation before issuing the HTTP request.
- R2. Validation MUST reject query parameter values that violate a declared `Enum`, returning a structured error naming the offending parameter, the value sent, and the full list of valid values.
- R3. Validation MUST reject query parameter NAMES that are not declared for the chosen operation (typo guard), returning a structured error naming the unknown parameter and the list of valid parameter names for that operation.
- R4. Validation MUST reject query parameter values that violate declared type/range constraints (`integer` Min/Max, `boolean`), returning a structured error naming the offending parameter, the value sent, and the constraint.
- R5. When pre-validation fails, the tool MUST NOT issue any HTTP request to Statbotics.
- R6. Pre-validation errors returned to the model MUST be in the same JSON envelope shape as a successful tool result so the harness/model can continue without special-casing error handling.

**Post-rewrite (server returned an error despite valid-looking input)**
- R7. When Statbotics returns HTTP 500 (or other 4xx/5xx) AFTER pre-validation passed, `StatboticsTool` MUST attempt to extract a teaching message from the response body and rewrite the failure into the same structured-error shape used by R6.
- R8. If no useful teaching message can be extracted from the response body, the rewrite MUST still surface: the request URL (with query), the HTTP status, and a hint that the OpenAPI doc may be stale or the value may be otherwise unsupported.

**Scope discipline**
- R9. Validation logic MUST be driven by the same OpenAPI document the surface tool uses — there is exactly one source of truth for enums and constraints.

## Success Criteria

- Sending `type=3` (or any other enum integer where a string is required) results in a structured tool error that names the parameter, the bad value, and the full enum — without any HTTP call to Statbotics.
- Sending a misspelled query parameter name (e.g. `?yearr=2026`) results in a structured tool error naming the unknown parameter and the documented parameter names — without any HTTP call to Statbotics.
- A run of "who are the top 10 teams in our division at worlds by EPA" no longer produces a Statbotics 500 from a bad enum value, and the bot reaches a real data fetch on its first or second tool call.
- A genuine Statbotics 500 (e.g. server outage, valid-looking input) returns a structured tool error with the URL and status the model can reason about, instead of an empty body.

## Scope Boundaries

- `TbaApiTool` is explicitly out of scope. If the same failure pattern shows up on TBA later, port the helper. Do not generalize speculatively now.
- Path-parameter validation (e.g. `{event}` shape, year ranges in path positions) is out of scope. The observed failure is a query-parameter enum violation; path validation can be added later if real failures show up.
- Auto-correction / fuzzy-matching of bad values to nearest valid enum is out of scope. The teaching message is enough; the model should pick.
- Refactoring `query` into a typed `parameters` object (Approach B from the brainstorm) is out of scope — keep the free-form `query` string contract; enforce at the tool boundary, not at the function-calling schema layer.

## Key Decisions

- **Approach D (validate + post-rewrite)** chosen over A (validate only) or B (typed schema). D gives belt-and-suspenders coverage with a small surface area: pre-validation prevents the common mistake without an HTTP round-trip, and post-rewrite catches everything else (genuine outages, schema lag, undocumented constraints) so the model never sees a bare 500.
- **Statbotics only.** TBA tool didn't exhibit the bug in the observed failure, so we ship narrow rather than wide. The OpenAPI-driven validation pattern, if it works for Statbotics, will be straightforward to lift to TBA later.
- **Strict validation (reject unknown query parameter names).** Catches model typos that would otherwise silently reach Statbotics and either be ignored or 500. Trade-off is OpenAPI-doc staleness — if the doc lags real API additions, the model can't use new params until the doc is updated. That's an acceptable forcing function: keep the OpenAPI doc current.
- **Same JSON envelope for errors as success.** Avoids forcing the harness/model to special-case error shapes; error content is just the `content` field of the existing tool-result envelope.

## Dependencies / Assumptions

- The Statbotics OpenAPI document (already loaded by `StatboticsApiSurfaceTool`) exposes every constraint we need: `Enum`, `Minimum`, `Maximum`, `Type`. Confirmed against the surface-tool response in `paste-1777263596176.txt`.
- The `query` argument is a URL-style query string the model assembles. Parsing it with the BCL is sufficient (no nested objects).

## Outstanding Questions

### Resolve Before Planning
*(none — ready for planning)*

### Deferred to Planning
- [Affects R1, R9][Technical] Is the loaded OpenAPI surface model already accessible to `StatboticsTool` via DI, or does it need to be lifted out of `StatboticsApiSurfaceTool` into a shared service?
- [Affects R7][Technical] Does the Statbotics 500 response body contain anything parseable in practice, or is it always empty? The observed failure shows `failed after 8534ms` and a snippet `[]`. If bodies are always empty, R8's fallback message is what the model will normally see — that's still acceptable.
- [Affects R2, R3, R4][Technical] What's the exact JSON shape of the structured error? Likely a small object with `error`, `parameter`, `sent`, and either `validValues` / `validNames` / `constraint` — to be finalized in planning against the existing tool-result envelope.

## Next Steps

→ `/ce-plan` for structured implementation planning
