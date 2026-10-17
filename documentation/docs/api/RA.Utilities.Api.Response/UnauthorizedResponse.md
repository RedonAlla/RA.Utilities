---
title: UnauthorizedResponse
sidebar_position: 8
---

```bash
Namespace: RA.Utilities.Api.Response
```

The `UnauthorizedResponse` class creates a standardized `401 Unauthorized` response. It derives from [`Response<T>`](./Response) with `T = ErrorResult` and is used when authentication is missing or invalid.

### 🎯 Purpose

Use `UnauthorizedResponse` when the request lacks valid credentials — for example, when a token is missing, expired, or rejected.

- **Standard envelope**: `ResponseCode = 401`, `ResponseType = ResponseType.Unauthorized`, `ResponseMessage = "Authentication failed or is missing."`
- **Optional payload**: `Result` can be `null` or an [`ErrorResult`](./ErrorResult) describing the authentication failure.

### Constructors

| Constructor | `Result` | `ResponseCode` | `ResponseMessage` |
|---|---|---|---|
| `UnauthorizedResponse(int responseCode, string responseMessage)` | `null` | `responseCode` | `responseMessage` |
| `UnauthorizedResponse(ErrorResult? result = null)` | `result` | `401` | `"Authentication failed or is missing."` |
| `UnauthorizedResponse(ErrorResult result, int? responseCode, string responseMessage = "")` | `result` | `responseCode ?? 401` | `responseMessage ?? standard` (the `""` default is used as-is) |

`ResponseType` is always `ResponseType.Unauthorized` (JSON `10`). The optional `result` parameter means `new UnauthorizedResponse()` is valid.

### Usage

```csharp showLineNumbers
using Microsoft.AspNetCore.Mvc;
// highlight-next-line
using RA.Utilities.Api.Response;

[HttpGet]
public IActionResult GetProfile()
{
    if (User.Identity?.IsAuthenticated != true)
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
```

### Example JSON Output

```json showLineNumbers
{
  "responseCode": 401,
  // highlight-next-line
  "responseType": 10,
  "responseMessage": "Authentication failed or is missing.",
  "result": {
    "errorCode": "Unauthorized",
    "errorMessage": "User is not authenticated."
  }
}
```

With `JsonStringEnumConverter`, `responseType` is `"Unauthorized"`.
