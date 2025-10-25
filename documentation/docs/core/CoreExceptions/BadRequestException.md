---
title: BadRequestException
sidebar_position: 4
---

```bash
Namespace: RA.Utilities.Core.Exceptions
```

The `BadRequestException` is a semantic exception used to represent failures caused by invalid client-side input.
It is designed to be caught and translated into a standardized **HTTP 400 Bad Request** response.

## 🎯 Purpose

This exception is the primary tool for handling validation errors discovered within your business logic (e.g., in a CQRS handler or service). Instead of throwing a generic exception, you can use `BadRequestException` to encapsulate detailed, structured information about what went wrong.

When used with the `Result` pattern, a `BadRequestException` is placed inside a `Failure` result. The API layer can then use this structured information to provide a rich, informative error response to the client, making it easy for developers and end-users to understand and correct the problem.

## `ValidationErrors` Class

The power of `BadRequestException` comes from its `Errors` property, which is an array of `ValidationErrors` objects. Each `ValidationErrors` object provides context about a single validation failure.

| Property         | Type     | Description                                                              |
|------------------|----------|--------------------------------------------------------------------------|
| **PropertyName**   | `string` | The name of the property that failed validation (e.g., "EmailAddress").  |
| **ErrorMessage**   | `string` | A user-friendly message describing the error (e.g., "Email is not valid."). |
| **AttemptedValue** | `object` | The actual value that was provided and caused the failure.               |
| **ErrorCode**      | `string` | A custom, machine-readable error code (e.g., "INVALID_FORMAT").          |

## 🚀 How to Use

Typically, you will create a `BadRequestException` within a service or handler when input validation fails.

### Example: Using with FluentValidation

A common use case is to catch a `ValidationException` from FluentValidation and convert it into a `BadRequestException`.

```csharp showLineNumbers
// In a CQRS handler or application service
using FluentValidation;
using RA.Utilities.Core;
// highlight-next-line
using RA.Utilities.Core.Exceptions;

public async Task<Result<UserDto>> CreateUserAsync(CreateUserCommand command)
{
    try
    {
        // Assume _validator is a FluentValidation validator
        await _validator.ValidateAndThrowAsync(command);
    }
    catch (ValidationException ex)
    {
        // Convert FluentValidation errors to our custom ValidationErrors
        var validationErrors = ex.Errors.Select(e => new ValidationErrors
        {
            PropertyName = e.PropertyName,
            ErrorMessage = e.ErrorMessage,
            AttemptedValue = e.AttemptedValue,
            ErrorCode = e.ErrorCode
        }).ToArray();

        // Return a failure Result containing the structured exception
        // highlight-next-line
        return new BadRequestException(validationErrors);
    }

    // ... continue with user creation logic if validation passes
}
```

### Example JSON Output

When the API layer (using `ErrorResultResponse`) handles the `Result` from the example above, it will automatically generate a `400 Bad Request` response with a body like this:

```json showLineNumbers
{
  "responseCode": 400,
  "responseType": "BadRequest",
  "responseMessage": "The request is invalid.",
  "result": [
    {
      "propertyName": "EmailAddress",
      "errorMessage": "'Email Address' is not a valid email address.",
      "attemptedValue": "not-an-email",
      "errorCode": "EmailValidator"
    }
  ]
}
```