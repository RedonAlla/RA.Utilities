---
title: RA.Utilities.Api.Middlewares
authors: [RedonAlla]
---

## Version 10.0.10-rc.2
[![NuGet version](https://img.shields.io/badge/NuGet-10.0.10--rc.2-orange?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Api.Middlewares/10.0.10-rc.2)

This release focuses on significantly improving the documentation and clarifying the purpose and usage of the middlewares provided in this package.
The goal is to make it easier for developers to implement robust logging and header validation in their APIs.

<!-- truncate -->

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
