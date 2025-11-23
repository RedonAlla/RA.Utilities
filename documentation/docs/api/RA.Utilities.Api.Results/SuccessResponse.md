---
title: SuccessResponse<T>
sidebar_position: 2
---

```powershell
Namespace: RA.Utilities.Api.Results
```

The `SuccessResponse<T>` class is a specialized model for creating standardized, successful API responses.
It inherits from the base [`Response<T>`](./Response) and simplifies the process of returning a `200 OK` result with a data payload.

### 🎯 Purpose

The `SuccessResponse<T>` class is a specialized, convenient wrapper for creating standardized successful API responses.
It inherits from the base [`Response<T>`](./Response) class and is designed to reduce boilerplate code in your controller actions.

Its primary purpose is to simplify the creation of a `200 OK` response by pre-configuring the standard response properties for you:

1. **Sets `ResponseCode` to 200**: It automatically sets the `ResponseCode` to `200` (using `BaseResponseCode.Success`).
2. **Sets [`ResponseType`](./Response) to `Success`**: It sets the [`ResponseType`](./Response) enum to `ResponseType.Success`.
3. **Provides a Default Message**: It assigns a default `ResponseMessage` of `"Operation completed successfully."` (from `BaseResponseMessages.Success`), which you can override if needed.
4. **Accepts the Payload**: Its constructor takes the data payload (`T`) that you want to send back to the client.

### ⚙️ How It Works

When you create an instance of `SuccessResponse<T>`, it pre-configures the following properties:

- **`ResponseCode`**: Set to `200` (from `BaseResponseCode.Success`).
- **`ResponseType`**: Set to `ResponseType.Success`.
- **`ResponseMessage`**: Defaults to `"Operation completed successfully."` (from `BaseResponseMessages.Success`), but can be overridden via the constructor.
- **`Result`**: Set to the data payload you provide.

### 🚀 Usage in a Controller

You can use this class directly in your controller actions to wrap your data and return a standardized `OkObjectResult` (HTTP 200).

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
        var product = new Product { Id = id, Name = "Sample Product" };

        // highlight-next-line
        return Ok(new SuccessResponse<Product>(product));
    }
}
```

### Example JSON Output

The code above would produce the following JSON response body:

```json showLineNumbers
{
  "responseCode": 200,
  "responseType": "Success",
  "responseMessage": "Operation completed successfully.",
  "result": {
    "id": 1,
    "name": "Sample Product"
  }
}
```
