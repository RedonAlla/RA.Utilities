---
title: ForbiddenResponse
sidebar_position: 7
---

```bash
Namespace: RA.Utilities.Api.Response
```

The `ForbiddenResponse` class creates a standardized `403 Forbidden` response. It derives from [`Response<T>`](./Response) with `T = ErrorResult` and is used when the caller is authenticated but lacks permission.

### 🎯 Purpose

Use `ForbiddenResponse` when authorization fails — for example, when a user tries to modify a resource they do not own.

- **Standard envelope**: `ResponseCode = 403`, `ResponseType = ResponseType.Forbidden`, `ResponseMessage = "You do not have permission to access this resource."`
- **Optional payload**: `Result` can be `null` or an [`ErrorResult`](./ErrorResult) describing the missing permission.

### Constructors

| Constructor | `Result` | `ResponseCode` | `ResponseMessage` |
|---|---|---|---|
| `ForbiddenResponse(int responseCode, string responseMessage)` | `null` | `responseCode` | `responseMessage` |
| `ForbiddenResponse(ErrorResult? result)` | `result` | `403` | `"You do not have permission to access this resource."` |
| `ForbiddenResponse(ErrorResult? result, int? responseCode, string responseMessage = "")` | `result` | `responseCode ?? 403` | `responseMessage ?? standard` (the `""` default is used as-is) |

`ResponseType` is always `ResponseType.Forbidden` (JSON `14`). There is no parameterless constructor; pass `null` to `ForbiddenResponse(ErrorResult?)` if you need no payload.

### Usage

```csharp showLineNumbers
using Microsoft.AspNetCore.Mvc;
// highlight-next-line
using RA.Utilities.Api.Response;

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
```

### Example JSON Output

```json showLineNumbers
{
  "responseCode": 403,
  // highlight-next-line
  "responseType": 14,
  "responseMessage": "You do not have permission to access this resource.",
  "result": {
    "errorCode": "AccessDenied",
    "errorMessage": "You do not have the 'Admin' role required to access this resource."
  }
}
```

With `JsonStringEnumConverter`, `responseType` is `"Forbidden"`.
