# RA.Utilities.Logging.Core

[![NuGet version](https://img.shields.io/nuget/v/RA.Utilities.Logging.Core?logo=nuget&label=NuGet)](https://www.nuget.org/packages/RA.Utilities.Logging.Core/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/RA.Utilities.Logging.Core.svg?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Logging.Core/)
[![Codecov](https://codecov.io/github/RedonAlla/RA.Utilities/graph/badge.svg)](https://codecov.io/github/RedonAlla/RA.Utilities)
[![Documentation](https://img.shields.io/badge/Documentation-read-brightgreen.svg?logo=readthedocs&logoColor=fff)](https://redonalla.github.io/RA.Utilities/nuget-packages/Logging/RA.Utilities.Logging.Core/)
[![GitHub license](https://img.shields.io/github/license/RedonAlla/RA.Utilities?logo=googledocs&logoColor=fff)](https://github.com/RedonAlla/RA.Utilities?tab=MIT-1-ov-file)

`RA.Utilities.Logging.Core` provides a set of opinionated helpers and configurations for setting up structured logging with Serilog in .NET applications. It ships with pre-wired Serilog packages (Console, File, Async sinks; Exception, Sensitive Data enrichers) so that consumers can configure them declaratively via `appsettings.json` without adding individual NuGet references.

## 📚 Table of Contents

- Purpose
- Key Features
- Installation
- How to Use
- Contributing

---

## Purpose

Setting up a comprehensive logging solution from scratch can be repetitive. This package abstracts away the boilerplate Serilog wiring by providing a single extension method, `AddLoggingWithConfiguration`, that bootstraps a production-ready foundation.

## Key Features

**Configured automatically by `AddLoggingWithConfiguration`:**

- **Configuration-driven**: Reads Serilog settings from `appsettings.json` via `ReadFrom.Configuration`, so log levels, sinks, and enrichers are controlled declaratively.
- **Request ID enrichment**: Adds `XRequestId` and `TraceId` properties to every log event via `RequestIdEnricher`, sourced from the `x-request-id` header, `HttpContext.TraceIdentifier`, or the current `Activity`.
- **Exception details**: Uses `Serilog.Exceptions` to destructure exceptions with full stack trace detail.
- **System.Text.Json destructuring**: Uses `Destructurama.SystemTextJson` so that `JsonElement` and related types log as readable values instead of `ValueKind` enum names.

**Available for `appsettings.json` configuration** (packages are included as dependencies — no additional NuGet install needed):

- **Console sink** — `Serilog.Sinks.Console`
- **File sink** — `Serilog.Sinks.File` with rolling and size limits
- **Async wrapper** — `Serilog.Sinks.Async` to offload I/O from the request thread
- **Sensitive data masking** — `Serilog.Enrichers.Sensitive`

---

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

Add a `Serilog` section to your `appsettings.json` file. The `AddLoggingWithConfiguration` method reads from this section automatically.

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "Microsoft.Hosting.Lifetime": "Information",
        "System": "Warning"
      }
    },
    "Using": [
      "Serilog.Sinks.Console",
      "Serilog.Sinks.File",
      "Serilog.Sinks.Async",
      "Serilog.Enrichers.Sensitive",
      "Serilog.Exceptions"
    ],
    "Enrich": [
      { "Name": "WithExceptionDetails" },
      {
        "Name": "WithSensitiveDataMasking",
        "Args": {
          "options": {
            "MaskValue": "***SECRET***",
            "Mode": "Globally"
          }
        }
      }
    ],
    "WriteTo:Async": {
      "Name": "Async",
      "Args": {
        "configure": [
          {
            "Name": "File",
            "Args": {
              "path": "Logs/app-.log",
              "rollingInterval": "Day",
              "rollOnFileSizeLimit": true,
              "outputTemplate": "{Timestamp:G} [{Level}] ({XRequestId})({TraceId}) {Message:lj} ({SourceContext}){NewLine}{Exception}"
            }
          },
          {
            "Name": "Console",
            "Args": {
              "outputTemplate": "{Timestamp:G} [{Level}] ({XRequestId})({TraceId}) {Message:lj} ({SourceContext}){NewLine}{Exception}"
            }
          }
        ]
      }
    }
  }
}
```

### Step 2: Add the Logger in `Program.cs`

Call `AddLoggingWithConfiguration()` on your `WebApplicationBuilder`. This should be one of the first things you do to ensure all application startup events are logged.

```csharp
using RA.Utilities.Logging.Core.Extensions; // Add this using statement

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog with opinionated defaults
builder.AddLoggingWithConfiguration();

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
