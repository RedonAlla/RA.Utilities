---
title: NotFoundResponse
sidebar_position: 4
---

```bash
Namespace: RA.Utilities.Api.Response
```

The `NotFoundResponse` class creates a standardized `404 Not Found` response. It derives from [`Response<T>`](./Response) with `T = NotFoundResult`, so `Result` identifies the entity that was searched for and the value that was used.

### 🎯 Purpose

Use `NotFoundResponse` when a lookup returns nothing. Instead of a bare "not found" string, the payload tells the client which resource type and identifier failed, and can carry a machine-readable code.

- **Standard envelope**: `ResponseCode = 404`, `ResponseType = ResponseType.NotFound`, `ResponseMessage = "The requested resource was not found."`
- **Typed payload**: `Result` is a [`NotFoundResult`](#notfoundresult) with `Entity` and `Value`.
- **Customizable**: Override the envelope code/message or the nested error code/message.

### Constructors

| Constructor | `Result` | `ResponseCode` | `ResponseMessage` |
|---|---|---|---|
| `NotFoundResponse(NotFoundResult model)` | `model` | `404` | `"The requested resource was not found."` |
| `NotFoundResponse(NotFoundResult model, int? responseCode, string responseMessage = "")` | `model` | `responseCode ?? 404` | `responseMessage ?? standard` (the `""` default is used as-is) |

`ResponseType` is always `ResponseType.NotFound` (JSON `8`).

### NotFoundResult

`NotFoundResult` inherits from [`ErrorResult`](./ErrorResult).

| Property | Type | Description |
|---|---|---|
| **Entity** | `string` | The type of resource that was searched for (e.g., `"Product"`, `"User"`). |
| **Value** | `object` | The identifier used in the search (e.g., `123`, `"john.doe@example.com"`). |
| **ErrorCode** | `string?` | Machine-readable code (inherited). Defaults to `"NotFound"`. Omitted from JSON when `null`. |
| **ErrorMessage** | `string` | Human-readable message (inherited). Defaults to an **empty string**, not the standard envelope message. Omitted when `null`. |

Constructor:

```csharp showLineNumbers
public NotFoundResult(
    string entity,
    object value,
    string errorCode = "NotFound",
    string message = "")
```

:::warning[Nested message defaults to empty]

The `NotFoundResponse(NotFoundResult)` convenience constructor sets the **envelope** message to `"The requested resource was not found."`, but it does not change the nested `NotFoundResult.ErrorMessage`. `new NotFoundResult("Product", 99)` therefore produces `"errorMessage": ""`.

Pass the `message` argument or set the property when you want text in the nested payload:

```csharp showLineNumbers
var result = new NotFoundResult("Product", 99)
{
    ErrorMessage = "Product with id 99 was not found."
};
```

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
            // highlight-next-line
            return NotFound(new NotFoundResponse(new NotFoundResult("Product", id)));
        }

        return Ok(new SuccessResponse<Product>(product));
    }
}
```

Override the nested message and code:

```csharp showLineNumbers
var result = new NotFoundResult(
    "Product",
    id,
    errorCode: "PRODUCT_NOT_FOUND",
    // highlight-next-line
    message: $"Product with id {id} was not found.");

return NotFound(new NotFoundResponse(result));
```

### Example JSON Output

Default constructor:

```json showLineNumbers
{
  "responseCode": 404,
  // highlight-next-line
  "responseType": 8,
  "responseMessage": "The requested resource was not found.",
  "result": {
    "entity": "Product",
    "value": 999,
    "errorCode": "NotFound",
    "errorMessage": ""
  }
}
```

With an explicit nested message, `errorMessage` carries the text. With `JsonStringEnumConverter`, `responseType` is `"NotFound"`.
