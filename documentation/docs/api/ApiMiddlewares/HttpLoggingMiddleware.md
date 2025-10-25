---
title: HttpLoggingMiddleware
sidebar_position: 2
---

```powershell
Namespace: RA.Utilities.Api.Middlewares
```

The `HttpLoggingMiddleware` is a high-performance ASP.NET Core middleware designed to provide comprehensive logging for HTTP requests and responses.
Its primary purpose is to capture and log detailed information about every incoming HTTP request and its corresponding outgoing response.
This provides deep visibility into your API's behavior, which is invaluable for debugging, monitoring, and auditing.

### Key Features

1.  **Detailed Capture**: Logs the full request and response cycle, including methods, paths, headers, and bodies.
2.  **High Performance**: Utilizes `Microsoft.IO.RecyclableMemoryStream` to minimize memory allocations and reduce garbage collection pressure, making it safe for high-throughput applications.
3.  **Structured Logging**: Attempts to parse request/response bodies as JSON, enabling powerful querying and analysis in modern logging platforms.
4.  **Configurability**: Allows you to exclude specific paths (e.g., `/swagger`, `/health`) from logging to reduce noise and set a maximum body size to prevent logging overly large payloads.

Here’s a breakdown of its key features and why they are important:

1. **Comprehensive Logging**: It doesn't just log a URL and a status code. It captures the full picture:
  * **Request**: Method, path, query string, headers, and the full request body.
  * **Response**: Status code, headers, the full response body, and the total processing time.

2. **High Performance by Design**: Logging request and response bodies can be memory-intensive, especially under high load.
This middleware is built for performance using `Microsoft.IO.RecyclableMemoryStream`.
Instead of allocating a new large memory block for each request body (which puts pressure on the garbage collector), it reuses memory from a shared pool. This makes it safe and efficient enough for production environments.

3. **Structured Logging**: The middleware attempts to parse the request and response bodies as JSON.
When successful, it logs them as structured objects.
This is a massive advantage when using modern logging platforms (like Seq, Splunk, or Elasticsearch), as it allows you to easily search, filter, and create dashboards based on the content of your API traffic (e.g., "show me all requests where user.id was '123'").

4. **Fail-Fast and Safe**: It's configurable.
You can exclude noisy or sensitive paths (like `/swagger` or `/health`) from being logged.
You can also set a maximum body size for logging, which prevents excessively large payloads from overwhelming your logging system.

In essence, HttpLoggingMiddleware provides the detailed "flight data recorder" for your API, helping you understand exactly what happened during any given interaction.

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

Place `app.UseMiddleware<HttpLoggingMiddleware>()` early in your middleware pipeline. This ensures it can capture the entire request/response cycle, including any modifications made by subsequent middlewares.

```csharp showLineNumbers
// Program.cs (continued)

var app = builder.Build();

// highlight-next-line
app.UseMiddleware<HttpLoggingMiddleware>();

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
  "Duration": "15.42 ms",
  "ResponseHeaders": { ... },
  "ResponseBody": { "id": 123, "name": "John Doe" }
}
```
