---
sidebar_position: 1
---

```powershell
Namespace: RA.Utilities.Dispatcher.Abstractions
```

The `IRequest` and `IRequest<TResponse>` marker interfaces identify your **requests** — the commands and queries of your CQRS application. They carry no members; their job is to give the dispatcher a compile-time contract to dispatch on.

## 📦 Type Definition

```csharp
/// <summary>
/// A marker interface for a feature (command or query).
/// This is used for features that do not return a value.
/// </summary>
public interface IRequest;

/// <summary>
/// A marker interface for a feature (command or query) that returns a value.
/// </summary>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public interface IRequest<TResponse> : IRequest;
```

Note the difference from some other libraries: `TResponse` is your **plain payload type** — the dispatcher wraps it in `Result<TResponse>` when it returns.

## 🚀 Usage

```csharp
// A command with no return value
public record DeleteProductCommand(int ProductId) : IRequest;

// A query that returns data
public record GetProductQuery(int ProductId) : IRequest<Product>;

// A command that returns the created ID
public record CreateProductCommand(string Name, decimal Price) : IRequest<int>;
```

## 🔗 Related

- [`IRequestHandler<TRequest>` / `IRequestHandler<TRequest, TResponse>`](IRequestHandler.md) — the handler contract for requests.
- [`RequestHandler`](../Handlers/RequestHandler.md) — abstract base classes that implement the handler plumbing.
- [`INotification`](INotification.md) — the counterpart marker for one-to-many notifications.
