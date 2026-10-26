---
title: ServiceUnavailableException
sidebar_position: 11
---

```bash
Namespace: RA.Utilities.Core.Exceptions
```

`ServiceUnavailableException` signals that the service is temporarily unable to handle requests. The API layer maps it to **HTTP 503 Service Unavailable**.

## 🎯 Purpose

Use this exception for temporary conditions that may resolve without a code change:

- Scheduled maintenance mode.
- A critical downstream dependency is unhealthy.
- The database is failing over or restarting.
- A circuit breaker is open.
- The application is starting and not yet ready.

Tell the client when to retry with a `Retry-After` header where possible — the condition is expected to be temporary.

## Constructors

| Constructor | `ErrorCode` | `Message` |
|---|---|---|
| `ServiceUnavailableException()` | `503` | `BaseResponseMessages.ServiceUnavailable` (`"The service is temporarily unavailable. Please try again later."`) |
| `ServiceUnavailableException(string message)` | `503` | supplied |
| `ServiceUnavailableException(int errorCode, string? message = null)` | supplied | supplied, or `BaseResponseMessages.ServiceUnavailable` when `null`/omitted |

## Properties

| Property | Type | Description |
|---|---|---|
| `ErrorCode` | `int` | Inherited. `503` unless supplied explicitly. |
| `ResponseType` | `ResponseType` | Inherited. `ResponseType.ServiceUnavailable`. |
| `Message` | `string` | Inherited. See the constructor table. |

## 🚀 How to Use

### Example: Maintenance mode

```csharp showLineNumbers
using RA.Utilities.Core.Results;
using RA.Utilities.Core.Exceptions;

public async Task<Result> ProcessRequestAsync()
{
    if (_maintenanceModeService.IsActive())
    {
        // highlight-next-line
        return new ServiceUnavailableException(
            "The service is currently undergoing scheduled maintenance. Please try again in a few minutes."
        );
    }

    // ... process the request
    return Result.Success();
}
```

### Example: Open circuit breaker

```csharp showLineNumbers
if (_circuitBreaker.IsOpen)
{
    // highlight-next-line
    return new ServiceUnavailableException(
        "The downstream service is currently unavailable. The circuit breaker will retry shortly."
    );
}
```

## 🔗 HTTP mapping

`ErrorResultResponse` maps `ServiceUnavailableException` to **HTTP 503** with a `ServiceUnavailableResponse`. The envelope carries `responseCode` from `ErrorCode` and `responseMessage` from `Message`; there is no nested `result` payload.

See [`ServiceUnavailableResponse`](../../api/RA.Utilities.Api.Response/ServiceUnavailableResponse.md) for the JSON shape.
