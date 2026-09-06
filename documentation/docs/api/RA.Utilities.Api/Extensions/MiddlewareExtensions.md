---
sidebar_position: 3
---

```powershell
Namespace: RA.Utilities.Api.Extensions
```

The `MiddlewareExtensions` class provides the `Add`/`Use` extension method pairs for registering and configuring every middleware shipped with this package: the `DefaultHeadersMiddleware`, the `GlobalExceptionHandler`, the `LoggingMiddleware`, and the `RequestContextLoggingMiddleware`.
Each pair follows the standard ASP.NET Core convention — `Add` registers services in DI, `Use` adds the middleware to the request pipeline.

## Default Headers Middleware

The `DefaultHeadersMiddleware` enforces required HTTP headers (by default `x-request-id`, auto-generated if missing and echoed in the response).

### `AddDefaultHeadersMiddleware()`

Registers the `DefaultHeadersMiddleware` and its dependencies in the dependency injection container.

| Parameter | Type | Description |
| --------- | ---- | ----------- |
| **services** | `IServiceCollection` | The service collection to configure. |
| **configureOptions** | `Action<DefaultHeadersOptions>?` | Optional delegate to configure `DefaultHeadersOptions` (e.g., required headers, paths to ignore). |

This method performs two registrations:
- **Configures** `DefaultHeadersOptions` if a delegate is provided.
- **Registers** `DefaultHeadersMiddleware` as a transient service.

`DefaultHeadersOptions` defaults to requiring the `x-request-id` header with auto-generation and response echoing enabled.

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

```csharp
var app = builder.Build();

app.UseDefaultHeadersMiddleware();
// Other middleware...
app.MapEndpoints();

app.Run();
```

## Global Exception Handling

The [`GlobalExceptionHandler`](../GlobalExceptionHandler.mdx) implements the `IExceptionHandler` interface (introduced in .NET 8).
It acts as a centralized safety net: it catches unhandled exceptions, logs them, maps them to a structured error response via `ErrorResultResponse.Result`, and writes that response to the client.
Without these extensions, wiring it up requires two separate, easy-to-forget calls:

```csharp
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
app.UseExceptionHandler();
```

`MiddlewareExtensions` wraps both calls in expressive, discoverable extension methods:

### `AddRaExceptionHandling()`

Registers the [`GlobalExceptionHandler`](../GlobalExceptionHandler.mdx) with the dependency injection (DI) container by calling `AddExceptionHandler<GlobalExceptionHandler>()`.
It should be called during service registration, on the `IServiceCollection` returned by `builder.Services`.

### `UseRaExceptionHandling()`

Adds the exception handler middleware to the request pipeline by calling `UseExceptionHandler()`.
It should be called early in the pipeline, before other middleware, so it can catch exceptions thrown by subsequent middleware and endpoints.

Both methods return the same instance they were called on, so additional calls can be chained.

```csharp showLineNumbers
// Program.cs

// highlight-next-line
using RA.Utilities.Api.Extensions;

WebApplicationBuilder builder =
  WebApplication.CreateBuilder(args);

// Registers the GlobalExceptionHandler with DI
// highlight-start
builder.Services
  .AddRaExceptionHandling();
// highlight-end

var app = builder.Build();

// Adds the exception handler middleware to the pipeline
// highlight-next-line
app.UseRaExceptionHandling();

app.Run();
```

That's it — one line for registration and one line for pipeline setup.
This keeps your `Program.cs` clean and guarantees that every unhandled exception is logged and converted into a consistent, structured error response.

## HTTP Request/Response Logging

The `LoggingMiddleware` captures structured logs of every HTTP request and response, including headers, body content, and duration.

### `AddLoggingMiddleware()`

Registers the `LoggingMiddleware` and its dependencies in the dependency injection container.

| Parameter | Type | Description |
| --------- | ---- | ----------- |
| **services** | `IServiceCollection` | The service collection to configure. |
| **configureOptions** | `Action<HttpLoggingOptions>?` | Optional delegate to configure `HttpLoggingOptions` (e.g., excluded headers, paths to ignore, max body length, warning threshold). |

This method performs three registrations:
- **Configures** `HttpLoggingOptions` if a delegate is provided.
- **Registers** `RecyclableMemoryStreamManager` as a singleton (using `TryAddSingleton` so an existing registration is preserved).
- **Registers** `LoggingMiddleware` as a transient `IMiddleware`.

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

```csharp
var app = builder.Build();

app.UseLoggingMiddleware();
// Other middleware...
app.MapEndpoints();

app.Run();
```

## Request Context Logging

:::caution
Do not use `RequestContextLoggingMiddleware` if you are already using `LoggingMiddleware`. The `LoggingMiddleware` already enriches log entries with request-scoped context — it creates a logging scope containing the `x-request-id` correlation ID for every request it processes. Adding `RequestContextLoggingMiddleware` on top of it is redundant and adds unnecessary overhead.
:::

### `AddRequestContextLoggingMiddleware()`

Registers the `RequestContextLoggingMiddleware` in the dependency injection container as a transient service.

```csharp
builder.Services.AddRequestContextLoggingMiddleware();
```

### `UseRequestContextLoggingMiddleware()`

Adds the `RequestContextLoggingMiddleware` to the request pipeline. Must be called after `AddRequestContextLoggingMiddleware()`.

The middleware enriches log entries with request-scoped context by creating a logging scope that includes the correlation ID (`x-request-id`) from the incoming request. This allows all log entries within the scope of a request to be correlated.

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

builder.Services.AddRaExceptionHandling();
builder.Services.AddDefaultHeadersMiddleware();
builder.Services.AddLoggingMiddleware();

var app = builder.Build();

app.UseRaExceptionHandling();
app.UseDefaultHeadersMiddleware();
app.UseLoggingMiddleware();
app.MapEndpoints();

app.Run();
```
