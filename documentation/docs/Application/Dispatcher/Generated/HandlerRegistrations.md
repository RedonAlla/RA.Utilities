---
sidebar_position: 1
---

```powershell
Namespace: RA.Utilities.Dispatcher.Generated
```

`HandlerRegistrations` is the runtime-side queue that carries the compile-time registrations of your handlers and closed pipeline behaviors. You never call it directly — the source generator's module initializers enqueue into it, and `AddDispatcher()` drains it.

## 📦 Type Definition

```csharp
public static class HandlerRegistrations
{
    public static void Add(Action<IServiceCollection> registration);
    internal static void ApplyAll(IServiceCollection services, DispatcherOptions? options = null);
}
```

## 🎯 How It Works

1. **At compile time**, the package's source generator emits a module initializer per assembly containing `IRequestHandler<,>` / `IRequestHandler<>` / `INotificationHandler<>` implementations or closed behaviors. The initializer calls `HandlerRegistrations.Add(...)` with a callback that registers:
   - handlers under their interfaces **and as themselves** (concrete type, for the generated dispatcher's direct injection),
   - closed behaviors under their behavior interfaces.
2. **At assembly load**, the module initializer runs and queues the callback.
3. **At startup**, `AddDispatcher()` calls `ApplyAll`, applying every queued registration to the `IServiceCollection` in one pass.

The overload `AddDispatcher(Action<DispatcherOptions> configure)` passes the options to `ApplyAll`, which then post-filters the descriptors added in this pass: handlers listed in [`ExcludeHandlers`](../Options/DispatcherOptions.md) and everything not listed in [`IncludeHandlers`](../Options/DispatcherOptions.md) are removed, together with the behaviors related to messages that no longer have a registered handler. Registrations added to the collection outside this queue are never removed.

The queue is thread-safe (a lock guards a snapshot on apply).

## ⏳ Lifetimes

| Registration | Lifetime |
|---|---|
| `IRequestHandler<TRequest, TResponse>` → handler | Scoped |
| `IRequestHandler<TRequest>` → handler | Scoped |
| `INotificationHandler<TNotification>` → handler | Transient |
| Handler as its concrete type | Same as the interface registration |
| Closed behavior under its interface | Transient |

## 🔗 Related

- [Auto Registration](../auto-registration) — the discovery rules, diagnostics (`DSAG001`–`DSAG005`, `DSAG010`), and the assembly-loading guarantee.
- [Filtering auto-registered handlers](../auto-registration#filtering-auto-registered-handlers) — how `IncludeHandlers` / `ExcludeHandlers` are applied.
- [`DispatcherOptions`](../Options/DispatcherOptions.md) — the filter options type.
- [`DispatcherRegistrations`](DispatcherRegistrations.md) — the sibling queue for the generated `Dispatcher`.
