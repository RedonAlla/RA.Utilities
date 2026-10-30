---
sidebar_position: 1
---

```powershell
Namespace: RA.Utilities.Dispatcher.Handlers
```

`RequestHandler` is an abstract base class that implements the [`IRequestHandler`](../Abstractions/IRequestHandler.md) interface. It implements the interface's plumbing so derived handlers only override a single `HandleAsync` method with their business logic.

The base classes are pure passthroughs: **no exception-to-`Result` wrapping** (exceptions you throw propagate; expected failures are returned as `Result.Failure(...)`), and no logger is required.

## 📦 Two Variants

| Class | Implements | Abstract Method Return |
|---|---|---|
| `RequestHandler<TRequest>` | `IRequestHandler<TRequest>` | `ValueTask<Result>` |
| `RequestHandler<TRequest, TResponse>` | `IRequestHandler<TRequest, TResponse>` | `ValueTask<Result<TResponse>>` |

## ⚙️ Members

Each base class declares:

1. **A public abstract `HandleAsync`** — `ValueTask<Result<TResponse>> HandleAsync(TRequest, CancellationToken)` (or `ValueTask<Result>` for void requests). This is the single method you override with your business logic.
2. **A protected virtual context-aware overload** — `HandleAsync<TContext>(TRequest, PipelineContext<TContext>, CancellationToken)`, which by default delegates to the abstract method. Override it to consume [`PipelineContext<T>`](../Models/PipelineContext.md) data.

The interface's `HandleAsync` methods are implemented **explicitly** and forward straight to your overrides — there is no logging and no try-catch around them.

## 🚀 Complete Example

### `RequestHandler<TRequest, TResponse>` (with response)

```csharp
using RA.Utilities.Core.Exceptions;
using RA.Utilities.Core.Results;
using RA.Utilities.Dispatcher.Handlers;

public class GetProductHandler : RequestHandler<GetProductQuery, Product>
{
    private readonly IProductRepository _repository;

    public GetProductHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public override async ValueTask<Result<Product>> HandleAsync(
        GetProductQuery query, CancellationToken cancellationToken)
    {
        var product = await _repository.FindByIdAsync(query.ProductId, cancellationToken);

        if (product is null)
        {
            return Result.Failure<Product>(new NotFoundException(nameof(Product), query.ProductId));
        }

        return Result.Success(product);
    }
}
```

### `RequestHandler<TRequest>` (no response)

```csharp
using RA.Utilities.Core.Results;
using RA.Utilities.Dispatcher.Handlers;

public class DeleteProductHandler : RequestHandler<DeleteProductCommand>
{
    private readonly IProductRepository _repository;

    public DeleteProductHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public override async ValueTask<Result> HandleAsync(
        DeleteProductCommand command, CancellationToken cancellationToken)
    {
        await _repository.RemoveAsync(command.ProductId, cancellationToken);
        return Result.Success();
    }
}
```

## 🧠 Summary

`RequestHandler` removes the interface-implementation boilerplate from your handlers — override one `HandleAsync` method, return results, and express expected failures as `Result.Failure(...)`. Handlers are auto-registered by the source generator when `AddDispatcher()` is called. For request-level logging, register the [`LoggingBehavior`](../Behaviors/LoggingBehavior.md) on the dispatch instead.
