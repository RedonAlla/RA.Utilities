---
sidebar_position: 2
---

```powershell
Namespace: RA.Utilities.Application.Validation.Utilities
```

`ValidationUtilities` class is a static helper designed to streamline the process of validating application requests using the [`FluentValidation`](https://docs.fluentvalidation.net/en/latest/) library.
It serves two primary functions, neatly encapsulated in its two methods:

### 1. `ValidateAsync<TRequest>`

This method acts as a validation orchestrator. Its purpose is to take a request object (like a command, query, or API model) and run it against all relevant [`FluentValidation`](https://docs.fluentvalidation.net/en/latest/) validators.

Here's a breakdown of its logic:

* It accepts a generic request object (`TRequest`) and a collection of validators (`IEnumerable<IValidator<TRequest>>`).
* It efficiently executes all validators asynchronously using `Task.WhenAll`.
* It then aggregates any validation failures from all the validators into a single, flat array of `ValidationFailure` objects.

In short, it centralizes the logic for running multiple validation rule sets against a single object and collecting all the errors.

```csharp
/// <summary>
/// Validates a request using a collection of FluentValidation validators.
/// </summary>
public static async Task<ValidationFailure[]> ValidateAsync<TRequest>(TRequest request, IEnumerable<IValidator<TRequest>> validators)
{
    if (!validators.Any())
    {
        return [];
    }

    var context = new ValidationContext<TRequest>(request);

    ValidationResult[] validationResults = await Task.WhenAll(
        validators.Select(validator => validator.ValidateAsync(context)));

    ValidationFailure[] validationFailures = [.. validationResults
        .Where(validationResult => !validationResult.IsValid)
        .SelectMany(validationResult => validationResult.Errors)];

    return validationFailures;
}
```

### 2. `CreateValidationErrorResult`

This method handles what happens after validation fails.
Its purpose is to convert the raw output from [`FluentValidation`](https://docs.fluentvalidation.net/en/latest/) into a standardized application-specific exception.

Here's how it works:

It takes the array of `ValidationFailure` objects produced by `ValidateAsync`.
It transforms each failure into a custom `ValidationErrors` data structure, which is a cleaner Data Transfer Object (DTO) for representing an error.
It then wraps these structured errors inside a [`BadRequestException`](../../core//RA.Utilities.Core.Exceptions/BadRequestException.md).
This custom exception is likely handled by a global exception handler in the API to produce a consistent, machine-readable `HTTP 400 Bad Request` response for the client.

```csharp
/// <summary>
/// Creates a <see cref="BadRequestException"/> from an array of validation failures.
/// </summary>
public static BadRequestException CreateValidationErrorResult(ValidationFailure[] validationFailures)
{
    ValidationErrors[] validationErrors = [.. validationFailures.Select(f => new ValidationErrors
    {
        PropertyName = f.PropertyName,
        ErrorMessage = f.ErrorMessage,
        AttemptedValue = f.AttemptedValue,
        ErrorCode = f.ErrorCode,
    })];

    return new BadRequestException(validationErrors);
}
```

## Summary
In essence, `ValidationUtilities` provides a reusable and consistent pattern for handling request validation within your application. It decouples the validation logic (the "what") from the error handling and response generation (the "how"), promoting cleaner code in your application's entry points (like MediatR pipeline behaviors or API controllers).