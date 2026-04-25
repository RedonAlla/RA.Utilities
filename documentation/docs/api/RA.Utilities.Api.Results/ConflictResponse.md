---
title: ConflictResponse
sidebar_position: 5
---

```powershell
Namespace: RA.Utilities.Api.Results
```

The `ConflictResponse` class is a specialized model for creating standardized `409 Conflict` responses.
It is used when a request cannot be completed because it conflicts with the current state of a resource, such as attempting to create a resource that already exists.
It inherits from [`Response<T>`](./Response), with the `Result` property typed as a [`ConflictResult`](#conflictresult) object.

### 🎯 Purpose

The `ConflictResponse` class is a specialized model for creating standardized `409 Conflict` API responses.
It inherits from the base [`Response<T>`](./Response) class and is used to signal that a request could not be completed because it conflicts with the current state of a resource.

Here are its primary functions:

1. **Standardizes Conflict Errors**: It provides a consistent structure for all `409 Conflict` errors.

2. **Reduces Boilerplate**: It automatically sets the response properties for a conflict scenario:

  * **ResponseCode**: Set to `409` (from `BaseResponseCode.Conflict`).
  * **ResponseType**: Set to `ResponseType.Conflict`.
  * **ResponseMessage**: Defaults to `"The request could not be completed due to a conflict with the current state of the resource."` (from `BaseResponseMessages.Conflict`).

3. **Provides Structured Context**: The response payload is a [`ConflictResult`](#conflictresult) object, which contains the entity name and the value that caused the conflict.

### ⚙️ How It Works

When you create an instance of `ConflictResponse`, it pre-configures the following properties:

- **`ResponseCode`**: Set to `409` (from `BaseResponseCode.Conflict`).
- **`ResponseType`**: Set to `ResponseType.Conflict`.
- **`ResponseMessage`**: Defaults to the standard conflict message.
- **`Result`**: A [`ConflictResult`](#conflictresult) object containing the `Entity` name and conflicting `Value`.

### 🚀 Usage in a Controller

```csharp showLineNumbers
using Microsoft.AspNetCore.Mvc;
// highlight-next-line
using RA.Utilities.Api.Results;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    [HttpPost]
    public IActionResult CreateUser(CreateUserRequest request)
    {
        if (_userService.EmailExists(request.Email))
        {
            // Return a 409 Conflict with structured details
            // highlight-next-line
            return Conflict(new ConflictResponse(new ConflictResult("User", request.Email)));
        }

        // ... proceed with user creation
        return Ok();
    }
}
```

### ConflictResult
Inherits from [`ErrorResult`](./ErrorResult).

| Property | Type | Description |
| -------- | -------- | ---------------------------------------------- |
| **Entity** | `string` | The type of resource causing the conflict (e.g., `"User"`). |
| **Value** | `object` | The specific value that caused the conflict (e.g., `"test@example.com"`). |
| **ErrorCode** | `string` | The specific error code (inherited from `ErrorResult`). |
| **ErrorMessage** | `string` | The error message (inherited from `ErrorResult`). |

### Example JSON Output

```json showLineNumbers
{
  "responseCode": 409,
  "responseType": "Conflict",
  "responseMessage": "The request could not be completed due to a conflict with the current state of the resource.",
  "result": {
    "entity": "User",
    "value": "existing.user@example.com",
    "errorCode": "Conflict",
    "errorMessage": "The request could not be completed due to a conflict with the current state of the resource."
  }
}
```
