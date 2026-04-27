---
date: 2026-04-25
topic: skill-infrastructure-removal
---

# Remove Redundant Skill Infrastructure

## Problem Frame

The gpt SDK wrapper defines a custom `IDiscordSkill` / `SkillRegistry` / `AddSkill<T>` layer that duplicates the GHCP SDK's native skill system (`SessionConfig.SkillDirectories`, `Rpc.Skill`, skill lifecycle events). The only implementation — `McpServerSkill` — wraps MCP servers, but the SDK may have native MCP support. The app (`services/ChatBot`) doesn't use any of this custom skill infrastructure. It should be removed.

> **Agent decomposition** (splitting `frc-data` into domain-specific agents) was evaluated in the same brainstorm but deferred after document review. The premise — prompt dilution at ~98 lines — lacked evidence of real answer quality degradation, and the split would introduce unproven SDK inference routing and cross-domain orchestration complexity. This can be revisited if concrete quality issues surface.

## Requirements

**Remove Custom Skill Infrastructure**

- R1. Remove `IDiscordSkill` interface from the Core project.
- R2. Remove `SkillRegistry` hosted service from the Copilot project.
- R3. Remove `AddSkill<T>` extension method from the Hosting project (`DiscordGptBuilderToolExtensions`).
- R4. Remove `McpServerSkill` from the Mcp project. If any active MCP server registrations exist (check ChatBot, ConsoleSample), migrate them to direct SDK configuration or remove them.
- R5. Remove skill-related wiring from `CopilotDiscordAgent`: the `SkillRegistry` constructor parameter, the `skillRegistry.Skills` iteration loop in `CollectFunctions`, and `IsSkillAuthorized` from `ICopilotToolAuthorizationPolicy` and its implementation. Also remove `AllowAllSkills`/`AllowSkillsInDirectMessages` from `CopilotToolAuthorizationOptions` if they exist.
- R6. Remove skill registry setup from `DiscordGptBuilderCopilotExtensions`.
- R7. Remove associated tests (`SkillRegistryTests`, MCP skill registration tests).

**SDK-Native Skill Configuration (Keep or Remove)**

- R8. Evaluate whether `AddSkillDirectory` and `DisableSkill` extension methods (in `CopilotBuilderAgentSkillToolExtensions`) should be kept as the builder-pattern API for SDK-native `SessionConfig.SkillDirectories`/`DisabledSkills`, or removed since nothing currently calls them. **Decision: remove** — they can be re-added if needed; dead code should not persist.
- R9. Remove `SkillDirectories` and `DisabledSkills` properties from `DiscordGptOptions` and the corresponding wiring in `DefaultSessionConfigSource`.

## Success Criteria

- All references to `IDiscordSkill`, `SkillRegistry`, `AddSkill<T>`, and `McpServerSkill` are removed from the codebase
- `CopilotDiscordAgent` compiles and functions without `SkillRegistry`
- All existing tests continue to pass (minus the removed skill-specific tests)
- No regression in bot behavior — the app never used this infrastructure at runtime

## Scope Boundaries

- Not changing the local agent architecture — `frc-data` stays as a single agent with its current prompt
- Not changing `IDiscordTool` registrations or interface (TBA, Statbotics, MealSignup tools stay as-is)
- Not reworking the MCP package beyond removing `McpServerSkill` — the MCP bridge (`McpToolBridge`, transport code) stays if it has other uses
- Not changing the Foundry agent prompt or routing

## Key Decisions

- **Remove `IDiscordSkill` entirely**: The SDK's native skill system covers the same ground. The custom layer is unused by the app and adds unnecessary abstraction.
- **Remove SDK-native skill configuration wrappers too**: `AddSkillDirectory`, `DisableSkill`, and the `DiscordGptOptions` properties are dead code. They wrap SDK-native features but nothing calls them. Remove to reduce surface; re-add if a consumer needs them later.
- **McpServerSkill migration is conditional**: Only needed if active MCP server registrations exist. Investigation suggests none exist in ChatBot, but the ConsoleSample uses `AddMcpServer` and may need updating or removal.

## Outstanding Questions

### Deferred to Planning

- [Affects R4] Confirm whether ConsoleSample's `AddMcpServer` usage needs migration or if the sample can be updated/removed.
- [Affects R5] Verify the full extent of `ICopilotToolAuthorizationPolicy` skill-related surface to remove.

## Next Steps

→ `/ce-plan` for structured implementation planning
