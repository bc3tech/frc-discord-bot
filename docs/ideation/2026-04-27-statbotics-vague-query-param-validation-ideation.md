---
date: 2026-04-27
topic: statbotics-vague-query-param-validation
focus: teach the agent legal values for query parameters whose OpenAPI descriptions are vague (e.g., `metric`, `country`, `state`)
---

# Ideation: Statbotics "Vague Query Parameter" Validation

## Problem

After the prior validation pass shipped (`fix(chatbot): validate Statbotics queries and rewrite opaque 500s`), the model recovers cleanly from enum-violation 500s. But it still fumbles the `metric=` parameter on `/v3/team_events` (and 6 other list endpoints) because the OpenAPI description is "any column in the table is valid" — which is true at the upstream ORM layer but *not* derivable from the response shape (the response is restructured by `to_dict()` into nested objects; the legal sort columns are ORM column names, not response keys).

We surveyed the OpenAPI spec for query parameters that are neither `enum`-declared, numeric+range, nor boolean. Five surfaced:

| Param | Endpoints | Cardinality | Inlineable? |
|---|---|---|---|
| `metric` | 7 | ~30–100 per endpoint | **Yes** — ORM source |
| `country` | 4 | ~30 (countries with FRC teams) | **Yes** — finite, but not constrained at OSS source |
| `state` | 4 | ~50 US + 13 CAN | **Yes** — finite, but not constrained at OSS source |
| `event` | 3 | thousands × many years | **No** — keep format hint |
| `match` | 1 | huge | **No** — keep format hint |

V1 scope: **`metric` only.** `country` and `state` are deferred — Statbotics doesn't constrain them at the source level (only `EventType`/`EventStatus`/`MatchWinner`/`MatchStatus`/`CompLevel` are in `backend/src/types/enums.py`, and those are already in the OpenAPI spec). Country/state values come from whatever the database has accumulated; a different mechanism is needed.

## Codebase Context

