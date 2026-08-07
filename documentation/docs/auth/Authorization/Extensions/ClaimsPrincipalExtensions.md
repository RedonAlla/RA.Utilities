---
title: ClaimsPrincipalExtensions
sidebar_position: 2
---

```powershell
Namespace: RA.Utilities.Authorization.Extensions
```

The `ClaimsPrincipalExtensions` class provides convenient extension methods for `ClaimsPrincipal`, simplifying common claim-related tasks.

### 🎯 Purpose

Instead of writing repetitive code to find, parse, and validate claims, these extension methods provide clean, readable, and reusable shortcuts.

## ✨ Available Methods

### GetUserId()

Retrieves the user's ID from the `NameIdentifier` claim and parses it into a `Guid`.
Throws if the claim is missing or invalid.

**Returns**: `Guid`
**Throws**: `InvalidOperationException` if the claim is missing or cannot be parsed.

```csharp
var userId = User.GetUserId(); // 'User' is the ClaimsPrincipal from HttpContext
```

### HasClaim(string claimType, string claimValue)

Checks if the principal has a claim with the specified type and value.

**Returns**: `bool`

```csharp
if (User.HasClaim("permission", "CanDeleteUsers"))
{
    // ... perform action
}
```

### HasScope(string scopeValue)

Checks if the principal has the specified OAuth 2.0 / OIDC scope.
Handles both space-separated scopes (the OIDC standard) and individual scope-per-claim entries.

**Returns**: `bool`

```csharp
if (User.HasScope("api.read"))
{
    // ... allow read access
}
```

These extensions are used by the `AppUser` service to build its higher-level API, but are also available for use directly on any `ClaimsPrincipal`.
