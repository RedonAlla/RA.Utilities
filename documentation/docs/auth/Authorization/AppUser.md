---
title: AppUser
sidebar_position: 1
---

```powershell
Namespace: RA.Utilities.Authorization
```

The `AppUser` class is a strongly-typed service that simplifies access to the claims of the currently authenticated user.

### 🎯 Purpose

The `AppUser` class is a strongly-typed service designed to simplify accessing the claims of the currently authenticated user in an ASP.NET Core application.

In a typical application, retrieving user information involves injecting [`IHttpContextAccessor`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.http.ihttpcontextaccessor) into your controllers or services and manually parsing the [`ClaimsPrincipal`](https://learn.microsoft.com/en-us/dotnet/api/system.security.claims.claimsprincipal).
This can be repetitive and makes unit testing difficult.

The `AppUser` class solves these problems by:

1. **Abstracting [`HttpContext`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.http.httpcontext)**: It acts as a wrapper around the user's [`ClaimsPrincipal`](https://learn.microsoft.com/en-us/dotnet/api/system.security.claims.claimsprincipal), providing a clean, injectable service (`ICurrentUser` which is implemented by `AppUser`) that doesn't require a direct dependency on [`HttpContext`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.http.httpcontext).
2. **Simplifying Claim Access**: It offers simple properties and methods to get common claims like `Id`, `Name`, and `Email` without needing to know the underlying claim type strings (e.g., `ClaimTypes.NameIdentifier`).
4. **Enhancing Testability**: Because it's an injectable service, you can easily mock `AppUser` in your unit tests to simulate various user scenarios (e.g., an authenticated user, an admin, an unauthenticated user) without needing to construct a complex [`HttpContext`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.http.httpcontext).

In short, `AppUser` provides a clean, reusable, and testable way to work with user identity, reducing boilerplate and improving the overall quality of your application's authorization logic.

### ✨ Key Benefits:

1.  **Simplified Access**: Inject `AppUser` instead of [`IHttpContextAccessor`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.http.ihttpcontextaccessor) to get user data.
2.  **Strongly-Typed**: Provides convenient methods to get the user's ID, name, and email without manual parsing.
3.  **Testability**: Easily mock `AppUser` in unit tests to simulate different user scenarios.
4.  **Reduced Boilerplate**: Eliminates repetitive code for accessing user claims.

### 🚀 Usage

#### Step 1: Register the Service

In your `Program.cs`, call `AddCurrentUser()` to register the service.

```csharp showLineNumbers
// Program.cs
using RA.Utilities.Authorization.Extensions;

var builder = WebApplication.CreateBuilder(args);

// highlight-next-line
builder.Services.AddCurrentUser();
```

#### Step 2: Inject and Use `AppUser`

Inject `AppUser` into your controllers or services to access user information.

```csharp showLineNumbers
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using RA.Utilities.Authorization.Abstractions;

[ApiController]
[Route("api/[controller]")]
[Authorize]
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
        var userInfo = new
        {
            UserId = _currentUser.GetId<Guid>(),
            UserName = _currentUser.GetName(),
            Email = _currentUser.GetEmail(),
            IsAdmin = _currentUser.IsInRole("Admin")
        };

        return Ok(userInfo);
    }
}
```

## API Reference

| Method/Property                 | Return Type          | Description                                                                                             |
| ------------------------------- | -------------------- | ------------------------------------------------------------------------------------------------------- |
| **IsAuthenticated()**             | `bool`               | Checks if the user is authenticated.                                                                    |
| **`GetId<T>()`**                    | `T?`                 | Gets the user's ID (`NameIdentifier` claim) and converts it to the specified type (e.g., `Guid`, `int`). |
| **GetName()**                     | `string?`            | Gets the user's name (`Name` claim).                                                                    |
| **GetEmail()**                    | `string?`            | Gets the user's email (`Email` claim).                                                                  |
| **IsInRole(string roleName)**     | `bool`               | Checks if the user is a member of the specified role.                                                   |
| **GetClaimValue(string claimType)** | `string?`            | Gets the value of the first claim with the specified type.                                              |
| **GetClaimValues(string claimType)**| `IEnumerable<string>`| Gets all values for a specific claim type.                                                              |
| **HasClaim(string claimValue)**   | `bool`               | Checks if the user has a claim with the type `claim` and the specified value.                           |
| **HasScope(string scopeValue)**   | `bool`               | Checks if the user has a claim with the type `scope` and the specified value.                           |
