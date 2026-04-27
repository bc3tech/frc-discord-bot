---
title: "refactor: Remove redundant skill infrastructure"
type: refactor
status: completed
date: 2026-04-25
origin: docs/brainstorms/2026-04-25-local-agent-decomposition-requirements.md
---

# refactor: Remove redundant skill infrastructure

## Overview

Remove the custom `IDiscordSkill` / `SkillRegistry` / `McpServerSkill` abstraction layer from the gpt SDK submodule. This layer duplicates the GHCP SDK's native skill system, is unused by the production app, and adds unnecessary public API surface. Also remove the dead SDK-native skill configuration wrappers (`AddSkillDirectory`, `DisableSkill`, `DiscordGptOptions.SkillDirectories/DisabledSkills`) since nothing calls them.

## Problem Frame

The gpt SDK wrapper defines a parallel skill system alongside its tool system. The custom `IDiscordSkill` → `SkillRegistry` → `CopilotDiscordAgent.CollectFunctions` pipeline mirrors the `IDiscordTool` → `ToolRegistry` pipeline but has zero registered implementations in production. The only concrete implementation — `McpServerSkill` in the Mcp project — is not even in the solution file. The `services/ChatBot` app uses only `IDiscordTool` and `WithLocalAgent`. All skill infrastructure is dead code.

(see origin: `docs/brainstorms/2026-04-25-local-agent-decomposition-requirements.md`)

## Requirements Trace

- R1. Remove `IDiscordSkill` interface from Core
- R2. Remove `SkillRegistry` hosted service from Copilot
- R3. Remove `AddSkill<T>` from Hosting
- R4. Remove `McpServerSkill` and the entire Mcp project (not in solution, dead code)
- R5. Remove skill wiring from `CopilotDiscordAgent`, `ICopilotToolAuthorizationPolicy`, `CopilotToolAuthorizationPolicy`, `CopilotToolAuthorizationOptions`
- R6. Remove skill registry setup from `DiscordGptBuilderCopilotExtensions`
- R7. Remove associated tests
- R8. Remove `AddSkillDirectory`/`DisableSkill` and `DiscordGptOptions.SkillDirectories/DisabledSkills` (dead SDK-native config wrappers)
- R9. Update ConsoleSample to remove MCP and skill references

## Scope Boundaries

- Not changing the local agent architecture — `frc-data` stays as-is
- Not changing `IDiscordTool` registrations or the tool authorization surface
- Not changing the Foundry agent prompt or routing
- Mcp project directories are deleted but `McpToolBridge` has no other consumers — safe to remove entirely

## Context & Research

### Relevant Code and Patterns

The skill infrastructure spans 5 projects with clear boundaries:

| Layer | File | Skill code | Non-skill code to preserve |
|-------|------|-----------|---------------------------|
| Core | `IDiscordSkill.cs` | Entire file | None |
| Copilot | `SkillRegistry.cs` | Entire file | None |
| Copilot | `CopilotDiscordAgent.cs` | `skillRegistry` param, skill loop (lines 83-93) | Tool loop, `RespondAsync`, logger messages |
| Copilot | `ICopilotToolAuthorizationPolicy.cs` | `IsSkillAuthorized` (lines 17-24) | `IsToolAuthorized` |
| Copilot | `CopilotToolAuthorizationPolicy.cs` | `IsSkillAuthorized` (lines 32-48) | `IsToolAuthorized`, `IsContextAuthorized` |
| Copilot | `CopilotToolAuthorizationOptions.cs` | `AllowedSkillNames`, `AllowAllSkills`, `AllowSkillsInDirectMessages` | All tool and context properties |
| Copilot | `DiscordGptBuilderCopilotExtensions.cs` | `SkillRegistry` factory + `AddHostedService` | All other DI wiring |
| Copilot | `CopilotBuilderAgentSkillToolExtensions.cs` | `AddSkillDirectory`, `DisableSkill` | `WithLocalAgent`, `AddTool` |
| Copilot | `DefaultSessionConfigSource.cs` | `SkillDirectories`/`DisabledSkills` blocks | All other session config |
| Copilot | `DiscorgGptOptions.cs` | `SkillDirectories`, `DisabledSkills` properties | All other options |
| Hosting | `DiscordGptBuilderToolExtensions.cs` | `AddSkill<TSkill>` method | `AddTool<TTool>`, `WithConversationStore`, `WithInMemoryConversationStore` |
| Mcp | Entire project | All files | None — delete directory |
| Mcp.Tests | Entire project | All files | None — delete directory |

