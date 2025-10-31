---
sidebar_position: 1
---

```powershell
Namespace: RA.Utilities.Feature.Behaviors
```

The `ValidationBehavior` is an implementation of the [`IPipelineBehavior`](../Abstractions/IPipelineBehavior.md) interface that automatically validates incoming requests using [`FluentValidation`](https://docs.fluentvalidation.net/en/latest/).

## 🎯 Purpose

`ValidationBehavior` is a crucial implementation of the [`IPipelineBehavior`](../Abstractions/IPipelineBehavior.md) interface. 
The primary purpose of the `ValidationBehavior` is to act as a validation gatekeeper for the CQRS pipeline.
It ensures that no request reaches its handler unless it passes all defined validation rules.
This is a critical component for building robust and secure applications.

## 🔑 Key Benefits:

1.  **Clean Handlers**: Your [`IRequestHandler`](../Abstractions/IRequestHandler.md) implementations are freed from the responsibility of validation, allowing them to focus purely on business logic.
2.  **Short-Circuiting**: If validation fails, the behavior immediately stops the pipeline and returns a structured error response. The handler is never executed with invalid data.
3.  **Centralized Logic**: Validation rules are defined in dedicated [`FluentValidation`](https://docs.fluentvalidation.net/en/latest/) classes, keeping them separate from the business logic and making them easy to manage and reuse.
4.  **Consistent Errors**: It guarantees that all validation failures across the application result in a consistent, predictable error response format.

## ⚙️ How It Works

The behavior is registered in the dependency injection container as part of the CQRS pipeline.
For every request sent through the mediator:

1.  **Intercepts the Request**: The `ValidationBehavior` intercepts the [`IRequest`](../Abstractions/IRequest.md) before it reaches its handler.
2.  **Resolves Validators**: It resolves all registered `IValidator<TRequest>` implementations for the specific request type from the DI container.
3.  **Executes Validation**: It runs the `ValidateAsync` method on all resolved validators.
4.  **Checks the Result**:
    - If there are no validation errors, it calls `await next()` to pass the request along the pipeline.
    - If there are validation errors, it constructs a `Result.Failure` containing a [`ValidationException`](../../../core/CoreExceptions/) and returns it immediately.

## 🚀 Usage Example

To use the behavior, you need to:

#### 1. Create a Validator:
Define a validator for your command or query using [`FluentValidation`](https://docs.fluentvalidation.net/en/latest/).

```csharp showLineNumbers
public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Price).GreaterThan(0);
    }
}
```

#### 2. Register Services
In your `Program.cs`, register the `ValidationBehavior` and your validators.

  ```csharp showLineNumbers
  // Program.cs
  using RA.Utilities.Feature.Behaviors;
  using FluentValidation;

  var services = builder.Services;

  // Register the pipeline behavior
  services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

  // Scan the assembly and register all validators
  services.AddValidatorsFromAssembly(typeof(CreateProductCommandValidator).Assembly);
  ```

With this setup, any `CreateProductCommand` sent through the mediator will be automatically validated.
