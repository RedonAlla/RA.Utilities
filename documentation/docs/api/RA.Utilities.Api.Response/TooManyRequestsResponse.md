---
title: TooManyRequestsResponse
sidebar_position: 10
---

```bash
Namespace: RA.Utilities.Api.Response
```

The `TooManyRequestsResponse` class creates a standardized `429 Too Many Requests` response. It derives from [`Response<T>`](./Response) with `T = ErrorResult` and is used when a client exceeds a rate limit.

### 🎯 Purpose

Use `TooManyRequestsResponse` when rate limiting rejects a request. Pair it with a `Retry-After` header in your middleware or endpoint when you can estimate the wait.

- **Standard envelope**: `ResponseCode = 429`, `ResponseType = ResponseType.TooManyRequests`, `ResponseMessage = "Too many requests. Please try again later."`
- **Optional payload**: `Result` can be `null` or an [`ErrorResult`](./ErrorResult) describing the limit.

### Constructors

| Constructor | `Result` | `ResponseCode` | `ResponseMessage` |
|---|---|---|---|
| `TooManyRequestsResponse(int responseCode, string responseMessage)` | `null` | `responseCode` | `responseMessage` |
| `TooManyRequestsResponse(ErrorResult? result = null)` | `result` | `429` | `"Too many requests. Please try again later."` |
| `TooManyRequestsResponse(ErrorResult result, int? responseCode, string responseMessage = "")` | `result` | `responseCode ?? 429` | `responseMessage ?? standard` (the `""` default is used as-is) |

`ResponseType` is always `ResponseType.TooManyRequests` (JSON `15`). The optional `result` parameter means `new TooManyRequestsResponse()` is valid.

### Usage

```csharp showLineNumbers
using Microsoft.AspNetCore.Mvc;
// highlight-next-line
using RA.Utilities.Api.Response;

[HttpGet("data")]
public IActionResult GetData()
{
    if (RateLimitExceeded())
    {
        // highlight-next-line
        return StatusCode(429, new TooManyRequestsResponse(new ErrorResult
        {
            ErrorCode = "RateLimitExceeded",
            ErrorMessage = "You have exceeded the maximum number of requests. Please wait and try again."
        }));
    }

    return Ok(new SuccessResponse<string>("Sensitive Data"));
}
```

### Example JSON Output

```json showLineNumbers
{
  "responseCode": 429,
  // highlight-next-line
  "responseType": 15,
  "responseMessage": "Too many requests. Please try again later.",
  "result": {
    "errorCode": "RateLimitExceeded",
    "errorMessage": "You have exceeded the maximum number of requests. Please wait and try again."
  }
}
```

With `JsonStringEnumConverter`, `responseType` is `"TooManyRequests"`.
