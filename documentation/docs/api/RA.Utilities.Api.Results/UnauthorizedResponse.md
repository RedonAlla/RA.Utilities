---
title: UnauthorizedResponse
sidebar_position: 8
---

```powershell
Namespace: RA.Utilities.Api.Results
```

The `UnauthorizedResponse` class is a specialized model for creating standardized `401 Unauthorized` responses.
It is used when the request requires user authentication.
It inherits from [`Response<T>`](./Response), with the `Result` property typed as an [`ErrorResult`](./ErrorResult) object.

### 🎯 Purpose

The `UnauthorizedResponse` class is a specialized model for creating standardized `401 Unauthorized` API responses.
It is used to signal that the request lacks valid authentication credentials for the target resource.

Its primary functions are:

1. **Standardizes Authentication Errors**: It ensures that every `401 Unauthorized` error response has the exact same structure.

2. **Reduces Boilerplate**: It automatically sets the response properties for an unauthorized request:

  * **ResponseCode**: Set to `401` (from `BaseResponseCode.Unauthorized`).
  * **ResponseType**: Set to `ResponseType.Unauthorized`.
  * **ResponseMessage**: Defaults to `"The request requires user authentication."` (from `BaseResponseMessages.Unauthorized`).

3. **Provides Structured Context**: It can include an [`ErrorResult`](./ErrorResult) payload to provide specific details about why the authentication failed (e.g., "Invalid token", "Token expired").

### ⚙️ How It Works

When you create an instance of `UnauthorizedResponse`, it pre-configures the following properties:

- **`ResponseCode`**: Set to `401` (from `BaseResponseCode.Unauthorized`).
- **`ResponseType`**: Set to `ResponseType.Unauthorized`.
- **`ResponseMessage`**: Defaults to `"The request requires user authentication."`.
- **`Result`**: An optional [`ErrorResult`](./ErrorResult) object containing the error code and message.

### 🚀 Usage in a Controller

You can use this class in your controller actions or middleware when authentication fails.

```csharp showLineNumbers
using Microsoft.AspNetCore.Mvc;
// highlight-next-line
using RA.Utilities.Api.Results;

[ApiController]
[Route("api/[controller]")]
public class ProfileController : ControllerBase
{
    [HttpGet]
    public IActionResult GetProfile()
    {
        if (!User.Identity.IsAuthenticated)
        {
            // highlight-next-line
            return Unauthorized(new UnauthorizedResponse(new ErrorResult 
            { 
                ErrorCode = "Unauthorized", 
                ErrorMessage = "User is not authenticated." 
            }));
        }

        return Ok(new SuccessResponse<UserProfile>(_service.GetProfile(User)));
    }
}
```

### Example JSON Output

```json showLineNumbers
{
  "responseCode": 401,
  "responseType": "Unauthorized",
  "responseMessage": "The request requires user authentication.",
  "result": {
    "errorCode": "Unauthorized",
    "errorMessage": "User is not authenticated."
  }
}
```
