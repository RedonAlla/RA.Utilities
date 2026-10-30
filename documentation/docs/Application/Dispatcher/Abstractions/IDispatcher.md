---
sidebar_position: 7
---

```powershell
Namespace: RA.Utilities.Dispatcher.Abstractions
```

The `IDispatcher` interface is the central abstraction for dispatching requests and publishing notifications within your CQRS (Command Query Responsibility Segregation) architecture.
It serves as the primary entry point for triggering business logic from your presentation layer (e.g., an API endpoint).

The implementation is **source-generated** — the package's generator emits the `Dispatcher` class into your assembly, and `AddDispatcher()` registers it as the `IDispatcher` implementation. See [Generated Dispatcher](../generated-dispatcher) for the dispatch strategy.

## 🎯 Purpose and Function

The `IDispatcher` interface elegantly decouples the sender of a message from its handler.
An endpoint doesn't need to know which class handles creating a product;
it simply creates a `CreateProductCommand` and asks the dispatcher to send it.
The dispatcher then takes responsibility for finding and invoking the correct handler.

This interface combines two fundamental communication patterns:

1. **Request/Response (`Send` methods)**:
This is used for commands (actions that change state) and queries (requests for data).
It's a one-to-one pattern where a single `IRequest` is dispatched to a single `IRequestHandler`.
  * `Send<TRequest>(...)`: For requests that do not return a value — returns `ValueTask<Result>`.
  * `Send<TRequest, TResponse>(...)`: For requests that return a value — returns `ValueTask<Result<TResponse>>`.

Dispatch is **Result-based**: expected failures are returned as `Result.Failure(exception)` instead of being thrown. Context overloads (`Send<TRequest, TResponse, TContext>` and `Send<TRequest, TContext>`) add an optional `PipelineContext<TContext>? context` parameter.

2. **Publish/Subscribe (`Publish` method)**:
This is used for events or notifications (`INotification`).
It's a one-to-many pattern where a single notification is published to all registered `INotificationHandlers` (zero, one, or many).
This is perfect for handling side effects like clearing a cache or sending an email after an operation completes.
`Publish` runs every handler and aggregates failures into the returned `ValueTask<Result>`.

## 🚀 Usage Example

```csharp
using RA.Utilities.Core.Results;
using RA.Utilities.Dispatcher.Abstractions;

app.MapPost("todos", static async (
    [FromServices] IDispatcher dispatcher,
    CancellationToken cancellationToken
) =>
{
  Result sendResult = await dispatcher.Send(new CreateUserCommand("Alice"), cancellationToken);
  if (sendResult.IsFailure)
      return Results.BadRequest(sendResult.Exception.Message);

  Result publishResult = await dispatcher.Publish(new UserCreatedEvent(Guid.NewGuid()), cancellationToken);
  return publishResult.IsSuccess ? Results.Ok() : Results.Accepted();
})
```

## 🔗 Related

- [Examples](../examples) — complete end-to-end usage of `IDispatcher` and the concrete `Dispatcher`.
- [Generated Dispatcher](../generated-dispatcher) — how the `IDispatcher` implementation is generated and registered.
