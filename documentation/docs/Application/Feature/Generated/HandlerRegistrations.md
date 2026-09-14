---
sidebar_position: 1
---

```powershell
Namespace: RA.Utilities.Feature.Generated
```

`HandlerRegistrations` is the runtime-side queue that the **handler registration source generator** feeds: every assembly referencing the package emits a module initializer that queues the DI registrations of its handlers and closed pipeline behaviors.

## 🎯 Purpose

Handlers and behaviors live in *your* assemblies, so the runtime package cannot reference them directly. The queue bridges that gap:

1. At assembly load, the generated module initializer queues one registration callback for the assembly.
2. Your startup code calls `services.AddMediator()`.
3. `AddMediator` applies every queued callback, registering:
   - request handlers — **scoped**, under `IRequestHandler<...>` **and** as themselves,
   - notification handlers — **transient**, under `INotificationHandler<...>` **and** as themselves,
   - closed pipeline behaviors (`IPipelineBehavior<,>`, `IPipelineBehavior<>`, `INotificationBehavior<>`) — **transient**, as themselves only, so the generated mediator can inject them directly by concrete type.

## ⚙️ Members

```csharp showLineNumbers
public static class HandlerRegistrations
{
    /// <summary>Queues a handler-registration callback. Called by generated module initializers.</summary>
    public static void Add(Action<IServiceCollection> registration);

    /// <summary>Applies all queued registrations. Called by AddMediator().</summary>
    internal static void ApplyAll(IServiceCollection services);
}
```

You never call `Add` yourself — it is for the generated initializer. See the [Auto Registration](../auto-registration) guide for the discovery rules, lifetimes, and the `FEAG001`–`FEAG005` diagnostics.

## 🧠 Summary

`HandlerRegistrations` carries the zero-config registrations from the source generator to `AddMediator()`. Its sibling [`MediatorRegistrations`](./MediatorRegistrations.md) carries the generated mediator registration through the same pattern.
