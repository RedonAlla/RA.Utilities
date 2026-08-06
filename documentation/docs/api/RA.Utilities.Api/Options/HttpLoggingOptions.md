---
sidebar_position: 2
---

```powershell
Namespace: RA.Utilities.Api.Middlewares.Options
```

The `HttpLoggingOptions` class provides configuration for the [`HttpLoggingMiddleware`](../HttpLoggingMiddleware). It allows you to control which requests are logged and how large request/response bodies are captured.

## Properties

| Property | Type | Default | Description |
| -------- | ---- | ------- | ----------- |
| **PathsToIgnore** | `ISet<string>` | empty | A set of request path prefixes to exclude from logging. Paths starting with any value in this set will not be logged. Comparisons are case-insensitive. |
| **MaxBodyLogLength** | `int` | `32768` (32 KB) | The maximum length of the request or response body to log in bytes. Payloads larger than this will be replaced with a placeholder message. |

## Usage

Configure `HttpLoggingOptions` via the `AddHttpLoggingMiddleware()` extension method:

```csharp
using RA.Utilities.Api.Middlewares.Extensions;

builder.Services.AddHttpLoggingMiddleware(options =>
{
    options.PathsToIgnore.Add("/swagger");
    options.PathsToIgnore.Add("/health");
    options.MaxBodyLogLength = 8192; // Truncate bodies larger than 8 KB
});
```

With this configuration:
- Requests to `/swagger`, `/health`, and their sub-paths will not be logged.
- Request and response bodies exceeding 8 KB are replaced with a truncation placeholder in the log output.
