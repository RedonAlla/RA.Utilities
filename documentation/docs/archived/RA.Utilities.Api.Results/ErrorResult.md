---
title: ErrorResult
sidebar_position: 10
---

```powershell
Namespace: RA.Utilities.Api.Results
```

The `ErrorResult` class is a base model used to represent structured error information in API responses. It is used as the `Result` payload for several specialized response types, including [`ErrorResponse`](./ErrorResponse), [`ForbiddenResponse`](./ForbiddenResponse), [`UnauthorizedResponse`](./UnauthorizedResponse), and [`UnprocessableResponse`](./UnprocessableResponse).

### 🎯 Purpose

The `ErrorResult` class provides a consistent way to return error details to the client. Instead of returning a simple string, it allows the API to provide both a machine-readable error code and a human-friendly error message.

### Class Definition

```csharp showLineNumbers
public class ErrorResult
{
    public string ErrorCode { get; set; }
    public string ErrorMessage { get; set; }
}
```

### Properties

| Property | Type | Description |
| -------- | --- | ------------------------------------------------ |
| **ErrorCode** | `string` | A specific error code that can be used for translation or programmatic handling on the client side. |
| **ErrorMessage** | `string` | A descriptive message explaining the error. |

### ⚙️ How It's Used

`ErrorResult` is rarely used on its own. Instead, it serves as the data payload for other response models.

For example, when using [`ErrorResponse`](./ErrorResponse):

```csharp showLineNumbers
var errorResult = new ErrorResult 
{ 
    ErrorCode = "InternalServerError", 
    ErrorMessage = "An unexpected error occurred." 
};

var response = new ErrorResponse(errorResult);
```

### Example JSON Output

When serialized as part of a response, it looks like this:

```json showLineNumbers
{
  "errorCode": "InternalServerError",
  "errorMessage": "An unexpected error occurred."
}
```
