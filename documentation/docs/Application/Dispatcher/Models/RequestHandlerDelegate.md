---
sidebar_position: 2
---

```powershell
Namespace: RA.Utilities.Dispatcher.Models
```

The `RequestHandlerDelegate` delegates are what [`IPipelineBehavior`](../Abstractions/IPipelineBehavior.md) receives as its `next` parameter — invoking them runs the next stage of the request pipeline.

## 📦 Type Definition

```csharp
/// <summary>Represents a delegate for handling a request without a response.</summary>
public delegate ValueTask<Result> RequestHandlerDelegate();

/// <summary>Represents a delegate for handling a request with a response.</summary>
public delegate ValueTask<Result<TResponse>> RequestHandlerDelegate<TResponse>();

/// <summary>Context-aware variant for a request without a response.</summary>
public delegate ValueTask<Result> RequestHandlerContextDelegate<TContext>(PipelineContext<TContext> context)
    where TContext : class, new();

/// <summary>Context-aware variant for a request with a response.</summary>
public delegate ValueTask<Result<TResponse>> RequestHandlerContextDelegate<TResponse, TContext>(PipelineContext<TContext> context)
    where TContext : class, new();
```

## 🎯 Purpose

The dispatcher composes pipeline behaviors around the handler into a delegate chain. Each behavior's `HandleAsync` receives a delegate that invokes the next behavior — or the handler itself, innermost. The delegate returns `ValueTask<Result<TResponse>>` (`ValueTask<Result>` for void requests): the handler's result on success, or `Result.Failure(...)` on expected failure.

| Behavior Interface | Delegate Parameter Type | Return Type |
|---|---|---|
| `IPipelineBehavior<TRequest>` | `RequestHandlerDelegate` | `ValueTask<Result>` |
| `IPipelineBehavior<TRequest, TResponse>` | `RequestHandlerDelegate<TResponse>` | `ValueTask<Result<TResponse>>` |

The context-aware variants are used by the `HandleAsync<TContext>` overloads; the default interface implementations adapt the plain delegate by closing over the context (`() => next(context)`).

## 🚀 Usage

```csharp
public class TimingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async ValueTask<Result<TResponse>> HandleAsync(
        TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        Result<TResponse> result = await next(); // runs the inner pipeline (or the handler)
        Console.WriteLine($"Handled in {stopwatch.ElapsedMilliseconds} ms");
        return result;
    }
}
```

## 🧠 Notes

- **No delegate chain without behaviors** — when a request has no pipeline behaviors, the generated dispatcher invokes the handler directly and no delegate is allocated at all.
- The delegate is what lets a behavior **short-circuit**: return a failed `Result` without calling `next()`.

## 🔗 Related

- [`IPipelineBehavior`](../Abstractions/IPipelineBehavior.md) — the consumer of this delegate.
- [`NotificationHandlerDelegate`](NotificationHandlerDelegate.md) — the notification-pipeline counterpart.
