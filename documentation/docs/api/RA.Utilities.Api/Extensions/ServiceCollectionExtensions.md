---
sidebar_position: 4
---

```powershell
Namespace: RA.Utilities.Api.Extensions
```

The `ServiceCollectionExtensions` class provides the `Add` extension methods that register every middleware shipped with this package — and its options — in the dependency injection container: the `DefaultHeadersMiddleware`, the `GlobalExceptionHandler`, the `LoggingMiddleware`, and the `ContextLoggingMiddleware`.

Each method pairs with a `Use` counterpart in [`ApplicationBuilderExtensions`](./ApplicationBuilderExtensions.md) — call the `Add` method during service configuration, then the `Use` method when building the request pipeline. The members are declared in a C# 14 extension block on `IServiceCollection`, so they are called as ordinary instance-style extension methods.

## Default Headers Middleware

The [`DefaultHeadersMiddleware`](../Middlewares/DefaultHeadersMiddleware.md) enforces required HTTP headers (by default `x-request-id`, auto-generated if missing and echoed in the response).

### `AddDefaultHeadersMiddleware()`

Registers the middleware and its dependencies in the dependency injection container.

| Parameter | Type | Description |
| --------- | ---- | ----------- |
| **configureOptions** | `Action<DefaultHeadersOptions>?` | Optional delegate to configure [`DefaultHeadersOptions`](../Options/DefaultHeadersOptions.md) (e.g., required headers, paths to ignore). |

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

## Global Exception Handling

The [`GlobalExceptionHandler`](../GlobalExceptionHandler.mdx) implements the `IExceptionHandler` interface (introduced in .NET 8).
It acts as a centralized safety net: it catches unhandled exceptions, logs them, maps them to a structured error response via `ErrorResultResponse.Result`, and writes that response to the client.
Without this extension, wiring it up requires an easy-to-forget call:

```csharp
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
```

### `AddRaExceptionHandling()`

Registers the [`GlobalExceptionHandler`](../GlobalExceptionHandler.mdx) with the dependency injection (DI) container by calling `AddExceptionHandler<GlobalExceptionHandler>()`.
It should be called during service registration, on the `IServiceCollection` returned by `builder.Services`.

## HTTP Request/Response Logging

The [`LoggingMiddleware`](../Middlewares/LoggingMiddleware.md) captures structured logs of every HTTP request and response, including headers, body content, and duration.

### `AddLoggingMiddleware()`

Registers the middleware and its dependencies in the dependency injection container.

| Parameter | Type | Description |
| --------- | ---- | ----------- |
| **configureOptions** | `Action<HttpLoggingOptions>?` | Optional delegate to configure [`HttpLoggingOptions`](../Options/HttpLoggingOptions.md) (e.g., excluded headers, paths to ignore, max body length, warning threshold). |

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

## Request Context Logging

### `AddRequestContextLoggingMiddleware()`

Registers the [`ContextLoggingMiddleware`](../Middlewares/ContextLoggingMiddleware.md) in the dependency injection container as a transient service.

```csharp
builder.Services.AddRequestContextLoggingMiddleware();
```

:::caution
Do not use `ContextLoggingMiddleware` if you are already using `LoggingMiddleware`. The `LoggingMiddleware` already enriches log entries with request-scoped context — it creates a logging scope containing the `x-request-id` correlation ID for every request it processes. Adding `ContextLoggingMiddleware` on top of it is redundant and adds unnecessary overhead.
:::

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
