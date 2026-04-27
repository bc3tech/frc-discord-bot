---
title: Nested repos need their own submodules for local project references
date: 2026-04-25
category: docs/solutions/workflow-issues/
module: git-submodules
problem_type: workflow_issue
component: development_workflow
severity: medium
applies_when:
  - A nested folder must become an independently pushable Git repository
  - A parent repository tracks that nested repository as a submodule
  - A standalone subrepo references shared project files from the parent checkout
symptoms:
  - Shared project references work from the parent checkout but fail inside the standalone subrepo
  - The subrepo references a path outside its own repository boundary
root_cause: missing_workflow_step
resolution_type: workflow_improvement
related_components:
  - tooling
  - documentation
tags: [git, submodules, dotnet, project-references, repository-layout]
---

# Nested repos need their own submodules for local project references

## Context

`lib\otel` started as a plain folder in the parent repository, but it needed to be an independently pushable repository for the `CopilotSdk.OpenTelemetry` package. After splitting it into its own repository and tracking it from the parent as a submodule, the `gpt` subrepo still referenced the parent checkout's `lib\otel` path.

That reference only worked when `gpt` was nested inside the parent repo. A standalone clone of `gpt` would not have the parent repo's `lib\otel` folder.

## Guidance

Each independently cloned repository needs its own submodule path for local `ProjectReference` dependencies. Do not point a subrepo project reference at a path that only exists because the subrepo is currently checked out inside a parent superproject.

Add the shared dependency as a submodule inside the repo that builds against it:

```powershell
Set-Location gpt
git submodule add https://github.com/brandonh-msft/copilotsdk-opentelemetry.git lib\otel
```

Then reference the project through the subrepo-local submodule path. In `gpt\src\BC3Technologies.DiscordGpt.Copilot\BC3Technologies.DiscordGpt.Copilot.csproj`, replace the parent-relative reference:

```xml
<ProjectReference Include="..\..\..\lib\otel\src\CopilotSdk.OpenTelemetry\CopilotSdk.OpenTelemetry.csproj" />
```

with the repo-local reference:

```xml
<ProjectReference Include="..\..\lib\otel\src\CopilotSdk.OpenTelemetry\CopilotSdk.OpenTelemetry.csproj" />
```

Commit the subrepo change first, then commit the parent repository's updated gitlink for that subrepo:

```powershell
git -C gpt add .gitmodules lib\otel src\BC3Technologies.DiscordGpt.Copilot\BC3Technologies.DiscordGpt.Copilot.csproj
git -C gpt commit -m "Add local OpenTelemetry submodule reference"

git add gpt
git commit -m "Update gpt OpenTelemetry submodule wiring"
```

## Why This Matters

A subrepo should build from a standalone clone. If `gpt` references `..\..\..\lib\otel`, it depends on the parent repository's directory layout rather than its own declared dependencies. That breaks independent development, CI jobs that clone only `gpt`, package builds, and onboarding.

Adding `gpt\lib\otel` as a submodule makes the dependency explicit inside the repository that needs it. The parent repo can still track both `gpt` and `lib\otel`, but `gpt` no longer relies on being inside that parent checkout.

## When to Apply

- A nested repository needs to reference another local project.
- The referenced project is independently versioned or pushed to its own remote.
- The nested repository must build outside the parent superproject.
- CI or contributors may clone the subrepo directly.
- A parent repository tracks multiple independently pushed components as submodules.

## Examples

The OpenTelemetry split used this sequence:

1. Convert parent `lib\otel` from tracked files into a standalone repository and parent submodule.
2. Add the same OpenTelemetry repo as `gpt\lib\otel` inside the `gpt` repository.
3. Change `BC3Technologies.DiscordGpt.Copilot.csproj` to reference `..\..\lib\otel\...` instead of `..\..\..\lib\otel\...`.
4. Build the affected project:

```powershell
dotnet build gpt\src\BC3Technologies.DiscordGpt.Copilot\BC3Technologies.DiscordGpt.Copilot.csproj --configuration Release
```

The relevant commits from the fix were:

- Parent `lib\otel` split: `48533bf`
- `gpt` local submodule/reference fix: `8840eb3`
- Parent `gpt` gitlink update: `2827071`

## Related

- `docs/solutions/logic-errors/opentelemetry-incorrect-span-hierarchy-2026-04-24.md` — same OpenTelemetry package, different telemetry logic issue.
- `docs/solutions/best-practices/builder-scoped-di-extensions-for-harness-concerns-2026-04-22.md` — same `gpt` project area, different guidance.
- `docs/solutions/integration-issues/copilot-sdk-sessionstatepath-must-be-non-empty-2026-04-24.md` — same Copilot project area, unrelated root cause.
