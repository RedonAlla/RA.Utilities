```powershell
Namespace: RA.Utilities.Feature.Extensions
```

The `MediatorServiceCollectionExtensions` class registers the core `IMediator` **service with the .NET dependency injection container** and **applies the compile-time handler registrations** produced by the package's source generator.

It provides a simple, one-line extension method, `AddMediator`, that handles both:

```csharp showLineNumbers
// From: /RA.Utilities/Application/RA.Utilities.Feature/Extensions/MediatorServiceCollectionExtensions.cs

public static IServiceCollection AddMediator(this IServiceCollection services)
{
    HandlerRegistrations.ApplyAll(services);
    MediatorRegistrations.ApplyAll(services);
    return services;
}
```

Since v10.2.0 a source generator shipped in the package emits a module initializer for every assembly containing `IRequestHandler<,>` / `IRequestHandler<>` / `INotificationHandler<>` implementations. Those initializers queue DI registrations (request handlers **scoped**, notification handlers **transient**) which `AddMediator()` then applies — that is why plain handlers need no explicit `AddFeature` / `AddNotification` call. See the [Auto Registration](../auto-registration) guide for the assembly-loading guarantee and diagnostics.

Since v11.1.0, the generator also emits the **`Mediator` implementation of `IMediator`** per assembly — the only mediator in the package. `AddMediator()` applies those registrations, so `IMediator` resolves to the generated implementation. See the [Generated Mediator](../generated-mediator) guide for the dispatch strategy and the message/handler diagnostics.

## Why is this important?

### 1. Simplifies Setup:
It provides a clean, discoverable way for a developer using your library to get it working.
Instead of needing to know the concrete implementation (`Mediator`), its correct lifetime (`Scoped`), or the auto-registration queue, they can simply call `services.AddMediator()`.

### 2. Encapsulation:
It hides the implementation details of the mediator pattern.
If you were to change the concrete `Mediator` class (or the registration mechanics) in the future, developers using your library wouldn't need to change their startup code as long as the extension method remains.

### 3. Foundation for Features:
This registration is the foundation upon which all other features from `RA.Utilities.Feature` are built.
The source generator and the fluent builders (`.AddFeature()`, `.AddNotification()`) register the handlers and behaviors, while `AddMediator` registers the central "dispatcher" that actually processes the requests and notifications — and applies the auto-discovered handlers.

In short, this class is the essential bootstrapping step required to use the CQRS pattern provided by the RA.Utilities.Feature library.
