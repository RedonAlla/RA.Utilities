---
sidebar_position: 2
---

```powershell
Namespace: RA.Utilities.Feature.Models
```

The `NotificationHandlerDelegate` class, which is actually a delegate type,
serves a crucial role in the `RA.Utilities.Feature` library's notification processing pipeline.

Its primary purpose is to **represent an asynchronous callback that invokes the next stage in the notification handling process.**

## ✨ Here's a more detailed breakdown:

#### 1. Enabling Pipeline Behaviors:
It is specifically designed to be used within [`INotificationBehavior`](../Abstractions/INotificationBehavior.md) implementations.
When you define a class that implements `INotificationBehavior<TNotification>`, its `HandleAsync` method receives a
`NotificationHandlerDelegate` as one of its parameters, typically named `next`.

#### 2. "Next" in the Chain:
This `next` delegate signifies ***"the rest of the pipeline"***. When an [`INotificationBehavior`](../Abstractions/INotificationBehavior.md) calls `await next()`, it's instructing the system to proceed with either:

  * The actual [`INotificationBehavior`](../Abstractions/INotificationBehavior.md)(s) registered for the notification.
  * The next [`INotificationBehavior`](../Abstractions/INotificationBehavior.md) in the chain, if multiple behaviors are configured.

#### 3. Facilitating Cross-Cutting Concerns:
By providing this next delegate, [`INotificationBehavior`](../Abstractions/INotificationBehavior.md) can wrap the execution of the core notification handling logic.
This allows you to implement cross-cutting concerns such as:
  * **Logging**: Log before and after the notification is handled.
  * **Error Handling**: Wrap the handler execution in a `try-catch` block.
  * **Performance Monitoring**: Measure the duration of the handling process.

#### 4. Asynchronous and Fire-and-Forget:
The delegate returns a `Task`, indicating that the operation is asynchronous.
Since notifications are typically "fire-and-forget" (meaning the publisher doesn't expect a return value from the handlers), the delegate doesn't return any specific data type.

## 🛠️ How It Works
Think of this process as building a set of Russian nesting dolls for each handler:

### 1. The Core (Innermost Doll):
The process starts by creating a `NotificationHandlerDelegate` that represents the actual work to be done—calling the `HandleAsync` method on an `INotificationHandler`.

```csharp
NotificationHandlerDelegate handlerDelegate = () =>
  handler.HandleAsync(notification, cancellationToken);
```

### 2. Wrapping with Behaviors (Adding Layers):

The code then iterates through any registered `INotificationBehaviors` in reverse.
For each behavior, it does the following:

  * It takes the ***current*** `handlerDelegate` (the doll we have so far) and stores it in a variable called next.
  * It creates a ***new*, larger doll by assigning a new delegate to `handlerDelegate`.
  This new delegate's job is to call the `behavior.HandleAsync` method, passing the next delegate into it.

This effectively wraps the previous delegate inside the new one.
If you have a [`LoggingBehavior`](../Behaviors/LoggingBehavior.md) and a
[`NotificationMetricsBehavior`](../Behaviors/NotificationMetricsBehavior.md), the final handlerDelegate will be a delegate that calls the [`LoggingBehavior`](../Behaviors/LoggingBehavior.md), which in turn will call the [`NotificationMetricsBehavior`](../Behaviors/NotificationMetricsBehavior.md), which finally calls the original handler.

### 3. Execution (Opening the Dolls):
Finally, `await handlerDelegate()` is called. 
his invokes the outermost delegate in the chain (e.g., the [`LoggingBehavior`](../Behaviors/LoggingBehavior.md)).
That behavior runs its "pre" logic, calls `await next()`, which invokes the next behavior in the chain, and so on,
until the original handler at the very center is executed.
Once the inner layers complete, execution unwinds back out, allowing behaviors to run their "post" logic.

This elegant use of delegates allows the `Mediator` to construct a flexible and dynamic pipeline for every notification,
applying cross-cutting concerns without the handlers or behaviors needing to know about each other.

## 🧠 Summary

In essence, `NotificationHandlerDelegate` is the mechanism that allows the `RA.Utilities.Feature` library to build a flexible "middleware" pipeline for notifications, enabling behaviors to intercept and augment the handling process without modifying the core [`INotificationHandler`](../Abstractions/INotificationHandler.md) logic.