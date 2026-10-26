---
title: Response<T>
sidebar_position: 1
---

```bash
Namespace: RA.Utilities.Api.Response
```

The `Response<T>` class is the universal wrapper for all API responses in the RA.Utilities ecosystem. It defines the JSON envelope shared by every response model: `responseCode`, `responseType`, `responseMessage`, and `result`.

### 🎯 Purpose

`Response<T>` is the base type for the package. It gives every success and failure response the same shape, so clients can parse the envelope once and then inspect the `Result` payload.

1. **Standardization**: Every response carries the same four properties.
2. **Predictability**: Clients can branch on `ResponseCode` or `ResponseType` before reading `Result`.
3. **Extensibility**: The package's response classes derive from `Response<T>`; you can derive from it too when you need a status code the package does not wrap.
4. **Immutability**: All properties use `init` accessors, so a response cannot be mutated after construction.

### Class Definition

```csharp showLineNumbers
public class Response<T>
{
    public int ResponseCode { get; init; }
    public ResponseType ResponseType { get; init; }
    public string? ResponseMessage { get; init; }
    public T? Result { get; init; }
}
```

### Properties

| Property | Type | Description |
|---|---|---|
| **ResponseCode** | `int` | The response code, typically an HTTP status code but allowed to be a domain-specific code. |
| **ResponseType** | `ResponseType` | The semantic outcome (`Success`, `BadRequest`, `NotFound`, ...) from `RA.Utilities.Core.Constants`. Serialized as a number by default; see JSON serialization. |
| **ResponseMessage** | `string?` | A human-readable message describing the outcome. |
| **Result** | `T?` | The payload. `null` for responses that carry no data. |

Because all four properties are `init`-only, object initializers work, but mutation after construction does not:

```csharp showLineNumbers
var response = new Response<string>
{
    ResponseCode = BaseResponseCode.Success,
    ResponseType = ResponseType.Success,
    ResponseMessage = "Operation completed successfully.",
    Result = "payload"
};
```

### JSON Serialization

ASP.NET Core serializes properties in camelCase by default. `ResponseType` is an enum, so `System.Text.Json` writes its **numeric value** unless a string-enum converter is registered.

```json showLineNumbers
{
  "responseCode": 200,
  // highlight-next-line
  "responseType": 0,
  "responseMessage": "Operation completed successfully.",
  "result": {
    "id": 123,
    "name": "Example Product"
  }
}
```

To write `"Success"` instead of `0`, register `JsonStringEnumConverter`:

```csharp showLineNumbers
using System.Text.Json.Serialization;

builder.Services.ConfigureHttpJsonOptions(options =>
{
    // highlight-next-line
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
```

For the full converter behavior and the numeric value of every member, see [`ResponseType`](../../core/RA.Utilities.Core.Constants/ResponseType#json-serialization).

### Derived Types

The package ships these derived response classes:

| Type | HTTP | Result type |
|---|---|---|
| [`SuccessResponse<T>`](./SuccessResponse) | 200 | `T` |
| [`BadRequestResponse`](./BadRequestResponse) | 400 | `BadRequestResult[]` |
| [`UnauthorizedResponse`](./UnauthorizedResponse) | 401 | `ErrorResult` |
| [`PaymentRequiredResponse`](./PaymentRequiredResponse) | 402 | `ErrorResult` |
| [`ForbiddenResponse`](./ForbiddenResponse) | 403 | `ErrorResult` |
| [`NotFoundResponse`](./NotFoundResponse) | 404 | `NotFoundResult` |
| [`ConflictResponse`](./ConflictResponse) | 409 | `ConflictResult` |
| [`UnprocessableResponse`](./UnprocessableResponse) | 422 | `ErrorResult` |
| [`TooManyRequestsResponse`](./TooManyRequestsResponse) | 429 | `ErrorResult` |
| [`ErrorResponse`](./ErrorResponse) | 500 | `ErrorResult` |
| [`ServiceUnavailableResponse`](./ServiceUnavailableResponse) | 503 | `ErrorResult` |
| [`GatewayTimeoutResponse`](./GatewayTimeoutResponse) | 504 | `ErrorResult` |

`Response<T>` has an implicit public parameterless constructor, so you can also instantiate it directly for a one-off response. Prefer a derived type when one matches your status code.
