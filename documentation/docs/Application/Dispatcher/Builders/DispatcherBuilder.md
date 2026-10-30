---
sidebar_position: 1
---

```powershell
Namespace: RA.Utilities.Dispatcher.Builders
```

`DispatcherBuilder<TRequest>` and `DispatcherBuilder<TRequest, TResponse>` are the fluent builders returned by `AddDispatcher<...>()`. They configure the pipeline of a single request: decorations (behaviors) and validators.

## 📦 Type Definition

```csharp
public class DispatcherBuilder<TRequest> : IDispatcherBuilder where TRequest : IRequest
{
    public IServiceCollection Services { get; }
    public DispatcherBuilder<TRequest> AddDecoration<TBehavior>() where TBehavior : class, IPipelineBehavior<TRequest>;
    public DispatcherBuilder<TRequest> AddValidator<TValidator>() where TValidator : class, IValidator<TRequest>;
}

public class DispatcherBuilder<TRequest, TResponse> : IDispatcherBuilder where TRequest : IRequest<TResponse>
{
    public IServiceCollection Services { get; }
    public DispatcherBuilder<TRequest, TResponse> AddDecoration<TBehavior>() where TBehavior : class, IPipelineBehavior<TRequest, TResponse>;
    public DispatcherBuilder<TRequest, TResponse> AddValidator<TValidator>() where TValidator : class, IValidator<TRequest>;
}
```

## 🎯 Member Breakdown

### `AddDecoration<TBehavior>()`

Registers the pipeline behavior **transiently** under its behavior interface:

```csharp
services.AddTransient<IPipelineBehavior<TRequest, TResponse>, TBehavior>();
```

Behaviors compose in **registration order, outermost first** — the first `AddDecoration` call wraps the rest of the pipeline. Auto-registered closed behaviors compose by message interface, then by [`[PipelineBehaviorOrder]`](../Attributes/PipelineBehaviorOrderAttribute.md), then by type name; explicit decorations keep their position relative to `AddDispatcher()` (before → outside the auto-registered block, after → inside it).

### `AddValidator<TValidator>()`

Two registrations in one call:

1. The validator itself: `services.AddTransient<IValidator<TRequest>, TValidator>()`
2. The [`ValidationBehavior`](../Behaviors/ValidationBehavior.md): `services.AddTransient<IPipelineBehavior<TRequest[, TResponse]>, ValidationBehavior<TRequest[, TResponse]>>()`

## 🚀 Usage

```csharp
builder.Services
    .AddDispatcher<CreateProductCommand, int, CreateProductHandler>()
    .AddDecoration<LoggingBehavior<CreateProductCommand, int>>()   // outermost — logs around everything
    .AddValidator<CreateProductCommandValidator>();                // innermost — validates first

// Execution order: Logging → Validation → Handler
```

:::note
`AddDispatcher` also registers the handler itself (scoped). Since plain handlers are already auto-registered by the source generator, the registration simply stacks — you only need `AddDispatcher` when the feature also has behaviors or validators to add.
:::

## 🔗 Related

- [`NotificationDispatcherBuilder<TNotification>`](NotificationDispatcherBuilder.md) — the notification counterpart.
- [`DispatcherBuilderExtensions`](../Extensions/DispatcherBuilderExtensions.md) — the extension methods that return these builders.
- [`PipelineBehaviorOrderAttribute`](../Attributes/PipelineBehaviorOrderAttribute.md) — ordering for auto-registered behaviors.
