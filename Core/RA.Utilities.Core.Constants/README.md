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

The package currently provides the following constant holders. Since v10.0.4, every value is a `public static readonly` field or an enum member — **not** a compile-time `const`. Ordinary reads are unchanged, but the values cannot be used in attributes, default parameter values, `case` labels, or `const` declarations.

### `BaseResponseCode`

Contains `public static readonly int` fields for common HTTP status codes, aligning with standard web practices. This class was previously named `HttpStatusCodes`.

| Field Name            | Value | Category           |
|-----------------------|-------|--------------------|
| `Success`             | 200   | Success (2xx)      |
| `Created`             | 201   | Success (2xx)      |
| `Accepted`            | 202   | Success (2xx)      |
| `NoContent`           | 204   | Success (2xx)      |
| `BadRequest`          | 400   | Client Error (4xx) |
| `Unauthorized`        | 401   | Client Error (4xx) |
| `PaymentRequired`     | 402   | Client Error (4xx) |
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

| Field Name          | Message                                                | Category |
|---------------------|--------------------------------------------------------|----------|
| `Success`           | "Operation completed successfully."                    | Success  |
| `Created`           | "Resource created successfully."                       | Success  |
| `Updated`           | "Resource updated successfully."                       | Success  |
| `Deleted`           | "Resource deleted successfully."                       | Success  |
| `Accepted`          | "The request has been accepted for processing."        | Success  |
| `NoContent`         | "No content."                                          | Success  |
| `BadRequest`        | "The request is invalid."                              | Error    |
| `Unauthorized`      | "Authentication failed or is missing."                 | Error    |
| `PaymentRequired`   | "Payment is required to access this resource."         | Error    |
| `Forbidden`         | "You do not have permission to access this resource."  | Error    |
| `NotFound`          | "The requested resource was not found."                | Error    |
| `Conflict`          | "A conflict occurred with the current state of the resource." | Error    |
| `Unprocessable`     | "Unprocessable entity." | Error    |
| `TooManyRequests`   | "Too many requests. Please try again later."           | Error    |
| `Error`               | "Something happened on our end."                       | Error    |
| `InternalServerError` | "An unexpected error occurred on the server."          | Error    |
| `ServiceUnavailable`  | "The service is temporarily unavailable. Please try again later." | Error    |
| `GatewayTimeout`      | "The server, while acting as a gateway or proxy, did not receive a timely response from the upstream server." | Error    |

### `BaseErrorCode`

Contains machine-readable error codes (`static readonly string`) for field-level validation failures. While `BaseResponseCode` covers transport-level concerns, `BaseErrorCode` describes *why* a request was rejected. The codes are used by the value objects in `RA.Utilities.Core.ValueObjects` (`Email`, `Currency`, `SSN`, `Money`, `IBAN`) to populate structured `ValidationError` entries.

| Field Name         | Value              | Triggered When                                            |
|--------------------|--------------------|-----------------------------------------------------------|
| `EmailRequired`    | `REQUIRED_EMAIL`   | The 'Email address' field is missing.                     |
| `EmailMaxLength`   | `EMAIL_MAX_LENGTH` | The 'Email address' field exceeds the maximum length.     |
| `EmailNotValid`    | `INVALID_EMAIL`    | The 'Email address' format is invalid.                    |
| `CurrencyRequired` | `REQUIRED_CURRENCY`| The 'Currency' field is missing.                          |
| `CurrencyLength`   | `CURRENCY_LENGTH`  | The 'Currency' field is not exactly 3 characters.         |
| `CurrencyMismatch` | `CURRENCY_MISMATCH`| Two currency values that must match are different.        |
| `PositiveMoney`    | `POSITIVE_MONEY`   | The 'Amount' value is negative (less than 0).             |
| `SsnRequired`      | `REQUIRED_SSN`     | The 'SSN' field is missing.                                |
| `SsnLength`        | `SSN_LENGTH`       | The 'SSN' field is not exactly 10 characters.              |
| `SsnNotValid`      | `NOT_VALID_SSN`    | The 'SSN' format is invalid.                               |
| `IbanRequired`     | `REQUIRED_IBAN`    | The 'IBAN' field is missing.                               |
| `IbanLength`       | `IBAN_LENGTH`      | The 'IBAN' total length does not match its country's expected length. |
| `IbanNotValid`     | `NOT_VALID_IBAN`   | The 'IBAN' format or structure is invalid.                 |
| `IbanCheckDigits`  | `IBAN_CHECK_DIGITS`| The 'IBAN' check digits fail the ISO 7064 mod-97 validation. |

### `BaseErrorMessage`

Contains the human-readable messages (`static readonly string`) matching the codes in `BaseErrorCode`. The code identifies the failure programmatically, while the message explains it to humans.

