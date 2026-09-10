---
sidebar_position: 1
---

```powershell
Namespace: RA.Utilities.Feature.Models
```

The `RequestHandlerDelegate` delegate types are fundamental to how the `RA.Utilities.Feature` library processes requests.
They represent the "rest of the pipeline" that an [`IPipelineBehavior`](../Abstractions/IPipelineBehavior.md) invokes via `next()`.

There are four `RequestHandlerDelegate` types:

#### 1. ```public delegate Task RequestHandlerDelegate();```

  * **Purpose**:
  This delegate represents an asynchronous callback for handling requests that **do not return a value**.
  * **Usage**:
  It's used in the `Mediator.Send<TRequest>` path (and by `IPipelineBehavior<TRequest>`), where `TRequest` implements `IRequest`.

#### 2. ```public delegate Task<TResponse> RequestHandlerDelegate<TResponse>();```

  * **Purpose**:
  This delegate represents an asynchronous callback for handling requests that ***are expected to return a value of type `TResponse`***.
  * **Usage**: It's used in the `Mediator.Send<TRequest, TResponse>` path (and by `IPipelineBehavior<TRequest, TResponse>`), where `TRequest` implements `IRequest<TResponse>`.

#### 3. ```public delegate Task RequestHandlerContextDelegate<TContext>(PipelineContext<TContext> context) where TContext : class, new();```

  * **Purpose**:
  A context-aware callback for requests without a response. It receives the [`PipelineContext<TContext>`](PipelineContext.md) flowing through the pipeline.
  * **Usage**: Used by the context-aware overload of `IPipelineBehavior<TRequest>`.

#### 4. ```public delegate Task<TResponse> RequestHandlerContextDelegate<TResponse, TContext>(PipelineContext<TContext> context) where TContext : class, new();```

  * **Purpose**:
  A context-aware callback for requests with a response.
  * **Usage**: Used by the context-aware overload of `IPipelineBehavior<TRequest, TResponse>`.

## ✨ Key Purposes and How They Work:
1. **Enabling Pipeline Behaviors ([`IPipelineBehavior`](../Abstractions//IPipelineBehavior.md))**:
Just like [`NotificationHandlerDelegate`](NotificationHandlerDelegate.md) for notifications, `RequestHandlerDelegate` (and its generic counterpart) is the cornerstone for implementing [`IPipelineBehaviors`](../Abstractions//IPipelineBehavior.md).
The `HandleAsync` method of an [`IPipelineBehaviors`](../Abstractions//IPipelineBehavior.md) receives one of these delegates as its `next` parameter.
This `next` delegate represents "the rest of the pipeline" – either the subsequent behavior or the actual `IRequestHandler`.

2. **Facilitating Cross-Cutting Concerns**:
By passing the next delegate, [`IPipelineBehaviors`](../Abstractions//IPipelineBehavior.md) implementations can wrap the execution of the core request handling logic.
This allows for the clean implementation of cross-cutting concerns such as:

  * **Validation**: Validate the request before calling `await next()`.
  If validation fails, the behavior can short-circuit the pipeline by throwing an exception (for example, `ValidationBehavior` throws a `BadRequestException`) without ever invoking the actual handler.
  * **Logging**: Log request details before and after the handler executes.
  * **Error Handling**: Wrap the `await next()` call in a try-catch block to translate or observe exceptions.
  * **Transactions**: Begin a database transaction before `await next()` and commit/rollback based on the outcome.

3. **Plain-task contract**:
The delegates return `Task` or `Task<TResponse>` directly — they do not carry a `Result` wrapper. Success is the returned value (or completion of the `Task`); failures propagate as exceptions out of the pipeline to the API layer's `GlobalExceptionHandler`.

## 🚀 Example Usage
Let's look at how these delegates are constructed and used in the Mediator's Send methods:

#### For requests with a response (`Send<TRequest, TResponse>`):

```csharp showLineNumbers
// From: /RA.Utilities/Application/RA.Utilities.Feature/Mediator.cs

public Task<TResponse> Send<TRequest, TResponse>(
    TRequest request,
    CancellationToken cancellationToken = default)
    where TRequest : IRequest<TResponse>
{
    // ... (handler and behaviors retrieval) ...

    // 1. The core handler's execution is encapsulated in a RequestHandlerDelegate<TResponse>.
    Task<TResponse> HandlerDelegate() => handler.HandleAsync(request, cancellationToken);

    // 2. The pipeline is built by aggregating behaviors around the core delegate.
    //    The 'nextDelegate' parameter in the Aggregate function is the delegate from the previous iteration (or the core handler initially).
    //    The lambda creates a new delegate that calls the current behavior, passing the 'nextDelegate' to it.
    RequestHandlerDelegate<TResponse> next = behaviors
        .Reverse() // Behaviors are applied in reverse order of registration to ensure the first registered is outermost.
        .Aggregate((RequestHandlerDelegate<TResponse>)HandlerDelegate,
            (nextDelegate, behavior) => () => behavior.HandleAsync(request, nextDelegate, cancellationToken));

    // 3. The outermost delegate (representing the entire pipeline) is executed.
    return next();
}
```

#### For requests without a response (`Send<TRequest>`):

```csharp showLineNumbers
// From: /RA.Utilities/Application/RA.Utilities.Feature/Mediator.cs

public Task Send<TRequest>(
    TRequest request,
    CancellationToken cancellationToken = default)
    where TRequest : IRequest
{
    // ... (handler and behaviors retrieval) ...

    // 1. The core handler's execution is encapsulated in a RequestHandlerDelegate.
    Task HandlerDelegate() => handler.HandleAsync(request, cancellationToken);

    // 2. The pipeline is built similarly, but using the non-generic RequestHandlerDelegate.
    RequestHandlerDelegate next = behaviors
        .Reverse()
        .Aggregate((RequestHandlerDelegate)HandlerDelegate,
            (nextDelegate, behavior) => () => behavior.HandleAsync(request, nextDelegate, cancellationToken));

    // 3. The outermost delegate is executed.
    return next();
}
```
