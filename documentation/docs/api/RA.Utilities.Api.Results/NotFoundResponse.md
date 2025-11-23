---
title: NotFoundResponse
sidebar_position: 4
---

```powershell
Namespace: RA.Utilities.Api.Results
```

The `NotFoundResponse` class is a specialized model for creating standardized `404 Not Found` responses.
It is used when a requested resource cannot be found at the specified URI. It inherits from [`Response<T>`](./Response), with the `Result` property typed as a `NotFoundResult` object.

### 🎯 Purpose

The `NotFoundResponse` class is a specialized model used to create standardized `404 Not Found` API responses.
It inherits from the base [`Response<T>`](./Response) class and is designed to clearly communicate when a requested resource does not exist.

Here's a breakdown of its key functions:

1. **Simplifies 404 Responses**: It reduces boilerplate code by automatically setting the response properties for a "not found" scenario:

  * **ResponseCode**: Set to `404` (from `BaseResponseCode.NotFound`).
  * **ResponseType**: Set to   * **ResponseType.NotFound  * **.
  * **ResponseMessage**: Generates a clear, dynamic message like `"Product with value '123' not found."`.

2. **Provides Structured Context**: Unlike a simple text response, `NotFoundResponse` uses a [`NotFoundResult`](#notfoundresult) object as its payload.
This object contains two key pieces of information:

  * **EntityName**: The type of resource that was being looked for (e.g., "Product", "User").
  * **EntityValue**: The identifier that was used in the search (e.g., `123`, `"john.doe@example.com"`).

3. **Improves Client-Side Handling**: This structured response allows clients to provide more intelligent feedback to the user. For example, a frontend application could parse this response and display a message like, "We couldn't find a product with the ID '123'."

Using `NotFoundResponse` ensures that all "not found" errors across your API are consistent, structured, and informative.

### NotFoundResult
Represent an entity not found searched by given value.

| Property        | Type     | Description     |
| --------------- | -------- | --------------- |
| **EntityName**  | `string` | The type of resource that was being looked for (e.g., "Product", "User"). |
| **EntityValue** | `object` | The identifier that was used in the search (e.g., `123`, `"john.doe@example.com"`). |

### ⚙️ How It Works

When you create an instance of `NotFoundResponse`, it pre-configures the following properties:

- **`ResponseCode`**: Set to `404` (from `BaseResponseCode.NotFound`).
- **`ResponseType`**: Set to `ResponseType.NotFound`.
- **`ResponseMessage`**: A dynamically generated message, such as `"Product with value '123' not found."`.
- **`Result`**: A `NotFoundResult` object containing the `EntityName` and `EntityValue` that were searched for.

### 🚀 Usage in a Controller

You can use this class in your controller actions when a database query or service call returns no result for a given identifier.

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
        // Assume product is fetched from a service
        var product = _productService.GetById(id);

        if (product == null)
        {
            // Return a 404 Not Found with structured details
            // highlight-next-line
            return NotFound(new NotFoundResponse("Product", id));
        }

        return Ok(new SuccessResponse<Product>(product));
    }
}
```

### Example JSON Output

A request to `/api/products/999` (where product 999 does not exist) would produce the following JSON response body:

```json showLineNumbers
{
  "responseCode": 404,
  "responseType": "NotFound",
  "responseMessage": "Product with value '999' not found.",
  "result": {
    "entityName": "Product",
    "entityValue": 999
  }
}
```
