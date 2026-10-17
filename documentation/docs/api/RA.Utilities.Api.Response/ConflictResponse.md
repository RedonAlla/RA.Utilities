---
title: ConflictResponse
sidebar_position: 5
---

```bash
Namespace: RA.Utilities.Api.Response
```

The `ConflictResponse` class creates a standardized `409 Conflict` response. It derives from [`Response<T>`](./Response) with `T = ConflictResult`, so `Result` identifies the entity and value that caused the conflict.

### 🎯 Purpose

Use `ConflictResponse` when a request cannot be completed because it conflicts with existing state — for example, creating a user whose email address is already registered.

- **Standard envelope**: `ResponseCode = 409`, `ResponseType = ResponseType.Conflict`, `ResponseMessage = "A conflict occurred with the current state of the resource."`
- **Typed payload**: `Result` is a [`ConflictResult`](#conflictresult) with `Entity` and `Value`.
- **Customizable**: Override the envelope code/message or the nested error code/message.

### Constructors

| Constructor | `Result` | `ResponseCode` | `ResponseMessage` |
|---|---|---|---|
| `ConflictResponse(ConflictResult model)` | `model` | `409` | `"A conflict occurred with the current state of the resource."` |
| `ConflictResponse(ConflictResult model, int responseCode, string responseMessage)` | `model` | `responseCode` | `responseMessage` |

`ResponseType` is always `ResponseType.Conflict` (JSON `9`).

### ConflictResult

`ConflictResult` inherits from [`ErrorResult`](./ErrorResult).

| Property | Type | Description |
|---|---|---|
| **Entity** | `string` | The resource type that caused the conflict (e.g., `"User"`). |
| **Value** | `object` | The value that caused the conflict (e.g., `"test@example.com"`). |
| **ErrorCode** | `string?` | Machine-readable code (inherited). Defaults to `null` and is omitted from JSON. |
| **ErrorMessage** | `string` | Human-readable message (inherited). Defaults to an **empty string**, not the standard envelope message. |

Constructor:

```csharp showLineNumbers
public ConflictResult(
    string entity,
    object value,
    string? errorCode = null,
    string errorMessage = "")
```

:::warning[Nested message defaults to empty]

`new ConflictResult("User", "a@b.com")` leaves `ErrorMessage` empty. The `ConflictResponse(ConflictResult)` convenience constructor only sets the envelope message. Set the nested message explicitly when clients need it:

```csharp showLineNumbers
var conflict = new ConflictResult("User", request.Email)
{
    ErrorCode = "EMAIL_ALREADY_EXISTS",
    ErrorMessage = "A user with this email address already exists."
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
public class UsersController : ControllerBase
{
    [HttpPost]
    public IActionResult CreateUser(CreateUserRequest request)
    {
        if (_userService.EmailExists(request.Email))
        {
            var conflict = new ConflictResult("User", request.Email)
            {
                ErrorCode = "EMAIL_ALREADY_EXISTS",
                // highlight-next-line
                ErrorMessage = $"A user with email '{request.Email}' already exists."
            };

            // highlight-next-line
            return Conflict(new ConflictResponse(conflict));
        }

        // ... create the user
        return Ok(new SuccessResponse<User>(user));
    }
}
```

### Example JSON Output

```json showLineNumbers
{
  "responseCode": 409,
  // highlight-next-line
  "responseType": 9,
  "responseMessage": "A conflict occurred with the current state of the resource.",
  "result": {
    "entity": "User",
    "value": "existing.user@example.com",
    "errorCode": "EMAIL_ALREADY_EXISTS",
    "errorMessage": "A user with email 'existing.user@example.com' already exists."
  }
}
```

With `JsonStringEnumConverter`, `responseType` is `"Conflict"`.
