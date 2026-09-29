---
title: IUserContext
sidebar_position: 1
---

```powershell
Namespace: RA.Utilities.Authorization
```

The `IUserContext` interface is a strongly-typed, injectable contract that simplifies access to the claims of the currently authenticated user.
It is implemented internally by the `UserContext` class, which wraps the user's `ClaimsPrincipal` from the current `HttpContext`.

> **v10.0.2 change**: `IUserContext` replaces the concrete `AppUser` class from earlier versions.
Register it with `AddUserContext()` and inject `IUserContext` instead of `AppUser`. See the [migration guide](./migration-guides.mdx).

### 🎯 Purpose

In a typical application, retrieving user information involves injecting [`IHttpContextAccessor`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.http.ihttpcontextaccessor) into your controllers or services and manually parsing the [`ClaimsPrincipal`](https://learn.microsoft.com/en-us/dotnet/api/system.security.claims.claimsprincipal). This is repetitive and makes unit testing difficult.

`IUserContext` solves these problems by:

1. **Abstracting `HttpContext`**: It exposes the user's claims through a clean, injectable interface that doesn't require a direct dependency on `HttpContext`.
2. **Simplifying Claim Access**: It offers simple properties for common claims like `Id`, `Name`, and `Email` without needing to know the underlying claim type strings.
3. **Enhancing Testability**: Because it's an interface, you can mock `IUserContext` directly in your unit tests — no `IHttpContextAccessor` or `HttpContext` construction required.

### ✨ Key Benefits:

1. **Simplified Access**: Inject `IUserContext` instead of `IHttpContextAccessor` to get user data.
2. **Strongly-Typed**: Provides `string? Id` and `Guid UserId` properties, plus `Name` and `Email`.
3. **Testability**: Mock `IUserContext` with any mocking framework to simulate different user scenarios.
4. **Reduced Boilerplate**: Eliminates repetitive code for accessing user claims.

```csharp showLineNumbers
namespace RA.Utilities.Authorization;

/// <summary>
/// Provides a strongly-typed way to access the claims of the currently authenticated user.
/// </summary>
public interface IUserContext
{
    bool IsAuthenticated { get; }
    string? Id { get; }
    Guid UserId { get; }
    string? Email { get; }
    string? Name { get; }
    string? GetClaimValue(string claimType);
    IEnumerable<string> GetClaimValues(string claimType);
    bool HasClaim(string claimType, string claimValue);
    bool HasScope(string scopeValue);
    bool IsInRole(string roleName);
}
```

### 🚀 Usage

#### Step 1: Register the Service

In your `Program.cs`, call `AddUserContext()` to register `IUserContext` (as scoped) along with its required `IHttpContextAccessor`.

```csharp showLineNumbers
// Program.cs
using RA.Utilities.Authorization.Extensions;

var builder = WebApplication.CreateBuilder(args);

// highlight-next-line
builder.Services.AddUserContext();
```

#### Step 2: Inject and Use `IUserContext`

Inject `IUserContext` into your controllers or services to access user information.

```csharp showLineNumbers
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using RA.Utilities.Authorization;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly IUserContext _user;

    public ProfileController(IUserContext user)
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
