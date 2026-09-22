---
sidebar_position: 1
---

```powershell
Namespace: RA.Utilities.Feature.Handlers
```

`RequestHandler` is an abstract base class that implements the [`IRequestHandler`](../Abstractions/IRequestHandler.md) interface. It implements the interface's plumbing so derived handlers only override a single `HandleAsync` method with their business logic.

Since v10.2.0 the base classes are pure passthroughs: **no exception-to-`Result` wrapping** (exceptions propagate to the caller), and no logger is required. Errors are reported by throwing typed exceptions from `RA.Utilities.Core.Exceptions`, which the API layer's `GlobalExceptionHandler` converts to HTTP responses.

## 📦 Two Variants

| Class | Implements | Abstract Method Return |
|---|---|---|
| `RequestHandler<TRequest>` | `IRequestHandler<TRequest>` | `Task` |
| `RequestHandler<TRequest, TResponse>` | `IRequestHandler<TRequest, TResponse>` | `Task<TResponse>` |

## ⚙️ Members

Each base class declares:

1. **A public abstract `HandleAsync`** — `Task<TResponse> HandleAsync(TRequest, CancellationToken)` (or `Task` for void requests). This is the single method you override with your business logic.
2. **A protected virtual context-aware overload** — `HandleAsync<TContext>(TRequest, PipelineContext<TContext>, CancellationToken)`, which by default delegates to the abstract method. Override it to consume [`PipelineContext<T>`](../Models/PipelineContext.md) data.

The interface's `HandleAsync` methods are implemented **explicitly** and forward straight to your overrides — there is no logging and no try-catch around them.

## 🚀 Complete Example

### `RequestHandler<TRequest, TResponse>` (with response)

```csharp
using RA.Utilities.Core.Exceptions;
using RA.Utilities.Feature.Handlers;

public class GetProductHandler : RequestHandler<GetProductQuery, Product>
{
    private readonly IProductRepository _repository;

    public GetProductHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public override async Task<Product> HandleAsync(
        GetProductQuery query, CancellationToken cancellationToken)
    {
        var product = await _repository.FindByIdAsync(query.ProductId, cancellationToken);

        if (product is null)
        {
            throw new NotFoundException(nameof(Product), query.ProductId);
        }

        return product;
    }
}
```

### `RequestHandler<TRequest>` (no response)

```csharp
using RA.Utilities.Feature.Handlers;

public class DeleteProductHandler : RequestHandler<DeleteProductCommand>
{
    private readonly IProductRepository _repository;

    public DeleteProductHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public override async Task HandleAsync(
        DeleteProductCommand command, CancellationToken cancellationToken)
    {
        await _repository.RemoveAsync(command.ProductId, cancellationToken);
    }
}
```

## 🧠 Summary

`RequestHandler` removes the interface-implementation boilerplate from your handlers — override one `HandleAsync` method, return values, and throw typed exceptions on failure. Handlers are auto-registered by the source generator when `AddMediator()` is called. For request-level logging, register the [`LoggingBehavior`](../Behaviors/LoggingBehavior.md) on the feature instead.
