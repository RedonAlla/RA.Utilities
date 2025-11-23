# RA.Utilities.Api.Middlewares

[![NuGet version](https://img.shields.io/nuget/v/RA.Utilities.Api.Middlewares.svg)](https://www.nuget.org/packages/RA.Utilities.Api.Middlewares/)
[![Codecov](https://codecov.io/github/RedonAlla/RA.Utilities/graph/badge.svg)](https://codecov.io/github/RedonAlla/RA.Utilities)
[![NuGet Downloads](https://img.shields.io/nuget/dt/RA.Utilities.Api.Middlewares.svg)](https://www.nuget.org/packages/RA.Utilities.Api.Middlewares/)
[![Documentation](https://img.shields.io/badge/Documentation-read-brightgreen.svg?logo=readthedocs&logoColor=fff)](https://redonalla.github.io/RA.Utilities/nuget-packages/api/RA.Utilities.Api.Middlewares/)
[![GitHub license](https://img.shields.io/github/license/RedonAlla/RA.Utilities?logo=googledocs&logoColor=fff)](https://github.com/RedonAlla/RA.Utilities?tab=MIT-1-ov-file)

`RA.Utilities.Api.Middlewares` provides a collection of useful ASP.NET Core middlewares to solve common cross-cutting concerns. It includes a high-performance middleware for logging HTTP requests/responses and another for enforcing the presence of required headers like `X-Request-Id` to ensure traceability.

## 📚 Table of Contents

- Getting started
- Dependencies
- Features
  - HTTP Logging Middleware
  - Default Headers Middleware
- Additional documentation
- Contributing
- License

---

## Getting started

Install the package via the .NET CLI:

```bash
dotnet add package RA.Utilities.Api.Middlewares
```

Or through the NuGet Package Manager in Visual Studio.

---

## 🔗 Dependencies

- [`RA.Utilities.Core.Constants`](https://redonalla.github.io/RA.Utilities/nuget-packages/core/RA.Utilities.Core.Constants/)
- [`RA.Utilities.Logging.Shared`](https://redonalla.github.io/RA.Utilities/nuget-packages/Logging/RA.Utilities.Logging.Shared/)
- [`RA.Utilities.Api.Results`](https://redonalla.github.io/RA.Utilities/nuget-packages/api/RA.Utilities.Api.Results/)
- [`Microsoft.AspNetCore.App`](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/metapackage-app)
- [`Microsoft.IO.RecyclableMemoryStream`](https://www.nuget.org/packages/Microsoft.IO.RecyclableMemoryStream)

---

## Features

### 1. HTTP Logging Middleware

The [`HttpLoggingMiddleware`] captures and logs the details of each HTTP request and response, including headers, body, and status codes. It is designed for performance and uses the structured logging models from `RA.Utilities.Logging.Shared`.

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

## Additional documentation

For more information on how this package fits into the larger RA.Utilities ecosystem, please see the main repository [documentation](https://redonalla.github.io/RA.Utilities/nuget-packages/api/RA.Utilities.Api.Middlewares/).

---

## Contributing

Contributions are welcome! If you have a suggestion or find a bug, please open an issue to discuss it first.

### Pull Request Process

1.  **Fork the Repository**: Start by forking the RA.Utilities repository.
2.  **Create a Branch**: Create a new branch for your feature or bug fix from the `main` branch. Please use a descriptive name (e.g., `feature/add-auth-helper` or `fix/middleware-bug`).
3.  **Make Your Changes**: Write your code, ensuring it adheres to the project's coding style.
4.  **Add Tests**: Add or update unit tests for your changes to ensure correctness and prevent regressions.
5.  **Update Documentation**: Add or update XML documentation for any new public APIs. If you are adding new functionality, please update the relevant `README.md` file.
6.  **Verify Locally**: Ensure the solution builds and all tests pass locally before submitting.
7.  **Submit a Pull Request**: Push your branch to your fork and open a pull request to the `main` branch of the original repository. Provide a clear description of the changes you have made.

### Coding Standards

- **Style**: Follow the coding conventions defined in the `.editorconfig` file at the root of the repository. The build is configured to enforce these styles.
- **Documentation**: Ensure all public members are documented with clear XML comments.
- **Commit Messages**: Consider using Conventional Commit messages (e.g., `feat:`, `fix:`, `docs:`) to keep the commit history clean and informative.
- **Scope**: Keep changes focused. A pull request should address a single feature or bug.

Thank you for contributing!

---

## 📜 License

This project is licensed under the MIT License. See the [LICENSE](https://github.com/RedonAlla/RA.Utilities?tab=MIT-1-ov-file) file for details.