# StatboticsKnownValues.SourceGen

Roslyn `IIncrementalGenerator` that emits a static `ChatBot.Tools.StatboticsKnownValues` class containing the legal value sets for the Statbotics API's vague query parameters: per-endpoint `metric` column names, plus global `country` and `state` value sets.

## Inputs

The generator consumes two file groups, supplied via `<AdditionalFiles>` in the consuming `.csproj` (currently `services/ChatBot/ChatBot.csproj`):

1. `vendor/statbotics/backend/src/db/models/*.py` — SQLAlchemy ORM declarations from the upstream Statbotics repo (a git submodule). The generator extracts each `mapped_column(...)` declaration as a legal sort column for the corresponding `/v3` list endpoint via `EndpointMapping.OrmClassToEndpoint`.
2. `vendor/statbotics-extras/country-state.json` — distinct `country` and `state` values snapshotted from Statbotics' live `/v3/teams` endpoint via the sibling `Tools/CountryStateRefresh` console tool.

## Output

A single generated file in the consumer's `obj/Generated/StatboticsKnownValues.SourceGen/`:

```csharp
internal static class StatboticsKnownValues
{
    public static readonly IReadOnlyDictionary<string, ImmutableHashSet<string>> MetricColumns;
    public static readonly ImmutableHashSet<string> KnownCountries;
    public static readonly ImmutableHashSet<string> KnownStates;
}
```

All collections are `StringComparer.Ordinal`. Sort order is `StringComparer.Ordinal` ascending — guarantees byte-for-byte deterministic output across machines (Success Criterion S3).

## Diagnostics

| ID | Severity | Triggers |
|---|---|---|
| `STATBOT001` | Error | An expected ORM file produced zero columns. Upstream conventions may have changed; inspect the file and update `PyOrmParser` if needed. |
| `STATBOT002` | Error | The country/state JSON snapshot file isn't registered as `<AdditionalFiles>`. Run `Tools/CountryStateRefresh` and verify `ChatBot.csproj` wiring. |
| `STATBOT003` | Warning | A parsed `*ORM` class isn't in `EndpointMapping.OrmClassToEndpoint`. Either add the mapping (if Statbotics added a new list endpoint) or narrow the parser scope. |

## Refresh ritual

When Statbotics ships changes (new ORM column, new endpoint, added country/state):

```sh
git submodule update --remote vendor/statbotics
# review changes:
git diff --submodule=log
# if a new endpoint appeared, update services/Statbotics/SourceGen/EndpointMapping.cs
# if STATBOT001 fires after this, inspect the offending file and adjust PyOrmParser
git add vendor/statbotics
git commit -m "chore: bump vendor/statbotics submodule"

# refresh country/state snapshot if the bot starts rejecting valid country/state values:
dotnet run --project services/Statbotics/SourceGen/Tools/CountryStateRefresh -- \
  --output vendor/statbotics-extras/country-state.json
git add vendor/statbotics-extras/country-state.json
git commit -m "chore: refresh Statbotics country/state snapshot"
```

## Debugging

To see the generated source code in your editor, temporarily add this to `services/ChatBot/ChatBot.csproj`:

```xml
<PropertyGroup>
  <EmitCompilerGeneratedFiles>true</EmitCompilerGeneratedFiles>
  <CompilerGeneratedFilesOutputPath>$(IntermediateOutputPath)Generated</CompilerGeneratedFilesOutputPath>
</PropertyGroup>
```

Then rebuild. The generated `StatboticsKnownValues.g.cs` will appear under `services/ChatBot/obj/Debug/net10.0/Generated/StatboticsKnownValues.SourceGen/StatboticsKnownValuesGenerator/`. Remove the property after debugging — the Roslyn generator pipeline doesn't need files on disk to function.

To set a debugger breakpoint inside the generator, add `System.Diagnostics.Debugger.Launch();` near the top of `Initialize` or `Emit`, build the consuming project, and attach when prompted.
