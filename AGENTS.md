# AGENTS.md — Custom Agent Personas and Invocation Patterns

This document catalogues custom agents, their roles, invocation patterns, and behavioral boundaries for the FRC Discord Bot project.

## Purpose

Agents are specialized personas that perform focused tasks: code review, research, document analysis, design verification, and debugging. This file defines:
- **Which agents exist** and what they do
- **When to invoke each agent** and whether they're user-invocable or framework-triggered
- **How agents interact** with each other (scoping, conflict resolution, sequencing)
- **What agents won't do** (explicit non-goals to reduce redundancy)

Agents read this file to understand their role and scope. Contributors use it to know which agent to invoke or expect for a given task.

---

## Meta-Rules: Agent Interaction & Boundaries

### Scoping & Non-Goals

1. **No overlapping invasiveness.** If multiple agents could address a finding, assign it to the agent with the narrowest scope:
   - `correctness-reviewer` flags logic bugs with high confidence
   - `adversarial-reviewer` escalates only when the diff is large (≥50 lines) OR touches high-risk domains (auth, payments, data, external APIs)
   - `security-reviewer` focuses only on exploitable paths, not generic hardening advice

2. **Conditional triggers are exclusive.** Agents with conditional activation patterns (marked "Conditional" below) activate when their specific conditions are met. Multiple conditional agents can activate for the same diff if conditions overlap; they coexist without conflict:
   - `security-reviewer` activates on auth/endpoint changes
   - `performance-reviewer` activates on database/loop changes
   - Both can activate on the same diff if it touches both areas

3. **Always-on agents run by default** and provide baseline coverage:
   - `correctness-reviewer`
   - `maintainability-reviewer`
   - `testing-reviewer`
   - `project-standards-reviewer`

### Invocation Patterns

**User-invocable agents** can be directly requested via `/invoke <agent-name>`. Example: `/invoke best-practices-researcher`.

**Framework-triggered agents** activate automatically under specific conditions. Example: `security-reviewer` runs automatically on PRs touching auth code.

**Document-review spawned agents** are invoked by the `document-review` skill on planning documents. They do not run independently. Example: `feasibility-reviewer` is spawned by document-review to validate a plan.

### Conflict Resolution

If two agents surface contradictory findings:
1. **Always-on agents take priority over conditional agents.** If `correctness-reviewer` and `adversarial-reviewer` disagree on severity, trust `correctness-reviewer` (always-on).
2. **Specificity wins over generality.** `security-reviewer` (auth-specific) overrules `adversarial-reviewer` (general hardening) on auth findings.
3. **Research agents inform but don't override.** `best-practices-researcher` surfaces conventions; code reviewers apply judgment on whether to adopt them.

### Output Expectations

- **Code review agents** return JSON findings with severity, confidence, and remediation guidance
- **Research agents** provide synthesized guidance with source attribution and implementation examples
- **Document review agents** surface specific gaps against rubrics, with actionable questions
- **Specialized verifiers** (design-sync, bug-validator) provide pass/fail or visual diffs with remediation steps

---

## Agent Catalog

### Always-On Code Review Personas

These run on every code review by default, providing baseline coverage.

| Agent | Purpose | Scope | Triggers |
|-------|---------|-------|----------|
| **correctness-reviewer** | Logic errors, edge cases, state management bugs, error propagation failures | Any code change | Always on |
| **maintainability-reviewer** | Premature abstraction, unnecessary indirection, dead code, coupling, naming clarity | Any code change | Always on |
| **testing-reviewer** | Test coverage gaps, weak assertions, brittle tests, missing edge cases | Any code change | Always on |
| **project-standards-reviewer** | Compliance with repo standards (CLAUDE.md, AGENTS.md, naming, portability, tool use) | Any code change | Always on |

**When to suppress:** All-on agents suppress findings below confidence 0.60. Do not invoke manually unless requesting a secondary pass on a specific concern.

---

### Conditional Code Review Personas

These activate automatically when their trigger conditions are met. User-invocable to force evaluation on any diff.

| Agent | Purpose | Activation Trigger | Scope |
|-------|---------|-------------------|-------|
| **security-reviewer** | Exploitable vulnerabilities: injections, auth bypasses, secrets, SSRF, deserialization | Diff touches auth, endpoints, input handling, permissions | Find attack paths; suppress generic hardening |
| **adversarial-reviewer** | Actively constructs failure scenarios; stress-tests impl | Diff ≥50 lines OR touches auth, payments, mutations, external APIs | Large diffs; high-risk domains |
| **performance-reviewer** | Database queries, loop-heavy transforms, caching, I/O paths | Diff touches queries, loops, caches, I/O | Trace algorithmic complexity & resource usage |
| **data-migrations-reviewer** | Data integrity & migration safety in migrations, schemas, transforms | Diff touches migrations, schema changes, backfills | Verify data consistency & rollback safety |
| **api-contract-reviewer** | Breaking changes in routes, types, serialization, versioning | Diff touches API routes, request/response types, exports | Check contract stability & versioning |
| **reliability-reviewer** | Error handling, retries, timeouts, health checks, background jobs, async | Diff touches error paths, retries, timeouts, async | Verify production failure modes & graceful degradation |
| **cli-readiness-reviewer** | CLI agent-friendliness (commands, args, output structure, idempotency) | Diff touches CLI command definitions, args, handlers | Ensure agents can use the CLI reliably |

