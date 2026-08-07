---
title: AppUser
sidebar_position: 1
---

```powershell
Namespace: RA.Utilities.Authorization
```

The `AppUser` class is a strongly-typed, injectable service that simplifies access to the claims of the currently authenticated user.

### 🎯 Purpose

In a typical application, retrieving user information involves injecting [`IHttpContextAccessor`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.http.ihttpcontextaccessor) into your controllers or services and manually parsing the [`ClaimsPrincipal`](https://learn.microsoft.com/en-us/dotnet/api/system.security.claims.claimsprincipal). This is repetitive and makes unit testing difficult.

`AppUser` solves these problems by:

1. **Abstracting `HttpContext`**: It wraps the user's `ClaimsPrincipal`, providing a clean, injectable service that doesn't require a direct dependency on `HttpContext`.
2. **Simplifying Claim Access**: It offers simple properties for common claims like `Id`, `Name`, and `Email` without needing to know the underlying claim type strings.
3. **Enhancing Testability**: Because it's an injectable concrete class, you can easily mock `AppUser` in your unit tests to simulate various user scenarios without constructing a complex `HttpContext`.

### ✨ Key Benefits:

1. **Simplified Access**: Inject `AppUser` instead of `IHttpContextAccessor` to get user data.
2. **Strongly-Typed**: Provides `string? Id` and `Guid UserId` properties, plus `Name` and `Email`.
3. **Testability**: Easily mock `AppUser` in unit tests to simulate different user scenarios.
4. **Reduced Boilerplate**: Eliminates repetitive code for accessing user claims.

### 🚀 Usage

#### Step 1: Register the Service

In your `Program.cs`, call `AddAppUser()` to register the service.

```csharp showLineNumbers
// Program.cs
using RA.Utilities.Authorization.Extensions;

var builder = WebApplication.CreateBuilder(args);

// highlight-next-line
builder.Services.AddAppUser();
```

#### Step 2: Inject and Use `AppUser`

Inject `AppUser` into your controllers or services to access user information.

```csharp showLineNumbers
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using RA.Utilities.Authorization;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly AppUser _user;

    public ProfileController(AppUser user)
    {
        _user = user;
    }

    [HttpGet]
    public IActionResult GetUserProfile()
    {
        var userInfo = new
        {
            UserId = _user.UserId,
            Name = _user.Name,
            Email = _user.Email,
            IsAdmin = _user.IsInRole("Admin")
        };

        return Ok(userInfo);
    }
}
```

## API Reference

| Member | Type | Description |
|---|---|---|
| **IsAuthenticated** | `bool` | Whether the current user is authenticated. |
| **Id** | `string?` | The user's unique identifier from the NameIdentifier claim, or null. |
| **UserId** | `Guid` | The user's unique identifier as a `Guid`. Throws `InvalidOperationException` if not authenticated or not a valid Guid. |
| **Name** | `string?` | The user's name from the Name claim, or null. |
| **Email** | `string?` | The user's email from the Email claim, or null. |
| **IsInRole(string roleName)** | `bool` | Whether the user is a member of the specified role. |
| **HasClaim(string claimType, string claimValue)** | `bool` | Whether the user has a claim with the given type and value. |
| **HasScope(string scopeValue)** | `bool` | Whether the user has the specified OAuth 2.0 / OIDC scope. Handles space-separated scopes. |
| **GetClaimValue(string claimType)** | `string?` | The value of the first claim with the specified type, or null. |
| **GetClaimValues(string claimType)** | `IEnumerable<string>` | All values for a specific claim type. |
