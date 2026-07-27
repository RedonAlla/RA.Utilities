---
title: ServiceUnavailableException
---

```bash
Namespace: RA.Utilities.Core.Exceptions
```

The `ServiceUnavailableException` is a semantic exception used to signal that the service is temporarily unable to handle requests.
It is designed to be translated into a standardized **HTTP 503 Service Unavailable** response.

## 🎯 Purpose

This exception represents a temporary outage or maintenance scenario where the server is currently unable to process requests but may recover. Common scenarios include:

- Scheduled maintenance mode
- A critical downstream dependency is unhealthy
- The database is undergoing a failover or restart
- Circuit breaker has tripped for an external service
- The application is starting up and not yet ready

By throwing a `ServiceUnavailableException`, your business logic clearly communicates the temporary nature of the failure. Clients can use the `Retry-After` header to know when to retry.

## Constructors

| Constructor | Description |
|---|---|
| `ServiceUnavailableException()` | Uses default error code (`503`) and default message. |
| `ServiceUnavailableException(string message)` | Uses default error code (`503`) with a custom message. |
| `ServiceUnavailableException(int errorCode, string message = ...)` | Custom error code with an optional custom message. |

## 🚀 How to Use

### Example: Maintenance Mode Check

```csharp showLineNumbers
using RA.Utilities.Core;
// highlight-next-line
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

### Example: Circuit Breaker

```csharp showLineNumbers
public async Task<Result<Data>> FetchFromDownstreamAsync()
{
    if (_circuitBreaker.IsOpen)
    {
        // highlight-next-line
        return new ServiceUnavailableException(
            errorCode: BaseResponseCode.ServiceUnavailable,
            message: "The downstream service is currently unavailable. The circuit breaker will retry shortly."
        );
    }

    try
    {
        return await _downstreamService.FetchDataAsync();
    }
    catch (HttpRequestException)
    {
        _circuitBreaker.RecordFailure();
        return new ServiceUnavailableException("Unable to reach the downstream service.");
    }
}
```

### Example JSON Output

When the API layer handles a `Failure` `Result` containing a `ServiceUnavailableException`, it will generate a `503 Service Unavailable` response:

```json showLineNumbers
{
  "responseCode": 503,
  "responseType": "ServiceUnavailable",
  "responseMessage": "The service is currently undergoing scheduled maintenance. Please try again in a few minutes.",
  "result": {
    "errorCode": 503,
    "errorMessage": "The service is currently undergoing scheduled maintenance. Please try again in a few minutes."
  }
}
```
