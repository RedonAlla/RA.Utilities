---
title: SuccessResponse<T>
sidebar_position: 2
---

```bash
Namespace: RA.Utilities.Api.Response
```

The `SuccessResponse<T>` class wraps a successful payload in the standard [`Response<T>`](./Response) envelope. It sets `ResponseCode = 200` (`BaseResponseCode.Success`), `ResponseType = ResponseType.Success`, and the standard success message.

### 🎯 Purpose

Use `SuccessResponse<T>` when you want a consistent `200 OK` body. It removes the need to repeat the status code and message at every call site and keeps the success contract aligned with the error responses.

### Constructors

| Constructor | `Result` | `ResponseCode` | `ResponseMessage` |
|---|---|---|---|
| `SuccessResponse(T result)` | `result` | `200` | `"Operation completed successfully."` |
| `SuccessResponse(T result, int? responseCode, string responseMessage = "")` | `result` | `responseCode ?? 200` | `responseMessage ?? standard` (the `""` default is used as-is) |
| `SuccessResponse()` | `default` | `200` | standard |
| `SuccessResponse(int responseCode)` | `default` | `responseCode` | standard |
| `SuccessResponse(string responseMessage)` | `default` | `200` | `responseMessage ?? standard` |

The standard message is `BaseResponseMessages.Success`: `"Operation completed successfully."`

:::warning[Overload resolution for `T = string` and `T = int`]

Because the class adds `SuccessResponse(string responseMessage)` and `SuccessResponse(int responseCode)`, a single-argument call is ambiguous in intent when `T` is `string` or `int`. Overload resolution picks the non-generic constructor:

```csharp showLineNumbers
// ⚠️ Sets ResponseMessage = "hello"; Result stays null
var wrong = new SuccessResponse<string>("hello");

// ✅ Sets Result = "hello"
var right = new SuccessResponse<string>("hello", null);

// ⚠️ Sets ResponseCode = 42; Result stays 0
var wrongInt = new SuccessResponse<int>(42);

// ✅ Sets Result = 42
var rightInt = new SuccessResponse<int>(42, null);
```

For all other payload types, `new SuccessResponse<MyDto>(dto)` selects the `T` overload as expected.

:::

### Usage

```csharp showLineNumbers
using Microsoft.AspNetCore.Mvc;
// highlight-next-line
using RA.Utilities.Api.Response;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    [HttpGet("{id}")]
    public IActionResult GetProduct(int id)
    {
        var product = _productService.GetById(id);
        if (product is null)
        {
            return NotFound(new NotFoundResponse(new NotFoundResult("Product", id)));
        }

        // highlight-next-line
        return Ok(new SuccessResponse<Product>(product));
    }
}
```

### Example JSON Output

```json showLineNumbers
{
  "responseCode": 200,
  // highlight-next-line
  "responseType": 0,
  "responseMessage": "Operation completed successfully.",
  "result": {
    "id": 1,
    "name": "Sample Product"
  }
}
```

With `JsonStringEnumConverter` registered, `responseType` is `"Success"`. See [`ResponseType`](../../core/RA.Utilities.Core.Constants/ResponseType#json-serialization).
