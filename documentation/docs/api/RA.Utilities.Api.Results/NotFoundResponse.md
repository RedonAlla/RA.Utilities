---
title: NotFoundResponse
sidebar_position: 4
---

```powershell
Namespace: RA.Utilities.Api.Results
```

The `NotFoundResponse` class is a specialized model for creating standardized `404 Not Found` responses.
It is used when a requested resource cannot be found. It inherits from [`Response<T>`](./Response), with the `Result` property typed as a [`NotFoundResult`](#notfoundresult) object.

### 🎯 Purpose

The `NotFoundResponse` class is a specialized model used to create standardized `404 Not Found` API responses.
It inherits from the base [`Response<T>`](./Response) class and is designed to clearly communicate when a requested resource does not exist.

Here's a breakdown of its key functions:

1. **Simplifies 404 Responses**: It reduces boilerplate code by automatically setting the response properties for a "not found" scenario:

  * **ResponseCode**: Set to `404` (from `BaseResponseCode.NotFound`).
  * **ResponseType**: Set to `ResponseType.NotFound`.
  * **ResponseMessage**: Defaults to `"The requested resource was not found."` (from `BaseResponseMessages.NotFound`).

2. **Provides Structured Context**: It uses a [`NotFoundResult`](#notfoundresult) object as its payload, containing the entity name and the identifier used in the search.

3. **Improves Client-Side Handling**: This structured response allows clients to provide more intelligent feedback to the user.

### ⚙️ How It Works

When you create an instance of `NotFoundResponse`, it pre-configures the following properties:

- **`ResponseCode`**: Set to `404` (from `BaseResponseCode.NotFound`).
- **`ResponseType`**: Set to `ResponseType.NotFound`.
- **`ResponseMessage`**: Defaults to `"The requested resource was not found."`.
- **`Result`**: A [`NotFoundResult`](#notfoundresult) object containing the `Entity` name and `Value` that were searched for.

### 🚀 Usage in a Controller

```csharp showLineNumbers
using Microsoft.AspNetCore.Mvc;
// highlight-next-line
using RA.Utilities.Api.Results;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    [HttpGet("{id}")]
    public IActionResult GetProduct(int id)
    {
        var product = _productService.GetById(id);

        if (product == null)
        {
            // Return a 404 Not Found with structured details
            // highlight-next-line
            return NotFound(new NotFoundResponse(new NotFoundResult("Product", id)));
        }

        return Ok(new SuccessResponse<Product>(product));
    }
}
```

### NotFoundResult
Inherits from [`ErrorResult`](./ErrorResult).

| Property | Type | Description |
| --------------- | -------- | --------------- |
| **Entity** | `string` | The type of resource that was being looked for (e.g., "Product", "User"). |
| **Value** | `object` | The identifier that was used in the search (e.g., `123`, `"john.doe@example.com"`). |
| **ErrorCode** | `string` | The specific error code (inherited from `ErrorResult`). |
| **ErrorMessage** | `string` | The error message (inherited from `ErrorResult`). |

### Example JSON Output

```json showLineNumbers
{
  "responseCode": 404,
  "responseType": "NotFound",
  "responseMessage": "The requested resource was not found.",
  "result": {
    "entity": "Product",
    "value": 999,
    "errorCode": "NotFound",
    "errorMessage": "The requested resource was not found."
  }
}
```
