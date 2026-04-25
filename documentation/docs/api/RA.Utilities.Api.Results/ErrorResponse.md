---
title: ErrorResponse
sidebar_position: 6
---

```powershell
Namespace: RA.Utilities.Api.Results
```

The `ErrorResponse` class is a specialized model for creating standardized `500 Internal Server Error` responses.
It is designed to be a "catch-all" for any unexpected or unhandled exceptions that occur within your application.
It inherits from [`Response<T>`](./Response), with the `Result` property typed as an [`ErrorResult`](./ErrorResult) object.

### 🎯 Purpose

The `ErrorResponse` class is a specialized model for creating standardized `500 Internal Server Error` API responses.
It serves as a "catch-all" for any unexpected or unhandled exceptions that occur during request processing.
It inherits from the base [`Response<T>`](./Response) class.

Its primary functions are:

1. **Handles Unexpected Failures**: It provides a consistent and safe way to respond when something goes wrong on the server that wasn't anticipated by a more specific exception (like `NotFoundException` or `ConflictException`).

2. **Prevents Information Leaks**: By default, it returns a generic error message. In a production environment, it avoids sending sensitive details to the client.

3. **Standardized Error Payload**: It uses [`ErrorResult`](./ErrorResult) to provide a machine-readable error code and a human-friendly error message.

4. **Reduces Boilerplate**: It automatically sets the response properties for a server error:

  * **ResponseCode**: Set to `500` (from `BaseResponseCode.InternalServerError`).
  * **ResponseType**: Set to `ResponseType.Error`.
  * **ResponseMessage**: Defaults to `"A general error occurred during the operation."` (from `BaseResponseMessages.Error`).

### ⚙️ How It Works

When you create an instance of `ErrorResponse`, it pre-configures the following properties:

- **`ResponseCode`**: Set to `500` (from `BaseResponseCode.InternalServerError`).
- **`ResponseType`**: Set to `ResponseType.Error`.
- **`ResponseMessage`**: Defaults to `"A general error occurred during the operation."`.
- **`Result`**: An [`ErrorResult`](./ErrorResult) object containing the error code and message.

### 🚀 Usage in a Controller

You can use this class in your controller actions or within a global exception handler to return a standardized error response.

```csharp showLineNumbers
using Microsoft.AspNetCore.Mvc;
// highlight-next-line
using RA.Utilities.Api.Results;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    [HttpPost]
    public IActionResult ProcessProduct(Product product)
    {
        try 
        {
            // ... some processing logic that might throw
            _service.Process(product);
            return Ok(new SuccessResponse<string>("Processed"));
        }
        catch (Exception ex)
        {
            // highlight-next-line
            return StatusCode(500, new ErrorResponse(new ErrorResult 
            { 
                ErrorCode = "InternalServerError", 
                ErrorMessage = "An unexpected error occurred while processing the product." 
            }));
        }
    }
}
```

### Example JSON Output

```json showLineNumbers
{
  "responseCode": 500,
  "responseType": "Error",
  "responseMessage": "A general error occurred during the operation.",
  "result": {
    "errorCode": "InternalServerError",
    "errorMessage": "An unexpected error occurred on the server."
  }
}
```