**When multiple activate:** All applicable conditional agents run. Their findings coexist; no conflict resolution needed unless they contradict (rare — see Conflict Resolution above).

---

### Specialized Code Review Personas

User-invocable for targeted analysis.

| Agent | Purpose | When to Use |
|-------|---------|------------|
| **security-sentinel** | Comprehensive security audit: vulnerabilities, input validation, OWASP compliance | Before deployment; before exposing new endpoints |
| **code-simplicity-reviewer** | YAGNI violations, over-engineering, minimal viable code | After implementation complete; before finalizing a feature |
| **agent-native-reviewer** | Ensures agent parity: any user action should be possible for an agent | After adding UI features, agent tools, or system prompts |
| **previous-comments-reviewer** | Verifies prior PR feedback has been addressed in current diff | When iterating on a PR with existing review comments |
| **pattern-recognition-specialist** | Design patterns, anti-patterns, naming conventions, duplication | When checking codebase consistency; verifying established patterns |

---

### Document Review Personas

Spawned by the `document-review` skill on planning documents. Do not invoke directly.

| Agent | Purpose | Invoked By | Output |
|-------|---------|-----------|--------|
| **coherence-reviewer** | Internal consistency: contradictions, terminology drift, structural issues | document-review | Specific inconsistencies & remediation |
| **feasibility-reviewer** | Survivability: architecture conflicts, dependency gaps, implementability | document-review | Risk assessment & implementation concerns |
| **scope-guardian-reviewer** | Scope alignment: unjustified complexity, unnecessary abstractions | document-review | Scope violations & simplification opportunities |
| **security-lens-reviewer** | Security gaps at plan level: auth/authz assumptions, data exposure, threat model | document-review | Threat model gaps & architectural mitigations |
| **design-lens-reviewer** | Design completeness: information architecture, interaction states, flows, AI slop | document-review | Design gaps & user flow issues |
| **product-lens-reviewer** | Product strategy: premise validation, goal alignment, strategic consequences | document-review | Premise challenges & goal misalignment |
| **adversarial-document-reviewer** | Conditional persona for high-stakes documents (>5 requirements, major decisions) | document-review | Stress-tested assumptions & architectural flaws |

---

### Research & Analysis Agents

User-invocable for research, onboarding, and forensic analysis.

| Agent | Purpose | When to Use |
|-------|---------|------------|
| **best-practices-researcher** | External best practices, industry standards, framework conventions, implementation examples | Need guidance on standards, conventions, or implementation patterns |
| **framework-docs-researcher** | Framework & library documentation, version constraints, breaking changes | Need version-specific implementation guidance |
| **repo-research-analyst** | Repository structure, documentation, conventions, patterns (onboarding) | New to codebase; need structure & pattern overview |
| **git-history-analyzer** | Git archaeology: code evolution, contributors, why patterns exist | Understanding historical context for code decisions |
| **learnings-researcher** | Institutional knowledge: search docs/solutions/ by metadata | Before implementing features or fixing problems (avoid repeated mistakes) |
| **issue-intelligence-analyst** | GitHub issue analysis: recurring themes, pain patterns, severity trends | Understanding project issue landscape or user pain points |

---

### Design & UX Agents

User-invocable for design verification and iteration.

| Agent | Purpose | When to Use |
|-------|---------|------------|
| **design-implementation-reviewer** | Visual comparison: live implementation vs. Figma design; discrepancy feedback | After writing/modifying HTML/CSS/React; verify design fidelity |
| **design-iterator** | Iterative refinement: screenshot-analyze-improve cycles | When design changes stall after 1-2 attempts; iterative refinement desired |
| **figma-design-sync** | Auto-detects & fixes visual differences between web impl. and Figma | Iteratively syncing implementation to Figma specs |
| **spec-flow-analyzer** | Specification analysis: user flows, edge cases, requirements gaps | When spec/plan needs flow analysis or gap identification |

---

### Specialized Verifiers & Validators

User-invocable for debugging and deployment validation.

