---
title: ValidationError
sidebar_position: 5
---

```bash
Namespace: RA.Utilities.Core.Exceptions
```

`ValidationError` describes a single validation failure. It is the element type of [`BadRequestException.Errors`](./BadRequestException.md), not an exception itself.

## 🎯 Purpose

A `BadRequestException` can report several failures at once. `ValidationError` gives each failure the context a client needs to fix it: which property failed, what was submitted, what was expected, and a stable machine-readable code.

The API layer projects these items directly into the `result` array of a `BadRequestResponse`.

## Properties

| Property | Type | Required | Description |
|---|---|---|---|
| **`ErrorMessage`** | `string` | ✅ constructor argument | Human-readable description of the failure. |
| **`PropertyName`** | `string?` | ❌ | Name of the property that failed validation (e.g., `"Email"`). Omitted from JSON when `null`. |
| **`AttemptedValue`** | `object?` | ❌ | Value supplied by the client that caused the failure. Omitted from JSON when `null`. |
| **`ExpectedValue`** | `object?` | ❌ | Value or shape that was expected, when applicable. Omitted from JSON when `null`. |
| **`ErrorCode`** | `string?` | ❌ | Machine-readable code for the failure (e.g., `"REQUIRED_EMAIL"`). Omitted from JSON when `null`. |

All properties use `{ get; init; }`. `ErrorMessage` is set through the constructor; the others are optional object-initializer values.

## Constructors

| Constructor | Description |
|---|---|
| `ValidationError(string errorMessage)` | Creates an error with the required message. All other properties start as `null`. |

## 🚀 How to Use

### Example: Build a structured error

```csharp showLineNumbers
using RA.Utilities.Core.Constants;
using RA.Utilities.Core.Exceptions;

var error = new ValidationError(BaseErrorMessage.EmailRequired)
{
    PropertyName = "Email",
    AttemptedValue = request.Email,
    ExpectedValue = "a valid email address",
    ErrorCode = BaseErrorCode.EmailRequired
};

return new BadRequestException(error);
```

### Example: Map FluentValidation failures

```csharp showLineNumbers
ValidationError[] errors = [.. ex.Errors.Select(e => new ValidationError(e.ErrorMessage)
{
    PropertyName = e.PropertyName,
    AttemptedValue = e.AttemptedValue,
    ErrorCode = e.ErrorCode
})];

return new BadRequestException(errors);
```

## 📦 JSON shape

When mapped by `RA.Utilities.Api`, each item is serialized with `camelCase` names; `null` properties are omitted:

```json showLineNumbers
{
  "propertyName": "EmailAddress",
  "errorMessage": "'Email Address' is not a valid email address.",
  "attemptedValue": "not-an-email",
  "errorCode": "EmailValidator"
}
```

See [`BadRequestResponse`](../../api/RA.Utilities.Api.Response/BadRequestResponse.md) for the full response envelope and [`BaseErrorCode`](../RA.Utilities.Core.Constants/BaseErrorCode) for the standard codes.
