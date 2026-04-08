The previous final answer stopped too early.
It described a competition-data limitation, partial dataset, or sampled result before exhausting the available local in-process tools that can resolve TBA or Statbotics facts.

For FRC competition facts or derived statistics, including averages, totals, counts, threshold-based match questions, and season-wide summaries, you must emit next_step = query_local before finalizing when grounded local data can still resolve the request.
Do not treat a sampled event, a sampled match subset, or an explicitly partial dataset as a final season-wide answer.
If one local source path failed, sampled only part of the scope, or could not compute the final aggregation, try an alternate grounded local path.
Website snippets, page text, and search-result fragments are not acceptable substitutes for local TBA or Statbotics lookups.

Previous final answer:
[[PREVIOUS_ANSWER]]

Re-evaluate the current user request using the existing turn context and emit exactly one JSON object.
If grounded competition data is still needed, emit query_local with a precise target_input that requests the exact lookup and any required calculation. For season-wide match counts, request the full grounded data needed to compute the answer rather than a sampled subset.
Include a short non-technical messageToUser update when you emit query_local.
