---
sidebar_position: 5
---

```powershell
Namespace: RA.Utilities.Dispatcher.Abstractions
```

`INotificationHandler<TNotification>` is the interface your notification subscribers implement. Each notification can have any number of handlers; `Publish` runs all of them, each wrapped in its own behavior pipeline.

## 📦 Type Definition

```csharp
public interface INotificationHandler<in TNotification> where TNotification : INotification
{
    ValueTask<Result> HandleAsync(TNotification notification, CancellationToken cancellationToken);

    ValueTask<Result> HandleAsync<TContext>(
        TNotification notification, PipelineContext<TContext> context, CancellationToken cancellationToken)
        where TContext : class, new()
        => HandleAsync(notification, cancellationToken); // default implementation
}
```

## 🎯 Purpose

Notification handlers perform side effects in response to events: send emails, update caches, write audit logs. Each handler:

- Returns `Result.Success()` on success, `Result.Failure(exception)` on expected failure.
- Is invoked **independently** — a failure in one handler never prevents the others from running. `Publish` aggregates the failures into its returned `Result`.
- Gets its **own** notification behavior pipeline (retry, logging, metrics) when behaviors are registered.

## 🚀 Usage

```csharp
using RA.Utilities.Core.Results;
using RA.Utilities.Dispatcher.Abstractions;

public class SendConfirmationEmail : INotificationHandler<OrderPlaced>
{
    private readonly IEmailService _emailService;

    public SendConfirmationEmail(IEmailService emailService) => _emailService = emailService;

    public async ValueTask<Result> HandleAsync(OrderPlaced notification, CancellationToken cancellationToken)
    {
        await _emailService.SendAsync(notification.CustomerEmail, "Order Confirmed");
        return Result.Success();
    }
}
```

## 🔧 Auto-Registration

Concrete, non-generic `INotificationHandler<TNotification>` implementations are discovered by the package's source generator and registered automatically when `AddDispatcher()` is called (**transient**) — no explicit `AddNotification().AddHandler<>()` needed for plain handlers.

## 🔗 Related

- [`INotification`](INotification.md) — the notification marker interface.
- [`INotificationBehavior<TNotification>`](INotificationBehavior.md) — per-handler cross-cutting concerns.
- [`NotificationDispatcherBuilder`](../Builders/NotificationDispatcherBuilder.md) — explicit registration of handlers and behaviors.
