---
sidebar_position: 3
---

```powershell
Namespace: RA.Utilities.Dispatcher.Behaviors
```

`NotificationLoggingBehavior<TNotification>` is a notification behavior that logs each notification handler invocation at start and finish.

## 📦 Type Definition

```csharp
public class NotificationLoggingBehavior<TNotification> : INotificationBehavior<TNotification>
    where TNotification : INotification
{
    public NotificationLoggingBehavior(ILogger<NotificationLoggingBehavior<TNotification>> logger);
}
```

## 🎯 What It Logs

| Event | Level | Message |
|---|---|---|
| Handler invoked | Information | `[Notification Logging] Start. Notification: {@Notification}` |
| Success | Information | `[Notification Logging] Finished. Notification: {@Notification}` |
| Failed result | Warning | `[Notification Logging] Failed. Notification: {@Notification}` (with the failure exception) |

The behavior wraps `next()` for a **single handler** — each notification handler gets its own logging wrap.

## 🚀 Usage

```csharp
builder.Services
    .AddNotification<OrderPlaced>()
    .AddDecoration<NotificationLoggingBehavior<OrderPlaced>>();
```

Because the behavior is generic, the auto-registration generator reports `DSAG010` and skips it — register it explicitly with `AddDecoration`, as above. Behaviors compose in registration order, outermost first — for the logging to see retries, register it **inside** the retry behavior:

```csharp
builder.Services
    .AddNotification<OrderPlaced>()
    .AddDecoration<NotificationRetryBehavior<OrderPlaced>>()     // outermost
    .AddDecoration<NotificationLoggingBehavior<OrderPlaced>>();  // innermost

// For each handler: Retry → Logging → Handler
```
