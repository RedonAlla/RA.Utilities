---
title: RaBaseException
sidebar_position: 1
---

```bash
Namespace: `RA.Utilities.Core.Exceptions`
```

`RaBaseException` is the abstract base class for all custom, semantic exceptions within the `RA.Utilities` ecosystem.
It provides a standardized structure for exceptions that represent predictable business-level failures.

## 🎯 Purpose

The primary purpose of `RaBaseException` is to establish a consistent contract for all application-specific exceptions.
It ensures that every exception carries not only a descriptive message but also a machine-readable error code, which typically corresponds to an HTTP status code.

By inheriting from `System.Exception` and adding both an `ErrorCode` property (an integer, typically an HTTP status code) and a `ResponseType` property (a type-safe record indicating the semantic outcome), it ensures that every custom exception in your application carries two key pieces of information:

1. A human-readable `Message` describing what went wrong.
2. A machine-readable `ErrorCode` (int) that can be used by higher-level handlers to translate the exception into a specific HTTP status code.
3. A semantic `ResponseType` (record) that categorizes the outcome (e.g., "NotFound", "Conflict") for structured error responses.


This allows higher-level layers, such as API middleware, to catch exceptions and automatically translate them into standardized HTTP error responses without needing to inspect the exception message text.

All other custom exceptions in this library, such as `NotFoundException` and `ConflictException`, inherit from `RaBaseException`.

## Properties

| Property       | Type                                       | Description                                                                                             |
|----------------|--------------------------------------------|---------------------------------------------------------------------------------------------------------|
| **ErrorCode**    | `int`                                      | The error code associated with the exception. By default, this is `500` (Internal Server Error).          |
| **ResponseType** | [`ResponseType`](../../RA.Utilities.Core.Constants/ResponseType.md) | A record categorizing the semantic outcome of the operation (e.g., "NotFound", "Conflict"). |
| **Message**      | `string`                                   | (Inherited from `Exception`) A human-readable message describing the error.                             |

## 🚀 How to Use

While you can throw `RaBaseException` directly for generic errors, its main function is to be the parent for more specific exceptions.

### Example 1: Throwing a General Error

If an error occurs that doesn't fit a more specific exception type, you can use `RaBaseException`. It's good practice to wrap the original exception as an `innerException` to preserve the stack trace.

```csharp
try
{
    // Some operation that might fail
    await _externalService.DoSomethingCritical();
}
catch (HttpRequestException ex)
{
    // Wrap the original exception in a RaBaseException
    throw new RaBaseException(
        BaseResponseCode.InternalServerError,
        ResponseType.Error,
        "A critical operation failed while communicating with an external service."
    );
}
```

### Example 2: Creating a Custom Derived Exception

The most common use case is to create your own semantic exceptions that inherit from `RaBaseException`.

```csharp showLineNumbers
public class PaymentFailedException : RaBaseException
{
    // Set a specific, non-default error code and response type for this exception type.
    public PaymentFailedException(string reason) 
        : base(BaseResponseCode.BadRequest, ResponseType.BadRequest, $"Payment failed: {reason}")
    {
    }
}
```
