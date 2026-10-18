---
sidebar_position: 2
---

```powershell
Namespace: RA.Utilities.Dispatcher.Extensions
```

The `MediatorServiceCollectionExtensions` class registers the core `IDispatcher` **service with the .NET dependency injection container** and **applies the compile-time handler registrations** produced by the package's source generator.

It provides a simple, one-line extension method, `AddMediator`, that handles both. The method is written with a **C# 14 extension block** — it lives inside `extension(IServiceCollection services)` rather than using the classic `this` parameter.

```csharp showLineNumbers
// From: /RA.Utilities/Application/RA.Utilities.Dispatcher/Extensions/MediatorServiceCollectionExtensions.cs

public static class MediatorServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddMediator()
        {
            HandlerRegistrations.ApplyAll(services);
            MediatorRegistrations.ApplyAll(services);
            return services;
        }
    }
}
```

The source generator shipped in the package emits a module initializer for every assembly containing `IRequestHandler<,>` / `IRequestHandler<>` / `INotificationHandler<>` implementations and closed pipeline behaviors. Those initializers queue DI registrations (request handlers **scoped**, notification handlers and behaviors **transient**) which `AddMediator()` then applies — that is why plain handlers need no explicit `AddDispatcher` / `AddNotification` call. The generator also emits the **`Mediator` implementation of `IDispatcher`** per assembly — the only dispatcher in the package — and `AddMediator()` applies its registration. See the [Auto Registration](../auto-registration) and [Generated Mediator](../generated-mediator) guides for the assembly-loading guarantee, dispatch strategy, and diagnostics.

## Why is this important?

### 1. Simplifies Setup:
It provides a clean, discoverable way for a developer using your library to get it working.
Instead of needing to know the concrete implementation (`Mediator`), its correct lifetime (`Scoped`), or the auto-registration queue, they can simply call `services.AddMediator()`.

### 2. Encapsulation:
It hides the implementation details of the mediator pattern.
If you were to change the concrete `Mediator` class (or the registration mechanics) in the future, developers using your library wouldn't need to change their startup code as long as the extension method remains.

### 3. Foundation for Features:
This registration is the foundation upon which all other features from `RA.Utilities.Dispatcher` are built.
The source generator and the fluent builders (`.AddDispatcher()`, `.AddNotification()`) register the handlers and behaviors, while `AddMediator` registers the central "dispatcher" that actually processes the requests and notifications — and applies the auto-discovered handlers.

In short, this class is the essential bootstrapping step required to use the CQRS pattern provided by the RA.Utilities.Dispatcher library.
