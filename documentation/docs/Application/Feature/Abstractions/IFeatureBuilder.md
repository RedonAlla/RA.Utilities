---
sidebar_position: 8
---

```powershell
Namespace: RA.Utilities.Feature.Abstractions
```

`IFeatureBuilder` defines the contract for building features within the application. It is the common abstraction behind the fluent builders returned by the mediator registration extensions, exposing the underlying [`IServiceCollection`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.iservicecollection) so features can be configured fluently.

```csharp
public interface IFeatureBuilder
{
    IServiceCollection Services { get; }
}
```

## 🛠️ How It's Used

Two concrete builders implement the interface:

- **`FeatureBuilder<TRequest>`** — returned by `AddFeature` for requests without a response; chains `AddValidator` and `AddDecoration` to register pipeline components.
- **`NotificationFeatureBuilder<TNotification>`** — returned by `AddNotificationFeature`; chains `AddHandler` and `AddDecoration`.

```csharp
services.AddFeature<CreateProductCommand, Result<int>, CreateProductCommandHandler>()
    .AddValidator<CreateProductCommandValidator>()
    .AddDecoration<LoggingBehavior<CreateProductCommand, Result<int>>>();
```

Each chained call registers a service on the collection exposed by `Services` and returns the builder for further chaining.

🧠 Summary

`IFeatureBuilder` keeps the mediator registration API fluent and consistent: both feature builders share one contract, and any extension method written against `IFeatureBuilder` works for requests and notifications alike.
