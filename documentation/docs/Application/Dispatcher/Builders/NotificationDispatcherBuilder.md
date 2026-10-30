---
sidebar_position: 2
---

```powershell
Namespace: RA.Utilities.Dispatcher.Builders
```

`NotificationDispatcherBuilder<TNotification>` is the fluent builder returned by `AddNotification<TNotification>()`. It configures the handlers and the per-handler behavior pipeline of a single notification.

## 📦 Type Definition

```csharp
public class NotificationDispatcherBuilder<TNotification> : IDispatcherBuilder where TNotification : INotification
{
    public IServiceCollection Services { get; }
    public NotificationDispatcherBuilder<TNotification> AddHandler<THandler>() where THandler : class, INotificationHandler<TNotification>;
    public NotificationDispatcherBuilder<TNotification> AddDecoration<TBehavior>() where TBehavior : class, INotificationBehavior<TNotification>;
}
```

## 🎯 Member Breakdown

### `AddHandler<THandler>()`

Registers a notification handler **transiently** under its handler interface:

```csharp
services.AddTransient<INotificationHandler<TNotification>, THandler>();
```

A notification can have any number of handlers — each call adds one more subscriber to the fan-out.

### `AddDecoration<TBehavior>()`

Registers a notification behavior **transiently** under its behavior interface:

```csharp
services.AddTransient<INotificationBehavior<TNotification>, TBehavior>();
```

Every notification handler gets its **own** behavior pipeline, composed in registration order (outermost first).

## 🚀 Usage

```csharp
builder.Services
    .AddNotification<OrderPlaced>()
    .AddHandler<SendConfirmationEmail>()
    .AddHandler<UpdateInventory>()
    .AddDecoration<NotificationRetryBehavior<OrderPlaced>>()     // outermost: retries the inner pipeline
    .AddDecoration<NotificationLoggingBehavior<OrderPlaced>>();  // innermost: logs around each handler

// For each handler: Retry → Logging → Handler
```

:::note
Plain (non-generic) notification handlers are already auto-registered by the source generator when `AddDispatcher()` is called — `AddHandler` is only needed for generic handlers or handlers in lazily-loaded assemblies. Behaviors (generic or closed) are always registered explicitly with `AddDecoration`.
:::

## 🔗 Related

- [`DispatcherBuilder<TRequest[, TResponse]>`](DispatcherBuilder.md) — the request-pipeline counterpart.
- [`DispatcherBuilderExtensions`](../Extensions/DispatcherBuilderExtensions.md) — the extension methods that return these builders.
