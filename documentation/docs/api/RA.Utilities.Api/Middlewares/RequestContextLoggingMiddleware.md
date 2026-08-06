---
title: RequestContextLoggingMiddleware
sidebar_position: 3
---

```powershell
Namespace: RA.Utilities.Api.Middlewares
```

The `RequestContextLoggingMiddleware` enriches every log entry within a request's lifetime with contextual metadata, starting with the `X-Request-Id` correlation identifier. By establishing a logging scope at the very beginning of the pipeline, it ensures that all downstream log entries — from application code, other middlewares, or framework internals — automatically carry the request's identity.

This makes it trivial to correlate all log entries belonging to a single request, even across multiple services, by filtering on the `X-Request-Id` value in your logging platform.

:::caution
Do not use `RequestContextLoggingMiddleware` if you are already using [`LoggingMiddleware`](./LoggingMiddlewareExtensions.md).
The `LoggingMiddleware` already enriches log entries with request-scoped context — it creates a logging scope containing the `x-request-id` correlation ID for every request it processes.

Adding `RequestContextLoggingMiddleware` on top of it is redundant and adds unnecessary overhead.
:::

### Key Features

1. **Automatic Log Enrichment**: Wraps the entire downstream pipeline in an `ILogger.BeginScope` call, so every `ILogger` call within the request automatically includes the `X-Request-Id`.

2. **Zero Configuration**: No options class, no setup callbacks — register and use. The middleware reads the `X-Request-Id` header from the incoming request, or generates one if missing, via `CommonUtilities.GetRequestId(context)`.

3. **Plays Well with Others**: Designed to sit at the very start of the middleware pipeline so that `DefaultHeadersMiddleware` and `LoggingMiddleware` — and your own code — all share the same enriched log scope.

## 🚀 Usage Guide

### Step 1: Register the middleware services in `Program.cs`

Call `AddRequestContextLoggingMiddleware()` — no options are needed.

```csharp showLineNumbers
// Program.cs
using RA.Utilities.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

// highlight-start
builder.Services.AddRequestContextLoggingMiddleware();
// highlight-end

var app = builder.Build();
```

### Step 2: Add the middleware to the pipeline

Place `app.UseRequestContextLoggingMiddleware()` **first** in the pipeline, before any other middleware that produces log output.

```csharp showLineNumbers
// Program.cs (continued)

// highlight-next-line
app.UseRequestContextLoggingMiddleware();

app.UseDefaultHeadersMiddleware();
app.UseLoggingMiddleware();

app.MapControllers();

app.Run();
```

### How It Works

For every incoming request, the middleware:

1. Reads the `X-Request-Id` header from the request (generating a new GUID if absent)
2. Opens a logging scope with the key `XRequestId` set to that value
3. Invokes the rest of the pipeline inside that scope

Any `ILogger` call made during the request — in your controllers, services, or other middlewares — will include the `XRequestId` property:

```json showLineNumbers
{
  "@timestamp": "2026-08-07T12:34:56.789Z",
  "level": "Information",
  "message": "Processing order #42",
  "XRequestId": "abc-123-def-456"
}
```

### Combining with Other Middlewares

The recommended pipeline order places `RequestContextLoggingMiddleware` first so all subsequent middlewares inherit its scope:

```
app.UseRequestContextLoggingMiddleware();   // ← enriches log scope
app.UseDefaultHeadersMiddleware();          // ← validates headers
app.UseLoggingMiddleware();                 // ← logs request/response
```

This way, the `LoggingMiddleware`'s structured log output automatically includes the `X-Request-Id` from the scope set by `RequestContextLoggingMiddleware`.
