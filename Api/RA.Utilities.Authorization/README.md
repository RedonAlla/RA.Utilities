# RA.Utilities.Authorization

[![NuGet version](https://img.shields.io/nuget/v/RA.Utilities.Authorization.svg)](https://www.nuget.org/packages/RA.Utilities.Authorization/)
[![Codecov](https://codecov.io/github/RedonAlla/RA.Utilities/graph/badge.svg)](https://codecov.io/github/RedonAlla/RA.Utilities)
[![GitHub license](https://img.shields.io/github/license/RedonAlla/RA.Utilities)](https://github.com/RedonAlla/RA.Utilities/blob/main/LICENSE)
[![NuGet Downloads](https://img.shields.io/nuget/dt/RA.Utilities.Authorization.svg)](https://www.nuget.org/packages/RA.Utilities.Authorization/)

A utility library to simplify permission-based authorization in ASP.NET Core applications. This package provides a clean and flexible way to define and enforce authorization policies based on permissions associated with a user's identity.

It solves the problem of hardcoding roles and policies directly in your application code. Instead of scattering `[Authorize(Roles = "Admin")]` attributes, you can define granular permissions and check for them dynamically. This makes your authorization logic more maintainable, testable, and easier to manage as your application grows.

## Getting started

Install the package via the .NET CLI:

```bash
dotnet add package RA.Utilities.Authorization
```

Or through the NuGet Package Manager in Visual Studio.

## 🔗 Dependencies

-   [`Microsoft.AspNetCore.Http`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.http)
-   [`Microsoft.Extensions.Options.ConfigurationExtensions`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection)


### Prerequisites

This package is designed to work on top of an existing authentication setup. It expects that the user's identity (`ClaimsPrincipal`) has been populated with claims. It works seamlessly with `RA.Utilities.Authentication.JwtBearer`, which helps configure JWT-based authentication.

Your JWTs should contain a "permissions" claim that holds the permissions assigned to the user.

**Example JWT Payload:**
```json
{
  "sub": "12345",
  "name": "John Doe",
  "permissions": [
    "products:read",
    "products:create"
  ]
}
```

## Usage

The core of this package is the `AddPermissionAuthorization()` extension method and the `HasPermission` attribute.

### 1. Define Your Permissions

It's good practice to define your permissions as constants to avoid magic strings.

```csharp
// Permissions/ProductPermissions.cs

public static class ProductPermissions
{
    public const string Read = "products:read";
    public const string Create = "products:create";
    public const string Update = "products:update";
    public const string Delete = "products:delete";
}
```

### 2. Register Authorization Services

In your `Program.cs`, call the `AddPermissionAuthorization()` extension method to register the necessary services and configure the authorization policies.

```csharp
// Program.cs

using RA.Utilities.Authorization.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Assumes you have authentication configured, e.g., using RA.Utilities.Authentication.JwtBearer
// builder.Services.AddJwtBearerAuthentication(builder.Configuration);

// 1. Add permission-based authorization services.
builder.Services.AddPermissionAuthorization();

builder.Services.AddControllers();

var app = builder.Build();

// 2. Add authentication and authorization middleware.
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
```

### 3. Protect Your Endpoints

Use the `[HasPermission]` attribute on your controller actions or minimal API endpoints to enforce permission checks.

#### Controller-based Example

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using RA.Utilities.Authorization.Attributes;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Ensures the user is authenticated before checking permissions
public class ProductsController : ControllerBase
{
    [HttpGet]
    [HasPermission(ProductPermissions.Read)]
    public IActionResult GetProducts()
    {
        return Ok("List of all products.");
    }

    [HttpPost]
    [HasPermission(ProductPermissions.Create)]
    public IActionResult CreateProduct()
    {
        return Created("api/products/1", "Product created.");
    }

    [HttpDelete("{id}")]
    [HasPermission(ProductPermissions.Delete)]
    public IActionResult DeleteProduct(int id)
    {
        return NoContent();
    }
}
```

#### Minimal API Example

You can also use the attribute with minimal APIs.

```csharp
using RA.Utilities.Authorization.Attributes;

app.MapGet("/dashboard", () => "Sensitive dashboard data.")
   .RequireAuthorization() // Ensures the user is authenticated
   .WithMetadata(new HasPermissionAttribute("dashboard:view"));
```

If a user tries to access an endpoint without the required permission, the middleware will automatically return an **HTTP 403 Forbidden** response.

## Additional documentation

For more information on how this package fits into the larger RA.Utilities ecosystem, please see the main repository [documentation](http://redonalla.github.io/RA.Utilities/nuget-packages/auth/Authorization/).

- To learn about setting up JWT authentication, see the `RA.Utilities.Authentication.JwtBearer` package documentation.
- For details on standardized API responses, refer to the `RA.Utilities.Api.Results` package.

## Feedback

Contributions are welcome! If you have a suggestion, find a bug, or want to provide feedback, please open an issue in the RA.Utilities [GitHub repository](https://github.com/RedonAlla/RA.Utilities).

### Pull Request Process

1.  **Fork the Repository**: Start by forking the RA.Utilities repository.
2.  **Create a Branch**: Create a new branch for your feature or bug fix from the `main` branch. Please use a descriptive name (e.g., `feature/add-policy-provider` or `fix/permission-claim-type`).
3.  **Make Your Changes**: Write your code, ensuring it adheres to the existing coding style. Add or update XML documentation for any new public APIs.
4.  **Update README**: If you are adding new functionality, please update the `README.md` file accordingly.
5.  **Submit a Pull Request**: Push your branch to your fork and open a pull request to the `main` branch of the original repository. Provide a clear description of the changes you have made.

### Coding Standards

- Follow the existing coding style and conventions used in the project.
- Ensure all public members are documented with clear XML comments.
- Keep changes focused. A pull request should address a single feature or bug.

Thank you for contributing!