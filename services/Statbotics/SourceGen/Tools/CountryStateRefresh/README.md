# CountryStateRefresh

On-demand console tool that maintainers run to refresh `vendor/statbotics-extras/country-state.json` from Statbotics' live `/v3/teams` endpoint.

## When to run

- After a new state or country gains an FRC team (rare, maybe once per year)
- If you observe `metric=` validation rejecting valid country/state values via repeated EventId 35 firings on values you expect to be legal
- As part of routine maintenance alongside `git submodule update --remote vendor/statbotics`

## How to run

```sh
dotnet run --project services/Statbotics/SourceGen/Tools/CountryStateRefresh

# or with custom output path:
dotnet run --project services/Statbotics/SourceGen/Tools/CountryStateRefresh -- \
  --output vendor/statbotics-extras/country-state.json
```

The tool paginates through `https://api.statbotics.io/v3/teams?limit=1000&offset=N` until it hits a short page or 50,000 teams (safety cap). Sorts both arrays alphabetically (ordinal) for deterministic diffs.

## After running

Review the diff and commit:

```sh
git diff vendor/statbotics-extras/country-state.json
git add vendor/statbotics-extras/country-state.json
git commit -m "chore: refresh Statbotics country/state snapshot"
```

The next `dotnet build` of `services/ChatBot/ChatBot.csproj` will pick up the new snapshot via the source generator's `<AdditionalFiles>` wiring.

## Failure modes

- **HTTP error**: tool exits with code 2 and writes the error to stderr. If it's a 5xx from Statbotics, retry. If it's a 429, the tool currently doesn't backoff — wait a minute and rerun.
- **JSON parse error**: probably indicates Statbotics changed the `/v3/teams` response shape. Inspect the raw response and update `Program.Team` if needed.
