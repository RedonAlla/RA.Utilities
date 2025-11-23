# RA.Utilities.Api.Results Release Notes

## Version 10.0.0
![Date Badge](https://img.shields.io/badge/Publish-23%20November%202025-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-v10.0.0-blue?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Api.Results/10.0.0)

Changed project version from a `10.0.0-rc.2` release candidate to final version `10.0.0` for production readiness.

## Version 10.0.0-rc.2

![Date Badge](https://img.shields.io/badge/Publish-18%20Octomber%202025-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-10.0.0--rc.2-orange?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Api.Results/10.0.0-rc.2)

This release aligns the `RA.Utilities.Api.Results` package with the latest `rc.2` versions of the RA.Utilities ecosystem. The focus is on ensuring the standardized response models are robust and well-documented to support the new features in dependent packages like `RA.Utilities.Api` and `RA.Utilities.Api.Middlewares`.

### ✨ New Features & Improvements

*   **Standardized Response Models**: Provides a consistent and predictable structure for all API responses, including:
    *   `SuccessResponse<T>`: For successful operations (2xx).
    *   `BadRequestResponse`: For validation failures (400).
    *   `NotFoundResponse`: For missing resources (404).
    *   `ConflictResponse`: For state conflicts (409).
    *   `ErrorResponse`: For unexpected server errors (500).

*   **Ecosystem Integration**: These models are the foundation for the standardized error handling in `RA.Utilities.Api`'s `GlobalExceptionHandler` and the success response helpers in `SuccessResponse`.

*   **Comprehensive Documentation**: The `README.md` has been updated to provide clear C# usage examples and JSON response samples for each model, making it easy for both backend and frontend developers to understand the API contract.

### 🚀 Getting Started

Use these models in your ASP.NET Core controllers or Minimal APIs to create consistent responses.

```csharp
app.MapGet("/products/{id}", (int id) =>
{
    var product = GetProductFromDb(id);
    return product is not null
        ? SuccessResponse.Ok(product) // Using helper from RA.Utilities.Api
        : new NotFoundResponse(nameof(Product), id);
});
```