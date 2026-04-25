---
title: ConflictException
---

```bash
Namespace: RA.Utilities.Core.Exceptions
```

The purpose of the `ForbiddenException` in your framework is to signal a permission-based failure.

It is used when a user has successfully authenticated (the server knows who they are), but they are specifically forbidden from performing the requested action or accessing the target resource because they lack the required roles, claims, or permissions. In RESTful standards, this maps to **HTTP 403 Forbidden**.

## 🚀 Example: When to use it
You should throw this exception in your domain services or application handlers when a business rule determines the user doesn't have the right "clearance," even if they are logged in.

```csharp
public async Task<Result> UpdateSensitiveDataAsync(Guid resourceId, User currentUser)
{
    var resource = await _repository.GetByIdAsync(resourceId);

    // Business Rule: Only the 'Owner' or 'SuperAdmin' can edit this specific resource.
    if (resource.OwnerId != currentUser.Id && !currentUser.IsSuperAdmin)
    {
        // The user is logged in (authenticated), but they are FORBIDDEN from this action.
        return new ForbiddenException(
            "INSUFFICIENT_PERMISSIONS", 
            "You do not have permission to modify this resource."
        );
    }

    // ... proceed with update
    return Result.Success();
}
```

**ForbiddenException (403)**: "I know who you are, but you aren't allowed to do this."

By using `ForbiddenException`, your `GlobalExceptionHandler` can automatically return a 403 status code, which helps frontend applications differentiate between a user needing to log back in (401) and a user needing to request higher access levels (403).