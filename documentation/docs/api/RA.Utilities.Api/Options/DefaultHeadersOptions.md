---
sidebar_position: 1
---

```powershell
Namespace: RA.Utilities.Api.Options
```

The `DefaultHeadersOptions` class provides configuration for the [`DefaultHeadersMiddleware`](../Middlewares/DefaultHeadersMiddleware). It allows you to specify which headers are required on incoming requests and which request paths should be excluded from header enforcement.

## Properties

| Property | Type | Description |
| -------- | ---- | ----------- |
| **PathsToIgnore** | `ISet<string>` | A set of request path prefixes to ignore for header enforcement. Paths starting with any value in this set will skip validation. Comparisons are case-insensitive. |
| **RequiredHeaders** | `ICollection<RequiredHeaderDefinition>` | The headers that must be present on incoming requests. Each entry defines the header name, whether to auto-generate a value when missing, whether to echo it in the response, and an optional custom error message. Defaults to a single entry requiring `x-request-id` with auto-generation and response echoing enabled. |

## Usage

Configure `DefaultHeadersOptions` via the `AddDefaultHeadersMiddleware()` extension method (in the `RA.Utilities.Api.Extensions` namespace):

```csharp
using RA.Utilities.Api.Extensions;

builder.Services.AddDefaultHeadersMiddleware(options =>
{
    options.RequiredHeaders.Add(new RequiredHeaderDefinition("x-api-key")
    {
        ErrorMessage = "API key is required."
    });
    options.PathsToIgnore.Add("/swagger");
    options.PathsToIgnore.Add("/health");
});
```

With this configuration, requests to `/swagger`, `/swagger/index.html`, `/health`, `/health/detailed`, etc. will bypass header validation, and all other paths additionally require the `x-api-key` header.
