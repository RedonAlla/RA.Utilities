<p align="center">
  <img src="https://raw.githubusercontent.com/RedonAlla/RA.Utilities/7609528d2f5783472cd1b6a8be8cc20957e85fbb/Assets/Images/logging.svg" alt="RA.Utilities.Logging.Core Logo" width="128">
</p>

# RA.Utilities.Logging.Core

[![NuGet version](https://img.shields.io/nuget/v/RA.Utilities.Logging.Core?logo=nuget&label=NuGet)](https://www.nuget.org/packages/RA.Utilities.Logging.Core/)
[![Codecov](https://codecov.io/github/RedonAlla/RA.Utilities/graph/badge.svg)](https://codecov.io/github/RedonAlla/RA.Utilities)
[![GitHub license](https://img.shields.io/github/license/RedonAlla/RA.Utilities)](https://github.com/RedonAlla/RA.Utilities/blob/main/LICENSE)
[![NuGet Downloads](https://img.shields.io/nuget/dt/RA.Utilities.Logging.Core.svg)](https://www.nuget.org/packages/RA.Utilities.Logging.Core/)


`RA.Utilities.Logging.Core` provides a set of opinionated helpers and configurations for setting up structured logging with Serilog in .NET applications. This package simplifies the integration of common sinks (Console, File), enrichers (Exceptions, Sensitive Data), and performance features like asynchronous logging, enabling a robust and consistent logging strategy out of the box.

## Purpose

Setting up a comprehensive logging solution from scratch can be repetitive. This package abstracts away the boilerplate configuration for Serilog by providing a single extension method, `AddRaSerilog`, that configures a production-ready logger with sensible defaults.

**Key Features Configured by Default:**

- **Structured Logging**: Logs are written in a structured format (JSON), making them easy to query and analyze.
- **Multiple Sinks**:
  - **Console Sink**: For easy viewing during development.
  - **File Sink**: For persistent log storage, with automatic rolling by file size.
- **Asynchronous Logging**: All sinks are wrapped in `Serilog.Sinks.Async` to minimize the performance impact of logging on your application's request thread.
- **Rich Enrichment**:
  - `FromLogContext`: Adds contextual information to logs.
  - `WithExceptionDetails`: Destructures exceptions to include detailed information like the stack trace.
  - `WithSensitiveDataMasking`: Automatically finds and masks sensitive data in log messages based on property names (e.g., "Password", "CreditCard").
- **Configuration-Driven**: Reads settings from `appsettings.json`, allowing you to easily adjust log levels and other parameters without changing code.

## 🛠️ Installation

You can install the package via the .NET CLI:

```bash
dotnet add package RA.Utilities.Logging.Core
```

Or through the NuGet Package Manager in Visual Studio.

---

## How to Use

Integrating the logger into your ASP.NET Core application is a two-step process.

### Step 1: Configure `appsettings.json`

Add a `Serilog` section to your `appsettings.json` file. The `AddRaSerilog` method will automatically read from this section. You can override log levels for different sources as needed.

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "Microsoft.Hosting.Lifetime": "Information"
      }
    }
  },
  "AllowedHosts": "*"
}
```

### Step 2: Add the Logger in `Program.cs`

Call the `AddRaSerilog()` extension method on your `WebApplicationBuilder`. This should be one of the first things you do to ensure all application startup events are logged.

```csharp
using RA.Utilities.Logging.Core.Extensions; // Add this using statement

var builder = WebApplication.CreateBuilder(args);

// Add the RA Serilog configuration
builder.AddRaSerilog();

try
{
    // Add other services
    builder.Services.AddControllers();

    var app = builder.Build();

    // Your middleware pipeline
    app.UseHttpsRedirection();
    app.MapControllers();

    Log.Information("Starting application...");
    app.Run();
}
catch (Exception ex)
{
    // Log fatal exceptions that prevent the app from starting
    Log.Fatal(ex, "Application failed to start.");
}
finally
{
    // Ensure all buffered logs are written to sinks before the app closes
    Log.CloseAndFlush();
}
```

That's it! Your application is now configured with structured, asynchronous, and enriched logging. You can inject `ILogger<T>` anywhere in your application via dependency injection to write logs.

---

## Contributing

Contributions are welcome! If you have a suggestion or find a bug, please open an issue to discuss it. Please follow the contribution guidelines outlined in the other `RA.Utilities` packages.

