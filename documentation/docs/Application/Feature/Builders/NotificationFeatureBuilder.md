---
sidebar_position: 2
---

```powershell
Namespace: RA.Utilities.Feature.Builders
```

The `NotificationFeatureBuilder<TNotification>` class is a fluent builder returned by the `AddNotification<TNotification>()` extension method. It allows you to register notification handlers and notification pipeline behaviors in a single, readable chain.

## 📦 Type Parameter

| Parameter | Constraint | Purpose |
|---|---|---|
| `TNotification` | `TNotification : INotification` | The notification type being configured |

## ⚙️ Builder Methods

### `AddHandler<THandler>()`

Registers a notification handler for the notification type. You can call this multiple times to register multiple handlers for the same notification.

```csharp
builder.Services
    .AddNotification<OrderPlaced>()
    .AddHandler<SendConfirmationEmail>()
    .AddHandler<UpdateInventory>();
```

Each handler is registered as `Transient` via `services.AddTransient<INotificationHandler<TNotification>, THandler>()`.

### `AddDecoration<TBehavior>()`

Registers a notification behavior that wraps each handler's execution. Behaviors run in registration order (first registered = outermost).

```csharp
builder.Services
    .AddNotification<OrderPlaced>()
    .AddHandler<SendConfirmationEmail>()
    .AddDecoration<NotificationRetryBehavior<OrderPlaced>>()
    .AddDecoration<NotificationLoggingBehavior<OrderPlaced>>();
```

Behaviors are registered as `Transient` via `services.AddTransient<INotificationBehavior<TNotification>, TBehavior>()`.

## 🚀 Complete Example

```csharp
// Program.cs
builder.Services
    .AddNotification<OrderPlaced>()
    .AddHandler<SendConfirmationEmail>()
    .AddHandler<UpdateInventory>()
    .AddHandler<AuditLogWriter>()
    .AddDecoration<NotificationRetryBehavior<OrderPlaced>>()
    .AddDecoration<NotificationMetricsBehavior<OrderPlaced>>()
    .AddDecoration<NotificationLoggingBehavior<OrderPlaced>>();
```

## 🧠 Summary

`NotificationFeatureBuilder` provides the same fluent experience for notifications that `FeatureBuilder` provides for requests — keeping all registrations for a single notification type together in one cohesive block.
