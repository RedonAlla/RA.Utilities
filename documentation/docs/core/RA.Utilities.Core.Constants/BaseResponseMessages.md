```bash
Namespace: RA.Utilities.Core.Constants
```

The `BaseResponseMessages` class provides a collection of predefined, read-only string messages for common API responses.
It lives within the `RA.Utilities.Core.Constants` package and is designed to work alongside [`BaseResponseCode`](./BaseResponseCode).

Its primary purpose is to eliminate "magic strings" from your application's response logic. Instead of scattering hard-coded messages like `"The requested resource was not found."` throughout your codebase, you can use a centralized, self-documenting member like `BaseResponseMessages.NotFound`.

This approach offers several significant benefits:

1. **Consistency**: It ensures that the tone and wording of your API's responses are uniform across all endpoints, providing a more professional and predictable experience for clients.
2. **Readability**: Code becomes cleaner and easier to understand. `return BadRequest(BaseResponseMessages.BadRequest)` is more explicit than `return BadRequest("The request is invalid.")`.
3. **Maintainability**: If you need to update the wording of a standard message (for example, for localization or to provide more detail), you only need to change it in one central location.
4. **Reduced Errors**: Using named values prevents typos in string literals, which can be difficult to spot and can lead to inconsistent client-side error handling.

> **Read-only, not `const`:** Since v10.0.4 these fields are `static readonly`, not compile-time constants. Read them directly at runtime, but do not use them where C# requires a constant expression — attribute arguments, default parameter values, `case` labels, or `const` declarations. See the [migration guide](./migration-guides.mdx) for replacements.

## Values

| Field Name              | Message                                                |
|-------------------------|--------------------------------------------------------|
| **Success**             | "Operation completed successfully."                    |
| **Created**             | "Resource created successfully."                       |
| **Updated**             | "Resource updated successfully."                       |
| **Deleted**             | "Resource deleted successfully."                       |
| **Accepted**            | "The request has been accepted for processing."        |
| **NoContent**           | "No content."                                          |
| **BadRequest**          | "The request is invalid."                              |
| **Unauthorized**        | "Authentication failed or is missing."                 |
| **PaymentRequired**     | "Payment is required to access this resource."         |
| **Forbidden**           | "You do not have permission to access this resource."  |
| **NotFound**            | "The requested resource was not found."                |
| **Conflict**            | "A conflict occurred with the current state of the resource." |
| **Unprocessable**       | "Unprocessable entity."                                |
| **TooManyRequests**     | "Too many requests. Please try again later."           |
| **InternalServerError** | "An unexpected error occurred on the server."          |
| **Error**               | "Something happened on our end."                       |
| **ServiceUnavailable**  | "The service is temporarily unavailable. Please try again later." |
| **GatewayTimeout**      | "The server, while acting as a gateway or proxy, did not receive a timely response from the upstream server." |

## 🚀 Usage Examples

Here's how you can use these values within an ASP.NET Core controller to create clean and consistent API endpoints.

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
            // Use the named value for the response message
            // highlight-next-line
            return NotFound(BaseResponseMessages.NotFound);
        }

        return Ok(product);
    }
}
```
