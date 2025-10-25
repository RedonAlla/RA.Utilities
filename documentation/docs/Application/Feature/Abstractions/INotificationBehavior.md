---
sidebar_position: 6
---

```powershell
Namespace: RA.Utilities.Feature.Abstractions
```

The `INotificationBehavior` interface defines a pipeline behavior that can be applied to the processing of an `INotification`.
It allows you to wrap the execution of `INotificationHandler`s to apply cross-cutting concerns.

## 🎯 Purpose

The primary purpose of `INotificationBehavior` is to create a "middleware" pipeline for notifications.
This allows you to encapsulate logic that should run for every notification without duplicating code in every `INotificationHandler`.

When a notification is published, it can be passed through a pipeline of one or more `INotificationBehavior` implementations before it reaches the actual handlers.
This allows you to execute logic both before and after the notification handlers are invoked.

Common use cases include:

-   **Logging**: Logging the start and end of a notification's processing.
-   **Error Handling**: Wrapping the execution of all handlers in a global try-catch block.
-   **Performance Monitoring**: Measuring the time it takes for all handlers to complete.

## ⚙️ How It Works

A class that implements `INotificationBehavior<TNotification>` must define a `HandleAsync` method.
This method receives the `notification` and a `next` delegate.
The `next` delegate represents the invocation of the actual `INotificationHandler`(s) for that notification.

A behavior can:
1.  Execute code **before** calling `await next()`.
2.  Execute code **after** `await next()` has completed.

This "wrapping" capability is what makes it a powerful tool for handling cross-cutting concerns in an event-driven architecture.

## 🚀 Usage Example

A common use case is to create a logging behavior to trace the flow of notifications through the system.

```csharp showLineNumbers
using Microsoft.Extensions.Logging;
using RA.Utilities.Feature.Abstractions;
using RA.Utilities.Feature.Models;

public class LoggingNotificationBehavior<TNotification> : INotificationBehavior<TNotification>
    where TNotification : INotification
{
    private readonly ILogger<LoggingNotificationBehavior<TNotification>> _logger;

    public LoggingNotificationBehavior(ILogger<LoggingNotificationBehavior<TNotification>> logger)
    {
        _logger = logger;
    }

    public async Task HandleAsync(TNotification notification, NotificationHandlerDelegate next, CancellationToken cancellationToken)
    {
        var notificationName = typeof(TNotification).Name;

        _logger.LogInformation("Handling notification {NotificationName}...", notificationName);

        // This calls the actual INotificationHandler(s)
        await next();

        _logger.LogInformation("Notification {NotificationName} handled.", notificationName);
    }
}
```

## 🧠 Summary
In summary, `INotificationBehavior` provides a clean, reusable way to add common logic to your event handling pipeline, keeping your individual notification handlers focused on their specific business logic.