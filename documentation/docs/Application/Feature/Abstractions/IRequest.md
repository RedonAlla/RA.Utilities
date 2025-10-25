---
sidebar_position: 1
---

```powershell
Namespace: RA.Utilities.Feature.Abstractions
```

In the MediatR pattern, `IRequest` and its generic counterpart `IRequest<TResponse>` are used to define the "messages" that your application sends. These messages can be either Commands (requests to change state) or Queries (requests to read data).

The purpose of inheriting from `IRequest` is to "mark" a class as a message that can be dispatched through the ***MediatR*** pipeline to a corresponding handler.

There are two primary versions:

1. **IRequest**: A marker interface for a feature (command or query).
This is used for features that do not return a value.
It's used for "fire-and-forget" operations.
2. **`IRequest<TResponse>`**: This is the most common one.
It marks a class as a message that, when processed by its handler, will return a value of type `TResponse`.

## 🛠️ How It's Used 

```csharp
// The command containing the data for the new product
public record CreateProductCommand(string Name, decimal Price) : IRequest<Result<int>>;
```
Let's break this down:

* **CreateProductCommand**: This is a C# record that holds the data needed to create a product.
* **`: IRequest<Result<int>>`**: By inheriting from `IRequest<Result<int>>`, this record is marked as a MediatR message.
This declaration tells the system two things:
  1. This is a request that can be sent via MediatR.
  2. When it is successfully handled, the expected response will be a `Result<int>` (which comes from your `RA.Utilities.Core` package).


🧠 Summary

In summary, `IRequest` is the foundational interface from MediatR that enables the entire CQRS pattern within your architecture.
It defines the messages that trigger your business logic, allowing you to create clean, decoupled, and highly maintainable vertical slices for each feature.
