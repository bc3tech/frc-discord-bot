---
title: "fix: Statbotics query validation and 500 error rewrite"
type: fix
status: active
date: 2026-04-26
origin: docs/brainstorms/2026-04-26-statbotics-query-validation-requirements.md
---

# fix: Statbotics query validation and 500 error rewrite

## Overview

The chat bot's LLM repeatedly violates Statbotics enum constraints (e.g. `type=3` instead of `type=champs_div`) even though `statbotics_api_surface` warns against it in prose. Statbotics returns a bare HTTP 500 with no body, leaving the model with no signal to self-correct, so it gives up and tells the user it has no data. This plan adds (a) pre-validation of the `query` argument against the embedded OpenAPI doc and (b) post-call rewriting of opaque 500s into structured guidance, so the model can correct course inside the same turn.

## Problem Frame

`StatboticsTool.QueryStatboticsAsync` accepts a free-form `query` string and forwards it to Statbotics. The OpenAPI doc declares enum/type/range constraints on query parameters, but those constraints exist only in the surface tool's response *text* — there is no schema enforcement at the `statbotics_api` boundary. When Statbotics rejects an out-of-enum value with a 500 (empty body), the existing tool envelope reports `statusCode: 500, error: "HTTP 500 Internal Server Error", text: null` with no actionable guidance. The model has no way to recover. This plan closes the loop by treating the embedded OpenAPI doc as the authoritative contract for the `query` argument and converting Statbotics 500s into the same structured guidance envelope used for path-validation failures (see origin: `docs/brainstorms/2026-04-26-statbotics-query-validation-requirements.md`).

## Requirements Trace

- R1. Pre-validate query parameter names against the matched endpoint's declared `QueryParameters`; reject unknown names.
- R2. Pre-validate query parameter values against declared `Enum`, `Type`, `Minimum`, `Maximum`.
- R3. Pre-validation failures return a structured tool envelope identifying which parameter(s) failed and naming the legitimate alternatives.
- R4. After a real call, if Statbotics returns HTTP 500 and the request had query parameters, rewrite the envelope to surface the most likely offending parameter and remediation guidance.
- R5. Validation errors and 500-rewrite errors use the same JSON envelope shape as the existing path-validation error path (lines 89-101 of `services/ChatBot/Tools/StatboticsTool.cs`).
- R6. Scope: Statbotics only. TBA tool unchanged.
- R7. The pre-validation logic uses the *same* loaded `StatboticsApiSurface` instance already used by `DescribeApiSurfaceAsync` and `TryMatchConcretePath` — no second OpenAPI load.
- R8. Add unit tests covering: unknown param rejected, enum violation rejected, type/range violation rejected, valid query passes through, post-rewrite of synthetic 500 response.

## Scope Boundaries

- TBA (`TbaApiTool`) is **not** changed. (R6)
- The `query` argument schema stays a free-form string — no breaking change to the function-calling contract.
- No retry of the rewritten 500 — the model retries on its next turn using the new guidance.
- Required-parameter validation (HTTP 400 from Statbotics for missing required params) is not added. Statbotics already returns parseable 4xx for those; the failure mode in the log was specifically opaque 500s on enum violations.
- No probing of the live Statbotics API. Synthetic 500 responses in tests are sufficient.

## Context & Research

### Relevant Code and Patterns

- `services/ChatBot/Tools/StatboticsTool.cs` lines 78-105 — `QueryStatboticsAsync` and the existing path-validation error envelope at lines 89-101 to mirror.
- `services/ChatBot/Tools/StatboticsTool.cs` lines 107-144 — `s_statboticsApiSurface` lazy-loaded once at first access; reachable from `QueryStatboticsAsync` directly (resolves deferred Q1 from origin doc — no DI lift needed).
- `services/ChatBot/Tools/StatboticsTool.cs` lines 324-332 — `StatboticsParameter` record already carries `Enum`, `Type`, `Minimum`, `Maximum` (added in checkpoint 003).
- `services/ChatBot/Tools/StatboticsTool.cs` lines 395-401 — `StatboticsApiSurface.TryMatchConcretePath` returns the matched `StatboticsEndpoint` whose `QueryParameters` is the validation source of truth.
- `services/ChatBot/Tools/HttpGetToolBase.cs` lines 107-149 — `SerializeToolResponse` is the canonical envelope; new error envelopes must match its field shape (`apiRequest, statusCode, ok, data, text, error, userReferencePages, citations`) so the model sees consistent tool output.
- `tests/FunctionApp.Tests/HttpGetToolBaseTests.cs` — existing tests already construct real `StatboticsTool` with `StubHttpClientFactory`; the same pattern works for new tests.