| Field Name           | Message                                    |
|----------------------|--------------------------------------------|
| `EmailRequired`      | "The 'Email address' field is required."   |
| `EmailNotValid`      | "'Email address' format is invalid."       |
| `CurrencyRequired`   | "The 'Currency' field is required."        |
| `CurrencyLength`     | "'Currency' must be exact 3 characters."   |
| `CurrencyMismatch`   | "Currency mismatch"                        |
| `PositiveMoney`      | "POSITIVE_MONEY"                           |
| `SsnRequired`        | "The 'SSN' field is required."             |
| `SsnLength`          | "'SSN' must be exact 10 characters."       |
| `SsnNotValid`        | "'SSN' format is invalid."                 |
| `IbanRequired`       | "The 'IBAN' field is required."            |
| `IbanLength`         | "'IBAN' length does not match the expected length for its country." |
| `IbanNotValid`       | "'IBAN' format is invalid."                |
| `IbanCheckDigits`    | "'IBAN' check digits are invalid."         |

> **Note:** There is no `EmailMaxLength` message field — the maximum-length message is built dynamically by the `Email` value object because it interpolates the configured limit into the text.

> **IBAN note:** The `IBAN` value object performs structural validation (a 13–34 character pattern plus the ISO 7064 mod-97 check), so malformed or wrongly sized values use `IbanNotValid`. `IbanLength` is provided for consumers that validate a country-specific length themselves.

### `HeaderParameters`

Contains read-only string fields for common HTTP header names, ensuring consistency when accessing or setting headers.

| Field Name      | Value            | Description                                                              |
|-----------------|------------------|--------------------------------------------------------------------------|
| `XRequestId`    | `"x-request-id"` | Used for request correlation and tracing.                                |
| `TraceId`       | `"trace-id"`     | Used for internal tracing.                                               |
| `Location`      | `"location"`     | Used in responses to redirect or indicate the location of a new resource.|
| `Authorization` | `"Authorization"`| Used for sending authentication credentials.                             |

### `ResponseType` (enum)

The `ResponseType` is a C# `enum` that provides a strongly-typed, descriptive vocabulary for the outcomes of your API operations. It has no built-in JSON converter, so with default `System.Text.Json` options it is serialized as its numeric member value (e.g., `NotFound` → `8`). Register `JsonStringEnumConverter` to emit names such as `"NotFound"`; the converter matches names case-insensitively and throws for unknown names.

| Member             | Value | Description                                                                     | Typical HTTP Status |
|--------------------|-------|---------------------------------------------------------------------------------|---------------------|
| `Success`          | 0     | The operation was successful.                                                   | 200 OK              |
| `Created`          | 1     | A resource was successfully created.                                            | 201 Created         |
| `Updated`          | 2     | A resource was successfully updated.                                            | 200 OK              |
| `Deleted`          | 3     | A resource was successfully deleted.                                            | 200 OK              |
| `NoContent`        | 4     | The request succeeded with no content to return.                                | 204 No Content      |
| `Accepted`         | 5     | The request was accepted for processing but is not yet complete.                | 202 Accepted        |
| `Validation`       | 6     | The request failed due to invalid input data.                                   | 400 Bad Request     |
| `Problem`          | 7     | An unexpected problem occurred, often used for RFC 7807 problem details.        | 500 Internal Server |
| `NotFound`         | 8     | The requested resource was not found.                                           | 404 Not Found       |
| `Conflict`         | 9     | The request conflicts with the current state of the resource.                   | 409 Conflict        |
| `Unauthorized`     | 10    | The request requires user authentication.                                       | 401 Unauthorized    |
| `Error`            | 11    | A general, non-specific error occurred during the operation.                    | 500 Internal Server |
| `BadRequest`       | 12    | The request was malformed or could not be processed for reasons other than validation. | 400 Bad Request     |
| `Unprocessable`    | 13    | The request was semantically incorrect and could not be processed.              | 422 Unprocessable   |
| `Forbidden`        | 14    | The server understood the request but refuses to authorize it.                  | 403 Forbidden       |
| `TooManyRequests`  | 15    | The client has sent too many requests in a given amount of time.                | 429 Too Many Requests |
| `ServiceUnavailable` | 16   | The service is temporarily unavailable.                                         | 503 Service Unavailable |
| `GatewayTimeout`   | 17    | The server, acting as a gateway, did not receive a timely response.             | 504 Gateway Timeout |
| `PaymentRequired`  | 18    | Payment is required to access the resource.                                     | 402 Payment Required |

#### Example JSON Response

With `JsonStringEnumConverter` registered:

```json
{
  "responseCode": 404,
  "responseType": "NotFound", // From the ResponseType enum
  "responseMessage": "Product with value '99' not found."
}
```

Without the converter, `responseType` is the numeric value (`8` for `NotFound`).
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
            // Use named values for both the status code and the response message
            return NotFound(BaseResponseMessages.NotFound);
        }

        return Ok(product);
    }

    [HttpPost]
    public IActionResult CreateProduct([FromBody] Product newProduct)
    {
        if (!ModelState.IsValid)
        {
            // Use the named value for a bad request
            return BadRequest(BaseResponseMessages.BadRequest);
        }

        var createdProduct = _productService.Create(newProduct);

        // Use the named value for a 'Created' response
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
