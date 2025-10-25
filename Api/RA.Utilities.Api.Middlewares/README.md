<p align="center">
  <img src="../../Assets/Images/middleware.svg" alt="RA.Utilities.Api.Middlewares Logo" width="128">
</p>

# RA.Utilities.Api.Middlewares

[![NuGet version](https://img.shields.io/nuget/v/RA.Utilities.Api.Middlewares.svg)](https://www.nuget.org/packages/RA.Utilities.Api.Middlewares/)

`RA.Utilities.Api.Middlewares` provides a collection of useful ASP.NET Core middlewares designed to improve diagnostics and enforce API best practices.
This package includes middlewares for efficient HTTP request/response logging and for ensuring the presence of default required headers.

## 🎯 Purpose

This package aims to solve common cross-cutting concerns in API development:

1.  **HTTP Request/Response Logging**: Provides a highly performant middleware to log detailed information about incoming requests and their corresponding responses. It uses `Microsoft.IO.RecyclableMemoryStream` to minimize memory allocations, making it suitable for high-throughput applications.
2.  **Header Enforcement**: Includes a middleware to validate the presence of essential headers, such as `X-Request-Id`, to ensure traceability and consistency across your services.

## 🛠️ Installation

You can install the package via the .NET CLI:

```sh
dotnet add package RA.Utilities.Api.Middlewares
```

Or through the NuGet Package Manager in Visual Studio.

---

## Features

### 1. HTTP Logging Middleware

The `HttpLoggingMiddleware` captures and logs the details of each HTTP request and response, including headers, body, and status codes. It is designed for performance and uses the structured logging models from `RA.Utilities.Logging.Shared`.

#### Usage

**Step 1: Register the middleware services in `Program.cs`**

Call `AddHttpLoggingMiddleware()` in your service configuration. You can also provide options to customize its behavior, such as excluding certain paths from logging.

```csharp
// Program.cs
using RA.Utilities.Api.Middlewares.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add the logging middleware services
builder.Services.AddHttpLoggingMiddleware(options =>
{
    options.ExcludePaths.Add("/swagger");
    options.ExcludePaths.Add("/health");
});

var app = builder.Build();
```

**Step 2: Add the middleware to the pipeline**

Place `app.UseMiddleware<HttpLoggingMiddleware>()` early in your middleware pipeline to ensure it captures the full request/response cycle.

```csharp
// Program.cs (continued)

app.UseMiddleware<HttpLoggingMiddleware>();

app.MapGet("/", () => "Hello World!");

app.Run();
```

### 2. Default Headers Middleware

The `DefaultHeadersMiddleware` enforces the presence of the `X-Request-Id` header on all incoming requests. This is crucial for distributed tracing and correlating logs across multiple services.

**How it works:**

- If the `X-Request-Id` header is present, it is added to the response headers for the client to see.
- If the header is **missing**, the middleware immediately short-circuits the request and returns an **HTTP 400 Bad Request** with a standardized error response. It also generates a new `X-Request-Id` and adds it to the response for traceability.

#### Usage

**Step 1: Register the middleware services in `Program.cs`**

Call `AddDefaultHeadersMiddleware()` in your service configuration.
You can also provide options to customize its behavior, such as excluding certain paths from header validation.

```csharp
// Program.cs
using RA.Utilities.Api.Middlewares.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDefaultHeadersMiddleware(options =>
{
    options.PathsToIgnore.Add("/swagger");
    options.PathsToIgnore.Add("/health");
});

var app = builder.Build();
```

**Step 2: Add the middleware to the pipeline**

Place `app.UseMiddleware<DefaultHeadersMiddleware>()` right after routing to ensure it runs for all API endpoints.

```csharp
// Program.cs (continued)

app.UseRouting();

app.UseMiddleware<DefaultHeadersMiddleware>();

app.MapControllers();

app.Run();
```

A request without the `X-Request-Id` header will receive a response like this:

```json
{
  "responseCode": 400,
  "responseType": "BadRequest",
  "responseMessage": "One or more validation errors occurred.",
  "result": [
    {
      "propertyName": "X-Request-Id",
      "errorMessage": "Header 'X-Request-Id' is required.",
      "attemptedValue": null,
      "errorCode": "NotNullValidator"
    }
  ]
}
```

---

## Contributing

Contributions are welcome! If you have a suggestion or find a bug, please open an issue to discuss it. Please follow the contribution guidelines outlined in the other `RA.Utilities` packages.