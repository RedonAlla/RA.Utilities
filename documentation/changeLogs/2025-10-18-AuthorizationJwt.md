---
title: RA.Utilities.Authentication.JwtBearer
authors: [RedonAlla]
---

## Version 1.0.0-preview.6.3
[![NuGet version](https://img.shields.io/nuget/v/RA.Utilities.Authentication.JwtBearer.svg)](https://www.nuget.org/packages/RA.Utilities.Authentication.JwtBearer/)

This is the initial release of `RA.Utilities.Authentication.JwtBearer`, a utility library designed to simplify the configuration of JWT Bearer authentication in ASP.NET Core applications.

<!-- truncate -->

### ✨ Features

*   **Simplified Setup**: Configure JWT Bearer authentication with a single call to `AddJwtBearerAuthentication()` in your `Program.cs`.
*   **Configuration-Driven**: Leverages `IConfiguration` to bind `JwtBearerOptions` directly from your `appsettings.json` file, reducing boilerplate code.
*   **Smart Key Handling**: Automatically converts the `IssuerSigningKeyString` from your configuration into a `SymmetricSecurityKey`.
*   **Convenience Methods**: Includes a `UseAuth()` extension method for `IApplicationBuilder` to register both authentication and authorization middleware.
*   **Customizable**: Supports further customization of `JwtBearerOptions` via an optional `Action` delegate.
