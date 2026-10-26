```bash
Namespace: RA.Utilities.Core.Constants
```

The `BaseResponseCode` class provides a centralized, single source of truth for common HTTP status codes used throughout your application ecosystem.

Its fields are `public static readonly int` values, so you can replace "magic numbers" (like `404`, `500`, etc.) with named, self-documenting members (e.g., `BaseResponseCode.NotFound`, `BaseResponseCode.InternalServerError`).

This practice offers several key advantages:

1. **Improved Readability**: Code becomes much easier to understand. `StatusCode(BaseResponseCode.NotFound)` is more explicit and descriptive than `StatusCode(404)`.
2. **Consistency**: It ensures that all developers and services in your ecosystem use the same integer values for the same meanings, preventing inconsistencies.
3. **Reduced Errors**: It eliminates the risk of typos when writing status codes, which can lead to hard-to-find bugs.
4. **Simplified Maintenance**: While HTTP status codes rarely change, having them centralized means that if you ever needed to adjust a custom code, you would only need to do it in one place.

> **Read-only, not `const`:** Since v10.0.4 these fields are `static readonly`, not compile-time constants. Read them directly at runtime, but do not use them where C# requires a constant expression — attribute arguments, default parameter values, `case` labels, or `const` declarations. See the [migration guide](./migration-guides.mdx) for replacements.

## Values

| Field Name              | Value | Meaning                                        |
|-------------------------|-------|------------------------------------------------|
| **Success**             | 200   | The request succeeded.                         |
| **Created**             | 201   | A resource was created.                        |
| **Accepted**            | 202   | The request was accepted for processing.       |
| **NoContent**           | 204   | The request succeeded with no content to return. |
| **BadRequest**          | 400   | The request is invalid.                        |
| **Unauthorized**        | 401   | Authentication is missing or failed.           |
| **PaymentRequired**     | 402   | Payment is required to access the resource.    |
| **Forbidden**           | 403   | The caller lacks permission.                   |
| **NotFound**            | 404   | The requested resource was not found.          |
| **Conflict**            | 409   | The request conflicts with the current state.  |
| **Unprocessable**       | 422   | The request is semantically incorrect.         |
| **TooManyRequests**     | 429   | The client exceeded a rate limit.              |
| **InternalServerError** | 500   | An unexpected server error occurred.           |
| **ServiceUnavailable**  | 503   | The service is temporarily unavailable.        |
| **GatewayTimeout**      | 504   | An upstream gateway did not respond in time.   |

## 🚀 Usage Example

```csharp showLineNumbers
using Microsoft.AspNetCore.Mvc;
// highlight-next-line
using RA.Utilities.Core.Constants;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    [HttpGet("{id}")]
    public IActionResult GetProduct(int id)
    {
        var product = _productService.GetById(id);

        if (product is null)
        {
            // Use the named status code and the matching message
            // highlight-next-line
            return StatusCode(BaseResponseCode.NotFound, BaseResponseMessages.NotFound);
        }

        return Ok(product);
    }
}
```

`BaseResponseCode.Created` (`201`) is available for responses you build manually. ASP.NET Core's `CreatedAtAction` and `Created` helpers already set `201`, so you only need the field when constructing a response yourself.