### Institutional Learnings

- Checkpoint 003 surfaced enum metadata in the *surface* tool but did not enforce it at the *query* tool boundary — this plan completes that work.
- Checkpoint 005 confirmed the model ignored the `type=champs_div, never type=3` warning text; in-prose constraints are insufficient when the model is under pressure.

### External References

None used. The .NET BCL `System.Web.HttpUtility.ParseQueryString` (in `System.Web.HttpUtility` namespace, available cross-platform in modern .NET) handles query parsing.

## Key Technical Decisions

- **Validate against the matched endpoint, not globally.** `TryMatchConcretePath` already returns the endpoint; iterate that endpoint's `QueryParameters`. This avoids ambiguity when the same name has different constraints on different endpoints.
- **Strict unknown-name rejection.** Unknown parameter names are rejected, not warned. Decision from origin doc; matches the brainstorm's "strict validation" choice.
- **Reuse the existing static `s_statboticsApiSurface` field.** No refactor to lift the surface into DI. The static is process-wide, lazily initialized, and already used by both `DescribeApiSurfaceAsync` and `QueryStatboticsAsync`. (Resolves origin deferred Q1.)
- **Post-rewrite parses the SendGetAsync result and replaces it.** `SendGetAsync` returns a JSON string; we parse it once, check `statusCode`, and if 500 with non-empty query, build a new envelope. This avoids changing `HttpGetToolBase` for a Statbotics-only concern. (R6)
- **500-rewrite heuristic: name the parameter whose value most likely violated its declared schema.** If exactly one query parameter has declared constraints (Enum/Min/Max) and its supplied value violates them, name it; otherwise list all query parameters that *have* declared constraints with their legal values. The model then has enough to retry. We do not assume the 500 came from a specific parameter — guidance is framed as "most likely".

## Open Questions

### Resolved During Planning

- **Where does the loaded OpenAPI doc live?** (Origin Q1) → Already accessible via `s_statboticsApiSurface` static; no lift needed.
- **Exact JSON envelope shape?** (Origin Q2) → Mirror lines 89-101 of `StatboticsTool.cs`: `{apiRequest:{path,kind:"api"}, statusCode, ok:false, error, guidance, suggestions?}`. Pre-validation uses `statusCode: 400`; 500-rewrite preserves `statusCode: 500` so the rewrite is observable in logs.

### Deferred to Implementation

- **Does Statbotics ever return a non-empty 500 body in the wild?** (Origin Q3) The post-rewrite preserves any text that *was* returned by appending it to the new `error` field if non-empty. No live probing needed at planning time.
- **Exact `System.Web` package reference / namespace import.** `System.Web.HttpUtility` is available in modern .NET via `System.Web` (no extra package on net10.0 — confirm at implementation time; fall back to manual `string.Split('&')` parsing if it requires an unwanted dependency).

## Implementation Units

- [ ] **Unit 1: Add query-parameter pre-validation to `QueryStatboticsAsync`**

**Goal:** Parse the `query` argument, validate each pair against the matched endpoint's `QueryParameters`, and short-circuit with a structured error envelope on any violation.

**Requirements:** R1, R2, R3, R5, R7

**Dependencies:** None.

**Files:**
- Modify: `services/ChatBot/Tools/StatboticsTool.cs`
- Test: `tests/FunctionApp.Tests/HttpGetToolBaseTests.cs`

**Approach:**
- After the existing `TryMatchConcretePath` block (current line 102) and before `SendGetAsync` (current line 104), add a `TryValidateQuery(endpoint, query)` step.
- Parse query via `System.Web.HttpUtility.ParseQueryString` (or manual split if the dependency is unwanted) into name/value pairs. Empty/null query → skip validation.
- For each parsed pair:
  1. If name not present in `endpoint.QueryParameters` → record "unknown parameter" violation listing legal names.
  2. If declared `Enum` is non-null and value not in enum (case-sensitive — Statbotics enums are lowercase strings) → record "enum violation" listing legal values.
  3. If declared `Type` indicates numeric (`integer`/`number`) and value fails `int.TryParse`/`double.TryParse` → record "type violation".
  4. If declared `Minimum`/`Maximum` present and parsed numeric value out of range → record "range violation".
- If any violations recorded, return `JsonSerializer.Serialize(new { apiRequest = new { path, kind = "api" }, statusCode = 400, ok = false, error = "Query parameter validation failed.", guidance = "...", violations = [...], legalQueryParameters = [...] })`. Field names mirror the existing path-error envelope; add `violations` array as a structured list `{name, suppliedValue, problem, legalValues?}` so the model can mechanically pick the fix.
- Place the new helper(s) as `private static` methods on `StatboticsTool`, beside `ReadParameters`.

