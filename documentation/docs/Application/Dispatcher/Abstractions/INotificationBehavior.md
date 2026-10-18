---
sidebar_position: 6
---

```powershell
Namespace: RA.Utilities.Dispatcher.Abstractions
```

`INotificationBehavior<TNotification>` lets you inject cross-cutting concerns around **notification handler** execution — retry, logging, metrics — per handler, independently of every other handler for the same notification.

## 📦 Type Definition

```csharp
public interface INotificationBehavior<in TNotification> where TNotification : INotification
{
    ValueTask<Result> HandleAsync(
        TNotification notification,
        NotificationHandlerDelegate next,
        CancellationToken cancellationToken);

    ValueTask<Result> HandleAsync<TContext>(
        TNotification notification,
        NotificationHandlerContextDelegate<TContext> next,
        PipelineContext<TContext> context,
        CancellationToken cancellationToken)
        where TContext : class, new()
        => HandleAsync(notification, () => next(context), cancellationToken); // default implementation
}
```

## 🎯 How It Works

Each notification handler gets its **own** behavior pipeline. When `Publish` fans out to `N` handlers, each handler is wrapped in its own chain of `INotificationBehavior<TNotification>` instances, composed in registration order (outermost first). A behavior can:

- **Do work before and after** — e.g., log at start and finish.
- **Retry** — catch a failed result and re-run the inner pipeline (`NotificationRetryBehavior`).
- **Measure** — time the inner pipeline and warn on slow handlers (`NotificationMetricsBehavior`).
- **Prevent a single handler from completing** — by not calling `next()` — but it can never stop the other handlers from running.

## 🚀 Custom Behavior Example

```csharp
public class AuditBehavior<TNotification> : INotificationBehavior<TNotification>
    where TNotification : INotification
{
    public async ValueTask<Result> HandleAsync(
        TNotification notification, NotificationHandlerDelegate next, CancellationToken cancellationToken)
    {
        Result result = await next();
        _auditLog.Record(typeof(TNotification).Name, result.IsSuccess);
        return result;
    }
}
```

## 🔧 Registration

Closed (non-generic) behaviors are **auto-registered** by the source generator under their interface. Generic behaviors must be registered explicitly:

```csharp
builder.Services
    .AddNotification<OrderPlaced>()
    .AddDecoration<AuditBehavior<OrderPlaced>>();
```

## 🔗 Related

- [`IPipelineBehavior`](IPipelineBehavior.md) — the request-pipeline counterpart. See [Pipeline vs Notification](PipelineVsNotification.md) for the differences.
- [Built-in notification behaviors](../Behaviors/index.mdx) — logging, metrics, retry.
- [`NotificationHandlerDelegate`](../Models/NotificationHandlerDelegate.md) — the delegate passed as `next`.
