---
sidebar_position: 3
---

```powershell
Namespace: RA.Utilities.Api.Extensions
```

The `RequestContextLoggingMiddlewareExtensions` class provides a paired set of extension methods for registering and configuring the `RequestContextLoggingMiddleware` in an ASP.NET Core application. The `Add`/`Use` pair follows the standard ASP.NET Core convention — `Add` registers services in DI, `Use` adds the middleware to the request pipeline.

:::caution
Do not use `RequestContextLoggingMiddleware` if you are already using [`LoggingMiddleware`](./LoggingMiddlewareExtensions.md). The `LoggingMiddleware` already enriches log entries with request-scoped context — it creates a logging scope containing the `x-request-id` correlation ID for every request it processes. Adding `RequestContextLoggingMiddleware` on top of it is redundant and adds unnecessary overhead.
:::

## Methods

### `AddRequestContextLoggingMiddleware()`

Registers the `RequestContextLoggingMiddleware` in the dependency injection container.

#### Parameters
| Parameter | Type | Description |
| --------- | ---- | ----------- |
| **services** | `IServiceCollection` | The service collection to configure. |

This method registers `RequestContextLoggingMiddleware` as a transient service.

#### Example

```csharp
builder.Services.AddRequestContextLoggingMiddleware();
```

### `UseRequestContextLoggingMiddleware()`

Adds the `RequestContextLoggingMiddleware` to the request pipeline. Must be called after `AddRequestContextLoggingMiddleware()`.

The middleware enriches log entries with request-scoped context by creating a logging scope that includes the correlation ID (`x-request-id`) from the incoming request. This allows all log entries within the scope of a request to be correlated.

#### Parameters
| Parameter | Type | Description |
| --------- | ---- | ----------- |
| **builder** | `IApplicationBuilder` | The application builder to add the middleware to. |

#### Returns
`IApplicationBuilder` for fluent chaining.

#### Example

```csharp
var app = builder.Build();

app.UseRequestContextLoggingMiddleware();
// Other middleware...
app.MapEndpoints();

app.Run();
```

## Usage in Program.cs

```csharp
using RA.Utilities.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Register the middleware service
builder.Services.AddRequestContextLoggingMiddleware();

var app = builder.Build();

// Add the middleware to the pipeline (early, to enrich all subsequent logs with request context)
app.UseRequestContextLoggingMiddleware();
app.MapEndpoints();

app.Run();
```
