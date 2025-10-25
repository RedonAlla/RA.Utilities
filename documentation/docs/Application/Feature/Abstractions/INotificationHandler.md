---
sidebar_position: 5
---

```powershell
Namespace: RA.Utilities.Feature.Abstractions
```

The `INotificationHandler<TNotification>` interface defines the contract for a class that handles a specific `INotification`.
It is the "subscriber" in the publish-subscribe pattern used for eventing within the CQRS architecture.

### 🎯 Purpose

The primary purpose of an `INotificationHandler` is to contain the logic for a single side effect that should occur in response to an event.
When a notification is published using the mediator, the mediator will find all registered handlers for that specific notification type and execute their `HandleAsync` methods.

This pattern allows for clean separation of concerns.
The code that publishes the event does not need to know what actions will be taken in response to it.

This interface is the "subscriber" part of the publish-subscribe pattern:

* **Publisher**: Some part of your code (like a command handler) publishes an `INotification`.
* **Subscribers**: One or more `INotificationHandler` classes that are registered to listen for that specific notification type will have their `HandleAsync` method invoked.


#### 🔑 Key Characteristics:

| Feature              | Description                                                                                                   |
| -------------------- | ------------------------------------------------------------------------------------------------------------- |
| **Pattern**          | Notification → Many Handlers (1-to-many)                                                                      |
| **Interface**        | `INotificationHandler<TNotification>`                                                                         |
| **Return value**     | `Task` (no return)                                                                                            |
| **Mediator method**  | `Publish()`                                                                                                   |
| **Typical use case** | Domain events, system events, sending emails, logging, etc. — where multiple handlers may react independently |

1.  **One-to-Many**: A single `INotification` can be handled by zero, one, or many `INotificationHandler` implementations.
2.  **Decoupled Logic**: It allows you to add new behaviors (like sending emails, logging, or invalidating caches) without modifying the code that triggers the event.
3.  **Fire-and-Forget**: The `HandleAsync` method returns a `Task`, not a `Task<T>`. This indicates that the handler performs an action but does not return a value to the publisher.

### ⚙️ How It Works

1.  **Implement the Interface**: You create a class that implements `INotificationHandler<TNotification>`, where `TNotification` is the specific event class you want to handle.
2.  **Implement `HandleAsync`**: You place your business logic for reacting to the event inside the `HandleAsync` method.
3.  **Register the Handler**: You register your handler in the dependency injection container. The mediator will then automatically discover and invoke it when the corresponding notification is published.

### 🚀 Usage Example

Following the example from the `INotification` documentation, after a `ProductCreatedNotification` is published, you might have two separate handlers to perform different actions.

#### Cache Invalidation Handler

This handler is responsible for clearing the cache when a new product is created.

```csharp
// In Features/Caching/CacheInvalidationHandler.cs
public class CacheInvalidationHandler : INotificationHandler<ProductCreatedNotification>
{
    public Task HandleAsync(ProductCreatedNotification notification, CancellationToken cancellationToken)
    {
        // Invalidate product cache logic here...
        Console.WriteLine($"Cache invalidated for product ID: {notification.ProductId}");
        return Task.CompletedTask;
    }
}
```

#### Audit Log Handler

This handler is responsible for writing an audit trail entry.

```csharp
// In Features/Auditing/AuditLogHandler.cs
public class AuditLogHandler : INotificationHandler<ProductCreatedNotification>
{
    public Task HandleAsync(ProductCreatedNotification notification, CancellationToken cancellationToken)
    {
        // Write to audit log logic here...
        Console.WriteLine($"Audit log: Product '{notification.ProductName}' created.");
        return Task.CompletedTask;
    }
}
```

## `IRequestHandler` 🆚 `INotificationHandler` 

| Feature             | `IRequestHandler`                | `INotificationHandler`                 |
| ------------------- | -------------------------------- | -------------------------------------- |
| **Purpose**         | Request–Response (command/query) | Publish–Subscribe (event)              |
| **Mediator Method** | `Send()`                         | `Publish()`                            |
| **Return Type**     | Has a response (`TResponse`)     | No response (`void` / `Task`)          |
| **Handlers**        | One handler per request          | Multiple handlers per notification     |
| **Use Case**        | Execute logic and return a value | Broadcast an event to many subscribers |


:::tip

This is fundamentally different from an `IRequestHandler`, which has a one-to-one relationship with its request.
An `INotification` can have zero, one, or many handlers.

:::