**Key finding:** The Mcp and Mcp.Tests projects are NOT in `DiscordGpt.slnx` — they're orphaned directories that don't build. The ConsoleSample references Mcp via `ProjectReference` in its `.csproj` but is also not part of the main solution build.

### Institutional Learnings

- **Builder-scoped DI extensions** (`docs/solutions/best-practices/builder-scoped-di-extensions-for-harness-concerns-2026-04-22.md`): `AddSkill<T>` belongs on `DiscordGptBuilder`, `AddSkillDirectory`/`DisableSkill` on `CopilotBuilder`. Confirms the API surface locations. Also warns: don't accidentally remove `AddTool<T>` which follows the same pattern.
- **Conversation-scoped sessions** (`docs/solutions/best-practices/conversation-scoped-copilot-sessions-2026-04-23.md`): Shows canonical DI wiring patterns. Warns about C# test namespace collisions when deleting/renaming test files.
- After removal, update the DI learning's classification cheat-sheet (line ~172) which references `IDiscordSkill` — file a follow-up or fix inline.

## Key Technical Decisions

- **Delete entire Mcp project and Mcp.Tests project**: Both are orphaned from the solution. `McpToolBridge` has no consumers outside `McpServerSkill`. Cleaner than surgical removal.
- **Remove SDK-native skill config wrappers too**: `AddSkillDirectory`, `DisableSkill`, `DiscordGptOptions.SkillDirectories/DisabledSkills`, and `DefaultSessionConfigSource` wiring are dead code. Nothing calls them. They configure the GHCP SDK's native `SessionConfig.SkillDirectories` — a separate concept from `IDiscordSkill` — but since nothing uses this configuration path, it's dead surface area. Can be re-added if needed.
- **Update ConsoleSample in place**: Remove Mcp project reference, MCP server block, and skill auth options. The sample remains functional with just tools.
- **Update doc comment on interface**: Rename `ICopilotToolAuthorizationPolicy` doc comment from "tools and skills" to "tools" since the skill surface is removed.

## Open Questions

### Resolved During Planning

- **ConsoleSample's `AddMcpServer` usage**: The sample calls `gptBuilder.AddMcpServer(...)` conditionally on `MCP_SERVER_COMMAND` env var. Since the Mcp project is being deleted entirely, remove the Mcp `ProjectReference` from the `.csproj`, the `using BC3Technologies.DiscordGpt.Mcp;` import, and the entire MCP server block. The sample works fine with just `AddTool<PingTool>`.
- **`ICopilotToolAuthorizationPolicy` skill surface extent**: `IsSkillAuthorized` method + `AllowedSkillNames`, `AllowAllSkills`, `AllowSkillsInDirectMessages` on options. No other skill surface exists. `IsContextAuthorized` is shared with tool auth and must stay.
- **LoggerMessage for unauthorized functions**: The log message at line 29 says "tools/skills" — update to just "tools" since skills are gone. The `filteredCount` variable in `CollectFunctions` only counts tool auth failures after the skill loop is removed.

### Deferred to Implementation

- Whether the solution doc's classification cheat-sheet should be updated inline or as a follow-up task.

## Implementation Units

- [ ] **Unit 1: Delete Mcp source and test projects**

**Goal:** Remove the entirely dead `McpServerSkill`, `McpToolBridge`, `McpServerOptions`, `McpServiceCollectionExtensions` and all associated tests.

**Requirements:** R4, R7

**Dependencies:** None

**Files:**
- Delete: `gpt/src/BC3Technologies.DiscordGpt.Mcp/` (entire directory)
- Delete: `gpt/tests/BC3Technologies.DiscordGpt.Mcp.Tests/` (entire directory)

**Approach:**
- Delete both directories entirely. They are not in `DiscordGpt.slnx` so no solution file edit needed.
- The ConsoleSample's `ProjectReference` to Mcp is handled in Unit 4.

**Patterns to follow:**
- Standard directory deletion. No special cleanup since projects aren't in the solution.

**Test expectation: none** — deleting orphaned projects not in the solution.

