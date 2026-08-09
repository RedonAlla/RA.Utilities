---
sidebar_position: 1
---

```powershell
Namespace: RA.Utilities.Feature.Builders
```

The `FeatureBuilder` class is a fluent builder returned by the `AddFeature` extension methods. It allows you to chain additional registrations — pipeline behaviors and validators — onto a feature in a single, readable block.

## 📦 Two Variants

The package provides two versions to match the two [`IRequest`](../Abstractions/IRequest.md) variants:

| Class | Constraint | Returned By |
|---|---|---|
| `FeatureBuilder<TRequest>` | `TRequest : IRequest` | `AddFeature<TRequest, THandler>()` |
| `FeatureBuilder<TRequest, TResponse>` | `TRequest : IRequest<TResponse>` | `AddFeature<TRequest, TResponse, THandler>()` |

Both implement [`IFeatureBuilder`](../Abstractions/IFeatureBuilder.md) and hold a reference to the [`IServiceCollection`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.iservicecollection).

## ⚙️ Builder Methods

### `AddDecoration<TBehavior>()`

Registers a pipeline behavior for the feature. The behavior wraps the handler and can execute logic before and after the request is processed.

```csharp
// TBehavior must implement IPipelineBehavior<TRequest, TResponse>
builder.Services
    .AddFeature<MyQuery, Result<Data>, MyQueryHandler>()
    .AddDecoration<LoggingBehavior<MyQuery, Result<Data>>>();
```

### `AddValidator<TValidator>()`

Registers a FluentValidation validator **and** the [`ValidationBehavior`](../Behaviors/ValidationBehavior.md) for the feature. This is a convenience method — it is equivalent to calling `AddDecoration<ValidationBehavior<...>>()` plus registering the validator.

```csharp
builder.Services
    .AddFeature<CreateProductCommand, Result<int>, CreateProductHandler>()
    .AddValidator<CreateProductCommandValidator>();
// Equivalent to:
//   services.AddTransient<IValidator<CreateProductCommand>, CreateProductCommandValidator>();
//   services.AddTransient<IPipelineBehavior<CreateProductCommand, Result<int>>, ValidationBehavior<CreateProductCommand, Result<int>>>();
```

## 🧠 Summary

`FeatureBuilder` is the linchpin of the fluent registration API. It transforms what would be multiple separate `IServiceCollection` calls into a single, cohesive vertical slice definition.
