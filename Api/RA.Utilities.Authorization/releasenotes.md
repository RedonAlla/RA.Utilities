# Release Notes for RA.Utilities.Authorization

## Version 10.0.1
![Date Badge](https://img.shields.io/badge/Publish-07%20August%202026-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-10.0.1-blue?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Authorization/10.0.1)

### 🔧 Fixes & Improvements

* **Fixed `HasClaim` signature**: `HasClaim(string claimValue)` → `HasClaim(string claimType, string claimValue)`. The old signature hardcoded the claim type to `"claim"` which never matched real auth provider claims.
* **Fixed `HasScope` for OIDC compliance**: `HasScope` now splits space-separated scope values per the OAuth 2.0 standard. A `"scope"` claim with value `"api.read api.write"` now correctly matches individual scopes.
* **New `Guid UserId` on `AppUser`**: Typed access to the user ID without calling the extension method. Throws `InvalidOperationException` if the user is not authenticated or the claim is invalid.
* **Null validation**: `AppUser` constructor now throws `ArgumentNullException` for null `IHttpContextAccessor` instead of silently accepting it.
* **Removed `FindFirstValue` extension**: Shadowed the built-in `ClaimsPrincipal.FindFirstValue(string)` method. Use the built-in directly.
* **`GetUserId` exception type**: Throws `InvalidOperationException` instead of `ApplicationException`.

### ⚠️ Breaking Change

`HasClaim(string claimValue)` → `HasClaim(string claimType, string claimValue)`. Update callers to provide both a claim type and value:

```csharp
// ❌ v10.0.0
user.HasClaim("CanDeleteUsers");

// ✅ v10.0.1
user.HasClaim("permission", "CanDeleteUsers");
```

## Version 10.0.0
![Date Badge](https://img.shields.io/badge/Publish-23%20November%202025-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-10.0.0-blue?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Authorization/10.0.0)

Updated the project version from `10.0.0-rc.2` to the stable release version `10.0.0` in preparation for a production release. The package provides a strongly-typed `AppUser` service with convenience extension methods for accessing authenticated user claims.

## Version 10.0.0-rc.2
![Date Badge](https://img.shields.io/badge/Publish-18%20October%202025-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-10.0.0--rc.2-orange?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Authorization/10.0.0-rc.2)

### ✨ Features

* **Strongly-typed user service**: `AppUser` provides injectable access to `HttpContext.User` claims via properties (`Id`, `Name`, `Email`) and methods (`IsInRole`, `HasClaim`, `HasScope`, `GetClaimValue`, `GetClaimValues`).
* **Convenience DI registration**: `AddAppUser()` registers `AppUser` and `IHttpContextAccessor` in a single call.
* **ClaimsPrincipal extension methods**: `GetUserId()`, `HasClaim(type, value)`, and `HasScope(value)` for working directly with `ClaimsPrincipal`.
