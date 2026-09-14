---
sidebar_position: 2
---

```powershell
Namespace: RA.Utilities.Feature.Generated
```

`MediatorRegistrations` is the runtime-side queue that the **mediator source generator** feeds: every assembly referencing the package emits a module initializer that calls `MediatorRegistrations.Add(...)` with the DI registrations of its generated `MediatorImpl`.

## 🎯 Purpose

The generated `MediatorImpl` lives in *your* assembly, so the runtime package cannot reference it directly. The queue bridges that gap:

1. At assembly load, the generated module initializer queues a registration callback (`AddScoped<MediatorImpl>()` plus `AddScoped<IMediator>(...)`).
2. Your startup code calls `services.AddMediator()`.
3. `AddMediator` applies the queued callbacks, so `IMediator` resolves to the generated implementation.

## ⚙️ Members

```csharp showLineNumbers
public static class MediatorRegistrations
{
    /// <summary>Queues a mediator-registration callback. Called by generated module initializers.</summary>
    public static void Add(Action<IServiceCollection> registration);

    /// <summary>Applies all queued registrations. Called by AddMediator().</summary>
    internal static void ApplyAll(IServiceCollection services);
}
```

You never call `Add` yourself — it is for the generated initializer. `ApplyAll` is invoked by [`AddMediator()`](../Extensions/MediatorServiceCollectionExtensions.md).

## 🧠 Summary

`MediatorRegistrations` is the plumbing between the generated `MediatorImpl` and `AddMediator()`. Its sibling [`HandlerRegistrations`](./HandlerRegistrations.md) carries the handler and behavior registrations through the same pattern. See the [Generated Mediator](../generated-mediator) guide for the full dispatch story.
