---
sidebar_position: 2
---

```powershell
Namespace: RA.Utilities.Dispatcher.Behaviors
```

`ValidationBehavior` is a pipeline behavior that runs all registered FluentValidation validators against the incoming request **before** the handler executes. Invalid requests never reach your business logic — the behavior short-circuits the pipeline and returns a failed result.

## 📦 Type Definition

```csharp
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators);
}

public class ValidationBehavior<TRequest> : IPipelineBehavior<TRequest>
    where TRequest : IRequest
{
    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators);
}
```

## 🎯 How It Works

1. The behavior resolves **all** registered `IValidator<TRequest>` implementations (empty collection when none are registered).
2. It runs them through `ValidationUtilities.ValidateAsync(...)` — the same asynchronous validation path used across RA.Utilities.
3. If validation **passes**, it calls `next()` and returns the inner result unchanged.
4. If validation **fails**, it returns `Result.Failure(...)` containing a `BadRequestException` built from the collected `ValidationFailure` entries (via `ValidationUtilities.CreateValidationErrorResult`). Map that failed result to a `400 Bad Request` at the call site.

## 🚀 Usage

`.AddValidator<TValidator>()` registers the validator **and** wires this behavior into the pipeline in one call:

```csharp
public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Price).GreaterThan(0);
    }
}

builder.Services
    .AddDispatcher<CreateProductCommand, int, CreateProductHandler>()
    .AddValidator<CreateProductCommandValidator>(); // registers validator + ValidationBehavior
```

Handling the failure at the call site:

```csharp
Result<int> result = await dispatcher.Send(command);
if (result.IsFailure)
    return Results.BadRequest(result.Exception.Message); // BadRequestException with structured errors
```

## 🧠 Notes

- Multiple validators for the same request are all executed.
- To register a validator **without** the behavior, use `AddTransient<IValidator<TRequest>, TValidator>()` directly.
