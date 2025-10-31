---
sidebar_position: 3
---

```powershell
Namespace: RA.Utilities.Feature.Behaviors
```

`NotificationMetricsBehavior` is an implementation of the [`INotificationBehavior`](../Abstractions/INotificationBehavior.md) interface.
Its primary purpose is to act as a performance monitoring "middleware" for your CQRS notification pipeline.

It works by wrapping the execution of your [`INotificationHandler`](../Abstractions/INotificationHandler.md)(s) with a `Stopwatch`.
This allows it to measure the total time it takes for all handlers to process a given notification.

The key function of this behavior is to **identify slow-running event handlers**.
If the total processing time exceeds a predefined threshold (in this case, 500 milliseconds), it logs a warning.
This is incredibly useful for:

* **Detecting Performance Bottlenecks**: Pinpointing which events in your system are taking too long to process, which could impact overall application responsiveness.
* **Ensuring System Health**: Proactively monitoring the performance of asynchronous or background tasks triggered by events.
* **Maintaining a Clean Architecture**: It separates the concern of performance monitoring from the business logic within the notification handlers themselves.