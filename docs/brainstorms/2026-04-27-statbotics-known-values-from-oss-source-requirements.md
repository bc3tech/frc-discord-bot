---
date: 2026-04-27
topic: statbotics-known-values-from-oss-source
---

# Statbotics Known-Value Validation from OSS Source

## Problem Frame

The `statbotics_api` tool currently rejects bad enum/numeric/range query parameters via a structured 400 envelope (shipped 2026-04-26). It does **not** validate parameters whose OpenAPI descriptions are vague — most notably `metric=` ("any column in the table is valid"), `country=`, and `state=`. The model fumbles `metric` repeatedly on `/v3/team_events` and 6 other list endpoints because:

- The legal sort columns are upstream **ORM column names** in `backend/src/db/models/*.py`
- The `to_dict()` response shape on the wire is heavily restructured (e.g., the model is sorted by `epa` but the response shows `epa.total_points.mean`), so the model can't infer legal values from prior responses

Statbotics is open-source ([avgupta456/statbotics](https://github.com/avgupta456/statbotics)). The legal value set for `metric` is mechanically derivable from the ORM model files. Country and state values can be snapshotted from the live `/v3/teams` endpoint.

## Requirements

**Source of truth (submodule)**
- R1. Add `vendor/statbotics/` as a plain shallow git submodule of `https://github.com/avgupta456/statbotics.git` (default branch: `master`). No sparse-checkout configuration; matches the existing `gpt` submodule style.
- R2. The submodule SHA pinned in `.gitmodules` is the source of truth for legal `metric=` values. Refresh = `git submodule update --remote vendor/statbotics && git commit`.
- R3. No runtime GitHub fetching. A stale submodule must surface as a forcing function for human review, not be silently self-healed.

**Build-time code generation (Roslyn source generator)**
- R4. Add `services/Statbotics/SourceGen/StatboticsKnownValues.SourceGen.csproj` — an `IIncrementalGenerator` targeting `netstandard2.0`. Referenced from `services/ChatBot/ChatBot.csproj` as `OutputItemType="Analyzer"` with `ReferenceOutputAssembly="false"`.
- R5. The generator reads `.py` files under `vendor/statbotics/backend/src/db/models/` (registered via `<AdditionalFiles>` in the consumer csproj), parses ORM `mapped_column(...)` declarations, and emits a single `StatboticsKnownValues.g.cs` containing:
  - `IReadOnlyDictionary<string, ImmutableHashSet<string>> MetricColumns` — keyed by endpoint template (e.g., `/v3/team_events`)
  - `ImmutableHashSet<string> KnownCountries`
  - `ImmutableHashSet<string> KnownStates`
- R6. The generator hard-fails as `Diagnostic("STATBOT001", DiagnosticSeverity.Error)` if any expected ORM model file produces zero columns. This signals upstream convention drift and forces human review during the next submodule bump.
- R7. The generator hard-fails as `Diagnostic("STATBOT002", DiagnosticSeverity.Error)` if the country/state JSON snapshot file is missing at build time.
- R8. The endpoint-template → ORM-class mapping is a hardcoded table inside the generator (7 entries: `TeamEventORM`/`TeamYearORM`/`TeamMatchORM`/`MatchORM`/`EventORM`/`YearORM`/`TeamORM`). Adding a new Statbotics list endpoint requires a generator update (intentional friction).

**Country/state snapshot (sibling refresh tool)**
- R9. Add `services/Statbotics/SourceGen/Tools/CountryStateRefresh/CountryStateRefresh.csproj` — a console project (.NET 10). Maintainers invoke `dotnet run --project services/Statbotics/SourceGen/Tools/CountryStateRefresh` to query Statbotics' live `/v3/teams?limit=10000` (paginating as needed), extract distinct `country`/`state` values, and write `vendor/statbotics-extras/country-state.json`.
- R10. The country/state JSON snapshot lives at `vendor/statbotics-extras/country-state.json` (NOT inside the submodule, since it's our snapshot of API data, not upstream source). It is committed to the repo and read by the source generator via `<AdditionalFiles>`.
- R11. The refresh tool is opt-in (manual invocation by maintainers). No build-time network. No runtime fallback.

**Validation behavior in `StatboticsTool.cs`**
- R12. `TryBuildQueryValidationError` consults `StatboticsKnownValues.MetricColumns[endpoint.Template]` for any `metric=...` query parameter. Reject violations with the same 400 envelope shape used yesterday — listing the legal columns under a new field (e.g., `legalMetricColumns`) alongside `legalQueryParameters`.
- R13. Apply the same validation to `country=...` against `KnownCountries` and `state=...` against `KnownStates`.
- R14. **Edge case — missing endpoint:** if `MetricColumns` has no entry for the matched endpoint (e.g., Statbotics added a new list endpoint but the generator's mapping table wasn't updated), skip metric validation for that endpoint. Let the call go through; the existing 500-rewrite still applies if it fails. Log at Information level so the gap is observable.
- R15. **Edge case — empty set:** if `KnownCountries` or `KnownStates` is somehow empty (build still succeeded but no data), skip the corresponding validation. Same observability log.

**Discovery-time teaching (statbotics_api_surface)**
- R16. The `statbotics_api_surface` tool's response includes a per-endpoint `legalMetricColumns` field for endpoints that have a known column set. This proactively teaches the model legal values at discovery time, not just on validation failure.

**Logging**
- R17. Add a new log entry (next free EventId, e.g. 37) `StatboticsValidationSkipped` at Information level: "Skipped {ParameterName} validation for {Path} ({Reason})" — covers R14/R15.
- R18. Existing EventIds 35 (`StatboticsQueryValidationRejected`) and 36 (`StatboticsApi500Rewritten`), shipped in the prior 2026-04-26 PR (`fix(chatbot): validate Statbotics queries and rewrite opaque 500s`), continue to fire as before. EventId 35 fires when this brainstorm's new validation rejects an unknown `metric`/`country`/`state` value — that is the signal referenced in Success Criterion bullet 2 below. No new EventId is allocated for "stale submodule detected" — staleness is observed via repeated EventId 35 firings on values the maintainer expects to be legal.

**Repository plumbing**
- R21. Update `.github/workflows/build.yml` so every `actions/checkout@v4` step uses `with: { submodules: recursive }`. Without this, CI builds will hard-fail on `STATBOT001` because the submodule contents aren't present.
- R22. Update repository `README.md` Development Environment section to instruct contributors to run `git submodule update --init --recursive` after cloning (or set `git config --global submodule.recurse true`). This makes the existing implicit dependency on the `gpt` submodule and the new dependency on `vendor/statbotics` explicit.

**Testing**
- R19. Generator unit tests use hand-rolled fixture `.py` files in the test project (hermetic; no submodule dependency in tests). Cover: happy path, multi-class file, zero-column hard-fail, malformed input.
- R20. `StatboticsTool` integration tests cover: valid `metric` passes pre-call validation; invalid `metric` produces the 400 envelope with `legalMetricColumns`; valid `country`/`state` pass; invalid reject; missing-endpoint case skips validation; surface tool output contains `legalMetricColumns`.

## Success Criteria

- The model, given the current Statbotics tooling and the 2026-04-23 prompt set, makes a `/v3/team_events?event=2026new&metric=epa` (or any other valid metric) call without first guessing an invalid name in observed Discord traces over a 7-day post-deploy window.
- A stale-submodule scenario (model rejects a now-legal `metric` value because Statbotics added a column) is visible in logs as repeated EventId 35 firings on a value the maintainer expected to be legal, and resolves with a single `git submodule update --remote vendor/statbotics && git commit` action.
- Build is fully offline-deterministic: a fresh clone with `--recurse-submodules` followed by `dotnet build` produces the same `.g.cs` on any machine without network access.

## Scope Boundaries

- **Not in scope:** validating `event=` or `match=` keys (cardinality too high to embed; existing format-hint description is sufficient).
- **Not in scope:** runtime fallback to fetch latest model files from GitHub when validation rejects an unknown value. Explicitly rejected — would hide the stale-submodule signal.
- **Not in scope:** auto-refresh of country/state snapshot on a schedule. Maintainers run `CountryStateRefresh` manually when they notice drift or as part of routine maintenance.
- **Not in scope:** bundling the source generator as a NuGet analyzer for external consumers. Internal use only.
- **Not in scope:** generating richer typed result models from the ORM declarations (could be a future expansion; out of scope here).
- **Comparison rules:** validation against `MetricColumns`, `KnownCountries`, and `KnownStates` is case-sensitive ordinal matching, mirroring the 2026-04-26 enum validation behavior. Any deviation from Statbotics' actual case sensitivity is to be confirmed during planning (see Outstanding Questions).

## Key Decisions

- **OSS source, not response inference:** ORM column names are the ground truth; response keys are a restructured projection. Inferring from responses would teach the wrong vocabulary.
- **Submodule over vendored snapshot:** Submodule SHA bumps are reviewable, version-pinned, and match the established repo pattern (`gpt` submodule).
- **Plain shallow over sparse-shallow:** ~10 MB clone is acceptable; sparse adds .gitmodules-incompatible setup steps for marginal disk savings.
- **Roslyn source generator over console-app generator:** Now that input files are local on disk, the source-generator path eliminates an MSBuild target + console project pair, gives IDE-time IntelliSense, and surfaces parser regressions as build errors with diagnostic IDs.
- **Hardcoded endpoint→ORM mapping over derived:** Adding a new endpoint should be deliberate; a regression where a new endpoint silently has no validation is worse than a build-time hard-fail when someone forgets to update the mapping.
- **No runtime fallback:** A self-healing runtime would hide the very signal (validation rejecting a now-legal value) that prompts the maintainer to bump the submodule and re-review the generator. Forcing-function behavior is intentional.
- **Two projects, one folder:** Source generator and refresh tool have different runtimes (analyzer vs. console) and different invocation patterns (every build vs. on-demand). Splitting them keeps each focused; placing them under `services/Statbotics/SourceGen/` groups them logically.

## Dependencies / Assumptions

- The `avgupta456/statbotics` repo continues to use SQLAlchemy `mapped_column(...)` declarations under model classes named `*ORM`. Any convention change there will be caught by `STATBOT001` at the next submodule bump.
- The Statbotics live `/v3/teams?limit=10000` endpoint returns full team objects with `country` and `state` fields. Pagination via `offset` is supported.
- New contributors and CI run `git submodule update --init --recursive` (or have `git config --global submodule.recurse true`). Repo `README.md` should mention this; the `gpt` submodule already implicitly requires it.

## Outstanding Questions

### Resolve Before Planning
*(none — all product decisions resolved)*

### Deferred to Planning
- [Affects R4][Technical] Exact .slnx integration: how should the new analyzer + console projects be added to `FRCDiscordBot.slnx` (solution folders, build configurations)?
- [Affects R5][Technical] Exact regex patterns (or Python AST approach) for parsing `mapped_column(...)` declarations — should handle multi-line decls, type annotations like `MOF`/`MS`/`MOS`, and comment lines.
- [Affects R6/R7][Technical] How exactly are diagnostic IDs registered and surfaced in the build output? Is there an existing diagnostic-ID registry pattern in the repo to follow?
- [Affects R9][Needs research] Statbotics live API rate limits — does pulling 10,000+ teams in one refresh need throttling or pagination?
- [Affects R12/R16][Technical] Exact field-naming conventions for the new envelope fields (`legalMetricColumns`, `legalCountries`, etc.) to match yesterday's envelope shape.
- [Affects R20][Technical] How to integrate the new analyzer into the existing test infrastructure (`tests/FunctionApp.Tests/` for tool-level tests, plus a new `tests/StatboticsKnownValues.SourceGen.Tests/` or similar for the generator unit tests).
- [Affects R5][Technical] Roslyn analyzer netstandard2.0 constraint: confirm `ImmutableHashSet<>`/`IReadOnlyDictionary<>` are reachable from generated code targeting `netstandard2.0` (likely yes via `System.Collections.Immutable`) before locking the generated-shape design.

## Next Steps
→ `/ce-plan` for structured implementation planning.
