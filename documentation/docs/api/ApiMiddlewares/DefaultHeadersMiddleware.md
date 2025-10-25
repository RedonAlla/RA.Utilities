---
sidebar_position: 1
---

```powershell
Namespace: DefaultHeadersMiddleware
```

The `DefaultHeadersMiddleware` is a piece of ASP.NET Core middleware designed to enforce API best practices by ensuring that every incoming request includes a specific, mandatory header: `X-Request-Id`.

Here’s a breakdown of its primary functions:

1. **Enforces Traceability**: In a distributed system (like microservices), tracking a single user operation across multiple services can be difficult.
By requiring an `X-Request-Id` on every request, you establish a unique identifier that can be logged by each service.
This allows you to correlate all related logs, making it much easier to debug issues and trace the entire lifecycle of a request.

2. **Provides Immediate Feedback**: If a client sends a request without the `X-Request-Id` header, the middleware immediately stops processing and returns a standardized `HTTP 400 Bad Request` error.
This "fail-fast" approach prevents invalid requests from proceeding further into your application, saving resources and providing clear, actionable feedback to the client.

3. **Maintains Consistency**: It ensures that the `X-Request-Id` from the incoming request is passed back in the response headers.
This allows the client to match its outgoing requests with the incoming responses.

4. **Configurable Exclusion**: The middleware is flexible. You can configure it to ignore certain URL paths (like `/swagger` or `/health`), which typically don't require this level of tracing.

In short, this middleware is a simple but powerful tool for building robust, observable, and consistent APIs.

## 🚀 Usage Guide

### Step 1: Register the middleware services in `Program.cs`

Call `AddDefaultHeadersMiddleware()` in your service configuration.
You can also provide options to customize its behavior, such as excluding certain paths from header validation.

```csharp showLineNumbers
// Program.cs
using RA.Utilities.Api.Middlewares.Extensions;

var builder = WebApplication.CreateBuilder(args);

// highlight-start
builder.Services.AddDefaultHeadersMiddleware(options =>
{
    options.PathsToIgnore.Add("/swagger");
    options.PathsToIgnore.Add("/health");
});
// highlight-end

var app = builder.Build();
```

## Step 2: Add the middleware to the pipeline

Place `app.UseMiddleware<DefaultHeadersMiddleware>()` right after routing to ensure it runs for all API endpoints.

```csharp showLineNumbers
// Program.cs (continued)

app.UseRouting();
// highlight-next-line
app.UseMiddleware<DefaultHeadersMiddleware>();

app.MapControllers();

app.Run();
```

A request without the `X-Request-Id` header will receive a response like this:

```json showLineNumbers
{
  "responseCode": 400,
  "responseType": "BadRequest",
  "responseMessage": "One or more validation errors occurred.",
  "result": [
    {
      "propertyName": "X-Request-Id",
      "errorMessage": "Header 'X-Request-Id' is required.",
      "attemptedValue": null,
      "errorCode": "NotNullValidator"
    }
  ]
}
```