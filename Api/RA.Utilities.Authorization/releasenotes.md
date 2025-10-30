# Release Notes for RA.Utilities.Authorization

## Version 1.1.0 (TBD)

This release marks a major evolution of the `RA.Utilities.Authorization` package, shifting its focus from a simple current user service to a powerful, claims-based authorization system. The package now provides a flexible and maintainable way to implement permission-based security in ASP.NET Core applications.

### 🚀 Features & Enhancements

*   **Permission-Based Authorization**: Introduced the `[HasPermission]` attribute to protect endpoints with granular permissions, moving away from role-based checks.
*   **Dynamic Policy Provider**: Implemented `IAuthorizationPolicyProvider` to dynamically create authorization policies based on permissions required by the `[HasPermission]` attribute. This eliminates the need to pre-register every policy.
*   **Simplified Setup**: Added the `AddPermissionAuthorization()` extension method to register all necessary services for permission-based authorization with a single line of code.
*   **Custom Requirement and Handler**: Created `PermissionRequirement` and `PermissionAuthorizationHandler` to perform the core logic of checking a user's "permissions" claim against the required permission.
*   **Comprehensive `README.md`**: The package documentation has been completely rewritten to reflect the new functionality, with clear examples for both controller-based and minimal APIs.

###  Breaking Changes

*   **Deprecation of `ICurrentUser`**: The `ICurrentUser` interface and `AppUser` class have been removed. The focus of this package is now solely on authorization. Accessing user claims should be done directly via `HttpContext.User`.
*   **Removed `AddCurrentUser`**: The `AddCurrentUser()` extension method has been replaced by `AddPermissionAuthorization()`.

---