**Verification:**
- Directories no longer exist
- Solution builds without these directories present

---

- [ ] **Unit 2: Remove IDiscordSkill and SkillRegistry**

**Goal:** Delete the core interface and the registry hosted service.

**Requirements:** R1, R2

**Dependencies:** None (can be parallel with Unit 1)

**Files:**
- Delete: `gpt/src/BC3Technologies.DiscordGpt.Core/IDiscordSkill.cs`
- Delete: `gpt/src/BC3Technologies.DiscordGpt.Copilot/SkillRegistry.cs`
- Delete: `gpt/tests/BC3Technologies.DiscordGpt.Copilot.Tests/SkillRegistryTests.cs`

**Approach:**
- Delete the three files. This will cause compile errors in dependent code — those are fixed in Unit 3.

**Patterns to follow:**
- Clean file deletion.

**Test expectation: none** — deleting source files; compile errors resolved in Unit 3.

**Verification:**
- Files no longer exist. Compile errors are expected until Unit 3 completes.

---

- [ ] **Unit 3: Remove skill wiring from Copilot project**

**Goal:** Excise all skill-related code from the Copilot project's mixed files — agent, auth policy, auth options, DI extensions, session config, options.

**Requirements:** R5, R6, R8

**Dependencies:** Unit 2 (references to deleted types must be gone)

**Files:**
- Modify: `gpt/src/BC3Technologies.DiscordGpt.Copilot/CopilotDiscordAgent.cs`
- Modify: `gpt/src/BC3Technologies.DiscordGpt.Copilot/ICopilotToolAuthorizationPolicy.cs`
- Modify: `gpt/src/BC3Technologies.DiscordGpt.Copilot/CopilotToolAuthorizationPolicy.cs`
- Modify: `gpt/src/BC3Technologies.DiscordGpt.Copilot/CopilotToolAuthorizationOptions.cs`
- Modify: `gpt/src/BC3Technologies.DiscordGpt.Copilot/DiscordGptBuilderCopilotExtensions.cs`
- Modify: `gpt/src/BC3Technologies.DiscordGpt.Copilot/CopilotBuilderAgentSkillToolExtensions.cs`
- Modify: `gpt/src/BC3Technologies.DiscordGpt.Copilot/DefaultSessionConfigSource.cs`
- Modify: `gpt/src/BC3Technologies.DiscordGpt.Copilot/DiscorgGptOptions.cs`
- Modify: `gpt/tests/BC3Technologies.DiscordGpt.Copilot.Tests/CopilotDiscordAgentTests.cs`
- Test: `gpt/tests/BC3Technologies.DiscordGpt.Copilot.Tests/CopilotDiscordAgentTests.cs`

**Approach:**

`CopilotDiscordAgent.cs`:
- Remove `SkillRegistry skillRegistry` from primary constructor parameters
- Remove the entire skill loop (lines 83-93) from `CollectFunctions`
- Update LoggerMessage at line 29: change "tools/skills" to "tools"
- Update comment at line 43: change "tools and skills" to "tools"

`ICopilotToolAuthorizationPolicy.cs`:
- Remove `IsSkillAuthorized` method (lines 17-24)
- Update interface doc comment: remove "and skills"

`CopilotToolAuthorizationPolicy.cs`:
- Remove `IsSkillAuthorized` method (lines 32-48)
- Update class doc comment: remove "and skills"

`CopilotToolAuthorizationOptions.cs`:
- Remove `AllowedSkillNames` property (line 32)
- Remove `AllowAllSkills` property (lines 40-44)
- Remove `AllowSkillsInDirectMessages` property (lines 52-56)
- Update class doc comment: remove "/skill"

`DiscordGptBuilderCopilotExtensions.cs`:
- Remove `SkillRegistry` singleton factory registration and `AddHostedService` line

`CopilotBuilderAgentSkillToolExtensions.cs`:
- Remove `AddSkillDirectory` method
- Remove `DisableSkill` method

`DefaultSessionConfigSource.cs`:
- Remove `SkillDirectories` and `DisabledSkills` blocks (lines ~41-49)

`DiscorgGptOptions.cs`:
- Remove `SkillDirectories` property
- Remove `DisabledSkills` property

`CopilotDiscordAgentTests.cs`:
- Remove `new SkillRegistry()` instantiation and constructor argument from all 3 test methods

