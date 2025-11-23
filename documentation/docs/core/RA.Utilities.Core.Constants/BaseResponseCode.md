```bash
Namespace: RA.Utilities.Core.Constants
```

The `BaseResponseCode` class is a static class within the `RA.Utilities.Core.Constants` package.
Its primary purpose is to provide a centralized, single source of truth for common HTTP status codes used throughout your application ecosystem.

By using this class, you can replace "magic numbers" (like `404`, `500`, etc.) in your code with named, self-documenting constants (e.g., `BaseResponseCode.NotFound`, `BaseResponseCode.InternalServerError`).

This practice offers several key advantages:

1. **Improved Readability**: Code becomes much easier to understand. `StatusCode(BaseResponseCode.NotFound)` is more explicit and descriptive than `StatusCode(404)`.
2. **Consistency**: It ensures that all developers and services in your ecosystem use the same integer values for the same meanings, preventing inconsistencies.
3. **Reduced Errors**: It eliminates the risk of typos when writing status codes, which can lead to hard-to-find bugs.
4. **Simplified Maintenance**: While HTTP status codes rarely change, having them centralized means that if you ever needed to adjust a custom code, you would only need to do it in one place.

In short, `BaseResponseCode` is a simple but effective tool for writing cleaner, more maintainable, and more robust API code.

## Constant Values

| Constant Name           | Value | Category           |
|-------------------------|-------|--------------------|
| **Success**             | 200   | Default response code for success. |
| **BadRequest**          | 400   | Default response code for a bad request. |
| **Unauthorized**        | 401   | CDefault response code for an unauthorized request. |
| **Forbidden**           | 403   | Default response code for a forbidden request. |
| **NotFound**            | 404   | Default response code for a resource not found. |
| **Conflict**            | 409   | Default response code for a conflict request. |
| **InternalServerError** | 500   | Default response code for an internal server error. |
| **ServiceUnavailable**  | 503   | Server Error (5xx) |