| Agent | Purpose | When to Use |
|-------|---------|------------|
| **bug-reproduction-validator** | Systematically reproduces & validates bug reports | When receiving a bug report needing verification |
| **architecture-strategist** | Architectural compliance: pattern adherence, design integrity, coupling/cohesion | Review PRs for architectural alignment; evaluate refactors |
| **performance-oracle** | Performance analysis: bottlenecks, algorithmic complexity, memory, scalability | After implementation; when performance concerns arise |
| **cli-agent-readiness-reviewer** | Severity-based rubric: CLI agent optimization (not just usability) | When assessing whether CLI is truly agent-optimized |

---

### Automation & Maintenance Agents

Framework-triggered or manually invoked for background tasks.

| Agent | Purpose | Trigger |
|-------|---------|---------|
| **pattern-observer** | Lightweight: analyzes recent tool use & code changes to identify emerging patterns | Automatic or on-demand; feeds learning pipeline |
| **pr-comment-resolver** | Resolves PR review threads: assesses validity, implements fixes, returns summaries | Spawned by resolve-pr-feedback skill |
| **lint** | Runs project linters and reports violations | Framework-triggered on code changes |

---

### Documentation & Writing Agents

User-invocable for documentation creation and updates.

| Agent | Purpose | When to Use |
|-------|---------|------------|
| **ankane-readme-writer** | Creates/updates README following Ankane style (Ruby gems) | Writing gem documentation with imperative voice & standard sections |

---

## Invocation Quick Reference

### "I need to review this code"
- **Default:** Let the always-on agents run (correctness, maintainability, testing, standards)
- **High-risk code:** Invoke `adversarial-reviewer` or `security-sentinel`
- **Specific domain:** Invoke conditional agent (security, performance, reliability, data-migrations, api-contract)

### "I need to review a plan or spec"
- **Always:** Use the `document-review` skill (spawns coherence, feasibility, scope, security-lens, design-lens, product-lens)
- **High-stakes document:** document-review spawns `adversarial-document-reviewer` automatically

### "I need research or guidance"
- **Industry standards:** `/invoke best-practices-researcher`
- **Framework-specific:** `/invoke framework-docs-researcher`
- **Onboarding:** `/invoke repo-research-analyst`
- **Historical context:** `/invoke git-history-analyzer`
- **Lessons learned:** `/invoke learnings-researcher`
- **Issue patterns:** `/invoke issue-intelligence-analyst`

### "I need to verify design or UX"
- **Compare to Figma:** `/invoke design-implementation-reviewer`
- **Iterative refinement:** `/invoke design-iterator`
- **Auto-sync Figma:** `/invoke figma-design-sync`
- **Flow/spec gaps:** `/invoke spec-flow-analyzer`

### "I need specialized validation"
- **Reproduce a bug:** `/invoke bug-reproduction-validator`
- **Architecture check:** `/invoke architecture-strategist`
- **Performance analysis:** `/invoke performance-oracle`
- **CLI agent-readiness:** `/invoke cli-agent-readiness-reviewer`

---

## Non-Goals & Exclusions

### What Agents Do NOT Do

- **Agents do not argue about style.** Styling & formatting is handled by `.editorconfig` and linters, not agents.
- **Agents do not suggest generic hardening.** "Consider adding rate limiting" without a specific exploitable finding is not a code review comment; it's architecture guidance for planning.
- **Agents do not flag defensive code that can't actually fail.** "This value can't be null" is not a bug if the code path proves it can't be.
- **Agents do not debug specific runtime issues.** That's what logs, debuggers, and test runs are for.
- **Agents do not replace human domain expertise.** For business logic, user experience, or strategic decisions, agent guidance informs but does not override human judgment.

---

## Common Workflows

### Code Review Workflow

1. **Every PR triggers always-on agents** (correctness, maintainability, testing, standards)
2. **Conditional agents auto-trigger** if their activation conditions match
3. **User can force additional agents** (e.g., `/invoke security-sentinel` for extra scrutiny)
4. **Prior-comments reviewer** runs if the PR has existing review comments
5. **Code simplicity reviewer** runs as a final pass before merge if requested

### Planning & Requirements Workflow

1. **User drafts requirements in /ce-brainstorm**
2. **Brainstorm produces a requirements document**
3. **User runs /ce-plan or document-review**
4. **document-review spawns coherence, feasibility, scope, and domain-specific reviewers**
5. **Agents surface gaps; user iterates**
6. **Planning proceeds once Resolve Before Planning questions are answered**

### Feature Implementation Workflow

1. **Implement feature following plan**
2. **All-on code reviewers run automatically**
3. **Conditional agents activate per trigger conditions**
4. **User can invoke security-sentinel or code-simplicity-reviewer for polish**
5. **If design component: use design-iterator for refinement**
6. **Merge once all agents are satisfied**

---

## References

- **copilot-instructions.md** — Developer principles, engineering philosophy, communication style
- **CLAUDE.md** — (if retained) Language/framework-specific guidance
- **.github/skills/** — Compound engineering workflows (brainstorm, plan, work, review)
- **.github/agents/** — Agent implementations (read for specific personas)

