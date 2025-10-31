---
title: RA.Utilities.Api.Results
authors: [RedonAlla]
---

## Version 10.0.0-rc.2
[![NuGet version](https://img.shields.io/badge/NuGet-10.0.0--rc.2-orange?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Api/10.0.0-rc.2)

This release aligns the `RA.Utilities.Api.Results` package with the latest `rc.2` versions of the RA.Utilities ecosystem.
The focus is on ensuring the standardized response models are robust and well-documented to support the new features in dependent packages like `RA.Utilities.Api` and `RA.Utilities.Api.Middlewares`.

<!-- truncate -->

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