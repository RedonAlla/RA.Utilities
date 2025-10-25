# Release Notes for RA.Utilities.Authorization

## Version 1.0.0-preview.6.3

This release significantly enhances the `RA.Utilities.Authorization` package by introducing the `ICurrentUser` interface for improved testability, refining the dependency injection setup, and adding comprehensive documentation.

### 🚀 Features & Enhancements

*   **Introduced `ICurrentUser` Interface**: The `AppUser` class now implements the `ICurrentUser` interface. This promotes dependency on abstractions rather than concrete implementations, making services that use this utility much easier to unit test.
*   **Improved Dependency Injection**: The `AddAppUser` extension method has been renamed to `AddCurrentUser`. It now registers the service against the `ICurrentUser` interface, aligning with best practices.
*   **Comprehensive Documentation**: Added detailed user guides for all public components, including usage examples and API references:
    *   `AppUser` & `ICurrentUser`
    *   `ClaimsPrincipalExtensions`
    *   `DependencyInjectionExtensions`

### 🐛 Bug Fixes & Minor Improvements

*   **Corrected `GetClaimValue`**: Fixed a misleading parameter name in the `AppUser.GetClaimValue` method for better code clarity.
*   **Improved XML Comments**: Updated XML documentation in `AppUser` to be more precise.

---
