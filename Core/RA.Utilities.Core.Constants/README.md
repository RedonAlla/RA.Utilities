# RA.Utilities.Core.Constants

[![NuGet version](https://img.shields.io/nuget/v/RA.Utilities.Core.Constants.svg?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Core.Constants/)
[![Codecov](https://codecov.io/github/RedonAlla/RA.Utilities/graph/badge.svg)](https://codecov.io/github/RedonAlla/RA.Utilities)
[![GitHub license](https://img.shields.io/github/license/RedonAlla/RA.Utilities?logo=googledocs&logoColor=fff)](https://github.com/RedonAlla/RA.Utilities?tab=MIT-1-ov-file)
[![NuGet Downloads](https://img.shields.io/nuget/dt/RA.Utilities.Core.Constants.svg?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Core.Constants/)
[![Documentation](https://img.shields.io/badge/Documentation-read-brightgreen.svg?logo=readthedocs&logoColor=fff)](https://redonalla.github.io/RA.Utilities/nuget-packages/core/RA.Utilities.Core.Constants/)

A centralized and consistent set of core constants for the RA Utilities ecosystem. This package helps streamline development, improve code readability, and reduce "magic strings" and "magic numbers" by providing a single source of truth for common values.

## Getting started

You can install the package via the .NET CLI:

```bash
dotnet add package RA.Utilities.Core.Constants
```

Or through the NuGet Package Manager in Visual Studio.

---

## ✨ Available Constants

The package currently provides the following static classes:

### `BaseResponseCode`

Contains integer constants for common HTTP status codes, aligning with standard web practices. This class was previously named `HttpStatusCodes`.

| Constant Name         | Value | Category           |
|-----------------------|-------|--------------------|
| `Success`             | 200   | Success (2xx)      |
| `Created`             | 201   | Success (2xx)      |
| `Accepted`            | 202   | Success (2xx)      |
| `NoContent`           | 204   | Success (2xx)      |
| `BadRequest`          | 400   | Client Error (4xx) |
| `Unauthorized`        | 401   | Client Error (4xx) |
| `Forbidden`           | 403   | Client Error (4xx) |
| `NotFound`            | 404   | Client Error (4xx) |
| `Conflict`            | 409   | Client Error (4xx) |
| `Unprocessable`       | 422   | Client Error (4xx) |
| `TooManyRequests`     | 429   | Client Error (4xx) |
| `InternalServerError` | 500   | Server Error (5xx) |
| `ServiceUnavailable`  | 503   | Server Error (5xx) |
| `GatewayTimeout`      | 504   | Server Error (5xx) |


### `BaseResponseMessages`

Contains default string messages for common API responses. This helps maintain a consistent tone and messaging for your API consumers.

| Constant Name       | Message                                                | Category |
|---------------------|--------------------------------------------------------|----------|
| `Success`           | "Operation completed successfully."                    | Success  |
| `Created`           | "Resource created successfully."                       | Success  |
| `Updated`           | "Resource updated successfully."                       | Success  |
| `Deleted`           | "Resource deleted successfully."                       | Success  |
| `Accepted`          | "The request has been accepted for processing."        | Success  |
| `NoContent`         | "No content."                                          | Success  |
| `BadRequest`        | "The request is invalid."                              | Error    |
| `Unauthorized`      | "Authentication failed or is missing."                 | Error    |
| `Forbidden`         | "You do not have permission to access this resource."  | Error    |
| `NotFound`          | "The requested resource was not found."                | Error    |
| `Conflict`          | "A conflict occurred with the current state of the resource." | Error    |
| `Unprocessable`     | "Unprocessable entity." | Error    |
| `TooManyRequests`   | "Too many requests. Please try again later."           | Error    |
| `Error`               | "Something happened on our end."                       | Error    |
| `InternalServerError` | "An unexpected error occurred on the server."          | Error    |
| `ServiceUnavailable`  | "The service is temporarily unavailable. Please try again later." | Error    |
| `GatewayTimeout`      | "The server, while acting as a gateway or proxy, did not receive a timely response from the upstream server." | Error    |

### `HeaderParameters`

Contains constant strings for common HTTP header names, ensuring consistency when accessing or setting headers.

| Constant Name   | Value            | Description                                                              |
|-----------------|------------------|--------------------------------------------------------------------------|
| `XRequestId`    | `"x-request-id"` | Used for request correlation and tracing.                                |
| `TraceId`       | `"trace-id"`     | Used for internal tracing.                                               |
| `Location`      | `"location"`     | Used in responses to redirect or indicate the location of a new resource.|
| `Authorization` | `"Authorization"`| Used for sending authentication credentials.                             |

### `ResponseType` (record)

The `ResponseType` is an implementation of the **type-safe enum pattern** using a `record` instead of a traditional C# `enum`. Its primary purpose is to create a strongly-typed, extensible, and descriptive vocabulary for the outcomes of your API operations. Unlike a traditional enum, `ResponseType` can be extended by consuming projects via inheritance.

| Field              | Description                                                                     | Typical HTTP Status |
|--------------------|---------------------------------------------------------------------------------|---------------------|
| `Success`          | The operation was successful.                                                   | 200 OK              |
| `Created`          | A resource was successfully created.                                            | 201 Created         |
| `Updated`          | A resource was successfully updated.                                            | 200 OK              |
| `Deleted`          | A resource was successfully deleted.                                            | 200 OK              |
| `NoContent`        | The request succeeded with no content to return.                                | 204 No Content      |
| `Accepted`         | The request was accepted for processing but is not yet complete.                | 202 Accepted        |
| `Validation`       | The request failed due to invalid input data.                                   | 400 Bad Request     |
| `Problem`          | An unexpected problem occurred, often used for RFC 7807 problem details.        | 500 Internal Server |
| `NotFound`         | The requested resource was not found.                                           | 404 Not Found       |
| `Conflict`         | The request conflicts with the current state of the resource.                   | 409 Conflict        |
| `Unauthorized`     | The request requires user authentication.                                       | 401 Unauthorized    |
| `Error`            | A general, non-specific error occurred during the operation.                    | 500 Internal Server |
| `BadRequest`       | The request was malformed or could not be processed for reasons other than validation. | 400 Bad Request     |
| `Unprocessable`    | The request was semantically incorrect and could not be processed.              | 422 Unprocessable   |
| `Forbidden`        | The server understood the request but refuses to authorize it.                  | 403 Forbidden       |
| `TooManyRequests`  | The client has sent too many requests in a given amount of time.                | 429 Too Many Requests |
| `ServiceUnavailable`| The service is temporarily unavailable.                                         | 503 Service Unavailable |
| `GatewayTimeout`   | The server, acting as a gateway, did not receive a timely response.             | 504 Gateway Timeout |

#### Example JSON Response

```json
{
  "responseCode": 404,
  "responseType": "NotFound", // From the ResponseType record
  "responseMessage": "Product with value '99' not found."
}
```
---

## 🚀 Usage Examples

Here’s how you can use these constants within an ASP.NET Core controller to create clean and consistent API endpoints.

```csharp
using Microsoft.AspNetCore.Mvc;
using RA.Utilities.Core.Constants; // Import the constants

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    [HttpGet("{id}")]
    public IActionResult GetProduct(int id)
    {
        var product = _productService.GetById(id);

        if (product == null)
        {
            // Use constants for both the status code and the response message
            return NotFound(BaseResponseMessages.NotFound);
        }

        return Ok(product);
    }

    [HttpPost]
    public IActionResult CreateProduct([FromBody] Product newProduct)
    {
        if (!ModelState.IsValid)
        {
            // Use constants for a bad request
            return BadRequest(BaseResponseMessages.BadRequest);
        }

        var createdProduct = _productService.Create(newProduct);

        // Use constants for a 'Created' response
        return StatusCode(BaseResponseCode.Created, createdProduct);
    }
}
```

## Additional documentation

For more information on how this package fits into the larger RA.Utilities ecosystem, please see the main repository [documentation](https://redonalla.github.io/RA.Utilities/nuget-packages/core/RA.Utilities.Core.Constants/).

## Contributing

Contributions are welcome! If you have a suggestion or find a bug, please open an issue to discuss it.

### Pull Request Process

1.  **Fork the Repository**: Start by forking the RA.Utilities repository.
2.  **Create a Branch**: Create a new branch for your feature or bug fix from the `main` branch. Please use a descriptive name (e.g., `feature/add-result-extensions` or `fix/readme-typo`).
3.  **Make Your Changes**: Write your code, ensuring it adheres to the existing coding style. Add or update XML documentation for any new public APIs.
4.  **Update README**: If you are adding new functionality, please update the `README.md` file accordingly.
5.  **Submit a Pull Request**: Push your branch to your fork and open a pull request to the `main` branch of the original repository. Provide a clear description of the changes you have made.

### Coding Standards

- Follow the existing coding style and conventions used in the project.
- Ensure all public members are documented with clear XML comments.
- Keep changes focused. A pull request should address a single feature or bug.

Thank you for contributing!
```
