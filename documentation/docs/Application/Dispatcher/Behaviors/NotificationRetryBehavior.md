---
sidebar_position: 5
---

```powershell
Namespace: RA.Utilities.Dispatcher.Behaviors
```

`NotificationRetryBehavior<TNotification>` is a notification behavior that retries a handler when it returns a failed result, with an incremental backoff.

## 📦 Type Definition

```csharp
public class NotificationRetryBehavior<TNotification> : INotificationBehavior<TNotification>
    where TNotification : INotification
{
    public NotificationRetryBehavior(
        ILogger<NotificationRetryBehavior<TNotification>> logger,
        int maxRetries = 3,
        int baseDelayMilliseconds = 200);
}
```

| Parameter | Default | Meaning |
|---|---|---|
| `maxRetries` | `3` | Maximum number of attempts (must be > 0) |
| `baseDelayMilliseconds` | `200` | Base delay; the actual delay per attempt is `baseDelayMilliseconds * attempt` (must be ≥ 0) |

## 🎯 How It Works

1. Invokes `next()` — the inner pipeline (and ultimately the handler).
2. A **failed result** (or a thrown exception, which is captured into a failed result) triggers a retry after `baseDelayMilliseconds * attempt` delay.
3. Each failure is logged at `Warning`: `[Notification Retry] Attempt {Attempt} failed for {NotificationType}. Retrying...`
4. After `maxRetries` failed attempts it logs at `Error` (`[Notification Retry] All {MaxRetries} attempts failed ...`) and returns the last failed result.

The retry wraps the **entire inner pipeline** — including any behaviors registered inside it.

## 🚀 Usage

```csharp
builder.Services
    .AddNotification<OrderPlaced>()
    .AddDecoration<NotificationRetryBehavior<OrderPlaced>>(); // defaults: 3 retries, 200 ms base delay
```

Custom configuration via DI registration:

```csharp
builder.Services.AddTransient<INotificationBehavior<OrderPlaced>>(sp =>
    new NotificationRetryBehavior<OrderPlaced>(
        sp.GetRequiredService<ILogger<NotificationRetryBehavior<OrderPlaced>>>(),
        maxRetries: 5,
        baseDelayMilliseconds: 100));
```

## 🧠 Notes

- **Latency** — worst-case extra delay with 3 retries and 200 ms base is `200 + 400 + 600 = 1200 ms`. Tune for your workload.
- **Only failed results are retried** — a successful handler runs once.
- Because the behavior is generic, the auto-registration generator reports `DSAG010` and skips it — register it explicitly with `AddDecoration`.
