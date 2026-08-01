# RA.Utilities.Api.Results

[![NuGet version](https://img.shields.io/nuget/v/RA.Utilities.Api.Results.svg?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Api.Results/)
[![Codecov](https://codecov.io/github/RedonAlla/RA.Utilities/graph/badge.svg)](https://codecov.io/github/RedonAlla/RA.Utilities)
[![NuGet Downloads](https://img.shields.io/nuget/dt/RA.Utilities.Api.Results.svg)](https://www.nuget.org/packages/RA.Utilities.Api.Results/)
[![Documentation](https://img.shields.io/badge/Documentation-read-brightgreen.svg?logo=readthedocs&logoColor=fff)](https://redonalla.github.io/RA.Utilities/nuget-packages/api/RA.Utilities.Api.Results/)
[![GitHub license](https://img.shields.io/github/license/RedonAlla/RA.Utilities?logo=googledocs&logoColor=fff)](https://github.com/RedonAlla/RA.Utilities?tab=MIT-1-ov-file)

`RA.Utilities.Api.Results` provides a standardized set of models for creating consistent API responses within the RA.Utilities ecosystem.
This package includes generic wrappers for success, error, and domain-specific failure scenarios, helping to streamline API development and improve client-side handling.

## Purpose

When building APIs, it's crucial to have a consistent and predictable response structure.
This package provides a set of ready-to-use C# classes that can be returned from your ASP.NET Core applications.
These models ensure that every response—whether it's a success, a generic error, or a detailed validation failure—has the same shape, making it easier for clients to parse and handle.

This library is designed to work seamlessly with the other `RA.Utilities` packages, such as `RA.Utilities.Core.Constants` for status codes and messages, and `RA.Utilities.Api` for endpoint helpers and exception handling middleware.

## 🛠️ Installation

You can install the package via the .NET CLI:

```bash
dotnet add package RA.Utilities.Api.Results
```

Or through the NuGet Package Manager in Visual Studio.

## 🔗 Dependencies

- [`RA.Utilities.Core.Constants`](https://redonalla.github.io/RA.Utilities/nuget-packages/core/RA.Utilities.Core.Constants/)

---

## Usage Example

Here is a simple example of how you can use these response models in an ASP.NET Core controller action.

```csharp
using Microsoft.AspNetCore.Mvc;
using RA.Utilities.Api.Results;
using RA.Utilities.Core.Constants;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    [HttpGet("{id}")]
    public IActionResult GetProduct(int id)
    {
        if (id <= 0)
        {
            // Return a 400 Bad Request with validation details
            var validationError = new BadRequestResult
            {
                PropertyName = "Id",
                ErrorMessage = "Product ID must be a positive number.",
                AttemptedValue = id
            };
            return BadRequest(new BadRequestResponse(validationError));
        }

        // Simulate finding a product
        var product = GetProductFromDatabase(id);

        if (product == null)
        {
            // Return a 404 Not Found
            return NotFound(new NotFoundResponse(new NotFoundResult("Product", id)));
        }

        try
        {
            // Simulate an operation that might fail
            product.Process();
        }
        catch (Exception ex)
        {
            // Return a 500 Internal Server Error
            return StatusCode(500, new ErrorResponse(new ErrorResult
            {
                ErrorCode = "PROCESSING_ERROR",
                ErrorMessage = ex.Message
            }));
        }

        // Return a 200 OK with the product data
        return Ok(new SuccessResponse<Product>(product));
    }
}
```

## Available Result Models

The core of the package is `Response<T>`, a sealed class that serves as a universal wrapper for all API responses. All properties use `init`-only accessors to ensure immutability after construction.

### `Response<T>`

Namespace: RA.Utilities.Api.Results<br>
Package: RA.Utilities.Api.Results<br>
Source: [RA.Utilities.Api.Results](https://github.com/RedonAlla/RA.Utilities.git)

```csharp
public class Response<T>
```

#### Properties

| Property          | Type           | Description                                                 |
| ----------------- | -------------- | ----------------------------------------------------------- |
| `ResponseCode`    | `int`          | A code for the response, often mapping to HTTP status codes. |
| `ResponseType`    | `ResponseType` | A record indicating the type of response (e.g., Success, Error). |
| `ResponseMessage` | `string?`       | A human-friendly message describing the response.             |
| `Result`          | `T?`           | The actual data payload of the response.                    |

---

### `SuccessResponse<T>`

Namespace: RA.Utilities.Api.Results<br>
Package: RA.Utilities.Api.Results<br>
Source: [RA.Utilities.Api.Results](https://github.com/RedonAlla/RA.Utilities.git)

```csharp
public sealed class SuccessResponse<T> : Response<T>
```

Creates a `Response<T>` object that produces a `Success` response, typically corresponding to an HTTP 200 OK status.

#### Defaults

| Property          | Value                                                 |
| ----------------- | ----------------------------------------------------- |
| `ResponseCode`    | `200` (from `BaseResponseCode.Success`)               |
| `ResponseMessage` | `"Operation completed successfully."`                  |
| `ResponseType`    | `ResponseType.Success`                                |

#### Example response:

```json
{
  "responseCode": 200,
  "responseType": "Success",
  "responseMessage": "Operation completed successfully.",
  "result": {
    // The 'T' payload goes here
  }
}
```

---

### `BadRequestResponse`

Namespace: RA.Utilities.Api.Results<br>
Package: RA.Utilities.Api.Results<br>
Source: [RA.Utilities.Api.Results](https://github.com/RedonAlla/RA.Utilities.git)

```csharp
public sealed class BadRequestResponse : Response<BadRequestResult[]>
```

Creates a `Response<BadRequestResult[]>` object for validation errors, producing a `BadRequest` response (HTTP 400).

The `Result` property contains an array of `BadRequestResult` objects, where each object details a specific validation failure.

#### Defaults

| Property          | Value                                           |
| ----------------- | ----------------------------------------------- |
| `ResponseCode`    | `400` (from `BaseResponseCode.BadRequest`)       |
| `ResponseMessage` | `"The request is invalid."`                      |
| `ResponseType`    | `ResponseType.BadRequest`                        |
| `Result`          | array of [`BadRequestResult`](#badrequestresult) |

#### `BadRequestResult`

Inherits from [`ErrorResult`](#errorresult).

| Property         | Type     | Description                                   |
| ---------------- | -------- | --------------------------------------------- |
| `PropertyName`   | `string?` | The name of the property that failed validation. |
| `ErrorMessage`   | `string`  | The error message (inherited from `ErrorResult`). |
| `AttemptedValue` | `object?` | The property value that caused the failure.     |
| `ExpectedValue`  | `object?` | The expected value for the property.            |
| `ErrorCode`      | `string?` | The specific error code (inherited from `ErrorResult`). |

#### Example response:

```json
{
  "responseCode": 400,
  "responseType": "BadRequest",
  "responseMessage": "The request is invalid.",
  "result": [
    {
      "propertyName": "Email",
      "errorMessage": "Email is not a valid email address.",
      "attemptedValue": "not-an-email",
      "errorCode": "INVALID_FORMAT"
    },
    {
      "propertyName": "Age",
      "errorMessage": "Age must be greater than 18.",
      "attemptedValue": 16,
      "errorCode": "AGE_TOO_LOW"
    }
  ]
}
```

---

### `NotFoundResponse`

Namespace: RA.Utilities.Api.Results<br>
Package: RA.Utilities.Api.Results<br>
Source: [RA.Utilities.Api.Results](https://github.com/RedonAlla/RA.Utilities.git)

```csharp
public sealed class NotFoundResponse : Response<NotFoundResult>
```

Creates a `Response<NotFoundResult>` object that produces a `NotFound` response (HTTP 404).
Used when a requested resource could not be found.

#### Defaults

| Property          | Value                                              |
| ----------------- | -------------------------------------------------- |
| `ResponseCode`    | `404` (from `BaseResponseCode.NotFound`)            |
| `ResponseMessage` | `"The requested resource was not found."`            |
| `ResponseType`    | `ResponseType.NotFound`                             |
| `Result`          | [`NotFoundResult`](#notfoundresult)                 |

#### `NotFoundResult`

Inherits from [`ErrorResult`](#errorresult).

| Property | Type     | Description                                                       |
| -------- | -------- | ----------------------------------------------------------------- |
| `Entity`  | `string` | The name of the entity that was not found (e.g., "Product").       |
| `Value`   | `object` | The identifier or value used to search for the entity (e.g., 123). |

#### Example response:

```json
{
  "responseCode": 404,
  "responseType": "NotFound",
  "responseMessage": "The requested resource was not found.",
  "result": {
    "entity": "Product",
    "value": 999,
    "errorCode": "NotFound",
    "errorMessage": "The requested resource was not found."
  }
}
```

---

### `ConflictResponse`

Namespace: RA.Utilities.Api.Results<br>
Package: RA.Utilities.Api.Results<br>
Source: [RA.Utilities.Api.Results](https://github.com/RedonAlla/RA.Utilities.git)

```csharp
public sealed class ConflictResponse : Response<ConflictResult>
```

Creates a `Response<ConflictResult>` object that produces a `Conflict` response (HTTP 409).
Used when an action cannot be completed because it conflicts with the current state of a resource.

#### Defaults

| Property          | Value                                                               |
| ----------------- | ------------------------------------------------------------------- |
| `ResponseCode`    | `409` (from `BaseResponseCode.Conflict`)                             |
| `ResponseMessage` | `"A conflict occurred with the current state of the resource."`       |
| `ResponseType`    | `ResponseType.Conflict`                                              |
| `Result`          | [`ConflictResult`](#conflictresult)                                  |

#### `ConflictResult`

Inherits from [`ErrorResult`](#errorresult).

| Property | Type     | Description                                          |
| -------- | -------- | ---------------------------------------------------- |
| `Entity`  | `string` | The name of the entity causing the conflict.          |
| `Value`   | `object` | The value of the entity that caused the conflict.     |

#### Example response:

```json
{
  "responseCode": 409,
  "responseType": "Conflict",
  "responseMessage": "A conflict occurred with the current state of the resource.",
  "result": {
    "entity": "User",
    "value": "existing@example.com",
    "errorCode": "Conflict",
    "errorMessage": "A conflict occurred with the current state of the resource."
  }
}
```

---

### `UnauthorizedResponse`

Namespace: RA.Utilities.Api.Results<br>
Package: RA.Utilities.Api.Results<br>
Source: [RA.Utilities.Api.Results](https://github.com/RedonAlla/RA.Utilities.git)

```csharp
public sealed class UnauthorizedResponse : Response<ErrorResult>
```

Creates a `Response<ErrorResult>` object for an unauthorized request (HTTP 401).
Used when the request requires user authentication.

#### Defaults

| Property          | Value                                                 |
| ----------------- | ----------------------------------------------------- |
| `ResponseCode`    | `401` (from `BaseResponseCode.Unauthorized`)           |
| `ResponseMessage` | `"Authentication failed or is missing."`               |
| `ResponseType`    | `ResponseType.Unauthorized`                            |
| `Result`          | [`ErrorResult`](#errorresult)                          |

#### Example response:

```json
{
  "responseCode": 401,
  "responseType": "Unauthorized",
  "responseMessage": "Authentication failed or is missing.",
  "result": null
}
```

---

### `ForbiddenResponse`

Namespace: RA.Utilities.Api.Results<br>
Package: RA.Utilities.Api.Results<br>
Source: [RA.Utilities.Api.Results](https://github.com/RedonAlla/RA.Utilities.git)

```csharp
public sealed class ForbiddenResponse : Response<ErrorResult>
```

Creates a `Response<ErrorResult>` object for a forbidden request (HTTP 403).
Used when the server understands the request but refuses to authorize it.

#### Defaults

| Property          | Value                                                        |
| ----------------- | ------------------------------------------------------------ |
| `ResponseCode`    | `403` (from `BaseResponseCode.Forbidden`)                     |
| `ResponseMessage` | `"You do not have permission to access this resource."`        |
| `ResponseType`    | `ResponseType.Forbidden`                                      |
| `Result`          | [`ErrorResult`](#errorresult)                                 |

#### Example response:

```json
{
  "responseCode": 403,
  "responseType": "Forbidden",
  "responseMessage": "You do not have permission to access this resource.",
  "result": null
}
```

---

### `TooManyRequestsResponse`

Namespace: RA.Utilities.Api.Results<br>
Package: RA.Utilities.Api.Results<br>
Source: [RA.Utilities.Api.Results](https://github.com/RedonAlla/RA.Utilities.git)

```csharp
public sealed class TooManyRequestsResponse : Response<ErrorResult>
```

Creates a `Response<ErrorResult>` object for a rate limit error (HTTP 429).
Used when the client has sent too many requests in a given amount of time.

#### Defaults

| Property          | Value                                                     |
| ----------------- | --------------------------------------------------------- |
| `ResponseCode`    | `429` (from `BaseResponseCode.TooManyRequests`)            |
| `ResponseMessage` | `"Too many requests. Please try again later."`              |
| `ResponseType`    | `ResponseType.TooManyRequests`                             |
| `Result`          | [`ErrorResult`](#errorresult)                              |

---

### `UnprocessableResponse`

Namespace: RA.Utilities.Api.Results<br>
Package: RA.Utilities.Api.Results<br>
Source: [RA.Utilities.Api.Results](https://github.com/RedonAlla/RA.Utilities.git)

```csharp
public sealed class UnprocessableResponse : Response<ErrorResult>
```

Creates a `Response<ErrorResult>` object that produces an `Unprocessable` response (HTTP 422).
Used when the server understands the content type but was unable to process the contained instructions.

#### Defaults

| Property          | Value                                                   |
| ----------------- | ------------------------------------------------------- |
| `ResponseCode`    | `422` (from `BaseResponseCode.Unprocessable`)            |
| `ResponseMessage` | `"Unprocessable entity."`                                 |
| `ResponseType`    | `ResponseType.Unprocessable`                             |
| `Result`          | [`ErrorResult`](#errorresult)                            |

---

### `ServiceUnavailableResponse`

Namespace: RA.Utilities.Api.Results<br>
Package: RA.Utilities.Api.Results<br>
Source: [RA.Utilities.Api.Results](https://github.com/RedonAlla/RA.Utilities.git)

```csharp
public sealed class ServiceUnavailableResponse : Response<ErrorResult>
```

Creates a `Response<ErrorResult>` object for a service unavailable error (HTTP 503).
Used when the server is temporarily unable to handle the request.

#### Defaults

| Property          | Value                                                              |
| ----------------- | ------------------------------------------------------------------ |
| `ResponseCode`    | `503` (from `BaseResponseCode.ServiceUnavailable`)                  |
| `ResponseMessage` | `"The service is temporarily unavailable. Please try again later."` |
| `ResponseType`    | `ResponseType.ServiceUnavailable`                                  |
| `Result`          | [`ErrorResult`](#errorresult)                                      |

---

### `GatewayTimeoutResponse`

Namespace: RA.Utilities.Api.Results<br>
Package: RA.Utilities.Api.Results<br>
Source: [RA.Utilities.Api.Results](https://github.com/RedonAlla/RA.Utilities.git)

```csharp
public sealed class GatewayTimeoutResponse : Response<ErrorResult>
```

Creates a `Response<ErrorResult>` object for a gateway timeout error (HTTP 504).
Used when the server, acting as a gateway or proxy, did not receive a timely response from the upstream server.

#### Defaults

| Property          | Value                                                                                               |
| ----------------- | --------------------------------------------------------------------------------------------------- |
| `ResponseCode`    | `504` (from `BaseResponseCode.GatewayTimeout`)                                                      |
| `ResponseMessage` | `"The server, while acting as a gateway or proxy, did not receive a timely response from the upstream server."` |
| `ResponseType`    | `ResponseType.GatewayTimeout`                                                                       |
| `Result`          | [`ErrorResult`](#errorresult)                                                                       |

---

### `ErrorResponse`

Namespace: RA.Utilities.Api.Results<br>
Package: RA.Utilities.Api.Results<br>
Source: [RA.Utilities.Api.Results](https://github.com/RedonAlla/RA.Utilities.git)

```csharp
public sealed class ErrorResponse : Response<ErrorResult>
```

Creates a `Response<ErrorResult>` that produces a generic `Error` response (HTTP 500).
This response type is used for unexpected server-side errors and serves as a fallback for unhandled exceptions.

#### Defaults

| Property          | Value                                                   |
| ----------------- | ------------------------------------------------------- |
| `ResponseCode`    | `500` (from `BaseResponseCode.InternalServerError`)      |
| `ResponseMessage` | `"Something happened on our end."`                       |
| `ResponseType`    | `ResponseType.Error`                                     |
| `Result`          | [`ErrorResult`](#errorresult)                            |

#### Example response:

```json
{
  "responseCode": 500,
  "responseType": "Error",
  "responseMessage": "Something happened on our end.",
  "result": {
    "errorCode": "InternalServerError",
    "errorMessage": "An unexpected error occurred on the server."
  }
}
```

---

### `ErrorResult`

Namespace: RA.Utilities.Api.Results<br>
Package: RA.Utilities.Api.Results<br>
Source: [RA.Utilities.Api.Results](https://github.com/RedonAlla/RA.Utilities.git)

```csharp
public class ErrorResult
```

Represents a result containing error information. Serves as the base for `BadRequestResult`, `NotFoundResult`, and `ConflictResult`.

| Property       | Type     | Description                                   |
| -------------- | -------- | --------------------------------------------- |
| `ErrorCode`    | `string?` | The machine-readable error code.              |
| `ErrorMessage` | `string`  | The human-readable description of the error.   |

---

## Response Type Reference

All response types and their defaults:

| Response Class                | HTTP Code | `ResponseType`          | `Result` Type          |
| ----------------------------- | --------- | ----------------------- | ---------------------- |
| `SuccessResponse<T>`          | 200       | `Success`               | `T`                    |
| `BadRequestResponse`          | 400       | `BadRequest`            | `BadRequestResult[]`   |
| `UnauthorizedResponse`        | 401       | `Unauthorized`          | `ErrorResult`          |
| `ForbiddenResponse`           | 403       | `Forbidden`             | `ErrorResult`          |
| `NotFoundResponse`            | 404       | `NotFound`              | `NotFoundResult`       |
| `ConflictResponse`            | 409       | `Conflict`              | `ConflictResult`       |
| `UnprocessableResponse`       | 422       | `Unprocessable`         | `ErrorResult`          |
| `TooManyRequestsResponse`     | 429       | `TooManyRequests`       | `ErrorResult`          |
| `ErrorResponse`               | 500       | `Error`                 | `ErrorResult`          |
| `ServiceUnavailableResponse`  | 503       | `ServiceUnavailable`    | `ErrorResult`          |
| `GatewayTimeoutResponse`      | 504       | `GatewayTimeout`        | `ErrorResult`          |

---

## Contributing

Contributions are welcome! If you have a suggestion or find a bug, please open an issue to discuss it.

### Pull Request Process

1. **Fork the Repository**: Start by forking the RA.Utilities repository.
2. **Create a Branch**: Create a new branch for your feature or bug fix from the `main` branch. Please use a descriptive name (e.g., `feature/add-new-exception` or `fix/readme-typo`).
3. **Make Your Changes**: Write your code, ensuring it adheres to the existing coding style. Add or update XML documentation for any new public APIs.
4. **Update README**: If you are adding a new exception or changing functionality, please update the `README.md` file accordingly.
5. **Submit a Pull Request**: Push your branch to your fork and open a pull request to the `main` branch of the original repository. Provide a clear description of the changes you have made.

### Coding Standards

- Follow the existing coding style and conventions used in the project.
- Ensure all public members are documented with clear XML comments.
- Keep changes focused. A pull request should address a single feature or bug.

Thank you for contributing!
