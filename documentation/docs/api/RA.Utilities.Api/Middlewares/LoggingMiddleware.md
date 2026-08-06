---
title: LoggingMiddleware
sidebar_position: 2
---

```powershell
Namespace: RA.Utilities.Api.Middlewares
```

The `HttpLoggingMiddleware` is a high-performance ASP.NET Core middleware designed to provide comprehensive logging for HTTP requests and responses.
Its primary purpose is to capture and log detailed information about every incoming HTTP request and its corresponding outgoing response.
This provides deep visibility into your API's behavior, which is invaluable for debugging, monitoring, and auditing.

### Key Features

1.  **Comprehensive Logging**: Captures the full request and response cycle — method, path, query string, headers, body, status code, and total processing time.

2.  **High Performance**: Utilizes [`Microsoft.IO.RecyclableMemoryStream`](https://www.nuget.org/packages/Microsoft.IO.RecyclableMemoryStream) to pool and reuse memory buffers instead of allocating new ones for each request, minimizing garbage collection pressure and making it safe for high-throughput production environments.

3.  **Structured Logging**: Attempts to parse request and response bodies as JSON. When successful, they are logged as structured objects, enabling powerful querying and analysis in modern logging platforms (e.g., Seq, Splunk, Elasticsearch).

4.  **Configurable**: Exclude specific paths (e.g., `/swagger`, `/health`) from logging to reduce noise, and set a maximum body size to prevent excessively large payloads from overwhelming your logging system.

In essence, `HttpLoggingMiddleware` provides the detailed "flight data recorder" for your API, helping you understand exactly what happened during any given interaction.

## 🚀 Usage Guide

### Step 1: Register the middleware services in `Program.cs`

Call `AddHttpLoggingMiddleware()` in your service configuration. You can also provide options to customize its behavior, such as excluding certain paths from logging.

```csharp showLineNumbers
// Program.cs
using RA.Utilities.Api.Middlewares.Extensions;

var builder = WebApplication.CreateBuilder(args);

// highlight-start
builder.Services.AddHttpLoggingMiddleware(options =>
{
    options.PathsToIgnore.Add("/swagger");
    options.PathsToIgnore.Add("/health");
    options.MaxBodyLogLength = 8192; // 8 KB
});
// highlight-end

var app = builder.Build();
```

### Step 2: Add the middleware to the pipeline

Place `app.UseHttpLoggingMiddleware()` early in your middleware pipeline. This ensures it can capture the entire request/response cycle, including any modifications made by subsequent middlewares.

```csharp showLineNumbers
// Program.cs (continued)

var app = builder.Build();

// highlight-next-line
app.UseHttpLoggingMiddleware();

app.UseRouting();

app.MapControllers();

app.Run();
```

### Example Log Output

When a request is processed, the middleware will generate two structured log entries.

#### Request Log:
```json showLineNumbers
{
  "TraceIdentifier": "0HMA1B2C3D4E5:00000001",
  "Method": "POST",
  "Path": "/api/users",
  "RequestHeaders": { ... },
  "RequestBody": { "name": "John Doe", "email": "john.doe@example.com" }
}
```

#### Response Log:
```json showLineNumbers
{
  "TraceIdentifier": "0HMA1B2C3D4E5:00000001",
  "StatusCode": 201,
  "Duration": 15.42,
  "ResponseHeaders": { ... },
  "ResponseBody": { "id": 123, "name": "John Doe" }
}
```
