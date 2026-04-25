---
title: ForbiddenResponse
sidebar_position: 7
---

```powershell
Namespace: RA.Utilities.Api.Results
```

The `ForbiddenResponse` class is a specialized model for creating standardized `403 Forbidden` responses.
It is used when the server understands the request but refuses to authorize it.
It inherits from [`Response<T>`](./Response), with the `Result` property typed as an [`ErrorResult`](./ErrorResult) object.

### 🎯 Purpose

The `ForbiddenResponse` class is a specialized model for creating standardized `403 Forbidden` API responses.
It is used to signal that the authenticated user does not have the necessary permissions to access a specific resource or perform a specific action.

Its primary functions are:

1. **Standardizes Authorization Errors**: It provides a consistent structure for all `403 Forbidden` errors.

2. **Reduces Boilerplate**: It automatically sets the response properties for a forbidden request:

  * **ResponseCode**: Set to `403` (from `BaseResponseCode.Forbidden`).
  * **ResponseType**: Set to `ResponseType.Forbidden`.
  * **ResponseMessage**: Defaults to `"The server understood the request but refuses to authorize it."` (from `BaseResponseMessages.Forbidden`).

3. **Provides Structured Context**: It uses an [`ErrorResult`](./ErrorResult) payload to provide specific details about why the request was forbidden.

### ⚙️ How It Works

When you create an instance of `ForbiddenResponse`, it pre-configures the following properties:

- **`ResponseCode`**: Set to `403` (from `BaseResponseCode.Forbidden`).
- **`ResponseType`**: Set to `ResponseType.Forbidden`.
- **`ResponseMessage`**: Defaults to `"The server understood the request but refuses to authorize it."`.
- **`Result`**: An [`ErrorResult`](./ErrorResult) object containing the error code and message.

### 🚀 Usage in a Controller

You can use this class in your controller actions when a permission check fails.

```csharp showLineNumbers
using Microsoft.AspNetCore.Mvc;
// highlight-next-line
using RA.Utilities.Api.Results;

[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    [HttpGet("secure-data")]
    public IActionResult GetSecureData()
    {
        if (!User.IsInRole("Admin"))
        {
            // highlight-next-line
            return StatusCode(403, new ForbiddenResponse(new ErrorResult 
            { 
                ErrorCode = "AccessDenied", 
                ErrorMessage = "You do not have the 'Admin' role required to access this resource." 
            }));
        }

        return Ok(new SuccessResponse<string>("Sensitive Data"));
    }
}
```

### Example JSON Output

```json showLineNumbers
{
  "responseCode": 403,
  "responseType": "Forbidden",
  "responseMessage": "The server understood the request but refuses to authorize it.",
  "result": {
    "errorCode": "AccessDenied",
    "errorMessage": "You do not have the 'Admin' role required to access this resource."
  }
}
```
