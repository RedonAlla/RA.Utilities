---
title: GatewayTimeoutResponse
sidebar_position: 12
---

```bash
Namespace: RA.Utilities.Api.Response
```

The `GatewayTimeoutResponse` class creates a standardized `504 Gateway Timeout` response. It derives from [`Response<T>`](./Response) with `T = ErrorResult` and is used when an upstream service does not respond in time.

### 🎯 Purpose

Use `GatewayTimeoutResponse` when your API acts as a gateway or proxy and the upstream dependency times out.

- **Standard envelope**: `ResponseCode = 504`, `ResponseType = ResponseType.GatewayTimeout`, `ResponseMessage = "The server, while acting as a gateway or proxy, did not receive a timely response from the upstream server."`
- **Optional payload**: `Result` can be `null` or an [`ErrorResult`](./ErrorResult) naming the upstream dependency.

### Constructors

| Constructor | `Result` | `ResponseCode` | `ResponseMessage` |
|---|---|---|---|
| `GatewayTimeoutResponse(int responseCode, string responseMessage)` | `null` | `responseCode` | `responseMessage` |
| `GatewayTimeoutResponse(ErrorResult? result = null)` | `result` | `504` | standard gateway-timeout message |
| `GatewayTimeoutResponse(ErrorResult result, int? responseCode, string responseMessage = "")` | `result` | `responseCode ?? 504` | `responseMessage ?? standard` (the `""` default is used as-is) |

`ResponseType` is always `ResponseType.GatewayTimeout` (JSON `17`). The optional `result` parameter means `new GatewayTimeoutResponse()` is valid.

### Usage

```csharp showLineNumbers
using Microsoft.AspNetCore.Mvc;
// highlight-next-line
using RA.Utilities.Api.Response;

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
```

### Example JSON Output

```json showLineNumbers
{
  "responseCode": 504,
  // highlight-next-line
  "responseType": 17,
  "responseMessage": "The server, while acting as a gateway or proxy, did not receive a timely response from the upstream server.",
  "result": {
    "errorCode": "UpstreamTimeout",
    "errorMessage": "The upstream server did not respond in time. Please try again."
  }
}
```

With `JsonStringEnumConverter`, `responseType` is `"GatewayTimeout"`.
