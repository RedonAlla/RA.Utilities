---
sidebar_position: 7
---

```powershell
Namespace: RA.Utilities.Feature.Abstractions
```

The `IMediator` interface is the central abstraction for dispatching requests and publishing notifications within your CQRS (Command Query Responsibility Segregation) architecture.
It serves as the primary entry point for triggering business logic from your presentation layer (e.g., an API controller).

## 🎯 Purpose and Function

The `IMediator` interface elegantly decouples the sender of a message from its handler.
A controller doesn't need to know which class handles creating a product;
it simply creates a `CreateProductCommand` and asks the mediator to send it.
The mediator then takes responsibility for finding and invoking the correct handler.

This interface combines two fundamental communication patterns:

1. **Request/Response (`Send` methods)**:
This is used for commands (actions that change state) and queries (requests for data).
It's a one-to-one pattern where a single `IRequest` is dispatched to a single `IRequestHandler`.
  * `Send<TRequest>(...)`: For requests that do not return a value.
  * `Send<TRequest, TResponse>(...)`: For requests that return a value.

2. **Publish/Subscribe (`Publish` method)**:
This is used for events or notifications (`INotification`).
It's a one-to-many pattern where a single notification is published to all registered `INotificationHandlers` (zero, one, or many).
This is perfect for handling side effects like clearing a cache or sending an email after an operation completes.

## 🚀 Usage Example

```csharp
using RA.Utilities.Api.Abstractions;

app.MapPost("todos", static async (
    [FromServices] IMediator mediator,
    CancellationToken cancellationToken
) =>
{
  await mediator.Send(new CreateUserCommand("Alice"), cancellationToken);
  await mediator.Publish(new UserCreatedEvent(Guid.NewGuid()), cancellationToken);
})
```