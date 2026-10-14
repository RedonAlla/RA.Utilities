---
title: ValidationUtilities
sidebar_position: 4
---

```bash
Namespace: RA.Utilities.Application.Validation.Utilities
```

`ValidationUtilities` is a static helper that streamlines request validation with [`FluentValidation`](https://docs.fluentvalidation.net/en/latest/).
It exposes two methods that together cover the whole validation pipeline: running validators and turning failures into a standardized exception.

### 1. `ValidateAsync<TRequest>`

This method is the validation orchestrator. It takes a request object (a command, query, or API model) and runs it against every supplied [`FluentValidation`](https://docs.fluentvalidation.net/en/latest/) validator:

* It accepts a generic request (`TRequest`), a collection of validators (`IEnumerable<IValidator<TRequest>>`), and a required `CancellationToken` (pass `CancellationToken.None` to opt out).
* It executes the validators asynchronously in parallel using `Task.WhenAll`, with fast paths: an empty validator list returns immediately and a single validator is awaited directly.
* **Each validator gets its own `ValidationContext`** — validators never share mutable state, so failures cannot leak between validators and results are thread-safe.
* It forwards the cancellation token to every validator, enabling cooperative cancellation for async rules.
* It aggregates the failures into a single, flat array of `ValidationFailure` objects, preserving validator order.

```csharp showLineNumbers
/// <summary>
/// Validates a request using a collection of FluentValidation validators.
/// </summary>
public static async Task<ValidationFailure[]> ValidateAsync<TRequest>(
    TRequest request,
    IEnumerable<IValidator<TRequest>> validators,
    CancellationToken cancellationToken
)
{
    // DI already materializes the validator list as an array; the fallback keeps the method
    // working with any IEnumerable without allocating a fresh snapshot on every request.
    IValidator<TRequest>[] validatorArray =
        validators as IValidator<TRequest>[] ?? validators.ToArray();

    if (validatorArray.Length == 0)
    {
        return [];
    }

    // Fast path: a single validator skips the Task.WhenAll machinery entirely.
    if (validatorArray.Length == 1)
    {
        ValidationResult result =
            await validatorArray[0].ValidateAsync(new ValidationContext<TRequest>(request), cancellationToken).ConfigureAwait(false);

        return result.IsValid ? [] : [.. result.Errors];
    }

    // Each validator gets its own context: FluentValidation's ValidationResult wraps the
    // context's failure list, so sharing one context would leak every validator's failures
    // into every result.
    var validationTasks = new Task<ValidationResult>[validatorArray.Length];
    for (int i = 0; i < validatorArray.Length; i++)
    {
        validationTasks[i] = validatorArray[i].ValidateAsync(new ValidationContext<TRequest>(request), cancellationToken);
    }

    ValidationResult[] validationResults =
        await Task.WhenAll(validationTasks).ConfigureAwait(false);

    ValidationFailure[] validationFailures = [.. validationResults
        .Where(validationResult => !validationResult.IsValid)
        .SelectMany(validationResult => validationResult.Errors)];

    return validationFailures;
}
```

### 2. `CreateValidationErrorResult`

This method handles what happens after validation fails: it converts the raw [`FluentValidation`](https://docs.fluentvalidation.net/en/latest/) output into a standardized application exception.

* It takes the array of `ValidationFailure` objects produced by `ValidateAsync`.
* It transforms each failure into a [`ValidationError`](../../core/RA.Utilities.Core.Exceptions/BadRequestException.md#validationerror-class) DTO (`PropertyName`, `ErrorMessage`, `AttemptedValue`, `ErrorCode`).
* It wraps the structured errors in a [`BadRequestException`](../../core/RA.Utilities.Core.Exceptions/BadRequestException.md) — typically thrown so a global exception handler produces a consistent, machine-readable `HTTP 400 Bad Request` response.

```csharp showLineNumbers
/// <summary>
/// Creates a <see cref="BadRequestException"/> from an array of validation failures.
/// </summary>
public static BadRequestException CreateValidationErrorResult(ValidationFailure[] validationFailures)
{
    ValidationError[] validationErrors = [.. validationFailures.Select(f => new ValidationError(f.ErrorMessage)
    {
        PropertyName = f.PropertyName,
        AttemptedValue = f.AttemptedValue,
        ErrorCode = f.ErrorCode,
    })];

    return new BadRequestException(validationErrors);
}
```

> **Note**: since v10.0.1 this method maps failures to `ValidationError` (singular). See the [migration guide](./migration-guides) if you upgrade from 10.0.0 and reference the old `ValidationErrors` type.

## Usage Example

```csharp showLineNumbers
using RA.Utilities.Application.Validation.Utilities;

ValidationFailure[] failures = await ValidationUtilities.ValidateAsync(request, validators, CancellationToken.None);

if (failures.Length > 0)
    throw ValidationUtilities.CreateValidationErrorResult(failures);
```

## Summary

`ValidationUtilities` provides a reusable, consistent pattern for request validation: it decouples the validation logic (the "what") from error handling and response generation (the "how"). It is the engine behind the [`ValidationBehavior`](../Feature/Behaviors/ValidationBehavior) in `RA.Utilities.Feature`, which wires it into the pipeline automatically — but it works just as well standalone in any entry point.
