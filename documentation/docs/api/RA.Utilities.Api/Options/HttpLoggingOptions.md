---
sidebar_position: 2
---

```powershell
Namespace: RA.Utilities.Api.Options
```

The `HttpLoggingOptions` class provides configuration for the [`LoggingMiddleware`](../Middlewares/LoggingMiddleware). It allows you to control which requests are logged, which headers are redacted, how much of request/response bodies is captured, and when an additional slow-call warning is written.

## Properties

| Property | Type | Default | Description |
| -------- | ---- | ------- | ----------- |
| **PathsToIgnore** | `ISet<string>` | empty | A set of request path prefixes to exclude from logging. Paths starting with any value in this set will not be logged. Comparisons are case-insensitive. |
| **MaxBodyLogLength** | `int` | `32768` (32 KB) | The maximum length of the request or response body to log in bytes. Payloads larger than this will be replaced with a placeholder message. |
| **ExcludedHeaders** | `ISet<string>` | empty | A set of header names to exclude from both request and response logging. Comparisons are case-insensitive. Defaults to an empty set (all headers are logged). |
| **WarningThresholdMilliseconds** | `double` | `0` | The response-duration threshold in milliseconds above which an additional `Warning` log entry is written for the call. Set to `0` to disable the warning threshold (default). |

## Usage

Configure `HttpLoggingOptions` via the `AddLoggingMiddleware()` extension method (in the `RA.Utilities.Api.Extensions` namespace):

```csharp
using RA.Utilities.Api.Extensions;

builder.Services.AddLoggingMiddleware(options =>
{
    options.PathsToIgnore.Add("/swagger");
    options.PathsToIgnore.Add("/health");
    options.MaxBodyLogLength = 8192;               // Truncate bodies larger than 8 KB
    options.ExcludedHeaders.Add("Authorization");  // Redact from logs
    options.WarningThresholdMilliseconds = 500;    // Extra Warning entry for responses slower than 500 ms
});
```

With this configuration:
- Requests to `/swagger`, `/health`, and their sub-paths will not be logged.
- Request and response bodies exceeding 8 KB are replaced with a truncation placeholder in the log output.
- The `Authorization` header is redacted from both request and response header dictionaries.
- Responses slower than 500 ms produce an additional `Warning` log entry alongside the response log.
