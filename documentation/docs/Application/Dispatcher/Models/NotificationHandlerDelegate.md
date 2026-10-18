---
sidebar_position: 4
---

```powershell
Namespace: RA.Utilities.Dispatcher.Models
```

The `NotificationHandlerDelegate` delegates are what [`INotificationBehavior`](../Abstractions/INotificationBehavior.md) receives as its `next` parameter — invoking them runs the next stage of a **single handler's** notification pipeline.

## 📦 Type Definition

```csharp
/// <summary>Represents a delegate for handling notifications.</summary>
public delegate ValueTask<Result> NotificationHandlerDelegate();

/// <summary>Context-aware variant carrying the typed pipeline context.</summary>
public delegate ValueTask<Result> NotificationHandlerContextDelegate<TContext>(PipelineContext<TContext> context)
    where TContext : class, new();
```

## 🎯 Purpose

When `Publish` fans out a notification to `N` handlers, each handler gets its own pipeline of `INotificationBehavior<TNotification>` instances. Each behavior's `HandleAsync` receives a delegate that invokes the next behavior — or the handler itself, innermost. The delegate returns `ValueTask<Result>`: the handler's result on success, or `Result.Failure(...)` on expected failure.

- The non-generic `NotificationHandlerDelegate` is the plain pipeline step.
- `NotificationHandlerContextDelegate<TContext>` is the context-aware step, used by the `HandleAsync<TContext>` overload.

## 🚀 Usage

```csharp
public class AuditBehavior<TNotification> : INotificationBehavior<TNotification>
    where TNotification : INotification
{
    public async ValueTask<Result> HandleAsync(
        TNotification notification, NotificationHandlerDelegate next, CancellationToken cancellationToken)
    {
        Result result = await next(); // runs the inner pipeline (or the handler)
        _auditLog.Record(typeof(TNotification).Name, result.IsSuccess);
        return result;
    }
}
```

## 🔗 Related

- [`INotificationBehavior<TNotification>`](../Abstractions/INotificationBehavior.md) — the consumer of this delegate.
- [`RequestHandlerDelegate`](RequestHandlerDelegate.md) — the request-pipeline counterpart.
