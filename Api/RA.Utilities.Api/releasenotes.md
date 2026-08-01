# RA.Utilities.Api Release Notes

## Version 10.0.6
![Date Badge](https://img.shields.io/badge/Publish-02%20August%202026-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-v10.0.6-blue?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Api/10.0.6)

### ✨ New Features & Improvements

*   **New Exception Mappings**: `ErrorResultMapper` and `ErrorResultResponse` now handle `TooManyRequestsException` (429), `ServiceUnavailableException` (503), and `GatewayTimeoutException` (504) with their dedicated response types.

*   **Extension Methods Added**: `AddRaExceptionHandling()` and `UseRaExceptionHandling()` are now available as convenience extension methods on `IServiceCollection` and `IApplicationBuilder`, matching the documented API.

*   **Improved Logging**: `GlobalExceptionHandler` now logs client errors (4xx) at `Warning` level and server errors (5xx) at `Error` level, reducing log noise from expected client behavior.

*   **Consistency & Polish**:
    *   Renamed static helper class `SuccessResponse` → `SuccessResult` to resolve the naming collision with `SuccessResponse<T>` from `RA.Utilities.Api.Results`.
    *   Removed unnecessary `where TResult : new()` constraints on `SuccessResult` helper methods.
    *   `MapEndpoints` now returns `WebApplication` instead of `IApplicationBuilder` for better fluent chaining.
    *   Error dispatcher now uses `BaseResponseCode` constants instead of hardcoded `StatusCodes` literals.
    *   Removed stale TODO comments and `<remarks>` blocks from `EndpointExtensions`.

*   **Refined Documentation**: Updated `README.md` to reflect all API changes, including corrected method signatures and new response type mappings.


## Version 10.0.5
![Date Badge](https://img.shields.io/badge/Publish-25%20April%202026-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-v10.0.5-blue?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Api/10.0.5)

### ✨ Improvements & Fixes

*   **Dynamic Error Mapping**: Updated `ErrorResultResponse` to utilize the `ResponseCode` directly from `RaBaseException`. This allows domain-level exceptions to dictate the HTTP status code returned by the API.
*   **Observability**: Enhanced the global exception fallback to log unhandled exception messages and stack traces to the error console, aiding in production debugging.
*   **Clarity**: Added internal documentation to the central error dispatcher to explain the pattern-matching logic.


## Version 10.0.2
![Date Badge](https://img.shields.io/badge/Publish-04%20Januaryr%202026-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-v10.0.2-blue?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Api/10.0.2)

Add `CreatedAtRoute` methods to `SuccessResult` for enhanced API response handling.


## Version 10.0.1
![Date Badge](https://img.shields.io/badge/Publish-17%20December%202025-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-v10.0.1-blue?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Api/10.0.1)

### ✨ New Features: FluentValidation Integration for Minimal APIs
This release introduces a comprehensive integration with FluentValidation for ASP.NET Core Minimal APIs. You can now effortlessly validate incoming requests and automatically reflect those validation rules in your OpenAPI (Swagger) documentation. This creates a single source of truth for your validation logic, improving maintainability and providing a better experience for your API consumers.

#### 1. Automatic Request Validation

A new endpoint filter has been introduced that automatically validates API request models. By simply adding `.Validate<TModel>()` to your endpoint definition, you can ensure that all incoming data is valid before your handler logic is executed. If validation fails, a standardized `400 Bad Request` response is returned with detailed error information.

#### How to use it:
Chain the `.Validate<TModel>()` extension method to your endpoint registration.

```csharp
// In your endpoint definition
app.MapPost("/users", (CreateUserRequest user) => {
    // Handler logic here...
    return SuccessResult.Created(user);
})
.Validate<CreateUserRequest>(); // This enables automatic validation
```

This leverages the `ValidationEndpointFilter<TModel>`, which resolves your `IValidator<TModel>` implementations from the dependency injection container and executes them.

---

## Version 10.0.0
![Date Badge](https://img.shields.io/badge/Publish-23%20November%202025-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-v10.0.0-blue?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Api/10.0.0)

Changed project version from a release candidate to final version `10.0.0` for production readiness.

Revised XML documentation comments to improve clarity and detail.
Improved the documentation in `IEndpoint` to clarify its purpose in grouping related API endpoints.
Adjusted parameter and return type descriptions in `EndpointExtensions` for better understanding of default assembly behavior.
Enhanced comments in SuccessResult to explicitly state response wrapping behavior.

## Version 10.0.100-rc.2
![Date Badge](https://img.shields.io/badge/Publish-18%20Octomber%202025-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-10.0.0--rc.2-orange?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Api/10.0.0-rc.2)

This release modernizes the `RA.Utilities.Api` package, introducing a suite of tools to build robust, consistent, and maintainable ASP.NET Core APIs. Key features include a .NET 8 global exception handler, helpers for standardized success responses, and a clean pattern for endpoint registration.

### ✨ New Features & Improvements

*   **Global Exception Handling (`AddRaExceptionHandling`)**:
    *   Introduced a .NET 8 `IExceptionHandler` implementation that automatically catches exceptions and transforms them into standardized JSON error responses.
    *   Catches semantic exceptions from `RA.Utilities.Core.Exceptions` (e.g., `NotFoundException`, `ConflictException`) and maps them to the correct HTTP status codes (404, 409, etc.).
    *   Handles any unhandled exceptions by returning a generic 500 Internal Server Error to prevent leaking sensitive information.

*   **Endpoint Registration Helpers (`AddEndpoints` & `MapEndpoints`)**:
    *   Provides a clean pattern for organizing API endpoints into separate files using the `IEndpoint` interface.
    *   Keeps `Program.cs` clean and maintainable by automatically discovering and registering all endpoint implementations in your project.

*   **Standardized Success Response Helpers (`SuccessResult`)**:
    *   Added a new static `SuccessResult` class with helper methods (`Ok`, `Created`, `NoContent`, etc.).
    *   These helpers simplify the creation of successful API responses (e.g., `Ok`, `Created`, `Accepted`, `NoContent`) and automatically wrap the payload in the standard `SuccessResponse<T>` model, ensuring consistency with error responses.

*   **Seamless `Result<T>` Integration**:
    *   The `SuccessResult` helpers and the exception handling middleware work together to provide a clean way to handle the `Result<T>` type from `RA.Utilities.Core`.
    *   Use the `Match` method on a `Result` to map success outcomes to `SuccessResult.Ok()` and failure outcomes to `ErrorResultResponse.Result()`.

*   **Comprehensive Documentation**:
    *   The `README.md` has been completely rewritten to provide clear, step-by-step instructions and usage examples for all major features.

### 🚀 Getting Started

Register the services in your `Program.cs`:

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRaExceptionHandling();
builder.Services.AddEndpoints(typeof(Program).Assembly);

var app = builder.Build();

app.UseRaExceptionHandling();
app.MapEndpoints();
```