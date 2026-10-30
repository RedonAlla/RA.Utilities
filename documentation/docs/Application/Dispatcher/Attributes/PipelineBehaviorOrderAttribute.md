---
sidebar_position: 1
---

```powershell
Namespace: RA.Utilities.Dispatcher.Attributes
```

`PipelineBehaviorOrderAttribute` specifies where a **closed (non-generic)** pipeline or notification behavior sits in the behavior chain. The source generator reads it at compile time and emits the behavior's registration in that position, so ordering costs nothing at runtime.

```csharp
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class PipelineBehaviorOrderAttribute : Attribute
{
    public PipelineBehaviorOrderAttribute(int order);
    public int Order { get; }
}
```

## 🎯 Semantics

| Aspect | Behavior |
|---|---|
| Chain position | **Lower values run closer to the outside** (executed earlier). `-100` is outermost, `100` is innermost. |
| Default | Behaviors without the attribute use order `0`. |
| Ties | Behaviors with the same order are registered by **type name** (ordinal), keeping output deterministic. |
| Scope | Applies to `IPipelineBehavior<TRequest>`, `IPipelineBehavior<TRequest, TResponse>`, and `INotificationBehavior<TNotification>` implementations that the generator auto-registers. |
| Generic behaviors | Not auto-registered (`DSAG010`), so the attribute has no effect on them — order them with `AddDecoration` call order. |
| Explicit decorations | `AddDecoration<TBehavior>()` registrations keep **call order**: added before `AddDispatcher()` they wrap the auto-registered behaviors (outermost); added after, they run inside them (innermost). |
| Inheritance | `Inherited = false`: a derived behavior must declare the attribute itself. |

The generator sorts each message's behaviors by interface, then `Order`, then type name, and the generated dispatcher wraps the chain in reverse — that is why the first registered behavior is outermost.

## 🚀 Complete Example

```csharp showLineNumbers
using RA.Utilities.Core.Results;
using RA.Utilities.Dispatcher.Abstractions;
using RA.Utilities.Dispatcher.Attributes;
using RA.Utilities.Dispatcher.Models;

// Outermost: runs first, sorting last alphabetically.
[PipelineBehaviorOrder(-100)]
public sealed class MetricsBehavior : IPipelineBehavior<CreateOrderCommand, CreateOrderResult>
{
    public ValueTask<Result<CreateOrderResult>> HandleAsync(
        CreateOrderCommand request, RequestHandlerDelegate<CreateOrderResult> next, CancellationToken ct)
    {
        // measure around next()
        return next();
    }
}

// Innermost: runs closest to the handler, sorting first alphabetically.
[PipelineBehaviorOrder(100)]
public sealed class CachingBehavior : IPipelineBehavior<CreateOrderCommand, CreateOrderResult>
{
    public ValueTask<Result<CreateOrderResult>> HandleAsync(
        CreateOrderCommand request, RequestHandlerDelegate<CreateOrderResult> next, CancellationToken ct)
        => next();
}

// No attribute → order 0, runs between the two.
public sealed class LoggingBehavior : IPipelineBehavior<CreateOrderCommand, CreateOrderResult>
{
    public ValueTask<Result<CreateOrderResult>> HandleAsync(
        CreateOrderCommand request, RequestHandlerDelegate<CreateOrderResult> next, CancellationToken ct)
        => next();
}
```

For a `Send(new CreateOrderCommand(...))` call the pipeline executes **Metrics → Logging → Caching → handler**. Without the attributes, the same behaviors would run in type-name order (Caching → Logging → Metrics).

## 🧠 Notification Behaviors

The attribute is equally valid on `INotificationBehavior<TNotification>`:

```csharp
[PipelineBehaviorOrder(-10)] // outermost
public sealed class NotificationRetryProbe : INotificationBehavior<OrderPlaced>
{
    public ValueTask<Result> HandleAsync(
        OrderPlaced notification, NotificationHandlerDelegate next, CancellationToken ct)
        => next(); // retry around the inner pipeline in real code
}

[PipelineBehaviorOrder(10)] // innermost
public sealed class NotificationAuditProbe : INotificationBehavior<OrderPlaced>
{
    public ValueTask<Result> HandleAsync(
        OrderPlaced notification, NotificationHandlerDelegate next, CancellationToken ct)
        => next(); // write the audit entry after next() in real code
}
```

Each notification handler is wrapped in its own pipeline, and the same ordered behavior list is applied to every handler.

## 🔗 Related

- [Auto Registration](../auto-registration) — closed behaviors are auto-registered; generic ones are not.
- [Generated Dispatcher](../generated-dispatcher) — the registration order the generator emits.
- [`DispatcherBuilder.AddDecoration`](../Builders/DispatcherBuilder.md) — explicit behavior registration and its call-order semantics.
