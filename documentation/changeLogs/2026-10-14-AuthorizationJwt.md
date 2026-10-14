---
title: RA.Utilities.Authentication.JwtBearer
authors: [RedonAlla]
---

## Version 10.0.2
![Date Badge](https://img.shields.io/badge/Publish-14%20October%202026-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-10.0.2-blue?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Authentication.JwtBearer/10.0.2)

`DependencyInjectionExtensions` is split into `ServiceCollectionExtensions` and `ApplicationBuilderExtensions`, and the extension methods are refactored to C# 14 extension members. Method names, signatures, and the namespace are unchanged.

<!-- truncate -->

### ⚠️ Breaking Changes

* **`DependencyInjectionExtensions` split into `ServiceCollectionExtensions` and `ApplicationBuilderExtensions`**: `AddJwtBearerAuthentication()` (extension on `IServiceCollection`) now lives in `ServiceCollectionExtensions`; `UseAuth()` (extension on `IApplicationBuilder`) lives in `ApplicationBuilderExtensions`. Both remain in the `RA.Utilities.Authentication.JwtBearer.Extensions` namespace. Call sites need no changes — extension methods resolve by target type — but code that references the `DependencyInjectionExtensions` class name directly (`typeof(...)`, unit-test assertions, reflection) must be updated.

```csharp
- var t = typeof(DependencyInjectionExtensions);
+ var s = typeof(ServiceCollectionExtensions);      // AddJwtBearerAuthentication()
+ var a = typeof(ApplicationBuilderExtensions);     // UseAuth()
```

### 📝 Improvements

* **C# 14 extension members**: both classes now declare their methods inside `extension(...)` blocks on `IServiceCollection` and `IApplicationBuilder`. The generated extension methods have identical names and signatures, so the refactor is source-compatible for all callers.
* The split mirrors the classic ASP.NET Core `Add`/`Use` naming convention used across the RA.Utilities packages — `ServiceCollectionExtensions` for service registration, `ApplicationBuilderExtensions` for pipeline setup.

## Version 10.0.1
![Date Badge](https://img.shields.io/badge/Publish-07%20August%202026-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-10.0.1-blue?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Authentication.JwtBearer/10.0.1)

### 🔧 Fixes & Improvements

* **Consolidated Options Configuration**: All `JwtBearerOptions` binding and special conversions now happen in a single place (`ConfigureJwtBearerOptions`), eliminating a subtle bug where the user-provided `configureOptions` callback could be silently overwritten. The callback now runs **last**, so programmatic overrides always win.
* **Fail-Fast Key Validation**: The issuer signing key is now validated at construction time rather than on first use, catching misconfiguration immediately at app startup.
* **Null-Reference Guard**: Added `ArgumentNullException.ThrowIfNull` for the `IConfiguration` parameter, matching the guard in the DI extension method.

<!-- truncate -->

## Version 10.0.0
![Date Badge](https://img.shields.io/badge/Publish-23%20November%202025-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-10.0.0-blue?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Authentication.JwtBearer/10.0.0)

Updated the project version from `10.0.0-rc.2` to the stable release version `10.0.0` in preparation for a production release.

## Version 10.0.0-rc.2
![Date Badge](https://img.shields.io/badge/Publish-18%20October%202025-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-10.0.0--rc.2-orange?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Authentication.JwtBearer/10.0.0-rc.2)


This release modernizes the `RA.Utilities.Authentication.JwtBearer` package, providing a streamlined, configuration-driven approach to setting up JWT Bearer authentication in ASP.NET Core applications.

### ✨ New Features & Improvements

*   **Configuration-Driven Setup**:
    *   Introduced the `AddJwtBearerAuthentication()` extension method, which configures JWT Bearer authentication by reading settings directly from the `Authentication:Schemes:Bearer` section in `appsettings.json`.
    *   Eliminates hardcoded values and boilerplate code in `Program.cs`.

*   **Smart Configuration Handling**:
    *   Automatically converts `ClockSkewInSeconds` from your configuration into a `TimeSpan`.
    *   Automatically converts `IssuerSigningKeyString` into a `SymmetricSecurityKey`, simplifying the setup for self-issued tokens.

*   **Convenience `UseAuth()` Method**:
    *   Includes a `UseAuth()` extension method that registers both `app.UseAuthentication()` and `app.UseAuthorization()` with a single call.

*   **Updated Documentation**:
    *   The `README.md` has been updated to provide clear, step-by-step instructions and a complete `appsettings.json` example.

### 🚀 Getting Started

Register the services in your `Program.cs`:

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddJwtBearerAuthentication(builder.Configuration);

var app = builder.Build();

app.UseAuth();
```
