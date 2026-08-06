---
title: TooManyRequestsResponse
sidebar_position: 9
---

```powershell
Namespace: RA.Utilities.Api.Results
```

The `TooManyRequestsResponse` class is a specialized model for creating standardized `429 Too Many Requests` responses.
It is used when the user has sent too many requests in a given amount of time (rate limiting).
It inherits from [`Response<T>`](./Response), with the `Result` property typed as an [`ErrorResult`](./ErrorResult) object.

### 🎯 Purpose

The `TooManyRequestsResponse` class is a specialized model for creating standardized `429 Too Many Requests` API responses.
It is used to signal that the user has exceeded the allowed number of requests within a given time window.

Its primary functions are:

1. **Standardizes Rate Limit Errors**: It provides a consistent structure for all `429 Too Many Requests` errors.

2. **Reduces Boilerplate**: It automatically sets the response properties for a rate-limited request:

  * **ResponseCode**: Set to `429` (from `BaseResponseCode.TooManyRequests`).
  * **ResponseType**: Set to `ResponseType.TooManyRequests`.
  * **ResponseMessage**: Defaults to `"Too many requests. Please try again later."` (from `BaseResponseMessages.TooManyRequests`).

3. **Provides Structured Context**: It uses an [`ErrorResult`](./ErrorResult) payload to provide specific details about why the request was rate limited.

### ⚙️ How It Works

When you create an instance of `TooManyRequestsResponse`, it pre-configures the following properties:

- **`ResponseCode`**: Set to `429` (from `BaseResponseCode.TooManyRequests`).
- **`ResponseType`**: Set to `ResponseType.TooManyRequests`.
- **`ResponseMessage`**: Defaults to `"Too many requests. Please try again later."`.
- **`Result`**: An [`ErrorResult`](./ErrorResult) object containing the error code and message.

### 🚀 Usage in a Controller

You can use this class in your controller actions or middleware when a rate limit check fails.

```csharp showLineNumbers
using Microsoft.AspNetCore.Mvc;
// highlight-next-line
using RA.Utilities.Api.Results;

[ApiController]
[Route("api/[controller]")]
public sealed class DataController : ControllerBase
{
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
}
```

### Example JSON Output

```json showLineNumbers
{
  "responseCode": 429,
  "responseType": "TooManyRequests",
  "responseMessage": "Too many requests. Please try again later.",
  "result": {
    "errorCode": "RateLimitExceeded",
    "errorMessage": "You have exceeded the maximum number of requests. Please wait and try again."
  }
}
```
