---
sidebar_position: 4
---

```powershell
Namespace: RA.Utilities.Feature.Abstractions
```

The primary purpose of the `INotification` interface is to mark a class as a **notification** or **event**.
This is a key part of the CQRS and Mediator patterns for handling side effects and cross-cutting concerns in a clean, decoupled way.

While it might seem similar to `IRequest`, there is a fundamental difference in how they are handled:

* **`IRequest` (Command/Query)**: Represents a message that is sent to a single handler.
It's a one-to-one communication pattern.
You send a command and expect one handler to process it.
* **`INotification` (Event)**: Represents a message that is published to multiple handlers (zero or more).
It's a one-to-many, "publish-subscribe" communication pattern.
You publish an event, and any part of your application that cares about that event can handle it without the publisher needing to know about the subscribers.

## 🛠️ How It's Used

In the context of your `RA.Utilities` solution, `INotification` enables you to trigger side effects after a primary operation completes.

For example, consider the `CreateProductHandler`.
After a product is successfully created, you might want to perform several follow-up actions:

1. Invalidate the product cache.
2. Send an email to the marketing team.
3. Write an entry to an audit log.
4. Instead of cluttering the `CreateProductHandler` with all this logic, you would publish a notification:

### Step 1: Define the Notification
First, you would define a class that implements `INotification`.

```csharp
// In a new file: Features/Products/ProductCreatedNotification.cs
using RA.Utilities.Feature.Models; // Assuming INotification is aliased or available here

public record ProductCreatedNotification(int ProductId, string ProductName) : INotification;
```

### Step 2: Publish the Notification
The CreateProductHandler would publish this notification after successfully saving the product. (This requires injecting a Publisher or Mediator service).

```csharp
// In CreateProductHandler.cs
// ...
var productId = await _productRepository.AddAsync(newProduct);

// Publish an event for other parts of the system to react to
await _publisher.Publish(new ProductCreatedNotification(productId, newProduct.Name), cancellationToken);

return productId;
```

### Step 3: Create Handlers for the Notification
Now, you can create multiple, small, focused handlers that each subscribe to ProductCreatedNotification.

```csharp
// In Features/Caching/CacheInvalidationHandler.cs
public class CacheInvalidationHandler : INotificationHandler<ProductCreatedNotification>
{
    public Task Handle(ProductCreatedNotification notification, CancellationToken cancellationToken)
    {
        // Invalidate product cache logic here...
        return Task.CompletedTask;
    }
}

// In Features/Auditing/AuditLogHandler.cs
public class AuditLogHandler : INotificationHandler<ProductCreatedNotification>
{
    public Task Handle(ProductCreatedNotification notification, CancellationToken cancellationToken)
    {
        // Write to audit log logic here...
        return Task.CompletedTask;
    }
}
```


## 🧠 Summary

In summary, `INotification` is the key to implementing a robust, decoupled eventing system within your Vertical Slice Architecture,
allowing features to remain self-contained while still communicating with other parts of the application.