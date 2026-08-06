---
sidebar_position: 1
---

```powershell
Namespace: RA.Utilities.Api.Middlewares
```

The `DefaultHeadersMiddleware` enforces the presence of required HTTP headers on incoming requests, ensuring consistency and traceability across your API. Unlike the deprecated package's version (which only enforced `X-Request-Id`), this implementation allows you to define **any number of required headers** with per-header configuration.

Here's a breakdown of its primary functions:

1. **Flexible Header Enforcement**: Define multiple required headers — each with its own name, error message, and behavior — via `RequiredHeaderDefinition` items in `DefaultHeadersOptions`.
2. **Auto-Generation**: For each header, you can opt to have the middleware generate a GUID value when the header is missing, rather than rejecting the request.
3. **Response Echo**: Configure headers to be echoed back in the response, allowing clients to confirm what values were received or assigned.
4. **Standardized Error Responses**: When one or more required headers are missing (and not auto-generated), the middleware returns a `400 Bad Request` with a structured error payload listing all missing headers.
5. **Path-Based Exclusion**: Configure paths to skip (e.g., `/swagger`, `/health`) via `PathsToIgnore`.

## 🚀 Usage Guide

### Step 1: Register the middleware services in `Program.cs`

Call `AddDefaultHeadersMiddleware()` with options to define your required headers.

```csharp showLineNumbers
// Program.cs
using RA.Utilities.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

// highlight-start
builder.Services.AddDefaultHeadersMiddleware(options =>
{
    // Require X-Request-Id — auto-generate a GUID if missing
    options.RequiredHeaders.Add(new RequiredHeaderDefinition("X-Request-Id")
    {
        AutoGenerate = true,
        EchoInResponse = true,
        ErrorMessage = "X-Request-Id header is required for request tracing."
    });

    // Require X-Tenant-Id — must be provided by the caller
    options.RequiredHeaders.Add(new RequiredHeaderDefinition("X-Tenant-Id")
    {
        AutoGenerate = false,
        ErrorMessage = "X-Tenant-Id header is required for multi-tenant routing."
    });

    options.PathsToIgnore.Add("/health");
});
// highlight-end

var app = builder.Build();
```

### Step 2: Add the middleware to the pipeline

Place `app.UseDefaultHeadersMiddleware()` early in the pipeline so header validation runs before your application logic.

```csharp showLineNumbers
// Program.cs (continued)

app.UseRouting();
// highlight-next-line
app.UseDefaultHeadersMiddleware();

app.MapControllers();

app.Run();
```

### Example: Missing Required Headers

A request missing the `X-Request-Id` header receives:

```json showLineNumbers
{
  "responseCode": 400,
  "responseType": "BadRequest",
  "responseMessage": "The request is invalid.",
  "result": [
    {
      "propertyName": "X-Request-Id",
      "errorMessage": "Header 'X-Request-Id' is required.",
      "errorCode": "NotNullValidator"
    }
  ]
}
```

The response includes a `Location` header and an auto-generated `X-Request-Id` for traceability.

### Example: Auto-Generated Headers

If `X-Request-Id` is missing but `AutoGenerate = true`, the middleware silently generates a GUID and echoes it in the response — no error is returned and the request proceeds normally.
