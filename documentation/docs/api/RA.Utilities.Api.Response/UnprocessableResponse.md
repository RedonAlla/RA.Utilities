---
title: UnprocessableResponse
sidebar_position: 9
---

```bash
Namespace: RA.Utilities.Api.Response
```

The `UnprocessableResponse` class creates a standardized `422 Unprocessable Entity` response. It derives from [`Response<T>`](./Response) with `T = ErrorResult` and is used when the request is syntactically valid but semantically impossible.

### 🎯 Purpose

Use `UnprocessableResponse` for business-rule violations that are not field validation errors — for example, cancelling an order that has already shipped.

- **Standard envelope**: `ResponseCode = 422`, `ResponseType = ResponseType.Unprocessable`, `ResponseMessage = "Unprocessable entity."`
- **Optional payload**: `Result` can be `null` or an [`ErrorResult`](./ErrorResult) describing the semantic failure.

### Constructors

| Constructor | `Result` | `ResponseCode` | `ResponseMessage` |
|---|---|---|---|
| `UnprocessableResponse()` | `null` | `422` | `"Unprocessable entity."` |
| `UnprocessableResponse(int responseCode, string responseMessage = "")` | `null` | `responseCode` | `responseMessage ?? standard` (the `""` default is used as-is) |
| `UnprocessableResponse(ErrorResult result)` | `result`, or a default `ErrorResult` when `null` | `422` | `"Unprocessable entity."` |
| `UnprocessableResponse(ErrorResult result, int responseCode, string responseMessage = "")` | `result`, or a default `ErrorResult` when `null` | `responseCode` | `responseMessage ?? standard` (the `""` default is used as-is) |

`ResponseType` is always `ResponseType.Unprocessable` (JSON `13`). When `result` is `null`, the constructor substitutes an `ErrorResult` with `ErrorCode = "Unprocessable"` and `ErrorMessage = "Unprocessable entity."`.

### Usage

```csharp showLineNumbers
using Microsoft.AspNetCore.Mvc;
// highlight-next-line
using RA.Utilities.Api.Response;

[HttpPost("{id}/cancel")]
public IActionResult CancelOrder(int id)
{
    var order = _service.GetById(id);
    if (order.Status == OrderStatus.Shipped)
    {
        // highlight-next-line
        return UnprocessableEntity(new UnprocessableResponse(new ErrorResult
        {
            ErrorCode = "ORDER_ALREADY_SHIPPED",
            ErrorMessage = "Cannot cancel an order that has already been shipped."
        }));
    }

    _service.Cancel(id);
    return Ok(new SuccessResponse<string>("Order cancelled."));
}
```

### Example JSON Output

```json showLineNumbers
{
  "responseCode": 422,
  // highlight-next-line
  "responseType": 13,
  "responseMessage": "Unprocessable entity.",
  "result": {
    "errorCode": "ORDER_ALREADY_SHIPPED",
    "errorMessage": "Cannot cancel an order that has already been shipped."
  }
}
```

With `JsonStringEnumConverter`, `responseType` is `"Unprocessable"`.
