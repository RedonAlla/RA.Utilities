```bash
Namespace: RA.Utilities.Core.Constants
```

The `BaseResponseMessages` class is a static class that provides a collection of predefined, constant string messages for common API responses.
It lives within the `RA.Utilities.Core.Constants` package and is designed to work alongside `BaseResponseCode`.

Its primary purpose is to eliminate "magic strings" from your application's response logic. Instead of scattering hard-coded messages like `"Resource not found."` throughout your codebase, you can use a centralized, self-documenting constant like `BaseResponseMessages.NotFound`.

This approach offers several significant benefits:

1. **Consistency**: It ensures that the tone and wording of your API's responses are uniform across all endpoints, providing a more professional and predictable experience for clients.
2. **Readability**: Code becomes cleaner and easier to understand. `return BadRequest(BaseResponseMessages.BadRequest)` is more explicit than `return BadRequest("One or more validation errors occurred.")`.
3. **Maintainability**: If you need to update the wording of a standard message (for example, for localization or to provide more detail), you only need to change it in one central location.
4. **Reduced Errors**: Using constants prevents typos in string literals, which can be difficult to spot and can lead to inconsistent client-side error handling.

## Constant Values

| Constant Name           | Message                                                |
|-------------------------|--------------------------------------------------------|
| **Success**             | "Operation completed successfully."                    |
| **Created**             | "Resource created successfully."                       |
| **Updated**             | "Resource updated successfully."                       |
| **Deleted**             | "Resource deleted successfully."                       |
| **BadRequest**          | "The request is invalid."                              |
| **Unauthorized**        | "Authentication failed or is missing."                 |
| **Forbidden**           | "You do not have permission to access this resource."  |
| **NotFound**            | "The requested resource was not found."                |
| **Conflict**            | "A conflict occurred with the current state of the resource." |
| **InternalServerError** | "An unexpected error occurred on the server."          |

## 🚀 Usage Examples

Here’s how you can use these constants within an ASP.NET Core controller to create clean and consistent API endpoints.

```csharp showLineNumbers
using Microsoft.AspNetCore.Mvc;
// highlight-next-line
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
            // highlight-next-line
            return NotFound(BaseResponseMessages.NotFound);
        }

        return Ok(product);
    }
}
```