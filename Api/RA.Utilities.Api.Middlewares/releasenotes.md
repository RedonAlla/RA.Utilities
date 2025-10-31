# RA.Utilities.Api.Middlewares Release Notes

## Version 10.0.100-rc.2

This release modernizes the middleware package, aligning it with the latest patterns and dependencies in the RA.Utilities ecosystem. The focus is on performance, consistency, and ease of use.

### ✨ New Features & Improvements

*   **`HttpLoggingMiddleware`**:
    *   Provides high-performance HTTP request/response logging suitable for production environments.
    *   Uses `Microsoft.IO.RecyclableMemoryStream` to minimize memory allocations and GC pressure.
    *   Integrates with `RA.Utilities.Logging.Shared` to produce structured logs, making them easy to query and analyze.
    *   Includes configurable options to exclude specific paths (e.g., `/swagger`, `/health`) from logging.

*   **`DefaultHeadersMiddleware`**:
    *   Enforces the presence of required headers, such as `X-Request-Id`, to ensure traceability in distributed systems.
    *   Automatically returns a standardized `400 Bad Request` response using models from `RA.Utilities.Api.Results` if a required header is missing.
    *   Includes configurable options to ignore header validation for specific paths.

*   **Simplified Registration**:
    *   Introduced extension methods (`AddHttpLoggingMiddleware`, `AddDefaultHeadersMiddleware`) for clean and simple registration in `Program.cs`.

*   **Updated Documentation**:
    *   The `README.md` has been updated to reflect the latest usage patterns and best practices, with clear code examples for .NET 8.

### 🚀 Getting Started

Register the middlewares in your `Program.cs`:

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpLoggingMiddleware();
builder.Services.AddDefaultHeadersMiddleware();

var app = builder.Build();

app.UseMiddleware<HttpLoggingMiddleware>();
app.UseMiddleware<DefaultHeadersMiddleware>();
```
