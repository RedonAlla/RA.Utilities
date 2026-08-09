---
sidebar_position: 1
---

```powershell
Namespace: RA.Utilities.Feature.Handlers
```

`RequestHandler` is an abstract base class that implements the [`IRequestHandler`](../Abstractions/IRequestHandler.md) interface. It provides automatic logging and exception-to-`Result` conversion, so derived handlers can focus purely on business logic.

## 📦 Two Variants

| Class | Implements | Abstract Method Return |
|---|---|---|
| `RequestHandler<TRequest>` | `IRequestHandler<TRequest>` | `Task<Result>` |
| `RequestHandler<TRequest, TResponse>` | `IRequestHandler<TRequest, TResponse>` | `Task<TResponse>` |

## 🔧 Constructor

```csharp
protected RequestHandler(ILogger logger)
```

The base class accepts an untyped `ILogger`. Pass your derived handler's typed logger (e.g., `ILogger<MyHandler>`) via `base(logger)`.

## ⚙️ Built-in Behavior

The interface method is implemented **explicitly**, providing a template that:

1. **Logs** `"[Handler] Start Handling {RequestType}"` at `Information` level
2. **Calls** your abstract `HandleAsync` override
3. **On success**: logs `"[Handler] Finished Handling {RequestType}"` and returns the result
4. **On exception**: logs `"[Handler] Failed Handling {RequestType}"` at `Error` level and returns `Result.Failure` (via implicit conversion from `Exception`)

## 🚀 Complete Example

### `RequestHandler<TRequest, TResponse>` (with response)

```csharp
using Microsoft.Extensions.Logging;
using RA.Utilities.Core.Results;
using RA.Utilities.Feature.Handlers;

public class GetProductHandler : RequestHandler<GetProductQuery, Result<Product>>
{
    private readonly IProductRepository _repository;

    public GetProductHandler(IProductRepository repository, ILogger<GetProductHandler> logger)
        : base(logger)
    {
        _repository = repository;
    }

    public override async Task<Result<Product>> HandleAsync(
        GetProductQuery query, CancellationToken cancellationToken)
    {
        var product = await _repository.FindByIdAsync(query.ProductId, cancellationToken);

        if (product is null)
        {
            return new NotFoundException(nameof(Product), query.ProductId);
        }

        return product;
    }
}
```

### `RequestHandler<TRequest>` (no response)

```csharp
public class DeleteProductHandler : RequestHandler<DeleteProductCommand>
{
    private readonly IProductRepository _repository;

    public DeleteProductHandler(IProductRepository repository, ILogger<DeleteProductHandler> logger)
        : base(logger)
    {
        _repository = repository;
    }

    public override async Task<Result> HandleAsync(
        DeleteProductCommand command, CancellationToken cancellationToken)
    {
        await _repository.RemoveAsync(command.ProductId, cancellationToken);
        return Result.Success();
    }
}
```

## 🧠 Summary

`RequestHandler` eliminates the boilerplate of logging and `try-catch` in every handler. Inherit from it, override `HandleAsync`, and write your business logic — the base class handles the rest.
