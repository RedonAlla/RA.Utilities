```powershell
Namespace: RA.Utilities.Feature.Extensions
```

The `MediatorServiceCollectionExtensions` class has a single, crucial purpose: to register the core `IMediator` **service with the .NET dependency injection container.**

It provides a simple, one-line extension method, `AddMediator`, that handles this registration.

```csharp showLineNumbers
// From: /RA.Utilities/Application/RA.Utilities.Feature/Extensions/MediatorServiceCollectionExtensions.cs

public static IServiceCollection AddMediator(this IServiceCollection services)
{
    services.AddScoped<IMediator, Mediator>();
    return services;
}
```

## Why is this important?

### 1. Simplifies Setup:
It provides a clean, discoverable way for a developer using your library to get it working.
Instead of needing to know the concrete implementation (`Mediator`) and its correct lifetime (`Scoped`), they can simply call `services.AddMediator()`.

### 2. Encapsulation:
It hides the implementation details of the mediator pattern.
If you were to change the concrete `Mediator` class in the future, developers using your library wouldn't need to change their startup code as long as the extension method remains.

### 3. Foundation for Features:
This registration is the foundation upon which all other features from `RA.Utilities.Feature` are built.
The `FeatureBuilderExtensions` (`.AddFeature()`, `.AddNotification()`) register the handlers, but `AddMediator` registers the central "dispatcher" that actually processes the requests and notifications.

In short, this class is the essential bootstrapping step required to use the CQRS pattern provided by the RA.Utilities.Feature library.
