---
title: GatewayTimeoutResponse
sidebar_position: 11
---

```powershell
Namespace: RA.Utilities.Api.Results
```

The `GatewayTimeoutResponse` class is a specialized model for creating standardized `504 Gateway Timeout` responses.
It is used when the server, while acting as a gateway or proxy, did not receive a timely response from the upstream server.
It inherits from [`Response<T>`](./Response), with the `Result` property typed as an [`ErrorResult`](./ErrorResult) object.

### 🎯 Purpose

The `GatewayTimeoutResponse` class is a specialized model for creating standardized `504 Gateway Timeout` API responses.
It is used to signal that the upstream server did not respond in time.

Its primary functions are:

1. **Standardizes Timeout Errors**: It provides a consistent structure for all `504 Gateway Timeout` errors.

2. **Reduces Boilerplate**: It automatically sets the response properties for a gateway timeout:

  * **ResponseCode**: Set to `504` (from `BaseResponseCode.GatewayTimeout`).
  * **ResponseType**: Set to `ResponseType.GatewayTimeout`.
  * **ResponseMessage**: Defaults to `"The server, while acting as a gateway or proxy, did not receive a timely response from the upstream server."` (from `BaseResponseMessages.GatewayTimeout`).

3. **Provides Structured Context**: It uses an [`ErrorResult`](./ErrorResult) payload to provide specific details about the upstream timeout.

### ⚙️ How It Works

When you create an instance of `GatewayTimeoutResponse`, it pre-configures the following properties:

- **`ResponseCode`**: Set to `504` (from `BaseResponseCode.GatewayTimeout`).
- **`ResponseType`**: Set to `ResponseType.GatewayTimeout`.
- **`ResponseMessage`**: Defaults to `"The server, while acting as a gateway or proxy, did not receive a timely response from the upstream server."`.
- **`Result`**: An [`ErrorResult`](./ErrorResult) object containing the error code and message.

### 🚀 Usage in a Controller

You can use this class in your controller actions or middleware when an upstream service times out.

```csharp showLineNumbers
using Microsoft.AspNetCore.Mvc;
// highlight-next-line
using RA.Utilities.Api.Results;

[ApiController]
[Route("api/[controller]")]
public sealed class ProxyController : ControllerBase
{
    [HttpGet("upstream-data")]
    public IActionResult GetUpstreamData()
    {
        var response = _upstreamService.Fetch();
        if (response.TimedOut)
        {
            // highlight-next-line
            return StatusCode(504, new GatewayTimeoutResponse(new ErrorResult 
            { 
                ErrorCode = "UpstreamTimeout", 
                ErrorMessage = "The upstream server did not respond in time. Please try again." 
            }));
        }

        return Ok(new SuccessResponse<string>("Upstream Data"));
    }
}
```

### Example JSON Output

```json showLineNumbers
{
  "responseCode": 504,
  "responseType": "GatewayTimeout",
  "responseMessage": "The server, while acting as a gateway or proxy, did not receive a timely response from the upstream server.",
  "result": {
    "errorCode": "UpstreamTimeout",
    "errorMessage": "The upstream server did not respond in time. Please try again."
  }
}
```