**Patterns to follow:**
- Existing path-validation error envelope at `services/ChatBot/Tools/StatboticsTool.cs` lines 89-101.
- `StatboticsParameter` record fields drive validation directly — no new model needed.

**Test scenarios:**
- Happy path: `path=/v3/team_events` `query=year=2026&type=champs_div` passes validation and reaches the stub HTTP client.
- Error path: `query=type=3` returns `statusCode: 400`, `ok: false`, `violations[0].problem` mentions enum, `violations[0].legalValues` includes `champs_div`.
- Error path: `query=foo=bar` (unknown name) returns 400 with `violations[0].problem` mentioning unknown parameter and `legalQueryParameters` populated for the matched endpoint.
- Error path: `query=year=notanumber` returns 400 with type violation.
- Error path: `query=limit=999999` (above maximum, if declared) returns 400 with range violation. Skip if no endpoint in the embedded spec has a maximum on `limit` — verify at implementation time.
- Edge case: `query=null` and `query=""` skip validation entirely and reach the stub HTTP client.
- Edge case: query against an endpoint whose template has zero declared `QueryParameters` rejects any supplied query parameter as unknown.

**Verification:**
- All test scenarios pass under `dotnet test tests/FunctionApp.Tests`.
- Validation error JSON parses and contains `statusCode: 400`, `violations` array with `name`, `suppliedValue`, `problem` keys.

---

- [ ] **Unit 2: Rewrite Statbotics HTTP 500 responses with parameter-aware guidance**

**Goal:** When `SendGetAsync` returns an envelope with `statusCode: 500` and the original call had query parameters, replace it with a structured envelope that names the constrained query parameters and their legal values.

**Requirements:** R4, R5, R6

**Dependencies:** Unit 1 (reuses the parsed query and matched endpoint).

**Files:**
- Modify: `services/ChatBot/Tools/StatboticsTool.cs`
- Test: `tests/FunctionApp.Tests/HttpGetToolBaseTests.cs`

