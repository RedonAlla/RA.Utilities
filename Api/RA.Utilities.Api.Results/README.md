# RA.Utilities.Api.Results

[![NuGet version](https://img.shields.io/nuget/v/RA.Utilities.Api.Results.svg)](https://www.nuget.org/packages/RA.Utilities.Api.Results/)
[![Codecov](https://codecov.io/github/RedonAlla/RA.Utilities/graph/badge.svg)](https://codecov.io/github/RedonAlla/RA.Utilities)
[![GitHub license](https://img.shields.io/github/license/RedonAlla/RA.Utilities)](https://github.com/RedonAlla/RA.Utilities/blob/main/LICENSE)
[![NuGet Downloads](https://img.shields.io/nuget/dt/RA.Utilities.Api.Results.svg)](https://www.nuget.org/packages/RA.Utilities.Api.Results/)

`RA.Utilities.Api.Results` provides a standardized set of models for creating consistent API responses within the RA.Utilities ecosystem.
This package includes generic wrappers for success, error, and validation failure scenarios, helping to streamline API development and improve client-side handling.

## Purpose

When building APIs, it's crucial to have a consistent and predictable response structure.
This package provides a set of ready-to-use C# records that can be returned from your ASP.NET Core controller actions.
These models ensure that every response—whether it's a success, a generic error, or a detailed validation failure—has the same shape, making it easier for clients to parse and handle.

This library is designed to work seamlessly with the other `RA.Utilities` packages, such as`RA.Utilities.Core.Constants` for status codes and messages.

## 🛠️ Installation

You can install the package via the .NET CLI:

```bash
dotnet add package RA.Utilities.Api.Results
```

Or through the NuGet Package Manager in Visual Studio.

## 🔗 Dependencies

-   [`RA.Utilities.Core.Constants`](https://redonalla.github.io/RA.Utilities/nuget-packages/core/RA.Utilities.Core.Constants/)


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
            var validationError = new BadRequestResult("Id", "Product ID must be a positive number.", id);
            return BadRequest(new BadRequestResponse(new[] { validationError }));
        }

        // Simulate finding a product
        var product = GetProductFromDatabase(id);

        if (product == null)
        {
            // Return a 404 Not Found
            return NotFound(new NotFoundResponse("Product", id));
        }

        // Return a 200 OK with the product data
        return Ok(new SuccessResponse<Product>(product));
    }
}
```

## Available Result Models

The core of the package is the `Response<T>` record, which serves as a universal wrapper for all API responses.

### `Response<T>`

Namespace: RA.Utilities.Api.Results</br>
Package: RA.Utilities.Api.Results v1.0.0</br>
Source: [RA.Utilities.Api.Results](https://github.com/RedonAlla/RA.Utilities.git)

```csharp
public class Response<T>
```

#### Properties 

| Property        | Type           | Required | Description                                                 |
| --------------- | -------------- | -------- | ----------------------------------------------------------- |
| `ResponseCode`  | `int`          | **true** | A code for the response, often mapping to HTTP status codes. |
| `ResponseType`  | `ResponseType` | **true** | An enum indicating the type of response (e.g., Success, Error). |
| `ResponseMessage` | `string?`    | **false**| A human-friendly message describing the response.             |
| `Result`        | `T?`           | **false**| The actual data payload of the response.                    |


### `SuccessResponse<T>`

Namespace: RA.Utilities.Api.Results</br>
Package: RA.Utilities.Api.Results v1.0.0</br>
Source: [RA.Utilities.Api.Results](https://github.com/RedonAlla/RA.Utilities.git)

```csharp
public sealed class SuccessResponse<T> : Response<T> where T : new()
```

Creates a `Response<T>` object that produces a `Success` response, typically corresponding to an HTTP 200 OK status.

#### Properties 

| Property        | Value                         |
| --------------- | ----------------------------- |
| `ResponseCode`    | `200` (from `BaseResponseCode.Success`) |
| `ResponseMessage` | `"OK"` (default from `BaseResponseMessages.Success`) |
| `ResponseType`  | `ResponseType.Success`        |


```json
{
  "responseCode": 200,
  "responseType": "Success", // Enum value for Success
  "responseMessage": "Operation completed successfully.",
  "result": {
    // The 'T' payload goes here
  }
}
```

### `BadRequestResponse`

Namespace: RA.Utilities.Api.Results
Package: RA.Utilities.Api.Results
Source: [RA.Utilities.Api.Results](https://github.com/RedonAlla/RA.Utilities.git)

```csharp
public sealed class BadRequestResponse : Response<BadRequestResult[]>
```

Creates a `Response<BadRequestResult>` object for a single validation error, producing a `BadRequest` response (HTTP 400).

It inherits from the generic Response<T> class, specifically as Response<BadRequestResult[]>. This means the Result property of the response will contain an array of BadRequestResult objects, where each object details a specific validation failure.

It automatically sets the ResponseType to ResponseType.BadRequest and defaults the ResponseCode to 400 (from BaseResponseCode.BadRequest), ensuring consistency across all bad request responses in your application.

#### Properties 

| Property | Value |
| --------------- | ------------------------------------------------ |
| ResponseCode | 400 (from BaseResponseCode.BadRequest) |
| ResponseMessage | "One or more validation errors occurred." (from BaseResponseMessages.BadRequest) |
| ResponseType | ResponseType.BadRequest |
| Result | array of [`BadRequestResult`](#badrequestresult) |

#### BadRequestResult 

| Property | Type | Value |
| -------- | --- | ------------------------------------------------ |
| PropertyName | `string` | The name of the property. |
| ErrorMessage | `string` | The error message |
| AttemptedValue | `object` | The property value that caused the failure. |
| ErrorCode | `string` | Gets or sets the error code. |

####  Example response:

```json
{
  "responseCode": 400,
  "responseType": "BadRequest", // Enum value for BadRequest
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

### ConflictResponse 
Namespace: RA.Utilities.Api.Results</br>
Package: RA.Utilities.Api.Results v1.0.0</br>
Source: RA.Utilities.Api.Results


```csharp
public sealed class ConflictResponse : Response<ConflictResult> 
```
Creates a `Response<ConflictResult>` object that produces a Conflict response, typically corresponding to an HTTP 409 Conflict status.
This is used when an action cannot be completed because it conflicts with the current state of a resource, such as trying to create an entity that already exists.

It inherits from `Response<T>`, specifically as `Response<ConflictResult>`.
The Result property will contain a ConflictResult object detailing the nature of the conflict.
It automatically sets `ResponseType` to `ResponseType.Conflict` and defaults the ResponseCode to 409 (from `BaseResponseCode.Conflict`).

#### Properties
| Property | Value |
| --------------- | ------------------------------------------------------------------ |
| ResponseCode | 409 (from BaseResponseCode.Conflict) |
| ResponseMessage | A generated message like "{Entity} with value '{Value}' already exists." |
| ResponseType | ResponseType.Conflict |
| Result | [`ConflictResult`](#conflictresult) |

#### ConflictResult
| Property | Type | Description |
| -------- | -------- | ---------------------------------------------- |
| Entity | string | The name of the entity causing the conflict. |
| Value | object | The value of the entity that caused the conflict. |

#### Example response:
This response indicates that a User with the email existing@example.com already exists.

```json {
  "responseCode": 409,
  "responseType": "Conflict",
  "responseMessage": "User with value 'existing@example.com' already exists.",
  "result": {
    "entity": "User",
    "value": "existing@example.com"
  }
}
```

### `ErrorResponse`

Namespace: RA.Utilities.Api.Results</br>
Package: RA.Utilities.Api.Results v1.0.0</br>
Source: RA.Utilities.Api.Results

```csharp
public sealed class ErrorResponse : Response<object>
```

Creates a `Response<object>` that produces a generic `Error` response, typically corresponding to an HTTP 500 Internal Server Error. This response type is used for unexpected server-side errors where a more specific response (like `BadRequest` or `NotFound`) is not appropriate.

It inherits from `Response<T>`, specifically as `Response<object>`. The `Result` property is `null` for this response type.

It automatically sets `ResponseType` to `ResponseType.Error` and defaults the `ResponseCode` to 500 (from `BaseResponseCode.InternalServerError`).

#### Properties

| Property        | Value                                                              |
| --------------- | ------------------------------------------------------------------ |
| `ResponseCode`    | `500` (from `BaseResponseCode.InternalServerError`) or other error code. |
| `ResponseMessage` | A generic error message like `"An unexpected error occurred on the server."` |
| `ResponseType`  | `ResponseType.Error`                                               |
| `Result`        | `null`                                                             |

#### Example response:

This response indicates a generic server error.

```json
{
  "responseCode": 500,
  "responseType": "Error",
  "responseMessage": "An unexpected error occurred on the server.",
  "result": null
}
```

### NotFoundResponse
Namespace: RA.Utilities.Api.Results</br>
Package: RA.Utilities.Api.Results v1.0.0</br>
Source: RA.Utilities.Api.Results

```csharp
public sealed class NotFoundResponse : Response<NotFoundResult>
```

Creates a `Response<NotFoundResult>` object that produces a NotFound response, typically corresponding to an HTTP 404 Not Found status.
This is used when a requested resource could not be found at the specified URI.

It inherits from `Response<T>`, specifically as `Response<NotFoundResult>`.
The Result property will contain a `NotFoundResult` object detailing which entity was not found. 
It automatically sets `ResponseType` to `ResponseType.NotFound` and defaults the ResponseCode to 404 (from `BaseResponseCode.NotFound`).

#### Properties
| Property | Value |
| --------------- | ------------------------------------------------------------------ |
| ResponseCode | 404 (from BaseResponseCode.NotFound) |
| ResponseMessage | A generated message like "{EntityName} with value '{EntityValue}' not found." or a custom message. |
| ResponseType | ResponseType.NotFound |
| Result | [`NotFoundResult`] |

#### NotFoundResult
| Property | Type | Description |
| ------------- | -------- | ------------------------------------------------------------------ |
| EntityName | `string` | The name of the entity that was not found (e.g., "Product"). |
| EntityValue | `object` | The identifier or value used to search for the entity (e.g., 123). |

#### Example response:
This response indicates that a Product with an ID of 999 could not be found.

```json {
  "responseCode": 404,
  "responseType": "NotFound",
  "responseMessage": "Product with value '999' not found.",
  "result": {
  "entityName": "Product",
  "entityValue": 999
  } 
}
```

---

## Contributing

Contributions are welcome! If you have a suggestion or find a bug, please open an issue to discuss it.

### Pull Request Process

1.  **Fork the Repository**: Start by forking the RA.Utilities repository.
2.  **Create a Branch**: Create a new branch for your feature or bug fix from the `main` branch. Please use a descriptive name (e.g., `feature/add-new-exception` or `fix/readme-typo`).
3.  **Make Your Changes**: Write your code, ensuring it adheres to the existing coding style. Add or update XML documentation for any new public APIs.
4.  **Update README**: If you are adding a new exception or changing functionality, please update the `README.md` file accordingly.
5.  **Submit a Pull Request**: Push your branch to your fork and open a pull request to the `main` branch of the original repository. Provide a clear description of the changes you have made.

### Coding Standards

- Follow the existing coding style and conventions used in the project.
- Ensure all public members are documented with clear XML comments.
- Keep changes focused. A pull request should address a single feature or bug.

Thank you for contributing!
