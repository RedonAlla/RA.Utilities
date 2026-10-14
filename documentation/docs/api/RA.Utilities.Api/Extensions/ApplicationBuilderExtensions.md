---
sidebar_position: 3
---

```powershell
Namespace: RA.Utilities.Api.Extensions
```

The `ApplicationBuilderExtensions` class provides the `Use` extension methods that register every middleware shipped with this package in the request pipeline: the `DefaultHeadersMiddleware`, the `GlobalExceptionHandler`, the `LoggingMiddleware`, and the `ContextLoggingMiddleware`.

Each method pairs with an `Add` counterpart in [`ServiceCollectionExtensions`](./ServiceCollectionExtensions.md) — call the `Add` method during service registration, then the `Use` method when building the pipeline. The members are declared in a C# 14 extension block on `IApplicationBuilder`, so they are called as ordinary instance-style extension methods and return the same builder for chaining.

## `UseDefaultHeadersMiddleware()`

Adds the [`DefaultHeadersMiddleware`](../Middlewares/DefaultHeadersMiddleware.md) to the request pipeline.
Must be called after [`AddDefaultHeadersMiddleware()`](./ServiceCollectionExtensions.md).

Place it early so header validation runs before your application logic.

```csharp showLineNumbers
var app = builder.Build();

// highlight-next-line
app.UseDefaultHeadersMiddleware();

app.MapEndpoints();

app.Run();
```

## `UseRaExceptionHandling()`

Adds the global exception handling middleware to the request pipeline by calling `app.UseExceptionHandler()`, wiring up the [`GlobalExceptionHandler`](../GlobalExceptionHandler.mdx) registered by [`AddRaExceptionHandling()`](./ServiceCollectionExtensions.md).

It should be called **early** in the pipeline, before other middleware, so it can catch exceptions thrown by subsequent middleware and endpoints.

Both `Add` and `Use` calls replace the manual ASP.NET Core setup:

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

## `UseLoggingMiddleware()`

Adds the [`LoggingMiddleware`](../Middlewares/LoggingMiddleware.md) to the request pipeline.
Must be called after [`AddLoggingMiddleware()`](./ServiceCollectionExtensions.md).

Place it early so it captures the full request/response cycle.

```csharp showLineNumbers
var app = builder.Build();

// highlight-next-line
app.UseLoggingMiddleware();

app.MapEndpoints();

app.Run();
```

## `UseContextLoggingMiddleware()`

Adds the [`ContextLoggingMiddleware`](../Middlewares/ContextLoggingMiddleware.md) to the request pipeline.
Must be called after [`AddRequestContextLoggingMiddleware()`](./ServiceCollectionExtensions.md).

Place it **first** in the pipeline, before any other middleware that produces log output, so all subsequent middlewares inherit its log scope.

```csharp showLineNumbers
var app = builder.Build();

// highlight-next-line
app.UseContextLoggingMiddleware();

app.MapEndpoints();

app.Run();
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
