# RA.Utilities.Authorization

[![NuGet version](https://img.shields.io/nuget/v/RA.Utilities.Authorization.svg?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Authorization/)
[![Codecov](https://codecov.io/github/RedonAlla/RA.Utilities/graph/badge.svg)](https://codecov.io/github/RedonAlla/RA.Utilities)
[![NuGet Downloads](https://img.shields.io/nuget/dt/RA.Utilities.Authorization.svg?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Authorization/)
[![Documentation](https://img.shields.io/badge/Documentation-read-brightgreen.svg?logo=readthedocs&logoColor=fff)](https://redonalla.github.io/RA.Utilities/nuget-packages/auth/Authorization/)
[![GitHub license](https://img.shields.io/github/license/RedonAlla/RA.Utilities?logo=googledocs&logoColor=fff)](https://github.com/RedonAlla/RA.Utilities?tab=MIT-1-ov-file)

`RA.Utilities.Authorization` provides a strongly-typed, injectable `AppUser` service that wraps `HttpContext.User` so you never hand-parse claims, role-check with magic strings, or inject `IHttpContextAccessor` into your business logic. One line in `Program.cs` and you have typed access to user identity everywhere.

## Getting started

Install the package via the .NET CLI:

```bash
dotnet add package RA.Utilities.Authorization
```

Or through the NuGet Package Manager in Visual Studio.

## 🔗 Dependencies

-   [`Microsoft.AspNetCore.Http`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.http)

### Prerequisites

This package works on top of an existing authentication setup. It expects that the user's identity (`ClaimsPrincipal`) has been populated by authentication middleware. It pairs seamlessly with `RA.Utilities.Authentication.JwtBearer`.

## Usage

### 1. Register the Service

In your `Program.cs`, call `AddAppUser()` to register `AppUser` and its required `IHttpContextAccessor`:

```csharp
using RA.Utilities.Authorization.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Assumes you have authentication configured, e.g., using RA.Utilities.Authentication.JwtBearer
// builder.Services.AddJwtBearerAuthentication(builder.Configuration);

builder.Services.AddAppUser();
```

### 2. Inject and Use AppUser

Inject `AppUser` into controllers, Minimal API endpoints, or services:

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using RA.Utilities.Authorization;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly AppUser _user;

    public ProfileController(AppUser user) => _user = user;

    [HttpGet]
    public IActionResult GetProfile()
    {
        if (!_user.IsAuthenticated)
            return Unauthorized();

        return Ok(new
        {
            UserId = _user.UserId,
            Name = _user.Name,
            Email = _user.Email,
            IsAdmin = _user.IsInRole("Admin")
        });
    }
}
```

#### Minimal API Example

```csharp
app.MapGet("/me", (AppUser user) => new
{
    user.UserId,
    user.Name,
    user.Email
}).RequireAuthorization();
```

## API Reference

### AppUser

| Member | Type | Description |
|---|---|---|
| **IsAuthenticated** | `bool` | Whether the current user is authenticated. |
| **Id** | `string?` | The user's unique identifier from the NameIdentifier claim, or null. |
| **UserId** | `Guid` | The user's ID as a `Guid`. Throws `InvalidOperationException` if not authenticated or not a valid Guid. |
| **Name** | `string?` | The user's name from the Name claim, or null. |
| **Email** | `string?` | The user's email from the Email claim, or null. |
| **IsInRole(string)** | `bool` | Whether the user is a member of the specified role. |
| **HasClaim(string, string)** | `bool` | Whether the user has a claim with the given type and value. |
| **HasScope(string)** | `bool` | Whether the user has the specified OAuth 2.0 / OIDC scope. Handles space-separated scopes. |
| **GetClaimValue(string)** | `string?` | The value of the first claim with the specified type, or null. |
| **GetClaimValues(string)** | `IEnumerable<string>` | All values for a specific claim type. |

### ClaimsPrincipalExtensions

| Method | Description |
|---|---|
| `GetUserId()` | Parses the NameIdentifier claim into a `Guid`. Throws `InvalidOperationException` if unavailable. |
| `HasClaim(type, value)` | Checks for a claim with the given type and value. |
| `HasScope(value)` | Checks for an OIDC scope (space-separated or individual claim entries). |

## Additional documentation

For more information on how this package fits into the larger RA.Utilities ecosystem, please see the main repository [documentation](http://redonalla.github.io/RA.Utilities/nuget-packages/auth/Authorization/).

- To learn about setting up JWT authentication, see the `RA.Utilities.Authentication.JwtBearer` package documentation.

## Contributing

Contributions are welcome! If you have a suggestion, find a bug, or want to provide feedback, please open an issue in the RA.Utilities [GitHub repository](https://github.com/RedonAlla/RA.Utilities).

### Pull Request Process

1.  **Fork the Repository**: Start by forking the RA.Utilities repository.
2.  **Create a Branch**: Create a new branch for your feature or bug fix from the `main` branch. Please use a descriptive name (e.g., `feature/add-claim-transforms` or `fix/scope-parsing`).
3.  **Make Your Changes**: Write your code, ensuring it adheres to the existing coding style. Add or update XML documentation for any new public APIs.
4.  **Update README**: If you are adding new functionality, please update the `README.md` file accordingly.
5.  **Submit a Pull Request**: Push your branch to your fork and open a pull request to the `main` branch of the original repository. Provide a clear description of the changes you have made.

### Coding Standards

- Follow the existing coding style and conventions used in the project.
- Ensure all public members are documented with clear XML comments.
- Keep changes focused. A pull request should address a single feature or bug.

Thank you for contributing!
