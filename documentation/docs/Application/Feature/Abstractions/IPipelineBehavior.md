---
sidebar_position: 3
---

```powershell
Namespace: RA.Utilities.Feature.Abstractions
```

The `IPipelineBehavior` interface defines a component that can participate in the processing of a request, allowing for the implementation of cross-cutting concerns.
It is conceptually similar to middleware in the ASP.NET Core request pipeline.

## 🎯 Purpose

The primary purpose of `IPipelineBehavior` is to intercept an incoming request (`IRequest`) on its way to its handler (`IRequestHandler`) and execute some logic both **before** and **after** the request is handled.

This allows you to encapsulate logic that applies to many different requests without duplicating that code in every single handler.
Common examples of cross-cutting concerns implemented with pipeline behaviors include:

-   **Validation**: Validating a request before it reaches the handler.
-   **Logging**: Logging request data and handler execution time.
-   **Caching**: Implementing a cache-aside pattern.
-   **Authorization**: Performing security checks.
-   **Transactional Behavior**: Wrapping handler execution in a database transaction.

## ⚙️ How It Works

A class that implements `IPipelineBehavior` must define a `HandleAsync` method. This method receives the `request` and a `next` delegate. The `next` delegate represents the next behavior in the pipeline, or the final request handler itself.

The `IPipelineBehavior` interface has a single method, `HandleAsync`, which receives three key parameters:

1. `request`: The incoming command or query.
2. `next`: A delegate that represents the next action in the pipeline.
Calling await `next()` passes control to the next behavior, or to the final request handler if it's the last behavior in the chain.
3. `cancellationToken`: The standard cancellation token.

A behavior can:
1.  Execute code **before** calling `await next()`.
2.  Choose to **short-circuit** the pipeline by returning a response without calling `next()`.
3.  Execute code **after** `await next()` has completed.

## 🧠 How pipeline ordering currently works

:::info
Pipelines are executed in **reverse order of registration** — the first registered behavior is outermost, the last registered is innermost (closest to the handler).
:::

```csharp
// From Mediator.cs — pipeline composition via Reverse().Aggregate()
RequestHandlerDelegate<TResponse> next = behaviors
    .Reverse()
    .Aggregate((RequestHandlerDelegate<TResponse>)HandlerDelegate,
        (nextDelegate, behavior) => () => behavior.HandleAsync(request, nextDelegate, cancellationToken));
```

That means:
The **first registered** behavior is **outermost**
The **last registered** behavior is **innermost**

Example:
```csharp
.AddDecoration<LoggingBehavior>()
.AddDecoration<ValidationBehavior>()
```

```
Logging → Validation → Handler
```

## 🚀 Usage Example

The `RA.Utilities.Feature` package includes a [`ValidationBehavior`](../Behaviors/ValidationBehavior.md) that demonstrates the power of this pattern.

```csharp
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators) =>
        _validators = validators ?? Array.Empty<IValidator<TRequest>>();

    public async Task<Result<TResponse>> HandleAsync(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // 1. Execute validation BEFORE the next delegate.
        var validationFailures = await ValidationUtilities.ValidateAsync(request, _validators);

        if (validationFailures.Length > 0)
        {
            // 2. Short-circuit the pipeline if validation fails.
            return ValidationUtilities.CreateValidationErrorResult(validationFailures);
        }

        // 3. Call the next delegate in the pipeline (the handler or next behavior).
        return await next();
    }
}
```

In this example, if validation fails, the handler is never executed, ensuring that your business logic only ever deals with valid data.

## 🧠 Summary
In summary, `IPipelineBehavior` is a fundamental pattern for building clean, maintainable, and robust applications by separating business logic from cross-cutting concerns.

