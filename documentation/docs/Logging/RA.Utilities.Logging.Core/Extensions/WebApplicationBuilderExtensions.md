```bash
Namespace: RA.Utilities.Logging.Core.Extensions
```

The `WebApplicationBuilderExtensions` class is the **primary entry point** for wiring Serilog into an ASP.NET Core application.
It contains a single extension member, `AddLoggingWithConfiguration()`, which provides a "one-liner" setup for a production-ready Serilog configuration.

It abstracts away the boilerplate required to configure Serilog correctly by:

* Hooking Serilog into the .NET host through `builder.Host.UseSerilog(...)`.
* Reading the `Serilog` configuration section from `appsettings.json` with `ReadFrom.Configuration`.
* Adding the `WithExceptionDetails` enricher so exceptions log with full detail.
* Enabling destructuring of `System.Text.Json` types for readable structured logs.

:::info v10.1.1
Before v10.1.1 this method was declared in a class named `LoggingExtensions`. It now lives in `WebApplicationBuilderExtensions` (one class per extended type) and is written as a C# 14 extension member. The method name, signature, and namespace are unchanged — see the [migration guide](../migration-guides.mdx).
:::

Request ID enrichment is **not** activated by this method. To enable it, add [`WithRequestIdEnricher`](./LoggerEnrichmentConfigurationExtensions.md) to the `Serilog` → `Enrich` configuration (or call the fluent extension yourself) and register `IHttpContextAccessor` with `builder.Services.AddHttpContextAccessor()`.

## Methods

### `AddLoggingWithConfiguration()`

Configures Serilog as the logging provider for the application with a set of sensible, production-ready defaults.

#### Parameters

| Parameter | Type | Description |
| --------- | ---- | ----------- |
| **builder** | `WebApplicationBuilder` | The application builder to configure. This is the **receiver** of the extension member. |

#### Returns

`void`. The method configures the `Host` property of the supplied `WebApplicationBuilder` and returns nothing.

#### Exceptions

| Exception | Condition |
| --------- | --------- |
| `ArgumentNullException` | Thrown when `builder` is `null`. |

#### What it configures

| Step | Call | Effect |
| ---- | ---- | ------ |
| 1 | `loggerConfig.ReadFrom.Configuration(context.Configuration)` | Reads the `Serilog` section of `appsettings.json`, including levels, enrichers, and sinks. |
| 2 | `.Enrich.WithExceptionDetails()` | Destructures exceptions with type, message, and stack-trace detail (`Serilog.Exceptions`). |
| 3 | `.Destructure.SystemTextJsonTypes()` | Renders `System.Text.Json` types as readable values (`Destructurama.SystemTextJson`). |

#### What it does *not* configure

* **Sinks** — none are added in code. Without a `WriteTo` entry in the `Serilog` configuration, events have nowhere to go.
* **`RequestIdEnricher`** — request ID enrichment stays opt-in.
* **`IHttpContextAccessor`** — since v10.1.0 the accessor is no longer registered for you. Add `builder.Services.AddHttpContextAccessor()` when request ID enrichment is enabled.

#### Example

1. **Configure `appsettings.json`** — control levels, sinks, and enrichers declaratively:

```json showLineNumbers
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft.AspNetCore": "Warning"
      }
    },
    "Enrich": [
      { "Name": "WithRequestIdEnricher" }
    ],
    "WriteTo": [
      { "Name": "Console" }
    ]
  }
}
```

2. **Call the extension method in `Program.cs`:**

```csharp showLineNumbers
using RA.Utilities.Logging.Core.Extensions; // Add this using
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// highlight-start
// Register this when WithRequestIdEnricher is enabled in appsettings.json.
builder.Services.AddHttpContextAccessor();
builder.AddLoggingWithConfiguration();
// highlight-end

try
{
    Log.Information("Configuring web application...");

    builder.Services.AddControllers();

    var app = builder.Build();

    app.UseHttpsRedirection();
    app.MapControllers();

    Log.Information("Starting application...");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application failed to start.");
}
finally
{
    // Ensure all buffered logs are written before the app closes
    Log.CloseAndFlush();
}
```

:::tip
Call `AddLoggingWithConfiguration()` as one of the first statements in `Program.cs`. Everything logged after that point — including dependency-injection failures and startup diagnostics — goes through Serilog instead of the default .NET logger.
:::