**Patterns to follow:**
- Follow existing tool-only patterns in same files (e.g., `IsToolAuthorized` stays, mirrors the shape)
- `AddTool` pattern in `CopilotBuilderAgentSkillToolExtensions.cs` stays untouched

**Test scenarios:**
- Happy path: `CopilotDiscordAgent` constructs without `SkillRegistry`, `CollectFunctions` returns only tool functions
- Happy path: All 3 existing `CopilotDiscordAgentTests` pass with updated constructor (no skill registry)
- Edge case: `CollectFunctions` with unauthorized tools still increments `filteredCount` correctly

**Verification:**
- `dotnet build gpt/DiscordGpt.slnx` succeeds with 0 errors
- All Copilot tests pass
- No references to `IDiscordSkill`, `SkillRegistry`, `AddSkillDirectory`, `DisableSkill`, `AllowAllSkills`, `AllowSkillsInDirectMessages`, `AllowedSkillNames`, `SkillDirectories`, `DisabledSkills` remain in `gpt/src/BC3Technologies.DiscordGpt.Copilot/`

---

- [ ] **Unit 4: Remove AddSkill from Hosting and update ConsoleSample**

**Goal:** Remove `AddSkill<T>` from the Hosting project and clean up the ConsoleSample's MCP and skill references.

**Requirements:** R3, R9

**Dependencies:** Unit 1 (Mcp project deleted), Unit 3 (skill options removed)

**Files:**
- Modify: `gpt/src/BC3Technologies.DiscordGpt.Hosting/DiscordGptBuilderToolExtensions.cs`
- Modify: `gpt/src/samples/BC3Technologies.DiscordGpt.ConsoleSample/Program.cs`
- Modify: `gpt/src/samples/BC3Technologies.DiscordGpt.ConsoleSample/BC3Technologies.DiscordGpt.ConsoleSample.csproj`

**Approach:**

`DiscordGptBuilderToolExtensions.cs`:
- Remove `AddSkill<TSkill>` method (lines 36-43)

`ConsoleSample/Program.cs`:
- Remove `using BC3Technologies.DiscordGpt.Mcp;` (line 5)
- Remove MCP server block (lines 51-64)
- Remove `options.AllowAllSkills = true;` and `options.AllowSkillsInDirectMessages = true;`
- Remove `MCP_SERVER_COMMAND`/`MCP_SERVER_ARGS` from the env var help text

`ConsoleSample/.csproj`:
- Remove `ProjectReference` to `BC3Technologies.DiscordGpt.Mcp.csproj`

**Patterns to follow:**
- `AddTool<TTool>` method in same file shows the pattern that stays

**Test expectation: none** — ConsoleSample is not in the main solution and has no test project. Hosting tests don't cover `AddSkill<T>`.

**Verification:**
- `DiscordGptBuilderToolExtensions.cs` contains only `AddTool`, `WithConversationStore`, `WithInMemoryConversationStore`
- ConsoleSample compiles if built standalone (no Mcp reference)
- No references to `AddSkill`, `McpServer`, `AllowAllSkills`, `AllowSkillsInDirectMessages` in Hosting or ConsoleSample

---

- [ ] **Unit 5: Update production app and integration tests**

**Goal:** Remove skill auth options from the ChatBot service and its integration tests.

**Requirements:** R5 (production config), R7 (test cleanup)

**Dependencies:** Unit 3 (options properties removed)

**Files:**
- Modify: `services/ChatBot/DependencyInjectionExtensions.cs`
- Modify: `tests/FunctionApp.Tests/ChatBot/DiscordGptIntegrationTests.cs`
- Test: `tests/FunctionApp.Tests/ChatBot/DiscordGptIntegrationTests.cs`

**Approach:**

`services/ChatBot/DependencyInjectionExtensions.cs`:
- Remove `options.AllowAllSkills = true;` and `options.AllowSkillsInDirectMessages = true;`

`tests/FunctionApp.Tests/ChatBot/DiscordGptIntegrationTests.cs`:
- Remove `Assert.True(authorization.AllowAllSkills);` and `Assert.True(authorization.AllowSkillsInDirectMessages);`

**Patterns to follow:**
- Tool auth options (`AllowAllTools`, `AllowToolsInDirectMessages`) stay — follow that pattern

