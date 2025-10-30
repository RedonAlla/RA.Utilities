<p align="center">
  <img src="../../Assets/Images/authorization.svg" alt="RA.Utilities.Authorization Logo" width="128">
</p>

# RA.Utilities.Authorization

[![NuGet version](https://img.shields.io/nuget/v/RA.Utilities.Authorization.svg)](https://www.nuget.org/packages/RA.Utilities.Authorization/)

`RA.Utilities.Authorization` offers a streamlined approach to handling user authentication and authorization in ASP.NET Core applications. It provides a strongly-typed, injectable service to easily access the current user's claims, such as user ID, name, and roles, directly from the [`HttpContext`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.http.httpcontext). This utility simplifies the process of retrieving authenticated user data, reducing boilerplate code and improving the readability and maintainability of your authorization logic.

## Purpose

In any ASP.NET Core application that requires authentication, you often need to access information about the currently logged-in user. This typically involves injecting [`IHttpContextAccessor`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.http.ihttpcontextaccessor) and manually parsing claims from `HttpContext.User`.

This package abstracts that logic away into a clean, reusable, and testable service, `ICurrentUser`.

The main benefits are:
- **Simplified Access**: No more [`IHttpContextAccessor`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.http.ihttpcontextaccessor) in your controllers and services. Just inject `ICurrentUser`.
- **Strongly-Typed**: Get the user's ID as a `Guid` or `int` without manual parsing and type conversion.
- **Testable**: Easily mock `ICurrentUser` in your unit tests to simulate different authenticated users and scenarios.
- **Reduced Boilerplate**: Drastically cuts down on repetitive code for accessing user claims.

## 🛠️ Installation

You can install the package via the .NET CLI:

```bash
dotnet add package RA.Utilities.Authorization
```

Or through the NuGet Package Manager in Visual Studio.

---

## Usage

Using the library involves two simple steps: registering the service and injecting it where needed.

### Step 1: Register the Service

In your `Program.cs` (or `Startup.cs`), call the `AddCurrentUser()` extension method to register the `ICurrentUser` service and its dependencies with the dependency injection container.

```csharp
// Program.cs

using RA.Utilities.Authorization.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Register the ICurrentUser service
builder.Services.AddCurrentUser();

var app = builder.Build();

// ...

app.Run();
```

### Step 2: Inject and Use `ICurrentUser`

Now you can inject `ICurrentUser` into any of your services or controllers to access the current user's information.

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using RA.Utilities.Authorization.Abstractions;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Ensure the user is authenticated
public class ProfileController : ControllerBase
{
    private readonly ICurrentUser _currentUser;

    public ProfileController(ICurrentUser currentUser)
    {
        _currentUser = currentUser;
    }

    [HttpGet]
    public IActionResult GetUserProfile()
    {
        if (!_currentUser.IsAuthenticated())
        {
            return Unauthorized();
        }

        var userInfo = new
        {
            UserId = _currentUser.GetId<Guid>(), // Get ID as a Guid
            UserName = _currentUser.GetName(),
            Email = _currentUser.GetEmail(),
            IsAdmin = _currentUser.IsInRole("Admin")
        };

        return Ok(userInfo);
    }
}
```

The `ICurrentUser` interface provides several helpful methods:
- `IsAuthenticated()`: Checks if the user is authenticated.
- `GetId<T>()`: Gets the user's ID and converts it to the specified type (e.g., `Guid`, `int`, `string`).
- `GetName()`: Gets the user's name claim.
- `GetEmail()`: Gets the user's email claim.
- `IsInRole(string roleName)`: Checks if the user belongs to a specific role.
- `GetClaimValue(string claimType)`: Gets the value of any specified claim.

---

## API Reference

### `AppUser` Class

| Property | Type | Description |
| -------- | ---- | ----------- |
| `IsAuthenticated` | **bool** | Checks if the user is authenticated. |
| `Id` | **string?** | Gets the user's unique identifier (from `ClaimTypes.NameIdentifier`). |
| `Name` | **string?** | Gets the user's name (from `ClaimTypes.Name`). |
| `Email` | **string?** | Gets the user's email (from `ClaimTypes.Email`). |
| `IsInRole(string roleName)` | **bool** | Checks if the user belongs to a specific role. |
| `GetClaimValue(string claimType)` | **string?** | Gets the value of the first claim of a specific type. |
| `GetClaimValues(string claimType)` | **IEnumerable\<string\>** | Gets all values for a specific claim type. |
| `HasClaim(string claimValue)` | **bool** | Checks if the current user has a specific scope value. Returns `true` if the current user has the specified `claim` value otherwise, `false`. |
| `HasScope(string scopeValue)` | **bool** | Checks if the current user has a specific claim value. Returns `true` if the current user has the specified `scope` value otherwise, `false`. |

## Contributing

Contributions are welcome! If you have a suggestion or find a bug, please open an issue to discuss it. Please follow the contribution guidelines outlined in the other projects in this repository.