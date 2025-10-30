---
title: RA.Utilities.Authorization
authors: [RedonAlla]
---

## Version 1.0.0-preview.6.31
[![NuGet version](https://img.shields.io/nuget/v/RA.Utilities.Authorization.svg)](https://www.nuget.org/packages/RA.Utilities.Authorization/)

This release significantly enhances the `RA.Utilities.Authorization` package by introducing the `ICurrentUser` interface for improved testability, refining the dependency injection setup, and adding comprehensive documentation.

<!-- truncate -->

### 🚀 Features & Enhancements

*   **Introduced `ICurrentUser` Interface**: The `AppUser` class now implements the `ICurrentUser` interface. This promotes dependency on abstractions rather than concrete implementations, making services that use this utility much easier to unit test.
*   **Improved Dependency Injection**: The `AddAppUser` extension method has been renamed to `AddCurrentUser`. It now registers the service against the `ICurrentUser` interface, aligning with best practices.
*   **Comprehensive Documentation**: Added detailed user guides for all public components, including usage examples and API references:
    *   `AppUser` & `ICurrentUser`
    *   `ClaimsPrincipalExtensions`
    *   `DependencyInjectionExtensions`
