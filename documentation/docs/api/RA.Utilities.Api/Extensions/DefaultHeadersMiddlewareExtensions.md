---
sidebar_position: 1
---

```powershell
Namespace: RA.Utilities.Api.Extensions
```

The `DefaultHeadersMiddlewareExtensions` class provides a paired set of extension methods for registering and configuring the `DefaultHeadersMiddleware` in an ASP.NET Core application. The `Add`/`Use` pair follows the standard ASP.NET Core convention — `Add` registers services in DI, `Use` adds the middleware to the request pipeline.

## Methods

### `AddDefaultHeadersMiddleware()`

Registers the `DefaultHeadersMiddleware` and its dependencies in the dependency injection container.

#### Parameters
| Parameter | Type | Description |
| --------- | ---- | ----------- |
| **services** | `IServiceCollection` | The service collection to configure. |
| **configureOptions** | `Action<DefaultHeadersOptions>?` | Optional delegate to configure `DefaultHeadersOptions` (e.g., required headers, paths to ignore). |

This method performs two registrations:
- **Configures** `DefaultHeadersOptions` if a delegate is provided.
- **Registers** `DefaultHeadersMiddleware` as a transient service.

`DefaultHeadersOptions` defaults to requiring the `x-request-id` header with auto-generation and response echoing enabled.

#### Example

```csharp
builder.Services.AddDefaultHeadersMiddleware(options =>
{
    options.PathsToIgnore.Add("/health");
    options.RequiredHeaders.Add(new RequiredHeaderDefinition
    {
        Name = "x-custom-header",
        AutoGenerate = false,
        ErrorMessage = "Missing required custom header."
    });
});
```

### `UseDefaultHeadersMiddleware()`

Adds the `DefaultHeadersMiddleware` to the request pipeline. Must be called after `AddDefaultHeadersMiddleware()`.

#### Parameters
| Parameter | Type | Description |
| --------- | ---- | ----------- |
| **builder** | `IApplicationBuilder` | The application builder to add the middleware to. |

#### Returns
`IApplicationBuilder` for fluent chaining.

#### Example

```csharp
var app = builder.Build();

app.UseDefaultHeadersMiddleware();
// Other middleware...
app.MapEndpoints();

app.Run();
```

## Usage in Program.cs

```csharp
using RA.Utilities.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Register the middleware services
builder.Services.AddDefaultHeadersMiddleware(options =>
{
    options.PathsToIgnore.Add("/health");
});

var app = builder.Build();

// Add the middleware to the pipeline (early, to enforce headers on all requests)
app.UseDefaultHeadersMiddleware();
app.MapEndpoints();

app.Run();
```