**Existing tooling (post yesterday's work):**
- `services/ChatBot/Tools/StatboticsTool.cs` — has `TryBuildQueryValidationError` (rejects bad query params pre-call with structured 400 envelope) and `TryRewriteServerError` (rewrites opaque 500s with constraint guidance). Both consume `StatboticsParameter` records from the embedded OpenAPI surface (`s_statboticsApiSurface`).
- `services/ChatBot/Log.cs` — EventIds 35 (`StatboticsQueryValidationRejected`) and 36 (`StatboticsApi500Rewritten`).
- `app/Apis/statbotics.json` — embedded OpenAPI v3 spec, source of `StatboticsParameter.Enum`/`Type`/`Minimum`/`Maximum`.

**Existing repo patterns:**
- `gpt` submodule (`https://git.bc3.tech/bc3tech/discord-gpt.git`) — already established; contributors know `--recurse-submodules`.
- No existing source-generator usage in the repo, but Roslyn `IIncrementalGenerator` is the modern .NET pattern for build-time codegen.

**Statbotics OSS structure** (`avgupta456/statbotics`):
- `backend/src/db/models/*.py` — SQLAlchemy ORM models. Each ORM class defines `mapped_column(...)` declarations whose names are the legal `metric` values for the corresponding endpoint.
- `backend/src/api/team_event.py` (and siblings) — pass `metric=...` straight to `common_filters` decorator which uses it as an ORM column reference.
- `backend/src/types/enums.py` — only contains enums already declared in the OpenAPI spec.

## Ranked Ideas

### 1. Sparse-shallow git submodule + Roslyn incremental source generator

**Description:**

- **Submodule:** `services/Statbotics/github-src` → `https://github.com/avgupta456/statbotics.git`, configured with `shallow = true` and sparse-checkout limited to `backend/src/db/models/` and `backend/src/types/`. SHA pinned in parent repo. Refresh = `git submodule update --remote services/Statbotics/github-src && git commit`.
- **Generator:** `services/Statbotics/SourceGen/StatboticsKnownValuesGenerator.csproj` — netstandard2.0, references `Microsoft.CodeAnalysis.CSharp` (analyzer-only, not bundled into runtime). Implements `IIncrementalGenerator`:
  1. Pulls `.py` files under `backend/src/db/models/` from `context.AdditionalTextsProvider`
  2. Per file: regex-extracts ORM class name + `mapped_column(...)` declarations
  3. Maps ORM class → endpoint template via in-generator lookup (7 entries)
  4. Emits a single `StatboticsKnownValues.g.cs` containing `IReadOnlyDictionary<string, ImmutableHashSet<string>> MetricColumns`
  5. **Hard-fails as `Diagnostic("STATBOT001", DiagnosticSeverity.Error)`** when any model file produces zero columns — surfaces as a build error AND IDE squiggle, forcing review.
- **`services/ChatBot/ChatBot.csproj` wiring:**
  ```xml
  <ItemGroup>
    <ProjectReference Include="..\Statbotics\SourceGen\StatboticsKnownValuesGenerator.csproj"
                      OutputItemType="Analyzer" ReferenceOutputAssembly="false" />
    <AdditionalFiles Include="..\Statbotics\github-src\backend\src\db\models\*.py" />
  </ItemGroup>
  ```
- **Validation:** `TryBuildQueryValidationError` consults `StatboticsKnownValues.MetricColumns[endpoint.Template]` for any `metric=...` query param. Reject violations with the same 400 envelope as yesterday, listing legal columns.
- **No runtime GitHub fallback.** Stale submodule = forcing function for review. The bot rejecting a now-legal `metric` until someone bumps the submodule is the *desired* signal.
- **CI guard:** a step that fails if the build produces a `.g.cs` different from the previous build — prevents drift between submodule SHA and code expectations.

**Rationale:**
- **Correctness:** ORM column names are the ground truth — response-shape inference would have taught the model the wrong vocabulary.
- **Build is offline-deterministic:** submodule files come from disk, no compile-time network.
- **Single moving part:** source generator + analyzer reference; no console-app, no MSBuild target with `<Exec>`, no manual `Inputs`/`Outputs`.
- **Inherent incrementality:** `IIncrementalGenerator` reruns only when an input changes.
- **IDE-time generation:** IntelliSense lights up immediately after a submodule bump.
- **Strong forcing-function signal:** stale submodule → next deploy surfaces the gap → triggers human review → reviewer also re-checks generator parser still matches upstream conventions.
- **Reuses established repo pattern:** `gpt` submodule already exists; contributors and CI know `--recurse-submodules`.
- **Generalizes:** future "vague" params just need new ORM-class-to-endpoint mappings in the generator.

**Downsides:**
- Submodule init/sparse-checkout adds one-time setup steps for new contributors (mitigated by `git config submodule.recurse=true` and a setup script).
- Refresh is a two-step ritual (`git submodule update --remote` + commit) — but that's *desirable* friction.
- Source generators run on every keystroke in IDE; regex parser must stay fast (~50ms for 10 small files).
- `netstandard2.0` constraint on generator project limits BCL surface (non-issue for regex + string emission).
- Debugging source generators requires `Debugger.Launch()` or `<EmitCompilerGeneratedFiles>true</EmitCompilerGeneratedFiles>` (documented in generator README).
- Doesn't auto-recover from upstream changes between refreshes; bot may reject a now-legal `metric` until someone bumps the submodule. Acceptable per design intent.

**Confidence:** 95%
**Complexity:** Medium
**Status:** Explored — handed off to /ce-brainstorm 2026-04-27

## Rejection Summary

| # | Idea | Reason Rejected |
|---|---|---|
| 1 | Live-sample cache (call `?limit=1`, parse response keys) | Wrong source-of-truth: response keys ≠ ORM column names; would have taught model wrong vocabulary |
| 2 | Hardcoded column maps in C# | Drift risk; inferior to OSS-sourced generation |
| 3 | Headless-browser scrape of statbotics.io tables | React-rendered page, heavy Playwright dep, brittle |
| 4 | Mine Statbotics frontend at build time | Couples to 3rd-party React internals; build-time complexity |
| 5 | Pre-call validation with sample fetch | Doubles every metric'd call |
| 6 | Auto-strip invalid metric and proceed | Hides intent from model; anti-pattern |
| 7 | Standalone `statbotics_columns(endpoint)` discovery tool | Adds tool surface; better to enrich existing surface tool |
| 8 | Augment `statbotics_api_surface` output with column hints (without source generation) | Subsumed by source-generated approach |
| 9 | Cheatsheet in `agent_prompt.txt` | Bloats prompt; drifts; inferior to validation envelope |
| 10 | Override OpenAPI param description in surface output | Subsumed by source-generated approach |
| 11 | Extend 500-rewrite to include sampled column list | Reactive; works but slower than pre-call validation |
| 12 | Default-sort fallback when `metric` omitted | Behavior change; masks model intent |
| 13 | Out-of-band PowerShell/script generator | Not part of build; user explicitly rejected |
| 14 | T4 templates | Modern .NET deprecated path; Roslyn source generators are the modern equivalent |
| 15 | Console-app generator + MSBuild `BeforeTargets="BeforeCompile"` `<Exec>` target | Replaced by Roslyn source generator now that input files are local (submodule); fewer moving parts |
| 16 | In-tree Python snapshot files (no submodule) | Submodule is version-pinned, reviewable as SHA bumps, matches existing repo pattern |
| 17 | Always-download-from-GitHub at build time | Breaks offline/air-gapped/CI determinism |
| 18 | Runtime GitHub-fetch fallback when validation rejects unknown metric | Would hide the stale-submodule signal — contradicts the design intent of using submodule SHA as a forcing function for review |
| 19 | Inlining `country`/`state` in v1 | OSS source doesn't constrain them; defer to follow-up with different data source |
| 20 | Inlining `event`/`match` keys | Cardinality (thousands × many years) makes embedding impractical |

## Session Log

- 2026-04-27: Initial ideation — 20 candidates generated, refined across 4 passes (initial broad ideation → OSS-source angle → vague-param survey & build-time generator → submodule + Roslyn source generator). 1 survivor.
- 2026-04-27: Idea #1 marked Explored; handed off to /ce-brainstorm.
