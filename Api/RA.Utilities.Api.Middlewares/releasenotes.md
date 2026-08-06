# RA.Utilities.Api.Middlewares Release Notes

## Version 10.0.2
![Date Badge](https://img.shields.io/badge/Publish-06%20August%202026-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-v10.0.2-blue?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Api.Middlewares/10.0.2)

Maintenance release with response model refactoring, log template improvements, and cancellation token support.

### 📝 Improvements

* **Response Model Refactoring**: `DefaultHeadersMiddleware` now constructs `BadRequestResponse` directly using object-initializer syntax instead of building a `BadRequestResult` array.
* **Log Template Enhancement**: `HttpLoggingMiddleware` now includes the `RequestId` in both request and response log templates.
* **Cancellation Token Support**: `WriteAsync` and `CopyToAsync` calls now pass `context.RequestAborted`.
* **Typo Fix**: Fixed `Schema` → `Scheme` property name in the request log template.

### ⚠️ Deprecation Notice

This package is deprecated and superseded by [`RA.Utilities.Api`](https://www.nuget.org/packages/RA.Utilities.Api/). All middleware types have been consolidated there with expanded features. See the [migration guide](https://redonalla.github.io/RA.Utilities/nuget-packages/api/RA.Utilities.Api/Middlewares/) for details.

## Version 10.0.0
![Date Badge](https://img.shields.io/badge/Publish-24%20November%202025-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-v10.0.0-blue?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Api.Middlewares/10.0.0)

Updated the project from version `10.0.100-rc.2` to `10.0.0`, marking the transition from release candidate to stable release.


## Version 10.0.0-rc.2
![Date Badge](https://img.shields.io/badge/Publish-18%20Octomber%202025-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-10.0.0--rc.2-orange?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Api.Middlewares/10.0.0-rc.2)

This release modernizes the middleware package, aligning it with the latest patterns and dependencies in the RA.Utilities ecosystem.
The focus is on performance, consistency, and ease of use.

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
