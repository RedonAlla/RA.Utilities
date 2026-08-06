---
title: LoggingMiddleware
sidebar_position: 2
---

```powershell
Namespace: RA.Utilities.Api.Middlewares
```

The `LoggingMiddleware` is a high-performance ASP.NET Core middleware that provides comprehensive structured logging for HTTP requests and responses. It captures the full request/response cycle — method, path, headers, body, status code, and processing duration — and logs it as structured data for easy querying in modern logging platforms.

This middleware supersedes the `HttpLoggingMiddleware` from the deprecated `RA.Utilities.Api.Middlewares` package, adding log-scope enrichment, header filtering, and slow-response warning thresholds.

### Key Features

1. **Comprehensive Logging**: Captures method, path, query string, headers, body, status code, remote address, and total processing time for every request.

2. **High Performance**: Uses [`Microsoft.IO.RecyclableMemoryStream`](https://www.nuget.org/packages/Microsoft.IO.RecyclableMemoryStream) to pool and reuse memory buffers, minimizing garbage collection pressure in high-throughput environments.

3. **Structured Logging**: Request and response bodies are parsed as JSON when possible, producing structured objects in your logs rather than opaque strings. This enables powerful querying in tools like Seq, Splunk, or Elasticsearch.

4. **Log-Scope Enrichment**: Each request's log scope is automatically enriched with the `X-Request-Id` value, so every log entry in the pipeline carries the correlation ID.

5. **Configurable**: Exclude specific paths (e.g., `/swagger`, `/health`) from logging; redact sensitive headers (e.g., `Authorization`); set a maximum body size to prevent large payloads from overwhelming your logging system; and configure a slow-response warning threshold.

## 🚀 Usage Guide

### Step 1: Register the middleware services in `Program.cs`

Call `AddLoggingMiddleware()` in your service configuration. Customize behavior with the options callback.

```csharp showLineNumbers
// Program.cs
using RA.Utilities.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

// highlight-start
builder.Services.AddLoggingMiddleware(options =>
{
    options.MaxBodyLogLength = 16384;              // 16 KB max body size
    options.WarningThresholdMilliseconds = 2000;   // Log Warning if > 2s
    options.PathsToIgnore.Add("/swagger");
    options.PathsToIgnore.Add("/health");
    options.ExcludedHeaders.Add("Authorization");  // Redact from logs
});
// highlight-end

var app = builder.Build();
```

### Step 2: Add the middleware to the pipeline

Place `app.UseLoggingMiddleware()` early in your middleware pipeline so it captures the full request/response cycle.

```csharp showLineNumbers
// Program.cs (continued)

var app = builder.Build();

// highlight-next-line
app.UseLoggingMiddleware();

app.UseRouting();

app.MapControllers();

app.Run();
```

### Log Output

When a request is processed, the middleware generates two structured log entries:

#### Request Log:
```json showLineNumbers
{
  "RequestId": "abc-123-def",
  "TraceIdentifier": "0HMA1B2C3D4E5:00000001",
  "Scheme": "https",
  "Host": "api.example.com",
  "Method": "POST",
  "Path": "/api/users",
  "QueryString": "?include=profile",
  "RemoteAddress": "192.168.1.100",
  "RequestHeaders": { "Content-Type": "application/json", "Accept": "application/json" },
  "RequestBody": { "name": "John Doe", "email": "john.doe@example.com" }
}
```

#### Response Log:
```json showLineNumbers
{
  "RequestId": "abc-123-def",
  "TraceIdentifier": "0HMA1B2C3D4E5:00000001",
  "Path": "/api/users",
  "RemoteAddress": "192.168.1.100",
  "StatusCode": 201,
  "Duration": 15.42,
  "ResponseHeaders": { "Content-Type": "application/json", "Location": "/api/users/123" },
  "ResponseBody": { "id": 123, "name": "John Doe" }
}
```

Responses that exceed `WarningThresholdMilliseconds` are logged at `LogLevel.Warning` instead of `LogLevel.Information`. Headers listed in `ExcludedHeaders` are redacted from both request and response header dictionaries. Bodies larger than `MaxBodyLogLength` are truncated with a descriptive message.
