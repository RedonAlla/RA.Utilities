<p align="center">
  <img src="../../Assets/Images/jwt.svg" alt="RA.Utilities.Authentication.JwtBearer Logo" width="128">
</p>

# RA.Utilities.Authentication.JwtBearer
[![NuGet version](https://img.shields.io/nuget/v/RA.Utilities.Authentication.JwtBearer.svg)](https://www.nuget.org/packages/RA.Utilities.Authentication.JwtBearer/)

A utility library to simplify the configuration of JWT Bearer authentication in ASP.NET Core applications.

## Overview

This library provides extension methods to streamline the setup of JWT Bearer authentication by leveraging the standard `IConfiguration` system.
It allows you to configure `JwtBearerOptions` directly from your `appsettings.json` file, reducing boilerplate code in your `Program.cs` or `Startup.cs`.

The main benefits are:
-   **Simplified Setup**: A single extension method call to configure JWT Bearer authentication.
-   **Configuration-driven**: Easily manage JWT validation parameters through `appsettings.json` without changing code.
-   **Customizable**: Supports custom configuration for `ClockSkew` and the `IssuerSigningKey`.

## 🛠️ Installation

You can install the package from NuGet.

```shell
dotnet add package RA.Utilities.Authentication.JwtBearer
```

*(Note: This assumes the package is published on NuGet with the name `RA.Utilities.Authentication.JwtBearer`.)*

## Usage

In your `Program.cs` (for minimal APIs) or `Startup.cs`, use the `AddJwtBearerAuthentication` extension method to configure authentication and authorization services.

### `Program.cs` (ASP.NET Core 6+)

```csharp
using RA.Utilities.Authentication.JwtBearer.Extensions;

var builder = WebApplication.CreateBuilder(args);

// 1. Add and configure JWT Bearer authentication using settings from IConfiguration.
// This single call also adds the necessary authorization services.
builder.Services.AddJwtBearerAuthentication(builder.Configuration);

var app = builder.Build();

// 2. Add the authentication and authorization middleware.
app.UseAuth(); // This is a convenience method for app.UseAuthentication() and app.UseAuthorization().

app.MapGet("/", () => "Hello World!");

app.Run();
```

## Configuration

This library reads JWT Bearer options from the `Authentication:Schemes:Bearer` section of your configuration file (e.g., `appsettings.json`).

Here is an example `appsettings.json` configuration:

```json
{
  "Authentication": {
    "Schemes": {
      "Bearer": {
        "Authority": "https://your-identity-provider.com/",
        "Audience": "your-api-audience",
        "TokenValidationParameters": {
          "ValidateIssuer": true,
          "ValidateAudience": true,
          "ValidateLifetime": true,
          "ValidateIssuerSigningKey": true,
          "ClockSkewInSeconds": 30,
          "IssuerSigningKeyString": "your-super-secret-key-that-is-long-enough-for-the-algorithm"
        }
      }
    }
  }
}
```

> [!TIP]
>
>When using an identity provider (`Authority`), you typically don't need to specify `ValidIssuer`, `ValidAudience`, or `IssuerSigningKey` as these are discovered from the metadata endpoint.
The example above shows settings for both scenarios (using an authority or validating a self-issued token).
>

The library automatically binds these settings to `JwtBearerOptions`. It also provides special handling for:

-   `ClockSkewInSeconds`: Converts this integer value into a `TimeSpan` for `TokenValidationParameters.ClockSkew`.
-   `IssuerSigningKeyString`: Converts this string into a `SymmetricSecurityKey` for `TokenValidationParameters.IssuerSigningKey`.

## Contributing

Contributions are welcome! If you have a suggestion or find a bug, please open an issue to discuss it.

### Pull Request Process

1.  **Fork the Repository**: Start by forking the RA.Utilities repository.
2.  **Create a Branch**: Create a new branch for your feature or bug fix from the `main` branch. Please use a descriptive name (e.g., `feature/add-claim-support` or `fix/readme-typo`).
3.  **Make Your Changes**: Write your code, ensuring it adheres to the existing coding style. Add or update XML documentation for any new public APIs.
4.  **Update README**: If you are adding new functionality, please update the `README.md` file accordingly.
5.  **Submit a Pull Request**: Push your branch to your fork and open a pull request to the `main` branch of the original repository. Provide a clear description of the changes you have made.

### Coding Standards

-   Follow the existing coding style and conventions used in the project.
-   Ensure all public members are documented with clear XML comments.
-   Keep changes focused. A pull request should address a single feature or bug.

Thank you for contributing!