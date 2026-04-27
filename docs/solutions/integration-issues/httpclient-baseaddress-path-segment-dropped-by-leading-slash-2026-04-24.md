---
title: HttpClient.BaseAddress path segment dropped when relative URI starts with "/"
date: 2026-04-24
category: integration-issues
module: services/ChatBot/Tools
problem_type: integration_issue
component: assistant
symptoms:
  - "TBA requests hitting host root (https://www.thebluealliance.com/team/frc2046) instead of /api/v3/team/frc2046, returning 404"
  - "Statbotics calls similarly bypassing the /v3 prefix"
  - "Chat bot tools returning empty/error data despite plausible-looking URLs in logs"
root_cause: wrong_api
resolution_type: code_fix
severity: high
tags: [httpclient, base-address, uri-composition, tba, statbotics, http-tools]
---

# HttpClient.BaseAddress path segment dropped when relative URI starts with "/"

## Problem
The chat bot's HTTP tools (TBA, Statbotics) were silently sending requests to the host root instead of under the API path prefix. URLs that *looked* correct in logs (because we logged the relative path) were resolving against the wrong absolute URL, returning 404 or empty results.

## Symptoms
- `GET /team/frc2046` going to `https://www.thebluealliance.com/team/frc2046` instead of `https://www.thebluealliance.com/api/v3/team/frc2046`.
- Tool responses indicating success at the HTTP layer but containing no useful data, or 404s.
- Agent answers like "I don't have data for that team" despite the team obviously existing.

## What Didn't Work
- **Reading the relative URL in logs.** The log line printed the relative target (`/team/frc2046`) which matched what we intended — masked the fact that the actual request URI lost the `/api/v3` segment.
- **Assuming `new Uri(base, relative)` would always concatenate.** It doesn't — see Why This Works.

## Solution
Two coordinated changes in production code; tests updated to match.

**1. Trailing slash on the `BaseAddress`** in `services/ChatBot/DependencyInjectionExtensions.cs`:

```csharp
// Before
client.BaseAddress = new Uri("https://www.thebluealliance.com/api/v3");

// After
client.BaseAddress = new Uri("https://www.thebluealliance.com/api/v3/");
```

Same change for the Statbotics client (`https://api.statbotics.io/v3/`).

**2. Strip a leading `/` from request targets** in `services/ChatBot/Tools/HttpGetToolBase.cs` before constructing the relative `Uri`:

```csharp
if (client.BaseAddress is not null && requestTarget.StartsWith('/'))
{
    requestTarget = requestTarget[1..];
}
var requestUri = new Uri(requestTarget, UriKind.Relative);
```

Tests in `tests/FunctionApp.Tests/HttpGetToolBaseTests.cs` were updated:
- Assertions on `apiRequest.path` no longer expect a leading `/`.
- Stub route matchers in `RoutingHttpMessageHandler` now compare against the full `/api/v3/...` AbsolutePath (because the corrected URL composition now preserves the base path).

## Why This Works
`new Uri(baseAddress, relativeUri)` follows RFC 3986 reference resolution. Two rules combine to bite you:

1. **A `BaseAddress` without a trailing slash treats its last segment as a "file"**, not a directory. Resolving anything against it replaces that last segment. So `new Uri("https://x.com/api/v3", "team/frc2046")` becomes `https://x.com/api/team/frc2046` — the `v3` segment is dropped.
2. **A relative URI starting with `/` is an absolute-path reference**, which replaces the entire path of the base. So `new Uri("https://x.com/api/v3/", "/team/frc2046")` becomes `https://x.com/team/frc2046` — the whole `/api/v3` prefix is gone.

You need *both* fixes to be safe: trailing slash on the base **and** no leading slash on the relative. The base-address fix alone still breaks if a caller passes `/team/frc2046`; the strip-leading-slash fix alone still breaks if the base lacks a trailing slash. Together, the relative path always resolves *under* the base path.

## Prevention
- **When configuring an `HttpClient.BaseAddress` that includes a path segment, always end it with `/`.** Treat it as a directory, not a file.
- **In any helper that builds relative request URIs, strip a leading `/` before constructing the `Uri`.** Callers naturally write `/team/frc2046` and shouldn't have to know about this footgun.
- **In tests, assert the full `AbsolutePath`** (or use a `RoutingHttpMessageHandler` keyed on it), not just the relative target. The bug we hit was invisible to assertions that only inspected the request *target string* — the test stubs and the production code agreed on the wrong thing.

Concrete test guard:

```csharp
// In a stub HttpMessageHandler, route by AbsolutePath so a regression
// that drops the base path immediately fails to match.
return request.RequestUri!.AbsolutePath switch
{
    "/api/v3/team/frc2046/events/2024/simple" => OkJson(events),
    _ => NotFound(),
};
```

## Related Issues
- None previously documented in this repo. This pattern affects any `HttpClient` with a path-bearing `BaseAddress` — worth grepping for `new Uri(...api...)` without a trailing slash if more API clients are added.
