---
title: BadRequestResponse
sidebar_position: 3
---

```bash
Namespace: RA.Utilities.Api.Response
```

The `BadRequestResponse` class creates a standardized `400 Bad Request` response for validation failures. It derives from [`Response<T>`](./Response) with `T = BadRequestResult[]`, so `Result` always contains one or more structured validation errors.

### 🎯 Purpose

`BadRequestResponse` replaces ad-hoc validation error bodies with one consistent shape. Each element in `Result` describes a single field failure: which property failed, what value was attempted, what was expected, and a machine-readable error code.

- **Standard envelope**: `ResponseCode = 400`, `ResponseType = ResponseType.BadRequest`, `ResponseMessage = "The request is invalid."`
- **Typed payload**: `Result` is `BadRequestResult[]`, never a bare string.
- **Client-friendly**: Clients can map errors to form fields using `PropertyName`.

### Constructors

| Constructor | `Result` | `ResponseCode` | `ResponseMessage` |
|---|---|---|---|
| `BadRequestResponse(BadRequestResult error)` | `[error]` | `400` | `"The request is invalid."` |
| `BadRequestResponse(BadRequestResult error, int responseCode, string responseMessage)` | `[error]` | `responseCode` | `responseMessage` |
| `BadRequestResponse(BadRequestResult[] errors)` | `errors` | `400` | `"The request is invalid."` |
| `BadRequestResponse(BadRequestResult[] errors, int responseCode, string responseMessage)` | `errors` | `responseCode` | `responseMessage` |

`ResponseType` is always `ResponseType.BadRequest` (JSON `12`).

### BadRequestResult

`BadRequestResult` inherits from [`ErrorResult`](./ErrorResult).

| Property | Type | Description |
|---|---|---|
| **PropertyName** | `string?` | The request property that failed validation. Omitted from JSON when `null`. |
| **AttemptedValue** | `object?` | The value supplied by the client. Omitted when `null`. |
| **ExpectedValue** | `object?` | The expected value or format. Omitted when `null`. |
| **ErrorCode** | `string?` | Machine-readable error code (inherited). Omitted when `null`. |
| **ErrorMessage** | `string` | Human-readable description (inherited). Omitted when `null`. |

### Usage

Return a single validation error:

```csharp showLineNumbers
using Microsoft.AspNetCore.Mvc;
// highlight-next-line
using RA.Utilities.Api.Response;

[HttpPost]
public IActionResult CreateUser(CreateUserRequest request)
{
    if (string.IsNullOrEmpty(request.Email))
    {
        var error = new BadRequestResult
        {
            PropertyName = "Email",
            ErrorMessage = "'Email' must not be empty.",
            AttemptedValue = request.Email,
            ErrorCode = "NotEmptyValidator"
        };

        // highlight-next-line
        return BadRequest(new BadRequestResponse(error));
    }

    // ... create the user
    return Ok(new SuccessResponse<User>(user));
}
```

Return several validation errors at once:

```csharp showLineNumbers
var errors = new[]
{
    new BadRequestResult
    {
        PropertyName = "Email",
        ErrorMessage = "'Email' must not be empty.",
        AttemptedValue = request.Email,
        ErrorCode = "NotEmptyValidator"
    },
    new BadRequestResult
    {
        PropertyName = "Age",
        ErrorMessage = "'Age' must be 18 or older.",
        AttemptedValue = request.Age,
        ErrorCode = "AGE_TOO_LOW"
    }
};

// highlight-next-line
return BadRequest(new BadRequestResponse(errors));
```

### Example JSON Output

```json showLineNumbers
{
  "responseCode": 400,
  // highlight-next-line
  "responseType": 12,
  "responseMessage": "The request is invalid.",
  "result": [
    {
      "propertyName": "Email",
      "attemptedValue": "",
      "errorCode": "NotEmptyValidator",
      "errorMessage": "'Email' must not be empty."
    }
  ]
}
```

`expectedValue` is omitted because it is `null` (the property uses `JsonIgnoreCondition.WhenWritingNull`). With `JsonStringEnumConverter`, `responseType` is `"BadRequest"`.
