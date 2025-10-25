---
title: IPipeline VS INotification
sidebar_position: 7
---

# IPipelineBehavior 🆚 INotificationBehavior

How do [`IPipelineBehavior`](./IPipelineBehavior.md) and [`INotificationBehavior`](./INotificationBehavior.md)
differ in their application within the RA.Utilities.Feature library?

That's an excellent question, as understanding the distinction between [`IPipelineBehavior`](./IPipelineBehavior.md) and [`INotificationBehavior`](./INotificationBehavior.md) is key to grasping the full power of the `RA.Utilities.Feature` library's design.
While both interfaces allow you to inject cross-cutting concerns into your application's processing flow, they operate on different types of messages and have distinct implications for how your application behaves.

Here's a breakdown of their differences:

### [`IPipelineBehavior<TRequest, TResponse>`](./IPipelineBehavior.md)
* **Purpose**: [`IPipelineBehavior`](./IPipelineBehavior.md) is designed to intercept and wrap the execution of **requests** (commands and queries).
Requests are typically part of a request-response pattern, where a single request is sent, and a single response is expected back.
* **Message Type**: It operates on types that implement [`IRequest<TResponse>`](./IRequest.md) (for requests with a response) or [`IRequest`](./IRequest.md) (for requests without a specific response, but still returning a [`Result`](../../../core/UtilitiesCore/Results.md)).
* **Mediator Method**: Used with the `IMediator.Send()` method.
* **Relationship**: There is a **one-to-one** relationship between a request and its primary [`IRequestHandler`](./IRequestHandler.md).
The [`IPipelineBehavior`](./IPipelineBehavior.md) wraps this single handler.
* **Return Type**: The `HandleAsync` method of [`IPipelineBehavior`](./IPipelineBehavior.md) returns a `Task<Result<TResponse>>` (or `Task<Result>`).
This means behaviors can:
  * **Modify the response**: A behavior can transform the `TResponse` before it's returned.
  * **Short-circuit the pipeline**: If a condition (like validation failure) is met, the behavior can return an error [`Result`](../../../core/UtilitiesCore/Results.md) immediately, preventing the actual [`IRequestHandler`](./IRequestHandler.md) from executing. This is crucial for concerns like validation.
* **Delegate**: It receives a [`RequestHandlerDelegate<TResponse>`](../Models/RequestHandlerDelegate.md) (or [RequestHandlerDelegate](../Models/RequestHandlerDelegate.md)) as its next parameter.
This delegate, when invoked, will execute the next stage in the request pipeline (either another behavior or the [`IRequestHandler`](../Abstractions/IRequestHandler.md)).
* **Common Use Cases**:
  * **Validation**: (e.g., [`ValidationBehavior`](../Behaviors/ValidationBehavior.md)) Validate incoming requests before they reach the handler.
  * **Logging**: Log request and response details.
  * **Transaction Management**: Wrap the handler execution in a database transaction.
  * **Caching**: Intercept queries to return cached results or cache results after execution.
  * **Error Handling**: Catch exceptions and transform them into standardized [`Result`](../../../core/UtilitiesCore/Results.md) failures.

### [`INotificationBehavior<TNotification>`](./INotificationBehavior.md)
* **Purpose: [`INotificationBehavior<TNotification>`](./INotificationBehavior.md)** is designed to intercept and wrap the execution of **notifications** (events).
Notifications are typically part of a publish-subscribe pattern, where an event is published, and multiple handlers (subscribers) can react to it independently.
* **Message Type**: It operates on types that implement [`INotification`](./INotification.md).
* **Mediator Method**: Used with the `IMediator.Publish()` method.
* **Relationship**: There is a **one-to-many** relationship between a notification and its [`INotificationHandlers`](./INotificationHandler.md).
Importantly, each [`INotificationHandlers`](./INotificationHandler.md) gets its own independent pipeline of [`INotificationBehavior<TNotification>`](./INotificationBehavior.md).
* **Return Type**: The `HandleAsync` method of [`INotificationBehavior<TNotification>`](./INotificationBehavior.md) returns a `Task`.
This signifies a "fire-and-forget" mechanism.
Behaviors can:
  * **Cannot modify the notification's outcome for the publisher**:
  Since notifications are fire-and-forget, the publisher doesn't expect a return value, and behaviors cannot alter a response that doesn't exist.
  * **Cannot short-circuit the entire publish operation**: A behavior can prevent a single [`INotificationHandlers`](./INotificationHandler.md) from executing by not calling `await next()`, but it cannot stop other handlers for the same notification from running.
  * **Delegate**: It receives a [`NotificationHandlerDelegate`](../Models/NotificationHandlerDelegate.md) as its `next` parameter.
This delegate, when invoked, will execute the next stage in the notification pipeline for a ***specific handler***.
* **Common Use Cases**:
  * **Logging**: (e.g., [`NotificationLoggingBehavior`](../Behaviors/NotificationLoggingBehavior.md)) Log when a notification is published and handled.
  * **Metrics**: (e.g., [`NotificationMetricsBehavior`](../Behaviors/NotificationMetricsBehavior.md)) Record performance metrics for notification handling.
  * **Retry Mechanisms**: (e.g., [`NotificationRetryBehavior`](../Behaviors/NotificationRetryBehavior.md)) Implement retry logic for potentially transient failures in notification handlers.
  * **Error Handling**: Catch exceptions within a single handler's pipeline to prevent it from crashing the entire publish operation.

## 🧠 Summary Table

| Feature	| [`IPipelineBehavior`](./IPipelineBehavior.md) |	[`INotificationBehavior<TNotification>`](./INotificationBehavior.md) |
| ------- | -------- | ----------- |
| **Message Type**	| [`IRequest`](./IRequest.md) / [`IRequest<TResponse>`](./IRequest.md) (Commands/Queries) |	[`INotification`](./INotification.md) (Events) |
| **Mediator Method**	| `Send()` |	`Publish()` |
| **Relationship** |	One-to-one (Request → Handler)	| One-to-many (Notification → Multiple Handlers, each with its own pipeline) |
| **Return Type** |	`Task<Result<TResponse>>` / `Task<Result>` |	`Task` (Fire-and-forget) |
| **Short-Circuit?** |	Yes, can prevent handler execution and return error |	No, cannot stop other handlers; can prevent a single handler from completing |
| **Delegate Type** |	[`RequestHandlerDelegate<TResponse>`](../Models/RequestHandlerDelegate.md) / [`RequestHandlerDelegate<TResponse>`](../Models/RequestHandlerDelegate.md) |	[`NotificationHandlerDelegate`](../Models/NotificationHandlerDelegate.md) |
| **Primary Goal** |	Control and augment request-response flow, potentially altering outcome	| Observe and augment event handling side effects |
| **Example** |	Validation, Transaction, Caching	| Logging, Metrics, Retries |

In essence, [`IPipelineBehavior`](./IPipelineBehavior.md) is about controlling and potentially altering
the outcome of a single, directed request, while [`INotificationBehavior<TNotification>`](./INotificationBehavior.md)
is about observing and adding side effects to the processing of events by multiple, independent subscribers.