**Test scenarios:**
- Happy path: `AddFrcChatBotRegistersExpectedDiscordGptOptionsAndServices` passes without skill assertions
- Happy path: All other integration tests pass unchanged

**Verification:**
- `dotnet build app/FunctionApp.csproj` succeeds
- Integration tests pass
- No skill-related references remain in `services/ChatBot/` or `tests/FunctionApp.Tests/`

---

- [ ] **Unit 6: Final validation and cleanup**

**Goal:** Verify the entire solution builds clean and all tests pass. Grep for any remaining skill references.

**Requirements:** All

**Dependencies:** Units 1-5

**Files:**
- No file changes — validation only

**Approach:**
- Build entire solution: `dotnet build gpt/DiscordGpt.slnx`
- Run all tests: `dotnet test gpt/DiscordGpt.slnx`
- Build production app: `dotnet build app/FunctionApp.csproj`
- Run integration tests
- Grep across entire repo for `IDiscordSkill`, `SkillRegistry`, `McpServerSkill`, `AddSkill<`, `AddSkillDirectory`, `DisableSkill`, `AllowAllSkills`, `AllowSkillsInDirectMessages`, `AllowedSkillNames`
- Any remaining references in documentation (READMEs) should be updated or noted

**Test expectation: none** — this is a validation unit, not a code change.

**Verification:**
- Solution builds with 0 errors, 0 warnings related to skill infrastructure
- All tests pass (minus deleted `SkillRegistryTests` and Mcp tests)
- Grep returns no code references to removed types (documentation mentions are acceptable if updated)

## System-Wide Impact

- **Interaction graph:** `CopilotDiscordAgent.CollectFunctions` loses its skill iteration loop. The only remaining function source is `ToolRegistry.Tools`. No callbacks, middleware, or observers are affected.
- **Error propagation:** No change — skill authorization failures silently filtered functions; tool authorization continues unchanged.
- **State lifecycle risks:** None — `SkillRegistry` was an `IHostedService` that managed skill lifecycle (`InitializeAsync`/`DisposeAsync`), but no skills were ever registered in production.
- **API surface parity:** The gpt SDK's public API loses: `IDiscordSkill`, `SkillRegistry`, `AddSkill<T>`, `AddSkillDirectory`, `DisableSkill`, `IsSkillAuthorized`, and 3 options properties. This is a breaking change to the library's public API, but the only consumers are ChatBot (doesn't use it) and ConsoleSample (updated in Unit 4).
- **Unchanged invariants:** `IDiscordTool`, `ToolRegistry`, `AddTool<T>`, `IsToolAuthorized`, `WithLocalAgent`, and all tool/agent infrastructure remain untouched.

## Risks & Dependencies

| Risk | Mitigation |
|------|------------|
| Breaking change to SDK public API surface | Only 2 consumers (ChatBot, ConsoleSample) — both updated. No external consumers known. |
| ConsoleSample won't compile if Mcp project is deleted first | Unit 4 handles both Mcp reference removal and ConsoleSample update together |
| Accidentally removing `AddTool<T>` which follows same pattern as `AddSkill<T>` | Institutional learning explicitly warns about this. Unit reviews preserve `AddTool`. |
| Test namespace collision when deleting test files | Institutional learning warns about this. Watch for compile errors in adjacent test files. |

## Documentation / Operational Notes

- The DI learning at `docs/solutions/best-practices/builder-scoped-di-extensions-for-harness-concerns-2026-04-22.md` references `IDiscordSkill` in its classification table (line ~172). Should be updated to remove that row.
- READMEs in the Hosting and Copilot projects may reference skills — check during Unit 6.

## Sources & References

- **Origin document:** [docs/brainstorms/2026-04-25-local-agent-decomposition-requirements.md](docs/brainstorms/2026-04-25-local-agent-decomposition-requirements.md)
- **DI extensions learning:** [docs/solutions/best-practices/builder-scoped-di-extensions-for-harness-concerns-2026-04-22.md](docs/solutions/best-practices/builder-scoped-di-extensions-for-harness-concerns-2026-04-22.md)
- **Session wiring learning:** [docs/solutions/best-practices/conversation-scoped-copilot-sessions-2026-04-23.md](docs/solutions/best-practices/conversation-scoped-copilot-sessions-2026-04-23.md)
