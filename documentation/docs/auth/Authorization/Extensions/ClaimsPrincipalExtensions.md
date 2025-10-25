---
title: ClaimsPrincipalExtensions
sidebar_position: 2
---

```powershell
Namespace: RA.Utilities.Authorization.Extensions
```

The `ClaimsPrincipalExtensions` class provides a set of convenient extension methods for the `ClaimsPrincipal` object, simplifying common claim-related tasks.

### 🎯 Purpose

The `ClaimsPrincipalExtensions` class is a static helper class that adds several convenient methods to the standard `ClaimsPrincipal` object.
Its primary purpose is to simplify and standardize the way you interact with a user's claims.

Instead of writing repetitive code to find, parse, and validate claims, these extension methods provide clean, readable, and reusable shortcuts.

## ✨ Available Methods

### GetUserId()

This is a standout feature.
It safely retrieves the user's ID from the `NameIdentifier` claim and attempts to parse it into a `Guid`.
If the claim is missing or invalid, it throws a clear exception.
This encapsulates error-prone parsing logic into a single, reliable method.

**Returns**: `Guid`
**Throws**: `ApplicationException` if the claim is missing or cannot be parsed into a `Guid`.

```csharp
var userId = User.GetUserId(); // 'User' is the ClaimsPrincipal from HttpContext
```

### FindFirstValue(string claimType)

This method acts as a null-safe wrapper around the built-in `FindFirstValue`, ensuring your code doesn't throw an exception if the `ClaimsPrincipal` object itself happens to be `null`.

**Returns**: `string?` - The claim value or `null` if not found.

```csharp
var email = User.FindFirstValue(ClaimTypes.Email);
```

### HasClaim(string claimValue)

Checks if the principal has a claim with the specific type `"claim"` and a matching value.
This is useful for simple permission checks.

**Returns**: `bool`

```csharp
if (User.HasClaim("CanDeleteUsers"))
{
    // ... perform action
}
```

### HasScope(string scopeValue)

Checks if the principal has a claim with the specific type `"scope"` and a matching value.
This is commonly used in OAuth 2.0 / OIDC scenarios.

**Returns**: `bool`

```csharp
if (User.HasScope("api.read"))
{
    // ... allow read access
}
```

These extensions are used by the `AppUser` service to build its higher-level, user-friendly API, but they are also available for you to use in any part of your application where you might be working directly with a `ClaimsPrincipal`.