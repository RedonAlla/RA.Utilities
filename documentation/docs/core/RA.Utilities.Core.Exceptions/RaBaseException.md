---
title: RaBaseException
sidebar_position: 1
---

```bash
Namespace: RA.Utilities.Core.Exceptions
```

`RaBaseException` is the base class for all custom, semantic exceptions in the `RA.Utilities` ecosystem. It is a concrete class — you can throw it directly — and derives from `System.Exception`.

## 🎯 Purpose

`RaBaseException` establishes the contract every semantic exception shares: a human-readable `Message`, a machine-readable `ErrorCode`, and a semantic `ResponseType`.

1. **`Message`** — describes what went wrong, for logs and clients.
2. **`ErrorCode`** — an `int` the API layer echoes into the JSON `responseCode` field.
3. **`ResponseType`** — an enum value that categorizes the outcome and tells the API layer which response shape to produce.

Because every exception carries the same three pieces of information, higher layers can translate exceptions into HTTP responses without parsing message text.

The parameterless constructor produces a generic server error (`500`, `ResponseType.Error`). All other exceptions in this package derive from `RaBaseException` and supply their own code and type.

## Properties

| Property | Type | Description |
|---|---|---|
| **`ErrorCode`** | `int` | Machine-readable code. `500` for the parameterless constructor; otherwise supplied by the derived type or caller. |
| **`ResponseType`** | [`ResponseType`](../RA.Utilities.Core.Constants/ResponseType) | Semantic outcome. `ResponseType.Error` for the parameterless constructor. |
| **`Message`** | `string` | Inherited from `Exception`. Defaults to `BaseResponseMessages.Error` (`"Something happened on our end."`). |

`ErrorCode` and `ResponseType` use `{ get; init; }` and are immutable after construction.

## Constructors

| Constructor | `ErrorCode` | `ResponseType` | `Message` |
|---|---|---|---|
| `RaBaseException()` | `500` | `Error` | `BaseResponseMessages.Error` |
| `RaBaseException(int errorCode, ResponseType errorType, string? message = null)` | supplied | supplied | supplied, or `BaseResponseMessages.Error` when `null`/omitted |

## 🚀 How to Use

### Example 1: Throw a general error

Use `RaBaseException` when no more specific type fits. The constructor has no `innerException` parameter, so log the original exception before throwing:

```csharp showLineNumbers
try
{
    await _externalService.DoSomethingCritical();
}
catch (HttpRequestException ex)
{
    _logger.LogError(ex, "The external service call failed.");

    throw new RaBaseException(
        BaseResponseCode.InternalServerError,
        ResponseType.Error,
        "A critical operation failed while communicating with an external service."
    );
}
```

### Example 2: Rely on the defaults

Omitting the message resolves to `BaseResponseMessages.Error` at runtime:

```csharp showLineNumbers
// 500 / ResponseType.Error / "Something happened on our end."
var error = new RaBaseException();

// 500 / ResponseType.Error / "Something happened on our end."
var same = new RaBaseException(500, ResponseType.Error);
```

### Example 3: Create a derived exception

Deriving is the idiomatic way to add a new outcome type. Use `BaseResponseCode` values so the exception matches the rest of the ecosystem:

```csharp showLineNumbers
public sealed class PaymentFailedException : RaBaseException
{
    public PaymentFailedException(string reason)
        : base(BaseResponseCode.BadRequest, ResponseType.BadRequest, $"Payment failed: {reason}")
    {
    }
}
```

## 🔗 HTTP mapping

`ErrorResultResponse` maps an `RaBaseException` that does not match a more specific type to **HTTP 500** with an `ErrorResponse` body. The exception's `ErrorCode` is echoed into `responseCode`; it does not change the HTTP status.

See [`ErrorResultResponse`](../../api/RA.Utilities.Api/mapper/ErrorResultResponse.md) for the response shapes and [`Troubleshooting`](./troubleshooting.mdx) for common questions.
