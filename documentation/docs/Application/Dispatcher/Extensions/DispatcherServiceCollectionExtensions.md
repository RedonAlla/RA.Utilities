---
sidebar_position: 2
---

```powershell
Namespace: RA.Utilities.Dispatcher.Extensions
```

The `DispatcherServiceCollectionExtensions` class registers the core `IDispatcher` **service with the .NET dependency injection container** and **applies the compile-time handler registrations** produced by the package's source generator.

It provides two `AddDispatcher` extension methods — a parameterless one that applies every auto-registration, and one that takes [`DispatcherOptions`](../Options/DispatcherOptions.md) to filter which handlers are registered. Both are written with a **C# 14 extension block** — they live inside `extension(IServiceCollection services)` rather than using the classic `this` parameter.

```csharp showLineNumbers
// From: /RA.Utilities/Application/RA.Utilities.Dispatcher/Extensions/DispatcherServiceCollectionExtensions.cs

public static class DispatcherServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddDispatcher()
        {
            ArgumentNullException.ThrowIfNull(services);

            return AddDispatcherCore(services, options: null);
        }

        public IServiceCollection AddDispatcher(Action<DispatcherOptions> configure)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(configure);

            var options = new DispatcherOptions();
            configure(options);

            return AddDispatcherCore(services, options);
        }
    }

    private static IServiceCollection AddDispatcherCore(
        IServiceCollection services, DispatcherOptions? options)
    {
        HandlerRegistrations.ApplyAll(services, options);
        DispatcherRegistrations.ApplyAll(services);

        return services;
    }
}
```

The source generator shipped in the package emits a module initializer for every assembly containing `IRequestHandler<,>` / `IRequestHandler<>` / `INotificationHandler<>` implementations and closed pipeline behaviors. Those initializers queue DI registrations (request handlers **scoped**, notification handlers and behaviors **transient**) which `AddDispatcher()` then applies — that is why plain handlers need no explicit `AddDispatcher<...>` / `AddNotification` call. The generator also emits the **`Dispatcher` implementation of `IDispatcher`** per assembly — the only dispatcher in the package — and `AddDispatcher()` applies its registration. See the [Auto Registration](../auto-registration) and [Generated Dispatcher](../generated-dispatcher) guides for the assembly-loading guarantee, dispatch strategy, and diagnostics.

## `AddDispatcher()`

Applies every queued handler, behavior, and dispatcher registration. This is the call that bootstraps the package — without it there is no `IDispatcher`, and `Send`/`Publish` fail at runtime.

```csharp
using RA.Utilities.Dispatcher.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddDispatcher();
```

## `AddDispatcher(Action<DispatcherOptions> configure)`

Applies the queued registrations **filtered by the supplied options**. Filtering affects source-generated registrations only — handlers registered explicitly, through the fluent builders, or through plain DI calls are untouched.

```csharp
using RA.Utilities.Dispatcher.Extensions;

builder.Services.AddDispatcher(options =>
{
    options.IncludeHandlers = [typeof(CreateOrderHandler)];
    options.ExcludeHandlers = [typeof(LegacyOrderImportHandler)];
});
```

Internally, `HandlerRegistrations.ApplyAll(services, options)` applies the filter, so the queue is drained **and** filtered in one pass; `DispatcherRegistrations.ApplyAll(services)` then registers the generated `Dispatcher` itself, which is never filtered. See [Filtering auto-registered handlers](../auto-registration#filtering-auto-registered-handlers) for the inclusion/exclusion rules and how behaviors are removed with their messages.

## Parameters

| Method | Parameter | Description |
|---|---|---|
| `AddDispatcher()` | — | No parameters. Applies all auto-registrations. |
| `AddDispatcher(Action<DispatcherOptions>)` | `configure` | Receives a [`DispatcherOptions`](../Options/DispatcherOptions.md) instance whose `IncludeHandlers` / `ExcludeHandlers` lists decide which auto-registered handlers — and the behaviors related to their messages — are kept. |

Both overloads return the same `IServiceCollection`, so calls chain. The parameterless overload and the fluent `AddDispatcher<TRequest, TResponse, THandler>()` builder are distinct methods, so both styles can be mixed in one startup sequence.

## Why is this important?

### 1. Simplifies Setup:
It provides a clean, discoverable way for a developer using your library to get it working.
Instead of needing to know the concrete implementation (`Dispatcher`), its correct lifetime (`Scoped`), or the auto-registration queue, they can simply call `services.AddDispatcher()`.

### 2. Encapsulation:
It hides the implementation details of the dispatcher pattern.
If you were to change the concrete `Dispatcher` class (or the registration mechanics) in the future, developers using your library wouldn't need to change their startup code as long as the extension method remains.

### 3. Foundation for Features:
This registration is the foundation upon which all other features from `RA.Utilities.Dispatcher` are built.
The source generator and the fluent builders (`.AddDispatcher<TRequest, TResponse, THandler>()`, `.AddNotification()`) register the handlers and behaviors, while `AddDispatcher` registers the central "dispatcher" that actually processes the requests and notifications — and applies the auto-discovered handlers.

In short, this class is the essential bootstrapping step required to use the CQRS pattern provided by the RA.Utilities.Dispatcher library.

## Related

- [Auto Registration](../auto-registration) — how handlers and behaviors reach the queue.
- [Filtering auto-registered handlers](../auto-registration#filtering-auto-registered-handlers) — the inclusion/exclusion rules.
- [`DispatcherOptions`](../Options/DispatcherOptions.md) — the filter options type.
- [`DispatcherBuilder`](../Builders/DispatcherBuilder.md) — the fluent registration API for behaviors and validators.
- [Generated Dispatcher](../generated-dispatcher) — the `IDispatcher` implementation registered here.
