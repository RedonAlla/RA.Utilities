<p align="center">
  <img src="../../Assets/Images/constant.svg" alt="RA.Utilities.Integrations Logo" width="128">
</p>

# RA.Utilities.Core.Constants

[![NuGet version](https://img.shields.io/nuget/v/RA.Utilities.Core.Constants.svg)](https://www.nuget.org/packages/RA.Utilities.Core.Constants/)

A centralized and consistent set of core constants for the RA Utilities ecosystem. This package helps streamline development, improve code readability, and reduce "magic strings" and "magic numbers" by providing a single source of truth for common values.

## 🎯 Purpose

The primary goal of this package is to ensure consistency across different projects and services. By using these predefined constants, you can:

- **Avoid Typos**: Prevent hard-to-find bugs caused by typos in strings or numbers.
- **Improve Readability**: Make your code more self-documenting (e.g., `HttpStatusCodes.NotFound` is clearer than `404`).
- **Simplify Maintenance**: Update a constant in one place, and the change propagates everywhere it's used.

## 📦 Installation

You can install the package via the .NET CLI:

```sh
dotnet add package RA.Utilities.Core.Constants
```

Or through the NuGet Package Manager in Visual Studio.

---

## ✨ Available Constants

The package currently provides the following static classes with constants:

### `HttpStatusCodes`

Contains integer constants for common HTTP status codes, aligning with standard web practices.

| Constant Name       | Value | Category           |
|---------------------|-------|--------------------|
| `Ok`                | 200   | Success (2xx)      |
| `Created`           | 201   | Success (2xx)      |
| `NoContent`         | 204   | Success (2xx)      |
| `BadRequest`        | 400   | Client Error (4xx) |
| `Unauthorized`      | 401   | Client Error (4xx) |
| `Forbidden`         | 403   | Client Error (4xx) |
| `NotFound`          | 404   | Client Error (4xx) |
| `Conflict`          | 409   | Client Error (4xx) |
| `InternalServerError` | 500   | Server Error (5xx) |
| `ServiceUnavailable`  | 503   | Server Error (5xx) |

### `ResponseMessages`

Contains default string messages for common API responses. This helps maintain a consistent tone and messaging for your API consumers.

| Constant Name       | Message                                                | Category |
|---------------------|--------------------------------------------------------|----------|
| `Success`           | "Operation completed successfully."                    | Success  |
| `Created`           | "Resource created successfully."                       | Success  |
| `Updated`           | "Resource updated successfully."                       | Success  |
| `Deleted`           | "Resource deleted successfully."                       | Success  |
| `BadRequest`        | "The request is invalid."                              | Error    |
| `Unauthorized`      | "Authentication failed or is missing."                 | Error    |
| `Forbidden`         | "You do not have permission to access this resource."  | Error    |
| `NotFound`          | "The requested resource was not found."                | Error    |
| `Conflict`          | "A conflict occurred with the current state of the resource." | Error    |
| `InternalServerError` | "An unexpected error occurred on the server."          | Error    |

---

## 🚀 Usage Examples

Here’s how you can use these constants within an ASP.NET Core controller to create clean and consistent API endpoints.

```csharp
using Microsoft.AspNetCore.Mvc;
using RA.Utilities.Core.Constants; // Import the constants namespace

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
            return NotFound(ResponseMessages.NotFound);
        }

        return Ok(product);
    }

    [HttpPost]
    public IActionResult CreateProduct([FromBody] Product newProduct)
    {
        if (!ModelState.IsValid)
        {
            // Use constants for a bad request
            return BadRequest(ResponseMessages.BadRequest);
        }

        var createdProduct = _productService.Create(newProduct);

        // Use constants for a 'Created' response
        return StatusCode(HttpStatusCodes.Created, createdProduct);
    }
}
```

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