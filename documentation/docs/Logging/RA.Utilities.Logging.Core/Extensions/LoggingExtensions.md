```bash
Namespace: RA.Utilities.Logging.Core.Extensions
```

The `LoggingExtensions` class is the primary entry point for your logging framework.
Its purpose is to provide a **"one-liner" setup for a production-ready Serilog configuration** in a .NET application.

It abstracts away all the complex boilerplate code required to configure Serilog correctly.
By calling its main method, `AddLoggingWithConfiguration()`, a developer can instantly get a powerful, pre-configured logger that includes:

* Integration with the .NET host and configuration system (`appsettings.json`).
* Custom enrichment, like the `RequestIdEnricher`, to add correlation IDs to every log.
* Detailed exception logging.
* Proper handling of `System.Text.Json` objects for better structured logs.

In short, it's a "batteries-included" extension that makes it incredibly simple for a developer to implement a consistent and robust logging strategy with minimal effort.

## Methods

### 1. `AddLoggingWithConfiguration()`

This is the main extension method. It configures Serilog as the logging provider for the application with a set of sensible, production-ready defaults. It abstracts away the boilerplate code for setting up Serilog, including reading from configuration, adding enrichers for request tracing and exceptions, and handling JSON destructuring.

#### Input Parameters
| Parameter	| Type |	Description |
| ---------	| ---- |	----------- |
| **builder** |	`WebApplicationBuilder` |	The application builder to configure. This is the this parameter for the extension method. |

#### Output Parameters
This method returns `void` as it directly configures the `Host` property of the provided `WebApplicationBuilder`.

#### Example
This example shows how to use `AddLoggingWithConfiguration` in your `Program.cs` to set up Serilog for your entire application.

1. **Configure `appsettings.json`**: Add a `Serilog` section to control log levels.

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft.AspNetCore": "Warning"
      }
    }
  }
}
```

2. **Call the extension method in `Program.cs`:**

```csharp
using RA.Utilities.Logging.Core.Extensions; // Add this using
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// highlight-start
// Add the RA Serilog configuration in one line
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


### `WithRequestIdEnricher()`
This is a fluent helper method that adds the custom RequestIdEnricher to the Serilog pipeline. The enricher is responsible for adding XRequestId and TraceId properties to log events for better request tracing and correlation.

#### Input Parameters
| Parameter	| Type |	Description |
| ---------	| ---- |	----------- |
| enrich	| `LoggerEnrichmentConfiguration` |	The Serilog enrichment configuration object. This is the this parameter for the extension method. |

| Parameter	| Type |	Description |
| ---------	| ---- |	----------- |
| Returns	| `LoggerConfiguration` |	The logger configuration, allowing for further chaining of configuration methods. |

#### Example
This method is typically used within the Serilog setup block.
While `AddLoggingWithConfiguration` calls it for you, here is how you would use it if you were configuring Serilog manually:

```csharp
using RA.Utilities.Logging.Core.Extensions;
using Serilog;

// Manual Serilog configuration
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    // highlight-next-line
    .Enrich.WithRequestIdEnricher() // Adds the custom enricher
    .WriteTo.Console()
    .CreateLogger();
```