---
sidebar_position: 4
---

```powershell
Namespace: RA.Utilities.Dispatcher.Behaviors
```

`NotificationMetricsBehavior<TNotification>` is a notification behavior that measures how long each notification handler takes and warns on slow handlers.

## 📦 Type Definition

```csharp
public class NotificationMetricsBehavior<TNotification> : INotificationBehavior<TNotification>
    where TNotification : INotification
{
    public NotificationMetricsBehavior(ILogger<NotificationMetricsBehavior<TNotification>> logger);
}
```

## 🎯 How It Works

1. Logs a `Debug`-level marker (`MetricsBehavior..`).
2. Starts a timestamp with `Stopwatch.GetTimestamp()` — **no per-call `Stopwatch` allocation** (the timestamp API is allocation-free).
3. Invokes `next()`.
4. If the inner pipeline took **over 500 ms**, logs a `Warning`:
   `Long running notification: {NotificationName} ({ElapsedMilliseconds}ms)`.

The measured result is passed through unchanged.

## 🚀 Usage

```csharp
builder.Services
    .AddNotification<OrderPlaced>()
    .AddDecoration<NotificationMetricsBehavior<OrderPlaced>>();
```

Because the behavior is generic, the auto-registration generator reports `DSAG010` and skips it — register it explicitly with `AddDecoration`, as above.
