---
sidebar_position: 1
---

```powershell
Namespace: RA.Utilities.Dispatcher.Options
```

`DispatcherOptions` configures the [`AddDispatcher(Action<DispatcherOptions>)`](../Extensions/DispatcherServiceCollectionExtensions.md) overload. It filters the registrations produced by the source generator **before they take effect** — the generated module initializer still queues everything, and the filter removes the descriptors that belong to excluded handlers (and to the pipeline/notification behaviors related to their messages).

Nothing is filtered by default: when both properties are `null`, every discovered handler and closed behavior registers exactly as described in [Auto Registration](../auto-registration).

## 📦 Type Definition

```csharp
public sealed class DispatcherOptions
{
    public List<Type>? IncludeHandlers { get; set; }
    public List<Type>? ExcludeHandlers { get; set; }
}
```

## 🎯 Member Breakdown

| Property | Type | Default | Description |
|---|---|---|---|
| `IncludeHandlers` | `List<Type>?` | `null` (ignored) | When set, only the listed handlers — and the pipeline/notification behaviors related to the messages they handle — are registered. |
| `ExcludeHandlers` | `List<Type>?` | `null` (ignored) | When set, the listed handlers are not registered, and neither are the behaviors related to a message once no other registered handler handles it. |

### Rules

- **Exclusions win** over inclusions when a type is in both lists.
- **Behavior types are accepted** in either list. A listed behavior is kept even when the handler of its message is filtered out (`IncludeHandlers`), and removed even when its handler is kept (`ExcludeHandlers`).
- A **request behavior** is removed together with the handler of its request. A behavior whose request has no handler in the compilation at all is left alone unless `IncludeHandlers` restricts registration.
- A **notification behavior** is removed only once no registered handler for its notification remains.
- The filter applies to **source-generated registrations only**. Handlers registered explicitly with `AddDispatcher<TRequest, TResponse, THandler>()` / `AddNotification<TNotification>()` and behaviors added with `AddDecoration<TBehavior>()` are untouched.
- Include and exclude are evaluated against the **handler's implementation type**, not the closed interface type. Registering the same handler twice (automatically and explicitly) leaves the explicit registration in place.

## 🚀 Complete Example

```csharp showLineNumbers
using RA.Utilities.Dispatcher.Extensions;
using RA.Utilities.Dispatcher.Options;

// Register every discovered handler except the legacy importer, and only the
// order slice in a trimmed-down worker host:
builder.Services.AddDispatcher(options =>
{
    options.IncludeHandlers = [typeof(CreateOrderHandler), typeof(GetOrderQueryHandler)];
    options.ExcludeHandlers = [typeof(LegacyImportHandler)];
});

// Default: register everything the generator discovered.
builder.Services.AddDispatcher();
```

With the configuration above, `CreateOrderHandler` and `GetOrderQueryHandler` (plus the behaviors bound to their messages) register; `LegacyImportHandler` and any handler not listed do not. A behavior listed in `ExcludeHandlers` is removed even if its message's handler survives.

## 🔗 Related

- [Auto Registration](../auto-registration) — discovery rules, lifetimes, and the assembly-loading guarantee.
- [Filtering auto-registered handlers](../auto-registration#filtering-auto-registered-handlers) — the same rules in guide form.
- [`DispatcherServiceCollectionExtensions`](../Extensions/DispatcherServiceCollectionExtensions.md) — the `AddDispatcher` overloads that accept these options.
