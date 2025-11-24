# RA.Utilities.Authentication.JwtBearer Release Notes

## Version 10.0.0
![Date Badge](https://img.shields.io/badge/Publish-23%20November%202025-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-10.0.0-blue?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Core.Exceptions/10.0.0)

Updated the project version from `10.0.0-rc.2` to the stable release version `10.0.0` in preparation for a production release.

## Version 10.0.0-rc.2
![Date Badge](https://img.shields.io/badge/Publish-23%20November%202025-lightblue?logo=fastly&logoColor=white)
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