**Approach:**
- After `SendGetAsync` returns its serialized JSON string, parse it with `JsonDocument.Parse`. If `statusCode == 500` AND the original `query` was non-empty AND the matched endpoint has at least one `QueryParameter` with declared constraints, build a replacement envelope.
- Replacement envelope: `{ apiRequest, statusCode: 500, ok: false, error: "Statbotics returned HTTP 500. The most common cause is a query parameter value that violates the endpoint's declared constraints (enum, type, or range).", guidance: "...", constrainedQueryParameters: [{name, type?, enum?, minimum?, maximum?}], suppliedQuery: <original query string>, originalResponseText: <text-from-original-envelope-or-null> }`.
- Preserve original `text` field content under `originalResponseText` for transparency.
- Do NOT rewrite if statusCode != 500, or if query was empty, or if no query parameter on the matched endpoint has declared constraints (in which case Statbotics' 500 is unlikely to be a value-validation issue and we have nothing useful to say).

**Patterns to follow:**
- Existing path-validation error envelope at `services/ChatBot/Tools/StatboticsTool.cs` lines 89-101.

**Test scenarios:**
- Happy path: stub returns 200 → envelope passes through unchanged (no rewrite).
- Error path: stub returns 500 for `path=/v3/team_events`, `query=year=2026&type=champs_div` → envelope is rewritten; `constrainedQueryParameters` includes `type` with its enum values.
- Error path: stub returns 500 for a path whose endpoint has no constrained query params → envelope is NOT rewritten (passes through).
- Error path: stub returns 500 for a call with no query → envelope is NOT rewritten.
- Edge case: stub returns 500 with non-empty body text → rewritten envelope's `originalResponseText` contains that text.
- Error path: stub returns 4xx (e.g. 404) → envelope is NOT rewritten (only 500 triggers rewrite).

**Verification:**
- Tests above pass.
- Rewritten envelope deserializes cleanly and contains `statusCode: 500`, `constrainedQueryParameters` array.

---

- [ ] **Unit 3: Logging for validation rejections and 500 rewrites**

**Goal:** Add structured log entries when pre-validation rejects a call or when a 500 is rewritten, so operators can see how often the model is hitting these paths.

**Requirements:** Supports R8 (operational visibility, mentioned implicitly in origin "Success Criteria").

**Dependencies:** Units 1 and 2.

**Files:**
- Modify: `services/ChatBot/Tools/StatboticsTool.cs` (or a `StatboticsToolLogs.cs` partial if one exists — verify at implementation time).

**Approach:**
- Use the source-generated logger pattern already in this repo (`[LoggerMessage]` on partial static `Log` class). Two new entries:
  - `StatboticsQueryValidationRejected(string path, string query, int violationCount)` at `LogLevel.Warning`.
  - `StatboticsApi500Rewritten(string path, string query)` at `LogLevel.Warning`.
- Match the file's existing logging conventions; if the file has no existing logger usage beyond the inherited base, add a small `partial class StatboticsTool` block in a new file `services/ChatBot/Tools/StatboticsTool.Logs.cs` mirroring the convention used by `MealSignupInfoTool` (verify at implementation time).

**Patterns to follow:**
- `services/ChatBot/Tools/HttpGetToolBase.cs` already calls `logger.HttpAPIToolCallFailedForClientNameRequestPathWithStatusStatusCode(...)` — same source-generated logger style.

**Test scenarios:**
- Test expectation: none — pure logging; covered by Units 1 and 2 behavioral tests. Add one smoke test only if the source-generator setup requires a compile-time check we cannot otherwise prove.

**Verification:**
- Solution builds with no new warnings.
- Manual log inspection during the next bot run shows the new entries.

## System-Wide Impact

- **Interaction graph:** Only `StatboticsTool.QueryStatboticsAsync` changes. `StatboticsApiSurfaceTool`, `TbaApiTool`, and `HttpGetToolBase` are untouched.
- **Error propagation:** Pre-validation returns a JSON envelope (string), same as today's path-validation error path — the calling Microsoft.Extensions.AI tool-invocation layer sees a normal successful tool result whose body indicates a validation failure. No exceptions thrown.
- **State lifecycle risks:** None — validation is stateless and purely derived from the already-loaded OpenAPI doc.
- **API surface parity:** TBA tool keeps its current behavior (free-form query). If the same enum-violation pattern shows up for TBA in future, this plan can be ported. (Origin doc explicitly scoped TBA out.)
- **Integration coverage:** Unit tests using `StubHttpClientFactory` cover the SendGetAsync→rewrite path end-to-end. No need for live integration tests.
- **Unchanged invariants:** `statbotics_api` function-calling schema (path:string, query:string?) is unchanged. Existing successful queries return identical envelopes. Path-validation error envelope at lines 89-101 is unchanged.

## Risks & Dependencies

| Risk | Mitigation |
|------|------------|
| Statbotics enum values change in a future OpenAPI revision and the embedded spec lags. | Validation is driven entirely by the embedded spec — when the spec is regenerated, validation tracks it automatically. No code change needed. |
| Some legitimate Statbotics 500s are not enum violations and the rewrite misleads the model. | Rewrite preserves `originalResponseText` and frames guidance as "most common cause", not absolute. Rewrite is also gated on the endpoint having at least one constrained query param. |
| `System.Web.HttpUtility` requires a reference that is undesirable. | Fall back to manual `query.Split('&')` parsing — query strings here are simple key=value, no encoding edge cases that would warrant a dependency. |
| Case sensitivity mismatch on enum comparison — Statbotics enums could be mixed case. | Inspect the embedded `ChatBot.OpenApi.statbotics.json` at implementation time; pick `Ordinal` or `OrdinalIgnoreCase` based on what the spec actually contains. |
| Unknown-parameter rejection breaks a legitimate query the embedded spec is missing. | Origin doc explicitly chose strict mode. If a real false-positive surfaces, the embedded spec is the fix path, not the validator. |

## Documentation / Operational Notes

- No README/docs update required — this is internal tool behavior the LLM consumes via JSON.
- Log entries from Unit 3 give operators visibility into how often the model is mis-using Statbotics; if the rate is high after deployment, that's a signal to tighten the surface tool's prose further.

## Sources & References

- **Origin document:** [docs/brainstorms/2026-04-26-statbotics-query-validation-requirements.md](../brainstorms/2026-04-26-statbotics-query-validation-requirements.md)
- Related code: `services/ChatBot/Tools/StatboticsTool.cs`, `services/ChatBot/Tools/HttpGetToolBase.cs`
- Related tests: `tests/FunctionApp.Tests/HttpGetToolBaseTests.cs`
- Prior checkpoint: `D:/.copilot/session-state/2b36039f-c2d4-4f0a-8ed2-7fe08cd745c1/checkpoints/003-statbotics-enum-fix-and-commit.md` (added enum metadata to surface tool)
- Diagnosing log: session file `paste-1777263596176.txt` — captured the canonical `type=3` failure
