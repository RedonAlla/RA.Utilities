---
title: ServiceUnavailableResponse
sidebar_position: 11
---

```bash
Namespace: RA.Utilities.Api.Response
```

The `ServiceUnavailableResponse` class creates a standardized `503 Service Unavailable` response. It derives from [`Response<T>`](./Response) with `T = ErrorResult` and is used when the service is temporarily unable to handle requests.

### 🎯 Purpose

Use `ServiceUnavailableResponse` during maintenance or overload. It tells clients the failure is temporary rather than a permanent server error.

- **Standard envelope**: `ResponseCode = 503`, `ResponseType = ResponseType.ServiceUnavailable`, `ResponseMessage = "The service is temporarily unavailable. Please try again later."`
- **Optional payload**: `Result` can be `null` or an [`ErrorResult`](./ErrorResult) describing the outage.

### Constructors

| Constructor | `Result` | `ResponseCode` | `ResponseMessage` |
|---|---|---|---|
| `ServiceUnavailableResponse(int responseCode, string responseMessage)` | `null` | `responseCode` | `responseMessage` |
| `ServiceUnavailableResponse(ErrorResult? result = null)` | `result` | `503` | `"The service is temporarily unavailable. Please try again later."` |
| `ServiceUnavailableResponse(ErrorResult result, int? responseCode, string responseMessage = "")` | `result` | `responseCode ?? 503` | `responseMessage ?? standard` (the `""` default is used as-is) |

`ResponseType` is always `ResponseType.ServiceUnavailable` (JSON `16`). The optional `result` parameter means `new ServiceUnavailableResponse()` is valid.

### Usage

```csharp showLineNumbers
using Microsoft.AspNetCore.Mvc;
// highlight-next-line
using RA.Utilities.Api.Response;

[HttpGet("forecast")]
public IActionResult GetForecast()
{
    if (IsUnderMaintenance())
    {
        // highlight-next-line
        return StatusCode(503, new ServiceUnavailableResponse(new ErrorResult
        {
            ErrorCode = "MaintenanceMode",
            ErrorMessage = "The service is currently under maintenance. Please try again later."
        }));
    }

    return Ok(new SuccessResponse<string>("Forecast Data"));
}
```

### Example JSON Output

```json showLineNumbers
{
  "responseCode": 503,
  // highlight-next-line
  "responseType": 16,
  "responseMessage": "The service is temporarily unavailable. Please try again later.",
  "result": {
    "errorCode": "MaintenanceMode",
    "errorMessage": "The service is currently under maintenance. Please try again later."
  }
}
```

With `JsonStringEnumConverter`, `responseType` is `"ServiceUnavailable"`.
