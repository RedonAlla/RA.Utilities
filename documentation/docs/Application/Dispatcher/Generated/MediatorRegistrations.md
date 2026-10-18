---
sidebar_position: 2
---

```powershell
Namespace: RA.Utilities.Dispatcher.Generated
```

`MediatorRegistrations` is the runtime-side queue that carries the registration of the **generated `Mediator`** — the source-generated `IDispatcher` implementation emitted into each of your assemblies. You never call it directly — the generator's module initializers enqueue into it, and `AddMediator()` drains it.

## 📦 Type Definition

```csharp
public static class MediatorRegistrations
{
    public static void Add(Action<IServiceCollection> registration);
    internal static void ApplyAll(IServiceCollection services);
}
```

## 🎯 How It Works

1. **At compile time**, the source generator emits the `Mediator` class (into `RA.Utilities.Dispatcher.Generated`, inside **your** assembly) together with a module initializer that queues its registration.
2. **At assembly load**, the module initializer runs and queues the callback.
3. **At startup**, `AddMediator()` applies the queued registration, so:
   - `IDispatcher` resolves to the generated `Mediator`,
   - `Mediator` is registered as itself, so it can be injected directly for the fastest path.

The queue is thread-safe (a lock guards a snapshot on apply).

:::note Assembly loading
Each referencing assembly queues its own generated `Mediator` — the registration of the **last loaded** assembly wins. Every assembly's generated mediator covers the messages visible to that assembly's compilation, including its referenced libraries. See [Generated Mediator](../generated-mediator) for the dispatch matrix.
:::

## 🔗 Related

- [Generated Mediator](../generated-mediator) — the dispatch strategy, object-based dispatch, and message/handler diagnostics (`DSAG006`–`DSAG009`).
- [`HandlerRegistrations`](HandlerRegistrations.md) — the sibling queue for handler and behavior registrations.
