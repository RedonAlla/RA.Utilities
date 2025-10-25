---
title: ConflictResponse
sidebar_position: 5
---

```powershell
Namespace: RA.Utilities.Api.Results
```

The `ConflictResponse` class is a specialized model for creating standardized `409 Conflict` responses.
It is used when a request cannot be completed because it conflicts with the current state of a resource, such as attempting to create a resource that already exists.
It inherits from [`Response<T>`](./Response), with the `Result` property typed as a `ConflictResult` object.

### 🎯 Purpose

The `ConflictResponse` class is a specialized model for creating standardized `409 Conflict` API responses.
It inherits from the base [`Response<T>`](./Response) class and is used to signal that a request could not be completed because it conflicts with the current state of a resource. The most common use case is attempting to create a resource that already exists (e.g., registering a user with an email that is already in use).

Here are its primary functions:

1. **Standardizes Conflict Errors**: It provides a consistent structure for all `409 Conflict` errors, ensuring clients receive a predictable response format.

2. **Reduces Boilerplate**: It automatically sets the response properties for a conflict scenario:

  * **ResponseCode**: Set to `409` (from `BaseResponseCode.Conflict`).
  * **ResponseType**: Set to `ResponseType.Conflict`.
  * **ResponseMessage**: Generates a clear, dynamic message like `"User with value 'test@example.com' already exists."`.

3. **Provides Structured Context**: The response payload is a [`ConflictResult`](#conflictresult) object, which contains two important pieces of information:

  * **Entity**: The type of resource causing the conflict (e.g., "User").
  * **Value**: The specific value that caused the conflict (e.g., "test@example.com").

This structured information allows clients to provide specific and helpful feedback to the user, such as "This email address is already taken."

### ConflictResult
Represents the result of a conflict, typically indicating that an entity with the given name and value already exists.

| Property   | Type     | Description     |
| ---------- | -------- | --------------- |
| **Entity** | `string` | The type of resource causing the conflict (e.g., `"User"`). |
| **Entity** | `object` | The specific value that caused the conflict (e.g., `"test@example.com"`). |

### ⚙️ How It Works

When you create an instance of `ConflictResponse`, it pre-configures the following properties:

- **`ResponseCode`**: Set to `409` (from `BaseResponseCode.Conflict`).
- **`ResponseType`**: Set to `ResponseType.Conflict`.
- **`ResponseMessage`**: A dynamically generated message, such as `"User with value 'test@example.com' already exists."`.
- **`Result`**: A [`ConflictResult`](#conflictresult)  object containing the `Entity` name and conflicting `Value`.

### 🚀 Usage in a Controller

You can use this class in your controller actions when an operation violates a uniqueness constraint, such as trying to create a user with an email that is already registered.

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
            return Conflict(new ConflictResponse("User", request.Email));
        }

        var newUser = _userService.Create(request);
        return CreatedAtAction(nameof(GetUser), new { id = newUser.Id }, newUser);
    }
}
```

### Example JSON Output

A `POST` request to `/api/users` with an email that already exists would produce the following JSON response body:

```json showLineNumbers
{
  "responseCode": 409,
  "responseType": "Conflict",
  "responseMessage": "User with value 'existing.user@example.com' already exists.",
  "result": {
    "entity": "User",
    "value": "existing.user@example.com"
  }
}
```
