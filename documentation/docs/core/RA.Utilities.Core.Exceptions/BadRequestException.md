---
title: BadRequestException
sidebar_position: 4
---

```bash
Namespace: RA.Utilities.Core.Exceptions
```

`BadRequestException` represents a failure caused by invalid client-side input. The API layer maps it to **HTTP 400 Bad Request**.

## 🎯 Purpose

This is the primary tool for validation errors discovered inside business logic (for example, in a CQRS handler or application service). Instead of a generic exception with a single string, `BadRequestException` carries a structured array of [`ValidationError`](./ValidationError.md) items so the client can see exactly which fields failed and why.

Use it when the request itself is invalid. When the request is well formed but the resource state prevents the operation, use [`UnprocessableException`](./UnprocessableException.md) instead.

## Properties

| Property | Type | Description |
|---|---|---|
| **`Errors`** | `ValidationError[]` | The validation failures that caused the exception. |
| `ErrorCode` | `int` | Inherited. `400` unless supplied explicitly. |
| `ResponseType` | `ResponseType` | Inherited. `ResponseType.BadRequest`. |
| `Message` | `string` | Inherited. `BaseResponseMessages.BadRequest` (`"The request is invalid."`) unless supplied explicitly. |

Each [`ValidationError`](./ValidationError.md) exposes `PropertyName`, `ErrorMessage`, `AttemptedValue`, `ExpectedValue`, and a custom `ErrorCode`.

## Constructors

| Constructor | `ErrorCode` | `Message` |
|---|---|---|
| `BadRequestException(ValidationError error)` | `400` | `BaseResponseMessages.BadRequest` |
| `BadRequestException(ValidationError error, int errorCode, string? message = null)` | supplied | supplied, or `BaseResponseMessages.BadRequest` when `null`/omitted |
| `BadRequestException(ValidationError[] errors)` | `400` | `BaseResponseMessages.BadRequest` |
| `BadRequestException(ValidationError[] errors, int errorCode, string? message = null)` | supplied | supplied, or `BaseResponseMessages.BadRequest` when `null`/omitted |

There is no message-only constructor: a `ValidationError` is always required.

## 🚀 How to Use

### Example: Using with FluentValidation

Catch a FluentValidation `ValidationException` and convert its failures into `ValidationError` items:

```csharp showLineNumbers
using FluentValidation;
using RA.Utilities.Core.Results;
using RA.Utilities.Core.Exceptions;

public async Task<Result<UserDto>> CreateUserAsync(CreateUserCommand command)
{
    try
    {
        await _validator.ValidateAndThrowAsync(command);
    }
    catch (ValidationException ex)
    {
        ValidationError[] validationErrors = [.. ex.Errors.Select(e => new ValidationError(e.ErrorMessage)
        {
            PropertyName = e.PropertyName,
            AttemptedValue = e.AttemptedValue,
            ErrorCode = e.ErrorCode
        })];

        // highlight-next-line
        return new BadRequestException(validationErrors);
    }

    // ... continue with user creation logic if validation passes
}
```

### Example: A single validation error

```csharp showLineNumbers
using RA.Utilities.Core.Constants;
using RA.Utilities.Core.Exceptions;

if (string.IsNullOrWhiteSpace(request.Email))
{
    // highlight-next-line
    throw new BadRequestException(new ValidationError(BaseErrorMessage.EmailRequired)
    {
        PropertyName = "Email",
        ErrorCode = BaseErrorCode.EmailRequired
    });
}
```

### Example: Attach a request-level code and message

```csharp showLineNumbers
return new BadRequestException(
    errors: validationErrors,
    errorCode: 4001,
    message: "The registration form is invalid."
);
```

## 🔗 HTTP mapping

`ErrorResultResponse` maps `BadRequestException` to **HTTP 400** with a `BadRequestResponse` whose `result` is an array of the exception's `Errors`. The `ErrorCode` is echoed into `responseCode`; it does not change the HTTP status.

See [`BadRequestResponse`](../../api/RA.Utilities.Api.Response/BadRequestResponse.md) for the exact JSON shape and the [`ValidationError`](./ValidationError.md) reference for each item's fields.
