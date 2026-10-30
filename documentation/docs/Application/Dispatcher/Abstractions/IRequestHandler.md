---
sidebar_position: 2
---

```powershell
Namespace: RA.Utilities.Dispatcher.Abstractions
```

`IRequestHandler<TRequest>` and `IRequestHandler<TRequest, TResponse>` are the interfaces your request handlers implement. They define a single abstract member, `HandleAsync`, plus a context-aware overload with a default implementation.

## 📦 Type Definition

```csharp
public interface IRequestHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    ValueTask<Result<TResponse>> HandleAsync(TRequest request, CancellationToken cancellationToken);

    ValueTask<Result<TResponse>> HandleAsync<TContext>(
        TRequest request, PipelineContext<TContext> context, CancellationToken cancellationToken)
        where TContext : class, new()
        => HandleAsync(request, cancellationToken); // default implementation
}

public interface IRequestHandler<in TRequest> where TRequest : IRequest
{
    ValueTask<Result> HandleAsync(TRequest request, CancellationToken cancellationToken);

    ValueTask<Result> HandleAsync<TContext>(
        TRequest request, PipelineContext<TContext> context, CancellationToken cancellationToken)
        where TContext : class, new()
        => HandleAsync(request, cancellationToken); // default implementation
}
```

## 🎯 Purpose

Handlers contain your business logic. They return results:

- **Success** — `Result.Success(value)` / `Result.Success()`.
- **Expected failure** — `Result.Failure(exception)` with a typed exception from `RA.Utilities.Core.Exceptions` (`NotFoundException`, `ConflictException`, `BadRequestException`, ...). Failures are returned, not thrown — truly exceptional cases still throw.

## 🚀 Usage

```csharp
using RA.Utilities.Core.Exceptions;
using RA.Utilities.Core.Results;
using RA.Utilities.Dispatcher.Abstractions;

public class CreateProductHandler : IRequestHandler<CreateProductCommand, int>
{
    public async ValueTask<Result<int>> HandleAsync(
        CreateProductCommand command, CancellationToken cancellationToken)
    {
        if (await _repository.DoesProductExistAsync(command.Name, cancellationToken))
            return Result.Failure<int>(new ConflictException(nameof(Product), command.Name));

        return Result.Success(await _repository.AddAsync(command, cancellationToken));
    }
}
```

## 🔧 Auto-Registration

Concrete, non-generic `IRequestHandler` implementations are discovered by the package's source generator and registered automatically when the parameterless `AddDispatcher()` is called (request handlers **scoped**) — no explicit `AddDispatcher<...>` builder call needed. See the [Auto Registration](../auto-registration) guide for the discovery rules and diagnostics.

## 🔗 Related

- [`RequestHandler`](../Handlers/RequestHandler.md) — abstract base classes that remove the interface plumbing.
- [`IRequest` / `IRequest<TResponse>`](IRequest.md) — the request marker interfaces.
