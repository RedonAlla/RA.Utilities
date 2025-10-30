<p align="center">
  <img src="../../Assets/Images/api.svg" alt="RA.Utilities.Api Logo" width="128">
</p>

# RA.Utilities.Api

[![NuGet version](https://img.shields.io/nuget/v/RA.Utilities.Api.svg)](https://www.nuget.org/packages/RA.Utilities.Api/)

`RA.Utilities.Api` provides a collection of essential utilities for building robust and consistent ASP.NET Core APIs. This package includes endpoint registration helpers, standardized response models, and exception handling middleware to streamline development and promote best practices.

It acts as the glue between the `RA.Utilities.Core` libraries and your web application, providing concrete implementations for ASP.NET Core.

## 🎯 Purpose

This package is designed to solve two common challenges in API development:

1.  **Consistent Error Handling**: It provides a middleware that automatically catches custom exceptions from `RA.Utilities.Core.Exceptions` (like `NotFoundException`, `ConflictException`) and transforms them into standardized JSON responses using models from `RA.Utilities.Api.Results`.
2.  **Clean Endpoint Organization**: It offers a simple pattern for organizing API endpoints into separate files, keeping your `Program.cs` clean and your routes maintainable as the application grows.

By using this package, you can significantly reduce boilerplate code and enforce consistency across all your API endpoints.

## 🛠️ Installation

You can install the package via the .NET CLI:

```sh
dotnet add package RA.Utilities.Api
```

Or through the NuGet Package Manager in Visual Studio.

---

## ✨ Features

### 1. Global Exception Handling

The `GlobalExceptionHandler` intercepts any unhandled exceptions thrown within your application and generates appropriate, structured error responses. This is the modern .NET 8 approach, replacing older custom middleware.

**How it works:**

- It catches `NotFoundException`, `ConflictException`, and `BadRequestException` from the `RA.Utilities.Core.Exceptions` package.
- It maps them to `NotFoundResponse`, `ConflictResponse`, and `BadRequestResponse` from the `RA.Utilities.Api.Results` package.
- For any other unhandled exception, it returns a generic `ErrorResponse` (HTTP 500) to avoid leaking sensitive information.
- It logs every exception for debugging purposes.

#### Usage

Register the exception handler in your `Program.cs` file.

```csharp
// Program.cs

var builder = WebApplication.CreateBuilder(args);

// 1. Add the exception handler service.
builder.Services.AddRaExceptionHandling();

var app = builder.Build();

// 2. Add the exception handler to the pipeline.
// This should be placed early to catch exceptions from subsequent middleware and endpoints.
app.UseRaExceptionHandling();

app.MapGet("/products/{id}", (int id) =>
{
    if (id <= 0)
    {
        // This will be caught and transformed into a 404 NotFoundResponse
        throw new NotFoundException("Product", id);
    }
    
    return Results.Ok(new { Id = id, Name = "Sample Product" });
});

app.Run();
```

A request to `/products/0` would automatically return a 404 response with the following body:

```json
{
  "responseCode": 404,
  "responseType": "NotFound",
  "responseMessage": "Product with value '0' not found.",
  "result": {
    "entityName": "Product",
    "entityValue": 0
  }
}
```

### 2. Endpoint Registration Helpers

As your API grows, defining all routes in `Program.cs` can become messy. This package provides a clean, discoverable pattern for organizing your endpoints using the `IEndpoint` interface.

**How it works:**

1.  **Create an Endpoint Class**: Create a class that implements the `IEndpoint` interface.
2.  **Define Routes**: Inside the `MapEndpoint` method, define your routes just as you would in `Program.cs`.
3.  **Register Services**: In `Program.cs`, use the `AddEndpoints()` extension method to scan your assembly and register all `IEndpoint` implementations with the DI container.
4.  **Map Endpoints**: After building the app, use the `MapEndpoints()` extension method to execute the route mapping for all registered endpoints.

#### Usage

**Step 1: Create an `IEndpoint` implementation**

```csharp
// Features/Products/ProductEndpoints.cs

using RA.Utilities.Api.Abstractions;

public class ProductEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/products", () =>
        {
            // Logic to get all products
            return Results.Ok("All products");
        });

        app.MapGet("/products/{id}", (int id) => 
        {
            // Logic to get a single product
            return Results.Ok($"Product {id}");
        });
    }
}
```

**Step 2: Register the endpoints in `Program.cs`**

The `MapEndpoints` method scans the specified assembly (or the calling assembly by default) for all types implementing `IEndpoint` and calls their `MapEndpoints` method.
```csharp
// Program.cs

var builder = WebApplication.CreateBuilder(args);

// Scans the assembly and registers all IEndpoint implementations with DI
builder.Services.AddEndpoints();

var app = builder.Build();

// Scans the assembly and registers all IEndpoint implementations
app.MapEndpoints();

app.Run();
```

This keeps your `Program.cs` clean and your endpoint definitions organized by feature.

### 3. Standardized Success Response Helpers

To complement the standardized error responses, this package provides the `SuccessResponse` static class. It offers convenient helper methods for creating consistent success `IResult` objects (like `200 OK` and `201 Created`) that are automatically wrapped in the `SuccessResponse<T>` model from `RA.Utilities.Api.Results`.

This ensures that your successful API responses follow the same structured format as your error responses, providing a predictable contract for your API consumers.

#### Available Helpers
The SuccessResponse class provides helpers for the most common success status codes:

* `Ok()`: Creates a `200 OK` response.
* `Ok<T>(T result)`: Creates a `200 OK` response with a payload.
* `Created()`: Creates a `201 Created` response.
* `Created<T>(T result)`: Creates a `201 Created` response with a payload.
* `Created<T>(string uri, T result)`: Creates a `201 Created` response with a `Location` header and a payload.
* `Accepted()`: Creates a `202 Accepted` response.
* `Accepted<T>(string uri, T result)`: Creates a `202 Accepted` response with a `Location` header and a payload.
* `NoContent()`: Creates a `204 No Content` response.

#### Usage

You can use these helpers directly in your endpoints to simplify response creation and ensure consistency.

```csharp
// In an IEndpoint implementation or Minimal API
app.MapGet("/products/{id}", (int id) => 
{
    var product = new Product { Id = id, Name = "Sample Product" };

    // Instead of this:
    // return Results.Ok(new SuccessResponse<Product>(product));

    // You can write this:
    return SuccessResponse.Ok(product);
});

app.MapPost("/products", (Product product) => 
{
    // Logic to save the product...
    var newProduct = new Product { Id = 123, Name = product.Name };

    // Creates a 201 Created response with a Location header and a structured body
    return SuccessResponse.Created($"/products/{newProduct.Id}", newProduct);
});
```

A request that returns a product would yield the following JSON body, wrapped in the standard response structure:

```json
{
  "responseCode": 200,
  "responseType": "Success",
  "responseMessage": "Operation completed successfully.",
  "result": {
    "id": 1,
    "name": "Sample Product"
  }
}
```

### 4. Using the `Result` Type with Endpoints

While the exception middleware is great for handling unexpected errors, the `Result` type from `RA.Utilities.Core` is perfect for handling expected business-level failures (e.g., validation errors, resource not found) without throwing exceptions.

You can combine the `IEndpoint` pattern with the `Result` type to create robust and highly readable API endpoints. The `Match` method from the `Result` type is used to transform the success or failure outcome into a standard ASP.NET Core `IResult`.

#### Usage

**Step 1: Create a service that returns a `Result`**

Your application or domain layer should return a `Result<T>` to indicate the outcome of an operation.

```csharp
// Services/ProductService.cs
using RA.Utilities.Core;
using RA.Utilities.Core.Exceptions;

public class ProductService
{
    public Result<Product> GetProductById(int id)
    {
        if (id <= 0)
        {
            // Return a failure Result for invalid input
            return new BadRequestException("Invalid product ID.");
        }

        if (id != 1)
        {
            // Return a failure Result if the product is not found
            return new NotFoundException(nameof(Product), id);
        }

        // Return a success Result with the product
        return new Product { Id = id, Name = "Sample Product" };
    }
}
```

**Step 2: Use `Match` in your endpoint to return an `IResult`**

In your `IEndpoint` implementation, call the service and use the `Match` method to map the `Result` to an HTTP response. This pattern forces you to handle both success and failure cases explicitly.

```csharp
// Features/Products/ProductEndpoints.cs

public class ProductEndpoints : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/products/{id}", (int id, ProductService service) => 
        {
            Result<Product> result = service.GetProductById(id);
            
            // Match the result to an appropriate HTTP response
            return result.Match(SuccessResponse.Ok, ErrorResultResponse.Result);
        }).WithTags("Products");
    }
}
```

---

## Contributing

Contributions are welcome! If you have a suggestion or find a bug, please open an issue to discuss it.

### Pull Request Process

1.  **Fork the Repository**: Start by forking the RA.Utilities repository.
2.  **Create a Branch**: Create a new branch for your feature or bug fix from the `main` branch. Please use a descriptive name (e.g., `feature/add-auth-helper` or `fix/middleware-bug`).
3.  **Make Your Changes**: Write your code, ensuring it adheres to the existing coding style. Add or update XML documentation for any new public APIs.
4.  **Update README**: If you are adding new functionality, please update the `README.md` file accordingly.
5.  **Submit a Pull Request**: Push your branch to your fork and open a pull request to the `main` branch of the original repository. Provide a clear description of the changes you have made.

### Coding Standards

- Follow the existing coding style and conventions used in the project.
- Ensure all public members are documented with clear XML comments.
- Keep changes focused. A pull request should address a single feature or bug.

Thank you for contributing!
