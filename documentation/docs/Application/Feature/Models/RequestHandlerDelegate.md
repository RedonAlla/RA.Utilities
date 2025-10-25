---
sidebar_position: 1
---

```powershell
Namespace: RA.Utilities.Feature.Models
```

The `RequestHandlerDelegate` class, or more accurately, the `RequestHandlerDelegate` delegate types,
are fundamental to how the `RA.Utilities.Feature` library processes requests.
They serve a similar purpose to [`NotificationHandlerDelegate`](NotificationHandlerDelegate.md) but are tailored for the request-response pattern,
specifically integrating with the `Result` pattern from `RA.Utilities.Core`.

There are two main `RequestHandlerDelegate` types:

#### 1. ```public delegate Task<Result> RequestHandlerDelegate();```

  * **Purpose**:
  This delegate represents an asynchronous callback for handling requests that **do not return a specific data type** but still need to indicate the success or failure of the operation.
  * **Usage**:
  It's used in the `Mediator.Send<TRequest>` method, where `TRequest` implements `IRequest` (meaning it doesn't specify a `TResponse`).
  The `Result` type from `RA.Utilities.Core` is used to convey whether the operation was successful and any associated errors if it failed.

#### 2. ```public delegate Task<Result<TResponse>> RequestHandlerDelegate<TResponse>();```

  * **Purpose**:
  This delegate represents an asynchronous callback for handling requests that ***are expected to return a specific data type (`TResponse`)***.
  The `TResponse` is wrapped in a `Result<TResponse>` to indicate success (with the `TResponse` value) or failure (with associated errors).
  * **Usage**: It's used in the `Mediator.Send<TRequest, TResponse>` method, where TRequest implements `IRequest<TResponse>`.

## ✨ Key Purposes and How They Work:
1. **Enabling Pipeline Behaviors ([`IPipelineBehavior`](../Abstractions//IPipelineBehavior.md))**:
Just like [`NotificationHandlerDelegate`](NotificationHandlerDelegate.md) for notifications, `RequestHandlerDelegate` (and its generic counterpart) is the cornerstone for implementing [`IPipelineBehaviors`](../Abstractions//IPipelineBehavior.md).
The `HandleAsync` method of an [`IPipelineBehaviors`](../Abstractions//IPipelineBehavior.md) receives one of these delegates as its `next` parameter.
This `next` delegate represents "the rest of the pipeline" – either the subsequent behavior or the actual `IRequestHandler`.

2. **Facilitating Cross-Cutting Concerns**:
By passing the next delegate, [`IPipelineBehaviors`](../Abstractions//IPipelineBehavior.md) implementations can wrap the execution of the core request handling logic.
This allows for the clean implementation of cross-cutting concerns such as:

  * **Validation**: Validate the request before calling `await next()`.
  If validation fails, the behavior can short-circuit the pipeline by returning an error `Result` without ever invoking the actual handler.
  * **Logging**: Log request details before and after the handler executes.
  * **Error Handling**: Wrap the `await next()` call in a try-catch block to handle exceptions uniformly.
  * **Transactions**: Begin a database transaction before `await next()` and commit/rollback based on the outcome.

3. **Integration with `RA.Utilities.Core.Results`**:
A critical aspect is that both `RequestHandlerDelegate` types return `Task<Result>` or `Task<Result<TResponse>>`.
This design choice enforces the use of the `Result` pattern throughout the request processing pipeline.
Every stage, from behaviors to the final handler, is expected to return a `Result`, making error handling explicit and consistent.

## 🚀 Example Usage
Let's look at how these delegates are constructed and used in the Mediator's Send methods:

#### For requests with a response (`Send<TRequest, TResponse>`):

```csharp showLineNumbers
// From: /RA.Utilities/Application/RA.Utilities.Feature/Mediator.cs

public async Task<Result<TResponse>> Send<TRequest, TResponse>(
    TRequest request,
    CancellationToken cancellationToken = default)
    where TRequest : IRequest<TResponse>
{
    // ... (handler and behaviors retrieval) ...

    // 1. The core handler's execution is encapsulated in a RequestHandlerDelegate<TResponse>.
    Task<Result<TResponse>> HandlerDelegate() => handler.HandleAsync(request, cancellationToken);

    // 2. The pipeline is built by aggregating behaviors around the core delegate.
    //    The 'nextDelegate' parameter in the Aggregate function is the delegate from the previous iteration (or the core handler initially).
    //    The lambda creates a new delegate that calls the current behavior, passing the 'nextDelegate' to it.
    RequestHandlerDelegate<TResponse> next = behaviors
        .Reverse() // Behaviors are applied in reverse order of registration to ensure the first registered is outermost.
        .Aggregate((RequestHandlerDelegate<TResponse>)HandlerDelegate,
            (nextDelegate, behavior) => () => behavior.HandleAsync(request, nextDelegate, cancellationToken));

    // 3. The outermost delegate (representing the entire pipeline) is executed.
    return await next();
}
```

#### For requests without a response (`Send<TRequest>`):

```csharp showLineNumbers
// From: /RA.Utilities/Application/RA.Utilities.Feature/Mediator.cs

public async Task<Result> Send<TRequest>(
    TRequest request,
    CancellationToken cancellationToken = default)
    where TRequest : IRequest
{
    // ... (handler and behaviors retrieval) ...

    // 1. The core handler's execution is encapsulated in a RequestHandlerDelegate.
    Task<Result> HandlerDelegate() => handler.HandleAsync(request, cancellationToken);

    // 2. The pipeline is built similarly, but using the non-generic RequestHandlerDelegate.
    RequestHandlerDelegate next = behaviors
        .Reverse()
        .Aggregate((RequestHandlerDelegate)HandlerDelegate,
            (nextDelegate, behavior) => () => behavior.HandleAsync(request, nextDelegate, cancellationToken));

    // 3. The outermost delegate is executed.
    return await next();
}
```