```bash
Namespace: RA.Utilities.Core.Constants
```

The `HeaderParameters` class is a static class that acts as a central repository for common HTTP header names used throughout your services.
It is part of the `RA.Utilities.Core.Constants` package.

Its purpose is to eliminate "magic strings" from your code. Instead of hard-coding header names like `"x-request-id"` or `"Authorization"` directly in your middleware, API endpoints, or HTTP clients, you use a named constant like `HeaderParameters.XRequestId`.

This simple practice provides several key benefits:

1. **Readability**: The code becomes more self-documenting and easier to understand. `Request.Headers[HeaderParameters.XRequestId]` is much clearer than `Request.Headers["x-request-id"]`.
2. **Consistency**: It guarantees that every part of your application ecosystem uses the exact same string for a given header, preventing subtle inconsistencies.
3. **Maintainability**: If you ever need to change a header name (e.g., for compliance with a new standard), you only need to update it in this one central file.
4. **Reduced Errors**: It prevents typos in string literals, which can lead to bugs that are often difficult to track down.

This class is used by other parts of the `RA.Utilities` library, such as the HeadersParameterTransformer in the `RA.Utilities.OpenApi` package to document headers, and by middleware to read correlation IDs from incoming requests.

## Constant Values

| Constant Name     | Value            | Description                                                              |
|-------------------|------------------|--------------------------------------------------------------------------|
| **XRequestId**    | `"x-request-id"` | Used for request correlation and tracing.                                |
| **TraceId**       | `"trace-id"`     | Used for internal tracing, often associated with a distributed trace.    |
| **Location**      | `"location"`     | Used in `201 Created` responses to point to the new resource's URL.       |
| **Authorization** | `"Authorization"`| Used for sending authentication credentials (e.g., a Bearer token).      |

```csharp showLineNumbers
using Microsoft.AspNetCore.Mvc;
// highlight-next-line
using RA.Utilities.Core.Constants;

[ApiController]
[Route("api/[controller]")]
public class ExampleController : ControllerBase
{
    [HttpGet("{id}")]
    public IActionResult GetProduct(int id)
    {
        return Ok(product);
    }

    [HttpGet("whoami")]
    public IActionResult GetUserFromHeader()
    {
        // Use a constant to safely access a header value
        // highlight-next-line
        if (Request.Headers.TryGetValue(HeaderParameters.Authorization, out var authHeader))
        {
            // Logic to parse the token...
            return Ok($"Authorization header found: {authHeader}");
        }

        // highlight-next-line
        return Unauthorized(BaseResponseMessages.Unauthorized);
    }
}
```