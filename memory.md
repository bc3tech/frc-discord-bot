# Memory

Durable notes for future coding agents working in this repository.

## Documented solutions

`docs\solutions\` contains searchable writeups for solved problems and durable practices. The docs are organized by category and use YAML frontmatter fields such as `module`, `problem_type`, `component`, `severity`, and `tags`.

Relevant before debugging, implementing in a documented area, or making repository-structure decisions.

## Submodule layout

The parent repository tracks `gpt` as a submodule. OpenTelemetry package code lives in the standalone `lib\otel` repository, also tracked from the parent as a submodule.

If a nested repository needs to build against OpenTelemetry, it must declare its own `lib\otel` submodule. Do not use project references that reach out to the parent checkout, because those paths do not exist when the nested repository is cloned independently.

For `gpt`, references to `CopilotSdk.OpenTelemetry` should resolve through `gpt\lib\otel`, for example:

```xml
<ProjectReference Include="..\..\lib\otel\src\CopilotSdk.OpenTelemetry\CopilotSdk.OpenTelemetry.csproj" />
```

## Submodule commit workflow

When changing a nested repository:

1. Commit inside the nested repository first.
2. Return to the parent repository.
3. Stage and commit the updated submodule gitlink.

This keeps the parent commit pointing at a real commit in the nested repository.
