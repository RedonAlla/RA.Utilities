---
title: UnprocessableResponse
sidebar_position: 9
---

```powershell
Namespace: RA.Utilities.Api.Results
```

The `UnprocessableResponse` class is a specialized model for creating standardized `422 Unprocessable Entity` responses.
It is used when the server understands the content type of the request entity, but was unable to process the contained instructions.
It inherits from [`Response<T>`](./Response), with the `Result` property typed as an [`ErrorResult`](./ErrorResult) object.

### 🎯 Purpose

The `UnprocessableResponse` class is a specialized model for creating standardized `422 Unprocessable Entity` API responses.
It is used to signal that while the request was syntactically correct (not a `400 Bad Request`), the server could not perform the requested operation due to business logic or semantic errors.

Its primary functions are:

1. **Handles Semantic Errors**: It provides a way to communicate errors that are not strictly validation-related but prevent an operation from completing (e.g., "Insufficient funds", "Invalid transition state").

2. **Reduces Boilerplate**: It automatically sets the response properties for an unprocessable request:

  * **ResponseCode**: Set to `422` (from `BaseResponseCode.Unprocessable`).
  * **ResponseType**: Set to `ResponseType.Unprocessable`.
  * **ResponseMessage**: Defaults to `"The server understands the content type of the request entity, but was unable to process the contained instructions."` (from `BaseResponseMessages.Unprocessable`).

3. **Provides Structured Context**: It uses an [`ErrorResult`](./ErrorResult) payload to provide specific details about why the operation was invalid.

### ⚙️ How It Works

When you create an instance of `UnprocessableResponse`, it pre-configures the following properties:

- **`ResponseCode`**: Set to `422` (from `BaseResponseCode.Unprocessable`).
- **`ResponseType`**: Set to `ResponseType.Unprocessable`.
- **`ResponseMessage`**: Defaults to the standard unprocessable message.
- **`Result`**: An [`ErrorResult`](./ErrorResult) object containing the error code and message.

### 🚀 Usage in a Controller

You can use this class in your controller actions when a business rule is violated.

```csharp showLineNumbers
using Microsoft.AspNetCore.Mvc;
// highlight-next-line
using RA.Utilities.Api.Results;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
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
}
```

### Example JSON Output

```json showLineNumbers
{
  "responseCode": 422,
  "responseType": "Unprocessable",
  "responseMessage": "The server understands the content type of the request entity, but was unable to process the contained instructions.",
  "result": {
    "errorCode": "ORDER_ALREADY_SHIPPED",
    "errorMessage": "Cannot cancel an order that has already been shipped."
  }
}
```
