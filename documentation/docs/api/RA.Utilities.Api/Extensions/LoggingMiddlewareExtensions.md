---
sidebar_position: 2
---

```powershell
Namespace: RA.Utilities.Api.Extensions
```

The `HttpLoggingExtensions` class provides a paired set of extension methods for registering and configuring the `LoggingMiddleware` in an ASP.NET Core application. The `Add`/`Use` pair follows the standard ASP.NET Core convention — `Add` registers services in DI, `Use` adds the middleware to the request pipeline.

## Methods

### `AddLoggingMiddleware()`

Registers the `LoggingMiddleware` and its dependencies in the dependency injection container.

#### Parameters
| Parameter | Type | Description |
| --------- | ---- | ----------- |
| **services** | `IServiceCollection` | The service collection to configure. |
| **configureOptions** | `Action<HttpLoggingOptions>?` | Optional delegate to configure `HttpLoggingOptions` (e.g., excluded headers, paths to ignore, max body length, warning threshold). |

This method performs three registrations:
- **Configures** `HttpLoggingOptions` if a delegate is provided.
- **Registers** `RecyclableMemoryStreamManager` as a singleton (using `TryAddSingleton` so an existing registration is preserved).
- **Registers** `LoggingMiddleware` as a transient `IMiddleware`.

#### Example

```csharp
builder.Services.AddLoggingMiddleware(options =>
{
    options.MaxBodyLogLength = 8192;
    options.WarningThresholdMilliseconds = 500;
    options.PathsToIgnore.Add("/health");
    options.ExcludedHeaders.Add("Authorization");
});
```

### `UseLoggingMiddleware()`

Adds the `LoggingMiddleware` to the request pipeline. Must be called after `AddLoggingMiddleware()`.

#### Parameters
| Parameter | Type | Description |
| --------- | ---- | ----------- |
| **builder** | `IApplicationBuilder` | The application builder to add the middleware to. |

#### Returns
`IApplicationBuilder` for fluent chaining.

#### Example

```csharp
var app = builder.Build();

app.UseLoggingMiddleware();
// Other middleware...
app.MapEndpoints();

app.Run();
```

## Usage in Program.cs

```csharp
using RA.Utilities.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Register the middleware services
builder.Services.AddLoggingMiddleware(options =>
{
    options.MaxBodyLogLength = 4096;
});

var app = builder.Build();

// Add the middleware to the pipeline (early, to capture all requests)
app.UseLoggingMiddleware();
app.MapEndpoints();

app.Run();
```
