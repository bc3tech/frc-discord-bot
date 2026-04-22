---
title: Builder-scoped DI extensions for harness-specific concerns
date: 2026-04-22
category: best-practices
module: discord-gpt-hosting
problem_type: best_practice
component: tooling
severity: medium
applies_when:
  - Adding a new feature package to the DiscordGpt SDK (storage, agent harness, transport, etc.)
  - Reviewing IServiceCollection extension methods that only make sense alongside DiscordGpt
  - The same concept has competing implementations and the user picks one (not many)
tags:
  - dependency-injection
  - api-design
  - builder-pattern
  - discord-gpt
  - extensibility
---

# Builder-scoped DI extensions for harness-specific concerns

## Context

DiscordGpt is composable: the core `AddDiscordGpt()` registration sets sensible defaults (in-memory conversation store, no AI harness), and feature packages layer on top — Foundry, GitHub Models, Azure Blob conversation store, Azure Table conversation store, etc.

Early extension methods were authored as `IServiceCollection` extensions (`services.AddBlobConversationStore(...)`, `services.AddTableConversationStore(...)`, `services.AddFoundryChatClient(...)`, `services.UseCopilot(...)`). That shape made the API self-defeating in two ways:

1. **It's a no-op without DiscordGpt.** A user who installs `BC3Technologies.DiscordGpt.Storage.Blob` and calls `services.AddBlobConversationStore(...)` without first calling `AddDiscordGpt()` ends up with a registration that nothing consumes. The method's surface area gives no hint that it's parasitic on the DiscordGpt registration.
2. **It implies "additive."** `Add*` reads like "register another one." But you don't run two conversation stores or two AI harnesses simultaneously — you pick one. The verb `With*` matches the actual semantic (replace the default), and chaining off the builder makes the dependency explicit.

The fix is a structural rule: **anything that only makes sense in the context of DiscordGpt (or any other harness this SDK might support) must hang off the relevant builder, not `IServiceCollection`.**

## Guidance

For every feature package that integrates with DiscordGpt:

- Public registration API is a `DiscordGptBuilder` extension method named `With{Feature}(...)`, returning `DiscordGptBuilder` for chaining.
- The extension lives in the package that provides the feature, but is placed in the `BC3Technologies.DiscordGpt.Hosting` namespace (use `#pragma warning disable IDE0130`) so callers find it without an extra `using`.
- The package's csproj references `BC3Technologies.DiscordGpt.Hosting`. If it doesn't already, add the project reference — the dependency is real, not an organizational accident.
- For "pick one of several implementations" registrations (conversation store, session store, AI harness, etc.), use `Services.Replace(ServiceDescriptor.Singleton<IInterface, TImpl>())`, not `AddSingleton`. The default registered by `AddDiscordGpt()` should be overridden, not stacked.
- Preserve a generic escape hatch (`WithConversationStore<TStore>()`) so users and tests can plug in their own implementation of the interface. Convenience `With{Feature}` wrappers are sugar over the generic primitive.
- Do **not** publish a parallel `IServiceCollection` extension. There is one way to register the feature.

```csharp
// In BC3Technologies.DiscordGpt.Storage.Blob/BlobConversationStoreBuilderExtensions.cs
#pragma warning disable IDE0130
namespace BC3Technologies.DiscordGpt.Hosting;

public static class BlobConversationStoreBuilderExtensions
{
    public static DiscordGptBuilder WithBlobConversationStore(
        this DiscordGptBuilder builder,
        Action<BlobConversationStoreOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (configure != null)
        {
            builder.Services.Configure(configure);
        }

        builder.Services.TryAddSingleton<
            IValidateOptions<BlobConversationStoreOptions>,
            BlobConversationStoreOptionsValidator>();
        builder.Services.Replace(
            ServiceDescriptor.Singleton<IConversationStore, BlobConversationStore>());

        return builder;
    }
}
```

Caller:

```csharp
services.AddDiscordGpt()
    .UseFoundry(opts => /* ... */)
    .WithTableConversationStore(o => o.TableName = "userChatThreads");
```

## Why This Matters

- **Discoverability.** IntelliSense after `AddDiscordGpt().` shows exactly what can be layered on. `services.` shows hundreds of unrelated extensions and gives no signal that DiscordGpt must be present first.
- **Correct-by-construction.** A user can't accidentally call `WithBlobConversationStore` without first calling `AddDiscordGpt()` — the receiver type forces the order.
- **Honest semantics.** `With*` says "configure the one DiscordGpt is using." `Add*` lies about being additive when only one will ever be resolved.
- **Future harnesses are pluggable.** If a second harness is ever added (e.g., `UseAgentFramework` as an alternative to `UseCopilot`), the same shape applies to its builder. The `IServiceCollection` namespace stays uncluttered and unambiguous.
- **Tests stay clean.** A test that wants a fake conversation store calls `.WithConversationStore<FakeStore>()` on the builder. No global service-collection plumbing.

## When to Apply

- Adding any new conversation store, session store, AI harness, transport, or other "pick one" component to the SDK.
- Adding configuration that only has meaning when DiscordGpt is registered (e.g., tools, skills, agents, prompts).
- Reviewing PRs that add `services.Add*` extensions named after a DiscordGpt feature — push back and request a builder extension instead.

Counter-cases (where `IServiceCollection` extensions are still correct):

- Registering shared infrastructure that has standalone value: `services.AddAzureClients(...)`, `services.AddSingleton<BlobServiceClient>(...)`. The caller is still responsible for these — `WithBlobConversationStore` does **not** register the underlying `BlobServiceClient` because that client may be used by other parts of the host.

## Examples

Before (rejected):

```csharp
services.AddDiscordGpt();
services.AddBlobConversationStore(o => o.ContainerName = "chats");
services.WithConversationStore<BlobConversationStore>();  // had to chain twice
```

After:

```csharp
services.AddDiscordGpt()
    .WithBlobConversationStore(o => o.ContainerName = "chats");
```

Same rule applied to the Copilot harness:

```csharp
// Before — looked like a global service-collection concern
services.UseCopilot(...)
        .AddFoundryChatClient(...)
        .AddTool<MyTool>();

// After — chained off the builder, where it belongs
services.AddDiscordGpt()
    .UseCopilot()
    .UseFoundry(opts => /* ... */)
    .WithTool<MyTool>()
    .WithSkill<MySkill>();
```

Generic escape hatch preserved for custom implementations and unit tests:

```csharp
services.AddDiscordGpt()
    .WithConversationStore<MyCustomStore>();
```

## Related

- `gpt/src/BC3Technologies.DiscordGpt.Hosting/DiscordGptBuilderToolExtensions.cs` — `WithConversationStore<T>()` generic primitive (line 53) and `WithInMemoryConversationStore()` (line 70).
- `gpt/src/BC3Technologies.DiscordGpt.Storage.Blob/BlobConversationStoreBuilderExtensions.cs` — reference implementation.
- `gpt/src/BC3Technologies.DiscordGpt.Storage.TableStorage/TableConversationStoreBuilderExtensions.cs` — mirror.
- Session storage (`WithBlobSessionStorage`) follows the same shape; will be revisited in a follow-up pass.
