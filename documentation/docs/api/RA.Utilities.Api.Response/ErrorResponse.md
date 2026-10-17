---
title: ErrorResponse
sidebar_position: 6
---

```bash
Namespace: RA.Utilities.Api.Response
```

The `ErrorResponse` class creates a standardized `500 Internal Server Error` response. It derives from [`Response<T>`](./Response) with `T = ErrorResult` and is the catch-all model for unexpected failures.

### 🎯 Purpose

Use `ErrorResponse` when an unhandled exception or an unexpected state occurs. The parameterless constructor returns a safe generic message so internal details do not leak to clients.

- **Standard envelope**: `ResponseCode = 500`, `ResponseType = ResponseType.Error`, `ResponseMessage = "Something happened on our end."`
- **Optional payload**: `Result` can be `null` or an [`ErrorResult`](./ErrorResult) with a machine-readable code and message.
- **Safe default**: `new ErrorResponse()` carries no internal exception details.

### Constructors

| Constructor | `Result` | `ResponseCode` | `ResponseMessage` |
|---|---|---|---|
| `ErrorResponse()` | `null` | `500` | `"Something happened on our end."` |
| `ErrorResponse(int responseCode, string responseMessage)` | `null` | `responseCode` | `responseMessage` |
| `ErrorResponse(ErrorResult? result)` | `result` | `500` | `"Something happened on our end."` |
| `ErrorResponse(ErrorResult? result, int? responseCode, string responseMessage = "")` | `result` | `responseCode ?? 500` | `responseMessage ?? standard` (the `""` default is used as-is) |

`ResponseType` is always `ResponseType.Error` (JSON `11`).

:::warning[Empty message on the all-argument overload]

`new ErrorResponse(result, null)` compiles, but it produces an **empty** `ResponseMessage` because the optional message defaults to `""`. Pass `BaseResponseMessages.Error` explicitly, or use `new ErrorResponse(result)` when the standard message is fine.

:::

### Usage

Return a generic server error:

```csharp showLineNumbers
using Microsoft.AspNetCore.Mvc;
// highlight-next-line
using RA.Utilities.Api.Response;

[HttpPost]
public IActionResult ProcessProduct(Product product)
{
    try
    {
        _service.Process(product);
        return Ok(new SuccessResponse<string>("Processed"));
    }
    catch (Exception)
    {
        // Safe, generic 500 — no exception details in the response
        // highlight-next-line
        return StatusCode(500, new ErrorResponse());
    }
}
```

Include a structured error result:

```csharp showLineNumbers
return StatusCode(500, new ErrorResponse(new ErrorResult
{
    ErrorCode = "PROCESSING_ERROR",
    ErrorMessage = "An unexpected error occurred while processing the product."
}));
```

### Example JSON Output

```json showLineNumbers
{
  "responseCode": 500,
  // highlight-next-line
  "responseType": 11,
  "responseMessage": "Something happened on our end.",
  "result": {
    "errorCode": "PROCESSING_ERROR",
    "errorMessage": "An unexpected error occurred while processing the product."
  }
}
```

With `new ErrorResponse()` the `result` property is `null`. With `JsonStringEnumConverter`, `responseType` is `"Error"`.
