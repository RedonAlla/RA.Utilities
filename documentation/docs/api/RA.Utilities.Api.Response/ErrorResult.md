---
title: ErrorResult
sidebar_position: 13
---

```bash
Namespace: RA.Utilities.Api.Response
```

The `ErrorResult` class is the base payload for structured errors. It is used directly by [`ErrorResponse`](./ErrorResponse), [`UnauthorizedResponse`](./UnauthorizedResponse), [`ForbiddenResponse`](./ForbiddenResponse), [`UnprocessableResponse`](./UnprocessableResponse), [`TooManyRequestsResponse`](./TooManyRequestsResponse), [`ServiceUnavailableResponse`](./ServiceUnavailableResponse), and [`GatewayTimeoutResponse`](./GatewayTimeoutResponse), and it is the base class for [`BadRequestResult`](./BadRequestResponse#badrequestresult), [`NotFoundResult`](./NotFoundResponse#notfoundresult), and [`ConflictResult`](./ConflictResponse#conflictresult).

### 🎯 Purpose

`ErrorResult` gives error responses two standard fields: a machine-readable `ErrorCode` and a human-readable `ErrorMessage`. Both are optional on the wire and are omitted when `null`.

### Class Definition

```csharp showLineNumbers
using System.Text.Json.Serialization;

public class ErrorResult
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ErrorCode { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string ErrorMessage { get; set; }
}
```

### Properties

| Property | Type | Description |
|---|---|---|
| **ErrorCode** | `string?` | Machine-readable code for client-side handling. Omitted from JSON when `null`. |
| **ErrorMessage** | `string` | Human-readable description of the error. Omitted from JSON when `null`. |

:::note[Nominally non-nullable `ErrorMessage`]

`ErrorMessage` is declared as `string`, but the class does not initialize it. `new ErrorResult()` therefore leaves it `null` at runtime; always set it when you return the object:

```csharp showLineNumbers
var error = new ErrorResult
{
    ErrorCode = "PROCESSING_ERROR",
    ErrorMessage = "An unexpected error occurred."
};
```

:::

### Usage

```csharp showLineNumbers
using RA.Utilities.Api.Response;

var error = new ErrorResult
{
    ErrorCode = "PROCESSING_ERROR",
    ErrorMessage = "An unexpected error occurred."
};

return StatusCode(500, new ErrorResponse(error));
```

### Derived Types

| Type | Adds | Used by |
|---|---|---|
| [`BadRequestResult`](./BadRequestResponse#badrequestresult) | `PropertyName`, `AttemptedValue`, `ExpectedValue` | `BadRequestResponse` |
| [`NotFoundResult`](./NotFoundResponse#notfoundresult) | `Entity`, `Value` | `NotFoundResponse` |
| [`ConflictResult`](./ConflictResponse#conflictresult) | `Entity`, `Value` | `ConflictResponse` |

### Example JSON Output

```json showLineNumbers
{
  "errorCode": "PROCESSING_ERROR",
  "errorMessage": "An unexpected error occurred."
}
```
