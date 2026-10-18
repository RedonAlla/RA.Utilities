---
sidebar_position: 1
---

```powershell
Namespace: RA.Utilities.Dispatcher.Generated
```

`HandlerRegistrations` is the runtime-side queue that carries the compile-time registrations of your handlers and closed pipeline behaviors. You never call it directly — the source generator's module initializers enqueue into it, and `AddMediator()` drains it.

## 📦 Type Definition

```csharp
public static class HandlerRegistrations
{
    public static void Add(Action<IServiceCollection> registration);
    internal static void ApplyAll(IServiceCollection services);
}
```

## 🎯 How It Works

1. **At compile time**, the package's source generator emits a module initializer per assembly containing `IRequestHandler<,>` / `IRequestHandler<>` / `INotificationHandler<>` implementations or closed behaviors. The initializer calls `HandlerRegistrations.Add(...)` with a callback that registers:
   - handlers under their interfaces **and as themselves** (concrete type, for the generated mediator's direct injection),
   - closed behaviors under their behavior interfaces.
2. **At assembly load**, the module initializer runs and queues the callback.
3. **At startup**, `AddMediator()` calls `ApplyAll`, applying every queued registration to the `IServiceCollection` in one pass.

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
- [`MediatorRegistrations`](MediatorRegistrations.md) — the sibling queue for the generated `Mediator`.
