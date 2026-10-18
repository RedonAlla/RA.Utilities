---
sidebar_position: 3
---

```powershell
Namespace: RA.Utilities.Dispatcher.Abstractions
```

`IPipelineBehavior<TRequest>` and `IPipelineBehavior<TRequest, TResponse>` let you inject cross-cutting concerns around request handler execution — validation, logging, transaction management, caching — without touching the handlers themselves.

## 📦 Type Definition

```csharp
public interface IPipelineBehavior<TRequest> where TRequest : IRequest
{
    ValueTask<Result> HandleAsync(TRequest request, RequestHandlerDelegate next, CancellationToken cancellationToken);

    ValueTask<Result> HandleAsync<TContext>(
        TRequest request, RequestHandlerContextDelegate<TContext> next,
        PipelineContext<TContext> context, CancellationToken cancellationToken)
        where TContext : class, new()
        => HandleAsync(request, () => next(context), cancellationToken); // default implementation
}

public interface IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    ValueTask<Result<TResponse>> HandleAsync(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken);

    ValueTask<Result<TResponse>> HandleAsync<TContext>(
        TRequest request, RequestHandlerContextDelegate<TResponse, TContext> next,
        PipelineContext<TContext> context, CancellationToken cancellationToken)
        where TContext : class, new()
        => HandleAsync(request, () => next(context), cancellationToken); // default implementation
}
```

## 🎯 How It Works

Behaviors form a chain around the handler. The dispatcher composes them in **registration order, outermost first**; the `next` delegate you receive invokes the next behavior — or the handler itself when you're innermost.

A behavior can:

- **Do work before and after** — e.g., start a stopwatch, await `next()`, then log the elapsed time.
- **Short-circuit** — return a failed `Result` without calling `next()` (that's exactly what `ValidationBehavior` does on invalid requests).
- **Transform** — inspect or replace the `Result` returned by the inner pipeline.

## 🚀 Custom Behavior Example

```csharp
public class TimingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async ValueTask<Result<TResponse>> HandleAsync(
        TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        Result<TResponse> result = await next();
        Console.WriteLine($"Handled in {stopwatch.ElapsedMilliseconds} ms");
        return result;
    }
}
```

## 🔧 Registration

Closed (non-generic) behaviors are **auto-registered** by the source generator under their interface. Generic behaviors (like the one above) must be registered explicitly:

```csharp
builder.Services
    .AddDispatcher<MyCommand, int, MyCommandHandler>()
    .AddDecoration<TimingBehavior<MyCommand, int>>();
```

See [Auto Registration](../auto-registration) for the discovery rules and the `DSAG010` diagnostic.

## 🔗 Related

- [`INotificationBehavior<TNotification>`](INotificationBehavior.md) — the counterpart for notification pipelines. See [Pipeline vs Notification](PipelineVsNotification.md) for the differences.
- [Built-in behaviors](../Behaviors/index.mdx) — logging and validation out of the box.
- [`RequestHandlerDelegate`](../Models/RequestHandlerDelegate.md) — the delegate passed as `next`.
